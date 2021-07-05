using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    private Cardset cardset;
    public List<string> itemSetsInPlay;
    public List<CardData> items = new List<CardData>();
    private List<Material> cardBacks = new List<Material>();

    public GameObject cardObj;
    public List<Card> cards;

    private int horizontalGridNum = 4;
    private int verticalGridNum = 2;
    public float spaceBetweenCards = 4f;
    public Vector2 cardScale = new Vector2(4.2f, 6);
    public Vector2 bounds = new Vector2(20, 20);
    public int victoryPairNum = 0;

    private int? storedCard = null;
    private int? storedId = null;

    private void Awake()
    {
        cardset = GetComponent<Cardset>();
    }

    private void Start()
    {
        items = cardset.GetItems(itemSetsInPlay);
        cardBacks = cardset.GetBacks(itemSetsInPlay);
        //Vector2 dividedPosition = new Vector2(bounds.x / (horizontalGridNum - 1), bounds.y / (verticalGridNum - 1));

        EventManager.StartListening("CompareCard", HandleCardPick);
        EventManager.StartListening("MakeGame", MakeCards);
    }

    private void OnDisable()
    {
        EventManager.StopListening("CompareCard", HandleCardPick);
        EventManager.StopListening("MakeGame", MakeCards);
    }

    private void MakeCards(EventParameter evn)
    {
        verticalGridNum = GameManager.Instance.verticalGridValue;
        horizontalGridNum = GameManager.Instance.horizontalGridValue;

        if (verticalGridNum * horizontalGridNum % 2 != 0) horizontalGridNum++;

        bounds = new Vector2((cardScale.x * verticalGridNum) + ((verticalGridNum - 1) * spaceBetweenCards), (cardScale.y * horizontalGridNum) + ((horizontalGridNum - 1) * spaceBetweenCards));

        for (int i = 0; i < horizontalGridNum; ++i)
        {
            for (int j = 0; j < verticalGridNum; ++j)
            {
                float ranXStart;
                float ranYStart;
                if (Random.value > 0.5)
                {
                    ranXStart = Random.value > 0.5 ? 110 : -110;
                    ranYStart = Random.Range(-50, 51);
                }
                else
                {
                    ranYStart = Random.value > 0.5 ? 60 : -60;
                    ranXStart = Random.Range(-80, 81);
                }

                float xVal = (-bounds.x * 0.5f) + ((cardScale.x + spaceBetweenCards) * j) + (cardScale.x * 0.5f);
                float yVal = (bounds.y * 0.5f) - ((cardScale.y + spaceBetweenCards) * i) - (cardScale.y * 0.5f);
                var newObj = Instantiate(cardObj, new Vector3(ranXStart, ranYStart, 1), new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w));
                cards.Add(newObj.GetComponent<Card>());
                float delayTime = ((i + 1) * (j + 1)) * 0.1f + 0.1f;
                int ranCardBack = Mathf.RoundToInt(Random.value * (cardBacks.Count - 1));
                cards[cards.Count - 1].cardBack.material = cardBacks[ranCardBack];
                StartCoroutine(cards[cards.Count - 1].MoveCardIn(delayTime, 1.0f, new Vector3(xVal, yVal, 0)));
            }
        }
        cards = ListRandomiser(cards, 3);
        items = ListRandomiser(items, 3);

        victoryPairNum = (verticalGridNum * horizontalGridNum) / 2;

        int posData = 0;
        int itemNumber = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (i % 2 == 0) itemNumber++;
            if (itemNumber > items.Count) itemNumber = 1;
            cards[i].SetCardData(items[itemNumber - 1]);
            cards[i].cardPos = posData;
            posData++;
        }
    }

    private void HandleCardPick(EventParameter evn)
    {
        if (storedCard == null)
        {
            storedCard = evn.intVar;
            storedId = evn.intVar2;

            EventManager.TriggerAction("CardSelected", new EventParameter() { boolVar = true });
        }
        else
        {
            int tempId = (int)storedId;
            int tempPos = (int)storedCard;

            if (tempId == evn.intVar2)
            {
                EventManager.TriggerAction("CardSelected", new EventParameter() { boolVar = true });
                cards[tempPos].correctCard.SetActive(true);
                cards[evn.intVar].correctCard.SetActive(true);

                victoryPairNum--;
                if (victoryPairNum == 0) EventManager.TriggerAction("Victory", new EventParameter());
            }
            else
            {
                cards[tempPos].FlipWrongCardsBack();
                cards[evn.intVar].FlipWrongCardsBack();
            }

            storedCard = null;
            storedId = null;
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