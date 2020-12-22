using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCamera : MonoBehaviour
{
    public Transform VR;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x - VR.localPosition.x, transform.localPosition.y - VR.localPosition.y, transform.localPosition.z - VR.localPosition.z);
    }
}
