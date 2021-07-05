using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 speedVal;
    public bool randomiseXDirection = false;
    public int pointIncrement = 1;
    public int explosionID;

    private Transform model;
    private float rotationSpeed;

    private void Awake()
    {
        model = GetComponentsInChildren<Transform>()[1];
    }

    private void OnEnable()
    {
        if (randomiseXDirection && Random.value < 0.5) speedVal.x *= -1;
        model.rotation = new Quaternion(0, Random.Range(-180, 180), 0, 0);
        rotationSpeed = Random.Range(0, 120);
    }

    private void Update()
    {
        transform.Translate(speedVal * Time.deltaTime);
        model.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone")) gameObject.SetActive(false);
        else if (other.CompareTag("ScoreZone"))
        {
            EventManager.TriggerAction("ScoreChange", new EventParameter() { intVar = pointIncrement });
        }
        else if (other.CompareTag("Terrain")) speedVal.x = -speedVal.x;
        else if (other.TryGetComponent(out PlayerControl play))
        {
            if (play.isEndless)
            {
                GameObject obj = ObjectPool.Instance.GetPooledObjects(play.explosionID);
                if (obj == null) return;
                obj.transform.position = play.transform.position;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);
                play.gameObject.SetActive(false);

                EventManager.TriggerAction("Defeat", new EventParameter());
            }
            else
            {
                GameObject obj = ObjectPool.Instance.GetPooledObjects(explosionID);
                if (obj == null) return;
                obj.transform.position = transform.position;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);

                EventManager.TriggerAction("ScoreChange", new EventParameter() { intVar = -1 });
                gameObject.SetActive(false);
            }
        }
    }
}
