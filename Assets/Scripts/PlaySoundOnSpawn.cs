using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnSpawn : MonoBehaviour
{
    // public AudioClip[] audioClips;
    // Start is called before the first frame update
    void Start()
    {
        // AudioClip randomAudio = audioClips[Random.Range(0, audioClips.Length)];

        AudioSource audio = GetComponent<AudioSource>();
        audio.pitch = Random.Range(0.75f, 1.25f);
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
