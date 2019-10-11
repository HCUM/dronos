using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Valve.VR;

public class RealtimeManager : BaseWaypointManager {

    [Header("Settings for manual Control")]
    public bool allowDistanceControl = true;
    public bool allowYawControl = true;

    

    public override void setupWaypoints()
    {
        WayPoints.AddRange(GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(go => go.name).ToArray());
        sphereMarker = GameObject.FindGameObjectWithTag("SphereMarker");
        //target = GameObject.FindGameObjectWithTag("Target");
        trackedDrone = GameObject.FindGameObjectWithTag("TrackedDrone");
        gameObject.transform.position = trackedDrone.transform.position;
        dronePilotScript = trackedDrone.GetComponent<ControllerRealtime>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        rightController = GameObject.FindGameObjectWithTag("RightController");
        touchPadAction = SteamVR_Input._default.TouchpadTouch;
    }

    public override void handleInputAndStates()
    {

        waypointAttach = rightController.transform.position + (rightController.transform.forward * distanceToController);
        WayPoints[nextWaypoint].transform.position = waypointAttach;

        Vector2 touchPadValue = touchPadAction.GetAxis(SteamVR_Input_Sources.RightHand);


        if (touchPadValue != Vector2.zero)
        {
            if (allowDistanceControl)
            {
                if (distanceToController <= farLimit && distanceToController >= closeLimit)
                {
                    distanceToController += touchPadValue.y * 0.007f;
                    if (distanceToController > farLimit)
                        distanceToController = farLimit;
                    if (distanceToController < closeLimit)
                        distanceToController = closeLimit;
                }
            }
            if (allowYawControl)
            {
                Quaternion rotation = WayPoints[nextWaypoint].transform.rotation * Quaternion.Euler(0, touchPadValue.x, 0);
                WayPoints[nextWaypoint].transform.rotation = rotation;
            }
        }


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
                    newLandPosition = true;
                }

                flyToNextWaypointSmooth();

                if (Vector3.Distance(transform.position, WayPoints[nextWaypoint].transform.position) <= triggerDistance)
                {
                    dronePilotScript.SetDroneState(BaseController.DroneState.Flying);
                }
                break;
            case BaseController.DroneState.Flying:
                manageWayPoints();
                flyToNextWaypointSmooth();
                break;
            case BaseController.DroneState.Landing:
                //if (transform.position == WayPoints[nextWaypoint].transform.position)
                //{
                //    ElapsedTime = 0;
                //}
                landAtCurrentPosition();
                break;
            case BaseController.DroneState.LandingProcedure:
                //if (transform.position == WayPoints[nextWaypoint].transform.position)
                //{
                //    ElapsedTime = 0;
                //}
                landAtCurrentPosition();
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
                wasFlying = false;
                nextWaypoint = 0;
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
