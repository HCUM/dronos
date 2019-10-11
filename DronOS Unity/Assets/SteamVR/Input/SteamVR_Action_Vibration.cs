//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using System.Collections;
using System;
using Valve.VR;
using System.Runtime.InteropServices;

namespace Valve.VR
{
    [Serializable]
    /// <summary>
    /// Vibration actions are used to trigger haptic feedback in vr controllers.
    /// </summary>
    public class SteamVR_Action_Vibration : SteamVR_Action_Out<SteamVR_Action_Vibration_Data>, ISteamVR_Action_Vibration
    {
        public SteamVR_Action_Vibration() { }

        /// <summary>
        /// Trigger the haptics at a certain time for a certain length
        /// </summary>
        /// <param name="secondsFromNow">How long from the current time to execute the action (in seconds - can be 0)</param>
        /// <param name="durationSeconds">How long the haptic action should last (in seconds)</param>
        /// <param name="frequency">How often the haptic motor should bounce (0 - 320 in hz. The lower end being more useful)</param>
        /// <param name="amplitude">How intense the haptic action should be (0 - 1)</param>
        /// <param name="inputSource">The device you would like to execute the haptic action. Any if the action is not device specific.</param>
        public void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude, SteamVR_Input_Sources inputSource)
        {
            if (initialized == false)
                Initialize();

            if (handle == 0)
                return;

            actionData.Execute(secondsFromNow, durationSeconds, frequency, amplitude, inputSource);
        }
    }

    /// <summary>
    /// Vibration actions are used to trigger haptic feedback in vr controllers.
    /// </summary>
    public class SteamVR_Action_Vibration_Data : SteamVR_Action_Out_Data, ISteamVR_Action_Vibration
    {
        public SteamVR_Action_Vibration_Data() { }

        /// <summary>
        /// Trigger the haptics at a certain time for a certain length
        /// </summary>
        /// <param name="secondsFromNow">How long from the current time to execute the action (in seconds - can be 0)</param>
        /// <param name="durationSeconds">How long the haptic action should last (in seconds)</param>
        /// <param name="frequency">How often the haptic motor should bounce (0 - 320 in hz. The lower end being more useful)</param>
        /// <param name="amplitude">How intense the haptic action should be (0 - 1)</param>
        /// <param name="inputSource">The device you would like to execute the haptic action. Any if the action is not device specific.</param>
        public void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude, SteamVR_Input_Sources inputSource)
        {
            lastChanged[inputSource] = Time.time;

            EVRInputError err = OpenVR.Input.TriggerHapticVibrationAction(handle, secondsFromNow, durationSeconds, frequency, amplitude, SteamVR_Input_Source.GetHandle(inputSource));

            //Debug.Log(string.Format("[{5}: haptic] secondsFromNow({0}), durationSeconds({1}), frequency({2}), amplitude({3}), inputSource({4})", secondsFromNow, durationSeconds, frequency, amplitude, inputSource, this.GetShortName()));

            if (err != EVRInputError.None)
                Debug.LogError("<b>[SteamVR]</b> TriggerHapticVibrationAction (" + fullPath + ") error: " + err.ToString() + " handle: " + handle.ToString());
        }
    }

    /// <summary>
    /// Vibration actions are used to trigger haptic feedback in vr controllers.
    /// </summary>
    public interface ISteamVR_Action_Vibration : ISteamVR_Action_Out
    {
        /// <summary>
        /// Trigger the haptics at a certain time for a certain length
        /// </summary>
        /// <param name="secondsFromNow">How long from the current time to execute the action (in seconds - can be 0)</param>
        /// <param name="durationSeconds">How long the haptic action should last (in seconds)</param>
        /// <param name="frequency">How often the haptic motor should bounce (0 - 320 in hz. The lower end being more useful)</param>
        /// <param name="amplitude">How intense the haptic action should be (0 - 1)</param>
        /// <param name="inputSource">The device you would like to execute the haptic action. Any if the action is not device specific.</param>
        void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude, SteamVR_Input_Sources inputSource);
    }
}