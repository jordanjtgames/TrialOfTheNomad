using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponPickup : MonoBehaviour
{
    public string weaponName;
    public string weaponDescription;

    [Header("PickupParams")]
    public int damage = 10;
    public int critDamage = 20;
    public float critChance = 0.2f;
    public float walkSpeed = 1f;
    public float sprintSpeed = 1.2f;

    public float chargeSpeed = 1f;
    public float minChargeTime = 0.25f;
    public float releaseSpeed = 1f;
    public float minReleaseTime = 0.8f;
    public float displaySpeed = 1f;

    public float range = 3f;

    public bool altSwing = true;
    public bool overhead = true;
    public float cancelTime = 3f;

    public bool isSpear = false;
    public float lungeChargeSpeed = 1f;
    public float minLungeChargeTime = 0.25f;
    public float lungeReleaseSpeed = 1f;
    public float minLungeReleaseTime = 0.8f;

    public int arrowsAmount = 1;
    public float arrowSpeed = 10f;
    public float arrowDropoff = 10f;

    public int blockUpToDMG = 15;
    public bool blockHeavy = false;

    [Header("MiscVariables")]
    public MeshRenderer mesh;
    private VisualEffect sparkle;
    public float worldScale = 1f;

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public Rarity myRarity = Rarity.Common;

    public enum WeaponType
    {
        OneHanded,
        TwoHanded,
        Bow,
        Shield
    }

    public WeaponType myWeaponType = WeaponType.OneHanded;

    public enum Element
    {
        Earth,
        Fire,
        Water,
        Wind
    }

    public Element element = Element.Earth;

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
                float colourIntensity = 1f;
                switch (myRarity) {
                    case Rarity.Common:
                        rID = 0;
                        colourIntensity = 0.3f;
                        sparkle.enabled = false;
                        break;
                    case Rarity.Uncommon:
                        rID = 1;
                        colourIntensity = 0.3f;
                        sparkle.enabled = false;
                        break;
                    case Rarity.Rare:
                        rID = 2;
                        colourIntensity = 1f;
                        sparkle.SetFloat("Percent", 0.2f);
                        sparkle.enabled = true;
                        break;
                    case Rarity.Epic:
                        rID = 3;
                        colourIntensity = 1f;
                        sparkle.SetFloat("Percent", 0.5f);
                        sparkle.enabled = true;
                        break;
                    case Rarity.Legendary:
                        rID = 4;
                        colourIntensity = 1f;
                        sparkle.SetFloat("Percent", 1f);
                        sparkle.enabled = true;
                        break;
                    
                }
                sparkle.SetVector4("RarityColour", WorldManager.rarity[rID] * 0.25f);

                mesh.material.SetColor("_OutlineColor", WorldManager.rarity[rID] * colourIntensity);
                mesh.material.SetColor("_RimLightColor", WorldManager.rarity[rID] * 0.25f * colourIntensity);
                mesh.material.SetFloat("_RimLightColorPower", original_rimColourPower * rimColourPowerMulti * colourIntensity);
                mesh.material.SetFloat("_OutlineWidth", original_outlineThickness * outlineMulti * colourIntensity);
                hasSet = true;
            }
        }
        
    }
}
