using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsCameraControl : CameraControl
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

        float v = 0;
        for (int i = 0; i < highestValue; ++i) v += 8.0f;

        destinationPos = new Vector3(0, 0, -v);
        destinationRot = new Vector3(360, 0, 0);
        StartCoroutine(MoveCameraAndStart(2.0f, destinationPos, destinationRot));
    }
}
