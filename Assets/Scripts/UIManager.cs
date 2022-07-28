using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public Transform player;

    public Transform center;
    public Transform testWWSS;
    public Transform testWWSS_Pos;

    public Volume postFX;

    public static bool weaponWheelOpen = false;
    public GameObject weaponWheelParent;
    public Transform weaponWheelTilt;
    public List<Transform> wheelTilts;
    Vector2 wheelTiltLerp;
    Vector2 startWheelPoint;

    Vector2 endWheelPoint;
    public Transform wheel_LR;
    public Transform wheel_UD;

    public Transform wheelArrow;

    public Transform weaponHoverUI;
    public WeaponPickup currentHoverItem;
    public TextMeshProUGUI weaponName;
    public RawImage weaponNameBG;
    public RawImage weaponStatsBG;

    void Start()
    {
        float tiltAmount = 0.5f;
        wheelTilts[0].localEulerAngles = new Vector3(0, -15 * tiltAmount, 0);
        wheelTilts[1].localEulerAngles = new Vector3(0, 15 * tiltAmount, 0);
        wheelTilts[2].localEulerAngles = new Vector3(15 * tiltAmount, 0, 0);
        wheelTilts[3].localEulerAngles = new Vector3(-15 * tiltAmount, 0, 0);
    }

    void Update()
    {
        testWWSS.position = Camera.main.WorldToScreenPoint(testWWSS_Pos.position);
        if (currentHoverItem != null) {

            if (Vector3.Distance(weaponHoverUI.position, center.position) < 500 && Vector3.Distance(player.position, currentHoverItem.transform.position) < 12
            && Vector3.Dot(player.TransformDirection(Vector3.forward), (currentHoverItem.transform.position - player.position)) > 0) {
                weaponHoverUI.gameObject.SetActive(true);
            } else {
                weaponHoverUI.gameObject.SetActive(false);
            }

            weaponHoverUI.position = Camera.main.WorldToScreenPoint(currentHoverItem.transform.position) + new Vector3(0,-100f,0);
            weaponName.text = currentHoverItem.weaponName + " [E]";
        } else {
            weaponHoverUI.gameObject.SetActive(false);
        }

        if(Vector3.Distance(testWWSS.position, center.position) < 500 && Vector3.Distance(player.position, testWWSS_Pos.position) < 12
            && Vector3.Dot(player.TransformDirection(Vector3.forward), (testWWSS_Pos.position - player.position)) > 0) {
            testWWSS.gameObject.SetActive(true);
        } else {
            testWWSS.gameObject.SetActive(false);
        }

        weaponWheelOpen = Inputs.weaponWheelHeld;
        if (postFX.profile.TryGet<DepthOfField>(out var DoF)) {
            DoF.nearMaxBlur = Mathf.Lerp(DoF.nearMaxBlur, weaponWheelOpen ? 8 : 0, Time.unscaledDeltaTime * (weaponWheelOpen ? 8f : 16f));
            DoF.farMaxBlur = Mathf.Lerp(DoF.farMaxBlur, weaponWheelOpen ? 16 : 0, Time.unscaledDeltaTime * (weaponWheelOpen ? 8f : 16f));
            if (weaponWheelOpen == false && DoF.nearMaxBlur < 0.2f)
                DoF.nearMaxBlur = 0;
            if (weaponWheelOpen == false && DoF.farMaxBlur < 0.2f)
                DoF.farMaxBlur = 0;
        }
        Time.timeScale = Mathf.Lerp(Time.timeScale, weaponWheelOpen ? 0.1f : 1, Time.deltaTime * 5);
        if (weaponWheelOpen == false && Time.timeScale > 0.99f)
            Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        weaponWheelParent.SetActive(weaponWheelOpen);

        if (!weaponWheelOpen) {
            endWheelPoint = Vector2.zero;
            startWheelPoint = Vector2.zero;
        } else {
            endWheelPoint += Inputs.look;
            endWheelPoint = Inputs.look;
        }

        wheelTiltLerp = Vector2.Lerp(wheelTiltLerp, endWheelPoint, Time.unscaledDeltaTime * 2.5f);//1.5f

        float AtanVal = Mathf.Atan2(wheelTiltLerp.x, wheelTiltLerp.y) * Mathf.Rad2Deg;
        float AtanVal_2 = Mathf.Atan2(wheelTiltLerp.y, wheelTiltLerp.x) * Mathf.Rad2Deg;
        wheelArrow.transform.localEulerAngles = Vector3.back * AtanVal;

        wheel_LR.localRotation = Quaternion.Lerp(wheelTilts[0].rotation, wheelTilts[1].rotation, Mathf.Abs(AtanVal_2) / 180);
        wheel_UD.localRotation = Quaternion.Lerp(wheelTilts[2].rotation, wheelTilts[3].rotation, Mathf.Abs(AtanVal) / 180);
    }
}
