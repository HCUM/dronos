//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using System.Collections;
using System;

using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Valve.VR
{
    [Serializable]
    /// <summary>
    /// Skeleton Actions are our best approximation of where your hands are while holding vr controllers and pressing buttons. We give you 31 bones to help you animate hand models.
    /// For more information check out this blog post: https://steamcommunity.com/games/250820/announcements/detail/1690421280625220068
    /// </summary>
    public class SteamVR_Action_Skeleton : SteamVR_Action_Pose_Base<SteamVR_Action_Skeleton_Data>, ISteamVR_Action_Skeleton
    {
        public SteamVR_Action_Skeleton() { }

        public const int numBones = 31;
        

        public override void UpdateValue(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            actionData.UpdateValue(inputSource);
        }

        public override void UpdateValue(SteamVR_Input_Sources inputSource, bool skipStateAndEventUpdates)
        {
            if (initialized == false)
                Initialize();

            actionData.UpdateValue(inputSource, skipStateAndEventUpdates);
        }

        /// <summary>
        /// Gets the bone positions in local space
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3[] GetBonePositions(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetBonePositions(inputSource);
        }

        /// <summary>
        /// Gets the bone rotations in local space
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Quaternion[] GetBoneRotations(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetBoneRotations(inputSource);
        }

        /// <summary>
        /// Gets the bone positions in local space from the previous update
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3[] GetLastBonePositions(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastBonePositions(inputSource);
        }

        /// <summary>
        /// Gets the bone rotations in local space from the previous update
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Quaternion[] GetLastBoneRotations(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastBoneRotations(inputSource);
        }

        /// <summary>
        /// Set the range of the motion of the bones in this skeleton. Options are "With Controller" as if your hand is holding your VR controller. 
        /// Or "Without Controller" as if your hand is empty.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void SetRangeOfMotion(SteamVR_Input_Sources inputSource, EVRSkeletalMotionRange range)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.SetRangeOfMotion(inputSource, range);
        }

        /// <summary>
        /// Sets the space that you'll get bone data back in. Options are relative to the Model, relative to the Parent bone, and Additive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        /// <param name="space">the space that you'll get bone data back in. Options are relative to the Model, relative to the Parent bone, and Additive.</param>
        public void SetSkeletalTransformSpace(SteamVR_Input_Sources inputSource, EVRSkeletalTransformSpace space)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.SetSkeletalTransformSpace(inputSource, space);
        }


        /// <summary>Executes a function when this action's bound state changes</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources, bool> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnActiveChangeListener(inputSource, functionToCall);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources, bool> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnActiveChangeListener(inputSource, functionToStopCalling);
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnChangeListener(inputSource, functionToCall);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnChangeListener(inputSource, functionToStopCalling);
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnUpdateListener(inputSource, functionToCall);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnUpdateListener(inputSource, functionToStopCalling);
        }
    }
    
    /// <summary>
     /// Skeleton Actions are our best approximation of where your hands are while holding vr controllers and pressing buttons. We give you 31 bones to help you animate hand models.
     /// For more information check out this blog post: https://steamcommunity.com/games/250820/announcements/detail/1690421280625220068
     /// </summary>
    public class SteamVR_Action_Skeleton_Data : SteamVR_Action_Pose_Data, ISteamVR_Action_Skeleton
    {
        public SteamVR_Action_Skeleton_Data() { }

        protected List<Vector3[]> bonePositions = new List<Vector3[]>();
        
        protected List<Quaternion[]> boneRotations = new List<Quaternion[]>();
        
        protected List<Vector3[]> lastBonePositions = new List<Vector3[]>();
        
        protected List<Quaternion[]> lastBoneRotations = new List<Quaternion[]>();
        
        protected Dictionary<SteamVR_Input_Sources, EVRSkeletalMotionRange> rangeOfMotion = new Dictionary<SteamVR_Input_Sources, EVRSkeletalMotionRange>(new SteamVR_Input_Sources_Comparer());
        
        protected VRBoneTransform_t[] tempBoneTransforms = new VRBoneTransform_t[SteamVR_Action_Skeleton.numBones];
        
        protected InputSkeletalActionData_t tempSkeletonActionData = new InputSkeletalActionData_t();
        
        protected uint skeletonActionData_size = 0;
        
        protected Dictionary<SteamVR_Input_Sources, EVRSkeletalTransformSpace> skeletalTransformSpace = new Dictionary<SteamVR_Input_Sources, EVRSkeletalTransformSpace>(new SteamVR_Input_Sources_Comparer());



        protected new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources, bool>> onActiveChange =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources, bool>>(new SteamVR_Input_Sources_Comparer());

        protected new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources>> onChange =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources>>(new SteamVR_Input_Sources_Comparer());

        protected new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources>> onUpdate =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources>>(new SteamVR_Input_Sources_Comparer());


        protected SteamVR_Action_Skeleton skeletonAction;

        public override void Initialize()
        {
            base.Initialize();
            skeletonActionData_size = (uint)Marshal.SizeOf(tempSkeletonActionData);

            skeletonAction = (SteamVR_Action_Skeleton)wrappingAction;
        }

        protected override void InitializeDictionaries(SteamVR_Input_Sources source)
        {
            base.InitializeDictionaries(source);

            bonePositions.Add(new Vector3[SteamVR_Action_Skeleton.numBones]);
            boneRotations.Add(new Quaternion[SteamVR_Action_Skeleton.numBones]);
            lastBonePositions.Add(new Vector3[SteamVR_Action_Skeleton.numBones]);
            lastBoneRotations.Add(new Quaternion[SteamVR_Action_Skeleton.numBones]);
            rangeOfMotion.Add(source, EVRSkeletalMotionRange.WithController);
            skeletalTransformSpace.Add(source, EVRSkeletalTransformSpace.Parent);

            onActiveChange.Add(source, null);
            onChange.Add(source, null);
            onUpdate.Add(source, null);
        }

        public override void UpdateValue(SteamVR_Input_Sources inputSource)
        {
            UpdateValue(inputSource, false);
        }

        public override void UpdateValue(SteamVR_Input_Sources inputSource, bool skipStateAndEventUpdates)
        {
            if (skipStateAndEventUpdates == false)
                base.ResetLastStates(inputSource);

            base.UpdateValue(inputSource, true);
            bool poseChanged = base.changed[inputSource];

            int inputSourceInt = (int)inputSource;

            if (skipStateAndEventUpdates == false)
            {
                changed[inputSource] = false;

                for (int boneIndex = 0; boneIndex < SteamVR_Action_Skeleton.numBones; boneIndex++)
                {
                    lastBonePositions[inputSourceInt][boneIndex] = bonePositions[inputSourceInt][boneIndex];
                    lastBoneRotations[inputSourceInt][boneIndex] = boneRotations[inputSourceInt][boneIndex];
                }
            }

            EVRInputError err = OpenVR.Input.GetSkeletalActionData(handle, ref tempSkeletonActionData, skeletonActionData_size);//, SteamVR_Input_Source.GetHandle(inputSource));
            if (err != EVRInputError.None)
            {
                Debug.LogError("<b>[SteamVR]</b> GetSkeletalActionData error (" + fullPath + "): " + err.ToString() + " handle: " + handle.ToString());
                active[inputSource] = false;
                return;
            }

            active[inputSource] = active[inputSource] && tempSkeletonActionData.bActive; //anding from the pose active state
            activeOrigin[inputSource] = tempSkeletonActionData.activeOrigin;

            if (active[inputSource])
            {
                err = OpenVR.Input.GetSkeletalBoneData(handle, skeletalTransformSpace[inputSource], rangeOfMotion[inputSource], tempBoneTransforms);//, SteamVR_Input_Source.GetHandle(inputSource));
                if (err != EVRInputError.None)
                    Debug.LogError("<b>[SteamVR]</b> GetSkeletalBoneData error (" + fullPath + "): " + err.ToString() + " handle: " + handle.ToString());

                for (int boneIndex = 0; boneIndex < tempBoneTransforms.Length; boneIndex++)
                {
                    // SteamVR's coordinate system is right handed, and Unity's is left handed.  The FBX data has its
                    // X axis flipped when Unity imports it, so here we need to flip the X axis as well
                    bonePositions[inputSourceInt][boneIndex].x = -tempBoneTransforms[boneIndex].position.v0;
                    bonePositions[inputSourceInt][boneIndex].y = tempBoneTransforms[boneIndex].position.v1;
                    bonePositions[inputSourceInt][boneIndex].z = tempBoneTransforms[boneIndex].position.v2;

                    boneRotations[inputSourceInt][boneIndex].x = tempBoneTransforms[boneIndex].orientation.x;
                    boneRotations[inputSourceInt][boneIndex].y = -tempBoneTransforms[boneIndex].orientation.y;
                    boneRotations[inputSourceInt][boneIndex].z = -tempBoneTransforms[boneIndex].orientation.z;
                    boneRotations[inputSourceInt][boneIndex].w = tempBoneTransforms[boneIndex].orientation.w;
                }

                // Now that we're in the same handedness as Unity, rotate the root bone around the Y axis
                // so that forward is facing down +Z
                Quaternion qFixUpRot = Quaternion.AngleAxis(Mathf.PI * Mathf.Rad2Deg, Vector3.up);

                boneRotations[inputSourceInt][0] = qFixUpRot * boneRotations[inputSourceInt][0];
            }

            changed[inputSource] = changed[inputSource] || poseChanged;

            if (skipStateAndEventUpdates == false)
            {
                for (int boneIndex = 0; boneIndex < tempBoneTransforms.Length; boneIndex++)
                {
                    if (Vector3.Distance(lastBonePositions[inputSourceInt][boneIndex], bonePositions[inputSourceInt][boneIndex]) > changeTolerance)
                    {
                        changed[inputSource] |= true;
                        break;
                    }

                    if (Mathf.Abs(Quaternion.Angle(lastBoneRotations[inputSourceInt][boneIndex], boneRotations[inputSourceInt][boneIndex])) > changeTolerance)
                    {
                        changed[inputSource] |= true;
                        break;
                    }
                }

                CheckAndSendEvents(inputSource);
            }

            if (changed[inputSource])
                lastChanged[inputSource] = Time.time;

            if (skipStateAndEventUpdates == false)
            {
                lastRecordedActive[inputSource] = active[inputSource];
                lastRecordedPoseActionData[inputSource] = poseActionData[inputSource];
            }
        }

        protected override void CheckAndSendEvents(SteamVR_Input_Sources inputSource)
        {
            if (poseActionData[inputSource].pose.eTrackingResult != lastPoseActionData[inputSource].pose.eTrackingResult && onTrackingChanged[inputSource] != null)
                onTrackingChanged[inputSource].Invoke(poseAction, inputSource, poseActionData[inputSource].pose.eTrackingResult);

            if (poseActionData[inputSource].pose.bPoseIsValid != lastPoseActionData[inputSource].pose.bPoseIsValid && onValidPoseChanged[inputSource] != null)
                onValidPoseChanged[inputSource].Invoke(poseAction, inputSource, poseActionData[inputSource].pose.bPoseIsValid);

            if (poseActionData[inputSource].pose.bDeviceIsConnected != lastPoseActionData[inputSource].pose.bDeviceIsConnected && onDeviceConnectedChanged[inputSource] != null)
                onDeviceConnectedChanged[inputSource].Invoke(poseAction, inputSource, poseActionData[inputSource].pose.bDeviceIsConnected);

            if (changed[inputSource])
            {
                if (onChange[inputSource] != null)
                    onChange[inputSource].Invoke(skeletonAction, inputSource);
            }

            if (onActiveChange[inputSource] != null)
            {
                if (lastActive[inputSource] != active[inputSource])
                    onActiveChange[inputSource].Invoke(skeletonAction, inputSource, active[inputSource]);
            }

            if (onUpdate[inputSource] != null)
                onUpdate[inputSource].Invoke(skeletonAction, inputSource);
        }

        /// <summary>
        /// Gets the bone positions in local space
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3[] GetBonePositions(SteamVR_Input_Sources inputSource)
        {
            return bonePositions[(int)inputSource];
        }

        /// <summary>
        /// Gets the bone rotations in local space
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Quaternion[] GetBoneRotations(SteamVR_Input_Sources inputSource)
        {
            return boneRotations[(int)inputSource];
        }

        /// <summary>
        /// Gets the bone positions in local space from the previous update
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3[] GetLastBonePositions(SteamVR_Input_Sources inputSource)
        {
            return lastBonePositions[(int)inputSource];
        }

        /// <summary>
        /// Gets the bone rotations in local space from the previous update
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Quaternion[] GetLastBoneRotations(SteamVR_Input_Sources inputSource)
        {
            return lastBoneRotations[(int)inputSource];
        }

        /// <summary>
        /// Set the range of the motion of the bones in this skeleton. Options are "With Controller" as if your hand is holding your VR controller. 
        /// Or "Without Controller" as if your hand is empty.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void SetRangeOfMotion(SteamVR_Input_Sources inputSource, EVRSkeletalMotionRange range)
        {
            rangeOfMotion[inputSource] = range;
        }

        /// <summary>
        /// Sets the space that you'll get bone data back in. Options are relative to the Model, relative to the Parent bone, and Additive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        /// <param name="space">the space that you'll get bone data back in. Options are relative to the Model, relative to the Parent bone, and Additive.</param>
        public void SetSkeletalTransformSpace(SteamVR_Input_Sources inputSource, EVRSkeletalTransformSpace space)
        {
            skeletalTransformSpace[inputSource] = space;
        }



        /// <summary>Executes a function when this action's bound state changes</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources, bool> functionToCall)
        {
            onActiveChange[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources, bool> functionToStopCalling)
        {
            onActiveChange[inputSource] -= functionToStopCalling;
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToCall)
        {
            onChange[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToStopCalling)
        {
            onChange[inputSource] -= functionToStopCalling;
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToCall)
        {
            onUpdate[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToStopCalling)
        {
            onUpdate[inputSource] -= functionToStopCalling;
        }
    }
    
    /// <summary>
     /// Skeleton Actions are our best approximation of where your hands are while holding vr controllers and pressing buttons. We give you 31 bones to help you animate hand models.
     /// For more information check out this blog post: https://steamcommunity.com/games/250820/announcements/detail/1690421280625220068
     /// </summary>
    public interface ISteamVR_Action_Skeleton : ISteamVR_Action_Pose
    {
        /// <summary>
        /// Gets the bone positions in local space
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Vector3[] GetBonePositions(SteamVR_Input_Sources inputSource);

        /// <summary>
        /// Gets the bone rotations in local space
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Quaternion[] GetBoneRotations(SteamVR_Input_Sources inputSource);

        /// <summary>
        /// Gets the bone positions in local space from the previous update
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Vector3[] GetLastBonePositions(SteamVR_Input_Sources inputSource);

        /// <summary>
        /// Gets the bone rotations in local space from the previous update
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Quaternion[] GetLastBoneRotations(SteamVR_Input_Sources inputSource);

        /// <summary>
        /// Set the range of the motion of the bones in this skeleton. Options are "With Controller" as if your hand is holding your VR controller. 
        /// Or "Without Controller" as if your hand is empty.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void SetRangeOfMotion(SteamVR_Input_Sources inputSource, EVRSkeletalMotionRange range);

        /// <summary>
        /// Sets the space that you'll get bone data back in. Options are relative to the Model, relative to the Parent bone, and Additive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        /// <param name="space">the space that you'll get bone data back in. Options are relative to the Model, relative to the Parent bone, and Additive.</param>
        void SetSkeletalTransformSpace(SteamVR_Input_Sources inputSource, EVRSkeletalTransformSpace space);

        

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources, bool> functionToCall);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources, bool> functionToStopCalling);

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToCall);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToStopCalling);

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToCall);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Skeleton, SteamVR_Input_Sources> functionToStopCalling);
    }

    /// <summary>
    /// The change in range of the motion of the bones in the skeleton. Options are "With Controller" as if your hand is holding your VR controller. 
    /// Or "Without Controller" as if your hand is empty.
    /// </summary>
    public enum SkeletalMotionRangeChange
    {
        None = -1,

        /// <summary>Estimation of bones in hand while holding a controller</summary>
        WithController = 0,

        /// <summary>Estimation of bones in hand while hand is empty (allowing full fist)</summary>
        WithoutController = 1,
    }
}