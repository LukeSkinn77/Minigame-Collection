using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBomb : MonoBehaviour
{
    public int explosionID;
    public int pointIncrement = 1;

    [Header("X Range")]
    public bool fallX = false;
    public float fallSpeedX = 5.0f;
    [Header("Y Range")]
    public bool fallY = true;
    public float fallSpeedY = 5.0f;
    [Header("Z Range")]
    public bool fallZ = false;
    public float fallSpeedZ = 5.0f;

    private float xVal;
    private float yVal;
    private float zVal;

    private Transform model;
    private float rotationSpeed;

    private void Awake()
    {
        xVal = 0;
        yVal = 0;
        zVal = 0;
        if (fallX) xVal = fallSpeedX;
        if (fallY) yVal = -fallSpeedY;
        if (fallZ) zVal = fallSpeedZ;

        model = GetComponentsInChildren<Transform>()[1];
    }

    private void OnEnable()
    {
        rotationSpeed = Random.Range(0, 120);
    }

    private void Update()
    {
        transform.Translate(new Vector3(xVal, yVal, zVal) * Time.deltaTime);
        model.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        gameObject.SetActive(false);

        GameObject obj = ObjectPool.Instance.GetPooledObjects(explosionID);
        if (obj == null) return;
        obj.transform.position = transform.position;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(true);
        EventManager.TriggerAction("ScoreChange", new EventParameter() { intVar = pointIncrement });
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
