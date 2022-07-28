using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowString : MonoBehaviour
{
    //public LineRenderer line;

    public SkinnedMeshRenderer line;
    public Transform bowL_Bone;
    public Transform bowR_Bone;
    public Transform bowC_Bone;

    public Transform bow_L;
    public Transform bow_R;
    public Transform bow_DrawPos;

    public GameObject bows;

    public static bool bowDrawn = false;
    public Transform bowPos;

    public float drawTime = 0;
    public float drawSpeed = 1;
    float circleScale = 1.65f;

    public Transform crosshairParent;
    public RawImage bowCrosshair_1;
    public RawImage bowCrosshair_2;
    public RawImage bowCrosshair_3;

    public PlayerLocomotion pl;

    void LateUpdate()
    {
        line.enabled = bows.activeSelf;

        //line.SetPosition(0, bow_L.position);
        //line.SetPosition(1, bow_DrawPos.position);
        //line.SetPosition(2, bow_R.position);


        bowL_Bone.position = bow_L.position;
        bowR_Bone.position = bow_R.position;

        if (pl.attackReleaseTime > 0) { //AttackingState.hasReleasedAttack
            if(pl.attackReleaseTime < 0.03f) {
                bowC_Bone.position = ((bow_L.position + bow_R.position) / 2f) + Camera.main.transform.TransformDirection(Vector3.forward * 0.142f);
            } else {
                bowC_Bone.position = Vector3.Lerp(bowC_Bone.position,(bow_L.position + bow_R.position) / 2f,Time.deltaTime * 25f);
            }

        } else
            bowC_Bone.position = bow_DrawPos.position;

        //bowPos.transform.localScale = Vector3.Lerp(bowPos.transform.localScale, 
        //    bowDrawn ? new Vector3(1.59051414f, 0.7051414f, 0.9051414f) : new Vector3(0.9051414f, 0.9051414f, 0.9051414f), Time.deltaTime * 9f);
        if (bowDrawn && pl.currentWeapon == PlayerLocomotion.Holding.Bow) {
            drawTime += Time.deltaTime * drawSpeed;
            drawTime = Mathf.Clamp(drawTime,0f,1.0f);
        } else {
            drawTime = 0;
        }

        float s = 0.1025f;//0.025f
        crosshairParent.localScale = Vector3.Lerp(Vector3.one, new Vector3(s, s, s), Mathf.Clamp(drawTime, 0f, 1.0f)) * circleScale;
        bowCrosshair_1.color = new Color(1, 1, 1, drawTime);
        bowCrosshair_2.color = new Color(1, 1, 1, drawTime);
        bowCrosshair_3.color = new Color(1, 1, 1, drawTime);

        switch (drawTime) {
            case var _ when drawTime < 0.33f:
                bowCrosshair_1.enabled = true;
                bowCrosshair_2.enabled = false;
                bowCrosshair_3.enabled = false;
                break;
            case var _ when drawTime < 0.86f:
                bowCrosshair_1.enabled = false;
                bowCrosshair_2.enabled = true;
                bowCrosshair_3.enabled = false;
                break;
            case var _ when drawTime < 0.99f:
                bowCrosshair_1.enabled = false;
                bowCrosshair_2.enabled = true;
                bowCrosshair_3.enabled = false;//true
                break;
        }
    }
}
