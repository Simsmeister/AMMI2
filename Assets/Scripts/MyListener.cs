/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 * Modifications for InterfaceLab 2020 to move a cube
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class MyListener : MonoBehaviour
{
    GameObject cubeModifier;

    public TMP_Text heartRateText;
    public string textToSearchFor = "Heart";

    public int heartRateInt;

    
    
    void Start() // Start is called before the first frame update
    {
        cubeModifier = GameObject.Find("Cube");
    }
void Update() // Update is called once per frame
    {
    }
void OnMessageArrived(string msg)
    {
        Debug.Log("moving at speed: " + msg);

        // Check if the msg contains "Heartrate:"
        if (msg.Contains("Heartrate:"))
        {
            // Remove "Heartrate:" from the string
            msg = msg.Replace("Heartrate:", "");

            // Trim any leading or trailing whitespace
            msg = msg.Trim();

            heartRateText.text = msg;

            heartRateInt = int.Parse(msg);
        }

        // Convert the modified msg to a float
        float speed = float.Parse(msg) * 100;
        cubeModifier.gameObject.transform.Translate(Vector3.up * Time.deltaTime * speed);

        // Set the heartRateText with the modified msg
        /*if(50 < msg < 180)
        {
            heartRateText.text = msg;
            Debug.Log("Found HeartRate!");
        }*/
    }
    void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }
}

