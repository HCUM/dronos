using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using UnityStandardAssets.CrossPlatformInput;
using Valve.VR;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


/*
 * BaseController Class is the main controller that all other Modality specific controllers derive from
 * This class takes care of PID calculations
 */
public class BaseController : MonoBehaviour
{

    bool calibrationDone = true;

    public String comPort = "COM6";
    private GameObject target;
    protected BaseWaypointManager directTargetScript;
    public static SteamVR_Action_Pose actionPose;
    private SteamVR_Behaviour_Pose leftController;
    TrackedDevicePose_t[] devicePoses;
    uint trackerIndex = 0;
    public Stopwatch timer;
    public bool drawPath = true;

    private bool trackingIsFine = true;

    public AudioClip errorSound;

    private SerialCommunication serialCommunication;
    private System.Object threadLocker = new System.Object();
    int frames;
    //string channelMap = "TAER1234";
    //string channelDescription = "Throttle, Roll, Pitch, Yaw, 1, 2, 3, 4";

    bool isArmed = false;
    int[] values = { 1000, 1500, 1500, 1500, 1000, 1000, 1000, 1000 };
    int[] copyOfValues = { 1000, 1500, 1500, 1500, 1000, 1000, 1000, 1000 };


    // These are used to display how many times per second the loop is being updated
    private int RegularUpdatesPerSecond;
    private int ThreadedUpdatedsPerSecondRadio;
    private int ThreadedUpdatedsPerSecondPID;

    // This is our thread that we'll use to do our super fast update loop with
    private Thread _updateRadioThread;
    private Thread _PIDThread;

    // This keeps tracks of how many times Update is run per second
    private int _updateCount;

    // This is used for keeping track of time
    private float _time;

    int throttlePID = 0;
    int throttlePID_copy = 1000;

    int yawPID = 1500;
    int yawPID_copy = 1500;

    Vector3 targetVector;
    Vector3 currentVector;
    Transform currentForwardTransform;
    Vector3 distanceToTarget;
    //Vector3 currentX;
    Vector3 targetVectorForward;
    Vector3 currentVectorForward;
    Vector3 velocity;
    List<Vector3> dronePathPositions = new List<Vector3>();
    LineRenderer lineRenderer;

    protected bool calcPID = false;
    XZPair xzResult = new XZPair(1500, 1500);
    XZPair xzResult_copy = new XZPair(1500, 1500);

    //velocity PID attitude
    Vector3 velocityTarget;
    Vector3 distanceBetweenLastTargetAndCurrent;
    Vector3 lastPositionTarget;

    public enum DroneState { Takeoff, Flying, Landing, LandingProcedure, EmergencyOff, Parked, TrackingErrorLanding, Hovering };
    public enum ScriptState { DefiningRoute, FlyingRoute };

    protected DroneState currentDroneState;
    protected ScriptState currentScriptState;

    protected TrackedDevicePose_t trackerPose;

    // Use this for initialization
    void Start()
    {

        serialCommunication = new SerialCommunication(comPort);
        target = GameObject.FindGameObjectWithTag("DirectTarget");
        directTargetScript = target.GetComponent<BaseWaypointManager>();
        // Start the PID thread 
        _PIDThread = new Thread(PIDLoop);
        _PIDThread.Start();
        // send commands to serial port thread
        _updateRadioThread = new Thread(SuperFastLoop);
        _updateRadioThread.Start();

        timer = new Stopwatch();

        //behaviourScript = gameObject.GetComponent<SteamVR_Behaviour_Pose>();


        _time = Time.time;

        currentDroneState = DroneState.Parked;
        currentScriptState = ScriptState.DefiningRoute;

        lineRenderer = gameObject.AddComponent<LineRenderer>();

        lastPositionTarget = target.transform.position;
        leftController = GameObject.FindGameObjectWithTag("LeftController").GetComponent<SteamVR_Behaviour_Pose>();

    }

    // Update Routine
    void Update()
    {
        checkTrackerStatus();
        drawPathWithLineRenderer();

        // this calibration sends out Stick commands to the drone in order to 
        // calibrate the gyroscope and accelerometer of the flight controller
        if (!calibrationDone)
        {
            calibrate();
        }
        else
        {
            manageInputAndStates();
            handleEmergencySwitches();
        }

        reportUpdateTime();
    }

    // Left hand controller is the controller of the study supervisor, 
    // Trigger leads to hold position hover of the drone
    // touchpad press leads to the drone instantly shutting off motors
    private void handleEmergencySwitches()
    {
        if (SteamVR_Input._default.Squeeze.GetAxis(SteamVR_Input_Sources.LeftHand) == 1)
        {
            if (SteamVR_Input._default.Squeeze.GetLastAxis(SteamVR_Input_Sources.LeftHand) < 1)
            {
                Debug.LogError("Emergency Hover");
                currentDroneState = DroneState.Hovering;
            }
        }

        if (SteamVR_Input._default.Teleport.GetState(SteamVR_Input_Sources.LeftHand))
        {
            currentDroneState = DroneState.EmergencyOff;
        }
    }

    public void flyDrone()
    {
        if (!trackerPose.bPoseIsValid || !trackerPose.bDeviceIsConnected || trackerPose.eTrackingResult != ETrackingResult.Running_OK)
        {
            // tracker tracking problems!
            trackingIsFine = false;
            playTrackingProblemSound();
        }
        else
        {
            // no  tracking problems

            //check if left controller is active for emergency off

            if (leftController.isActive)
            {
                trackingIsFine = true;
                calculatePIDandPositions();
            }
            else
            {
                Debug.LogError("Left Controller INACIVE, both controllers neet to be enabled and bound!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }

    // emergency "soft" landing when tracking is lost
    public void trackingErrorLandNow()
    {
        if (throttlePID > 1004)
        {
            throttlePID -= 4;
            values[0] = throttlePID;
        }
        else
        {
            currentDroneState = DroneState.EmergencyOff;
        }

    }

    // tracking is fine, calculate PIDs and position
    private void calculatePIDandPositions()
    {
        if (SoundManager.instance.musicSource.isPlaying)
            SoundManager.instance.musicSource.Stop();

        targetVector = target.transform.position;
        currentVector = gameObject.transform.position;
        //currentX = gameObject.transform.right;
        currentForwardTransform = gameObject.transform;
        targetVectorForward = target.transform.forward;
        currentVectorForward = currentForwardTransform.forward;

        distanceToTarget = gameObject.transform.InverseTransformDirection(targetVector - currentVector);
        distanceBetweenLastTargetAndCurrent = gameObject.transform.InverseTransformDirection(target.transform.position - lastPositionTarget);
        velocityTarget = distanceBetweenLastTargetAndCurrent / Time.deltaTime;
        lastPositionTarget = target.transform.position;

        //Debug.Log(gameObject.transform.position);

        calcPID = true;
        if (currentDroneState != DroneState.LandingProcedure)
        {
            throttlePID = throttlePID_copy;
            values[0] = throttlePID;
        }
        else
        {
            if (throttlePID > 1004)
            {
                throttlePID -= 4;
                values[0] = throttlePID;
            }
        }
        xzResult = xzResult_copy;
        values[1] = Convert.ToInt32(xzResult.roll);
        values[2] = Convert.ToInt32(xzResult.pitch);
        yawPID = yawPID_copy;
        values[3] = yawPID;

        if (timer.IsRunning)
        {
            timer.Stop();
            Debug.LogError("Lost tracking for " + timer.ElapsedMilliseconds + "ms");
        }
    }

    // play tracking error sound
    private void playTrackingProblemSound()
    {
        if (!SoundManager.instance.musicSource.isPlaying)
            SoundManager.instance.musicSource.Play();
        // tracker tracking problems!


        if (!trackerPose.bDeviceIsConnected)
        {
            Debug.LogError("Device NOT connected");
        }
        else
        {
            if (!trackerPose.bPoseIsValid)
                Debug.LogError("bPoseNOTValid");
        }
        //Debug.LogError(trackerPose.eTrackingResult);

        if (!timer.IsRunning)
        {
            timer.Start();
        }
        else
        {
            if (timer.ElapsedMilliseconds > 100)
            {
                currentDroneState = DroneState.TrackingErrorLanding;
            }
        }
    }

    private void reportUpdateTime()
    {
        // One second has occured
        if (Time.time - _time >= 1f)
        {
            RegularUpdatesPerSecond = _updateCount;
            _updateCount = 0;
            _time = Time.time;
        }
        _updateCount++;
    }

    public virtual void manageInputAndStates()
    {

    }

    // calibrates gyro and acc on flight controller
    private void calibrate()
    {
        if (Time.time < 2)
        {

            calibrateGyro();
        }
        else if (Time.time < 4)
        {

            resetValues();
        }
        else if (Time.time < 6)
            calibrateAcc();
        else if (Time.time < 8)
            resetValues();
        else
        {
            calibrationDone = true;
            Debug.Log("Calibration done!");
        }
    }

    // draws path of the drone
    private void drawPathWithLineRenderer()
    {
        if (drawPath)
        {
            dronePathPositions.Add(gameObject.transform.position);
            Material mat = new Material(Shader.Find("Standard"));
            mat.SetColor("_Color", Color.red);
            lineRenderer.material = mat;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;

            //Change how mant points based on the mount of positions is the List
            lineRenderer.positionCount = dronePathPositions.Count;

            for (int i = 0; i < dronePathPositions.Count; i++)
            {
                //Change the postion of the lines
                lineRenderer.SetPosition(i, dronePathPositions[i]);
            }
        }
    }

    // check status of Vive tracker 
    private void checkTrackerStatus()
    {
        if (actionPose == null)
        {
            //Debug.LogError("Action pose is null");
            if (SteamVR_Input._default.VRTracker.GetActive(SteamVR_Input_Sources.Any))
            {
                actionPose = SteamVR_Input._default.VRTracker;
                Debug.LogError(SteamVR_Input._default.VRTracker.GetLocalPosition(SteamVR_Input_Sources.Any));
            }
            else
            {
                return;
            }
        }

        if (OpenVR.System == null)
            return;

        trackerIndex = actionPose.GetDeviceIndex(SteamVR_Input_Sources.Any);
        //Debug.Log("isactive: " + behaviourScript.inputSource);
        //Debug.LogError("Tracker not initialized");
        gameObject.transform.position = actionPose.GetLocalPosition(SteamVR_Input_Sources.Any);
        gameObject.transform.rotation = actionPose.GetLocalRotation(SteamVR_Input_Sources.Any) * Quaternion.Euler(-90, 90, 90);

        velocity = actionPose.GetVelocity(SteamVR_Input_Sources.Any);

        devicePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        OpenVR.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0.0f, devicePoses);
        trackerPose = devicePoses[trackerIndex];
    }

    public struct XZPair
    {
        public double roll, pitch;

        public XZPair(double p1, double p2)
        {
            roll = p1;
            pitch = p2;
        }
    }

    // PID Values
    double error_prior = 0;
    double integral = 0;

    [Header("Throttle PIDs")]
    public double KP_throttle = 0.66;    // 2    // 1.5      //0.6                   0.6
    public double KI_throttle = 0.48;    // 2    // 1.5      //1.8                   1.8
    public double KD_throttle = 0.5;    // 1.1  // 1.1 best //1.4 new best tuning   1.4
    double feedForwardHover = 1450; // needs to be evaluated for a different drone / weight setup
    double output = 0;
    int upperLimit = 1550; // may be necessary to be adapted to more weight/other battery. theoretical max is 2000
    int lowerLimit = 1000;

    int integralUpperLimit = 150;
    int integralLowerLimit = -150;
    //int N = 100;
    //int FilterCoefficient = 0;
    //int Filter_DSTATE = 0;
    double smoothingFactorThrottle = 0.1;
    double lastDerivativeFilteredThrottle = 0;
    double derivativeFilteredThrottle = 0;

    // calculates the PID of throttle
    private int calculateThrottlePID(double deltaTime)
    {
        float error = (targetVector.y - currentVector.y) * 100;

        //Debug.Log(currentVector.y);
        //float error = (target.transform.position.y - gameObject.transform.position.y) * 100;
        integral = integral + (error * deltaTime);
        if (integral >= integralUpperLimit)
        {
            integral = integralUpperLimit;
        }
        else if (integral <= integralLowerLimit)
        {
            integral = integralLowerLimit;
        }


        double derivative = (error - error_prior) / deltaTime;

        derivativeFilteredThrottle = (1 - smoothingFactorThrottle) * lastDerivativeFilteredThrottle + smoothingFactorThrottle * derivative;
        lastDerivativeFilteredThrottle = derivativeFilteredThrottle;

        output = KP_throttle * error + KI_throttle * integral + KD_throttle * derivativeFilteredThrottle + feedForwardHover;
        error_prior = error;

        if (output > upperLimit)
            output = upperLimit;
        if (output < lowerLimit)
            output = lowerLimit;

        String throttle = "";

        for (int i = 0; i < output; i += 20)
        {
            throttle += "|";

        }


        //Debug.Log("Error: " + error + ", Prop: " + KP * error + ", Integral: "  + KI * integral + ", Derivative: " + KD * derivativeFiltered + ", "+ throttle);

        return Convert.ToInt32(output);
    }


    [Space(10)]
    XZPair errorPriorRP = new XZPair(0, 0);
    [Header("Roll PIDs")]
    public double KP_Roll = 1;     //1.5      // 3    //0.9                       1.5
    public double KI_Roll = 0.4;     //0.1    // 0.5    //0.1                       0.8
    public double KD_Roll = 3;       //4      // 6 best //4 newest values           4

    [Header("Pitch PIDs")]
    public double KP_Pitch = 1;     //1.5      // 3      //0.9                    1.5
    public double KI_Pitch = 0.4;     //0.1    // 0.5       //0.1                   0.8
    public double KD_Pitch = 3;       //4      // 6 best    //4 newest values       4

    double hoverState = 1500;
    double integralRoll = 0;
    double integralPitch = 0;
    double integralUpperLimitRollPitch = 100;
    double integralLowerLimitRollPitch = -100;
    double derivativeUpperLimitRollPitch = 200;
    double derivativeLowerLimitRollPitch = -200;
    double upperLimitRollPitch = 1600;
    double lowerLimitRollPitch = 1400;
    double smoothingFactorRP = 0.1;
    double lastDerivativeFilteredRoll = 0;
    double lastDerivativeFilteredPitch = 0;
    double derivativeFilteredRoll = 0;
    double derivativeFilteredPitch = 0;

    // calculates PID of pitch and roll
    private XZPair calculateRollPitchPID(double deltaTime)
    {
        //calc error to body frame
        XZPair errorToBodyFrame = calcErrorToBodyFrame(currentVector, currentVectorForward, targetVector);

        //calc pitch and roll commands
        integralRoll = integralRoll + (errorToBodyFrame.roll * deltaTime);
        if (integralRoll >= integralUpperLimitRollPitch)
        {
            integralRoll = integralUpperLimitRollPitch;
        }
        else if (integralRoll <= integralLowerLimitRollPitch)
        {
            integralRoll = integralLowerLimitRollPitch;
        }
        integralPitch = integralPitch + (errorToBodyFrame.pitch * deltaTime);
        if (integralPitch >= integralUpperLimitRollPitch)
        {
            integralPitch = integralUpperLimitRollPitch;
        }
        else if (integralPitch <= integralLowerLimitRollPitch)
        {
            integralPitch = integralLowerLimitRollPitch;
        }

        double derivativeRoll = (errorToBodyFrame.roll - errorPriorRP.roll) / deltaTime;
        double derivativePitch = (errorToBodyFrame.pitch - errorPriorRP.pitch) / deltaTime;

        derivativeFilteredRoll = (1 - smoothingFactorRP) * lastDerivativeFilteredRoll + smoothingFactorRP * derivativeRoll;
        if (KD_Roll * derivativeFilteredRoll > derivativeUpperLimitRollPitch)
            derivativeFilteredRoll = derivativeUpperLimitRollPitch / KD_Roll;
        if (KD_Roll * derivativeFilteredRoll < derivativeLowerLimitRollPitch)
            derivativeFilteredRoll = derivativeLowerLimitRollPitch / KD_Roll;
        lastDerivativeFilteredRoll = derivativeFilteredRoll;

        derivativeFilteredPitch = (1 - smoothingFactorRP) * lastDerivativeFilteredPitch + smoothingFactorRP * derivativePitch;
        if (KD_Pitch * derivativeFilteredPitch > derivativeUpperLimitRollPitch)
            derivativeFilteredPitch = derivativeUpperLimitRollPitch / KD_Pitch;
        if (KD_Pitch * derivativeFilteredPitch < derivativeLowerLimitRollPitch)
            derivativeFilteredPitch = derivativeLowerLimitRollPitch / KD_Pitch;
        lastDerivativeFilteredPitch = derivativeFilteredPitch;

        double roll = KP_Roll * (errorToBodyFrame.roll - velocity.x) + KI_Roll * integralRoll + KD_Roll * derivativeFilteredRoll + hoverState;
        double pitch = KP_Pitch * (errorToBodyFrame.pitch - velocity.z) + KI_Pitch * integralPitch + KD_Pitch * derivativeFilteredPitch + hoverState;

        errorPriorRP = errorToBodyFrame;

        if (roll > upperLimitRollPitch)
            roll = upperLimitRollPitch;
        if (roll < lowerLimitRollPitch)
            roll = lowerLimitRollPitch;

        if (pitch > upperLimitRollPitch)
            pitch = upperLimitRollPitch;
        if (pitch < lowerLimitRollPitch)
            pitch = lowerLimitRollPitch;

        //Debug.Log("Error Roll: " + errorToBodyFrame.roll + "....Error Pitch: " + errorToBodyFrame.pitch);

        return new XZPair(roll, pitch);

    }

    private XZPair calcErrorToBodyFrame(Vector3 current, Vector3 currentForward, Vector3 target)
    {
        distanceToTarget.y = 0;
        return new XZPair(distanceToTarget.x * 100, distanceToTarget.z * 100); //convert to cm
    }



    private XZPair calculateRPVelocitiesPID(double deltaTime)
    {
        // errorx = target.velocity.x - object.velocity.x;

        distanceBetweenLastTargetAndCurrent.y = 0;


        Vector3 velocityDrone = velocity;

        float velocityErrorX = velocityTarget.x - velocityDrone.x;
        float velocityErrorY = velocityTarget.y - velocityDrone.y;

        Debug.LogError(velocityTarget + ", " + velocityDrone + ", " + velocityErrorX + ", " + velocityErrorY);


        // errory = target.velocity.x - object.velocity.x;



        return new XZPair(0, 0);
    }

    double KP_Yaw = 5;
    double neutralPosition = 1500;
    double yawUpperLimit = 1700;
    double yawLowerLimit = 1300;

    // calculates PID for Yaw
    private int calculateYawPID()
    {
        double errorAngle = Vector3.Angle(currentVectorForward, targetVectorForward);
        Vector3 cross = Vector3.Cross(currentVectorForward, targetVectorForward);
        if (cross.y < 0)
            errorAngle = -errorAngle;

        double result = KP_Yaw * errorAngle + neutralPosition;
        if (result >= yawUpperLimit)
            result = yawUpperLimit;
        if (result <= yawLowerLimit)
            result = yawLowerLimit;

        int output = Convert.ToInt16(result);
        //Debug.Log(output);
        return output;
    }

    // fast running PID loop that is not depending on graphics update of unity
    private void PIDLoop()
    {
        // We can't use Time.time which is a Unity API, instead we'll use this
        var time = System.DateTime.UtcNow.Ticks;
        const int oneSecond = 10000000;
        var count = 0;
        Stopwatch sw = null;
        double deltaTime = 0;


        // This begins our Update loop
        while (true)
        {
            if (sw != null)
            {
                deltaTime = sw.Elapsed.TotalMilliseconds / 1000;
            }
            else
            {
                deltaTime = 0.008;
            }
            sw = Stopwatch.StartNew();
            if (System.DateTime.UtcNow.Ticks - time >= oneSecond)
            {
                ThreadedUpdatedsPerSecondPID = count;
                count = 0;
                time = System.DateTime.UtcNow.Ticks;
            }

            if (calcPID)
            {
                throttlePID_copy = calculateThrottlePID(deltaTime);
                xzResult_copy = calculateRollPitchPID(deltaTime);
                //calculateRPVelocitiesPID(deltaTime);
                yawPID_copy = calculateYawPID();
            }

            count++;
            // This suspends the thread for 11 milliseconds, making this code execute 90 times per second
            Thread.Sleep(11);
            sw.Stop();

        }
    }

    // obstacle collision handling
    void OnCollisionEnter(Collision collision)
    {
        Collider myCollider = collision.contacts[0].thisCollider;

        if (currentDroneState == DroneState.Takeoff || currentDroneState == DroneState.Flying || currentDroneState == DroneState.Landing)
        {
            Debug.LogError(collision.collider.name);
            if (collision.collider.tag == "Obstacle")
            {
                Debug.LogError("Entered no-fly zone, shutting down");
                currentDroneState = DroneState.TrackingErrorLanding;
            }
        }
    }

    public DroneState GetDroneState()
    {
        return currentDroneState;
    }

    public void SetDroneState(DroneState state)
    {
        currentDroneState = state;
    }

    public ScriptState GetScriptState()
    {
        return currentScriptState;
    }

    private void calibrateAcc()
    {
        isArmed = false;
        values[0] = 2000;
        values[1] = 1500;
        values[2] = 1000;
        values[3] = 1000;
    }

    public void resetValues()
    {
        values[0] = 1000;
        values[1] = 1500;
        values[2] = 1500;
        values[3] = 1500;
    }

    private void calibrateGyro()
    {
        isArmed = false;
        values[0] = 1000;
        values[1] = 1500;
        values[2] = 1000;
        values[3] = 1000;
    }


    // thread that generates the serial message and sends it to the serial port 
    // (Serial-to-USB device) -> (ESP32) -> (Taranis) -> Drone
    private void SuperFastLoop()
    {
        // We can't use Time.time which is a Unity API, instead we'll use this
        var time = System.DateTime.UtcNow.Ticks;
        const int oneSecond = 10000000;
        var count = 0;

        // This begins our Update loop
        while (true)
        {
            if (System.DateTime.UtcNow.Ticks - time >= oneSecond)
            {
                ThreadedUpdatedsPerSecondRadio = count;
                count = 0;
                time = System.DateTime.UtcNow.Ticks;
            }


            lock (threadLocker)
            {
                if (isArmed)
                {
                    values[4] = 1800;
                }
                else
                {
                    values[4] = 1000;
                }

                copyOfValues = values;

            }



            //if (isArmed)
            //{
            //    values[4] = 1800;
            //}
            //else
            //{
            //    values[4] = 1004;
            //}

            copyOfValues = values;

            string message = BuildString(copyOfValues);
            serialCommunication.WriteSerial(message);

            count++;

            // This suspends the thread for 5 milliseconds, making this code execute 200 times per second
            Thread.Sleep(25);
        }
    }

    public Vector3 getVelocityDrone()
    {
        return velocity;
    }

    public bool getTrackingStatus()
    {
        return trackingIsFine;
    }

    public Vector3 getVelocityTarget()
    {
        return velocityTarget;
    }

    private string BuildString(int[] values)
    {
        string message = "<";
        foreach (int i in values)
        {
            message += i + ", ";
        }

        message = message.Remove(message.Length - 2);
        message += ">";

        return message;
    }

    private void OnApplicationQuit()
    {
        serialCommunication.OnQuit();
    }


    private void OnDisable()
    {

        // Stop the thread when disabled, or it will keep running in the background
        _updateRadioThread.Abort();
        _PIDThread.Abort();

        // Waits for the Thread to stop
        _updateRadioThread.Join();
        _PIDThread.Join();
    }




}
