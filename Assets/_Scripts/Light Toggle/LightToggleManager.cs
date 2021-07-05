using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightToggleManager : MonoBehaviour
{
    private int horizontalGridNum = 4;
    private int verticalGridNum = 2;
    private Vector2 bounds = new Vector2(20, 20);
    private ToggleButton[,] lightButtons;
    private int maxOn = 3;
    //private List<LightButton> buttons = new List<LightButton>();

    [Header("Light Button Settings")]
    public GameObject buttonObj;
    public float spaceBetweenBtns = 4f;
    public Vector2 buttonScale = new Vector2(2.5f, 2.5f);

    public AudioPlay audia;

    private void Awake()
    {
        audia.AssignAudioSource(GetComponent<AudioSource>());
    }

    private void Start()
    {
        EventManager.StartListening("StartGame", MakeLights);
        EventManager.StartListening("CheckButton", ToggleAndCheckButtons);
    }

    private void OnDisable()
    {
        EventManager.StopListening("StartGame", MakeLights);
        EventManager.StopListening("CheckButton", ToggleAndCheckButtons);
    }

    private void MakeLights(EventParameter evn)
    {
        GameManager gm = GameManager.Instance;
        horizontalGridNum = gm.horizontalGridValue;
        verticalGridNum = gm.verticalGridValue;
        bounds = new Vector2((buttonScale.x * verticalGridNum) + ((verticalGridNum - 1) * spaceBetweenBtns), (buttonScale.y * horizontalGridNum) + ((horizontalGridNum - 1) * spaceBetweenBtns));
        lightButtons = new ToggleButton[horizontalGridNum, verticalGridNum];

        //int idNum = 0;
        for (int i = 0; i < horizontalGridNum; ++i)
        {
            for (int j = 0; j < verticalGridNum; ++j)
            {
                float xVal = (-bounds.x * 0.5f) + ((buttonScale.x + spaceBetweenBtns) * j) + (buttonScale.x * 0.5f);
                float yVal = (bounds.y * 0.5f) - ((buttonScale.y + spaceBetweenBtns) * i) - (buttonScale.y * 0.5f);
                var newObj = Instantiate(buttonObj, new Vector3(xVal, 0.6f, yVal), new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w));
                lightButtons[i, j] = newObj.GetComponent<ToggleButton>();
                lightButtons[i, j].xVar = j;
                lightButtons[i, j].yVar = i;

                if (maxOn > 0)
                {
                    if (Random.value < 0.3)
                    {
                        lightButtons[i, j].SetAlignment();
                        maxOn--;
                    }
                }

                //buttons.Add(newObj.GetComponent<LightButton>());
                //buttons[buttons.Count - 1].id = idNum;
                //idNum++;
            }
        }
    }

    private void ToggleAndCheckButtons(EventParameter evn)
    {
        if (evn.intVar - 1 > -1) lightButtons[evn.intVar2, evn.intVar - 1].SetAlignment();
        if (evn.intVar + 1 < verticalGridNum) lightButtons[evn.intVar2, evn.intVar + 1].SetAlignment();
        if (evn.intVar2 - 1 > -1) lightButtons[evn.intVar2 - 1, evn.intVar].SetAlignment();
        if (evn.intVar2 + 1 < horizontalGridNum) lightButtons[evn.intVar2 + 1, evn.intVar].SetAlignment();

        bool isWon = true;
        for (int i = 0; i < horizontalGridNum; ++i)
        {
            for (int j = 0; j < verticalGridNum; ++j)
            {
                if (!lightButtons[j, i].isGood)
                {
                    isWon = false;
                    break;
                }
            }
        }
        if (isWon)
        {
            for (int i = 0; i < horizontalGridNum; ++i)
            {
                for (int j = 0; j < verticalGridNum; ++j)
                {
                    lightButtons[j, i].canSelect = false;

                }
            }
            audia.PlaySoundEffect(1);
            EventManager.TriggerAction("Victory", new EventParameter());
        }
        else audia.PlaySoundEffect(0);
    }
}
