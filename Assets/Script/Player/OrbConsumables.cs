using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OrbConsumables : MonoBehaviour
{
    public VisualEffect OrbVFX;
    float selfDestructTime = 0.5f;//0.02f
    bool selfDestruct = false;
    Transform playerTransform;
    public AnimationCurve attractionOverDistance;
    float attractionMultiplier = 1;
    public int orbID = 1;
    float wakeUpTime = 0;

    void Start()
    {
        playerTransform = Camera.main.transform;
    }

    void Update()
    {
        if(wakeUpTime <= 1)
            wakeUpTime += Time.deltaTime;

        float dist = Vector3.Distance(transform.position, playerTransform.position - Vector3.up);

        if (dist <= 10) {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position - Vector3.up, Time.deltaTime * (attractionOverDistance.Evaluate(dist) * attractionMultiplier * wakeUpTime));
            if (dist < 0.55f && !selfDestruct) {
                if (GameObject.Find("Player"))
                    GameObject.Find("_MyPlayer").transform.Find("Player").SendMessage("StartGlow", new Color(orbID == 1 ? 1 : 0, orbID == 2 ? 1 : 0, orbID == 3 ? 1 : 0), SendMessageOptions.DontRequireReceiver);
                if (GameObject.Find("PlayerController")) {
                    Color sendColour = new Color(orbID == 1 ? 1 : 0, orbID == 2 ? 1 : 0, orbID == 3 ? 1 : 0);
                    GameObject.Find("_MyPlayer").transform.Find("PlayerController").SendMessage("StartGlow", sendColour, SendMessageOptions.DontRequireReceiver);
                }
                selfDestruct = true;
            }
        }

        if (selfDestruct) {
            selfDestructTime -= Time.unscaledDeltaTime;

            OrbVFX.SetFloat("Alpha", Mathf.Lerp(OrbVFX.GetFloat("Alpha"), 0f, Time.deltaTime * 16f));
            OrbVFX.SetFloat("SizeMod", Mathf.Lerp(OrbVFX.GetFloat("SizeMod"), 0f, Time.deltaTime * 16f));

        }
        if (selfDestructTime <= 0) {
            
            Destroy(transform.parent.gameObject);
        }
    }
}
