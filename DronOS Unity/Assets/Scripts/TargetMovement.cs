using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/*
 * Can be used to animate target movement like rotations (look at commented code)
 */
public class TargetMovement : MonoBehaviour {

    public double waitTimeInSeconds;
    public bool landHere;
    public float landHeight;
    [Header("Speed in m/s, max is 0.5")]
    public float speedToThisWaypoint = 0.3f;

    //GameObject target;
    //int maxRotation = 45;
    //float maxMovement = 0.002f;
    //float speed = 1f;


    // Use this for initialization
    void Start () {
        //target = gameObject;

    }
	
	// Update is called once per frame
	void Update () {
        //target.transform.rotation = Quaternion.Euler(0f, maxRotation * Mathf.Sin(Time.time * speed), 0f);


        //Vector3 pos = target.transform.position;
        //pos.y += maxMovement * Mathf.Sin(Time.time * speed);
        //target.transform.position = pos;
        
        
    }
}
