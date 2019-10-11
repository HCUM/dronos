using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

/*
 * The calibration class is used to log a desired amount of positions in the room to a file
 * in order to compensate for shifts in the Vive tracking 
 */
public class CalibrationScript : MonoBehaviour {

    protected List<GameObject> calibrationPoints = new List<GameObject>();
    protected GameObject waypoint;
    protected GameObject rightController;
    private SteamVR_Behaviour_Pose controllerPose;
    protected LogConstants logConstants;

    private String path;

    // only starts up properly if a valid log file name is created
    void Start () {
        waypoint = GameObject.FindGameObjectWithTag("Waypoint");
        rightController = GameObject.FindGameObjectWithTag("RightController");
        controllerPose = rightController.GetComponent<SteamVR_Behaviour_Pose>();

        logConstants = GameObject.FindGameObjectWithTag("TaskID").GetComponent<LogConstants>();
        if (logConstants.routeID == "" || logConstants.taskID == "")
        {
            Debug.LogError("No TaskID provided, also double check participantID!");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            path = "Assets/Logs/Calibration/" + logConstants.getParticipantID() + "_" + SceneManager.GetActiveScene().name + "_Task_" + logConstants.taskID + "_Route_" + logConstants.routeID + ".txt";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            else
            {
                Debug.LogError("Log file already exists with this participant! Check route and task IDs!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

    }
	
	// Update routine handles controller input and writes to file if finished
	void Update () {

        if (SteamVR_Input._default.Squeeze.GetAxis(SteamVR_Input_Sources.RightHand) == 1)
        {
            if (SteamVR_Input._default.Squeeze.GetLastAxis(SteamVR_Input_Sources.RightHand) < 1)
            {
                // add new waypoint
                GameObject tipOfController = rightController.transform.GetChild(0).gameObject;

                // trigger haptic feedback
                controllerPose.hapticSignal.Execute(0f, 0.1f, 160, 0.5f, controllerPose.inputSource);

                GameObject newWaypoint = Instantiate(waypoint, tipOfController.transform.position, Quaternion.identity);
                newWaypoint.name = "Waypoint " + calibrationPoints.Count;
                newWaypoint.tag = "CalibrationPoint";
                Material mat = new Material(Shader.Find("Standard"));
                mat.SetColor("_Color", Color.red);
                newWaypoint.GetComponent<Renderer>().material = mat;
                calibrationPoints.Add(newWaypoint);
            }
        }

        if (SteamVR_Input._default.Teleport.GetState(SteamVR_Input_Sources.RightHand))
        {
            if (!SteamVR_Input._default.Teleport.GetLastState(SteamVR_Input_Sources.RightHand))
            {
                //delete last waypoint
                if (calibrationPoints.Count > 0)
                {
                    int lastObject = calibrationPoints.Count - 1;
                    Destroy(calibrationPoints.ElementAt(lastObject));
                    calibrationPoints.RemoveAt(lastObject);

                    // trigger haptic feedback
                    controllerPose.hapticSignal.Execute(0f, 0.2f, 160, 0.5f, controllerPose.inputSource);
                }
            }
        }

        //accespts to calibration points and writes to file
        if (Input.GetButtonDown("Accept"))
        {
            if(calibrationPoints.Count > 0)
            {

                TextWriter writer = new StreamWriter(path, true);
                DateTime dt = DateTime.Now;
                writer.WriteLine("New Calibration: " + dt.ToString("dd.MM.yyyy HH:mm:ss"));

                foreach (GameObject obj in calibrationPoints)
                {
                    writer.WriteLine(obj.name +", " + rnd(obj.transform.position.x) + ", " + rnd(obj.transform.position.y) + ", " + rnd(obj.transform.position.z));
                }
                writer.WriteLine();
                foreach (GameObject obj in calibrationPoints)
                {
                    Destroy(obj);
                }
                
                writer.Close();
                calibrationPoints.Clear();
            }
        }

    }

    // rounds a float to 3 decimals
    private double rnd(float x)
    {
        return Math.Round(x, 3);
    }
}
