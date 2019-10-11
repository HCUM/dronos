using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScriptVive : BaseController {

    public override void manageInputAndStates()
    {
        if (Input.GetButtonDown("DefineRoute"))
        {
            currentScriptState = ScriptState.DefiningRoute;
            currentDroneState = DroneState.Parked;
        }
        if (Input.GetButtonDown("FlyRoute"))
        {
            currentScriptState = ScriptState.FlyingRoute;
        }

        if (currentScriptState == ScriptState.FlyingRoute)
        {

            if (Input.GetButtonDown("StartFlying"))
            {
                currentDroneState = DroneState.Takeoff;
            }
            if (Input.GetButtonDown("Landing"))
            {
                directTargetScript.resetElapsedTime();
                currentDroneState = DroneState.Landing;
            }
            if (Input.GetButtonDown("EmergencyOff"))
            {
                currentDroneState = DroneState.EmergencyOff;
            }
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
