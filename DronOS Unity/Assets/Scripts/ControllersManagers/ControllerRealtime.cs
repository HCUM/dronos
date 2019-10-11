using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerRealtime : BaseController {


    public override void manageInputAndStates()
    {

        if (SteamVR_Input._default.Squeeze.GetAxis(SteamVR_Input_Sources.RightHand) == 1)
        {
            if (SteamVR_Input._default.Squeeze.GetLastAxis(SteamVR_Input_Sources.RightHand) < 1)
            {
                if (currentDroneState == DroneState.Takeoff || currentDroneState == DroneState.Flying || currentDroneState == DroneState.Hovering)
                {
                    directTargetScript.resetElapsedTime();
                    currentDroneState = DroneState.Landing;
                }
                else
                {
                    currentDroneState = DroneState.Takeoff;
                }
            }
        }

        //if (SteamVR_Input._default.Teleport.GetState(SteamVR_Input_Sources.RightHand))
        //{
        //    currentDroneState = DroneState.EmergencyOff;
        //    Debug.LogError("Emergency OFf");
        //}
        if (Input.GetButtonDown("EmergencyOff"))
        {
            currentDroneState = DroneState.EmergencyOff;
        }

        //return;
        switch (currentDroneState)
        {
            case DroneState.Takeoff:
                flyDrone();
                break;
            case DroneState.Flying:
                flyDrone();
                break;
            case DroneState.Landing:
                flyDrone();
                break;
            case DroneState.LandingProcedure:
                flyDrone();
                break;
            case DroneState.EmergencyOff:
                calcPID = false;
                resetValues();
                break;
            case DroneState.Parked:
                calcPID = false;
                resetValues();
                break;
            case DroneState.TrackingErrorLanding:
                calcPID = false;
                flyDrone();
                //trackingErrorLandNow();
                // this was commented in the study to prevent unwanted tracking lost aborts
                break;
            case DroneState.Hovering:
                flyDrone();
                break;
            default:
                calcPID = false;
                resetValues();
                break;
        }
    }
}
