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
    /// <summary>An analog action with a value generally from 0 to 1. Also provides a delta since the last update.</summary>
    public class SteamVR_Action_Single : SteamVR_Action_In<SteamVR_Action_Single_Data>, ISteamVR_Action_Single
    {
        public SteamVR_Action_Single() { }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public override void UpdateValue(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            actionData.UpdateValue(inputSource);
        }

        /// <summary>The analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public float GetAxis(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetAxis(inputSource);
        }

        /// <summary>The delta from the analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public float GetAxisDelta(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetAxisDelta(inputSource);
        }

        /// <summary>The previous analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public float GetLastAxis(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastAxis(inputSource);
        }

        /// <summary>The previous delta from the analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public float GetLastAxisDelta(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastAxisDelta(inputSource);
        }


        /// <summary>Executes a function when this action's bound state changes</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnActiveChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnActiveChangeListener(functionToCall, inputSource);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnActiveChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnActiveChangeListener(functionToStopCalling, inputSource);
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToCall, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnChangeListener(functionToCall, inputSource);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnChangeListener(functionToStopCalling, inputSource);
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnUpdateListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToCall, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnUpdateListener(functionToCall, inputSource);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnUpdateListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnUpdateListener(functionToStopCalling, inputSource);
        }
    }

    /// <summary>An analog action with a value generally from 0 to 1. Also provides a delta since the last update.</summary>
    public class SteamVR_Action_Single_Data : SteamVR_Action_In_Data, ISteamVR_Action_Single
    {
        public SteamVR_Action_Single_Data() { }

        protected Dictionary<SteamVR_Input_Sources, InputAnalogActionData_t> actionData = new Dictionary<SteamVR_Input_Sources, InputAnalogActionData_t>(new SteamVR_Input_Sources_Comparer());
        
        protected Dictionary<SteamVR_Input_Sources, InputAnalogActionData_t> lastActionData = new Dictionary<SteamVR_Input_Sources, InputAnalogActionData_t>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Single, SteamVR_Input_Sources, bool>> onActiveChange =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Single, SteamVR_Input_Sources, bool>>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Single, SteamVR_Input_Sources, float>> onChange =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Single, SteamVR_Input_Sources, float>>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Single, SteamVR_Input_Sources, float>> onUpdate =
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Single, SteamVR_Input_Sources, float>>(new SteamVR_Input_Sources_Comparer());
        
        protected InputAnalogActionData_t tempActionData = new InputAnalogActionData_t();
        
        protected uint actionData_size = 0;

        protected SteamVR_Action_Single singleAction;

        public override void Initialize()
        {
            base.Initialize();
            actionData_size = (uint)Marshal.SizeOf(tempActionData);
            singleAction = (SteamVR_Action_Single)wrappingAction;
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        protected override void InitializeDictionaries(SteamVR_Input_Sources source)
        {
            base.InitializeDictionaries(source);

            actionData.Add(source, new InputAnalogActionData_t());
            lastActionData.Add(source, new InputAnalogActionData_t());

            onActiveChange.Add(source, null);
            onChange.Add(source, null);
            onUpdate.Add(source, null);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public override void UpdateValue(SteamVR_Input_Sources inputSource)
        {
            lastActionData[inputSource] = actionData[inputSource];
            lastActive[inputSource] = active[inputSource];

            EVRInputError err = OpenVR.Input.GetAnalogActionData(handle, ref tempActionData, actionData_size, SteamVR_Input_Source.GetHandle(inputSource));
            if (err != EVRInputError.None)
                Debug.LogError("<b>[SteamVR]</b> GetAnalogActionData error (" + fullPath + "): " + err.ToString() + " handle: " + handle.ToString());

            active[inputSource] = tempActionData.bActive;
            activeOrigin[inputSource] = tempActionData.activeOrigin;
            updateTime[inputSource] = tempActionData.fUpdateTime;
            changed[inputSource] = false;
            actionData[inputSource] = tempActionData;

            if (Mathf.Abs(GetAxisDelta(inputSource)) > changeTolerance)
            {
                changed[inputSource] = true;
                lastChanged[inputSource] = Time.time;

                if (onChange[inputSource] != null)
                    onChange[inputSource].Invoke(singleAction, inputSource, tempActionData.x);
            }

            if (onUpdate[inputSource] != null)
            {
                onUpdate[inputSource].Invoke(singleAction, inputSource, tempActionData.x);
            }

            if (onActiveChange[inputSource] != null && lastActive[inputSource] != active[inputSource])
                onActiveChange[inputSource].Invoke(singleAction, inputSource, active[inputSource]);
        }

        /// <summary>The analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public float GetAxis(SteamVR_Input_Sources inputSource)
        {
            return actionData[inputSource].x;
        }

        /// <summary>The delta from the analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public float GetAxisDelta(SteamVR_Input_Sources inputSource)
        {
            return actionData[inputSource].deltaX;
        }

        /// <summary>The previous analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public float GetLastAxis(SteamVR_Input_Sources inputSource)
        {
            return lastActionData[inputSource].x;
        }

        /// <summary>The previous delta from the analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public float GetLastAxisDelta(SteamVR_Input_Sources inputSource)
        {
            return lastActionData[inputSource].deltaX;
        }


        /// <summary>Executes a function when this action's bound state changes</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnActiveChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource)
        {
            onActiveChange[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnActiveChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            onActiveChange[inputSource] -= functionToStopCalling;
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToCall, SteamVR_Input_Sources inputSource)
        {
            onChange[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            onChange[inputSource] -= functionToStopCalling;
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnUpdateListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToCall, SteamVR_Input_Sources inputSource)
        {
            onUpdate[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnUpdateListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            onUpdate[inputSource] -= functionToStopCalling;
        }
    }
    
    /// <summary>An analog action with a value generally from 0 to 1. Also provides a delta since the last update.</summary>
    public interface ISteamVR_Action_Single : ISteamVR_Action_In
    {
        /// <summary>The analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        float GetAxis(SteamVR_Input_Sources inputSource);

        /// <summary>The delta from the analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        float GetAxisDelta(SteamVR_Input_Sources inputSource);

        /// <summary>The previous analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        float GetLastAxis(SteamVR_Input_Sources inputSource);

        /// <summary>The previous delta from the analog value</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        float GetLastAxisDelta(SteamVR_Input_Sources inputSource);


        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnActiveChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, bool> action, SteamVR_Input_Sources inputSource);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnActiveChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, bool> action, SteamVR_Input_Sources inputSource);

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> action, SteamVR_Input_Sources inputSource);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnChangeListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> action, SteamVR_Input_Sources inputSource);

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnUpdateListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToCall, SteamVR_Input_Sources inputSource);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnUpdateListener(Action<SteamVR_Action_Single, SteamVR_Input_Sources, float> functionToStopCalling, SteamVR_Input_Sources inputSource);
    }
}