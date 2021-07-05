using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

public class GunDownManager : MonoBehaviour
{
    private float dropTime = 0;
    public float dropRate = 2.5f;
    public string lvlName;
    [Header("Drop Percentages")]
    public int easyDropPercentage = 65;
    public int easyDropId = 0;
    public int easyBadDropId = 3;
    public int normalDropPercentage = 25;
    public int normalDropId = 1;
    public int normalBadDropId = 4;
    public int hardDropPercentage = 10;
    public int hardDropId = 2;
    public int hardBadDropId = 5;
    private List<int> dropListGood = new List<int>();
    private List<int> dropListBad = new List<int>();

    private int scoreLimit = 1;
    private int currentScore = 0;
    private bool hasWon = false;

    public Vector3 spawnCentrePosition;

    [Header("X Range")]
    public bool dropAlongX = true;
    public float dropRangeX = 50f;
    [Header("Y Range")]
    public bool dropAlongY = false;
    public float dropRangeY = 50f;
    [Header("Z Range")]
    public bool dropAlongZ = false;
    public float dropRangeZ = 50f;

    private float startDropX;
    private float startDropY;
    private float startDropZ;

    private bool isFall = false;

    [Header("Board Image")]
    public Renderer imageRenderer;
    public List<string> itemSetsInPlay;
    private Cardset imageSet;


    private void Awake()
    {
        imageSet = GetComponent<Cardset>();

        //itemSetsInPlay = new List<string>();
        //XDocument xDoc = XDocument.Load("Assets/Resources/Documents/imagesetsInUse.xml"); ;
        //IEnumerable<XElement> items = xDoc.Descendants("set").Elements();
        //StartCoroutine(AssignData(items));

        if (easyDropPercentage + normalDropPercentage + hardDropPercentage > 100)
        {
            int largNum = Mathf.Max(Mathf.Max(easyDropPercentage, normalDropPercentage), hardDropPercentage);
            if (largNum == easyDropPercentage)
            {
                int num = 100 - (normalDropPercentage + hardDropPercentage);
                easyDropPercentage = easyDropPercentage - (easyDropPercentage - num);
            }
            else if (largNum == normalDropPercentage)
            {
                int num = 100 - (easyDropPercentage + hardDropPercentage);
                normalDropPercentage = normalDropPercentage - (normalDropPercentage - num);
            }
            else if (largNum == hardDropPercentage)
            {
                int num = 100 - (normalDropPercentage + easyDropPercentage);
                hardDropPercentage = hardDropPercentage - (hardDropPercentage - num);
            }
        }
        AddToList(easyDropPercentage, easyDropId, ref dropListGood);
        AddToList(normalDropPercentage, normalDropId, ref dropListGood);
        AddToList(hardDropPercentage, hardDropId, ref dropListGood);
        AddToList(easyDropPercentage, easyBadDropId, ref dropListBad);
        AddToList(normalDropPercentage, normalBadDropId, ref dropListBad);
        AddToList(hardDropPercentage, hardBadDropId, ref dropListBad);

        //dropRate *= 100;
        if (dropAlongX) startDropX = spawnCentrePosition.x - (dropRangeX * 0.5f);
        else startDropX = spawnCentrePosition.x;
        if (dropAlongY) startDropY = spawnCentrePosition.y - (dropRangeY * 0.5f);
        else startDropY = spawnCentrePosition.y;
        if (dropAlongZ) startDropZ = spawnCentrePosition.z - (dropRangeZ * 0.5f);
        else startDropZ = spawnCentrePosition.z;
    }

    private IEnumerator AssignData(IEnumerable<XElement> items)
    {
        foreach (var item in items)
        {
            if (item.Parent.Attribute("level").Value != lvlName) continue;
            itemSetsInPlay.Add(item.Value);
        }
        yield return null;
    }

    private void AddToList(int num, int numToAdd, ref List<int> list)
    {
        for (int i = 0; i < num; ++i)
        {
            list.Add(numToAdd);
        }
    }

    private void Start()
    {
        List<CardData> images = imageSet.GetItems(itemSetsInPlay);
        images = ListRandomiser(images, 3);
        imageRenderer.material = images[0].cardMat;
        imageRenderer.transform.localScale = new Vector3(images[0].cardImageScale.x, images[0].cardImageScale.y, 1);

        EventManager.StartListening("MakeGame", SetFallOn);
        EventManager.StartListening("ScoreChange", UpdateScore);
    }

    private void OnDisable()
    {
        EventManager.StopListening("MakeGame", SetFallOn);
        EventManager.StopListening("ScoreChange", UpdateScore);
    }

    private void SetFallOn(EventParameter evn)
    {
        GameManager gm = GameManager.Instance;
        scoreLimit = gm.maxScoreValue;
        isFall = true;
    }

    private void Update()
    {
        if (isFall) dropTime += Time.deltaTime;
        if (dropTime >= dropRate)
        {
            SpawnThing();
            dropTime = 0;
        }
    }

    private void SpawnThing()
    {
        float xVal = startDropX;
        float yVal = startDropY;
        float zVal = startDropZ;

        if (dropAlongX) xVal += Random.Range(0, dropRangeX);
        if (dropAlongY) yVal += Random.Range(0, dropRangeY);
        if (dropAlongZ) zVal += Random.Range(0, dropRangeZ);

        float prizeVal = Random.value;
        int newPriveVal = Mathf.RoundToInt(prizeVal * 100);
        if (newPriveVal == 100) newPriveVal -= 1;
        int randomPrize;
        
        if (Random.value < 0.7) randomPrize = dropListGood[newPriveVal];
        else randomPrize = dropListBad[newPriveVal];
        //int randomPrize = Mathf.RoundToInt(Random.value);
        GameObject obj = ObjectPool.Instance.GetPooledObjects(randomPrize);
        if (obj == null) return;
        obj.transform.position = new Vector3(xVal, yVal, zVal);
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(true);
    }

    private void UpdateScore(EventParameter evn)
    {
        currentScore += evn.intVar;
        if (currentScore < 0) currentScore = 0;

        EventManager.TriggerAction("UpdateScoreUI", new EventParameter() { intVar = currentScore });

        if (currentScore >= scoreLimit && !hasWon)
        {
            EventManager.TriggerAction("Victory", new EventParameter());
            hasWon = true;
        }
    }

    private List<T> ListRandomiser<T>(List<T> list, int numOfTimes)
    {
        for (int i = 0; i < numOfTimes; ++i)
        {
            for (int j = 0; j < list.Count; ++j)
            {
                int num = Mathf.FloorToInt(Random.value * (j + 1));
                T obj = list[j];
                list[j] = list[num];
                list[num] = obj;
            }
        }
        return list;
    }
}
