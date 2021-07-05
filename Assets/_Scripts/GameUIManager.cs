using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

public class GameUIManager : UIManager
{
    public TMP_InputField horiLines;
    public TMP_InputField vertLines;
    public TMP_InputField scoreLines;
    public Toggle endlessToggle;

    public int maxVert = 5;
    public int maxHori = 5;
    public int maxScore = 99;

    public int defaultVert = 3;
    public int defaultHori = 4;
    public int defaultScore = 5;
    private RangeInt lineNums;

    private void Awake()
    {
        currentPanel = GameObject.Find("Main Panel").GetComponent<RectTransform>();
        lineNums.start = 2;
        lineNums.length = 6;
    }

    public void SwitchPanel(RectTransform newPanel)
    {
        if (currentPanel) currentPanel.gameObject.SetActive(false);
        currentPanel = null;
        currentPanel = newPanel;
        currentPanel.gameObject.SetActive(true);
    }

    public void SetScore()
    {
        int maxScoreNum = 1;
        if (scoreLines && scoreLines.text != "")
        {
            if (Inbetween(int.Parse(scoreLines.text), maxScore, 1)) maxScoreNum = int.Parse(scoreLines.text);
            else maxScoreNum = defaultScore;
        }
        GameManager.Instance.SetGameValues(0, 0, maxScoreNum);
    }

    public void SetEndless()
    {
        GameManager.Instance.isEndless = endlessToggle.isOn;
    }

    public void SetGrid()
    {
        bool hasMaxScore = false;
        int horNum = 2;
        int verNum = 2;
        int maxScoreNum = 2;

        if (horiLines && horiLines.text != "")
        {
            if (Inbetween(int.Parse(horiLines.text), maxHori, lineNums.start)) horNum = int.Parse(horiLines.text);
            else horNum = defaultHori;
        }
        if (vertLines && vertLines.text != "")
        {
            if (Inbetween(int.Parse(vertLines.text), maxVert, lineNums.start)) verNum = int.Parse(vertLines.text);
            else verNum = defaultVert;
        }
        if (scoreLines && scoreLines.text != "")
        {
            hasMaxScore = true;
            if (Inbetween(int.Parse(scoreLines.text), maxScore, 1)) maxScoreNum = int.Parse(scoreLines.text);
            else maxScoreNum = defaultScore;
        }

        if (hasMaxScore) GameManager.Instance.SetGameValues(horNum, verNum, maxScoreNum);
        else GameManager.Instance.SetGameValues(horNum, verNum);
    }

    private bool Inbetween(int value, int upperBound, int lowerBound)
    {
        if (upperBound > lowerBound) return value >= lowerBound && value <= upperBound;
        return value >= upperBound && value <= lowerBound;
    }

    public void KillMenuAndStart()
    {
        EventManager.TriggerAction("StartGame", new EventParameter());
        Destroy(gameObject);
    }
}
