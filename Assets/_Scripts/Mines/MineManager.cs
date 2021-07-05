using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : MonoBehaviour
{
    public int bombMaxNum = 6;
    private int horizontalGridNum = 8;
    private int verticalGridNum = 8;
    private Vector2 bounds = new Vector2(20, 20);
    private MineBlock[,] blocks;

    private bool firstClick = false;
    private int maxBlocks;

    [Header("Bomb")]
    public List<int> bombObjs;
    private List<MineBlock> bombs;
    
    [Header("Block Settings")]
    public GameObject blockObj;
    public float spaceBetweenBlocks = 4f;
    public Vector2 blockScale = new Vector2(1, 1);

    private void Start()
    {
        EventManager.StartListening("StartGame", MakeBlocks);
        EventManager.StartListening("MakeGame", TurnOnCollision);
        EventManager.StartListening("CheckBomb", CheckBlock);
    }

    private void MakeBlocks(EventParameter evn)
    {
        GameManager gm = GameManager.Instance;
        horizontalGridNum = gm.horizontalGridValue;
        verticalGridNum = gm.verticalGridValue;

        maxBlocks = horizontalGridNum * verticalGridNum;
        bounds = new Vector2((blockScale.x * verticalGridNum) + ((verticalGridNum - 1) * spaceBetweenBlocks), (blockScale.y * horizontalGridNum) + ((horizontalGridNum - 1) * spaceBetweenBlocks));
        blocks = new MineBlock[verticalGridNum, horizontalGridNum];

        for (int i = 0; i < horizontalGridNum; ++i)
        {
            for (int j = 0; j < verticalGridNum; ++j)
            {
                float xVal = (-bounds.x * 0.5f) + ((blockScale.x + spaceBetweenBlocks) * j) + (blockScale.x * 0.5f);
                float yVal = (bounds.y * 0.5f) - ((blockScale.y + spaceBetweenBlocks) * i) - (blockScale.y * 0.5f);
                var newObj = Instantiate(blockObj, new Vector3(xVal, 0.0f, yVal), new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w));
                MineBlock block = newObj.GetComponent<MineBlock>();
                blocks[j, i] = block;
                block.SetVariables(j, i);
            }
        }

        bombs = new List<MineBlock>();
        for (int i = 0; i < bombMaxNum; ++i)
        {
            int horiNum = Mathf.RoundToInt(Random.Range(0, horizontalGridNum));
            int vertNum = Mathf.RoundToInt(Random.Range(0, verticalGridNum));

            if (blocks[vertNum, horiNum].blockType == MineBlockType.Bomb) continue;

            blocks[vertNum, horiNum].blockType = MineBlockType.Bomb;
            //blocks[vertNum, horiNum].GetComponentInChildren<Renderer>().material = blocks[vertNum, horiNum].bombTex;
            bombs.Add(blocks[vertNum, horiNum]);
        }
    }

    private void EdgeDetect()
    {
        for (int i = 0; i < bombs.Count; ++i)
        {
            int[] v = bombs[i].GetVariables();
            int xPos = v[0];
            int yPos = v[1];

            if (xPos + 1 < verticalGridNum)
            {
                blocks[xPos + 1, yPos].SetEdge();
                if (yPos + 1 < horizontalGridNum) blocks[xPos + 1, yPos + 1].SetEdge();
                if (yPos - 1 > -1) blocks[xPos + 1, yPos - 1].SetEdge();
            }
            if (xPos - 1 > -1)
            {
                blocks[xPos - 1, yPos].SetEdge();
                if (yPos + 1 < horizontalGridNum) blocks[xPos - 1, yPos + 1].SetEdge();
                if (yPos - 1 > -1) blocks[xPos - 1, yPos - 1].SetEdge();
            }
            if (yPos + 1 < horizontalGridNum) blocks[xPos, yPos + 1].SetEdge();
            if (yPos - 1 > -1) blocks[xPos, yPos - 1].SetEdge();
        }
    }

    private void OnDisable()
    {
        EventManager.StopListening("MakeGame", TurnOnCollision);
        EventManager.StopListening("CheckBomb", CheckBlock);
        EventManager.StopListening("StartGame", MakeBlocks);
    }

    private void TurnOnCollision(EventParameter evn)
    {
        for (int i = 0; i < horizontalGridNum; ++i)
        {
            for (int j = 0; j < verticalGridNum; ++j)
            {
                blocks[j, i].EnableCol();
            }
        }
    }

    private void CheckBlock(EventParameter evn)
    {
        if (evn.boolVar)
        {
            if (firstClick)
            {
                for (int i = 0; i < bombs.Count; ++i)
                {
                    bombs[i].gameObject.SetActive(false);
                    int bombID = bombObjs[Mathf.RoundToInt(Random.Range(0, bombObjs.Count - 1))];

                    GameObject obj = ObjectPool.Instance.GetPooledObjects(bombID);
                    if (obj == null) return;
                    obj.transform.position = bombs[i].transform.position;
                    obj.transform.rotation = Quaternion.identity;
                    obj.SetActive(true);
                }    
                EventManager.TriggerAction("Defeat", new EventParameter());
                return;
            }
            else
            {
                blocks[evn.intVar, evn.intVar2].blockType = MineBlockType.Regular;
                bombs.Remove(blocks[evn.intVar, evn.intVar2]);
                blocks[0, 0].blockType = MineBlockType.Bomb;
                bombs.Add(blocks[0, 0]);
            }
        }

        if (!firstClick)
        {
            firstClick = true;
            EdgeDetect();
        }
        if (blocks[evn.intVar, evn.intVar2].blockType == MineBlockType.Regular) FloodCheck(evn.intVar, evn.intVar2);
        else
        {
            blocks[evn.intVar, evn.intVar2].gameObject.SetActive(false);
            maxBlocks--;
        }
        //int v = 0;
        //for (int i = 0; i < horizontalGridNum; ++i)
        //{
        //    for (int j = 0; j < verticalGridNum; ++j)
        //    {
        //        if (blocks[j, i].isActiveAndEnabled) v++;
        //    }
        //}
        if (maxBlocks == bombs.Count) EventManager.TriggerAction("Victory", new EventParameter());

        //switch (blocks[evn.intVar, evn.intVar2].blockType)
        //{
        //    case MineBlockType.Regular:
        //        //if (!firstClick)
        //        //{
        //        //    firstClick = true;
        //        //    EdgeDetect();
        //        //}

        //        FloodCheck(evn.intVar, evn.intVar2);
        //        break;
        //    case MineBlockType.Edge:
        //        //if (!firstClick)
        //        //{
        //        //    firstClick = true;
        //        //    EdgeDetect();
        //        //}
        //        blocks[evn.intVar, evn.intVar2].gameObject.SetActive(false);
        //        break;
        //}
    }

    private void FloodCheck(int x, int y)
    {
        if (x < 0 || x > verticalGridNum - 1 || y < 0 || y > horizontalGridNum - 1) return;
        if (!blocks[x, y].isActiveAndEnabled || blocks[x, y].blockType == MineBlockType.Bomb) return;

        blocks[x, y].gameObject.SetActive(false);
        maxBlocks--;

        if (blocks[x, y].blockType == MineBlockType.Edge) return;

        FloodCheck(x + 1, y);
        FloodCheck(x - 1, y);
        FloodCheck(x + 1, y + 1);
        FloodCheck(x + 1, y - 1);
        FloodCheck(x - 1, y + 1);
        FloodCheck(x - 1, y - 1);
        FloodCheck(x, y + 1);
        FloodCheck(x, y - 1);
    }
}
