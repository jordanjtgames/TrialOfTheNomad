using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class CameraShake : MonoBehaviour
{
    Camera cam;

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

    public static CameraShake instance;

    public static Volume postFX;

    float fxTime;
    public static bool startBlink = false;
    float startFOV;
    public AnimationCurve blinkAberration;
    public AnimationCurve blinkLensDist;
    public AnimationCurve blinkExtraFOV;
    public AnimationCurve blinkMotionBlur;
    public AnimationCurve blinkVignette;

    float additionalFOV = 0;

    void Start()
    {
        if (postFX == null)
            postFX = GameObject.Find("Sky and Fog Volume").GetComponent<Volume>();

        cam = transform.GetChild(0).GetComponent<Camera>();
        instance = this;

        
    }

    void Update()
    {
        
        if ((CombatMovement.isSprinting || CombatMovement.isSliding) && CombatMovement.inputDirRaw != Vector2.zero)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 80 + additionalFOV, Time.deltaTime * 6);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 75 + additionalFOV, Time.deltaTime * 6);


        if (startBlink) {
            fxTime += Time.unscaledDeltaTime * 4.25f;

            if (postFX.profile.TryGet<ChromaticAberration>(out var chromAb)) {
                chromAb.intensity.value = blinkAberration.Evaluate(1 - fxTime);
            }
            if (postFX.profile.TryGet<LensDistortion>(out var lensDis)) {
                lensDis.intensity.value = blinkLensDist.Evaluate(fxTime);
            }
            if (postFX.profile.TryGet<MotionBlur>(out var moBlur)) {
                moBlur.intensity.value = blinkMotionBlur.Evaluate(1 - fxTime * 0.25f);
            }
            if (postFX.profile.TryGet<Vignette>(out var vig)) {
                vig.intensity.value = blinkVignette.Evaluate(1- fxTime);
            }
            additionalFOV = blinkExtraFOV.Evaluate(fxTime);

            if (fxTime >= 1) {
                chromAb.active = false;
                lensDis.active = false;
                moBlur.active = false;
                vig.active = false;
                additionalFOV = 0;

                fxTime = 0;
                startBlink = false;
            }
        }
    }

    void LateUpdate()
    {
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
    }
    public void StartShake(float intensity)
    {
        //shakeIntensity = intensity;
        shakeIntensity = startIntensity * intensity;
        shakeRotation = startRotationIntensity * intensity;
        t = 1;
        shakeInterval = 1;
    }

    public static void StartBlinkFX()
    {
        startBlink = true;

        if (postFX.profile.TryGet<ChromaticAberration>(out var chromAb)) {
            chromAb.active = true;
        }
        if (postFX.profile.TryGet<LensDistortion>(out var lensDis)) {
            lensDis.active = true;
        }
        if (postFX.profile.TryGet<MotionBlur>(out var moBlur)) {
            moBlur.active = true;
        }
        if (postFX.profile.TryGet<Vignette>(out var vig)) {
            vig.active = true;
        }
    }
}
