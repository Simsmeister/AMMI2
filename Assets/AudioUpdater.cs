using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AudioUpdater : MonoBehaviour
{
    private MyListener myListener; // Reference to the MyListener script
    private AudioSource audioSource;
    public AudioClip[] heartBeatMusic;
    public int heartRate;
    public int heartRateInterval;
    public int currentInterval;
    public float fadeTime = 2f;

    private float lastExecutionTime;
    public bool hasPlayedOnce = false;
    public float elapsedTime = 60f;

    //private bool sixtyPlaying = false, seventyPlaying = false, eightyPlaying = false, ninetyPlaying = false, hundredtenPlaying = false;
    void Start()
    {
        // Find the Cube object and get the MyListener component
        GameObject cubeObject = GameObject.Find("Cube");
        GameObject camera = GameObject.Find("Main Camera");
        if (cubeObject != null)
        {
            myListener = cubeObject.GetComponent<MyListener>();
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

        // Set the time interval for the if statement to run (e.g., once per minute)
        float timeInterval = 60f; // 60 seconds = 1 minute

        // Check if a new interval is reached
        if (heartRateInterval != currentInterval && elapsedTime >= timeInterval)
        {
            currentInterval = heartRateInterval;

            // Stop the current audio and initiate the fade-out
            StartCoroutine(FadeOutAndStop(audioSource, fadeTime));

            // Play the appropriate audio clip and initiate the fade-in
            PlayAudioForInterval(currentInterval);
            elapsedTime = Time.time - lastExecutionTime;
            lastExecutionTime = Time.time;
        }
    }
}

void PlayAudioForInterval(int interval)
{
    AudioClip clipToPlay = null;

    switch (interval)
    {
        case 60:
            clipToPlay = heartBeatMusic[0];
            break;
        case 70:
            clipToPlay = heartBeatMusic[1];
            break;
        case 80:
            clipToPlay = heartBeatMusic[2];
            break;
        case 90:
            clipToPlay = heartBeatMusic[3];
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