using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform car;
    public Transform MiniMapCamera;
    public float MapZoomHeight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MiniMapCamera.position = new Vector3(car.position.x,MapZoomHeight,car.position.z);
        MiniMapCamera.rotation = Quaternion.Euler(90,car.eulerAngles.y,car.eulerAngles.z);
    }
}
