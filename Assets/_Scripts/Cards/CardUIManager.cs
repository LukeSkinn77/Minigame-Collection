using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

public class CardUIManager : UIManager
{
    public Image background;
    public TMP_Text txt;
    public Image btn;
    public TMP_Text btnTxt;

    private void Start()
    {
        EventManager.StartListening("Victory", VictoryTrig);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Victory", VictoryTrig);
    }

    private void VictoryTrig(EventParameter evn)
    {
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0);
        btn.color = new Color(btn.color.r, btn.color.g, btn.color.b, 0);
        btnTxt.color = new Color(btnTxt.color.r, btnTxt.color.g, btnTxt.color.b, 0);
        background.gameObject.SetActive(true);
        txt.gameObject.SetActive(true);
        btn.gameObject.SetActive(true);
        StartCoroutine(AlphaShift(txt,600.0f, 0, 255));
        StartCoroutine(AlphaShift(background, 600.0f, 0, 255));
        StartCoroutine(AlphaShift(btnTxt, 600.0f, 0, 255));
        StartCoroutine(AlphaShift(btn, 600.0f, 0, 255));
    }
}
