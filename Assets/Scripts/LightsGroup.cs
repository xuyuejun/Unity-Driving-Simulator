using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsGroup : MonoBehaviour
{
    [Header("Objects")]
    public CarController car;
    public Light HeadLightsLeft;
    public Light HeadLightRight;
    public GameObject BarkeLights;
    public GameObject MainLights;
    public GameObject DayTimeLight;
    [Header("Status")]
    public bool onBrake;
    public bool onHeadlight;
    void Update()
    {
        onBrake = car.brakeInput > 0?true:false;
        onHeadlight = car.HeadLight;

        if (onBrake)
        {
            BarkeLights.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red * 5);
        }
        else
        {
            BarkeLights.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }

        if (onHeadlight)
        {
            HeadLightsLeft.enabled = true;
            HeadLightRight.enabled = true;
            MainLights.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * 2);
            DayTimeLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * 2);
        } else {
            HeadLightsLeft.enabled = false;
            HeadLightRight.enabled = false;
            MainLights.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
            DayTimeLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }
    }
}
