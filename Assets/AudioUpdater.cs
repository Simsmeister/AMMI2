using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUpdater : MonoBehaviour
{
    private MyListener myListener; // Reference to the MyListener script

    public int heartRate;
    public int heartRateInterval;
    void Start()
    {
        // Find the Cube object and get the MyListener component
        GameObject cubeObject = GameObject.Find("Cube");
        if (cubeObject != null)
        {
            myListener = cubeObject.GetComponent<MyListener>();
        }

        // Check if MyListener component is found
        if (myListener == null)
        {
            Debug.LogError("MyListener script not found on Cube object!");
        }
    }

    void Update()
    {
        // Check if MyListener is not null to avoid errors
        if (myListener != null)
        {
            // Get the heart rate value from MyListener
            heartRate = myListener.heartRateInt;
            heartRateInterval = Mathf.RoundToInt(heartRate / 10.0f) * 10;


            // Calculate the interval for the switch-case based on heart rate
            int interval = (heartRate - 60) / 10;

            // Use a switch-case statement for different intervals
            switch (heartRateInterval)
            {
                case 60:
                    Debug.Log("Heart rate is between 55 and 64!");
                    break;
                case 70:
                    Debug.Log("Heart rate is between 65 and 74!");
                    break;
                case 80:
                    Debug.Log("Heart rate is between 75 and 84!");
                    break;
                case 90:
                    Debug.Log("Heart rate is between 85 and 94!");
                    break;
                case 100:
                    Debug.Log("Heart rate is between 95 and 104!");
                    break;
                case 110:
                    Debug.Log("Heart rate is between 105 and 114!");
                    break;
                case 120:
                    Debug.Log("Heart rate is between 115 and 124!");
                    break;
                case 130:
                    Debug.Log("Heart rate is between 125 and 134!");
                    break;
                case 140:
                    Debug.Log("Heart rate is between 135 and 144!");
                    break;
                case 150:
                    Debug.Log("Heart rate is between 145 and 154!");
                    break;
                    case 160:
                    Debug.Log("Heart rate is between 155 and 164!");
                    break;
                // Add more cases for other intervals

                // You can continue adding cases based on your intervals
                default:
                    // Handle cases beyond the specified range
                    Debug.Log("Heart rate is beyond the specified range!");
                    break;
            }
        }
    }
}
