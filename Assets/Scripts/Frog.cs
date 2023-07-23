using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Frog : MonoBehaviour
{
    Rigidbody rb;
    Vector3 randomDir;


    [SerializeField]
    float rotationSpeed = 2f;

    [SerializeField]
    AudioClip[] audioClips;

    AudioSource audioSource;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PeriodicJump());
    }

    IEnumerator PeriodicJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3,6));
            // JUMP
            randomDir = new Vector3(Random.Range(-5f, 5f), 5, 0).normalized;
            rb.AddForce(randomDir * 5f, ForceMode.Impulse);
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
        }
    }

    void FixedUpdate()
    {
        // UPRIGHT
        Vector3 torque = Vector3.Cross(transform.forward, Vector3.up);
        float angleToUpright = Vector3.Angle(transform.forward, Vector3.up);
        rb.AddTorque(torque * angleToUpright * rotationSpeed);

        // JUMP-DIR this is fucked
        // transform.LookAt(transform.position + randomDir);
        Vector3 torqueDir = Vector3.Cross(transform.up, randomDir);
        float angleToLookAt = Vector3.Angle(transform.up, randomDir);
        rb.AddTorque(torqueDir * angleToLookAt * 0.05f);

    }
}
