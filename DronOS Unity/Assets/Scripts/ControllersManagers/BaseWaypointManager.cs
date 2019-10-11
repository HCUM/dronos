using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Valve.VR;
using System.IO;
using UnityEngine.SceneManagement;

/*
 * This class handles the target management
 * each modality derives from it
 */
public class BaseWaypointManager : MonoBehaviour {

    //private GameObject target;
    protected List<GameObject> WayPoints = new List<GameObject>();
    protected GameObject waypoint;
    protected GameObject trackedDrone;
    protected GameObject sphereMarker;
    //private float startSpeed = 0.4f;
    private float landingSpeed = 0.3f;
    // Angular speed in radians per sec.
    private float angularSpeed = 0.02f;
    protected float triggerDistance = 0.005f;
    protected float triggerDistanceLanding = 0.30f;
    protected bool wasFlying = false;
    protected Vector3 startPosition = new Vector3(0, 0, 0);
    protected Vector3 landingPosition = new Vector3(0, -200, 0);
    protected float ElapsedTime = 0;
    //float FinishTime = 4f;
    protected int nextWaypoint = 0;
    //private List<Vector3> currentPath = new List<Vector3>();
    //private bool isArmed = false;
    protected BaseController.DroneState currentDroneState;
    protected BaseController.ScriptState currentScriptState;
    protected BaseController dronePilotScript;
    private bool hasNotStartedWaiting = true;
    private float startTime = 0;
    private bool haveToWait = false;
    protected float rawLerp;
    private float lerp;
    public bool drawPath;
    List<Vector3> targetPathPositions = new List<Vector3>();
    protected LineRenderer lineRenderer;
    protected float startTimeLerp;
    protected Vector3 startPoint;
    protected float journeyLength;
    //private float lerpSpeedMovement = 0.11f;
    protected bool startMovingTarget = true;
    private float landingHeightCorrection = 0.00f;
    protected GameObject rightController;
    protected SteamVR_Behaviour_Pose controllerPose;
    protected Vector3 waypointAttach;
    protected SteamVR_Action_Vector2 touchPadAction;
    protected float distanceToController = 1f;
    TextWriter writer;
    List<String> logList = new List<string>();

    protected Vector3 hoveringPosition;
    protected bool startHovering = false;
    protected bool newLandPosition = false;

    protected LogConstants logConstants;


    protected float farLimit = 1.8f;
    protected float closeLimit = 1f;

    protected bool changedColor = true;

    private String path;
    private bool printedParkedLog = false;

    private bool dontLog = false;

    // Use this for initialization
    void Start () {
        setupWaypoints();


        }

    void Awake()
    {
        setupLoggers();
    }

    // setup loggers and cancel if no uniqe id is given
    private void setupLoggers()
    {
        logConstants = GameObject.FindGameObjectWithTag("TaskID").GetComponent<LogConstants>();
        if (logConstants.routeID == "" || logConstants.taskID == "")
        {
            dontLog = true;
            Debug.LogError("No TaskID provided, also double check participantID!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            path = "Assets/Logs/Study/" + logConstants.getParticipantID() + "_" + SceneManager.GetActiveScene().name + "_Task_" + logConstants.taskID + "_Route_" + logConstants.routeID + ".txt";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            else
            {
                dontLog = true;
                Debug.LogError("Log file already exists with this participant! Check route and task IDs!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        getControllerStates();
        handleInputAndStates();
        drawPaths();

        logPositionsAndData();
    }

    // log position and other data each frame if the drone is not in parked state
    private void logPositionsAndData()
    {
        if (currentDroneState != BaseController.DroneState.Parked && currentDroneState != BaseController.DroneState.EmergencyOff)
        {
            DateTime dt = DateTime.Now;
            Vector3 velDrone = dronePilotScript.getVelocityDrone();
            Vector3 velTarget = dronePilotScript.getVelocityTarget();
            String tracking = "";
            if (dronePilotScript.getTrackingStatus())
            {
                //tracking is fine
                tracking = "Tracking is fine";
            }
            else
            {
                //tracking is bad
                tracking = "No tracking!";
            }

            logList.Add(dt.ToString("dd.MM.yyyy, HH:mm:ss:fff, ") + currentDroneState + ", " + rnd(trackedDrone.transform.position.x) + ", " + rnd(trackedDrone.transform.position.y) + ", " +
                rnd(trackedDrone.transform.position.z) + ", " + rnd(transform.position.x) + ", " + rnd(transform.position.y) + ", " + rnd(transform.position.z) + ", " + rnd(velDrone.x) + ", " + rnd(velDrone.y)
                 + ", " + rnd(velDrone.z) + ", " + rnd(velTarget.x) + ", " + rnd(velTarget.y) + ", " + rnd(velTarget.z) + ", " + tracking + "\r\n");

            //writer.WriteLine(dt.ToString("dd.MM.yyyy, HH:mm:ss:fff, ") + currentDroneState + ", " + trackedDrone.transform.position.x + ", " + trackedDrone.transform.position.y + ", " +
            //    trackedDrone.transform.position.z + ", " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z + ", " + velDrone.x + ", " + velDrone.y
            //     + ", " + velDrone.z + ", " + velTarget.x + ", " + velTarget.y + ", " + velTarget.z + ", " + tracking);

            printedParkedLog = false;
        }
        else
        {
            if (!printedParkedLog)
            {
                DateTime dt = DateTime.Now;
                logList.Add("\r\n");
                logList.Add("\r\n");
                logList.Add(dt.ToString("dd.MM.yyyy, HH:mm:ss:fff, ") + currentDroneState);
                logList.Add("\r\n");
                logList.Add("\r\n");
                //writer.WriteLine();
                //writer.WriteLine();
                //writer.WriteLine();
                //writer.WriteLine();
                //writer.WriteLine(dt.ToString("dd.MM.yyyy, HH:mm:ss:fff, ") + currentDroneState);
                //writer.WriteLine();
                //writer.Close();
                printedParkedLog = true;
            }
        }
    }

    private double rnd(float x)
    {
        return Math.Round(x, 3);
    }

    private void OnApplicationQuit()
    {
        if (!dontLog) {
            String textToLog = "";

            String scene = "Condition: " + SceneManager.GetActiveScene().name;
            textToLog = textToLog + scene + "\r\n";
            textToLog = textToLog + "User Waypoints: " + "\r\n";
            foreach (GameObject obj in WayPoints)
            {
                textToLog = textToLog + obj.transform.position.x + ", " + obj.transform.position.y + ", " + obj.transform.position.z + "\r\n";

            }
            foreach (String str in logList)
            {
                textToLog = textToLog + str;
            }
            textToLog = textToLog + "\r\n";
            textToLog = textToLog + "\r\n";
            textToLog = textToLog + "\r\n";
            File.AppendAllText(path, textToLog);
        }
    }

    private void drawPaths()
    {
        if (drawPath)
        {
            targetPathPositions.Add(gameObject.transform.position);
            Material mat = new Material(Shader.Find("Standard"));
            mat.SetColor("_Color", Color.blue);
            lineRenderer.material = mat;
            //lineRenderer.material = new Material(Shader.Find("Particles/AlphaBlendedShader"));
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;

            //Change how mant points based on the mount of positions is the List
            lineRenderer.positionCount = targetPathPositions.Count;

            for (int i = 0; i < targetPathPositions.Count; i++)
            {
                //Change the postion of the lines
                lineRenderer.SetPosition(i, targetPathPositions[i]);
            }
        }
    }

    public virtual void handleInputAndStates()
    {
        
    }

    private void getControllerStates()
    {
        currentDroneState = dronePilotScript.GetDroneState();
        currentScriptState = dronePilotScript.GetScriptState();
    }

    public virtual void setupWaypoints()
    {
      
    }

    // landing at the current position of the drone
    protected void landAtCurrentPosition()
    {
        if (landingPosition == new Vector3(0, -200, 0) || newLandPosition)
        {
            landingPosition = trackedDrone.transform.position;
            landingPosition.y = 0;
            landingPosition.y -= landingHeightCorrection;
            newLandPosition = false;
        }

        ElapsedTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, landingPosition, Time.deltaTime * landingSpeed);

        Vector3 droneY = trackedDrone.transform.position;
        droneY.z = 0;
        droneY.x = 0;
        Vector3 landingY = landingPosition;
        landingY.z = 0;
        landingY.x = 0;
        if (Vector3.Distance(droneY, landingY) <= triggerDistanceLanding)
        {
            dronePilotScript.SetDroneState(BaseController.DroneState.LandingProcedure);
        }

        if (Vector3.Distance(transform.position, landingPosition) <= triggerDistance)
        {
            dronePilotScript.SetDroneState(BaseController.DroneState.Parked);
        }
    }

    // landing at a custom position
    protected void landAtCustomPosition()
    {
        if (landingPosition == new Vector3(0, -200, 0))
        {
            landingPosition = startPosition;
            landingPosition.y -= landingHeightCorrection;
        }

        ElapsedTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, landingPosition, Time.deltaTime * landingSpeed);

        Vector3 droneY = trackedDrone.transform.position;
        droneY.z = 0;
        droneY.x = 0;
        Vector3 landingY = landingPosition;
        landingY.z = 0;
        landingY.x = 0;
        if (Vector3.Distance(droneY, landingY) <= triggerDistanceLanding)
        {
            dronePilotScript.SetDroneState(BaseController.DroneState.LandingProcedure);
        }

        if (Vector3.Distance(transform.position, landingPosition) <= triggerDistance)
        {
            dronePilotScript.SetDroneState(BaseController.DroneState.Parked);
        }
    }

    protected void flyToNextWaypointDirect()
    {
        transform.position = WayPoints[nextWaypoint].transform.position;
        transform.rotation = WayPoints[nextWaypoint].transform.rotation;
    }

    // move the target to next waypoint with lerp 
    protected void flyToNextWaypointDefinedSpeed()
    {
        if (startMovingTarget)
        {

            journeyLength = Vector3.Distance(transform.position, WayPoints[nextWaypoint].transform.position);
            float timeNeeded = journeyLength / WayPoints[nextWaypoint].GetComponent<TargetMovement>().speedToThisWaypoint;
            StartCoroutine(MoveOverSeconds(gameObject, WayPoints[nextWaypoint].transform.position, timeNeeded));
            startMovingTarget = false;
        }


        //rotate towards target
        rawLerp += Time.deltaTime * angularSpeed;
        lerp = Mathf.Min(rawLerp, 1);
        transform.rotation = Quaternion.Slerp(transform.rotation, WayPoints[nextWaypoint].transform.rotation, lerp);
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            if (currentDroneState == BaseController.DroneState.EmergencyOff || currentDroneState == BaseController.DroneState.Landing || currentDroneState == BaseController.DroneState.Hovering)
            {
                break;
            }
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, Mathf.SmoothStep(0.0f, 1f, elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (!(currentDroneState == BaseController.DroneState.EmergencyOff || currentDroneState == BaseController.DroneState.Landing || currentDroneState == BaseController.DroneState.Hovering))
        {
            objectToMove.transform.position = end;
        }
    }

    // move target to next waypoint smooth
    protected void flyToNextWaypointSmooth()
    {
        ElapsedTime += Time.deltaTime;
        // move towards the target

        // Distance moved = time * speed.
        //float distCovered = (Time.time - startTimeLerp) * lerpSpeedMovement;
        float distCovered = (Time.time - startTimeLerp) * WayPoints[nextWaypoint].GetComponent<TargetMovement>().speedToThisWaypoint;

        // Fraction of journey completed = current distance divided by total distance.
        float fracJourney = distCovered / journeyLength;



        transform.position = Vector3.Lerp(startPoint, WayPoints[nextWaypoint].transform.position, Mathf.SmoothStep(0.0f, 1.0f, fracJourney));
        //transform.position = Vector3.SmoothDamp(transform.position, WayPoints[nextWaypoint].transform.position, ref velocity, smoothTime, maxSpeed);
        //transform.position = Vector3.Lerp(startPos, WayPoints[nextWaypoint].transform.position, fracJourney);
        //transform.position = Vector3.MoveTowards(transform.position, WayPoints[nextWaypoint].transform.position, Time.deltaTime * startSpeed);

        //rotate towards target
        rawLerp += Time.deltaTime * angularSpeed;
        lerp = Mathf.Min(rawLerp, 1);
        transform.rotation = Quaternion.Slerp(transform.rotation, WayPoints[nextWaypoint].transform.rotation, lerp);
    }

    protected void manageWayPoints()
    {
        if (Vector3.Distance(transform.position, WayPoints[nextWaypoint].transform.position) <= triggerDistance)
        {
            //is at next waypoint
            if (WayPoints[nextWaypoint].GetComponent<TargetMovement>().waitTimeInSeconds > 0)
            {
                //wait some time at waitpoint
                if (hasNotStartedWaiting)
                {
                    hasNotStartedWaiting = false;
                    startTime = Time.time;
                    haveToWait = true;
                }
                if (Time.time - startTime >= WayPoints[nextWaypoint].GetComponent<TargetMovement>().waitTimeInSeconds)
                {
                    //enough time elapsed
                    haveToWait = false;
                    hasNotStartedWaiting = true;
                }
            }

            if (!haveToWait)
            {
                //continue to next waypoint
                if (nextWaypoint + 1 < WayPoints.Count)
                {
                    // next waypoint exists
                    nextWaypoint += 1;
                    rawLerp = 0;
                    startTimeLerp = Time.time;
                    startPoint = transform.position;
                    journeyLength = Vector3.Distance(transform.position, WayPoints[nextWaypoint].transform.position);
                    ElapsedTime = 0;
                    startMovingTarget = true;
                }
                else
                {
                    //current was last waypoint -> land
                    //TODO: decide if landing here or startingposition
                    if (WayPoints[nextWaypoint].GetComponent<TargetMovement>().landHere)
                    {
                        //land here
                        landingPosition = WayPoints[nextWaypoint].transform.position;
                        landingPosition.y = WayPoints[nextWaypoint].GetComponent<TargetMovement>().landHeight;
                        landingPosition.y -= landingHeightCorrection;
                        ElapsedTime = 0;
                        dronePilotScript.SetDroneState(BaseController.DroneState.Landing);

                    }
                    else
                    {
                        landingPosition = startPosition;
                        landingPosition.y -= landingHeightCorrection;
                        ElapsedTime = 0;
                        dronePilotScript.SetDroneState(BaseController.DroneState.Landing);
                    }
                }
            }
        }
    }

    public void resetElapsedTime()
    {
        ElapsedTime = 0;
    }


}
