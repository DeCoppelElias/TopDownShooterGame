using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CurrentSceneManager : MonoBehaviour
{
    public UnityEvent onSceneLoad;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PerformAfterRealDelay(0.1f, () => onSceneLoad.Invoke()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PerformAfterRealDelay(float delay, Action action)
    {
        yield return new WaitForSecondsRealtime(delay);

        action();
    }
}
