using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperManager : MonoBehaviour
{
    [SerializeField] private int currentScore = 0;
    private int maxScore = 10;
    private int horizontalGridNum = 4;
    private int verticalGridNum = 2;
    private int minJumpNum = 1;
    private int maxJumpNum = 3;
    private Vector2 bounds = new Vector2(20, 20);
    private List<Jumper> jumpers = new List<Jumper>();

    public float riseRate = 2f;
    private float riseCounter = 0;
    private bool hasWon = false;

    public GameObject jumperObj;
    public float spaceBetweenJumps = 4f;
    public Vector2 jumperScale = new Vector2(2, 2);

    [Header("Jumper Material")]
    public Material[] jumperMats;
    public Material[] correctMats;
    public Material[] incorrectMats;

    private void Start()
    {
        //MakeJumpers(new EventParameter());
        EventManager.StartListening("StartGame", MakeJumpers);
        EventManager.StartListening("ScoreChange", UpdateScore);
    }

    private void OnDisable()
    {
        EventManager.StopListening("StartGame", MakeJumpers);
        EventManager.StopListening("ScoreChange", UpdateScore);
    }

    private void UpdateScore(EventParameter evn)
    {
        currentScore += evn.intVar;
        if (currentScore < 0) currentScore = 0;
        if (currentScore > maxScore) currentScore = maxScore;

        EventManager.TriggerAction("UpdateScoreUI", new EventParameter() { intVar = currentScore });

        if (currentScore == maxScore && !hasWon)
        {
            hasWon = true;
            EventManager.TriggerAction("Victory", new EventParameter());
        }
    }

    private void Update()
    {
        riseCounter += Time.deltaTime;
        if (riseCounter > riseRate && jumpers.Count >= maxJumpNum)
        {
            int jumpNum = Random.Range(minJumpNum, maxJumpNum + 1);
            for (int i = 0; i < jumpNum; ++i)
            {
                bool isGood;
                Material mat;
                if (Random.value < 0.7f)
                {
                    isGood = true;
                    mat = correctMats[Mathf.RoundToInt(Random.value * (correctMats.Length - 1))];
                }
                else
                {
                    isGood = false;
                    mat = incorrectMats[Mathf.RoundToInt(Random.value * (incorrectMats.Length - 1))];
                }
                Jumper ranJmp = jumpers[Mathf.RoundToInt(Random.value * (jumpers.Count - 1))];
                if (!ranJmp.inUse)
                {
                    ranJmp.SetMatAndAllegiance(mat, isGood);
                    ranJmp.StartJump();
                    riseCounter = 0;
                }
            }
        }
    }

    private void MakeJumpers(EventParameter evn)
    {
        GameManager gm = GameManager.Instance;
        horizontalGridNum = gm.horizontalGridValue;
        verticalGridNum = gm.verticalGridValue;
        maxScore = gm.maxScoreValue;

        bounds = new Vector2((jumperScale.x * verticalGridNum) + ((verticalGridNum - 1) * spaceBetweenJumps), (jumperScale.y * horizontalGridNum) + ((horizontalGridNum - 1) * spaceBetweenJumps));

        for (int i = 0; i < horizontalGridNum; ++i)
        {
            for (int j = 0; j < verticalGridNum; ++j)
            {
                float xVal = (-bounds.x * 0.5f) + ((jumperScale.x + spaceBetweenJumps) * j) + (jumperScale.x * 0.5f);
                float yVal = (bounds.y * 0.5f) - ((jumperScale.y + spaceBetweenJumps) * i) - (jumperScale.y * 0.5f);
                var newObj = Instantiate(jumperObj, new Vector3(xVal, 0, yVal), new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w));
                jumpers.Add(newObj.GetComponent<Jumper>());
                jumpers[jumpers.Count - 1].transform.GetChild(1).GetComponent<Renderer>().material = jumperMats[Mathf.RoundToInt(Random.value * (jumperMats.Length - 1))];
            }
        }
    }
}
