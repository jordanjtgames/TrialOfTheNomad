using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public Camera cam;
    public static CamShake instance;

    public float startIntensity;
    public float startRotationIntensity;
    public float fadeTime;

    public static float shakeIntensity = 1;
    private float t;
    private float shakeRotation;

    float shakeInterval;
    float shakeX;
    float shakeY;
    float shakeRot;

    float targetFOV = 75;
    float FOVSpeed = 6;//4

    void Start()
    {
        instance = this;
    }

    public void Shake(float intensity, float newFadeTime) {
        fadeTime = newFadeTime;
        shakeIntensity = startIntensity * intensity;
        shakeRotation = startRotationIntensity * intensity;
        t = 1;
        shakeInterval = 1;
    }

    void Update()
    {
        //shakeIntensity = intensity;
    }

    void LateUpdate() {
        if (t > 0) {
            t -= Time.unscaledDeltaTime * fadeTime;
            shakeIntensity = shakeIntensity * t;
            shakeRotation = shakeRotation * t;

            if (shakeInterval > 0)
                shakeInterval -= Time.unscaledDeltaTime * 30;
            else {
                shakeX = Random.Range(-1f, 1f) * shakeIntensity;
                shakeY = Random.Range(-1f, 1f) * shakeIntensity;
                shakeRot = Random.Range(-1f, 1f) * shakeRotation;
                shakeInterval = 1;
            }

            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(shakeX, shakeY, 0), Time.unscaledDeltaTime * 103);
            cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(new Vector3(0, 0, shakeRot)), Time.unscaledDeltaTime * 103);


        }

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * FOVSpeed);
    }

    public void SetTargetFOV(float newTarget) {
        targetFOV = newTarget;
    }
}
