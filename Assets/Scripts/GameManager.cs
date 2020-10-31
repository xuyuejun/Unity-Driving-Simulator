using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Dash board")]
    public CarController car;
    public GameObject needle;
    public Text kph;
    public Text gearNum;
    private float startPosiziton = 32f, endPosition = -211f;        // 转速仪表盘
    private float desiredPosition;

    private void Awake() {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeGear();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        updateKPH();
        updateNeedle();
    }

    public void updateKPH()
    {
        kph.text = car.Kph.ToString ("0");
    }

    public void ChangeGear()
    {
        gearNum.text = "D";
    }

    public void updateNeedle()
    {
        desiredPosition = startPosiziton - endPosition;
        float temp = car.engineRPM / 10000;
        needle.transform.eulerAngles = new Vector3(0, 0, (startPosiziton - temp * desiredPosition));
    }
}
