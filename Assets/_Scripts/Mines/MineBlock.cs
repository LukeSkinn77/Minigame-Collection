using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MineBlock : MonoBehaviour
{
    public MineBlockType blockType;
    public TMP_Text text;

    private BoxCollider boxCol;

    private int xPos;
    private int yPos;

    private int edgeBombs = 0;

    //public Material bombTex;

    private void Start()
    {
        EventManager.StartListening("Defeat", Defeat);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Defeat", Defeat);
    }

    private void Defeat(EventParameter evn)
    {
        boxCol.enabled = false;
    }

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider>();
        boxCol.enabled = false;
    }

    public void SetVariables(int x, int y)
    {
        xPos = x;
        yPos = y;
    }

    public int[] GetVariables()
    {
        int[] v = new int[2];
        v[0] = xPos;
        v[1] = yPos;
        return v;
    }

    public void EnableCol()
    {
        boxCol.enabled = true;
    }

    public void SetEdge()
    {
        if (blockType == MineBlockType.Bomb) return;
        blockType = MineBlockType.Edge;
        edgeBombs++;
        text.transform.SetParent(null);// = null;
        text.gameObject.SetActive(true);
        text.text = edgeBombs.ToString();
    }

    private void OnMouseDown()
    {
        //EventManager.TriggerAction("CheckBomb", new EventParameter() { intVar = xPos, intVar2 = yPos, boolVar = true }); ;
        bool isBomb = false;
        if (blockType == MineBlockType.Bomb) isBomb = true;
        EventManager.TriggerAction("CheckBomb", new EventParameter() { intVar = xPos, intVar2 = yPos, boolVar = isBomb }); ;

        //switch (blockType)
        //{
        //    case MineBlockType.Bomb:
        //        EventManager.TriggerAction("CheckBomb", new EventParameter() { intVar = xPos, intVar2 = yPos, boolVar = true }); ;
        //        break;
        //    case MineBlockType.Regular:
        //        EventManager.TriggerAction("CheckBomb", new EventParameter() { intVar = xPos, intVar2 = yPos, boolVar = false });
        //        break;
        //    case MineBlockType.Edge:
        //        EventManager.TriggerAction("CheckBomb", new EventParameter() { intVar = xPos, intVar2 = yPos, boolVar = false });
        //        break;
        //}
    }
}

public enum MineBlockType
{
    Regular,
    Bomb,
    Edge
}