using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScriptUnityManager : BaseWaypointManager {

    public override void setupWaypoints()
    {
        WayPoints.AddRange(GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(go => go.name).ToArray());
        sphereMarker = GameObject.FindGameObjectWithTag("SphereMarker");
        //target = GameObject.FindGameObjectWithTag("Target");
        trackedDrone = GameObject.FindGameObjectWithTag("TrackedDrone");
        gameObject.transform.position = trackedDrone.transform.position;
        dronePilotScript = trackedDrone.GetComponent<ControllerScriptUnity>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
    }

    public override void handleInputAndStates()
    {
        switch (currentDroneState)
        {
            case BaseController.DroneState.Takeoff:
                if (!wasFlying)
                {
                    transform.position = trackedDrone.transform.position;
                    startPosition = trackedDrone.transform.position;
                    wasFlying = true;
                    rawLerp = 0;
                    startTimeLerp = Time.time;
                    startPoint = transform.position;
                    journeyLength = Vector3.Distance(transform.position, WayPoints[nextWaypoint].transform.position);
                    ElapsedTime = 0;
                    startMovingTarget = true;
                }

                flyToNextWaypointDefinedSpeed();

                if (Vector3.Distance(transform.position, WayPoints[nextWaypoint].transform.position) <= triggerDistance)
                {
                    dronePilotScript.SetDroneState(BaseController.DroneState.Flying);
                }
                break;
            case BaseController.DroneState.Flying:
                manageWayPoints();
                flyToNextWaypointDefinedSpeed();
                break;
            case BaseController.DroneState.Landing:
                //if (transform.position == WayPoints[nextWaypoint].transform.position)
                //{
                //    ElapsedTime = 0;
                //    Debug.Log("landinhhahaha");
                //}
                landAtCustomPosition();
                break;
            case BaseController.DroneState.LandingProcedure:
                //if (transform.position == WayPoints[nextWaypoint].transform.position)
                //{
                //    ElapsedTime = 0;
                //    Debug.Log("landinhhahaha");
                //}
                landAtCustomPosition();
                break;
            case BaseController.DroneState.EmergencyOff:
                Vector3 pos = trackedDrone.transform.position;
                pos.y = 0f;
                gameObject.transform.position = pos;
                wasFlying = false;
                break;
            case BaseController.DroneState.Parked:
                Vector3 trackedPosition = trackedDrone.transform.position;
                trackedPosition.y -= 1f;
                gameObject.transform.position = trackedPosition;
                startPosition = trackedDrone.transform.position;
                nextWaypoint = 0;
                wasFlying = false;
                ElapsedTime = 0;
                startHovering = false;
                break;
            case BaseController.DroneState.Hovering:
                // hover on current position;
                if (!startHovering)
                {
                    hoveringPosition = trackedDrone.transform.position;
                    transform.position = hoveringPosition;
                    startHovering = true;
                }
                break;
            default:
                break;
        }

    }

}
