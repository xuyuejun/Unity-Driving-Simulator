using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetection : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject manager;
    public bool GameInProgress = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        // If the player passes through the checkpoint, we activate it
        if (other.tag == "StartCheckpoint")
        {
            if (GameInProgress == false)
            {
                manager.SendMessage("OnGameStart");
                GameInProgress = true;
            }
            else
            {
                manager.SendMessage("OnGameEnd");
                GameInProgress = false;
            }
        }
    }
}
