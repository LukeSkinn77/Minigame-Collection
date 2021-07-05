using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    public bool isGood = false;
    public bool canSelect = false;

    public Renderer render;

    public int xVar;
    public int yVar;

    [Header("Materials")]
    public Material badMat;
    public Material goodMat;

    private void Start()
    {
        EventManager.StartListening("MakeGame", SetSelectOn);
    }

    private void OnDisable()
    {
        EventManager.StopListening("MakeGame", SetSelectOn);
    }

    public void SetAlignment()
    {
        if (isGood) SetBad();
        else SetGood();
    }

    private void SetGood()
    {
        isGood = true;
        render.material = goodMat;
    }

    private void SetBad()
    {
        isGood = false;
        render.material = badMat;
    }

    private void SetSelectOn(EventParameter evn)
    {
        canSelect = true;
    }

    private void OnMouseDown()
    {
        if (canSelect)
        {
            SetAlignment();
            EventManager.TriggerAction("CheckButton", new EventParameter() { intVar = xVar, intVar2 = yVar });
        }
    }
}
