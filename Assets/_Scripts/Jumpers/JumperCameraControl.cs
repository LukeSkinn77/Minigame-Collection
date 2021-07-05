using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperCameraControl : CameraControl
{
    private int highestValue;

    private void Start()
    {
        EventManager.StartListening("StartGame", StartGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening("StartGame", StartGame);
    }

    private void StartGame(EventParameter evn)
    {
        GameManager gm = GameManager.Instance;
        if (gm.horizontalGridValue > gm.verticalGridValue) highestValue = gm.horizontalGridValue;
        else highestValue = gm.verticalGridValue;

        float yVal = 2;
        float zVal = -2;
        for (int i = 0; i < highestValue; ++i)
        {
            yVal += 1f;
            zVal -= 1f;
        }

        destinationPos = new Vector3(0, yVal, zVal);
        destinationRot = new Vector3(50, 0, 0);

        StartCoroutine(MoveCameraAndStart(1.0f, destinationPos, destinationRot));
    }
}