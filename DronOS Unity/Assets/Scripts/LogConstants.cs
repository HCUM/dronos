using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used to set participant IDs unique
 */
public class LogConstants : MonoBehaviour {

    private string ParticipantID = "33";
    public string taskID = "";
    public string routeID = "";


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public string getParticipantID()
    {
        return ParticipantID;
    }
}
