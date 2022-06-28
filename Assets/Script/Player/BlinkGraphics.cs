using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BlinkGraphics : MonoBehaviour
{
    public Transform testMagicHand;
    public Transform blinkUpArrowModel;
    public Transform blinkUpArrow;
    
    bool hasSetupBlink = false;

    public BlinkFinder bf_1;
    public BlinkFinder bf_2;

    public Light blinkHeadLight;
    public Transform blinkHeadGraphic;
    Vector3 targetHeadPos;
    public Transform blinkFinderEnd;

    public SkinnedMeshRenderer magicHand;
    public Light handLight_Top;
    public Light handLight_Bottom;
    public VisualEffect streaksVFX;
    public VisualEffect wispsVFX;


    public VisualEffect arrowVFX;
    public VisualEffect headVFX;

    void Start()
    {
        testMagicHand.parent = null;
        testMagicHand.position = Vector3.zero;
        testMagicHand.eulerAngles = Vector3.zero;

        blinkUpArrowModel.parent = null;
        blinkUpArrowModel.position = Vector3.zero;
        blinkUpArrowModel.eulerAngles = Vector3.zero;
    }

    void Update()
    {
        testMagicHand.GetComponent<VisualEffect>().enabled = CombatMovement.isFindingBlink;
        streaksVFX.enabled = CombatMovement.isFindingBlink;
        wispsVFX.SetFloat("Opacity", CombatMovement.isFindingBlink ? 1 : 0);
        magicHand.materials[0].SetFloat("_Opacity", CombatMovement.isFindingBlink ? 1 : 0);
        magicHand.materials[1].SetFloat("_Opacity", CombatMovement.isFindingBlink ? 1 : 0);
        handLight_Top.intensity = CombatMovement.isFindingBlink ? 3.84e+07f : 0;
        handLight_Bottom.intensity = CombatMovement.isFindingBlink ? 3.84e+07f : 0;

        if (CombatMovement.isFindingBlink) {
            if (!hasSetupBlink) {
                bf_1.SetUpMidpoint();
                bf_2.SetUpMidpoint();
                blinkHeadGraphic.position = blinkFinderEnd.position;
                targetHeadPos = blinkFinderEnd.position;
                hasSetupBlink = true;
            }

            if (CombatMovement.blinkWallrunFound) {
                headVFX.SetFloat("Opacity", 0.1f);
                arrowVFX.SetFloat("Opacity", 1);

                arrowVFX.SetFloat("ClimbOrWallrun", 1);
                if (CombatMovement.blinkWallrunID == 0)
                    blinkUpArrow.eulerAngles = blinkFinderEnd.eulerAngles;
                else if (CombatMovement.blinkWallrunID == 1)
                    blinkUpArrow.eulerAngles = blinkFinderEnd.eulerAngles + new Vector3(0, 180, 0);
            }
            else if(CombatMovement.blinkMantelFound) {
                headVFX.SetFloat("Opacity", 0.1f);
                arrowVFX.SetFloat("Opacity", 1);

                arrowVFX.SetFloat("ClimbOrWallrun", 0);
                blinkUpArrow.eulerAngles = blinkFinderEnd.eulerAngles;
            }
            else {
                headVFX.SetFloat("Opacity", 1);
                arrowVFX.SetFloat("Opacity", 0);
            }

            blinkHeadLight.transform.Rotate(new Vector3(1, 1, 1) * Time.deltaTime * 50);
        }
        else {
            hasSetupBlink = false;
        }

        blinkHeadGraphic.position = Vector3.Lerp(blinkHeadGraphic.position, targetHeadPos, Time.deltaTime * 25);
        //blinkUpArrow.rotation = blinkFinderEnd.rotation;
    }

    void FixedUpdate()
    {
        targetHeadPos = blinkFinderEnd.position;

    }
}
