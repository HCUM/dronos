using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

/*
 * Class used to control the SerialPort
 */
public class SerialCommunication{

    public static SerialPort sp;
    public static string strIn;


    public SerialCommunication(string comPort)
    {
        sp = new SerialPort(comPort, 115200, Parity.Odd, 8, StopBits.One);
        OpenConnection();
    }

    public void WriteSerial(String message)
    {
        sp.Write(message);
    }

    private static void OpenConnection()
    {
        if (sp != null)
        {
            if (sp.IsOpen)
            {
                sp.Close();
                Debug.Log("Closing port, because it was already open!");
            }
            else
            {
                sp.Open();  // opens the connection
                sp.ReadTimeout = 50;  // sets the timeout value before reporting error
                Debug.Log("Port Opened!");
            }
        }
        else
        {
            if (sp.IsOpen)
            {
                Debug.Log("Port is already open");
            }
            else
            {
                Debug.Log("Port == null");
            }
        }
    }

    public void OnQuit()
    {
        sp.Close();
        Debug.Log("Port Closed.");
    }
}
