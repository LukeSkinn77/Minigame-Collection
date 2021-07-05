using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeParticles : MonoBehaviour
{
    public float timeToDestroy;

    private void OnEnable()
    {
        Invoke("WaitDestruct", timeToDestroy);
    }

    private void WaitDestruct()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
