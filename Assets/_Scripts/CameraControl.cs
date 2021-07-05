using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    protected Vector3 destinationPos;
    protected Vector3 destinationRot;
    private int highestValue;

    [Header("End Start Values")]
    public int startYVal;
    public int startXVal;
    public int startZVal;
    public int startOrthoSize;
    public bool isOrtho = false;
    [Header("Increment Values")]
    public int incrementY;
    public int incrementX;
    public int incrementZ;
    public float incrementOrthoSize;
    public bool ifIncrementing = true;
    [Header("Rotation Values")]
    public Vector3 rotationVal;

    private void Start()
    {
        EventManager.StartListening("StartGame", StartGame);
    }

    private void OnDisable()
    {
        EventManager.StopListening("StartGame", StartGame);
    }

    [ContextMenu("Camera Start")]
    private void DebugStart()
    {
        StartGame(new EventParameter());
    }

    private void StartGame(EventParameter evn)
    {
        GameManager gm = GameManager.Instance;
        if (gm.horizontalGridValue > gm.verticalGridValue) highestValue = gm.horizontalGridValue;
        else highestValue = gm.verticalGridValue;

        float yVal = startYVal;
        float xVal = startXVal;
        float zVal = startZVal;
        float oSize = startOrthoSize;
        if (ifIncrementing)
        {
            for (int i = 0; i < highestValue; ++i)
            {
                yVal += incrementY;
                xVal += incrementX;
                zVal += incrementZ;
                oSize += incrementOrthoSize;
            }
        }

        if (isOrtho) GetComponent<Camera>().orthographicSize = oSize;
        destinationPos = new Vector3(xVal, yVal, zVal);
        destinationRot = rotationVal;

        StartCoroutine(MoveCameraAndStart(1.0f, destinationPos, destinationRot));
    }

    protected IEnumerator MoveCameraAndStart(float timeToUse, Vector3 endPos, Vector3 endRot)
    {
        Vector3 startRot = transform.eulerAngles;
        //float endRot = 360;
        //rot = 0 - startRot;
        yield return new WaitForSeconds(0.75f);

        Vector3 startPos = transform.position;

        float currentTime = 0;
        while (currentTime < timeToUse)
        {
            currentTime += Time.deltaTime;
            Vector3 v = Vector3.Lerp(startRot, endRot, currentTime / timeToUse);
            
            Vector3 newPos = Vector3.Lerp(startPos, endPos, currentTime / timeToUse);
            transform.eulerAngles = v;
            transform.position = newPos;
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - ((newPos / zoomTime) * Time.deltaTime));

            yield return null;
        }
        EventManager.TriggerAction("MakeGame", new EventParameter());
        yield return null;
    }
}
