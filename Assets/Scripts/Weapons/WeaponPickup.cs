using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponPickup : MonoBehaviour
{
    public MeshRenderer mesh;

    public float damage = 10;
    public float critDamage = 20;
    public float critChance = 0.2f;
    public float walkSpeed = 1f;
    public float sprintSpeed = 1.2f;

    public float chargeSpeed = 1f;
    public float minChargeTime = 0.25f;
    public float releaseSpeed = 1f;
    public float minReleaseTime = 0.8f;

    public float range = 3f;

    public bool altSwing = true;
    public float cancelTime;

    private VisualEffect sparkle;

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public Rarity myRarity = Rarity.Common;

    private float original_outlineThickness;

    [ColorUsage(true, true)]
    private Color original_rimColour;

    private float original_rimColourPower;

    [ColorUsage(true,true)]
    private Color original_outlineColour;

    public float outlineMulti = 10f;
    public float rimColourPowerMulti = 5.5f;

    bool hasSet = false;

    void Start()
    {
        sparkle = transform.GetChild(0).GetComponent<VisualEffect>();
    }

    void Update()
    {
        if(WorldManager.rarity.Count > 1 && sparkle != null) {

            original_outlineColour = mesh.material.GetColor("_OutlineColor");
            original_rimColour = mesh.material.GetColor("_RimLightColor");
            original_outlineThickness = mesh.material.GetFloat("_OutlineWidth");
            original_rimColourPower = mesh.material.GetFloat("_RimLightColorPower");

            if (!hasSet) {
                int rID = 0;
                switch (myRarity) {
                    case Rarity.Common:
                        rID = 0;
                        sparkle.enabled = false;
                        break;
                    case Rarity.Uncommon:
                        rID = 1;
                        sparkle.enabled = false;
                        break;
                    case Rarity.Rare:
                        rID = 2;
                        sparkle.SetFloat("Percent", 0.2f);
                        sparkle.enabled = true;
                        break;
                    case Rarity.Epic:
                        rID = 3;
                        sparkle.SetFloat("Percent", 0.5f);
                        sparkle.enabled = true;
                        break;
                    case Rarity.Legendary:
                        rID = 4;
                        sparkle.SetFloat("Percent", 1f);
                        sparkle.enabled = true;
                        break;
                    
                }
                sparkle.SetVector4("RarityColour", WorldManager.rarity[rID] * 0.25f);

                mesh.material.SetColor("_OutlineColor", WorldManager.rarity[rID]);
                mesh.material.SetColor("_RimLightColor", WorldManager.rarity[rID] * 0.25f);
                mesh.material.SetFloat("_RimLightColorPower", original_rimColourPower * rimColourPowerMulti);
                mesh.material.SetFloat("_OutlineWidth", original_outlineThickness * outlineMulti);
                hasSet = true;
            }
        }
        
    }
}
