using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class CombatMovement : MonoBehaviour
{
    public float maxGlobalSpeedMultiplier = 1.5f;
    public static float GSM = 1; //Global Speed Multiplier
    public static bool isSpeedBoost = false;
    public float maxBoostTime = 3;
    public static float staticMaxBoostTime = 3;
    public static float speedBoostTimer = 0;
    public static float playerSpeedMultiplier = 1;

    private string currentCamState;
    private string currentArmsState;
    private string currentArmsLeftHandState;
    private string currentArmsRightShoulderState;
    private string currentLegsState;
    private string currentSlideLegsState;
    private string currentWSAState;

    public Animator camAnim;
    public Animator armsAnim;
    public Animator handAnim;
    public Animator legsAnim;
    public Animator legsSlideAnim;
    public Animator worldSpaceArmsAnim;
    bool canAnimate = true;

    public Transform worldSpaceArms;
    private Renderer wsaRend;
    float wsaTransparencyScan = 0;
    public AnimationCurve wsaTransparency;

    const string c_Idle = "Idle";
    const string c_Jump = "Jump";
    const string c_Slide = "Slide";
    const string c_ClimbLedge = "ClimbLedge";
    const string c_WallrunRight = "WallrunRight";
    const string c_WallrunLeft = "WallrunLeft";
    const string c_PoleLand = "PoleLand";
    const string c_RingGrab = "RingGrab";
    const string c_ChainGrab = "ChainGrab";

    const string a_Idle = "Idle";
    const string a_Walk = "Walk";
    const string a_Sprint = "Sprint";
    const string a_Slide = "Slide";
    const string a_Leap = "Leap";
    const string a_IdleJump = "Idle Jump";
    const string a_Fall = "Fall";
    const string a_Land = "Land";
    const string a_RopeBalance = "RopeBalance";
    const string a_AttackCharge = "AttackCharge";
    const string a_AttackSwing = "AttackSwing";
    const string a_AttackSwingAlt = "AttackSwingAlt";
    const string a_AttackChargeOverhead = "AttackChargeOverhead";
    const string a_AttackSwingOverhead = "AttackSwingOverhead";

    const string l_Idle = "Idle";
    const string l_KickHold = "KickHold";
    const string l_KickRelease = "KickRelease";

    const string sl_Idle = "Idle";
    const string sl_Slide = "Slide";
    const string sl_Kicking = "Kicking";

    const string wsa_RingGrab = "RingGrab";
    const string wsa_LedgeClimb = "LedgeClimb";
    const string wsa_WallrunLeft = "WallrunLeft";
    const string wsa_WallrunRight = "WallrunRight";

    public Transform camHolder;
    public Transform playerCam;
    public Transform L_R_Tilt;
    public Transform F_B_Tilt;
    public Transform L_R_ArmsTilt;
    public Transform F_B_ArmsTilt;
    public Transform L_R_LookTilt;
    public Transform F_B_LookTilt;

    float camTiltMultiplier = 0.25f;
    public List<Transform> tilts;
    float armTiltMultiplier = 0.5f;
    public List<Transform> armTilts;
    float lookTiltMultiplier = 0.25f;
    public List<Transform> lookTilts;

    float H_Lerp = 0;
    float V_Lerp = 0;
    float H_ArmsLerp = 0;
    float V_ArmsLerp = 0;
    float H_LookLerp = 0;
    float V_LookLerp = 0;

    public CharacterController cc;

    public bool playedLandedAnim = true;
    public bool playedHardLandAnim = true;
    float landedAnim = 0;
    public AnimationCurve landAnimCurve;

    public static bool isSliding = false;
    float slideAnim = 0;
    public static Vector3 slideDir = new Vector3(0,0,1);
    public AnimationCurve slideSpeed;
    public AnimationCurve ccHeight;
    public static bool isDodging = false;
    public AnimationCurve dodgeSpeed;
    private float slideDelay = 0.5f;
    public static float currentSlideDelay = 0;

    public static bool isCrouched = false;

    public Transform wallrunHolder;
    public Transform wallrunCheck_L;
    public Transform wallrunCheck_R;
    public Transform prevWallrunHolderRot;
    public Transform wallrunRotationChecker;
    public Transform prevWallrunRotationChecker;
    public Transform wallrunRotatingLeft;
    public Transform wallrunRotatingRight;
    float wallrunRotationCheckInterval = 0.1f;
    //float prevLeftCheckDist = 0;
    //float prevRightCheckDist = 0;
    float wallrunViewRotate = 0;
    float currentWallrunViewRotate = 0;
    float wallrunCooldown = 0;
    Vector3 wallrunPrevPos;
    float wallrunStationaryTime = 0;
    float wallrunTimer = 0;
    public static int wrID = 0; //1 = left, 2 = right, 0 = none/idle

    public static bool isWallrunning = false;
    bool hasStartedWallrunning = false;
    public static Vector3 wallParallel;
    float wallrunCheckTime = 0;
    float wallrunDetectDist = 1;
    bool canChainWR = false;

    float airTime = 0;

    public static bool isSprinting;

    public static bool isPositionHeld = false;
    public static Vector3 heldPosition;
    bool isOnPole = false;
    bool isOnRing = false;

    bool ledgeClimb = false;
    float ledgeClimbTime = 0;
    public Transform ledgeDetectTOP;
    public Transform ledgeDetectFWD;
    Vector3 hitPointTop;

    bool isClimbingChain = false;
    GameObject currentChain;
    float chainCooldown = 0;

    bool isSwingingChain = false;
    public Transform chainSwingHolder;
    public AnimationCurve swingCurve; // 1 = FWD, 0 = BACK
    public Transform holdOnPoint;
    float swingProgress = 0;
    float swingIntensity = 1;
    Quaternion chainStartingRotation;
    GameObject initalParent;
    bool wasSwinging = false;

    bool isRopeWalking = false;
    GameObject currentRope;
    GameObject oldRope;
    float oldRopeCooldown = 0;
    int currentRopeIndex = 0;
    bool isRopeInverted = false;

    public Transform armsHolder;
    public Transform lookRotation;
    float armsLerpMultiplier = 1;
    public Transform armsConceal;
    bool isConcealed = false;
    Transform concealTrans;
    Transform revealTrans;
    Transform crouchConcealTrans;

    public Transform slideLegsHolder;
    public SkinnedMeshRenderer slideLegsRend;
    public Transform legsSlideDirections;

    bool hasjumped = false;
    bool requiresLeap = false;

    public static bool isKicking = false;
    float kickDelay = 0; //1
    bool hasKicked = false;
    

    //--------------------------------------Input

    public static Vector2 inputDirRaw;
    Vector2 lookAxis;
    Vector2 lookAxisSmooth;
    InputAction.CallbackContext jumpHeldInput_;
    InputAction.CallbackContext jumpTapInput_;
    InputAction.CallbackContext sprintInput_;
    InputAction.CallbackContext sprintTapInput_;
    InputAction.CallbackContext crouchInput_;
    InputAction.CallbackContext slideInput_;
    InputAction.CallbackContext attackInput_;
    InputAction.CallbackContext blockMagicInput_;
    InputAction.CallbackContext kickInput_;
    InputAction.CallbackContext debugKeyInput_;

    //--------------------------------------Combat & Magic

    int leftHandID = 2; //0=Unarmed, 1=Shield, 2=Teleport, 3=Telekinesis, 4=ElementalAoE, 5=Projectile, 6=Beam
    int rightHandID = 0; //0=Unarmed, 1=OneHanded, 2=TwoHanded, 3=Bow

    bool isChargingAttack = false;
    bool hasSwung = false;
    float attackSwingProgress = 0;
    bool altSwing = false;
    float timeSinceLastSwing = 4;
    float attackChargeTime = 0;
    float attackSwingDelay = 0;

    public static bool isFindingBlink = false;
    public static bool blinkMantelFound = false;
    public static bool blinkWallrunFound = false;
    public static int blinkWallrunID = 0;
    public float maxBlinkReach = 10; //public
    float maxBlinkMantelHeight = 2; //public
    float blinkSpeed = 7; //public
    float currentHitDistance = 0;
    bool isTravelingBlink = false;
    Vector3 targetBlinkDest;
    //public static bool blinkMantelFound = false;
    //public static bool blinkWallrunFound = false;

    public RawImage fadeToBlack;
    bool isFading = false;
    float fadeToBlackTime = 0;
    string resetPointName;

    void Start()
    {
        //Debug.LogError(Mathf.Pow(2,2));
        //Debug.LogError(Mathf.Abs((swingCurve.Evaluate(0.5f)-0.5f)*2));
        tilts[0].localRotation = new Quaternion(0, 0, 0.1f * camTiltMultiplier, 1.0f);
        tilts[1].localRotation = new Quaternion(0, 0, -0.1f * camTiltMultiplier, 1.0f);
        tilts[2].localRotation = new Quaternion(0.1f * camTiltMultiplier, 0, 0, 1.0f);
        tilts[3].localRotation = new Quaternion(-0.1f * camTiltMultiplier, 0, 0, 1.0f);

        armTilts[0].localRotation = new Quaternion(0, 0, 0.1f * armTiltMultiplier, 1.0f);
        armTilts[1].localRotation = new Quaternion(0, 0, -0.1f * armTiltMultiplier, 1.0f);
        armTilts[2].localRotation = new Quaternion(0.1f * armTiltMultiplier, 0, 0, 1.0f);
        armTilts[3].localRotation = new Quaternion(-0.1f * armTiltMultiplier, 0, 0, 1.0f);

        lookTilts[0].localRotation = new Quaternion(0, 0, 0.1f * lookTiltMultiplier, 1.0f);
        lookTilts[1].localRotation = new Quaternion(0, 0, -0.1f * lookTiltMultiplier, 1.0f);
        lookTilts[2].localRotation = new Quaternion(0.1f * lookTiltMultiplier, 0, 0, 1.0f);
        lookTilts[3].localRotation = new Quaternion(-0.1f * lookTiltMultiplier, 0, 0, 1.0f);

        concealTrans = armsConceal.parent.Find("Conceal");
        revealTrans = armsConceal.parent.Find("Reveal");
        crouchConcealTrans = armsConceal.parent.Find("CrouchConceal");
        slideLegsHolder.gameObject.SetActive(true);

        if (GameObject.Find("ResetPoint")) {
            GameObject.Find("ResetPoint").transform.position = transform.position;
        }

        wsaRend = worldSpaceArmsAnim.transform.GetChild(1).GetComponent<Renderer>();
        wsaRend.enabled = false;

        staticMaxBoostTime = maxBoostTime;
    }

    void Update()
    {
        float H = FPSInputController.inputDirSmoothed.x;
        float V = FPSInputController.inputDirSmoothed.y;

        lookAxisSmooth = Vector2.Lerp(lookAxisSmooth, lookAxis, Time.deltaTime * 5);

        if (Keyboard.current.spaceKey.ReadValue() == 1) {
            //Debug.Log(0987);
        }

        H_Lerp = Mathf.Lerp(H_Lerp, H, Time.deltaTime * 6);
        V_Lerp = Mathf.Lerp(V_Lerp, V, Time.deltaTime * 6);
        H_ArmsLerp = Mathf.Lerp(H_ArmsLerp, H, Time.deltaTime * 12);
        V_ArmsLerp = Mathf.Lerp(V_ArmsLerp, V, Time.deltaTime * 12);
        H_LookLerp = Mathf.Lerp(H_LookLerp, lookAxisSmooth.x, Time.deltaTime * 112);
        V_LookLerp = Mathf.Lerp(V_LookLerp, lookAxisSmooth.y, Time.deltaTime * 112);

        L_R_Tilt.rotation = Quaternion.Lerp(tilts[0].rotation, tilts[1].rotation, ((H_Lerp + 1) / 2));
        F_B_Tilt.rotation = Quaternion.Lerp(tilts[3].rotation, tilts[2].rotation, ((V_Lerp + 1) / 2));
        L_R_ArmsTilt.rotation = Quaternion.Lerp(armTilts[0].rotation, armTilts[1].rotation, ((H_ArmsLerp + 1) / 2));
        F_B_ArmsTilt.rotation = Quaternion.Lerp(armTilts[3].rotation, armTilts[2].rotation, ((V_ArmsLerp + 1) / 2));
        L_R_LookTilt.rotation = Quaternion.Lerp(lookTilts[0].rotation, lookTilts[1].rotation, ((H_LookLerp + 1) / 2)); //**************************************
        F_B_LookTilt.rotation = Quaternion.Lerp(lookTilts[3].rotation, lookTilts[2].rotation, ((V_LookLerp + 1) / 2));

        isConcealed = isOnRing || isWallrunning || isClimbingChain || ledgeClimb ||isSwingingChain || isKicking || hasKicked;
        if (isConcealed)
            armsConceal.rotation = Quaternion.Lerp(armsConceal.rotation, concealTrans.rotation, Time.deltaTime * 8);
        else
            armsConceal.rotation = Quaternion.Lerp(armsConceal.rotation, revealTrans.rotation, Time.deltaTime * 8);

        GSM = isSpeedBoost ? maxGlobalSpeedMultiplier : 1;
        if (speedBoostTimer > 0)
            speedBoostTimer -= Time.deltaTime;
        if (speedBoostTimer <= 0)
            isSpeedBoost = false;
        //armsAnim.SetBool("hasJumped", hasjumped);
        //handAnim.SetBool("hasJumped", hasjumped);
        //camAnim.SetBool("hasJumped", hasjumped);
        //armsAnim.SetBool("isGrounded", cc.isGrounded);
        //handAnim.SetBool("isGrounded", cc.isGrounded);


        camHolder.localPosition = new Vector3(0, landAnimCurve.Evaluate(landedAnim) - ((2 - cc.height) / 4), 0);
        if (cc.isGrounded) {
            canChainWR = false;
            if (airTime >= 1.3f)
                playedHardLandAnim = false;
            if (!playedLandedAnim) {
                landedAnim += Time.deltaTime * 3.25f;
                //c
                if (landedAnim >= 1) {
                    playedLandedAnim = true;
                    playedHardLandAnim = true;
                }
            }
            airTime = 0;
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                hasjumped = true;
            //else
            //    hasjumped = false;
            
        }
        else {
            playedLandedAnim = false;
            landedAnim = 0;
            airTime += Time.deltaTime;
            if (airTime > 0.985f) {
                hasjumped = false;
                requiresLeap = false;
            }
        }

        isSprinting = Keyboard.current.leftShiftKey.ReadValue() == 1 && V > 0 && airTime < 0.63f;
        //isPositionHeld = Input.GetKey(KeyCode.U);

        if ((Mouse.current.middleButton.wasPressedThisFrame) && !isDodging && !isSliding && currentSlideDelay <= 0 && cc.isGrounded &&!isCrouched) {
            if (V > 0.25f && isSprinting) { //Slide
                isSliding = true;
                slideAnim = 0;
                //slideDir = new Vector3(H, 0, V);
                slideDir = Vector3.zero;
                currentSlideDelay = slideDelay;
            }

            if (inputDirRaw.y <= 0) { //Dodge
                slideAnim = 0;
                isSliding = true;
                isDodging = true;
                currentSlideDelay = slideDelay / 2;
            }


        }
        /*
        legsAnim.SetBool("isKicking", isKicking);

        camAnim.SetBool("isSliding", isSliding);
        camAnim.SetBool("isDodging", isDodging);
        camAnim.SetBool("isClimbingLedge", ledgeClimb);

        armsAnim.SetInteger("Horizontal", (int)inputDirRaw.x);
        handAnim.SetInteger("Horizontal", (int)inputDirRaw.x);
        armsAnim.SetInteger("Vertical", (int)inputDirRaw.y);
        handAnim.SetInteger("Vertical", (int)inputDirRaw.y);
        armsAnim.SetBool("movementInput", inputDirRaw != Vector2.zero);
        handAnim.SetBool("movementInput", inputDirRaw != Vector2.zero);
        armsAnim.SetBool("isSprinting", isSprinting);
        handAnim.SetBool("isSprinting", isSprinting);
        armsAnim.SetBool("isSliding", isSliding);
        handAnim.SetBool("isSliding", isSliding);

        legsSlideAnim.SetBool("isSliding", isSliding);
        legsSlideAnim.SetBool("isKicking", isKicking);
        legsSlideAnim.SetBool("isDodging", isDodging);
        legsSlideAnim.SetFloat("currentSlideDelay", currentSlideDelay);
        //
        armsAnim.SetBool("isChargingAttack", isChargingAttack);
        handAnim.SetBool("isChargingAttack", isChargingAttack);
        */

        
        /*
        if (inputDirRaw != Vector2.zero)
            ChangeAnimationState("Walk");
        else
            ChangeAnimationState("Idle");
        */

        Vector3 headroom = camHolder.TransformDirection(Vector3.up);
        RaycastHit hit_Head;

        isCrouched = (Keyboard.current.cKey.ReadValue() == 1 && slideAnim == 0) || Physics.Raycast(camHolder.transform.position, headroom, out hit_Head, 0.5f) && cc.isGrounded;

        

        if (isCrouched) {
            cc.height = Mathf.Lerp(cc.height, 0.001f, Time.deltaTime * 4);
            //playerSpeedMultiplier = 0.64f;
        }
        else {
            if (isSliding) {
                if (isDodging) {
                    slideAnim += Time.deltaTime * 3.35f;
                    playerSpeedMultiplier = dodgeSpeed.Evaluate(slideAnim) * 0.25f;
                    cc.height = Mathf.Lerp(cc.height, 2, Time.deltaTime * 3);
                }
                else {
                    slideAnim += Time.deltaTime * 1.35f;
                    playerSpeedMultiplier = slideSpeed.Evaluate(slideAnim) * 0.25f;
                    cc.height = Mathf.Lerp(cc.height, 0.001f, Time.deltaTime * 4);
                    //Debug.LogError(inputDirRaw.x);
                }

                if (slideAnim >= 1) {
                    slideAnim = 0;
                    playerSpeedMultiplier = 1;
                    isSliding = false;
                    isDodging = false;
                }
                slideLegsRend.material.SetFloat("_TransparentThreshold", Mathf.Lerp(slideLegsRend.material.GetFloat("_TransparentThreshold"), -40, Time.unscaledDeltaTime * 50));

            }
            else {
                cc.height = Mathf.Lerp(cc.height, 2, Time.deltaTime * 3);
                currentSlideDelay -= Time.deltaTime;
                slideLegsRend.material.SetFloat("_TransparentThreshold", Mathf.Lerp(slideLegsRend.material.GetFloat("_TransparentThreshold"), 4, Time.unscaledDeltaTime * 50));
                playerSpeedMultiplier = 1;
            }
        }

        slideLegsHolder.transform.position = transform.position;
        if (!(isSliding && !isDodging) && !(currentSlideDelay > 0)) {
            if(inputDirRaw.x > -0.1 && inputDirRaw.x < 0.1f)
                slideLegsHolder.rotation = legsSlideDirections.GetChild(0).rotation;
            if (inputDirRaw.x > 0.5f)
                slideLegsHolder.rotation = legsSlideDirections.GetChild(2).rotation;
            if (inputDirRaw.x < -0.5f)
                slideLegsHolder.rotation = legsSlideDirections.GetChild(1).rotation;
        }

        if (isCrouched || isDodging)
            revealTrans.localRotation = crouchConcealTrans.localRotation;
        else
            revealTrans.localEulerAngles = Vector3.zero;

        //Wall running
        wallrunHolder.position = transform.position;
        prevWallrunHolderRot.position = transform.position;
        if (isWallrunning) {
            wallrunTimer += Time.deltaTime;
            worldSpaceArms.position = wallrunHolder.position;
            worldSpaceArms.rotation = wallrunHolder.rotation;

            if (!hasStartedWallrunning) {
                wsaRend.enabled = true;
                hasStartedWallrunning = true;
            }

            if (wallrunRotationCheckInterval > 0)
                wallrunRotationCheckInterval -= Time.deltaTime;
            if (wallrunRotationCheckInterval <= 0) {
                wallrunRotationCheckInterval = 0.05f;
                prevWallrunHolderRot.rotation = wallrunHolder.rotation;
                wallrunPrevPos = new Vector3(transform.position.x, 0, transform.position.z);
            }

            if (Vector3.Distance(wallrunRotationChecker.position, wallrunRotatingRight.position) < Vector3.Distance(prevWallrunRotationChecker.position, wallrunRotatingRight.position)) {
                wallrunViewRotate = 10;
            }
            else if (Vector3.Distance(wallrunRotationChecker.position, wallrunRotatingLeft.position) < Vector3.Distance(prevWallrunRotationChecker.position, wallrunRotatingLeft.position)) {
                wallrunViewRotate = -10;
            }
            else {
                wallrunViewRotate = 0;
            }

            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), wallrunPrevPos) < 0.0001f)
                wallrunStationaryTime += Time.deltaTime;
            else
                wallrunStationaryTime = 0;
            if (wallrunStationaryTime >= 0.25f) {
                isWallrunning = false;
                wallrunCooldown = 0.3f;
                wallrunStationaryTime = 0;
                wallrunCheckTime = 0;
                wallrunDetectDist = 0.65f;
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                isWallrunning = false;
                wallrunCheckTime = 0;
                wallrunDetectDist = 0.65f;
                //gameObject.SendMessage("SetVelocity", Vector3.up * 3);
                CharacterMotor.externalVelocity = transform.TransformDirection(new Vector3(wrID == 1 ? 3.5f : -3.5f, 1.31f, 4) * 0.06f);
                CharacterMotor.exVelBias = 1;
                CharacterMotor.exVelBiasLerpSpeed = 7;
                wallrunCooldown = 0.3f;

                airTime = 0;
                requiresLeap = true;
                hasjumped = true;
            }
        }
        else {
            wallrunViewRotate = 0;
            prevWallrunHolderRot.rotation = wallrunHolder.rotation;
            wallrunHolder.rotation = transform.rotation;
            wallrunTimer = 0;
            //camAnim.SetInteger("WallrunID", 0);
            if (cc.isGrounded)
                wrID = 0;
            if (wallrunCooldown > 0)
                wallrunCooldown -= Time.deltaTime;
            if (hasStartedWallrunning) {
                wsaRend.enabled = false;
                hasStartedWallrunning = false;
            }
        }

        currentWallrunViewRotate = Mathf.Lerp(currentWallrunViewRotate, wallrunViewRotate, Time.deltaTime * 5);
        //transform.Rotate(new Vector3(0, currentWallrunViewRotate, 0) * (Time.deltaTime * 20) * Vector3.Distance(wallrunRotationChecker.position, wallrunRotatingRight.position) * 5);


        Vector3 L_Dir = wallrunCheck_L.TransformDirection(Vector3.forward);
        Vector3 R_Dir = wallrunCheck_R.TransformDirection(Vector3.forward);
        RaycastHit hit_L;
        RaycastHit hit_R;
        if (Physics.Raycast(wallrunCheck_L.transform.position, L_Dir, out hit_L, wallrunDetectDist) && !cc.isGrounded && wallrunCooldown <= 0 && wallrunTimer < 2.5f) { //LEFT
            wallrunCheckTime += Time.deltaTime;
            //Debug.LogError(983624);
            if (!isWallrunning && V > 0 && ((1==1 && wallrunCheckTime > 0.1f) || canChainWR) && wrID != 1) {
                if (Keyboard.current.leftShiftKey.ReadValue() == 1) {
                    wallParallel = hit_L.normal;
                    isWallrunning = true;
                    wallrunCheckTime = 1;
                    //camAnim.SetInteger("WallrunID", -1);
                    //transform.position = (hit_L.point + hit_L.normal.normalized) - Vector3.up;
                    wrID = 1;
                    canChainWR = true;
                }
            }
            else if (isWallrunning) {
                wallParallel = hit_L.normal;
                wallrunDetectDist = 1.5f; //2.5f
                wallrunHolder.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(-90, Vector3.up) * wallParallel, Vector3.up);
            }
        }
        else if (Physics.Raycast(wallrunCheck_R.transform.position, R_Dir, out hit_R, wallrunDetectDist) && !cc.isGrounded && wallrunCooldown <= 0 && wallrunTimer < 2.5f) { //RIGHT
            wallrunCheckTime += Time.deltaTime;
            //Debug.LogError(983624);
            if (!isWallrunning && V > 0 && ((1==1 && wallrunCheckTime > 0.1f) || canChainWR) && wrID != 2) {
                if (Keyboard.current.leftShiftKey.ReadValue() == 1) {
                    wallParallel = hit_R.normal;
                    isWallrunning = true;
                    wallrunCheckTime = 1;
                    //camAnim.SetInteger("WallrunID", 1);
                    //transform.position = (hit_L.point + hit_L.normal.normalized) - Vector3.up;
                    wrID = 2;
                    canChainWR = true;
                }
            }
            else if (isWallrunning) {
                wallParallel = hit_R.normal;
                wallrunDetectDist = 1.5f; //2.5f
                wallrunHolder.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(90, Vector3.up) * wallParallel, Vector3.up);
            }
        }
        else {
            if (wallrunCheckTime > 0 && isWallrunning) {
                wallrunCheckTime -= Time.deltaTime * 20;
                if (wrID == 1)
                    CharacterMotor.externalVelocity = (Quaternion.AngleAxis(-90, Vector3.up) * wallParallel) * 0.26f;
                else
                    CharacterMotor.externalVelocity = (Quaternion.AngleAxis(90, Vector3.up) * wallParallel) * 0.26f;
                CharacterMotor.exVelBias = 1;

            }
            else {
                wallrunCheckTime = 0;
                wallrunDetectDist = 0.65f;
                isWallrunning = false;
            }
        }

        //isHolding

        if (isOnPole && !ledgeClimb) {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                CharacterMotor.externalVelocity = transform.TransformDirection(new Vector3(0, 1.81f, 9) * 0.06f);
                CharacterMotor.exVelBias = 1;
                CharacterMotor.exVelBiasLerpSpeed = 7;
                isOnPole = false;
                isPositionHeld = false;

                airTime = 0;
                requiresLeap = true;
                hasjumped = true;
            }

        }
        if (isOnRing) {

            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.cKey.ReadValue() == 1) {
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    CharacterMotor.externalVelocity = transform.TransformDirection(new Vector3(0, 2.81f, 4) * 7.59f * Time.deltaTime);
                else
                    CharacterMotor.externalVelocity = transform.TransformDirection(new Vector3(0, 0, -1) * 0.09f);
                CharacterMotor.exVelBias = 1;
                CharacterMotor.exVelBiasLerpSpeed = 15;
                isOnRing = false;
                isPositionHeld = false;

                airTime = 0;
                requiresLeap = true;
                hasjumped = true;
                wsaRend.enabled = false;
            }
        }

        //LedgeClimb

        Vector3 TOP_Dir = ledgeDetectTOP.TransformDirection(Vector3.forward);
        Vector3 FWD_Dir = ledgeDetectFWD.TransformDirection(Vector3.forward);
        RaycastHit hit_TOP;
        RaycastHit hit_FWD;
        if (Physics.Raycast(ledgeDetectTOP.transform.position, TOP_Dir, out hit_TOP, 0.5f) && !isSliding) { //Top
            if (Physics.Raycast(ledgeDetectFWD.transform.position, FWD_Dir, out hit_FWD, 0.31f) && !ledgeClimb) { //Top
                //Debug.LogError("ClimbLedge"); 
                ledgeClimb = true;
                heldPosition = transform.position;
                isPositionHeld = true;
                hitPointTop = hit_TOP.point;
                wsaRend.enabled = true;
                worldSpaceArms.position = new Vector3(transform.position.x, hit_TOP.point.y, transform.position.z);
                worldSpaceArms.rotation = Quaternion.LookRotation(-hit_FWD.normal);
            }
        }

        if (ledgeClimb) {
            ledgeClimbTime += Time.unscaledDeltaTime;
            if (transform.position.y <= hitPointTop.y)
                heldPosition += (Vector3.up + transform.TransformDirection(Vector3.back * .2f)) * Time.deltaTime * 40;//40
            else if ((Vector3.Distance(transform.position, hitPointTop + Vector3.up * 0.25f) < 0.15f) || ledgeClimbTime > 0.91f) {
                isPositionHeld = false;
                ledgeClimb = false;
                wsaRend.enabled = false;
                //Debug.LogError("ddd");
            }
            else
                heldPosition = hitPointTop + Vector3.up * 0.25f;
            if(ledgeClimbTime > 0.15f)
                wsaRend.enabled = false;
        }
        else
            ledgeClimbTime = 0;

        if (isClimbingChain) {
            wasSwinging = false;
            if (inputDirRaw.y > 0 && heldPosition.y < (currentChain.GetComponent<CapsuleCollider>().height / 2) + currentChain.transform.position.y) {
                heldPosition += new Vector3(0, 1, 0) * Time.deltaTime * 10;
            }
            if (inputDirRaw.y < 0 && heldPosition.y > currentChain.transform.position.y - (currentChain.GetComponent<CapsuleCollider>().height / 2)) {
                heldPosition += new Vector3(0, -1, 0) * Time.deltaTime * 10;
            }
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                CharacterMotor.externalVelocity = transform.TransformDirection(new Vector3(0, 2.81f, 8) * 0.09f);
                CharacterMotor.exVelBias = 1;
                CharacterMotor.exVelBiasLerpSpeed = 15;
                currentChain.GetComponent<CapsuleCollider>().enabled = true;
                isClimbingChain = false;
                isPositionHeld = false;
                chainCooldown = 0.3f;

                airTime = 0;
                requiresLeap = true;
                hasjumped = true;
            }
        }
        else {
            if (chainCooldown >= 0)
                chainCooldown -= Time.deltaTime;
        }

        if (isSwingingChain) {
            wasSwinging = true;
            heldPosition = holdOnPoint.position;
            swingIntensity = swingIntensity + (swingIntensity >= 2 ? 0 : Time.deltaTime * .25f);
            /*
            swingProgress += Time.deltaTime * 0.5f;
            if (swingProgress >= 1)
                swingProgress = 0;
            */
            //swingProgress = swingProgress >= 1 ? 0 : swingProgress += Time.deltaTime * 0.5f;
            swingProgress = swingProgress >= 1 ? 0 : swingProgress += Time.deltaTime * (0.5f * (1 - Mathf.Pow(Mathf.Abs((swingCurve.Evaluate(swingProgress) - 0.5f) * 1.25f), 2))) * 1.25f;

            //CharacterMotor.heldOffsetSpeed = ((1 - Mathf.Abs((swingCurve.Evaluate(swingProgress) - 0.5f) * 2)) * 0.1f) + 0.1f;
            //CharacterMotor.heldOffsetSpeed = (Mathf.Clamp(Vector3.Distance(transform.position, heldPosition),0,0.5f) * 0.3f) * ((1 - Mathf.Abs(((swingCurve.Evaluate(swingProgress)+0.1f) - 0.5f) * 1.5f)));
            CharacterMotor.heldOffsetSpeed = 0.001f;

            chainSwingHolder.Find("CurrentSwing").rotation = Quaternion.LerpUnclamped(chainSwingHolder.Find("SwingBACK").rotation, chainSwingHolder.Find("SwingFWD").rotation, swingCurve.Evaluate(swingProgress) * swingIntensity);

            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                CharacterMotor.externalVelocity = transform.TransformDirection(new Vector3(0, 2.81f, 8) * 0.09f);
                CharacterMotor.exVelBias = 1;
                CharacterMotor.exVelBiasLerpSpeed = 15;
                currentChain.GetComponent<CapsuleCollider>().enabled = true;
                isSwingingChain = false;
                isPositionHeld = false;
                chainCooldown = 0.3f;
                CharacterMotor.heldOffsetSpeed = 0.2f;

                //currentChain.transform.parent.parent.rotation = chainStartingRotation;
                airTime = 0;
                requiresLeap = true;
                hasjumped = true;

                currentChain.transform.parent.parent = initalParent.transform;
                holdOnPoint.parent = null;
                chainSwingHolder.Find("CurrentSwing").rotation = chainSwingHolder.rotation;
            }
        }
        else {
            //CharacterMotor.heldOffsetSpeed = 0.2f;
            if (currentChain != null && wasSwinging)
                currentChain.transform.parent.rotation = Quaternion.Lerp(currentChain.transform.parent.rotation, chainStartingRotation, Time.deltaTime * 5);
            //currentChain.transform.parent.parent.rotation = Quaternion.Lerp(currentChain.transform.parent.parent.rotation, chainStartingRotation, Time.deltaTime * 10);

            swingProgress = 0;
            swingIntensity = 1;
        }

        if (isRopeWalking) {
            Vector3 ropeFwdPos = transform.position + transform.TransformDirection(Vector3.forward);
            Vector3 ropeBackPos = transform.position + transform.TransformDirection(Vector3.back);
            Vector3 option1;
            Vector3 option2;
            //GameObject.Find("TesterSphere").transform.position = ropeFwdPos;

            if (Vector3.Distance(transform.position, currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex])) < 0.18f) {
                if (inputDirRaw.y > 0) {
                    if(currentRopeIndex > 0 && currentRopeIndex < (currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1)) {
                        option2 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex + 1]);
                        option1 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex - 1]);
                        //Debug.LogError(9);
                        if (Vector3.Distance(ropeFwdPos, option1) < Vector3.Distance(ropeFwdPos, option2))
                            currentRopeIndex--; //currentRopeIndex += isRopeInverted ? -1 : 1;
                        else
                            currentRopeIndex++;
                    }
                    else {
                        if(currentRopeIndex == 0) {
                            option2 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex + 1]);
                            option1 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[0]);
                            if (Vector3.Distance(ropeFwdPos, option1) > Vector3.Distance(ropeFwdPos, option2))
                                currentRopeIndex++; //currentRopeIndex += isRopeInverted ? -1 : 1;

                        }
                        else if(currentRopeIndex == (currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1)) {
                            option2 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1]);
                            option1 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex - 1]);
                            if (Vector3.Distance(ropeFwdPos, option1) < Vector3.Distance(ropeFwdPos, option2))
                                currentRopeIndex--; //currentRopeIndex += isRopeInverted ? -1 : 1;
                        }
                    }
                    heldPosition = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex]);
                }
                else if (inputDirRaw.y < 0) {
                    if (currentRopeIndex > 0 && currentRopeIndex < (currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1)) {
                        option2 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex + 1]);
                        option1 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex - 1]);
                        //Debug.LogError(9);
                        if (Vector3.Distance(ropeBackPos, option1) < Vector3.Distance(ropeBackPos, option2))
                            currentRopeIndex--; //currentRopeIndex += isRopeInverted ? -1 : 1;
                        else
                            currentRopeIndex++;
                    }
                    else {
                        if (currentRopeIndex == 0) {
                            option2 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex + 1]);
                            option1 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[0]);
                            if (Vector3.Distance(ropeBackPos, option1) > Vector3.Distance(ropeBackPos, option2))
                                currentRopeIndex++; //currentRopeIndex += isRopeInverted ? -1 : 1;

                        }
                        else if (currentRopeIndex == (currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1)) {
                            option2 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1]);
                            option1 = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex - 1]);
                            if (Vector3.Distance(ropeBackPos, option1) < Vector3.Distance(ropeBackPos, option2))
                                currentRopeIndex--; //currentRopeIndex += isRopeInverted ? -1 : 1;
                        }
                    }
                    heldPosition = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex]);
                }
                /*
                if (Input.GetKey(KeyCode.S)) {
                    if (currentRopeIndex < currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1) {
                        currentRopeIndex += isRopeInverted ? 1 : -1;
                    }
                    heldPosition = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex]);
                }
                */

                //heldPosition = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex]);
            }

            if ((currentRopeIndex == 0 || (currentRopeIndex >= currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1)) && Vector3.Distance(transform.position, heldPosition) < 0.25f) {
                //Debug.LogError(31111111);
                if (!currentRope.name.StartsWith("#")) {
                    oldRope = currentRope;
                    CharacterMotor.externalVelocity = transform.TransformDirection(new Vector3(0, 0.81f, 3.5f) * 0.09f);
                    CharacterMotor.exVelBias = 1;
                    CharacterMotor.exVelBiasLerpSpeed = 15;
                    isRopeWalking = false;
                    isPositionHeld = false;
                    currentRope = null;
                    oldRopeCooldown = 0.3f;
                }
            }
            if (Keyboard.current.spaceKey.ReadValue() == 1) {
                oldRope = currentRope;
                CharacterMotor.externalVelocity = transform.TransformDirection(new Vector3(0, 1.81f, 5) * 0.09f);
                CharacterMotor.exVelBias = 1;
                CharacterMotor.exVelBiasLerpSpeed = 15;
                isRopeWalking = false;
                isPositionHeld = false;
                currentRope = null;
                oldRopeCooldown = 0.3f;

                airTime = 0;
                requiresLeap = true;
                hasjumped = true;
            }
        }
        else {
            oldRopeCooldown -= oldRopeCooldown >= 0 ? 1 : 0;
        }
        /*
        if (Input.GetKeyDown(KeyCode.F)) {
            //isWallrunning = false;
            //transform.Rotate(new Vector3(0, 10, 0) * Time.deltaTime * 5);

            CameraShake.instance.StartShake();
        }
        else {
        }
        */

        //--------------------------------------------------------Combat

        //Kick
        if (Keyboard.current.fKey.ReadValue() == 1 && kickDelay <= 0 && (inputDirRaw == Vector2.zero || isSliding)) {
            isKicking = true;
        }
        else {
            if (isKicking) {
                //Debug.LogError(9);
                RaycastHit kickHit;
                if (Physics.SphereCast(playerCam.position - playerCam.TransformDirection(Vector3.forward), .75f, playerCam.TransformDirection(Vector3.forward), out kickHit, 3f)) {
                    if (kickHit.transform.GetComponent<Rigidbody>()) {
                        if (kickHit.collider.transform.gameObject.layer != 10) {
                            //kickHit.transform.GetComponent<Rigidbody>().AddExplosionForce(1999, kickHit.point, 0.5f);
                            kickHit.transform.GetComponent<Rigidbody>().velocity = (transform.position - kickHit.transform.position).normalized * -10;
                        }

                        if (kickHit.collider.tag == "Pot") {
                            //Debug.LogError(90);
                            if(isSliding)
                                kickHit.transform.parent.GetComponent<PotScript>().BreakPot(2, transform.position);
                            else
                                kickHit.transform.parent.GetComponent<PotScript>().BreakPot(0, transform.position);
                        }
                    }
                }
                kickDelay = 0.5f;
                CameraShake.instance.StartShake(1);
                hasKicked = true;
            }
            isKicking = false;
        }

        if (hasKicked)
            kickDelay -= Time.unscaledDeltaTime;
        if (kickDelay <= 0)
            hasKicked = false;

        //Attack
        if (Mouse.current.leftButton.ReadValue() == 1 && timeSinceLastSwing >= 0.3f) {
            isChargingAttack = true;
            if(attackChargeTime <= 4)
                attackChargeTime += Time.deltaTime;
            attackSwingProgress = 0;
            hasSwung = false;

            //if (attackChargeTime >= 1)
            //    altSwing = true;
            //if (attackChargeTime <= 1)
            //    altSwing = !altSwing;
        }
        else {
            if (isChargingAttack) {
                if (attackSwingProgress <= 0) {
                    if (timeSinceLastSwing > 1 || (timeSinceLastSwing < 1 && altSwing))
                        altSwing = false;
                    else if (timeSinceLastSwing < 1 && !altSwing)
                        altSwing = true;
                    attackSwingProgress = 0.0001f;
                }

                if (attackSwingProgress > 0.05f && !hasSwung) {
                    RaycastHit attackHit;
                    if (Physics.SphereCast(playerCam.position - playerCam.TransformDirection(Vector3.forward), .75f, playerCam.TransformDirection(Vector3.forward), out attackHit, 3f)) {
                        if (attackHit.transform.GetComponent<Rigidbody>()) {
                            attackHit.transform.GetComponent<Rigidbody>().AddExplosionForce(1999, attackHit.point, 0.5f);

                            if (attackHit.collider.tag == "Pot") {
                                //Debug.LogError(90);
                                attackHit.transform.parent.GetComponent<PotScript>().BreakPot(3, transform.position);
                            }

                            if(attackHit.collider.gameObject.layer == 11) {
                                //Debug.LogError(0112);
                                if (attackHit.collider.GetComponent<SlimeScript>())
                                    attackHit.collider.GetComponent<SlimeScript>().Attacked(attackHit.point);
                            }
                        }
                    }
                    CameraShake.instance.StartShake(0.5f);
                    //if (timeSinceLastSwing < 1)
                    //altSwing = !altSwing;
                    

                    

                    timeSinceLastSwing = 0;
                    

                    hasSwung = true;
                }

                if (attackSwingProgress <= 1)
                    attackSwingProgress += Time.deltaTime * 2;
                else
                    attackSwingProgress = 1;
                if (attackSwingProgress >= 1) {
                    isChargingAttack = false;
                    attackChargeTime = 0;
                    //altSwing = !altSwing;
                }

                
            }
            
        }
        if (timeSinceLastSwing <= 4)
            timeSinceLastSwing += Time.deltaTime * 1.5f;
        
        //if (timeSinceLastSwing >= 1 || attackChargeTime > 0.75f)
         //   altSwing = false;

        //Blink
        if (leftHandID == 2) {
            if (Mouse.current.rightButton.ReadValue() == 1 && !isTravelingBlink) {
                GameObject.Find("TestBlinkGraphics").transform.position = Vector3.zero;
                RaycastHit blinkHit;
                float tempBlinkDist = Keyboard.current.xKey.ReadValue() == 1 ? 1000 : maxBlinkReach;
                if (Physics.SphereCast(playerCam.position, .5f, playerCam.TransformDirection(Vector3.forward), out blinkHit, tempBlinkDist)) {
                    currentHitDistance = blinkHit.distance;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").transform.position = playerCam.position + playerCam.TransformDirection(Vector3.forward) * currentHitDistance;
                    //GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").LookAt(playerCam.transform);
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").rotation = Quaternion.LookRotation(blinkHit.normal);
                }
                else {
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").transform.position = playerCam.position + playerCam.TransformDirection(Vector3.forward) * tempBlinkDist;
                }

                isFindingBlink = true;

                RaycastHit blinkGroundHit;
                if (Physics.Raycast(GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position, Vector3.down, out blinkGroundHit, 10)) {
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_2").position = blinkGroundHit.point;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<LineRenderer>().SetPosition(1, GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_2").position);
                }
                else {
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_2").position = Vector3.zero;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<LineRenderer>().SetPosition(1, GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position);
                }

                RaycastHit blinkMantelHit;
                blinkMantelFound = Physics.Raycast(GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position + Vector3.up * maxBlinkMantelHeight + transform.TransformDirection(Vector3.forward), Vector3.down, out blinkMantelHit, 2f);
                if (blinkMantelFound || blinkWallrunFound) {
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<Renderer>().enabled = false;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<LineRenderer>().enabled = false;
                    if (blinkMantelFound)
                        GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetChild(0).GetComponent<Renderer>().enabled = true;
                    else
                        GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetChild(0).GetComponent<Renderer>().enabled = false;

                    if (blinkWallrunFound)
                        GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = true;
                    else
                        GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = false;

                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_2").GetComponent<Renderer>().enabled = false;
                }
                else {
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<Renderer>().enabled = true;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<LineRenderer>().enabled = true;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetChild(0).GetComponent<Renderer>().enabled = false;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetChild(0).GetChild(0).GetComponent<Renderer>().enabled = false;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_2").GetComponent<Renderer>().enabled = true;

                }

                RaycastHit wallrunHit;
                if (Physics.Raycast(GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position, transform.TransformDirection(Vector3.left), out wallrunHit, 0.6f) && !blinkMantelFound) {
                    //GameObject.Find("TestBlinkGraphics").transform.Find("TestGraphic").GetComponent<Renderer>().enabled = true;
                    //GameObject.Find("TestBlinkGraphics").transform.Find("TestGraphic").position = GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position;
                    blinkWallrunFound = true;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetChild(0).GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                    blinkWallrunID = 0;
                }
                else if (Physics.Raycast(GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position, transform.TransformDirection(Vector3.right), out wallrunHit, 0.6f) && !blinkMantelFound) {
                    //GameObject.Find("TestBlinkGraphics").transform.Find("TestGraphic").GetComponent<Renderer>().enabled = true;
                    //GameObject.Find("TestBlinkGraphics").transform.Find("TestGraphic").position = GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position;
                    blinkWallrunFound = true;
                    GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetChild(0).GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
                    blinkWallrunID = 1;

                }
                else {
                    //GameObject.Find("TestBlinkGraphics").transform.Find("TestGraphic").GetComponent<Renderer>().enabled = false;
                    blinkWallrunFound = false;
                }


                GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<LineRenderer>().SetPosition(0, GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position);

            }
            else {
                if (isFindingBlink && !isTravelingBlink) {
                    GetComponent<FPSInputController>().enabled = false;
                    GetComponent<CharacterMotor>().enabled = false;
                    GetComponent<CharacterController>().enabled = false;

                    targetBlinkDest = GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position - new Vector3(0, 0.49f, 0);
                    CameraShake.StartBlinkFX();
                    CameraShake.instance.StartShake(1);
                    isTravelingBlink = true;

                    //transform.position = GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position - new Vector3(0,0.49f,0);

                }

                GameObject.Find("TestBlinkGraphics").transform.position = Vector3.down * 100;
                GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<LineRenderer>().SetPosition(0, GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position);
                GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").GetComponent<LineRenderer>().SetPosition(1, GameObject.Find("TestBlinkGraphics").transform.Find("Graphic_1").position);

            }
        }
        if (isTravelingBlink) {

            transform.position = Vector3.Lerp(transform.position, targetBlinkDest, Time.deltaTime * 14);

            if(Vector3.Distance(transform.position, targetBlinkDest) < 0.1f) {
                transform.SendMessage("SetVelocity", Vector3.zero);

                GetComponent<FPSInputController>().enabled = true;
                GetComponent<CharacterMotor>().enabled = true;
                GetComponent<CharacterController>().enabled = true;

                isSprinting = true;
                isFindingBlink = false;
                isTravelingBlink = false;
            }
        }


        //----------------------------------------------------------------------------------------------------ANIM LOGIC
        camAnim.speed = isSpeedBoost ? 1.2f : 1;
        armsAnim.speed = isSpeedBoost ? 1.2f : 1;
        handAnim.speed = isSpeedBoost ? 1.2f : 1;
        legsAnim.speed = isSpeedBoost ? 1.2f : 1;
        legsSlideAnim.speed = isSpeedBoost ? 1.2f : 1;
        worldSpaceArmsAnim.speed = isSpeedBoost ? 1.2f: 1;
        
        //wsaRend.enabled = true;
        if (wsaRend.enabled) {
            wsaTransparencyScan += Time.unscaledDeltaTime * 3;
            wsaRend.material.SetFloat("_TransparentThreshold", wsaTransparency.Evaluate(wsaTransparencyScan));
        }
        else {
            ChangeWSAAnimationState("EMPTY", 0.02f);
            wsaTransparencyScan = 0;
            wsaRend.material.SetFloat("_TransparentThreshold", 1);
        }

        if (canAnimate) {
            if (isFindingBlink) {
                //Debug.LogError(8);
                ChangeArmsAnimationState("LowerShoulder", 0.4f, 2);
                if (isTravelingBlink)
                    ChangeArmsAnimationState("BlinkRelease", 0.02f, 1);
                else
                    ChangeArmsAnimationState("BlinkCharge", 0.2f, 1);
            }
            else {
                ChangeArmsAnimationState("ShoulderEmpty", 0.4f, 2);
                ChangeArmsAnimationState("LeftHandEmpty", 0.2f, 1);
            }
            if (isChargingAttack && !isWallrunning && !isSwingingChain && !isClimbingChain && !isOnRing && !ledgeClimb && !isConcealed && !isDodging && attackSwingProgress < 1) {
                if (cc.isGrounded) {
                    if (attackSwingProgress <= 0) {
                        if (timeSinceLastSwing > 1 || (timeSinceLastSwing < 1 && altSwing))
                            ChangeArmsAnimationState(a_AttackCharge, 0.05f, 0);
                    }
                    else {
                        //Debug.LogError(9);

                        if (altSwing)
                            ChangeArmsAnimationState(a_AttackSwingAlt, 0.03f, 0);
                        else
                            ChangeArmsAnimationState(a_AttackSwing, 0.03f, 0);
                        
                    }

                }
                else {
                    if(attackSwingProgress <= 0)
                        ChangeArmsAnimationState(a_AttackChargeOverhead, 0.3f, 0);
                    else
                        ChangeArmsAnimationState(a_AttackSwingOverhead, 0.02f, 0);
                }
            }
            else {
                if (cc.isGrounded) {
                    if (isSliding && !isDodging) {
                        ChangeArmsAnimationState(a_Slide, 0.3f, 0);
                        ChangeCamAnimationState(c_Slide, 0.2f);
                        if (isKicking) {
                            ChangeSlideLegsAnimationState(sl_Kicking, 0.2f);
                            ChangeLegsAnimationState(l_KickHold, 0.2f);
                        }
                        else {
                            if (hasKicked) {
                                ChangeSlideLegsAnimationState(sl_Kicking, 0.2f);
                                ChangeLegsAnimationState(l_KickRelease, 0.02f);
                            }
                            else {
                                ChangeSlideLegsAnimationState(sl_Slide, 0.2f);
                                ChangeLegsAnimationState(l_Idle, 0.2f);
                            }
                        }
                    }
                    else {
                        ChangeSlideLegsAnimationState(sl_Idle, 0.2f);
                        if (isKicking) {
                            ChangeLegsAnimationState(l_KickHold, 0.2f);
                        }
                        else {
                            if (hasKicked) {
                                ChangeLegsAnimationState(l_KickRelease, 0.02f);
                            }
                            else {
                                ChangeLegsAnimationState(l_Idle, 0.2f);
                                if (inputDirRaw != Vector2.zero) {
                                    if (isSprinting) {
                                        ChangeArmsAnimationState(a_Sprint, 0.2f, 0);
                                        ChangeCamAnimationState("Run", 0.2f);
                                    }
                                    else {
                                        ChangeArmsAnimationState(a_Walk, 0.2f, 0);
                                        ChangeCamAnimationState(c_Idle, 0.2f);
                                    }
                                }
                                else {
                                    ChangeCamAnimationState(c_Idle, 0.2f);
                                    ChangeArmsAnimationState(a_Idle, 0.3f, 0);
                                }
                            }
                        }
                    }
                }
                else {
                    if (isWallrunning) {
                        if (wrID == 1) {//left
                            ChangeCamAnimationState(c_WallrunLeft, 0.2f);
                            ChangeWSAAnimationState("WallrunLeft", 0.2f);
                        }
                        else if (wrID == 2) {
                            ChangeCamAnimationState(c_WallrunRight, 0.2f);
                            ChangeWSAAnimationState("WallrunRight", 0.2f);
                        }
                    }
                    else {
                        if (ledgeClimb) {
                            ChangeWSAAnimationState(wsa_LedgeClimb, 0.1f);
                            ChangeCamAnimationState(c_ClimbLedge, 0.2f);
                        }
                        else {
                            if (isOnRing) {
                                ChangeWSAAnimationState(wsa_RingGrab, 0.1f);
                                ChangeCamAnimationState(c_RingGrab, 0.2f);
                            }
                            else {
                                if (isOnPole || isRopeWalking) {
                                    if (isOnPole) {
                                        ChangeArmsAnimationState(a_RopeBalance, 0.1f, 0);
                                        ChangeCamAnimationState(c_PoleLand, 0.2f);
                                    }
                                    if (isRopeWalking) {
                                        if (inputDirRaw != Vector2.zero)
                                            ChangeArmsAnimationState(a_Walk, 0.1f, 0);
                                        else
                                            ChangeArmsAnimationState(a_RopeBalance, 0.1f, 0);
                                    }
                                }
                                else {
                                    if (hasjumped) {
                                        if (isSprinting || requiresLeap) {
                                            ChangeArmsAnimationState(a_Leap, 0.1f, 0);
                                        }
                                        else if (airTime < 0.7f)
                                            ChangeArmsAnimationState(a_IdleJump, 0.2f, 0);
                                        else
                                            ChangeArmsAnimationState(a_Fall, 0.1f, 0);
                                        ChangeCamAnimationState(c_Jump, 0.2f);

                                    }
                                    else {
                                        ChangeArmsAnimationState(a_Fall, 0.1f, 0);
                                    }
                                }
                            }
                        }


                    }

                }
            }


        }
        //----------------------------------------------------------------------------------------------------ANIM LOGIC
    }

    void ChangeCamAnimationState(string newState, float transitionTime)
    {
        if (!playedHardLandAnim && cc.isGrounded)
            newState = "Land";

        if (currentCamState == newState) return;
    
        //armsAnim.Play(newState);
        camAnim.CrossFadeInFixedTime(newState, 0.2f);
        currentCamState = newState;


    }
    void ChangeArmsAnimationState(string newState, float transitionTime, int layer)
    {

        
        if(layer == 0) {
            if (currentArmsState == newState) return;

            armsAnim.CrossFadeInFixedTime(newState, transitionTime, 0);
            handAnim.CrossFadeInFixedTime(newState, transitionTime, 0);
            currentArmsState = newState;
        }
        if (layer == 1) {
            if (currentArmsLeftHandState == newState) return;

            armsAnim.CrossFadeInFixedTime(newState, transitionTime, 1);
            handAnim.CrossFadeInFixedTime(newState, transitionTime, 1);
            currentArmsLeftHandState = newState;
        }
        if (layer == 2) {
            if (currentArmsRightShoulderState == newState) return;

            armsAnim.CrossFadeInFixedTime(newState, transitionTime, 2);
            handAnim.CrossFadeInFixedTime(newState, transitionTime, 2);
            currentArmsRightShoulderState = newState;
        }
        
    }
    void ChangeLegsAnimationState(string newState, float transitionTime)
    {
        if (currentLegsState == newState) return;

        //armsAnim.Play(newState);
        legsAnim.CrossFadeInFixedTime(newState, transitionTime, 0);
        currentLegsState = newState;
    }
    void ChangeSlideLegsAnimationState(string newState, float transitionTime)
    {
        if (currentSlideLegsState == newState) return;

        //armsAnim.Play(newState);
        legsSlideAnim.CrossFadeInFixedTime(newState, 0.2f, 0);
        currentSlideLegsState = newState;
    }

    void ChangeWSAAnimationState(string newState, float transitionTime)
    {
        if (currentWSAState == newState) return;

        //armsAnim.Play(newState);
        worldSpaceArmsAnim.Play (newState, 0);
        currentWSAState = newState;
    }


    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 1);
        Gizmos.DrawWireSphere(playerCam.position + playerCam.TransformDirection(Vector3.forward) * currentHitDistance, 1f);
    }
    */
    void LateUpdate()
    {
        Vector3 camHolderFWD = camHolder.position + camHolder.TransformDirection(Vector3.forward);
        Vector3 armsHolderFWD = camHolder.position + armsHolder.TransformDirection(Vector3.forward);

        armsLerpMultiplier =  Mathf.Clamp01(Vector3.Distance(camHolderFWD, armsHolderFWD)) * 3;

        if (isSwingingChain)
            transform.position = heldPosition;

        //armsHolder.rotation = camHolder.rotation;
        armsHolder.position = camHolder.position;
        //armsHolder.rotation = Quaternion.Slerp(armsHolder.rotation, camHolder.rotation, Time.deltaTime * 57 );//* armsLerpMultiplier
        armsHolder.rotation = camHolder.rotation;

        if (Keyboard.current.digit8Key.wasPressedThisFrame) {
            GetComponent<FPSInputController>().enabled = false;
            GetComponent<CharacterMotor>().enabled = false;
            GetComponent<CharacterController>().enabled = false;

            //transform.position = Vector3.zero;
            RaycastHit debugHit;
            if (Physics.SphereCast(playerCam.position, .5f, playerCam.TransformDirection(Vector3.forward), out debugHit, 900)) {
                transform.position = debugHit.point;
            }

            GetComponent<FPSInputController>().enabled = true;
            GetComponent<CharacterMotor>().enabled = true;
            GetComponent<CharacterController>().enabled = true;
        }

        if (isFading) {
            targetBlinkDest = GameObject.Find(resetPointName).transform.position;

            float totalFadeTime = 1;
            float fadeSpeed = 15;
            fadeToBlackTime += Time.deltaTime;
            if(fadeToBlackTime < totalFadeTime / 2) {
                fadeToBlack.color = Color.Lerp(fadeToBlack.color, new Color(0,0,0,1), Time.unscaledDeltaTime * fadeSpeed);
            }
            else {
                fadeToBlack.color = Color.Lerp(fadeToBlack.color, new Color(0, 0, 0, 0), Time.unscaledDeltaTime * fadeSpeed);
            }

            if(fadeToBlackTime > ((totalFadeTime / 2)-0.05f) && fadeToBlackTime < ((totalFadeTime / 2) + 0.05f)){
                GetComponent<FPSInputController>().enabled = false;
                GetComponent<MouseLook>().enabled = false;
                GetComponent<CharacterMotor>().enabled = false;
                GetComponent<CharacterController>().enabled = false;

                transform.position = GameObject.Find(resetPointName).transform.position;
                transform.rotation = GameObject.Find(resetPointName).transform.rotation;

                GetComponent<FPSInputController>().enabled = true;
                GetComponent<MouseLook>().enabled = true;
                GetComponent<CharacterMotor>().enabled = true;
                GetComponent<CharacterController>().enabled = true;

                GetComponent<CharacterMotor>().SetVelocity(Vector3.zero);
            }

            if (fadeToBlackTime >= totalFadeTime) {
                fadeToBlackTime = 0; isFading = false; resetPointName = null;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Respawn") {
            //Debug.LogError("0uwhe");
            if(col.gameObject.name == "IslandRespawnTrigger" && !isFading) {
                GameObject islandRespawnPoint = GameObject.Find(col.gameObject.name + "_ResetPos");
                Vector3 islandCastPos = Vector3.Lerp(transform.position, GameObject.Find("IslandCenterPoint").transform.position,0.2f) + 
                    new Vector3(0,90,0);
                islandRespawnPoint.transform.position = islandCastPos;
                RaycastHit hit;
                if(Physics.Raycast(islandCastPos, Vector3.down, out hit, 180)) {
                    islandRespawnPoint.transform.position = hit.point + new Vector3(0,1f,0);
                }


                resetPointName = col.gameObject.name + "_ResetPos";
                isFading = true;
                isTravelingBlink = false;
            } else if(GameObject.Find(col.gameObject.name + "_ResetPos")) {
                resetPointName = col.gameObject.name + "_ResetPos";
                isFading = true;
                isTravelingBlink = false;
            }
        }

        if(col.tag == "Pole") {
            heldPosition = col.transform.position;
            isOnPole = true;
            isPositionHeld = true;
        }
        if (col.tag == "Ring") {
            heldPosition = col.transform.position;
            isOnRing = true;
            isPositionHeld = true;

            worldSpaceArms.position = heldPosition;
            worldSpaceArms.LookAt(col.transform.position + col.transform.TransformDirection(Vector3.back));
            //worldSpaceArms.rotation
            wsaRend.enabled = true;
        }
        if (col.tag == "ChainStationary" && chainCooldown <= 0) {
            Vector3 chainPlayerPos = new Vector3(col.transform.position.x, transform.position.y, col.transform.position.z);
            heldPosition = chainPlayerPos + (transform.position - chainPlayerPos).normalized * 0.75f;
            isClimbingChain = true;
            isPositionHeld = true;
            currentChain = col.gameObject;
            currentChain.GetComponent<CapsuleCollider>().enabled = false;
        }
        if (col.tag == "ChainSwing" && chainCooldown <= 0) {
            Vector3 chainPlayerPos = new Vector3(col.transform.position.x, transform.position.y, col.transform.position.z);
            heldPosition = chainPlayerPos + (transform.position - chainPlayerPos).normalized * 0.75f;
            isSwingingChain = true;
            isPositionHeld = true;
            if (currentChain != null && wasSwinging) {
                if(currentChain.transform.parent != null)
                    currentChain.transform.parent.rotation = chainStartingRotation;
            }
            currentChain = col.gameObject;
            chainStartingRotation = currentChain.transform.parent.rotation;
            currentChain.GetComponent<CapsuleCollider>().enabled = false;

            
            chainSwingHolder.position = currentChain.transform.parent.parent.position;
            initalParent = currentChain.transform.parent.parent.gameObject;
            currentChain.transform.parent.parent = chainSwingHolder.Find("CurrentSwing");
            chainSwingHolder.rotation = transform.rotation;
            holdOnPoint.position = transform.position;
            holdOnPoint.parent = currentChain.transform;
            //holdOnPoint.localPosition = new Vector3(holdOnPoint.localPosition.x, 0, holdOnPoint.localPosition.z);
        }
        if (col.tag == "Rope" || col.tag == "RopeInverted") {

            if(currentRope != null) {
                if (currentRope != GameObject.Find(GameObject.Find(col.gameObject.name).name.Replace("*", "")))
                    SetUpRope(col); //Continue along current rope
            }
            else {
                if (oldRope != null) {
                    if (oldRope == GameObject.Find(GameObject.Find(col.gameObject.name).name.Replace("*", "")) && oldRopeCooldown <= 0)
                        SetUpRope(col); //landing back on prev rope after cooldown
                    else if (oldRope != GameObject.Find(GameObject.Find(col.gameObject.name).name.Replace("*", "")))
                        SetUpRope(col); //new different rope regardless of cooldown
                }
                else
                    SetUpRope(col); //first rope use
            }
        }
        
    }

    private void OnTriggerStay(Collider col)
    {
        /*
        if (col.gameObject.name == "ResetTrigger") {
            cc.enabled = false;
            transform.position = GameObject.Find("ResetPoint").transform.position;
            cc.enabled = true;
        }
        */
    }

    void SetUpRope(Collider col)
    {
        currentRope = GameObject.Find(GameObject.Find(col.gameObject.name).name.Replace("*", ""));
        float lastBestDist = 10;

        for (int i = 0; i < currentRope.GetComponent<MeshFilter>().mesh.vertexCount; i++) {
            if ((transform.position - currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[i])).magnitude < lastBestDist) {
                lastBestDist = (transform.position - currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[i])).magnitude;
                currentRopeIndex = i;
            }
        }

        if (currentRopeIndex == 0)
            currentRopeIndex = 1;
        else if (currentRopeIndex == currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 1)
            currentRopeIndex = currentRope.GetComponent<MeshFilter>().mesh.vertices.Length - 2;

        //GameObject.Find("TesterSphere").transform.position = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex]);
        //Debug.LogError("YES");

        heldPosition = currentRope.transform.TransformPoint(currentRope.GetComponent<MeshFilter>().mesh.vertices[currentRopeIndex]);
        isRopeWalking = true;
        isPositionHeld = true;

        isRopeInverted = col.tag == "RopeInverted";
    }

    //----Input

    public void OnMovement(InputAction.CallbackContext value){ inputDirRaw = value.ReadValue<Vector2>(); }
    public void OnLook(InputAction.CallbackContext value) {
        if (value.action.activeControl.layout.ToString() == "Button")
            lookAxis = value.ReadValue<Vector2>() * 0.5f;
        else
            lookAxis = value.ReadValue<Vector2>() * 0.025f;
    }
    public void OnSprint(InputAction.CallbackContext value){ sprintInput_ = value; }
    public void OnSprintTap(InputAction.CallbackContext value) { sprintTapInput_ = value; }
    public void OnJumpInput(InputAction.CallbackContext value){ jumpHeldInput_ = value; }
    public void OnJumpTap(InputAction.CallbackContext value) { jumpTapInput_ = value; }
    public void OnSlide(InputAction.CallbackContext value){ slideInput_ = value; }
    public void OnCrouch(InputAction.CallbackContext value){ crouchInput_ = value; }
    public void OnAttack(InputAction.CallbackContext value) { attackInput_ = value; }
    public void OnBlockMagic(InputAction.CallbackContext value) { blockMagicInput_ = value; }
    public void OnKick(InputAction.CallbackContext value) { kickInput_ = value; }
    public void OnDebugKey(InputAction.CallbackContext value) { debugKeyInput_ = value; }
}
