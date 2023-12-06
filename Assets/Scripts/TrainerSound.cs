using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerSound : MonoBehaviour
{
    public AudioClip velkomstKlip;

    public AudioClip[] almostClips;

    public AudioClip[] completedClips;

    public AudioSource trainerAudioSource;

    public GameObject pipeServerObject;

    public PipeServer pipeServer;

    public bool playedSeven = false;
    public bool playedTen = false;
    // Start is called before the first frame update
    void Start()
    {
        trainerAudioSource = this.gameObject.GetComponent<AudioSource>();
        pipeServer = pipeServerObject.GetComponent<PipeServer>();
        trainerAudioSource.PlayOneShot(velkomstKlip);
    }

    // Update is called once per frame
    void Update()
    {
        if (pipeServer.Counter == 7 && !playedSeven)
            {
                trainerAudioSource.PlayOneShot(almostClips[Random.Range(0, 3)]);
                playedSeven = true;
            }
        if (pipeServer.Counter == 10 && !playedTen)
            {
                trainerAudioSource.PlayOneShot(completedClips[Random.Range(0, 3)]);
                playedTen = true;
            }
    }
}
