using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

public class ScriptViveManager : BaseWaypointManager {

    [Header("Land at last Waypoint or at start position")]
    public bool landAtLastWaypoint;

    public override void setupWaypoints()
    {
        //WayPoints.AddRange(GameObject.FindGameObjectsWithTag("Waypoint").OrderBy(go => go.name).ToArray());
        waypoint = GameObject.FindGameObjectWithTag("Waypoint");
        //target = GameObject.FindGameObjectWithTag("Target");
        trackedDrone = GameObject.FindGameObjectWithTag("TrackedDrone");
        gameObject.transform.position = trackedDrone.transform.position;
        dronePilotScript = trackedDrone.GetComponent<ControllerScriptVive>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        rightController = GameObject.FindGameObjectWithTag("RightController");
        controllerPose = rightController.GetComponent<SteamVR_Behaviour_Pose>();
    }

    public override void handleInputAndStates()
    {

        if (currentScriptState == BaseController.ScriptState.DefiningRoute)
        {
            if (changedColor)
            {
                //change color of waypoints for active edit mode
                Material mat = new Material(Shader.Find("Standard"));
                mat.SetColor("_Color", Color.red);
                foreach (GameObject obj in WayPoints)
                {
                    obj.GetComponent<Renderer>().material = mat;
                    obj.GetComponent<TargetMovement>().landHere = false;
                    obj.GetComponent<TargetMovement>().landHeight = 0;
                }
                changedColor = false;
            }
            if (SteamVR_Input._default.Squeeze.GetAxis(SteamVR_Input_Sources.RightHand) == 1)
            {
                if (SteamVR_Input._default.Squeeze.GetLastAxis(SteamVR_Input_Sources.RightHand) < 1)
                {
                    // add new waypoint
                    GameObject tipOfController = rightController.transform.GetChild(0).gameObject;

                    // trigger haptic feedback
                    controllerPose.hapticSignal.Execute(0f, 0.1f, 160, 0.5f, controllerPose.inputSource);

                    GameObject newWaypoint = Instantiate(waypoint, tipOfController.transform.position, Quaternion.identity);
                    newWaypoint.transform.rotation = waypoint.transform.rotation;
                    newWaypoint.name = "Waypoint " + (WayPoints.Count + 1);
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.SetColor("_Color", Color.red);
                    newWaypoint.GetComponent<Renderer>().material = mat;
                    WayPoints.Add(newWaypoint);
                }
            }

            if (SteamVR_Input._default.Teleport.GetState(SteamVR_Input_Sources.RightHand))
            {
                if (!SteamVR_Input._default.Teleport.GetLastState(SteamVR_Input_Sources.RightHand))
                {
                    //delete last waypoint
                    if (WayPoints.Count > 0)
                    {
                        int lastObject = WayPoints.Count - 1;
                        Destroy(WayPoints.ElementAt(lastObject));
                        WayPoints.RemoveAt(lastObject);

                        // trigger haptic feedback
                        controllerPose.hapticSignal.Execute(0f, 0.2f, 160, 0.5f, controllerPose.inputSource);
                    }
                }
            }
        }

        if (currentScriptState == BaseController.ScriptState.FlyingRoute)
        {
            if (WayPoints.Count != 0)
            {
                if (!changedColor)
                {
                    //change color of waypoints for active flymode
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.SetColor("_Color", Color.green);
                    foreach (GameObject obj in WayPoints)
                    {
                        obj.GetComponent<Renderer>().material = mat;
                    }
                    changedColor = true;

                    if (landAtLastWaypoint)
                    {
                        WayPoints.ElementAt(WayPoints.Count - 1).GetComponent<TargetMovement>().landHere = true;
                        WayPoints.ElementAt(WayPoints.Count - 1).GetComponent<TargetMovement>().landHeight = WayPoints.ElementAt(WayPoints.Count - 1).transform.position.y;
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

    }
}
