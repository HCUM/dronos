//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using System.Collections;
using System;
using Valve.VR;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Valve.VR
{
    [Serializable]
    /// <summary>
    /// This is the base level action for SteamVR Input. All SteamVR_Action_In and SteamVR_Action_Out inherit from this.
    /// Initializes the ulong handle for the action and has helper references.
    /// </summary>
    public abstract class SteamVR_Action<T> : SteamVR_Action, ISteamVR_Action where T : SteamVR_Action_Data, new()
    {
        [NonSerialized]
        protected T actionData;

        /// <summary>The amount this action has to change to trigger a change event (unless the action has a built in change flag)</summary>
        public override float changeTolerance
        {
            get
            {
                if (initialized == false)
                    Initialize();

                return actionData.changeTolerance;
            }
            set
            {
                actionData.changeTolerance = value;
            }
        }

        /// <summary>The full string path for this action</summary>
        public override string fullPath
        {
            get
            {
                if (initialized == false)
                    Initialize();

                return actionData.fullPath;
            }
        }

        /// <summary>The underlying handle for this action used for native SteamVR Input calls</summary>
        public override ulong handle
        {
            get
            {
                if (initialized == false)
                    Initialize();

                return actionData.handle;
            }
        }

        /// <summary>The actionset this action is contained within</summary>
        public override SteamVR_ActionSet actionSet
        {
            get
            {
                if (initialized == false)
                    Initialize();

                return actionData.actionSet;
            }
        }

        /// <summary>The action direction of this action (in for input, out for output)</summary>
        public override SteamVR_ActionDirections direction
        {
            get
            {
                if (initialized == false)
                    Initialize();

                return actionData.direction;
            }
        }

        protected bool initialized = false;

        public override void PreInitialize(string newActionPath)
        {
            actionPath = newActionPath;

            actionData = new T();
            actionData.PreInitialize(this, actionPath);

            initialized = true;
        }

        /// <summary>
        /// Initializes the handle for the action
        /// </summary>
        public override void Initialize(bool createNew = false)
        {
            if (createNew)
            {
                actionData.Initialize();
            }
            else
            {
                actionData = SteamVR_Input.GetActionDataFromPath<T>(actionPath);

                if (actionData == null)
                {
                    Debug.LogError("<b>[SteamVR]</b> Could not find action with path: " + actionPath);
                }
            }
            
            initialized = true;
        }

        /// <summary>Gets the last timestamp this action was changed. (by Time.realtimeSinceStartup)</summary>
        /// <param name="inputSource">The input source to use to select the last changed time</param>
        public override float GetTimeLastChanged(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetTimeLastChanged(inputSource);
        }

        /// <summary>Gets the last part of the path for this action. Removes action set.</summary>
        public override string GetShortName()
        {
            if (initialized == false)
                Initialize();

            return actionData.GetShortName();
        }

        public override SteamVR_Action_Data GetActionData()
        {
            return actionData;
        }

        protected override void InitializeCopy(string newActionPath, SteamVR_Action_Data newData)
        {
            this.actionPath = newActionPath;
            this.actionData = (T)newData;

            initialized = true;
        }

        protected void CheckSourceUpdatingState(SteamVR_Input_Sources inputSource)
        {
            SteamVR_Input_Source.AddSourceToUpdateList(inputSource);
        }
    }

    [Serializable]
    public abstract class SteamVR_Action : IEquatable<SteamVR_Action>, ISteamVR_Action
    {
        public SteamVR_Action() { }

        [SerializeField]
        protected string actionPath;

        public static CreateType Create<CreateType>(string newActionPath) where CreateType : SteamVR_Action, new()
        {
            CreateType action = new CreateType();
            action.PreInitialize(newActionPath);
            return action;
        }

        public CreateType GetCopy<CreateType>() where CreateType : SteamVR_Action, new()
        {
            #if UNITY_EDITOR
            CreateType action = new CreateType();
            action.InitializeCopy(this.actionPath, this.GetActionData());
            return action;

            #else      
            return (CreateType)this; //no need to make copies in builds - will reduce memory alloc
            #endif
        }

        protected abstract void InitializeCopy(string newActionPath, SteamVR_Action_Data newData);

        /// <summary>The amount this action has to change to trigger a change event (unless the action has a built in change flag)</summary>
        public abstract float changeTolerance { get; set; }

        /// <summary>The full string path for this action</summary>
        public abstract string fullPath { get; }

        /// <summary>The underlying handle for this action used for native SteamVR Input calls</summary>
        public abstract ulong handle { get; }

        /// <summary>The actionset this action is contained within</summary>
        public abstract SteamVR_ActionSet actionSet { get; }

        /// <summary>The action direction of this action (in for input, out for output)</summary>
        public abstract SteamVR_ActionDirections direction { get; }

        public abstract void PreInitialize(string newActionPath);

        /// <summary>
        /// Initializes the handle for the action
        /// </summary>
        public abstract void Initialize(bool createNew = false);

        /// <summary>Gets the last timestamp this action was changed. (by Time.realtimeSinceStartup)</summary>
        /// <param name="inputSource">The input source to use to select the last changed time</param>
        public abstract float GetTimeLastChanged(SteamVR_Input_Sources inputSource);

        /// <summary>Gets the last part of the path for this action. Removes action set.</summary>
        public abstract string GetShortName();

        public abstract SteamVR_Action_Data GetActionData();
        

        public string GetPath()
        {
            return actionPath;
        }


        public override int GetHashCode()
        {
            if (actionPath == null)
                return 0;
            else
                return actionPath.GetHashCode();
        }

        public bool Equals(SteamVR_Action other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return this.actionPath == other.actionPath;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                if (string.IsNullOrEmpty(this.actionPath)) //if we haven't set a path, say this action is equal to null
                    return true;
                return false;
            }

            if (ReferenceEquals(this, other))
                return true;

            if (other is SteamVR_Action)
                return this.Equals((SteamVR_Action)other);

            return false;
        }

        public static bool operator !=(SteamVR_Action action1, SteamVR_Action action2)
        {
            return !(action1 == action2);
        }

        public static bool operator ==(SteamVR_Action action1, SteamVR_Action action2)
        {
            bool action1null = (ReferenceEquals(null, action1));
            bool action2null = (ReferenceEquals(null, action2));

            if (action1null && action2null)
                return true;
            else if (action1null == false && action2null == true)
            {
                if (string.IsNullOrEmpty(action1.actionPath))
                    return true; //if we haven't set a path, say this action is equal to null
                return false;
            }
            else if (action1null == true && action2null == false)
            {
                if (string.IsNullOrEmpty(action2.actionPath))
                    return true; //if we haven't set a path, say this action is equal to null
                return false;
            }

            return action1.Equals(action2);
        }
    }

    public class SteamVR_Action_Data : ISteamVR_Action
    {
        protected float _changeTolerance = 0.000001f;
        public float changeTolerance { get { return _changeTolerance; } set { _changeTolerance = value; } }

        public string fullPath { get; protected set; }
        
        public ulong handle { get; protected set; }

        public SteamVR_ActionSet actionSet { get; protected set; }

        public SteamVR_ActionDirections direction { get; protected set; }
        
        protected Dictionary<SteamVR_Input_Sources, float> lastChanged = new Dictionary<SteamVR_Input_Sources, float>(new SteamVR_Input_Sources_Comparer());

        protected SteamVR_Action wrappingAction;

        public virtual void PreInitialize(SteamVR_Action wrappingAction, string actionPath)
        {
            this.wrappingAction = wrappingAction;
            fullPath = actionPath;

            SteamVR_Input_Sources[] sources = SteamVR_Input_Source.GetAllSources();
            for (int sourceIndex = 0; sourceIndex < sources.Length; sourceIndex++)
            {
                InitializeDictionaries(sources[sourceIndex]);
            }

            actionSet = SteamVR_Input.GetActionSetFromPath(GetActionSetPath());
            if (actionSet == null)
                Debug.LogError("Could not find action set for: " + GetActionSetPath());

            direction = GetActionDirection();
        }

        /// <summary>
        /// Initializes the handle for the action
        /// </summary>
        public virtual void Initialize()
        {
            ulong newHandle = 0;
            EVRInputError err = OpenVR.Input.GetActionHandle(fullPath.ToLower(), ref newHandle);
            handle = newHandle;

            if (err != EVRInputError.None)
                Debug.LogError("<b>[SteamVR]</b> GetActionHandle (" + fullPath + ") error: " + err.ToString());
        }

        protected virtual void InitializeDictionaries(SteamVR_Input_Sources source)
        {
            lastChanged.Add(source, 0);
        } 

        public float GetTimeLastChanged(SteamVR_Input_Sources inputSource)
        {
            return lastChanged[inputSource];
        }

        [NonSerialized]
        private string cachedShortName;

        /// <summary>Gets the last part of the path for this action. Removes action set.</summary>
        public string GetShortName()
        {
            if (cachedShortName == null)
            {
                cachedShortName = SteamVR_Input_ActionFile.GetShortName(fullPath);
            }

            return cachedShortName;
        }

        private string GetActionSetPath()
        {
            int actionsEndIndex = fullPath.IndexOf('/', 1);
            int setStartIndex = actionsEndIndex + 1;
            int setEndIndex = fullPath.IndexOf('/', setStartIndex);
            int count = setEndIndex;

            return fullPath.Substring(0, count);
        }

        private SteamVR_ActionDirections GetActionDirection()
        {
            int actionsEndIndex = fullPath.IndexOf('/', 1);
            int setStartIndex = actionsEndIndex + 1;
            int setEndIndex = fullPath.IndexOf('/', setStartIndex);
            int directionEndIndex = fullPath.IndexOf('/', setEndIndex + 1);
            int count = directionEndIndex - setEndIndex - 1;//
            string direction = fullPath.Substring(setEndIndex + 1, count);

            if (direction == "in")
                return SteamVR_ActionDirections.In;
            else if (direction == "out")
                return SteamVR_ActionDirections.Out;
            else
                Debug.LogError("Could not find match for direction: " + direction);
            return SteamVR_ActionDirections.In;
        }
    }


    public interface ISteamVR_Action
    {
        /// <summary>The amount this action has to change to trigger a change event (unless the action has a built in change flag)</summary>
        float changeTolerance { get; set; }

        /// <summary>The full string path for this action</summary>
        string fullPath { get; }

        /// <summary>The underlying handle for this action used for native SteamVR Input calls</summary>
        ulong handle { get; }

        /// <summary>The actionset this action is contained within</summary>
        SteamVR_ActionSet actionSet { get; }

        /// <summary>The action direction of this action (in for input, out for output)</summary>
        SteamVR_ActionDirections direction { get; }

        /// <summary>Gets the last timestamp this action was changed. (by Time.realtimeSinceStartup)</summary>
        /// <param name="inputSource">The input source to use to select the last changed time</param>
        float GetTimeLastChanged(SteamVR_Input_Sources inputSource);

        /// <summary>Gets the last part of the path for this action. Removes action set.</summary>
        string GetShortName();
    }
}