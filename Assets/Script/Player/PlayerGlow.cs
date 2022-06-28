using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlow : MonoBehaviour
{
    public float timeMultiplier = 1;
    float t = 0;
    bool isAnimating = false;
    bool hasResetValues = false;

    public SkinnedMeshRenderer armsRend;
    public SkinnedMeshRenderer WSA_Rend;
    public SkinnedMeshRenderer slideLegsRend;
    public SkinnedMeshRenderer legsRend;

    public AnimationCurve intensityOverTime;
    public AnimationCurve unfillOverTime;
    public AnimationCurve colourLerpOverTime;

    public float defaultUnfill = 2.17f;
    public Color defaultColour;
    Color targetColour;

    public Color healthColour;
    public Color speedColour;
    public Color manaColour;

    void Start()
    {
        //armsRend.material.SetFloat("_RimLightColorPower", 10);
    }

    void Update()
    {
        if (isAnimating) {
            if(targetColour == speedColour) {
                if (t <= 0.74f) {
                    t += Time.unscaledDeltaTime * timeMultiplier;
                }
                else {
                    if (!CombatMovement.isSpeedBoost)
                        t += Time.unscaledDeltaTime * timeMultiplier;
                }
            }
            
            

            armsRend.material.SetFloat("_RimLightColorPower", intensityOverTime.Evaluate(t));
            WSA_Rend.material.SetFloat("_RimLightColorPower", intensityOverTime.Evaluate(t));
            legsRend.material.SetFloat("_RimLightColorPower", intensityOverTime.Evaluate(t));
            slideLegsRend.material.SetFloat("_RimLightColorPower", intensityOverTime.Evaluate(t));

            armsRend.material.SetColor("_RimLightColor", Color.Lerp(defaultColour,targetColour, colourLerpOverTime.Evaluate(t)));
            WSA_Rend.material.SetColor("_RimLightColor", Color.Lerp(defaultColour,targetColour, colourLerpOverTime.Evaluate(t)));
            legsRend.material.SetColor("_RimLightColor", Color.Lerp(defaultColour, targetColour, colourLerpOverTime.Evaluate(t)));
            slideLegsRend.material.SetColor("_RimLightColor", Color.Lerp(defaultColour, targetColour, colourLerpOverTime.Evaluate(t)));

            armsRend.material.SetFloat("_RimLightUnfill", unfillOverTime.Evaluate(t));
            WSA_Rend.material.SetFloat("_RimLightUnfill", unfillOverTime.Evaluate(t));
            legsRend.material.SetFloat("_RimLightUnfill", unfillOverTime.Evaluate(t));
            slideLegsRend.material.SetFloat("_RimLightUnfill", unfillOverTime.Evaluate(t));


            if (t >= 1)
                isAnimating = false;
        }
        else {
            if (!hasResetValues) {
                armsRend.material.SetFloat("_RimLightColorPower", 1.0f);
                WSA_Rend.material.SetFloat("_RimLightColorPower", 1.0f);
                legsRend.material.SetFloat("_RimLightColorPower", 1.0f);
                slideLegsRend.material.SetFloat("_RimLightColorPower", 1.0f);

                armsRend.material.SetColor("_RimLightColor", defaultColour);
                WSA_Rend.material.SetColor("_RimLightColor", defaultColour);
                legsRend.material.SetColor("_RimLightColor", defaultColour);
                slideLegsRend.material.SetColor("_RimLightColor", defaultColour);

                armsRend.material.SetFloat("_RimLightInLight", 1.0f);
                WSA_Rend.material.SetFloat("_RimLightInLight", 1.0f);
                legsRend.material.SetFloat("_RimLightInLight", 1.0f);
                slideLegsRend.material.SetFloat("_RimLightInLight", 1.0f);

                armsRend.material.SetFloat("_RimLightUnfill", defaultUnfill);
                WSA_Rend.material.SetFloat("_RimLightUnfill", defaultUnfill);
                legsRend.material.SetFloat("_RimLightUnfill", defaultUnfill);
                slideLegsRend.material.SetFloat("_RimLightUnfill", defaultUnfill);
                hasResetValues = true;
            }
        }
    }

    void StartGlow(Color newColour)
    {
        //Debug.LogError(9);

        armsRend.material.SetFloat("_RimLightInLight", 0.0f);
        WSA_Rend.material.SetFloat("_RimLightInLight", 0.0f);
        legsRend.material.SetFloat("_RimLightInLight", 0.0f);
        slideLegsRend.material.SetFloat("_RimLightInLight", 0.0f);

        t = 0;
        isAnimating = true;
        hasResetValues = false;

        if (newColour == new Color(1, 0, 0))
            targetColour = healthColour;
        else if (newColour == new Color(0, 1, 0)) {
            targetColour = speedColour;
            CombatMovement.speedBoostTimer = CombatMovement.staticMaxBoostTime;
            CombatMovement.isSpeedBoost = true;
        }
        else if (newColour == new Color(0, 0, 1))
            targetColour = manaColour;
        else
            targetColour = newColour;
    }
}
