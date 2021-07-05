using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightButton : MonoBehaviour
{
    public Transform buttonTran;
    private Renderer buttonRend;

    public int id = 0;
    public float riseTime = 0.1f;
    public float downTime = 0.1f;
    private bool isMove = false;
    private bool canSelect = false;
    private Vector3 btnIndentPos;

    private void Awake()
    {
        buttonRend = buttonTran.GetComponent<Renderer>();
        btnIndentPos = new Vector3(buttonTran.position.x, buttonTran.position.y - 0.15f, buttonTran.position.z);

        buttonRend.material.color = new Color(Random.value, Random.value, Random.value);
    }

    private void Start()
    {
        EventManager.StartListening("SetSelection", SelectModify);
    }

    private void OnDisable()
    {
        EventManager.StopListening("SetSelection", SelectModify);
    }

    private IEnumerator MoveButton(Vector3 endPos)
    {
        Vector3 startPos = buttonTran.position;

        float currentTime = 0;
        while (currentTime < downTime)
        {
            currentTime += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startPos, endPos, currentTime / downTime);
            buttonTran.position = newPos;
            yield return null;
        }

        currentTime = 0;
        while (currentTime < riseTime)
        {
            currentTime += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(endPos, startPos, currentTime / riseTime);
            buttonTran.position = newPos;
            yield return null;
        }

        isMove = false;
        yield return null;
    }

    //private IEnumerator ColourChange(float timeToUse, Color endCol)
    //{
    //    Color col = buttonRend.material.color;

    //    float currentTime = 0;
    //    while (currentTime < timeToUse)
    //    {
    //        currentTime += Time.deltaTime;
    //        buttonRend.material.color = Color.Lerp(col, endCol, currentTime / timeToUse);
    //        yield return null;
    //    }
    //}

    private IEnumerator ColourChange(Color endCol)
    {
        Color col = buttonRend.material.color;

        float currentTime = 0;
        while (currentTime < downTime)
        {
            currentTime += Time.deltaTime;
            buttonRend.material.color = Color.Lerp(col, endCol, currentTime / downTime);
            yield return null;
        }

        currentTime = 0;
        while (currentTime < riseTime)
        {
            currentTime += Time.deltaTime;
            buttonRend.material.color = Color.Lerp(endCol, col, currentTime / riseTime);
            yield return null;
        }
    }

    private void SelectModify(EventParameter evn)
    {
        canSelect = evn.boolVar;
    }

    [ContextMenu("Press Button")]
    public void PressButton()
    {
        isMove = true;
        StartCoroutine(MoveButton(btnIndentPos));
        StartCoroutine(ColourChange(Color.red));
    }

    private void OnMouseDown()
    {
        if (!isMove && canSelect)
        {
            //isMove = true;
            PressButton();
            EventManager.TriggerAction("CheckButton", new EventParameter() { intVar = id });
        }
    }
}
