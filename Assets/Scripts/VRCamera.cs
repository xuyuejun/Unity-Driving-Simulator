﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCamera : MonoBehaviour
{
    public Transform VR;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x - VR.localPosition.x, transform.localPosition.y - VR.localPosition.y, transform.localPosition.z - VR.localPosition.z);

        // transform.position = new Vector3(transform.position.x - VR.position.x, transform.position.y - VR.position.y, transform.position.z - VR.position.z);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
