using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    //public JumperManager jm;

    private float waitTime = 1f;
    private float riseTime = 0.7f;
    private float downTime = 0.6f;
    private float upPos;
    private float downPos;

    public GameObject model;
    public Renderer modelRend;
    private BoxCollider boxCol;

    public bool isGood = true;
    public bool inUse = false;

    private void Awake()
    {
        downPos = model.transform.position.y;
        upPos = model.transform.position.y + 1.8f;
        boxCol = GetComponent<BoxCollider>();
        boxCol.enabled = false;

        model.SetActive(false);
    }

    public void SetMatAndAllegiance(Material mat, bool _isGood)
    {
        isGood = _isGood;
        modelRend.material = mat;
    }

    public void StartJump()
    {
        model.SetActive(true);
        boxCol.enabled = true;
        StartCoroutine(MoveTheStone(riseTime, upPos, true, true));
    }

    private IEnumerator MoveTheStone(float timeToUse, float yPos, bool wait, bool isFullSeq)
    {
        float startPos = model.transform.position.y;

        float currentTime = 0;
        while (currentTime < timeToUse)
        {
            currentTime += Time.deltaTime;
            float newPos = Mathf.Lerp(startPos, yPos, currentTime / timeToUse);
            model.transform.position = new Vector3(model.transform.position.x, newPos, model.transform.position.z);
            yield return null;
        }
        if (wait)
        {
            inUse = true;
            yield return new WaitForSeconds(waitTime);
        }
        if (isFullSeq) StartCoroutine(MoveTheStone(downTime, downPos, false, false));
        else
        {
            inUse = false;
            model.SetActive(false);
        }
        yield return null;
    }

    private void OnMouseDown()
    {
        if (inUse)
        {
            boxCol.enabled = false;
            StopAllCoroutines();
            StartCoroutine(MoveTheStone(downTime, downPos, false, false));
            if (isGood) EventManager.TriggerAction("ScoreChange", new EventParameter() { intVar = 1 });
            else EventManager.TriggerAction("ScoreChange", new EventParameter() { intVar = -1 });
        }
    }
}
