using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[SerializeField]
	private float shakeDuration = 0.5f;
	[SerializeField]
	private float shakeAmount = 0.7f;

	private float shakeStart = 0;
	private bool shaking = false;
	private Vector3 originalPos;
	public void ShakeScreen()
	{
		if (!shaking)
        {
			shaking = true;
			shakeStart = Time.time;
			originalPos = Camera.main.transform.position;
		}
	}

    private void Update()
    {
		if (shaking && Time.timeScale > 0)
		{
			if (Time.time - shakeStart <= shakeDuration)
            {
				Camera.main.transform.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;
			}
            else
            {
				Camera.main.transform.position = originalPos;
				shaking = false;
            }
		}
	}
}
