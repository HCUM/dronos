//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace Valve.VR
{
    /// <summary>
    /// This component simplifies the use of Pose actions. Adding it to a gameobject will auto set that transform's position and rotation every update to match the pose.
    /// Advanced velocity estimation is also handled through a buffer of the last 30 updates.
    /// </summary>
    public class SteamVR_Behaviour_Pose : MonoBehaviour
    {
        public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

        [Tooltip("The device this action should apply to. Any if the action is not device specific.")]
        public SteamVR_Input_Sources inputSource;

        [Tooltip("If not set, relative to parent")]
        public Transform origin;

        public SteamVR_Action_Vibration hapticSignal;

        /// <summary>Returns whether or not the current pose is in a valid state</summary>
        public bool isValid { get { return poseAction.GetPoseIsValid(inputSource); } }

        /// <summary>Returns whether or not the pose action is bound and able to be updated</summary>
        public bool isActive { get { return poseAction.GetActive(inputSource); } }

        /// <summary>This event will fire whenever the position or rotation of this transform is updated.</summary>
        public SteamVR_Behaviour_PoseEvent onTransformUpdated;

        /// <summary>This event will fire whenever the position or rotation of this transform is changed.</summary>
        public SteamVR_Behaviour_PoseEvent onTransformChanged;

        /// <summary>This event will fire whenever the device is connected or disconnected</summary>
        public SteamVR_Behaviour_Pose_ConnectedChangedEvent onConnectedChanged;

        /// <summary>This event will fire whenever the device's tracking state changes</summary>
        public SteamVR_Behaviour_Pose_TrackingChangedEvent onTrackingChanged;

        protected int deviceIndex = -1;

        protected SteamVR_HistoryBuffer historyBuffer = new SteamVR_HistoryBuffer(30);
        

        protected virtual void Start()
        {
            if (poseAction == null)
            {
                Debug.LogError("<b>[SteamVR]</b> No pose action set for this component");
                this.enabled = false;
                return;
            }

            CheckDeviceIndex();

            if (origin == null)
                origin = this.transform.parent;
        }

        protected virtual void OnDeviceConnectedChanged(SteamVR_Action_Pose changedAction, SteamVR_Input_Sources changedSource, bool connected)
        {
            CheckDeviceIndex();

            if (onConnectedChanged != null)
                onConnectedChanged.Invoke(poseAction, inputSource, connected);
        }

        protected virtual void OnTrackingChanged(SteamVR_Action_Pose changedAction, SteamVR_Input_Sources changedSource, ETrackingResult trackingChanged)
        {
            if (onTrackingChanged != null)
                onTrackingChanged.Invoke(poseAction, inputSource, trackingChanged);
        }

        protected virtual void CheckDeviceIndex()
        {
            if (poseAction.GetActive(inputSource))
            {
                if (poseAction.GetDeviceIsConnected(inputSource))
                {
                    int currentDeviceIndex = (int)poseAction.GetDeviceIndex(inputSource);

                    if (deviceIndex != currentDeviceIndex)
                    {
                        deviceIndex = currentDeviceIndex;
                        this.gameObject.BroadcastMessage("SetInputSource", inputSource, SendMessageOptions.DontRequireReceiver);
                        this.gameObject.BroadcastMessage("SetDeviceIndex", deviceIndex, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the device index for the device bound to the pose. 
        /// </summary>
        public int GetDeviceIndex()
        {
            if (deviceIndex == -1)
                CheckDeviceIndex();

            return deviceIndex;
        }



        protected virtual void OnEnable()
        {
            SteamVR.Initialize();

            SteamVR_Input.OnPosesUpdated += SteamVR_Input_OnPosesUpdated;

            if (poseAction != null)
            {
                poseAction.AddOnDeviceConnectedChanged(inputSource, OnDeviceConnectedChanged);
                poseAction.AddOnTrackingChanged(inputSource, OnTrackingChanged);
            }
        }

        protected virtual void OnDisable()
        {
            SteamVR_Input.OnPosesUpdated -= SteamVR_Input_OnPosesUpdated;

            if (poseAction != null)
            {
                poseAction.RemoveOnDeviceConnectedChanged(inputSource, OnDeviceConnectedChanged);
                poseAction.RemoveOnTrackingChanged(inputSource, OnTrackingChanged);
            }

            historyBuffer.Clear();
        }

        /// <summary>Returns the current velocity of the pose (as of the last update)</summary>
        public Vector3 GetVelocity()
        {
            return poseAction.GetVelocity(inputSource);
        }

        /// <summary>Returns the current angular velocity of the pose (as of the last update)</summary>
        public Vector3 GetAngularVelocity()
        {
            return poseAction.GetAngularVelocity(inputSource);
        }

        /// <summary>Returns the velocities of the pose at the time specified. Can predict in the future or return past values.</summary>
        public bool GetVelocitiesAtTimeOffset(float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
        {
            return poseAction.GetVelocitiesAtTimeOffset(inputSource, secondsFromNow, out velocity, out angularVelocity);
        }

        protected void UpdateHistoryBuffer()
        {
            historyBuffer.Update(poseAction.GetLocalPosition(inputSource), poseAction.GetLocalRotation(inputSource), poseAction.GetVelocity(inputSource), poseAction.GetAngularVelocity(inputSource));
        }

        /// <summary>Uses previously recorded values to find the peak speed of the pose and returns the corresponding velocity and angular velocity</summary>
        public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
        {
            int top = historyBuffer.GetTopVelocity(10, 1);

            historyBuffer.GetAverageVelocities(out velocity, out angularVelocity, 2, top);
        }

        private void SteamVR_Input_OnPosesUpdated(bool obj)
        {
            UpdateHistoryBuffer();
            Update();
        }

        protected virtual void Update()
        {
            if (poseAction == null)
                return;

            CheckDeviceIndex();

            if (origin != null)
            {
                transform.position = origin.transform.TransformPoint(poseAction.GetLocalPosition(inputSource));
                transform.rotation = origin.rotation * poseAction.GetLocalRotation(inputSource);
            }
            else
            {
                transform.localPosition = poseAction.GetLocalPosition(inputSource);
                transform.localRotation = poseAction.GetLocalRotation(inputSource);
            }

            if (poseAction.GetChanged(inputSource))
            {
                if (onTransformChanged != null)
                    onTransformChanged.Invoke(poseAction, inputSource);
            }

            if (onTransformUpdated != null)
                onTransformUpdated.Invoke(poseAction, inputSource);
        }
    }

    [Serializable]
    public class SteamVR_Behaviour_PoseEvent : UnityEvent<SteamVR_Action_Pose, SteamVR_Input_Sources> { }

    [Serializable]
    public class SteamVR_Behaviour_Pose_ConnectedChangedEvent : UnityEvent<SteamVR_Action_Pose, SteamVR_Input_Sources, bool> { }

    [Serializable]
    public class SteamVR_Behaviour_Pose_TrackingChangedEvent : UnityEvent<SteamVR_Action_Pose, SteamVR_Input_Sources, ETrackingResult> { }
}