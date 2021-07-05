using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireRunManager : MonoBehaviour
{
    public float zPos = 30f;
    public float yPos = 1.5f;
    public float xPosRange = 28f;
    private float startXPos;

    public float spawnRate = 2.5f;
    private float spawnTime = 0;

    private bool isEndless = false;
    private bool isFall = false;
    private int scoreLimit = 1;
    private int currentScore = 0;
    private bool hasWon = false;

    private int currentObjectCount = 0;
    private int currentObjectRound = 0;

    [Header("Drop Values")]
    public List<Percentages> percentages;
    public bool enableDoubles = false;
    private bool doubleSpawn = false;
    public int startRoundForDouble = 3;
    //public int easyDropPercentage = 65;
    //public int easyDropId = 0;
    //public int normalDropPercentage = 25;
    //public int normalDropId = 1;
    //public int hardDropPercentage = 10;
    //public int hardDropId = 2;
    private List<int> dropList = new List<int>();

    private void Awake()
    {
        spawnTime = spawnRate - 0.1f;
        startXPos = -xPosRange * 0.5f;

        int maxNum = 0;
        for (int i = 0; i < percentages.Count; ++i)
        {
            //maxNum += percentages[i].percentage;
            //if (maxNum > 100)
            //{
            //    int temp = maxNum / maxNum
            //}
            AddToList(percentages[i].percentage, percentages[i].poolID, ref dropList);
        }

        //if (easyDropPercentage + normalDropPercentage + hardDropPercentage > 100)
        //{
        //    int largNum = Mathf.Max(Mathf.Max(easyDropPercentage, normalDropPercentage), hardDropPercentage);
        //    if (largNum == easyDropPercentage)
        //    {
        //        int num = 100 - (normalDropPercentage + hardDropPercentage);
        //        easyDropPercentage = easyDropPercentage - (easyDropPercentage - num);
        //    }
        //    else if (largNum == normalDropPercentage)
        //    {
        //        int num = 100 - (easyDropPercentage + hardDropPercentage);
        //        normalDropPercentage = normalDropPercentage - (normalDropPercentage - num);
        //    }
        //    else if (largNum == hardDropPercentage)
        //    {
        //        int num = 100 - (normalDropPercentage + easyDropPercentage);
        //        hardDropPercentage = hardDropPercentage - (hardDropPercentage - num);
        //    }
        //}
        //AddToList(easyDropPercentage, easyDropId, ref dropList);
        //AddToList(normalDropPercentage, normalDropId, ref dropList);
        //AddToList(hardDropPercentage, hardDropId, ref dropList);
    }

    private void Start()
    {
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
        isEndless = gm.isEndless;
        isFall = true;
    }

    private void Update()
    {
        if (isFall) spawnTime += Time.deltaTime;
        if (spawnTime >= spawnRate)
        {
            spawnTime = 0;
            SpawnThing();
            if (doubleSpawn) SpawnThing();
            if (currentObjectRound < 5) currentObjectCount++;
            if (currentObjectCount > 10)
            {
                currentObjectCount = 0;
                currentObjectRound++;
                if (currentObjectRound == startRoundForDouble && enableDoubles) doubleSpawn = true;
            }
        }
    }

    private void SpawnThing()
    {
        float xPos = startXPos + Random.Range(0, xPosRange);

        float prizeVal = Random.value;
        int newPriveVal = Mathf.RoundToInt(prizeVal * 100);
        if (newPriveVal == 100) newPriveVal -= 1;
        int randomPrize = dropList[newPriveVal];
        if (randomPrize > currentObjectRound) randomPrize = 0;

        GameObject obj = ObjectPool.Instance.GetPooledObjects(randomPrize);
        if (obj == null) return;
        obj.transform.position = new Vector3(xPos, yPos, zPos);
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(true);
    }

    private void UpdateScore(EventParameter evn)
    {
        currentScore += evn.intVar;
        if (currentScore < 0) currentScore = 0;

        EventManager.TriggerAction("UpdateScoreUI", new EventParameter() { intVar = currentScore });

        if (currentScore >= scoreLimit && !hasWon && !isEndless)
        {
            EventManager.TriggerAction("Victory", new EventParameter());
            hasWon = true;
        }
    }

    private void AddToList(int num, int numToAdd, ref List<int> list)
    {
        for (int i = 0; i < num; ++i)
        {
            list.Add(numToAdd);
        }
    }
}

[System.Serializable]
public class Percentages
{
    public string name;
    public int percentage;
    public int poolID;
}