//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using Valve.VR;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
    public partial class SteamVR_Input
    {
        public const string defaultInputGameObjectName = "[SteamVR Input]";
        private const string localizationKeyName = "localization";
        public static string actionsFilePath;

        /// <summary>True if the actions file has been initialized</summary>
        public static bool fileInitialized = false;

        /// <summary>True if the steamvr input system initialization process has completed successfully</summary>
        public static bool initialized = false;

        /// <summary>True if the preinitialization process (setting up dictionaries, etc) has completed successfully</summary>
        public static bool preInitialized = false;

        /// <summary>The serialized version of the actions file we're currently using (only used in editor)</summary>
        public static SteamVR_Input_ActionFile actionFile;

        /// <summary>The hash of the current action file on disk</summary>
        public static string actionFileHash;

        /// <summary>A reference to the method that has been generated to initialize action sets</summary>
        public static Action InitializeActionSets;

        /// <summary>A reference to the method that has been generated to initialize actions</summary>
        public static Action InitializeActions;

        /// <summary>A reference to the method that has been generated to initialize the references to the action set objects</summary>
        public static Action InitializeInstanceActionSets;

        /// <summary>A reference to the method that has been generated to initialize the references to the action objects</summary>
        public static Action InitializeInstanceActions;

        /// <summary>An event that fires when the non visual actions (everything except poses / skeletons) have been updated</summary>
        public static event Action OnNonVisualActionsUpdated;

        /// <summary>An event that fires when the pose actions have been updated</summary>
        public static event Action<bool> OnPosesUpdated;

        /// <summary>An event that fires when the skeleton actions have been updated</summary>
        public static event Action<bool> OnSkeletonsUpdated;


        protected static Type inputType = typeof(SteamVR_Input);

        protected static bool initializing = false;

        #region array accessors
        /// <summary>An array of all action sets</summary>
        public static SteamVR_ActionSet[] actionSets;

        /// <summary>An array of all actions (in all action sets)</summary>
        public static SteamVR_Action[] actions;

        /// <summary>An array of all input actions</summary>
        public static ISteamVR_Action_In[] actionsIn;

        /// <summary>An array of all output actions (haptic)</summary>
        public static ISteamVR_Action_Out[] actionsOut;

        /// <summary>An array of all the boolean actions</summary>
        public static SteamVR_Action_Boolean[] actionsBoolean;

        /// <summary>An array of all the single actions</summary>
        public static SteamVR_Action_Single[] actionsSingle;

        /// <summary>An array of all the vector2 actions</summary>
        public static SteamVR_Action_Vector2[] actionsVector2;

        /// <summary>An array of all the vector3 actions</summary>
        public static SteamVR_Action_Vector3[] actionsVector3;

        /// <summary>An array of all the pose actions</summary>
        public static SteamVR_Action_Pose[] actionsPose;

        /// <summary>An array of all the skeleton actions</summary>
        public static SteamVR_Action_Skeleton[] actionsSkeleton;

        /// <summary>An array of all the vibration (haptic) actions</summary>
        public static SteamVR_Action_Vibration[] actionsVibration;

        /// <summary>An array of all the input actions that are not pose or skeleton actions (boolean, single, vector2, vector3)</summary>
        public static ISteamVR_Action_In[] actionsNonPoseNonSkeletonIn;

        protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPath = new Dictionary<string, SteamVR_ActionSet>();
        protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPathLowered = new Dictionary<string, SteamVR_ActionSet>();
        protected static Dictionary<string, SteamVR_Action> actionsByPath = new Dictionary<string, SteamVR_Action>();
        protected static Dictionary<string, SteamVR_Action> actionsByPathLowered = new Dictionary<string, SteamVR_Action>();

        protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPathCache = new Dictionary<string, SteamVR_ActionSet>();
        protected static Dictionary<string, SteamVR_Action> actionsByPathCache = new Dictionary<string, SteamVR_Action>();

        protected static Dictionary<string, SteamVR_Action> actionsByNameCache = new Dictionary<string, SteamVR_Action>();
        protected static Dictionary<string, SteamVR_ActionSet> actionSetsByNameCache = new Dictionary<string, SteamVR_ActionSet>();
        #endregion

        /// <summary>
        /// Get all the handles for actions and action sets. 
        /// Initialize our dictionaries of action / action set names. 
        /// Setup the tracking space universe origin
        /// </summary>
        public static void Initialize()
        {
            if (initialized)
                return;

#if UNITY_EDITOR
            CheckSetup();
            if (IsOpeningSetup())
                return;
#endif

            //Debug.Log("<b>[SteamVR]</b> Initializing SteamVR input...");
            initializing = true;

            SteamVR_ActionSet_Manager.Initialize();
            SteamVR_Input_Source.Initialize();

            for (int actionIndex = 0; actionIndex < actions.Length; actionIndex++)
            {
                SteamVR_Action action = actions[actionIndex];
                action.Initialize(true);
            }

            for (int actionSetIndex = 0; actionSetIndex < actionSets.Length; actionSetIndex++)
            {
                SteamVR_ActionSet set = actionSets[actionSetIndex];
                set.Initialize(true);
            }

            if (SteamVR.settings.activateFirstActionSetOnStart)
            {
                if (actionSets.Length > 0)
                    actionSets[0].Activate();
                else
                {
                    Debug.LogError("<b>[SteamVR]</b> No action sets to activate.");
                }
            }

            SteamVR_Action_Pose.SetTrackingUniverseOrigin(SteamVR_Settings.instance.trackingSpace);

            initialized = true;
            initializing = false;
            //Debug.Log("<b>[SteamVR]</b> Input initialization complete.");
        }

        protected static void FinishPreInitActionSets()
        {
            for (int actionSetIndex = 0; actionSetIndex < actionSets.Length; actionSetIndex++)
            {
                SteamVR_ActionSet actionSet = actionSets[actionSetIndex];
                actionSet.FinishPreInitialize();
            }
        }

        protected static void InitializeActionSetDictionaries()
        {
            actionSetsByPath.Clear();
            actionSetsByPathLowered.Clear();
            actionSetsByPathCache.Clear();

            for (int actionSetIndex = 0; actionSetIndex < actionSets.Length; actionSetIndex++)
            {
                SteamVR_ActionSet actionSet = actionSets[actionSetIndex];
                actionSetsByPath.Add(actionSet.fullPath, actionSet);
                actionSetsByPathLowered.Add(actionSet.fullPath.ToLower(), actionSet);
            }
        }

        protected static void InitializeActionDictionaries()
        {
            actionsByPath.Clear();
            actionsByPathLowered.Clear();
            actionsByPathCache.Clear();

            for (int actionIndex = 0; actionIndex < actions.Length; actionIndex++)
            {
                SteamVR_Action action = actions[actionIndex];
                actionsByPath.Add(action.fullPath, action);
                actionsByPathLowered.Add(action.fullPath.ToLower(), action);
            }
        }

        /// <summary>Gets called by SteamVR_Behaviour every Update and updates actions if the steamvr settings are configured to update then.</summary>
        public static void Update()
        {
            if (initialized == false)
                return;

            if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnUpdate))
            {
                UpdateNonVisualActions();
            }
            if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnUpdate))
            {
                UpdateVisualActions();
            }
        }

        /// <summary>
        /// Gets called by SteamVR_Behaviour every LateUpdate and updates actions if the steamvr settings are configured to update then. 
        /// Also updates skeletons regardless of settings are configured to so we can account for animations on the skeletons.
        /// </summary>
        public static void LateUpdate()
        {
            if (initialized == false)
                return;

            if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnLateUpdate))
            {
                UpdateNonVisualActions();
            }

            if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnLateUpdate))
            {
                //update poses and skeleton
                UpdateVisualActions();
            }
            else
            {
                //force skeleton update so animation blending sticks
                UpdateSkeletonActions(true);
            }
        }

        /// <summary>Gets called by SteamVR_Behaviour every FixedUpdate and updates actions if the steamvr settings are configured to update then.</summary>
        public static void FixedUpdate()
        {
            if (initialized == false)
                return;

            if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnFixedUpdate))
            {
                UpdateNonVisualActions();
            }

            if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnFixedUpdate))
            {
                UpdateVisualActions();
            }
        }

        /// <summary>Gets called by SteamVR_Behaviour every OnPreCull and updates actions if the steamvr settings are configured to update then.</summary>
        public static void OnPreCull()
        {
            if (initialized == false)
                return;

            if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnPreCull))
            {
                UpdateNonVisualActions();
            }
            if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
            {
                UpdateVisualActions();
            }
        }

        /// <summary>
        /// Updates the states of all the visual actions (pose / skeleton)
        /// </summary>
        /// <param name="skipStateAndEventUpdates">Controls whether or not events are fired from this update call</param>
        public static void UpdateVisualActions(bool skipStateAndEventUpdates = false)
        {
            if (initialized == false)
                return;

            SteamVR_ActionSet_Manager.UpdateActionSetsState();

            UpdatePoseActions(skipStateAndEventUpdates);

            UpdateSkeletonActions(skipStateAndEventUpdates);
        }

        /// <summary>
        /// Updates the states of all the pose actions
        /// </summary>
        /// <param name="skipStateAndEventUpdates">Controls whether or not events are fired from this update call</param>
        public static void UpdatePoseActions(bool skipStateAndEventUpdates = false)
        {
            if (initialized == false)
                return;

            var sources = SteamVR_Input_Source.GetUpdateSources();

            for (int sourceIndex = 0; sourceIndex < sources.Length; sourceIndex++)
            {
                UpdatePoseActions(sources[sourceIndex], skipStateAndEventUpdates);
            }

            if (OnPosesUpdated != null)
                OnPosesUpdated(false);
        }

        /// <summary>
        /// Updates the states of all the pose actions for a specific input source (left hand / right hand / any)
        /// </summary>
        /// <param name="skipStateAndEventUpdates">Controls whether or not events are fired from this update call</param>
        protected static void UpdatePoseActions(SteamVR_Input_Sources inputSource, bool skipStateAndEventUpdates = false)
        {
            if (initialized == false)
                return;

            for (int actionIndex = 0; actionIndex < actionsPose.Length; actionIndex++)
            {
                SteamVR_Action_Pose action = actionsPose[actionIndex] as SteamVR_Action_Pose;

                if (action != null)
                {
                    if (action.actionSet.IsActive(inputSource))
                    {
                        action.UpdateValue(inputSource, skipStateAndEventUpdates);
                    }
                }
            }
        }


        /// <summary>
        /// Updates the states of all the skeleton actions
        /// </summary>
        /// <param name="skipStateAndEventUpdates">Controls whether or not events are fired from this update call</param>
        public static void UpdateSkeletonActions(bool skipStateAndEventUpdates = false)
        {
            if (initialized == false)
                return;

            var sources = SteamVR_Input_Source.GetUpdateSources();

            for (int sourceIndex = 0; sourceIndex < sources.Length; sourceIndex++)
            {
                UpdateSkeletonActions(sources[sourceIndex], skipStateAndEventUpdates);
            }

            if (OnSkeletonsUpdated != null)
                OnSkeletonsUpdated(skipStateAndEventUpdates);
        }


        /// <summary>
        /// Updates the states of all the skeleton actions for a specific input source (left hand / right hand / any)
        /// </summary>
        /// <param name="skipStateAndEventUpdates">Controls whether or not events are fired from this update call</param>
        protected static void UpdateSkeletonActions(SteamVR_Input_Sources inputSource, bool skipStateAndEventUpdates = false)
        {
            if (initialized == false)
                return;

            for (int actionIndex = 0; actionIndex < actionsSkeleton.Length; actionIndex++)
            {
                SteamVR_Action_Skeleton action = actionsSkeleton[actionIndex];

                if (action != null)
                {
                    if (action.actionSet.IsActive(inputSource))
                    {
                        action.UpdateValue(inputSource, skipStateAndEventUpdates);
                    }
                }
            }
        }


        /// <summary>
        /// Updates the states of all the non visual actions (boolean, single, vector2, vector3)
        /// </summary>
        public static void UpdateNonVisualActions()
        {
            if (initialized == false)
                return;

            SteamVR_ActionSet_Manager.UpdateActionSetsState();

            var sources = SteamVR_Input_Source.GetUpdateSources();

            for (int sourceIndex = 0; sourceIndex < sources.Length; sourceIndex++)
            {
                UpdateNonVisualActions(sources[sourceIndex]);
            }

            if (OnNonVisualActionsUpdated != null)
                OnNonVisualActionsUpdated();
        }

        /// <summary>
        /// Updates the states of all the non visual actions (boolean, single, vector2, vector3) for a given input source (left hand / right hand / any)
        /// </summary>
        /// <param name="inputSource"></param>
        public static void UpdateNonVisualActions(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                return;

            for (int actionSetIndex = 0; actionSetIndex < actionSets.Length; actionSetIndex++)
            {
                SteamVR_ActionSet set = actionSets[actionSetIndex];

                if (set.IsActive(inputSource))
                {
                    for (int actionIndex = 0; actionIndex < set.nonVisualInActions.Length; actionIndex++)
                    {
                        ISteamVR_Action_In actionIn = set.nonVisualInActions[actionIndex] as ISteamVR_Action_In;

                        if (actionIn != null)
                        {
                            actionIn.UpdateValue(inputSource);
                        }
                    }
                }
            }
        }


        #region String accessor helpers

        #region action accessors
        /// <summary>
        /// Get an action's action data by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static T GetActionDataFromPath<T>(string path, bool caseSensitive = false) where T : SteamVR_Action_Data
        {
            SteamVR_Action action = GetActionFromPath(path, caseSensitive);
            if (action != null)
            {
                T actionData = (T)action.GetActionData();
                return actionData;
            }

            return null;
        }

        /// <summary>
        /// Get an action set's data by the full path to that action. Action set paths are in the format /actions/[actionSet]
        /// </summary>
        /// <param name="path">The full path to the action you want (Action set paths are in the format /actions/[actionSet])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_ActionSet_Data GetActionSetDataFromPath(string path, bool caseSensitive = false)
        {
            SteamVR_ActionSet actionSet = GetActionSetFromPath(path, caseSensitive);
            if (actionSet != null)
            {
                return actionSet.GetActionSetData();
            }

            return null;
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static T GetActionFromPath<T>(string path, bool caseSensitive = false) where T : SteamVR_Action, new()
        {
            SteamVR_Action foundAction = GetActionFromPath(path, caseSensitive);
            if (foundAction != null)
                return foundAction.GetCopy<T>();

            return null;
        }

        // non-copy version
        protected static SteamVR_Action GetActionFromPath(string path, bool caseSensitive = false)
        {
            if (caseSensitive)
            {
                if (actionsByPath.ContainsKey(path))
                {
                    return (SteamVR_Action)actionsByPath[path];
                }
            }
            else
            {
                if (actionsByPathCache.ContainsKey(path))
                {
                    return (SteamVR_Action)actionsByPathCache[path];
                }
                else if (actionsByPath.ContainsKey(path))
                {
                    actionsByPathCache.Add(path, actionsByPath[path]);
                    return (SteamVR_Action)actionsByPath[path];
                }
                else
                {
                    string loweredPath = path.ToLower();
                    if (actionsByPathLowered.ContainsKey(loweredPath))
                    {
                        actionsByPathCache.Add(path, actionsByPathLowered[loweredPath]);
                        return (SteamVR_Action)actionsByPath[loweredPath];
                    }
                    else
                    {
                        actionsByPathCache.Add(path, null);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Boolean GetBooleanActionFromPath(string path, bool caseSensitive = false)
        {
            return GetActionFromPath<SteamVR_Action_Boolean>(path, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Single GetSingleActionFromPath(string path, bool caseSensitive = false)
        {
            return GetActionFromPath<SteamVR_Action_Single>(path, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vector2 GetVector2ActionFromPath(string path, bool caseSensitive = false)
        {
            return GetActionFromPath<SteamVR_Action_Vector2>(path, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vector3 GetVector3ActionFromPath(string path, bool caseSensitive = false)
        {
            return GetActionFromPath<SteamVR_Action_Vector3>(path, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vibration GetVibrationActionFromPath(string path, bool caseSensitive = false)
        {
            return GetActionFromPath<SteamVR_Action_Vibration>(path, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Pose GetPoseActionFromPath(string path, bool caseSensitive = false)
        {
            return GetActionFromPath<SteamVR_Action_Pose>(path, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Skeleton GetSkeletonActionFromPath(string path, bool caseSensitive = false)
        {
            return GetActionFromPath<SteamVR_Action_Skeleton>(path, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static T GetAction<T>(string actionSetName, string actionName, bool caseSensitive = false) where T : SteamVR_Action, new()
        {
            if (actions == null)
                return null;

            if (string.IsNullOrEmpty(actionSetName))
            {
                for (int actionIndex = 0; actionIndex < actions.Length; actionIndex++)
                {
                    if (caseSensitive)
                    {
                        if (actions[actionIndex].GetShortName() == actionName)
                            return actions[actionIndex].GetCopy<T>();
                    }
                    else
                    {
                        if (string.Equals(actions[actionIndex].GetShortName(), actionName, StringComparison.CurrentCultureIgnoreCase))
                            return actions[actionIndex].GetCopy<T>();
                    }
                }
            }
            else
            {
                SteamVR_ActionSet actionSet = GetActionSet(actionSetName, caseSensitive); 

                if (actionSet != null)
                {
                    for (int actionIndex = 0; actionIndex < actionSet.allActions.Length; actionIndex++)
                    {
                        if (caseSensitive)
                        {
                            if (actionSet.allActions[actionIndex].GetShortName() == actionName)
                                return actionSet.allActions[actionIndex].GetCopy<T>();
                        }
                        else
                        {
                            if (string.Equals(actionSet.allActions[actionIndex].GetShortName(), actionName, StringComparison.CurrentCultureIgnoreCase))
                                return actionSet.allActions[actionIndex].GetCopy<T>();
                        }
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static T GetAction<T>(string actionName, bool caseSensitive = false) where T : SteamVR_Action, new()
        {
            return GetAction<T>(null, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Boolean GetBooleanAction(string actionSetName, string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Boolean>(actionSetName, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Boolean GetBooleanAction(string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Boolean>(null, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Single GetSingleAction(string actionSetName, string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Single>(actionSetName, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Single GetSingleAction(string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Single>(null, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vector2 GetVector2Action(string actionSetName, string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Vector2>(actionSetName, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vector2 GetVector2Action(string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Vector2>(null, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vector3 GetVector3Action(string actionSetName, string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Vector3>(actionSetName, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vector3 GetVector3Action(string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Vector3>(null, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Pose GetPoseAction(string actionSetName, string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Pose>(actionSetName, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Pose GetPoseAction(string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Pose>(null, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Skeleton GetSkeletonAction(string actionSetName, string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Skeleton>(actionSetName, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Skeleton GetSkeletonAction(string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Skeleton>(null, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vibration GetVibrationAction(string actionSetName, string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Vibration>(actionSetName, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action by the full path to that action. Action paths are in the format /actions/[actionSet]/[direction]/[actionName]
        /// </summary>
        /// <typeparam name="T">The type of action you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action you want (Action paths are in the format /actions/[actionSet]/[direction]/[actionName])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_Action_Vibration GetVibrationAction(string actionName, bool caseSensitive = false)
        {
            return GetAction<SteamVR_Action_Vibration>(null, actionName, caseSensitive);
        }

        /// <summary>
        /// Get an action set by the full path to that action set. Action set paths are in the format /actions/[actionSet]
        /// </summary>
        /// <typeparam name="T">The type of action set you're expecting to get back</typeparam>
        /// <param name="actionSetName">The name to the action set you want</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static T GetActionSet<T>(string actionSetName, bool caseSensitive = false) where T : SteamVR_ActionSet, new()
        {
            if (actionSets == null)
                return null;

            for (int actionSetIndex = 0; actionSetIndex < actionSets.Length; actionSetIndex++)
            {
                if (caseSensitive)
                {
                    if (actionSets[actionSetIndex].GetShortName() == actionSetName)
                        return actionSets[actionSetIndex].GetCopy<T>();
                }
                else
                {
                    if (string.Equals(actionSets[actionSetIndex].GetShortName(), actionSetName, StringComparison.CurrentCultureIgnoreCase))
                        return actionSets[actionSetIndex].GetCopy<T>();
                }
            }

            return null;
        }

        /// <summary>
        /// Get an action set by the full path to that action set. Action set paths are in the format /actions/[actionSet]
        /// </summary>
        /// <typeparam name="T">The type of action set you're expecting to get back</typeparam>
        /// <param name="actionSetName">The name to the action set you want</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_ActionSet GetActionSet(string actionSetName, bool caseSensitive = false)
        {
            return GetActionSet<SteamVR_ActionSet>(actionSetName, caseSensitive);
        }

        /// <summary>
        /// Get an action set by the full path to that action set. Action set paths are in the format /actions/[actionSet]
        /// </summary>
        /// <typeparam name="T">The type of action set you're expecting to get back</typeparam>
        /// <param name="path">The full path to the action set you want (Action paths are in the format /actions/[actionSet])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static T GetActionSetFromPath<T>(string path, bool caseSensitive = false) where T : SteamVR_ActionSet, new()
        {
            if (caseSensitive)
            {
                if (actionSetsByPath.ContainsKey(path))
                {
                    return actionSetsByPath[path].GetCopy<T>();
                }
            }
            else
            {
                if (actionSetsByPathCache.ContainsKey(path))
                {
                    return actionSetsByPathCache[path].GetCopy<T>();
                }
                else if (actionSetsByPath.ContainsKey(path))
                {
                    actionSetsByPathCache.Add(path, actionSetsByPath[path]);
                    return actionSetsByPath[path].GetCopy<T>();
                }
                else
                {
                    string loweredPath = path.ToLower();
                    if (actionSetsByPathLowered.ContainsKey(loweredPath))
                    {
                        actionSetsByPathCache.Add(path, actionSetsByPathLowered[loweredPath]);
                        return actionSetsByPath[loweredPath].GetCopy<T>();
                    }
                    else
                    {
                        actionSetsByPathCache.Add(path, null);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get an action set by the full path to that action set. Action set paths are in the format /actions/[actionSet]
        /// </summary>
        /// <param name="path">The full path to the action set you want (Action paths are in the format /actions/[actionSet])</param>
        /// <param name="caseSensitive">case sensitive searches are faster</param>
        public static SteamVR_ActionSet GetActionSetFromPath(string path, bool caseSensitive = false)
        {
            return GetActionSetFromPath<SteamVR_ActionSet>(path, caseSensitive);
        }
        #endregion

        #region digital string accessors
        /// <summary>
        /// Get the state of an action by the action set name, action name, and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="actionSet">The name of the action set the action is contained in</param>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        public static bool GetState(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            SteamVR_Action_Boolean booleanAction = GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
            if (booleanAction != null)
            {
                return booleanAction.GetState(inputSource);
            }

            return false;
        }

        /// <summary>
        /// Get the state of an action by the action name and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        public static bool GetState(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            return GetState(null, action, inputSource, caseSensitive);
        }

        /// <summary>
        /// Get the state down of an action by the action set name, action name, and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="actionSet">The name of the action set the action is contained in</param>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        /// <returns>True when the action was false last update and is now true. Returns false again afterwards.</returns>
        public static bool GetStateDown(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            SteamVR_Action_Boolean booleanAction = GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
            if (booleanAction != null)
            {
                return booleanAction.GetStateDown(inputSource);
            }

            return false;
        }

        /// <summary>
        /// Get the state down of an action by the action name and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        /// <returns>True when the action was false last update and is now true. Returns false again afterwards.</returns>
        public static bool GetStateDown(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            return GetStateDown(null, action, inputSource, caseSensitive);
        }

        /// <summary>
        /// Get the state up of an action by the action set name, action name, and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="actionSet">The name of the action set the action is contained in</param>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        /// <returns>True when the action was true last update and is now false. Returns false again afterwards.</returns>
        public static bool GetStateUp(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            SteamVR_Action_Boolean booleanAction = GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive);
            if (booleanAction != null)
            {
                return booleanAction.GetStateUp(inputSource);
            }

            return false;
        }


        /// <summary>
        /// Get the state up of an action by the action name and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        /// <returns>True when the action was true last update and is now false. Returns false again afterwards.</returns>
        public static bool GetStateUp(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            return GetStateDown(null, action, inputSource, caseSensitive);
        }
        #endregion

        #region analog string accessors
        /// <summary>
        /// Get the float value of an action by the action set name, action name, and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="actionSet">The name of the action set the action is contained in</param>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        public static float GetFloat(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            SteamVR_Action_Single singleAction = GetAction<SteamVR_Action_Single>(actionSet, action, caseSensitive);
            if (singleAction != null)
            {
                return singleAction.GetAxis(inputSource);
            }

            return 0;
        }

        /// <summary>
        /// Get the float value of an action by the action name and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        public static float GetFloat(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            return GetFloat(null, action, inputSource, caseSensitive);
        }

        /// <summary>
        /// Get the Vector2 value of an action by the action set name, action name, and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="actionSet">The name of the action set the action is contained in</param>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        public static Vector2 GetVector2(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            SteamVR_Action_Vector2 vectorAction = GetAction<SteamVR_Action_Vector2>(actionSet, action, caseSensitive);
            if (vectorAction != null)
            {
                return vectorAction.GetAxis(inputSource);
            }

            return Vector2.zero;
        }

        /// <summary>
        /// Get the Vector2 value of an action by the action name and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        public static Vector2 GetVector2(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            return GetVector2(null, action, inputSource, caseSensitive);
        }

        /// <summary>
        /// Get the Vector3 value of an action by the action set name, action name, and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="actionSet">The name of the action set the action is contained in</param>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        public static Vector3 GetVector3(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            SteamVR_Action_Vector3 vectorAction = GetAction<SteamVR_Action_Vector3>(actionSet, action, caseSensitive);
            if (vectorAction != null)
            {
                return vectorAction.GetAxis(inputSource);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Get the Vector3 value of an action by the action name and input source. Optionally case sensitive (for faster results)
        /// </summary>
        /// <param name="action">The name of the action to get the state of</param>
        /// <param name="inputSource">The input source to get the action state from</param>
        /// <param name="caseSensitive">Whether or not the action set and action name searches should be case sensitive (case sensitive searches are faster)</param>
        public static Vector3 GetVector3(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
        {
            return GetVector3(null, action, inputSource, caseSensitive);
        }
        #endregion

        #endregion

        /// <summary>
        /// Returns all of the action sets. If we're in the editor, doesn't rely on the actionSets field being filled.
        /// </summary>
        public static SteamVR_ActionSet[] GetActionSets()
        {
            return actionSets;
        }

        /// <summary>
        /// Returns all of the actions of the specified type. If we're in the editor, doesn't rely on the arrays being filled.
        /// </summary>
        /// <typeparam name="T">The type of actions you want to get</typeparam>
        public static T[] GetActions<T>() where T : SteamVR_Action
        {
            Type type = typeof(T);

            if (type == typeof(SteamVR_Action))
            {
                return actions as T[];
            }
            else if (type == typeof(ISteamVR_Action_In))
            {
                return actionsIn as T[];
            }
            else if (type == typeof(ISteamVR_Action_Out))
            {
                return actionsOut as T[];
            }
            else if (type == typeof(SteamVR_Action_Boolean))
            {
                return actionsBoolean as T[];
            }
            else if (type == typeof(SteamVR_Action_Single))
            {
                return actionsSingle as T[];
            }
            else if (type == typeof(SteamVR_Action_Vector2))
            {
                return actionsVector2 as T[];
            }
            else if (type == typeof(SteamVR_Action_Vector3))
            {
                return actionsVector3 as T[];
            }
            else if (type == typeof(SteamVR_Action_Pose))
            {
                return actionsPose as T[];
            }
            else if (type == typeof(SteamVR_Action_Skeleton))
            {
                return actionsSkeleton as T[];
            }
            else if (type == typeof(SteamVR_Action_Vibration))
            {
                return actionsVibration as T[];
            }
            else
            {
                Debug.Log("<b>[SteamVR]</b> Wrong type.");
            }

            return null;
        }


        /// <summary>Tell SteamVR that we're using the actions file at the path defined in SteamVR_Settings.</summary>
        public static void IdentifyActionsFile()
        {
            string currentPath = Application.dataPath;
            int lastIndex = currentPath.LastIndexOf('/');
            currentPath = currentPath.Remove(lastIndex, currentPath.Length - lastIndex);

            string fullPath = System.IO.Path.Combine(currentPath, SteamVR_Settings.instance.actionsFilePath);
            fullPath = fullPath.Replace("\\", "/");

            if (File.Exists(fullPath))
            {
                var err = OpenVR.Input.SetActionManifestPath(fullPath);
                if (err != EVRInputError.None)
                    Debug.LogError("<b>[SteamVR]</b> Error loading action manifest into SteamVR: " + err.ToString());
                else
                    Debug.Log(string.Format("<b>[SteamVR]</b> Successfully loaded action manifest into SteamVR ({0})", fullPath));
            }
            else
            {
                Debug.LogError("<b>[SteamVR]</b> Could not find actions file at: " + fullPath);
            }
        }

        protected static Delegate GetMethod<T>(string methodName)
        {
            MethodInfo methodInfo = inputType.GetMethod(methodName);

            return Delegate.CreateDelegate(typeof(T), methodInfo);
        }

        /// <summary>
        /// Does the actions file in memory differ from the one on disk as determined by a md5 hash
        /// </summary>
        public static bool HasFileInMemoryBeenModified()
        {
            string projectPath = Application.dataPath;
            int lastIndex = projectPath.LastIndexOf("/");
            projectPath = projectPath.Remove(lastIndex, projectPath.Length - lastIndex);
            actionsFilePath = Path.Combine(projectPath, SteamVR_Settings.instance.actionsFilePath);

            string jsonText = null;

            if (File.Exists(actionsFilePath))
            {
                jsonText = System.IO.File.ReadAllText(actionsFilePath);
            }
            else
            {
                return true;
            }

            string newHashFromFile = SteamVR_Utils.GetBadMD5Hash(jsonText);

            string newJSON = JsonConvert.SerializeObject(SteamVR_Input.actionFile, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string newHashFromMemory = SteamVR_Utils.GetBadMD5Hash(newJSON);

            return newHashFromFile != newHashFromMemory;
        }

        public static bool CreateEmptyActionsFile(bool completelyEmpty = false)
        {
            string projectPath = Application.dataPath;
            int lastIndex = projectPath.LastIndexOf("/");
            projectPath = projectPath.Remove(lastIndex, projectPath.Length - lastIndex);
            actionsFilePath = Path.Combine(projectPath, SteamVR_Settings.instance.actionsFilePath);

            if (File.Exists(actionsFilePath))
            {
                Debug.LogErrorFormat("<b>[SteamVR]</b> Actions file already exists in project root: {0}", actionsFilePath);
                return false;
            }

            actionFile = new SteamVR_Input_ActionFile();

            if (completelyEmpty == false)
            {
                actionFile.action_sets.Add(SteamVR_Input_ActionFile_ActionSet.CreateNew());
                actionFile.actions.Add(SteamVR_Input_ActionFile_Action.CreateNew(actionFile.action_sets[0].shortName, 
                    SteamVR_ActionDirections.In, SteamVR_Input_ActionFile_ActionTypes.boolean));
            }

            string newJSON = JsonConvert.SerializeObject(actionFile, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            File.WriteAllText(actionsFilePath, newJSON);
            
            actionFile.InitializeHelperLists();
            fileInitialized = true;
            return true;
        }

        /// <summary>
        /// Load from disk and deserialize the actions file
        /// </summary>
        /// <param name="force">Force a refresh of this file from disk</param>
        public static bool InitializeFile(bool force = false, bool showErrors = true)
        {
            string projectPath = Application.dataPath;
            int lastIndex = projectPath.LastIndexOf("/");
            projectPath = projectPath.Remove(lastIndex, projectPath.Length - lastIndex);
            actionsFilePath = Path.Combine(projectPath, SteamVR_Settings.instance.actionsFilePath);

            string jsonText = null;

            if (File.Exists(actionsFilePath))
            {
                jsonText = System.IO.File.ReadAllText(actionsFilePath);
            }
            else
            {
                if (showErrors)
                    Debug.LogErrorFormat("<b>[SteamVR]</b> Actions file does not exist in project root: {0}", actionsFilePath);

                return false;
            }

            if (fileInitialized == true || (fileInitialized == true && force == false))
            {
                string newHash = SteamVR_Utils.GetBadMD5Hash(jsonText);

                if (newHash == actionFileHash)
                {
                    return true;
                }

                actionFileHash = newHash;
            }

            actionFile = Valve.Newtonsoft.Json.JsonConvert.DeserializeObject<SteamVR_Input_ActionFile>(jsonText);
            actionFile.InitializeHelperLists();
            fileInitialized = true;
            return true;
        }

#if UNITY_EDITOR
        public static string GetResourcesFolderPath(bool fromAssetsDirectory = false)
        {
            string inputFolder = string.Format("Assets/{0}", SteamVR_Settings.instance.steamVRInputPath);

            string path = Path.Combine(inputFolder, "Resources");

            bool createdDirectory = false;
            if (Directory.Exists(inputFolder) == false)
            {
                Directory.CreateDirectory(inputFolder);
                createdDirectory = true;
            }


            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
                createdDirectory = true;
            }

            if (createdDirectory)
                UnityEditor.AssetDatabase.Refresh();

            if (fromAssetsDirectory == false)
                return path.Replace("Assets/", "");
            else
                return path;
        }



        private static bool checkingSetup = false;
        private static bool openingSetup = false;
        public static bool IsOpeningSetup() { return openingSetup; }
        private static void CheckSetup()
        {
            if (checkingSetup == false && openingSetup == false && (SteamVR_Input.actions == null || SteamVR_Input.actions.Length == 0))
            {
                checkingSetup = true;
                Debug.Break();

                bool open = UnityEditor.EditorUtility.DisplayDialog("[SteamVR]", "It looks like you haven't generated actions for SteamVR Input yet. Would you like to open the SteamVR Input window?", "Yes", "No");
                if (open)
                {
                    openingSetup = true;
                    UnityEditor.EditorApplication.isPlaying = false;
                    Type editorWindowType = FindType("Valve.VR.SteamVR_Input_EditorWindow");
                    if (editorWindowType != null)
                    {
                        var window = UnityEditor.EditorWindow.GetWindow(editorWindowType, false, "SteamVR Input", true);
                        if (window != null)
                            window.Show();
                    }
                }
                else
                {
                    Debug.LogError("<b>[SteamVR]</b> This version of SteamVR will not work if you do not create and generate actions. Please open the SteamVR Input window or downgrade to version 1.2.3 (on github)");
                }
                checkingSetup = false;
            }
        }

        private static Type FindType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
#endif
    }
}