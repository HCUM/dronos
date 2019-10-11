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
    /// Boolean actions are either true or false. There is an onStateUp and onStateDown event for the rising and falling edge.
    /// </summary>
    public class SteamVR_Action_Boolean : SteamVR_Action_In<SteamVR_Action_Boolean_Data>, ISteamVR_Action_Boolean
    {
        public SteamVR_Action_Boolean() { }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public override void UpdateValue(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            actionData.UpdateValue(inputSource);
        }

        /// <summary>Returns true if the value of the action has been set to true (from false) in the most recent update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetStateDown(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetStateDown(inputSource);
        }

        /// <summary>Returns true if the value of the action has been set to false (from true) in the most recent update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetStateUp(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetStateUp(inputSource);
        }

        /// <summary>Returns true if the value of the action is currently true</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetState(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetState(inputSource);
        }

        /// <summary>Returns true if the value of the action has been set to true (from false) in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastStateDown(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastStateDown(inputSource);
        }

        /// <summary>Returns true if the value of the action has been set to false (from true) in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastStateUp(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastStateUp(inputSource);
        }

        /// <summary>Returns true if the value of the action was true in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastState(SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            return actionData.GetLastState(inputSource);
        }

        /// <summary>Executes a function when the active state of this action (with the specified inputSource) changes. This happens when the action is bound or unbound</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's active state changes and the corresponding input source</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnActiveChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnActiveChangeListener(functionToCall, inputSource);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnActiveChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnActiveChangeListener(functionToStopCalling, inputSource);
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnChangeListener(functionToCall, inputSource);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnChangeListener(functionToStopCalling, inputSource);
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnUpdateListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.AddOnUpdateListener(functionToCall, inputSource);
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnUpdateListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            CheckSourceUpdatingState(inputSource);

            actionData.RemoveOnUpdateListener(functionToStopCalling, inputSource);
        }
    }

    /// <summary>
    /// Boolean actions are either true or false. There is an onStateUp and onStateDown event for the rising and falling edge.
    /// </summary>
    public class SteamVR_Action_Boolean_Data : SteamVR_Action_In_Data, ISteamVR_Action_Boolean
    {
        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources>> onStateDown = 
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources>>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources>> onStateUp = 
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources>>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, InputDigitalActionData_t> actionData = 
            new Dictionary<SteamVR_Input_Sources, InputDigitalActionData_t>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, InputDigitalActionData_t> lastActionData = 
            new Dictionary<SteamVR_Input_Sources, InputDigitalActionData_t>(new SteamVR_Input_Sources_Comparer());

        protected InputDigitalActionData_t tempActionData = new InputDigitalActionData_t();

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool>> onActiveChange = 
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool>>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool>> onChange = 
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool>>(new SteamVR_Input_Sources_Comparer());

        protected Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool>> onUpdate = 
            new Dictionary<SteamVR_Input_Sources, Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool>>(new SteamVR_Input_Sources_Comparer());

        protected uint actionData_size = 0;

        protected SteamVR_Action_Boolean booleanAction;

        public SteamVR_Action_Boolean_Data() { }

        public override void Initialize()
        {
            base.Initialize();
            actionData_size = (uint)Marshal.SizeOf(tempActionData);
            booleanAction = (SteamVR_Action_Boolean)wrappingAction;
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        protected override void InitializeDictionaries(SteamVR_Input_Sources source)
        {
            base.InitializeDictionaries(source);

            onStateDown.Add(source, null);
            onStateUp.Add(source, null);
            actionData.Add(source, new InputDigitalActionData_t());
            lastActionData.Add(source, new InputDigitalActionData_t());

            onChange.Add(source, null);
            onActiveChange.Add(source, null);
            onUpdate.Add(source, null);
        }

        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public override void UpdateValue(SteamVR_Input_Sources inputSource)
        {
            lastActionData[inputSource] = actionData[inputSource];
            lastActive[inputSource] = active[inputSource];

            EVRInputError err = OpenVR.Input.GetDigitalActionData(handle, ref tempActionData, actionData_size, SteamVR_Input_Source.GetHandle(inputSource));
            if (err != EVRInputError.None)
                Debug.LogError("<b>[SteamVR]</b> GetDigitalActionData error (" + fullPath + "): " + err.ToString() + " handle: " + handle.ToString());

            actionData[inputSource] = tempActionData;
            changed[inputSource] = tempActionData.bChanged;
            active[inputSource] = tempActionData.bActive;
            activeOrigin[inputSource] = tempActionData.activeOrigin;
            updateTime[inputSource] = tempActionData.fUpdateTime;

            if (changed[inputSource])
                lastChanged[inputSource] = Time.realtimeSinceStartup;


            if (onStateDown[inputSource] != null && GetStateDown(inputSource))
                onStateDown[inputSource].Invoke(booleanAction, inputSource);

            if (onStateUp[inputSource] != null && GetStateUp(inputSource))
                onStateUp[inputSource].Invoke(booleanAction, inputSource);

            if (onChange[inputSource] != null && GetChanged(inputSource))
                onChange[inputSource].Invoke(booleanAction, inputSource, actionData[inputSource].bState);

            if (onUpdate[inputSource] != null)
                onUpdate[inputSource].Invoke(booleanAction, inputSource, actionData[inputSource].bState);

            if (onActiveChange[inputSource] != null && lastActive[inputSource] != active[inputSource])
                onActiveChange[inputSource].Invoke(booleanAction, inputSource, active[inputSource]);
        }

        /// <summary>Returns true if the value of the action has been set to true (from false) in the most recent update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetStateDown(SteamVR_Input_Sources inputSource)
        {
            return actionData[inputSource].bState && actionData[inputSource].bChanged;
        }

        /// <summary>Returns true if the value of the action has been set to false (from true) in the most recent update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetStateUp(SteamVR_Input_Sources inputSource)
        {
            return actionData[inputSource].bState == false && actionData[inputSource].bChanged;
        }

        /// <summary>Returns true if the value of the action is currently true</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetState(SteamVR_Input_Sources inputSource)
        {
            return actionData[inputSource].bState;
        }

        /// <summary>Returns true if the value of the action has been set to true (from false) in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastStateDown(SteamVR_Input_Sources inputSource)
        {
            return lastActionData[inputSource].bState && lastActionData[inputSource].bChanged;
        }

        /// <summary>Returns true if the value of the action has been set to false (from true) in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastStateUp(SteamVR_Input_Sources inputSource)
        {
            return lastActionData[inputSource].bState == false && lastActionData[inputSource].bChanged;
        }

        /// <summary>Returns true if the value of the action was true in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public bool GetLastState(SteamVR_Input_Sources inputSource)
        {
            return lastActionData[inputSource].bState;
        }


        /// <summary>Executes a function when this action's bound state changes</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnActiveChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource)
        {
            onActiveChange[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnActiveChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            onActiveChange[inputSource] -= functionToStopCalling;
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource)
        {
            onChange[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            onChange[inputSource] -= functionToStopCalling;
        }

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void AddOnUpdateListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource)
        {
            onUpdate[inputSource] += functionToCall;
        }

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        public void RemoveOnUpdateListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource)
        {
            onUpdate[inputSource] -= functionToStopCalling;
        }
    }
    /// <summary>
    /// Boolean actions are either true or false. There is an onStateUp and onStateDown event for the rising and falling edge.
    /// </summary>
    public interface ISteamVR_Action_Boolean : ISteamVR_Action_In
    {

        /// <summary>Returns true if the value of the action has been set to true (from false) in the most recent update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetStateDown(SteamVR_Input_Sources inputSource);

        /// <summary>Returns true if the value of the action has been set to false (from true) in the most recent update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetStateUp(SteamVR_Input_Sources inputSource);

        /// <summary>Returns true if the value of the action is currently true</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetState(SteamVR_Input_Sources inputSource);

        /// <summary>Returns true if the value of the action has been set to true (from false) in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetLastStateDown(SteamVR_Input_Sources inputSource);

        /// <summary>Returns true if the value of the action has been set to false (from true) in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetLastStateUp(SteamVR_Input_Sources inputSource);

        /// <summary>Returns true if the value of the action was true in the previous update.</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        bool GetLastState(SteamVR_Input_Sources inputSource);


        /// <summary>Executes a function when this action's bound state changes</summary>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnActiveChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> action, SteamVR_Input_Sources inputSource);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnActiveChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> action, SteamVR_Input_Sources inputSource);

        /// <summary>Executes a function when the state of this action (with the specified inputSource) changes</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> action, SteamVR_Input_Sources inputSource);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive on change events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnChangeListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> action, SteamVR_Input_Sources inputSource);

        /// <summary>Executes a function when the state of this action (with the specified inputSource) is updated.</summary>
        /// <param name="functionToCall">A local function that receives the boolean action who's state has changed, the corresponding input source, and the new value</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void AddOnUpdateListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToCall, SteamVR_Input_Sources inputSource);

        /// <summary>Stops executing the function setup by the corresponding AddListener</summary>
        /// <param name="functionToStopCalling">The local function that you've setup to receive update events</param>
        /// <param name="inputSource">The device you would like to get data from. Any if the action is not device specific.</param>
        void RemoveOnUpdateListener(Action<SteamVR_Action_Boolean, SteamVR_Input_Sources, bool> functionToStopCalling, SteamVR_Input_Sources inputSource);
    }
}