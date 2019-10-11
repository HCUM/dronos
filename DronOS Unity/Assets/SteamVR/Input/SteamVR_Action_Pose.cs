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
    /// Pose actions represent a position and orientation inside the tracked space. 
    /// SteamVR also keeps a log of past poses so you can retrieve old poses with GetPoseAtTimeOffset or GetVelocitiesAtTimeOffset.
    /// You can also pass in times in the future to these methods for SteamVR's best prediction of where the pose will be at that time.
    /// </summary>
    public class SteamVR_Action_Pose : SteamVR_Action_Pose_Base<SteamVR_Action_Pose_Data>
    {
        public SteamVR_Action_Pose() { }
    }

    [Serializable]
    /// <summary>
    /// The base pose action (pose and skeleton inherit from this)
    /// </summary>
    public abstract class SteamVR_Action_Pose_Base<T> : SteamVR_Action_In<T>, ISteamVR_Action_Pose where T : SteamVR_Action_Pose_Data, new()
    {
        public float predictedSecondsFromNow
        {
            get
            {
                if (initialized == false)
                    Initialize();

                return actionData.predictedSecondsFromNow;
            }

            set
            {
                if (initialized == false)
                    Initialize();

                actionData.predictedSecondsFromNow = value;
            }
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public override void UpdateValue(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            actionData.UpdateValue(inputSource);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public virtual void UpdateValue(SteamVR_Input_Sources inputSource, bool skipStateAndEventUpdates)
        {
            if (initialized == false)
                Initialize();

            actionData.UpdateValue(inputSource, skipStateAndEventUpdates);
        }

        /// <summary>
        /// SteamVR keeps a log of past poses so you can retrieve old poses or estimated poses in the future by passing in a secondsFromNow value that is negative or positive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetVelocitiesAtTimeOffset(SteamVR_Input_Sources inputSource, float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetVelocitiesAtTimeOffset(inputSource, secondsFromNow, out velocity, out angularVelocity);
        }

        /// <summary>
        /// SteamVR keeps a log of past poses so you can retrieve old poses or estimated poses in the future by passing in a secondsFromNow value that is negative or positive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetPoseAtTimeOffset(SteamVR_Input_Sources inputSource, float secondsFromNow, out Vector3 position, out Quaternion rotation, out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetPoseAtTimeOffset(inputSource, secondsFromNow, out position, out rotation, out velocity, out angularVelocity);
        }

        /// <summary>
        /// Update a transform's local position and local roation to match the pose.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        /// <param name="transformToUpdate">The transform of the object to be updated</param>
        public void UpdateTransform(SteamVR_Input_Sources inputSource, Transform transformToUpdate)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.UpdateTransform(inputSource, transformToUpdate);
        }

        /// <param name="newUniverseOrigin">The origin of the universe. Don't get this wrong.</param>
        public static void SetTrackingUniverseOrigin(ETrackingUniverseOrigin newUniverseOrigin)
        {
            SteamVR_Action_Pose_Data.SetTrackingUniverseOrigin(newUniverseOrigin);
        }

        /// <summary>The local position of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetLocalPosition(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLocalPosition(inputSource);
        }

        /// <summary>The local rotation of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Quaternion GetLocalRotation(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLocalRotation(inputSource);
        }

        /// <summary>The local velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetVelocity(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetVelocity(inputSource);
        }

        /// <summary>The local angular velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetAngularVelocity(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetAngularVelocity(inputSource);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetDeviceIsConnected(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetDeviceIsConnected(inputSource);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetPoseIsValid(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetPoseIsValid(inputSource);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public ETrackingResult GetTrackingResult(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetTrackingResult(inputSource);
        }



        /// <summary>The last local position of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetLastLocalPosition(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastLocalPosition(inputSource);
        }

        /// <summary>The last local rotation of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Quaternion GetLastLocalRotation(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastLocalRotation(inputSource);
        }

        /// <summary>The last local velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetLastVelocity(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastVelocity(inputSource);
        }

        /// <summary>The last local angular velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetLastAngularVelocity(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastAngularVelocity(inputSource);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastDeviceIsConnected(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastDeviceIsConnected(inputSource);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastPoseIsValid(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastPoseIsValid(inputSource);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public ETrackingResult GetLastTrackingResult(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastTrackingResult(inputSource);
        }


        /// <summary>Fires an event when a device is connected or disconnected.</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when a device is connected. Should take a SteamVR_Action_Pose as a param</param>
        public void AddOnDeviceConnectedChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnDeviceConnectedChanged(inputSource, functionToCall);
        }

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when a device is connected. Should take a SteamVR_Action_Pose as a param</param>
        public void RemoveOnDeviceConnectedChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnDeviceConnectedChanged(inputSource, functionToStopCalling);
        }


        /// <summary>Fires an event when the tracking of the device has changed</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when tracking has changed. Should take a SteamVR_Action_Pose as a param</param>
        public void AddOnTrackingChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnTrackingChanged(inputSource, functionToCall);
        }

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when tracking has changed. Should take a SteamVR_Action_Pose as a param</param>
        public void RemoveOnTrackingChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnTrackingChanged(inputSource, functionToStopCalling);
        }


        /// <summary>Fires an event when the device now has a valid pose or no longer has a valid pose</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when the pose has become valid or invalid. Should take a SteamVR_Action_Pose as a param</param>
        public void AddOnValidPoseChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnValidPoseChanged(inputSource, functionToCall);
        }

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when the pose has become valid or invalid. Should take a SteamVR_Action_Pose as a param</param>
        public void RemoveOnValidPoseChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnValidPoseChanged(inputSource, functionToStopCalling);
        }


        /// <summary>Executes a function when this action's bound state changes</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnActiveChangeListener(inputSource, functionToCall);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnActiveChangeListener(inputSource, functionToStopCalling);
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnChangeListener(inputSource, functionToCall);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnChangeListener(inputSource, functionToStopCalling);
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToCall)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnUpdateListener(inputSource, functionToCall);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToStopCalling)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnUpdateListener(inputSource, functionToStopCalling);
        }
    }
    
    /// <summary>
     /// Pose actions represent a position and orientation inside the tracked space. 
     /// SteamVR also keeps a log of past poses so you can retrieve old poses with GetPoseAtTimeOffset or GetVelocitiesAtTimeOffset.
     /// You can also pass in times in the future to these methods for SteamVR's best prediction of where the pose will be at that time.
     /// </summary>
    public class SteamVR_Action_Pose_Data : SteamVR_Action_In_Data, ISteamVR_Action_Pose
    {
        public SteamVR_Action_Pose_Data() { }

        protected static ETrackingUniverseOrigin universeOrigin = ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated;

        protected float _predictedSecondsFromNow = 0.011f;
        public float predictedSecondsFromNow { get { return _predictedSecondsFromNow; } set { _predictedSecondsFromNow = value; } }
        
        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult>> onTrackingChanged = 
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult>>(new SteamVR_Input_Sources_Comparer());
        
        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool>> onValidPoseChanged = 
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool>>(new SteamVR_Input_Sources_Comparer());
        
        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool>> onDeviceConnectedChanged = 
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool>>(new SteamVR_Input_Sources_Comparer());


        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool>> onActiveChange =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool>>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources>> onChange =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources>>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources>> onUpdate =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Pose, SteamVR_Input_Sources>>(new SteamVR_Input_Sources_Comparer());


        protected Dictionary<SteamVR_Input_Sources, InputPoseActionData_t> poseActionData = 
            new Dictionary<SteamVR_Input_Sources, InputPoseActionData_t>(new SteamVR_Input_Sources_Comparer());
        
        protected Dictionary<SteamVR_Input_Sources, InputPoseActionData_t> lastPoseActionData = 
            new Dictionary<SteamVR_Input_Sources, InputPoseActionData_t>(new SteamVR_Input_Sources_Comparer());
        
        protected Dictionary<SteamVR_Input_Sources, InputPoseActionData_t> lastRecordedPoseActionData = 
            new Dictionary<SteamVR_Input_Sources, InputPoseActionData_t>(new SteamVR_Input_Sources_Comparer());
        
        protected Dictionary<SteamVR_Input_Sources, bool> lastRecordedActive = 
            new Dictionary<SteamVR_Input_Sources, bool>(new SteamVR_Input_Sources_Comparer());
        
        protected InputPoseActionData_t tempPoseActionData = new InputPoseActionData_t();
        
        protected uint poseActionData_size = 0;

        protected SteamVR_Action_Pose poseAction;

        public override void Initialize()
        {
            base.Initialize();
            poseActionData_size = (uint)Marshal.SizeOf(tempPoseActionData);
            poseAction = wrappingAction as SteamVR_Action_Pose;
        }

        /// <param name="inputSource">The device you would like to initialize dictionaries for</param>
        protected override void InitializeDictionaries(SteamVR_Input_Sources source)
        {
            base.InitializeDictionaries(source);

            onTrackingChanged.Add(source, null);
            onValidPoseChanged.Add(source, null);
            onDeviceConnectedChanged.Add(source, null);
            poseActionData.Add(source, new InputPoseActionData_t());
            lastPoseActionData.Add(source, new InputPoseActionData_t());
            lastRecordedPoseActionData.Add(source, new InputPoseActionData_t());
            lastRecordedActive.Add(source, false);

            onActiveChange.Add(source, null);
            onChange.Add(source, null);
            onUpdate.Add(source, null);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public override void UpdateValue(SteamVR_Input_Sources inputSource)
        {
            UpdateValue(inputSource, false);
        }

        protected void ResetLastStates(SteamVR_Input_Sources inputSource)
        {
            lastPoseActionData[inputSource] = lastRecordedPoseActionData[inputSource];
            lastActive[inputSource] = lastRecordedActive[inputSource];
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public virtual void UpdateValue(SteamVR_Input_Sources inputSource, bool skipStateAndEventUpdates)
        {
            changed[inputSource] = false;
            if (skipStateAndEventUpdates == false)
            {
                ResetLastStates(inputSource);
            }

            EVRInputError err = OpenVR.Input.GetPoseActionData(handle, universeOrigin, predictedSecondsFromNow, ref tempPoseActionData, poseActionData_size, SteamVR_Input_Source.GetHandle(inputSource));
            if (err != EVRInputError.None)
            {
                Debug.LogError("<b>[SteamVR]</b> GetPoseActionData error (" + fullPath + "): " + err.ToString() + " handle: " + handle.ToString());
            }

            poseActionData[inputSource] = tempPoseActionData;
            active[inputSource] = tempPoseActionData.bActive;
            activeOrigin[inputSource] = tempPoseActionData.activeOrigin;
            updateTime[inputSource] = Time.time;

            if (Vector3.Distance(GetLocalPosition(inputSource), GetLastLocalPosition(inputSource)) > changeTolerance)
            {
                changed[inputSource] = true;
            }
            else if (Mathf.Abs(Quaternion.Angle(GetLocalRotation(inputSource), GetLastLocalRotation(inputSource))) > changeTolerance)
            {
                changed[inputSource] = true;
            }

            if (skipStateAndEventUpdates == false)
            {
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

        /// <summary>
        /// SteamVR keeps a log of past poses so you can retrieve old poses or estimated poses in the future by passing in a secondsFromNow value that is negative or positive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetVelocitiesAtTimeOffset(SteamVR_Input_Sources inputSource, float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
        {
            EVRInputError err = OpenVR.Input.GetPoseActionData(handle, universeOrigin, secondsFromNow, ref tempPoseActionData, poseActionData_size, SteamVR_Input_Source.GetHandle(inputSource));
            if (err != EVRInputError.None)
            {
                Debug.LogError("<b>[SteamVR]</b> GetPoseActionData error (" + fullPath + "): " + err.ToString() + " handle: " + handle.ToString()); //todo: this should be an error

                velocity = Vector3.zero;
                angularVelocity = Vector3.zero;
                return false;
            }

            velocity = new Vector3(tempPoseActionData.pose.vVelocity.v0, tempPoseActionData.pose.vVelocity.v1, -tempPoseActionData.pose.vVelocity.v2);
            angularVelocity = new Vector3(-tempPoseActionData.pose.vAngularVelocity.v0, -tempPoseActionData.pose.vAngularVelocity.v1, tempPoseActionData.pose.vAngularVelocity.v2);

            return true;
        }

        /// <summary>
        /// SteamVR keeps a log of past poses so you can retrieve old poses or estimated poses in the future by passing in a secondsFromNow value that is negative or positive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetPoseAtTimeOffset(SteamVR_Input_Sources inputSource, float secondsFromNow, out Vector3 position, out Quaternion rotation, out Vector3 velocity, out Vector3 angularVelocity)
        {
            EVRInputError err = OpenVR.Input.GetPoseActionData(handle, universeOrigin, secondsFromNow, ref tempPoseActionData, poseActionData_size, SteamVR_Input_Source.GetHandle(inputSource));
            if (err != EVRInputError.None)
            {
                if (err == EVRInputError.InvalidHandle)
                {
                    //todo: ignoring this error for now since it throws while the dashboard is up
                }
                else
                {
                    Debug.LogError("GetPoseActionData error (" + fullPath + "): " + err.ToString() + " handle: " + handle.ToString()); //todo: this should be an error
                }

                velocity = Vector3.zero;
                angularVelocity = Vector3.zero;
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return false;
            }

            velocity = new Vector3(tempPoseActionData.pose.vVelocity.v0, tempPoseActionData.pose.vVelocity.v1, -tempPoseActionData.pose.vVelocity.v2);
            angularVelocity = new Vector3(-tempPoseActionData.pose.vAngularVelocity.v0, -tempPoseActionData.pose.vAngularVelocity.v1, tempPoseActionData.pose.vAngularVelocity.v2);
            position = SteamVR_Utils.GetPosition(tempPoseActionData.pose.mDeviceToAbsoluteTracking);
            rotation = SteamVR_Utils.GetRotation(tempPoseActionData.pose.mDeviceToAbsoluteTracking);

            return true;
        }

        /// <summary>
        /// Update a transform's local position and local roation to match the pose.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        /// <param name="transformToUpdate">The transform of the object to be updated</param>
        public void UpdateTransform(SteamVR_Input_Sources inputSource, Transform transformToUpdate)
        {
            transformToUpdate.localPosition = GetLocalPosition(inputSource);
            transformToUpdate.localRotation = GetLocalRotation(inputSource);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        protected virtual void CheckAndSendEvents(SteamVR_Input_Sources inputSource)
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
                    onChange[inputSource].Invoke(poseAction, inputSource);
            }

            if (onActiveChange[inputSource] != null)
            {
                if (lastActive[inputSource] != active[inputSource])
                    onActiveChange[inputSource].Invoke(poseAction, inputSource, active[inputSource]);
            }

            if (onUpdate[inputSource] != null)
                onUpdate[inputSource].Invoke(poseAction, inputSource);
        }

        /// <param name="newUniverseOrigin">The origin of the universe. Don't get this wrong.</param>
        public static void SetTrackingUniverseOrigin(ETrackingUniverseOrigin newUniverseOrigin)
        {
            universeOrigin = newUniverseOrigin;
        }

        /// <summary>The local position of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetLocalPosition(SteamVR_Input_Sources inputSource)
        {
            // Convert the transform from SteamVR's coordinate system to Unity's coordinate system.
            // ie: flip the X axis
            return new Vector3(poseActionData[inputSource].pose.mDeviceToAbsoluteTracking.m3,
                poseActionData[inputSource].pose.mDeviceToAbsoluteTracking.m7,
                -poseActionData[inputSource].pose.mDeviceToAbsoluteTracking.m11);
        }

        /// <summary>The local rotation of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Quaternion GetLocalRotation(SteamVR_Input_Sources inputSource)
        {
            return SteamVR_Utils.GetRotation(poseActionData[inputSource].pose.mDeviceToAbsoluteTracking);
        }

        /// <summary>The local velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetVelocity(SteamVR_Input_Sources inputSource)
        {
            return new Vector3(poseActionData[inputSource].pose.vVelocity.v0, poseActionData[inputSource].pose.vVelocity.v1, -poseActionData[inputSource].pose.vVelocity.v2);
        }

        /// <summary>The local angular velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetAngularVelocity(SteamVR_Input_Sources inputSource)
        {
            return new Vector3(-poseActionData[inputSource].pose.vAngularVelocity.v0, -poseActionData[inputSource].pose.vAngularVelocity.v1, poseActionData[inputSource].pose.vAngularVelocity.v2);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetDeviceIsConnected(SteamVR_Input_Sources inputSource)
        {
            return poseActionData[inputSource].pose.bDeviceIsConnected;
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetPoseIsValid(SteamVR_Input_Sources inputSource)
        {
            return poseActionData[inputSource].pose.bPoseIsValid;
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public ETrackingResult GetTrackingResult(SteamVR_Input_Sources inputSource)
        {
            return poseActionData[inputSource].pose.eTrackingResult;
        }



        /// <summary>The last local position of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetLastLocalPosition(SteamVR_Input_Sources inputSource)
        {
            return new Vector3(lastPoseActionData[inputSource].pose.mDeviceToAbsoluteTracking.m3,
                lastPoseActionData[inputSource].pose.mDeviceToAbsoluteTracking.m7,
                -lastPoseActionData[inputSource].pose.mDeviceToAbsoluteTracking.m11);
        }

        /// <summary>The last local rotation of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Quaternion GetLastLocalRotation(SteamVR_Input_Sources inputSource)
        {
            return SteamVR_Utils.GetRotation(lastPoseActionData[inputSource].pose.mDeviceToAbsoluteTracking);
        }

        /// <summary>The last local velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetLastVelocity(SteamVR_Input_Sources inputSource)
        {
            return new Vector3(lastPoseActionData[inputSource].pose.vVelocity.v0, lastPoseActionData[inputSource].pose.vVelocity.v1, -lastPoseActionData[inputSource].pose.vVelocity.v2);
        }

        /// <summary>The last local angular velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public Vector3 GetLastAngularVelocity(SteamVR_Input_Sources inputSource)
        {
            return new Vector3(-lastPoseActionData[inputSource].pose.vAngularVelocity.v0, -lastPoseActionData[inputSource].pose.vAngularVelocity.v1, lastPoseActionData[inputSource].pose.vAngularVelocity.v2);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastDeviceIsConnected(SteamVR_Input_Sources inputSource)
        {
            return lastPoseActionData[inputSource].pose.bDeviceIsConnected;
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastPoseIsValid(SteamVR_Input_Sources inputSource)
        {
            return lastPoseActionData[inputSource].pose.bPoseIsValid;
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public ETrackingResult GetLastTrackingResult(SteamVR_Input_Sources inputSource)
        {
            return lastPoseActionData[inputSource].pose.eTrackingResult;
        }


        /// <summary>Fires an event when a device is connected or disconnected.</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when a device is connected. Should take a SteamVR_Action_Pose as a param</param>
        public void AddOnDeviceConnectedChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> action)
        {
            onDeviceConnectedChanged[inputSource] += action;
        }

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when a device is connected. Should take a SteamVR_Action_Pose as a param</param>
        public void RemoveOnDeviceConnectedChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> action)
        {
            onDeviceConnectedChanged[inputSource] -= action;
        }


        /// <summary>Fires an event when the tracking of the device has changed</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when tracking has changed. Should take a SteamVR_Action_Pose as a param</param>
        public void AddOnTrackingChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult> action)
        {
            onTrackingChanged[inputSource] += action;
        }

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when tracking has changed. Should take a SteamVR_Action_Pose as a param</param>
        public void RemoveOnTrackingChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult> action)
        {
            onTrackingChanged[inputSource] -= action;
        }


        /// <summary>Fires an event when the device now has a valid pose or no longer has a valid pose</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when the pose has become valid or invalid. Should take a SteamVR_Action_Pose as a param</param>
        public void AddOnValidPoseChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> action)
        {
            onValidPoseChanged[inputSource] += action;
        }

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when the pose has become valid or invalid. Should take a SteamVR_Action_Pose as a param</param>
        public void RemoveOnValidPoseChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> action)
        {
            onValidPoseChanged[inputSource] -= action;
        }


        /// <summary>Executes a function when this action's bound state changes</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToCall)
        {
            onActiveChange[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToStopCalling)
        {
            onActiveChange[inputSource] -= functionToStopCalling;
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToCall)
        {
            onChange[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToStopCalling)
        {
            onChange[inputSource] -= functionToStopCalling;
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToCall)
        {
            onUpdate[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToStopCalling)
        {
            onUpdate[inputSource] -= functionToStopCalling;
        }
    }
    
    /// <summary>
     /// Pose actions represent a position and orientation inside the tracked space. 
     /// SteamVR also keeps a log of past poses so you can retrieve old poses with GetPoseAtTimeOffset or GetVelocitiesAtTimeOffset.
     /// You can also pass in times in the future to these methods for SteamVR's best prediction of where the pose will be at that time.
     /// </summary>
    public interface ISteamVR_Action_Pose : ISteamVR_Action_In
    {
        float predictedSecondsFromNow { get; set; }

        /// <summary>
        /// SteamVR keeps a log of past poses so you can retrieve old poses or estimated poses in the future by passing in a secondsFromNow value that is negative or positive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetVelocitiesAtTimeOffset(SteamVR_Input_Sources inputSource, float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity);

        /// <summary>
        /// SteamVR keeps a log of past poses so you can retrieve old poses or estimated poses in the future by passing in a secondsFromNow value that is negative or positive.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetPoseAtTimeOffset(SteamVR_Input_Sources inputSource, float secondsFromNow, out Vector3 position, out Quaternion rotation, out Vector3 velocity, out Vector3 angularVelocity);

        /// <summary>
        /// Update a transform's local position and local roation to match the pose.
        /// </summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        /// <param name="transformToUpdate">The transform of the object to be updated</param>
        void UpdateTransform(SteamVR_Input_Sources inputSource, Transform transformToUpdate);

        /// <summary>The local position of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Vector3 GetLocalPosition(SteamVR_Input_Sources inputSource);

        /// <summary>The local rotation of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Quaternion GetLocalRotation(SteamVR_Input_Sources inputSource);

        /// <summary>The local velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Vector3 GetVelocity(SteamVR_Input_Sources inputSource);

        /// <summary>The local angular velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Vector3 GetAngularVelocity(SteamVR_Input_Sources inputSource);

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetDeviceIsConnected(SteamVR_Input_Sources inputSource);

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetPoseIsValid(SteamVR_Input_Sources inputSource);

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        ETrackingResult GetTrackingResult(SteamVR_Input_Sources inputSource);



        /// <summary>The last local position of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Vector3 GetLastLocalPosition(SteamVR_Input_Sources inputSource);

        /// <summary>The last local rotation of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Quaternion GetLastLocalRotation(SteamVR_Input_Sources inputSource);

        /// <summary>The last local velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Vector3 GetLastVelocity(SteamVR_Input_Sources inputSource);

        /// <summary>The last local angular velocity of the pose relative to the center of the tracked space.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        Vector3 GetLastAngularVelocity(SteamVR_Input_Sources inputSource);

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetLastDeviceIsConnected(SteamVR_Input_Sources inputSource);

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetLastPoseIsValid(SteamVR_Input_Sources inputSource);

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        ETrackingResult GetLastTrackingResult(SteamVR_Input_Sources inputSource);


        /// <summary>Fires an event when a device is connected or disconnected.</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when a device is connected. Should take a SteamVR_Action_Pose as a param</param>
        void AddOnDeviceConnectedChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToCall);

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when a device is connected. Should take a SteamVR_Action_Pose as a param</param>
        void RemoveOnDeviceConnectedChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToStopCalling);


        /// <summary>Fires an event when the tracking of the device has changed</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when tracking has changed. Should take a SteamVR_Action_Pose as a param</param>
        void AddOnTrackingChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult> functionToCall);

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when tracking has changed. Should take a SteamVR_Action_Pose as a param</param>
        void RemoveOnTrackingChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult> functionToStopCalling);


        /// <summary>Fires an event when the device now has a valid pose or no longer has a valid pose</summary>
        /// <param name="inputSource">The device you would like to add an event to. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to be called when the pose has become valid or invalid. Should take a SteamVR_Action_Pose as a param</param>
        void AddOnValidPoseChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToCall);

        /// <param name="inputSource">The device you would like to remove an event from. Any if the action is not device specific.</param>
        /// <param name="action">The method you would like to stop calling when the pose has become valid or invalid. Should take a SteamVR_Action_Pose as a param</param>
        void RemoveOnValidPoseChanged(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToStopCalling);



        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToCall);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnActiveChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> functionToStopCalling);

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToCall);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnChangeListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToStopCalling);

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToCall);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnUpdateListener(SteamVR_Input_Sources inputSource, Action<SteamVR_Action_Pose, SteamVR_Input_Sources> functionToStopCalling);
    }
}