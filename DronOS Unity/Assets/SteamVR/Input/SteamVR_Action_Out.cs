//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;
using System.Collections;
using System;
using Valve.VR;
using System.Runtime.InteropServices;

namespace Valve.VR
{
    /// <summary>
    /// There is currently only one output type action - vibration. But there may be more in the future.
    /// </summary>
    public abstract class SteamVR_Action_Out<T> : SteamVR_Action<T>, ISteamVR_Action_Out where T : SteamVR_Action_Data, new()
    {
    }

    public abstract class SteamVR_Action_Out_Data : SteamVR_Action_Data
    {
    }

    public interface ISteamVR_Action_Out : ISteamVR_Action
    {
    }
}