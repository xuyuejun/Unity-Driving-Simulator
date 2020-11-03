using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class audio : MonoBehaviour
{
    public CarController car;
    public AudioSource engineAudioSource;
    public float minPitch = 0.5f;
    public float maxPitch = 1.5f;
    private float targetPitch;
    private float pitchFactor;
    void Start()
    {
        engineAudioSource.Play();
    }

    void Update()
    {
        if (car.acceleratorInput != 0)
        {
            pitchFactor = 1;
        } else {
            pitchFactor = 0.5f;
        }
        targetPitch = Mathf.Abs((car.engineRPM) / car.MaxRPM) * pitchFactor;

        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, Mathf.Lerp(minPitch, maxPitch, targetPitch), 20 * Time.deltaTime);
        engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, 0.3f + targetPitch * 0.7f, 20 * Time.deltaTime);
    }
}