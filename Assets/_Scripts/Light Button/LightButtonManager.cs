using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightButtonManager : MonoBehaviour
{
    private int maxRounds = 10;
    private int currentRound = 0;
    private int horizontalGridNum = 4;
    private int verticalGridNum = 2;
    public AudioPlay aud;
    private Vector2 bounds = new Vector2(20, 20);
    private List<LightButton> buttons = new List<LightButton>();

    [Header("Light Button Settings")]
    public GameObject buttonObj;
    public float spaceBetweenBtns = 4f;
    public Vector2 buttonScale = new Vector2(2.5f, 2.5f);

    [Header("Round Settings")]
    public int startRoundLength = 3;
    public int maxRoundLength = 6;
    private int currentRoundLength;
    private List<int> roundLength;
    private int currentButtonInRound = 0;

    private void Awake()
    {
        aud.AssignAudioSource(GetComponent<AudioSource>());
        //aud = new AudioPlayer(GetComponent<AudioSource>());
    }

    private void Start()
    {
        EventManager.StartListening("StartGame", MakeButtons);
        EventManager.StartListening("CheckButton", CheckAnswer);
        EventManager.StartListening("MakeGame", FirstRound);
    }

    private void OnDisable()
    {
        EventManager.StopListening("CheckButton", CheckAnswer);
        EventManager.StopListening("MakeGame", FirstRound);
        EventManager.StopListening("StartGame", MakeButtons);
    }

    private void MakeButtons(EventParameter evn)
    {
        GameManager gm = GameManager.Instance;
        horizontalGridNum = gm.horizontalGridValue;
        verticalGridNum = gm.verticalGridValue;
        maxRounds = gm.maxScoreValue;
        bounds = new Vector2((buttonScale.x * verticalGridNum) + ((verticalGridNum - 1) * spaceBetweenBtns), (buttonScale.y * horizontalGridNum) + ((horizontalGridNum - 1) * spaceBetweenBtns));
        currentRoundLength = startRoundLength;

        int idNum = 0;
        for (int i = 0; i < horizontalGridNum; ++i)
        {
            for (int j = 0; j < verticalGridNum; ++j)
            {
                float xVal = (-bounds.x * 0.5f) + ((buttonScale.x + spaceBetweenBtns) * j) + (buttonScale.x * 0.5f);
                float yVal = (bounds.y * 0.5f) - ((buttonScale.y + spaceBetweenBtns) * i) - (buttonScale.y * 0.5f);
                var newObj = Instantiate(buttonObj, new Vector3(xVal, 0.6f, yVal), new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w));
                buttons.Add(newObj.GetComponent<LightButton>());
                buttons[buttons.Count - 1].id = idNum;
                idNum++;
            }
        }
    }

    private void FirstRound(EventParameter evn)
    {
        CreateRound();
    }

    private void CreateRound()
    {
        roundLength = new List<int>();
        for (int i = 0; i < currentRoundLength; ++i)
        {
            roundLength.Add(Random.Range(0, buttons.Count));
            bool isLast = false;
            if (i == currentRoundLength - 1) isLast = true;
            StartCoroutine(LightSequence(roundLength[roundLength.Count - 1], i + 1, isLast));
        }
    }

    IEnumerator LightSequence(int light, int delayNum, bool isLast)
    {
        yield return new WaitForSeconds(1.5f * delayNum);
        aud.PlaySoundEffect(0);
        buttons[light].PressButton();
        if (isLast) EventManager.TriggerAction("SetSelection", new EventParameter() { boolVar = true });
    }

    IEnumerator Wait(float timeToUse, bool isNewRound)
    {
        yield return new WaitForSeconds(timeToUse);
        if (isNewRound) CreateRound();
        else
        {
            for (int i = 0; i < roundLength.Count; ++i)
            {
                bool isLast = false;
                if (i == roundLength.Count - 1) isLast = true;
                StartCoroutine(LightSequence(roundLength[i], i + 1, isLast));
            }
        }
    }

    private void CheckAnswer(EventParameter evn)
    {
        if (evn.intVar == roundLength[currentButtonInRound])
        {
            currentButtonInRound++;
            if (currentButtonInRound == roundLength.Count)
            {
                aud.PlaySoundEffect(1);
                currentRound++;
                if (currentRound == maxRounds)
                {
                    EventManager.TriggerAction("Victory", new EventParameter());
                    return;
                }

                if (currentRoundLength < maxRoundLength) currentRoundLength++;
                currentButtonInRound = 0;
                StartCoroutine(Wait(1.0f, true));
                //CreateRound();
            }
            else aud.PlaySoundEffect(0);
        }
        else
        {
            aud.PlaySoundEffect(2);
            EventManager.TriggerAction("SetSelection", new EventParameter() { boolVar = false });
            currentButtonInRound = 0;
            StartCoroutine(Wait(1.0f, false));
            //for (int i = 0; i < roundLength.Count; ++i)
            //{
            //    bool isLast = false;
            //    if (i == roundLength.Count - 1) isLast = true;
            //    StartCoroutine(LightSequence(roundLength[i], i + 1, isLast));
            //}
        }
    }
}
