using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform car;
    public Transform MiniMapCamera;
    public float[] Zooms;
    public int zoom = 2;
    void OnZoomChange()
    {
        if (zoom == 0)
        {
            zoom = Zooms.Length - 1;
        }
        else
        {
            zoom --;
        }
    }
    void Update()
    {
        MiniMapCamera.position = new Vector3(car.position.x, Zooms[zoom], car.position.z);
        MiniMapCamera.rotation = Quaternion.Euler(90, car.eulerAngles.y, car.eulerAngles.z);
    }
}
