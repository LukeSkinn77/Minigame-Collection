using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardData cardData;
    public GameObject cardImage;
    public Renderer cardBack;
    public GameObject correctCard;
    public GameObject wrongCard;
    private Renderer cardImageRend;
    public int cardPos;

    public float zoomInYVal = 2.5f;
    public float flipTime = 1.5f;

    private bool isSelectable = false;
    private bool isBeingUsed = false;

    private void Awake()
    {
        cardImageRend = cardImage.GetComponent<Renderer>();
    }

    private void Start()
    {
        EventManager.StartListening("CardSelected", SwitchSelected);
    }

    private void OnDisable()
    {
        EventManager.StopListening("CardSelected", SwitchSelected);
    }

    private void SwitchSelected(EventParameter evn)
    {
        isSelectable = evn.boolVar;
    }

    public void SetCardData(CardData cd)
    {
        cardData = cd;
        cardImageRend.material = cardData.cardMat;
        cardImage.transform.localScale = new Vector3(cardData.cardImageScale.x, cardData.cardImageScale.y, 1);
    }

    public IEnumerator MoveCardIn(float delayTime, float timeToUse, Vector3 pos)
    {
        yield return new WaitForSeconds(delayTime);

        Vector3 startPos = transform.position;
        float currentTime = 0;
        while (currentTime < timeToUse)
        {
            currentTime += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startPos, pos, currentTime / timeToUse);
            transform.position = newPos;
            yield return null;
        }
        isSelectable = true;
        yield return null;
    }

    //IEnumerator ZoomInCard(float zoomTime)
    //{
    //    //float newPos = transform.position.z - zoomInYVal;
    //    float newPos = zoomInYVal;
    //    float startRot = transform.eulerAngles.y;
    //    float tim = 0;

    //    while (tim < zoomTime)
    //    {
    //        tim += Time.deltaTime;
    //        float v = Mathf.Lerp(startRot, 180, tim / zoomTime);

    //        transform.eulerAngles = new Vector3(transform.eulerAngles.x, v, transform.eulerAngles.z);
    //        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + ((180 / zoomTime) * Time.deltaTime), transform.eulerAngles.z);
    //        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - ((newPos / zoomTime) * Time.deltaTime));

    //        yield return null;
    //    }
    //    EventManager.TriggerAction("CompareCard", new EventParameter() { intVar = cardPos, intVar2 = cardData.id });
    //    yield return null;
    //}

    public void FlipWrongCardsBack()
    {
        wrongCard.SetActive(true);

        StartCoroutine(Wait(0.75f));
        //StartCoroutine(MoveCardInOut(flipTime * 0.5f));
        //StartCoroutine(FlipCardBack(flipTime));
    }

    private IEnumerator Wait(float timeToUse)
    {
        yield return new WaitForSeconds(timeToUse);
        StartCoroutine(MoveCardInOut(flipTime * 0.5f));
        StartCoroutine(FlipCardBack(flipTime));
        yield return null;
    }

    IEnumerator MoveCardInOut(float timeToUse)
    {
        float newPos = zoomInYVal;
        float currentTime = 0;
        while (currentTime < timeToUse)
        {
            currentTime += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - ((newPos / timeToUse) * Time.deltaTime));
            yield return null;
        }

        currentTime = 0;
        while (currentTime < timeToUse)
        {
            currentTime += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + ((newPos / timeToUse) * Time.deltaTime));
            yield return null;
        }
    }

    IEnumerator FlipCardOver(float timeToUse)
    {
        float startRot = transform.eulerAngles.y;
        float currentTime = 0;
        while (currentTime < timeToUse)
        {
            currentTime += Time.deltaTime;
            float v = Mathf.Lerp(startRot, 180, currentTime / timeToUse);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, v, transform.eulerAngles.z);
            yield return null;
        }
        EventManager.TriggerAction("CompareCard", new EventParameter() { intVar = cardPos, intVar2 = cardData.id });
        yield return null;
    }

    IEnumerator FlipCardBack(float timeToUse)
    {
        float startRot = transform.eulerAngles.y;
        float currentTime = 0;
        while (currentTime < timeToUse)
        {
            currentTime += Time.deltaTime;
            float v = Mathf.Lerp(startRot, 0, currentTime / timeToUse);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, v, transform.eulerAngles.z);
            yield return null;
        }
        wrongCard.SetActive(false);
        EventManager.TriggerAction("CardSelected", new EventParameter() { boolVar = true });
        isBeingUsed = false;
        yield return null;
    }

    //public IEnumerator ZoomOutCard(float zoomTime)
    //{
    //    //float newPos = transform.position.z + zoomInYVal;
    //    float newPos = zoomInYVal;
    //    float startRot = transform.eulerAngles.y;
    //    float tim = 0;

    //    while (tim < zoomTime)
    //    {
    //        tim += Time.deltaTime;
    //        float v = Mathf.Lerp(startRot, 0, tim / zoomTime);

    //        transform.eulerAngles = new Vector3(transform.eulerAngles.x, v, transform.eulerAngles.z);
    //        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + ((newPos / zoomTime) * Time.deltaTime));

    //        yield return null;
    //    }
    //    EventManager.TriggerAction("CardSelected", new EventParameter() { boolVar = true });
    //    isBeingUsed = false;
    //    yield return null;
    //}

    private void OnMouseDown()
    {
        if (isSelectable && !isBeingUsed)
        {
            EventParameter evn = new EventParameter(){ boolVar = false };
            EventManager.TriggerAction("CardSelected", evn);
            //evn.boolVar = true;
            //SwitchSelected(evn);
            isBeingUsed = true;
            StartCoroutine(MoveCardInOut(flipTime * 0.5f));
            StartCoroutine(FlipCardOver(flipTime));
        }
    }
}

[System.Serializable]
public class CardData
{
    public int id;
    public Material cardMat;
    public Vector2 cardImageScale;

    public CardData(int _id ,Material _cardMat, Vector2 _cardImageScale)
    {
        id = _id;
        cardMat = _cardMat;
        cardImageScale = _cardImageScale;
    }
}