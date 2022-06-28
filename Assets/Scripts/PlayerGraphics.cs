using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;
using System.Linq;

public class PlayerGraphics : MonoBehaviour
{
    public PlayerLocomotion pl;

    public Color blinkColour;
    public Color telekinesisColour;
    public Color elementalColour;
    public Color rockColour;
    public Color laserColour;

    public Color currentMagicColour;
    public SkinnedMeshRenderer magicHand;
    public VisualEffect magicIntersect;
    public VisualEffect magicLines;
    public Light topMagicLight;
    public Light bottomMagicLight;

    public Transform magicRocks;
    public List<Renderer> rocks;
    private List<Vector3> rockRots;

    public static float extraHandIntensity = 0f;

    public SkinnedMeshRenderer magicFinder;
    public Transform magicFinder_A;
    public Transform magicFinder_B;
    public Transform magicFinder_C;
    public Transform finder;
    public Transform A_Rot;
    public Transform B_Rot;
    public Transform C_Rot;
    public static Vector3 finderTargetPos;
    public static Vector3 finderLerpPos;

    public VisualEffect TK_VFX;
    public Light TK_Light;
    public static bool grabbedVFX = false;

    public static bool searchingBlink = false;
    public VisualEffect blinkHead;
    public static Vector3 blinkTargetPos;
    bool hasResetBlinkVFX = false;

    void Start()
    {
        pl = GetComponent<PlayerLocomotion>();
        rocks = magicRocks.GetComponentsInChildren<Renderer>().ToList();
        rockRots = new List<Vector3>();
        rockRots.Add(new Vector3(10,0,0));
        rockRots.Add(new Vector3(9,0,0));
        rockRots.Add(new Vector3(11,0,0));
        rockRots.Add(new Vector3(8,0,0));
        rockRots.Add(new Vector3(12,0,0));
        rockRots.Add(new Vector3(14,0,0));
        rockRots.Add(new Vector3(16,0,0));
        rockRots.Add(new Vector3(7,0,0));
        rockRots.Add(new Vector3(11,0,0));
        rockRots.Add(new Vector3(10,0,0));
        rockRots.Add(new Vector3(15,0,0));
        rockRots.Add(new Vector3(12,0,0));
        rockRots.Add(new Vector3(13,0,0));
        rockRots.Add(new Vector3(14,0,0));
        rockRots.Add(new Vector3(9,0,0));
        rockRots.Add(new Vector3(8,0,0));

        rockRots.Add(new Vector3(10, 0, 0));
        rockRots.Add(new Vector3(9, 0, 0));
        rockRots.Add(new Vector3(11, 0, 0));
        rockRots.Add(new Vector3(8, 0, 0));
        rockRots.Add(new Vector3(12, 0, 0));
        rockRots.Add(new Vector3(14, 0, 0));
        rockRots.Add(new Vector3(16, 0, 0));
        rockRots.Add(new Vector3(7, 0, 0));
        rockRots.Add(new Vector3(11, 0, 0));
        rockRots.Add(new Vector3(10, 0, 0));
        rockRots.Add(new Vector3(15, 0, 0));
        rockRots.Add(new Vector3(12, 0, 0));
        rockRots.Add(new Vector3(13, 0, 0));
        rockRots.Add(new Vector3(14, 0, 0));
        rockRots.Add(new Vector3(9, 0, 0));
        rockRots.Add(new Vector3(8, 0, 0));

        
    }

    void Update()
    {
        switch (pl.magicID) {
            case 0:
                currentMagicColour = Color.black;
                break;
            case 1:
                currentMagicColour = blinkColour;
                break;
            case 2:
                currentMagicColour = telekinesisColour;
                break;
            case 3:
                currentMagicColour = elementalColour;
                break;
            case 4:
                currentMagicColour = rockColour;
                break;
            case 5:
                currentMagicColour = laserColour;
                break;
        }

        if (extraHandIntensity > 1)
            extraHandIntensity -= Time.deltaTime * 30f;
        else
            extraHandIntensity = 1f;

        topMagicLight.color = currentMagicColour * extraHandIntensity;
        bottomMagicLight.color = currentMagicColour * extraHandIntensity;
        magicIntersect.SetVector4("Colour", currentMagicColour * extraHandIntensity);
        magicLines.SetVector4("OrbColour", currentMagicColour * extraHandIntensity);
        magicLines.SetVector4("OrbColour_2", currentMagicColour * extraHandIntensity);
        magicHand.materials[0].SetColor("_HandColour", currentMagicColour * extraHandIntensity);
        magicHand.materials[1].SetColor("_HandColour", currentMagicColour * extraHandIntensity);
        magicFinder.material.SetColor("_Colour", currentMagicColour * 1f);


        magicLines.SetFloat("ForceIntensity", pl.magicID == 4 || pl.magicID == 2 ? 0.2f : 0.99f);
        magicLines.SetFloat("Opacity", 0.2f);
        magicRocks.gameObject.SetActive(pl.magicID == 4);

        for (int i = 0; i < rocks.Count; i++) {
            rocks[i].transform.Rotate(rockRots[i] * Time.deltaTime * 10f);//65f
            float scale = Mathf.Abs(Mathf.Sin(((rockRots[i].x + rockRots[i].y + rockRots[i].z) * 2) + (Time.time * 2.5f))) + 0.6f;
            //Debug.Log(scale);
            if (i < 8)
                rocks[i].transform.localScale = new Vector3(1, 1, 1.5f) * scale * 0.7f;
            else
                rocks[i].transform.localScale = Vector3.one * 0.01f;
        }

        if (grabbedVFX) {
            TK_Light.GetComponent<HDAdditionalLightData>().intensity = Mathf.Lerp(TK_Light.GetComponent<HDAdditionalLightData>().intensity, 50, Time.deltaTime * 35f);
            TK_VFX.SetFloat("Opacity", Mathf.Lerp(TK_VFX.GetFloat("Opacity"), 0.18f, Time.deltaTime * 15f));
            float displace = Mathf.Clamp(Inputs.lookSmoothed.magnitude / 25f + BlockingState.TKpushDirLerp, 0.0f, 1.0f);
            TK_VFX.SetFloat("Displace", Mathf.Lerp(TK_VFX.GetFloat("Displace"), displace, Time.deltaTime * 45f));
        } else {
            TK_Light.GetComponent<HDAdditionalLightData>().intensity = Mathf.Lerp(TK_Light.GetComponent<HDAdditionalLightData>().intensity, 0, Time.deltaTime * 35f);
            TK_VFX.SetFloat("Opacity", Mathf.Lerp(TK_VFX.GetFloat("Opacity"), 0.0f, Time.deltaTime * 45f));
            TK_VFX.SetFloat("Displace", Mathf.Lerp(TK_VFX.GetFloat("Displace"), 0.0f, Time.deltaTime * 15f));
        }

        if (searchingBlink) {
            float extraBlinkIntesnity = Mathf.Clamp(Inputs.lookSmoothed.magnitude / 25f + BlockingState.TKpushDirLerp, 0.0f, 1.0f);
            blinkHead.SetFloat("Opacity", Mathf.Lerp(blinkHead.GetFloat("Opacity"), 0.18f + extraBlinkIntesnity, Time.deltaTime * 24f));
            blinkHead.SetFloat("Displace", Mathf.Lerp(blinkHead.GetFloat("Displace"), extraBlinkIntesnity * 10f, Time.deltaTime * 24f));
            blinkHead.transform.position = Vector3.Lerp(blinkHead.transform.position, blinkTargetPos, Time.smoothDeltaTime * 20f);
            blinkHead.transform.localScale = Vector3.Lerp(blinkHead.transform.localScale, new Vector3(1,1,1), Time.deltaTime * 32f);
            if (!hasResetBlinkVFX) {
                blinkHead.transform.position = blinkTargetPos;
                blinkHead.Stop();
                blinkHead.Play();
                hasResetBlinkVFX = true;
            }
            
        } else {
            blinkHead.SetFloat("Opacity", Mathf.Lerp(blinkHead.GetFloat("Opacity"), 0.0f, Time.deltaTime * 64f));
            blinkHead.SetFloat("Displace", Mathf.Lerp(blinkHead.GetFloat("Displace"), 0f, Time.deltaTime * 24f));
            blinkHead.transform.localScale = Vector3.Lerp(blinkHead.transform.localScale, new Vector3(5, 1, 5), Time.deltaTime * 42f);
            hasResetBlinkVFX = false;
        }

    }

    void LateUpdate() {
        finder.gameObject.SetActive(grabbedVFX || searchingBlink);
        finder.position = (pl.cam.transform.position + finderTargetPos) / 2;
        finder.LookAt(finderTargetPos);

        A_Rot.transform.Rotate(Vector3.up * 218f * Time.deltaTime);
        B_Rot.transform.Rotate(Vector3.up * -288f * Time.deltaTime);
        C_Rot.transform.Rotate(Vector3.up * 162f * Time.deltaTime);
        magicFinder_A.localRotation = A_Rot.localRotation;
        magicFinder_B.localRotation = B_Rot.localRotation;
        magicFinder_C.localRotation = C_Rot.localRotation;

        finderLerpPos = Vector3.Lerp(finderLerpPos, finder.position, Time.deltaTime * 5f);

        magicFinder_A.position = pl.cam.transform.position + pl.cam.transform.TransformDirection(new Vector3(-1f*0.7f,-1f*0.7f,1f));
        magicFinder_B.position = finderLerpPos;
        magicFinder_C.position = finderTargetPos;
    }
}
