using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AudioUpdater : MonoBehaviour
{
    public GameObject cubeObject;
    public MyListener myListener; // Reference to the MyListener script
    private AudioSource audioSource;
    public AudioClip[] heartBeatMusic;
    public int heartRate;
    public int heartRateInterval;
    public int currentInterval;
    public float fadeTime = 2f;
    public float timer = 0f;
    public bool hasPlayedOnce = false;
    public bool timeToCount = false;

    public GameObject spotLightOne;
    public GameObject spotLightTwo;

    public GameObject spotLightChildOne;

    public GameObject spotLightChildTwo;

    public RainbowLight spotLightLightOne;

    public RainbowLight spotLightLightTwo;

    public RainbowLight spotLightLightChildOne;

    public RainbowLight spotLightLightChildTwo;


    //private bool sixtyPlaying = false, seventyPlaying = false, eightyPlaying = false, ninetyPlaying = false, hundredtenPlaying = false;
    void Start()
    {
        // Find the Cube object and get the MyListener component
        cubeObject = GameObject.Find("Cube");
        GameObject camera = GameObject.Find("Main Camera");
        Debug.Log(cubeObject);
        spotLightLightOne = spotLightOne.GetComponent<RainbowLight>();
        spotLightLightTwo = spotLightTwo.GetComponent<RainbowLight>();
        spotLightLightChildOne = spotLightChildOne.GetComponent<RainbowLight>();
        spotLightLightChildTwo = spotLightChildTwo.GetComponent<RainbowLight>();



        if (cubeObject != null)
        {
            myListener = cubeObject.GetComponent<MyListener>();
            Debug.Log("MIN LISTENER" + myListener);
            audioSource = camera.GetComponent<AudioSource>();
        }

        // Check if MyListener component is found
        if (myListener == null)
        {
            Debug.LogError("MyListener script not found on Cube object!");
        }
    }

    void Update()
{
    if (myListener != null)
    {
        heartRate = myListener.heartRateInt;
        heartRateInterval = Mathf.RoundToInt(heartRate / 10.0f) * 10;

        if(timeToCount)
        {
            timer += Time.deltaTime;
        }

        // Set the time interval for the if statement to run (e.g., once per minute)
        float timeInterval = 60f; // 60 seconds = 1 minute

        // Check if a new interval is reached
        if (heartRateInterval != currentInterval && timer >= timeInterval || heartRateInterval != currentInterval && !hasPlayedOnce)
        {
            currentInterval = heartRateInterval;

            // Stop the current audio and initiate the fade-out
            StartCoroutine(FadeOutAndStop(audioSource, fadeTime));

            // Play the appropriate audio clip and initiate the fade-in
            PlayAudioForInterval(currentInterval);
            hasPlayedOnce = true;
            timeToCount = true;
            timeInterval = 0f;
        }

        /*if (heartRateInterval != currentInterval)
        {
            currentInterval = heartRateInterval;

            // Stop the current audio and initiate the fade-out
            StartCoroutine(FadeOutAndStop(audioSource, fadeTime));

            // Play the appropriate audio clip and initiate the fade-in
            PlayAudioForInterval(currentInterval);
            
        }*/


    }
}

void PlayAudioForInterval(int interval)
{
    AudioClip clipToPlay = null;

    switch (interval)
    {
        case 60:
            clipToPlay = heartBeatMusic[0];
            spotLightLightOne.colorChangeSpeed = 0.5f;
            spotLightLightTwo.colorChangeSpeed = 0.5f;
            spotLightLightChildOne.colorChangeSpeed = 0.5f;
            spotLightLightChildTwo.colorChangeSpeed = 0.5f;
            break;
        case 70:
            clipToPlay = heartBeatMusic[1];
            spotLightLightOne.colorChangeSpeed = 1f;
            spotLightLightTwo.colorChangeSpeed = 1f;
            spotLightLightChildOne.colorChangeSpeed = 1f;
            spotLightLightChildTwo.colorChangeSpeed = 1f;
            break;
        case 80:
            clipToPlay = heartBeatMusic[2];
            spotLightLightOne.colorChangeSpeed = 1.5f;
            spotLightLightTwo.colorChangeSpeed = 1.5f;
            spotLightLightChildOne.colorChangeSpeed = 1.5f;
            spotLightLightChildTwo.colorChangeSpeed = 1.5f;
            break;
        case 90:
            clipToPlay = heartBeatMusic[3];
            spotLightLightOne.colorChangeSpeed = 2f;
            spotLightLightTwo.colorChangeSpeed = 2f;
            spotLightLightChildOne.colorChangeSpeed = 2f;
            spotLightLightChildTwo.colorChangeSpeed = 2f;
            break;
        case 100:
            clipToPlay = heartBeatMusic[4];
            spotLightLightOne.colorChangeSpeed = 2.5f;
            spotLightLightTwo.colorChangeSpeed = 2.5f;
            spotLightLightChildOne.colorChangeSpeed = 2.5f;
            spotLightLightChildTwo.colorChangeSpeed = 2.5f;
            break;
        case 110:
            clipToPlay = heartBeatMusic[5];
            spotLightLightOne.colorChangeSpeed = 3f;
            spotLightLightTwo.colorChangeSpeed = 3f;
            spotLightLightChildOne.colorChangeSpeed = 3f;
            spotLightLightChildTwo.colorChangeSpeed = 3f;
            break;
        case 120:
            clipToPlay = heartBeatMusic[6];
            spotLightLightOne.colorChangeSpeed = 3.5f;
            spotLightLightTwo.colorChangeSpeed = 3.5f;
            spotLightLightChildOne.colorChangeSpeed = 3.5f;
            spotLightLightChildTwo.colorChangeSpeed = 3.5f;
            break;
        case 130:
            clipToPlay = heartBeatMusic[7];
            spotLightLightOne.colorChangeSpeed = 4f;
            spotLightLightTwo.colorChangeSpeed = 4f;
            spotLightLightChildOne.colorChangeSpeed = 4f;
            spotLightLightChildTwo.colorChangeSpeed = 4f;
            break;
        case 140:
            clipToPlay = heartBeatMusic[8];
            spotLightLightOne.colorChangeSpeed = 4.5f;
            spotLightLightTwo.colorChangeSpeed = 4.5f;
            spotLightLightChildOne.colorChangeSpeed = 4.5f;
            spotLightLightChildTwo.colorChangeSpeed = 4.5f;
            break;
        case 150:
            clipToPlay = heartBeatMusic[9];
            spotLightLightOne.colorChangeSpeed = 5f;
            spotLightLightTwo.colorChangeSpeed = 5f;
            spotLightLightChildOne.colorChangeSpeed = 5f;
            spotLightLightChildTwo.colorChangeSpeed = 5f;
            break;
        case 160:
            clipToPlay = heartBeatMusic[10];
            spotLightLightOne.colorChangeSpeed = 5.5f;
            spotLightLightTwo.colorChangeSpeed = 5.5f;
            spotLightLightChildOne.colorChangeSpeed = 5.5f;
            spotLightLightChildTwo.colorChangeSpeed = 5.5f;
            break;
        case 170:
            clipToPlay = heartBeatMusic[11];
            spotLightLightOne.colorChangeSpeed = 6f;
            spotLightLightTwo.colorChangeSpeed = 6f;
            spotLightLightChildOne.colorChangeSpeed = 6f;
            spotLightLightChildTwo.colorChangeSpeed = 6f;
            break;
        
        // Add other cases as needed
    }

    if (clipToPlay != null)
    {
        audioSource.clip = clipToPlay;
        StartCoroutine(FadeIn(audioSource, fadeTime));
    }
}

IEnumerator FadeOutAndStop(AudioSource audioSource, float fadeTime)
{
    if (audioSource.isPlaying)
    {
        yield return StartCoroutine(FadeAudioSource.StartFade(audioSource, fadeTime, 0f));
        audioSource.Stop();
    }
}

IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
{
    //audioSource.Play();
    yield return StartCoroutine(FadeAudioSource.StartFade(audioSource, fadeTime, 1f));
    audioSource.Play();
}
}