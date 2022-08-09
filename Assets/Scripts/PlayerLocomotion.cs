using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

class CharacterState
{
    public PlayerLocomotion pl;
    virtual public CharacterState handelInput() {
        return this;
    }
}
class WalkState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = true;
        pl.currentState = PlayerLocomotion.PlayerState.Idle;

        
        //    return new RunningState();
        if (Inputs.attackPressed || Inputs.attackHeld) {
            //return new AttackingState();
        }
        if (Inputs.jumpPressed && pl.cc.isGrounded) {
            pl.hasJumped = true;
            pl.extraVel.y = pl.jumpPower;
            pl.jumpDelay = 0.3f;
            if (Inputs.sprintHeld && Inputs.inputDirRaw.y > 0.1f)
                pl.leap = true;
            return new FallingState();
        }



        if(pl.atkSubstate >= 2) {
            //Attack Animations
        } else {
            if (Inputs.inputDirRaw == Vector2.zero) {
                pl.ChangeArmsAnimationState("Idle", 0.2f, 0);
                pl.currentState = PlayerLocomotion.PlayerState.Idle;
            } else {
                if (Inputs.sprintHeld) {
                    pl.ChangeArmsAnimationState("Sprint", 0.2f, 0);
                    pl.currentState = PlayerLocomotion.PlayerState.Sprinting;
                } else {
                    pl.ChangeArmsAnimationState("Walk", 0.2f, 0);
                    pl.currentState = PlayerLocomotion.PlayerState.Walking;
                }
            }
        }

        return this;
    }
}

class FallingState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = true;

        if (pl.hasJumped && pl.cc.isGrounded && pl.jumpDelay <= 0f) {
            pl.hasJumped = false;
            pl.leap = false;
            return new WalkState();
        }


        if (pl.atkSubstate >= 2) {
            //Attack Animations
        } else {
            if (pl.hasJumped) {
                if(pl.leap)
                    pl.ChangeArmsAnimationState("Leap", 0.2f, 0);
                else
                    pl.ChangeArmsAnimationState("Idle Jump", 0.2f, 0);
            } else {
                pl.ChangeArmsAnimationState("Fall", 0.2f, 0);

            }
        }

        return this;
    }
}

class RingState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = false;
        return this;
    }
}

class ChainClimbState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = false;
        return this;
    }
}

class ChainSwingState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = false;
        return this;
    }
}

class LedgeScaleState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = false;
        return this;
    }
}

class PoleJumpState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = true;
        return this;
    }
}

class RopeBalanceState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = true;
        return this;
    }
}

class WallrunState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = true;
        return this;
    }
}

class BlinkState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = false;
        return this;
    }
}

class RunningState : CharacterState
{
    override public CharacterState handelInput() {
        pl.attackState = true;

        if (pl.atkSubstate >= 2) {
            //Attack Animations
        } else {
            if (Inputs.sprintHeld) {
                pl.currentState = PlayerLocomotion.PlayerState.Sprinting;
                pl.ChangeArmsAnimationState("Sprint", 0.2f, 0);

            } else {
                pl.ChangeArmsAnimationState("Walk", 0.2f, 0);
                pl.currentState = PlayerLocomotion.PlayerState.Walking;
            }
        }

        
        //if (Keyboard.current.gKey.wasPressedThisFrame) {
        //    return new IdleState();
        //}
        //PlayerLocomotion.cc.Move(new Vector3(Inputs.inputDirRaw.x,0, Inputs.inputDirRaw.y));

        if (Inputs.inputDirRaw == Vector2.zero)
            return new WalkState();
        if (Inputs.attackPressed || Inputs.attackHeld) {
            //Debug.LogError("ddd");
            //return new AttackingState();
        }
        if (Inputs.blockPressed || Inputs.blockHeld) {
            //return new BlockingState();
        }
        if (Inputs.slidePressed) {
            //Debug.LogError("ddd");
            return new SlidingState();
        }
        //return new AttackingState();
        
        return this;
    }
}

class AttackingState : CharacterState
{
    private float attackReleaseTime = 0;
    public bool hasReleasedAttack = false;

    override public CharacterState handelInput() {
        pl.currentState = PlayerLocomotion.PlayerState.Attacking;
        pl.P_hasReleaseAttack = hasReleasedAttack;
        //if (Inputs.inputDirRaw == Vector2.zero)
        //    return new IdleState();
        if ((Inputs.attackPressed || Inputs.attackPressed) && !hasReleasedAttack) {
            hasReleasedAttack = false;
            attackReleaseTime = 0;
        }
        if((Inputs.attackReleased || !Inputs.attackHeld) && !hasReleasedAttack) {
            hasReleasedAttack = true;
            attackReleaseTime = 1;
        }

        if (attackReleaseTime > 0 && hasReleasedAttack) {
            attackReleaseTime -= Time.deltaTime;
        }
        if (attackReleaseTime <= 0 && hasReleasedAttack) {
            return new WalkState();
        }
        if(pl.currentWeapon == PlayerLocomotion.Holding.Bow)
            CamShake.instance.SetTargetFOV(hasReleasedAttack ? 75 : 80);

        //    return new RunningState();
        if(hasReleasedAttack)
            pl.ChangeArmsAnimationState("AttackRelease", 0.02f, 0);
        else
            pl.ChangeArmsAnimationState("AttackCharge", 0.2f, 0);
        return this;
    }
}

class BlockingState : CharacterState
{
    private float blockReleaseTime = 0;
    bool hasReleasedBlock = false;

    float rockShootDelay = 0.2f;
    bool shotRock = false;

    bool hasGrabbed = false;
    public GameObject currentGrabbed;
    Vector3 grabOffset;
    float holdDist = 10f;
    float startDrag = 0f;
    public static bool forceLetGo = false;

    public static float TKpushDirLerp = 0;
    public float TKpushDirLerp_Anim = 0;

    override public CharacterState handelInput() {
        pl.currentState = PlayerLocomotion.PlayerState.Blocking;


        if (pl.currentWeapon == PlayerLocomotion.Holding.Shortsword) {
            //if(pl.magicID == 1)
                
        }
        if (pl.currentWeapon == PlayerLocomotion.Holding.Shield) {

        }

        if ((Inputs.blockPressed || Inputs.blockPressed) && !hasReleasedBlock) {
            hasReleasedBlock = false;
            blockReleaseTime = 0;
        }
        if ((Inputs.blockReleased || !Inputs.blockHeld) && !hasReleasedBlock) {
            hasReleasedBlock = true;
            if(pl.magicID != 2)
                blockReleaseTime = 1;
            else
                blockReleaseTime = 0.15f;
        }

        if (blockReleaseTime > 0 && hasReleasedBlock) {
            blockReleaseTime -= Time.deltaTime;
        }
        if (blockReleaseTime <= 0 && hasReleasedBlock) {
            //if(pl.magicID == 0)
                pl.ChangeArmsAnimationState("LeftHandEmpty", 0.2f, 1);
            if (pl.magicID != 92)
                return new WalkState();
        }
        if (pl.currentWeapon == PlayerLocomotion.Holding.Bow)
            CamShake.instance.SetTargetFOV(hasReleasedBlock ? 75 : 80);

        //    return new RunningState();

        if(pl.magicID == 1) {
            
            PlayerGraphics.searchingBlink = Inputs.blockHeld;
            PlayerGraphics.blinkTargetPos = pl.cam.transform.position + pl.cam.transform.TransformDirection(Vector3.forward * 10f);
            if (Inputs.blockHeld) {
                PlayerGraphics.finderTargetPos = PlayerGraphics.blinkTargetPos;
            } else {
                PlayerGraphics.finderTargetPos = Vector3.zero;

            }
        }

        if (pl.magicID == 4) {
            if (Keyboard.current.digit5Key.wasPressedThisFrame || (pl.currentState == PlayerLocomotion.PlayerState.Blocking && Inputs.blockReleased)) {
                pl.rockShootPos.position = pl.cam.transform.position;
                pl.rockShootPos.rotation = pl.cam.transform.rotation;
                rockShootDelay = 0.2f;
                shotRock = true;
            }
            if (shotRock) {
                if (rockShootDelay > 0)
                    rockShootDelay -= Time.deltaTime;
                else {
                    float rot = Random.Range(0.99f, 360f);
                    for (int i = 0; i < 3; i++) {
                        float spread = 0.065f;//0.05f
                        Vector3 camFWDPos = pl.rockShootPos.transform.forward + (pl.rockShootPos.transform.right * spread);
                        camFWDPos = Quaternion.AngleAxis(rot, pl.rockShootPos.transform.forward) * camFWDPos;

                        //Vector3 dir = camFWDPos + cam.transform.TransformDirection(new Vector3(Random.Range(-0.99f,0.99f), Random.Range(-0.99f, 0.99f), 0) * spread);
                        //Vector3 dir = camFWDPos + cam.transform.TransformDirection(new Vector3(0, 0, 0) * spread);

                        Vector3 dir = Quaternion.AngleAxis((240) * i, pl.rockShootPos.transform.forward) * camFWDPos;


                        //newRock.transform.localScale *= 0.5f;

                        RaycastHit hit;
                        if (Physics.Raycast(pl.rockShootPos.transform.position, dir, out hit, 60)) {
                            //GameObject G = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            //G.transform.position = hit.point;
                            //G.transform.localScale *= 0.1f;

                            GameObject loadedRock = Resources.Load("RockProjectile") as GameObject;
                            GameObject newRock = PlayerLocomotion.Instantiate(loadedRock, hit.point, Quaternion.LookRotation(dir));
                            newRock.GetComponent<RockProj>().sendMSG = hit.transform.gameObject;
                            newRock.transform.localScale *= Random.Range(0.82f, 0.99f) * 1.2f;
                        }
                    }
                    PlayerGraphics.extraHandIntensity = 30f;
                    CamShake.instance.Shake(0.15f,1f);
                    shotRock = false;
                }
            }
        }

        TKpushDirLerp = Mathf.Lerp(TKpushDirLerp, (Inputs.weaponScrollUp ? 1 : 0) + (Inputs.weaponScrollDown ? -1 : 0), Time.deltaTime * 12f);
        TKpushDirLerp_Anim = Mathf.Lerp(TKpushDirLerp_Anim, ((Inputs.weaponScrollUp ? 1 : 0) + (Inputs.weaponScrollDown ? -1 : 0)) * 10f, Time.deltaTime * 2f);
        if (pl.magicID == 2) {
            if (Inputs.blockHeld && !forceLetGo) {
                if (!hasGrabbed && !forceLetGo) {
                    forceLetGo = false;
                    RaycastHit hit;
                    if (Physics.Raycast(pl.cam.transform.position, pl.cam.transform.forward, out hit, 20)) {
                        if (hit.transform.gameObject.GetComponent<Rigidbody>() && hit.transform.childCount > 0) {
                            currentGrabbed = hit.transform.gameObject;
                            currentGrabbed.GetComponent<Rigidbody>().useGravity = false;
                            startDrag = currentGrabbed.GetComponent<Rigidbody>().drag;
                            PlayerGraphics.grabbedVFX = true;
                            PlayerGraphics.finderLerpPos = (pl.cam.transform.position + hit.point) / 2;
                            holdDist = Vector3.Distance(pl.cam.transform.position, hit.point);
                            hasGrabbed = true;
                            grabOffset = currentGrabbed.transform.position - hit.point;
                            
                        }
                    }
                } else if(currentGrabbed != null && !forceLetGo){
                    PlayerGraphics.finderTargetPos = currentGrabbed.transform.GetChild(0).position;
                    holdDist += TKpushDirLerp * Time.deltaTime * 250f;//50f
                    holdDist = Mathf.Clamp(holdDist, 2f, 20f);
                    pl.TK_VFX.position = currentGrabbed.transform.GetChild(0).position;
                    Vector3 dest = pl.cam.transform.position + (pl.cam.transform.forward * holdDist) + grabOffset;

                    float camForce = Vector3.Distance(dest, pl.cam.transform.position) * 40f;
                    float force = Vector3.Distance(dest, currentGrabbed.transform.position) * 30f * camForce;//10f
                    Debug.Log(force);
                    force = Mathf.Clamp(force, 5f, 150f);


                    currentGrabbed.GetComponent<Rigidbody>().AddForce((dest - currentGrabbed.transform.position) * force);
                    currentGrabbed.GetComponent<Rigidbody>().drag = 10f;
                    //currentGrabbed.transform.position = dest;
                }
            } else {
                //Debug.LogError("LG");
                if (hasGrabbed || forceLetGo || currentGrabbed != null) {
                    if (currentGrabbed != null) {
                        currentGrabbed.GetComponent<Rigidbody>().useGravity = true;
                        currentGrabbed.GetComponent<Rigidbody>().drag = startDrag;
                    }
                    
                    PlayerGraphics.grabbedVFX = false;
                    PlayerGraphics.finderTargetPos = Vector3.zero;
                    
                    forceLetGo = false;
                    currentGrabbed = null;
                    //Debug.LogError("LG");
                    hasGrabbed = false;
                }
            }
        }


        if (hasReleasedBlock) {
            if(pl.magicID != 0)
                pl.ChangeArmsAnimationState("MagicRelease_" + pl.magicID.ToString(), 0.02f, 1);
        } else {
            if(pl.currentState == PlayerLocomotion.PlayerState.Blocking && pl.magicID == 0) {
                pl.ChangeArmsAnimationState("BlockCharge", 0.075f, 0);
            } else {
                if (pl.magicID != 0) {
                    if (pl.magicID == 2 && hasGrabbed) {
                        if (TKpushDirLerp_Anim > 0.1f)
                            pl.ChangeArmsAnimationState("MagicChargePush_" + pl.magicID.ToString(), 0.2f, 1);
                        else if (TKpushDirLerp_Anim < -0.1f)
                            pl.ChangeArmsAnimationState("MagicChargePull_" + pl.magicID.ToString(), 0.2f, 1);
                        else
                            pl.ChangeArmsAnimationState("MagicCharge_" + pl.magicID.ToString(), 0.2f, 1);
                    } else {
                        pl.ChangeArmsAnimationState("MagicCharge_" + pl.magicID.ToString(), 0.2f, 1);
                    }
                }
            }
            
        }

        /*
        if (pl.magicID != 0) {
            //pl.ChangeArmsAnimationState("MagicCharge_" + pl.magicID.ToString(), 0.2f, 1);
            pl.ChangeArmsAnimationState("MagicCharge_" + "4", 0.2f, 1);
        } else {
            pl.ChangeArmsAnimationState("Block", 0.2f, 1);
        }
        */

        return this;
    }
}

class SlidingState : CharacterState
{
    float slideTime = 10f;
    
    override public CharacterState handelInput() {
        pl.attackState = true;
        pl.currentState = PlayerLocomotion.PlayerState.Sliding;
        pl.gravityVector = new Vector3(0,-9.8f,0);
        //pl.gravityVector = new Vector3(0,0,0);

        RaycastHit hit;
        if(Physics.Raycast(pl.cc.transform.position + new Vector3(0,1.0f,0), pl.cc.transform.TransformDirection(Vector3.down), out hit, 10.5f)) {

            Quaternion surface = Quaternion.LookRotation(hit.normal);
            //surface = Quaternion.AngleAxis();
            pl.slideDir.rotation = Quaternion.Lerp(pl.slideDir.rotation, surface, Time.deltaTime * 2.5f);
            //Debug.LogError("SLD");

            bool frontBehind = true;
            Vector3 A = pl.transform.position + pl.slideDir.TransformDirection(Vector3.up);
            Vector3 B = pl.transform.position + pl.slideDir.TransformDirection(Vector3.down);
            Vector3 C = pl.transform.position + (pl.playerVelDir * 2f);
            frontBehind = Vector3.Distance(A, C) > Vector3.Distance(B, C);

            //frontBehind = false;

            GameObject.Find("_#1").transform.position = A;
            GameObject.Find("_#2").transform.position = B;
            GameObject.Find("_#3").transform.position = C;

            //pl.slideVector = ((pl.transform.position + pl.slideDir.TransformDirection(frontBehind ? Vector3.up : Vector3.down)) - pl.transform.position).normalized * 10f;

            float highestVal = A.y > B.y ? A.y : B.y;
            float smallestVal = A.y < B.y ? A.y : B.y; ;

            //float slope = highestVal - smallestVal;
            float slope = Vector3.Angle(hit.normal, Vector3.up);
            
            pl.slideSpeed += pl.slideAccel.Evaluate(slope) * Time.deltaTime * 1500f;
            pl.slideSpeed = Mathf.Clamp(pl.slideSpeed,0.0f, 9999f);
            Debug.Log(pl.slideSpeed);
            pl.slideDir.GetChild(0).localRotation = Quaternion.Lerp(pl.slideDir.GetChild(1).localRotation, pl.slideDir.GetChild(2).localRotation, (Inputs.inputDirSmoothed.x + 1) / 2);
            pl.slideVector = pl.slideDir.GetChild(0).TransformDirection(new Vector3(0, frontBehind ? -pl.slideSpeed : pl.slideSpeed, 0) * Time.deltaTime);

            
        } else {

        }
        


        if (slideTime > 0) {
            slideTime -= Time.deltaTime;
            if(slideTime < 9f) {
                pl.armsAnim.speed = 0.01f;
                pl.handAnim.speed = 0.01f;
            }
        } else {
            slideTime = 10f;
            pl.armsAnim.speed = 1f;
            pl.handAnim.speed = 1f;
            pl.slideVector = Vector3.zero;
            pl.gravityVector = new Vector3(0, pl.gravity, 0);
            pl.slideSpeed = 1000f;
            return new WalkState();
        }

        pl.ChangeArmsAnimationState("Slide", 0.2f, 0);

        return this;
    }
}

public class PlayerLocomotion : MonoBehaviour
{
    private CharacterState state;
    
    private bool keyboardOrGamepad = false; //true = GAMEPAD
    

    Vector2 lastFrameInputDir;
    Vector2 lastFrameLook;
    float lastFrame_LTrigger = 0;
    float lastFrame_RTrigger = 0;



    [Header("Exposed Variables")]
    public CharacterController cc;
    public Holding currentWeapon = Holding.Shortsword;
    public int magicID = 0; //0 = None, 1 = Blink
    public float forwardSpeed = 10;
    public float sprintMod = 1.35f;
    public float sidewaysSpeed = 8;
    public float backwardSpeed = 6;
    public float gravity = -9.81f;
    public float jumpPower = 30f;
    private float currentFallSpeed = 0f;
    public float maxFallSpeed = -20f;
    public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));



    [Header("Misc Variables")]
    float speedMod = 1f;
    public Camera cam;
    public UIManager uiMan;
    public List<Transform> tilts;
    public Transform tiltHolder;
    public Transform tiltLR;
    public Transform tiltFB;
    Vector2 tiltLerp;
    float tiltAmount = 0.12f;
    public List<Transform> armsTilts;
    public Transform armsTiltHolder;
    public Transform armsTiltLR;
    public Transform armsTiltFB;
    Vector2 armsTiltLerp;
    float armsTiltAmount = 0.12f;
    public List<Transform> lookTilts;
    public Transform lookTiltHolder;
    public Transform lookTiltLR;
    public Transform lookTiltFB;
    Vector2 lookTiltLerp;
    float lookTiltAmount = 0.1f;
    public Animator armsAnim;
    public Animator handAnim;

    public Vector3 playerSpd;

    public Transform armsConceal;

    public Transform shortswords;
    public Transform broadswords;
    public Transform shields;
    public Transform bows;
    public Transform magic;

    float airTime = 0;

    float weaponSwitchTime = 0;
    public AnimationCurve weaponSwitchCurve;
    bool hasSwitchedWeapon = false;

    public Vector3 slideVector = new Vector3(0,0,0);
    public Vector3 gravityVector = new Vector3(0,-9.8f,0);

    public float slideSpeed = 1000f;

    public Transform slideDir;
    //float attackReleaseTime;
    public Vector3 playerVelDir;
    private Vector3 lastPlayerPos;
    float lastPosInterval = 0f;

    public Transform rockShootPos;
    //Animation
    public enum Holding
    {
        Shortsword, //or magic
        Broadsword,
        Shield,
        Bow
    }
    public enum PlayerState
    {
        Idle, //or magic
        Walking,
        Sprinting,
        Attacking,
        Blocking,
        Sliding,
    }
    public PlayerState currentState;
    public Holding newWeapon = Holding.Shortsword;

    private float animSpeed = 1;//3
    private string currentArmsState;
    private string currentArmsLeftHandState;
    private string currentArmsRightShoulderState;

    public AnimationCurve slideAccel;

    public Transform TK_VFX;
    public bool P_hasReleaseAttack;

    public bool attackState = false;
    public int atkSubstate = 0; //0 = Deactivated, 1 = Activated, 2 = Attacking, 3 = Magic,

    float attackChargeTime = 0;
    public float attackReleaseTime = 0;
    public bool isChargingAttack = false;
    public bool isReleasingAttack = false;
    float minChargeTime = 0.25f;
    float minReleaseTime = 0.8f;
    bool releaseAction = false;
    float currentReleaseActionTime = 0;
    public float releaseActionTime = 0.05f;

    float lastAttackDelay = 0;
    bool hasAltSwing = true; //Weapon can alt swing
    bool hasOverhead = true; 
    bool isAltSwing = false; //Player needs to alt swing
    bool isOverhead = false;
    bool needsToAltSwing = false;
    bool needsToSlam = false;
    float maxAltSwingTime = 1f;

    float magicChargeTime = 0;
    float magicReleaseTime = 0;
    bool isChargingMagic = false;
    bool isReleasingMagic = false;
    float minMagicChargeTime = 0.75f;
    float minMagicReleaseTime = 0.55f;
    bool releaseMagicAction = false;
    float currentReleaseMagicActionTime = 0;
    public float releaseMagicActionTime = 0.05f;

    float interactDelay = 0f;

    public bool hasJumped = false;
    public bool leap = false;
    public float jumpDelay = 0f;
    public Vector3 extraVel;

    bool needsToReleaseAttackBlock = false;

    //Weapon Details

    public int damage = 10;
    public float critChance = 0.2f;
    public int critDamage = 20;
    public float weaponWalkMod = 1f;
    public float weaponSprintMod = 1f;
    public float chargeSpeed = 1f;
    public float releaseSpeed = 1f;

    public Transform oneHanded_Pos;
    public Transform twoHanded_Pos;
    public Transform bow_Pos;
    public Transform shield_Pos;

    public Transform displayArrow;

    //public Transform blinkVFXPos;

    bool hasGrabbed = false;
    public GameObject currentGrabbed;
    Vector3 grabOffset;
    float holdDist = 10f;
    float startDrag = 0f;
    public static bool forceLetGo = false;

    public static float TKpushDirLerp = 0;
    public float TKpushDirLerp_Anim = 0;
    public static float TKCooldown = 0;

    void Start()
    {
        state = new CharacterState();
        //state = GetComponent<CharacterState>();
        state = new WalkState();

        gravityVector = new Vector3(0, gravity, 0);

        tilts[0].localEulerAngles = new Vector3(0, 0, 15) * tiltAmount;
        tilts[1].localEulerAngles = new Vector3(0, 0, -15) * tiltAmount;
        tilts[2].localEulerAngles = new Vector3(15, 0, 0) * tiltAmount;
        tilts[3].localEulerAngles = new Vector3(-15, 0, 0) * tiltAmount;

        armsTilts[0].localEulerAngles = new Vector3(0, 0, 15) * armsTiltAmount;
        armsTilts[1].localEulerAngles = new Vector3(0, 0, -15) * armsTiltAmount;
        armsTilts[2].localEulerAngles = new Vector3(15, 0, 0) * armsTiltAmount;
        armsTilts[3].localEulerAngles = new Vector3(-15, 0, 0) * armsTiltAmount;

        //lookTilts[0].localEulerAngles = new Vector3(0, 0, 15) * lookTiltAmount;
        //lookTilts[1].localEulerAngles = new Vector3(0, 0, -15) * lookTiltAmount;
        lookTilts[0].localEulerAngles = new Vector3(0, 15, 0) * lookTiltAmount;
        lookTilts[1].localEulerAngles = new Vector3(0, -15, 0) * lookTiltAmount;
        lookTilts[2].localEulerAngles = new Vector3(15, 0, 0) * lookTiltAmount;
        lookTilts[3].localEulerAngles = new Vector3(-15, 0, 0) * lookTiltAmount;

        ChangeWeapon(currentWeapon);
    }

    void Update()
    {
        state.pl = this;
        state = state.handelInput();

        float movementSlopeAngle = Mathf.Asin(cc.velocity.normalized.y) * Mathf.Rad2Deg;
        float maxSpeed = cc.isGrounded ? slopeSpeedMultiplier.Evaluate(movementSlopeAngle) : 1f;

        speedMod = Mathf.Lerp(speedMod, currentState == PlayerState.Sprinting ? sprintMod : 1f, Time.deltaTime * 6f);
        playerSpd = new Vector3(sidewaysSpeed,Inputs.inputDirSmoothed.y >= 0 ? forwardSpeed : backwardSpeed,1) * speedMod * maxSpeed;
        //playerSpd = new Vector3(playerSpd.x * maxSpeed, playerSpd.y,playerSpd.z * maxSpeed);




        if (currentState == PlayerState.Sliding)
            playerSpd = Vector3.zero;

        if (jumpDelay > 0f)
            jumpDelay -= Time.deltaTime;
        else
            jumpDelay = 0f;

        if (cc.isGrounded && !hasJumped) {
            extraVel.y = gravity * 0.25f;
            airTime = 0f;
        } else {
            if(airTime < 10f)
                airTime += Time.deltaTime;
            if (extraVel.y > maxFallSpeed) {
                extraVel.y += Time.deltaTime * (hasJumped ? gravity : gravity * 0.45f);
            }
        }
        

        Vector3 finalVel = extraVel;
        cc.Move((cc.transform.TransformDirection(new Vector3(Inputs.inputDirSmoothed.x * playerSpd.x, 0, Inputs.inputDirSmoothed.y * playerSpd.y)) + slideVector + finalVel) * Time.deltaTime);

        //Interact
        RaycastHit iHit;

        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out iHit, 3f)) {
            if (iHit.collider.GetComponent<WeaponPickup>() != null) {
                uiMan.currentHoverItem = iHit.collider.GetComponent<WeaponPickup>();
                switch (uiMan.currentHoverItem.myRarity) {
                    case WeaponPickup.Rarity.Common:
                        Color newBGcol_Common = WorldManager.rarity[0] * 0f;
                        uiMan.weaponNameBG.color = new Color(newBGcol_Common.r, newBGcol_Common.g, newBGcol_Common.b, uiMan.weaponNameBG.color.a);
                        uiMan.weaponStatsBG.color = new Color(newBGcol_Common.r * 0.5f, newBGcol_Common.g * 0.5f, newBGcol_Common.b * 0.5f, uiMan.weaponStatsBG.color.a);
                        break;
                    case WeaponPickup.Rarity.Uncommon:
                        Color newBGcol_Uncommon = WorldManager.rarity[1] * 0.01f;
                        uiMan.weaponNameBG.color = new Color(newBGcol_Uncommon.r, newBGcol_Uncommon.g, newBGcol_Uncommon.b, uiMan.weaponNameBG.color.a);
                        uiMan.weaponStatsBG.color = new Color(newBGcol_Uncommon.r * 0f, newBGcol_Uncommon.g * 0f, newBGcol_Uncommon.b * 0f, uiMan.weaponStatsBG.color.a);
                        break;
                    case WeaponPickup.Rarity.Rare:
                        Color newBGcol_Rare = WorldManager.rarity[2] * 0.02f;
                        uiMan.weaponNameBG.color = new Color(newBGcol_Rare.r, newBGcol_Rare.g, newBGcol_Rare.b, uiMan.weaponNameBG.color.a);
                        uiMan.weaponStatsBG.color = new Color(newBGcol_Rare.r * 0f, newBGcol_Rare.g * 0f, newBGcol_Rare.b * 0f, uiMan.weaponStatsBG.color.a);
                        break;
                    case WeaponPickup.Rarity.Epic:
                        Color newBGcol_Epic = WorldManager.rarity[3] * 0.02f;
                        uiMan.weaponNameBG.color = new Color(newBGcol_Epic.r, newBGcol_Epic.g, newBGcol_Epic.b, uiMan.weaponNameBG.color.a);
                        uiMan.weaponStatsBG.color = new Color(newBGcol_Epic.r * 0f, newBGcol_Epic.g * 0f, newBGcol_Epic.b * 0f, uiMan.weaponStatsBG.color.a);
                        break;
                    case WeaponPickup.Rarity.Legendary:
                        Color newBGcol_Legendary = WorldManager.rarity[4] * 0.025f;
                        uiMan.weaponNameBG.color = new Color(newBGcol_Legendary.r, newBGcol_Legendary.g, newBGcol_Legendary.b, uiMan.weaponNameBG.color.a);
                        uiMan.weaponStatsBG.color = new Color(newBGcol_Legendary.r * 0f, newBGcol_Legendary.g * 0f, newBGcol_Legendary.b * 0f, uiMan.weaponStatsBG.color.a);
                        break;
                }


            }

            if (Inputs.interactPressed && interactDelay <= 0) {
                interactDelay = 0.25f;
            }
        }

        if (interactDelay > 0)
            interactDelay -= Time.deltaTime;
        

        if(lastAttackDelay >= maxAltSwingTime) {
            isAltSwing = false;
        } else if(!isReleasingAttack) {
            lastAttackDelay += Time.deltaTime * (isChargingAttack ? 0f : 1f);
        }

        if (attackState) {
            //Attacking
            if ((Inputs.attackHeld || Inputs.attackPressed) && atkSubstate <= 1 && !needsToReleaseAttackBlock) {
                isChargingAttack = true;
                atkSubstate = 2;
            }

            if(atkSubstate == 2) {
                //needsToAltSwing = false;
            }

            if ((!Inputs.attackHeld || Inputs.attackReleased) && isChargingAttack && !needsToReleaseAttackBlock) {
                //Debug.Log("REL");
                if (isReleasingAttack && lastAttackDelay <= maxAltSwingTime) {
                    needsToAltSwing = true;
                }

                isReleasingAttack = true;
            }
            //Debug.Log(airTime);

            if (isOverhead && airTime >= 0.6f) {
                RaycastHit groundHit;
                if(Physics.Raycast(transform.position,Vector3.down, out groundHit, 1f) && !isReleasingAttack && attackChargeTime >= minChargeTime) {
                    //needsToAltSwing = false; //Needs to slam
                    isReleasingAttack = true;
                    needsToSlam = true;
                }
            }

            //Magic
            if ((Inputs.blockHeld || Inputs.blockPressed) && atkSubstate <= 1 && !needsToReleaseAttackBlock) {
                isChargingMagic = true;
                atkSubstate = 3;
            }

            if ((!Inputs.blockHeld || Inputs.blockReleased) && isChargingMagic && !needsToReleaseAttackBlock) {
                //Debug.Log("REL");
                isReleasingMagic = true;
            }

            if (!hasAltSwing) {
                isAltSwing = false;
            }
            if(airTime > 0.05f) {
                //isAltSwing = false;
            }

            //Canceling
            if (isChargingAttack && Inputs.attackHeld && !isReleasingAttack) {
                if (Inputs.blockPressed) {
                    ResetAttackBlockParams();
                    needsToReleaseAttackBlock = true;
                }
            } else if(isChargingMagic && Inputs.blockHeld && !isReleasingMagic) {
                if (Inputs.attackPressed) {
                    ResetAttackBlockParams();
                    needsToReleaseAttackBlock = true;
                }
            }
            if (needsToReleaseAttackBlock)
                atkSubstate = 0;

            if (!Inputs.attackHeld && !Inputs.blockHeld)
                needsToReleaseAttackBlock = false;

            if (attackReleaseTime == 0 && !isReleasingAttack)
                isOverhead = airTime > 0.05f;
            if (!isOverhead)
                needsToSlam = false;

            if (isChargingAttack) {
                currentState = PlayerState.Attacking;
                attackChargeTime += Time.deltaTime;
                if (attackReleaseTime == 0) {
                    AttackChargeUpdate();
                }
                if (attackChargeTime >= minChargeTime) {
                    if (isReleasingAttack) {
                        attackReleaseTime += Time.deltaTime;
                        currentReleaseActionTime += Time.deltaTime;
                        if(isAltSwing)
                            ChangeArmsAnimationState("AttackReleaseAlt", 0.02f, 0);//AttackRelease
                        else
                            ChangeArmsAnimationState(isOverhead ? "AttackReleaseOverhead" : "AttackRelease", 0.02f, 0);//AttackRelease


                        if (releaseAction && currentReleaseActionTime >= releaseActionTime) {
                            
                            //Attack Stuff ========================================================================================================================

                            RaycastHit[] allHits = Physics.SphereCastAll(cam.transform.position, 0.5f, cam.transform.TransformDirection(Vector3.forward), 5f);

                            foreach (RaycastHit hit in allHits) {
                                //Debug.Log(hit.collider.gameObject.name);
                                hit.collider.SendMessage("OnAttackHit", SendMessageOptions.DontRequireReceiver);
                            }

                            if(currentWeapon == Holding.Shortsword) {
                                if (needsToSlam) {
                                    //SLAM ATTACK
                                    RaycastHit[] allSlamHits = Physics.SphereCastAll(cam.transform.position, 3.65f, cam.transform.TransformDirection(Vector3.forward), 3f);

                                    foreach (RaycastHit hitSlam in allSlamHits) {
                                        //Debug.Log(hit.collider.gameObject.name);
                                        hitSlam.collider.SendMessage("OnAttackHit", SendMessageOptions.DontRequireReceiver);
                                    }
                                    CamShake.instance.Shake(0.202f,0.2f);
                                }
                            }
                            if (currentWeapon == Holding.Broadsword) {

                            }
                            if (currentWeapon == Holding.Shield) {

                            }
                            if (currentWeapon == Holding.Bow) {
                                GameObject loadedArrow = Resources.Load("Arrow_Projectile") as GameObject;
                                GameObject newArrow = Instantiate(loadedArrow, displayArrow.position, displayArrow.rotation);
                                Vector3 arrowVel = ((cam.transform.position + cam.transform.TransformDirection(Vector3.forward * 20f)) - displayArrow.position).normalized * 47f;
                                newArrow.GetComponent<ArrowProjectile>().SetArrowVel(arrowVel);
                                //Debug.Log("ArrowShoot");
                            }


                            releaseAction = false;
                            currentReleaseActionTime = 0f;
                        }

                        if (attackReleaseTime >= minReleaseTime) {
                            lastAttackDelay = 0f;
                            isChargingAttack = false;
                            isReleasingAttack = false;
                            attackChargeTime = 0;
                            attackReleaseTime = 0;
                            releaseAction = true;
                            atkSubstate = 1;
                            isAltSwing = !isAltSwing;
                            needsToAltSwing = false;
                        }
                    } else {
                        if(isAltSwing)
                            ChangeArmsAnimationState("AttackChargeAlt", 0.02f, 0);
                        else
                            ChangeArmsAnimationState(isOverhead ? "AttackChargeOverhead" : "AttackCharge", 0.02f, 0);
                    }
                } else {
                    if (isAltSwing)
                        ChangeArmsAnimationState("AttackChargeAlt", 0.02f, 0);
                    else
                        ChangeArmsAnimationState(isOverhead ? "AttackChargeOverhead" : "AttackCharge", 0.02f, 0);
                }
            } else if(isChargingMagic) {
                currentState = PlayerState.Blocking;
                magicChargeTime += Time.deltaTime;
                if (magicChargeTime >= minMagicChargeTime) {
                    if (isReleasingMagic) {
                        magicReleaseTime += Time.deltaTime;
                        currentReleaseMagicActionTime += Time.deltaTime;
                        //Magic Block Release
                        if(magicID != 0) {
                            ChangeArmsAnimationState("MagicRelease_" + magicID.ToString(), 0.02f, 1);
                        } else {
                            ChangeArmsAnimationState("BlockCharge", 0.075f, 0);
                        }

                        if (releaseMagicAction && currentReleaseMagicActionTime >= releaseMagicActionTime) {

                            //Magic Block Stuff ========================================================================================================================
                            if(magicID != 0) {
                                if(magicID == 1) {
                                    //Blink

                                }
                                if(magicID == 2) {
                                    Debug.Log("TK_Release");
                                    TKCooldown = 0.3f;
                                    //TK
                                    
                                }
                                if(magicID == 4) {
                                    //Rocks
                                    rockShootPos.position = cam.transform.position;
                                    rockShootPos.rotation = cam.transform.rotation;
                                    float rot = Random.Range(0.99f, 360f);
                                    for (int i = 0; i < 3; i++) {
                                        float spread = 0.065f;//0.05f
                                        Vector3 camFWDPos = rockShootPos.transform.forward + (rockShootPos.transform.right * spread);
                                        camFWDPos = Quaternion.AngleAxis(rot, rockShootPos.transform.forward) * camFWDPos;

                                        //Vector3 dir = camFWDPos + cam.transform.TransformDirection(new Vector3(Random.Range(-0.99f,0.99f), Random.Range(-0.99f, 0.99f), 0) * spread);
                                        //Vector3 dir = camFWDPos + cam.transform.TransformDirection(new Vector3(0, 0, 0) * spread);

                                        Vector3 dir = Quaternion.AngleAxis((240) * i, rockShootPos.transform.forward) * camFWDPos;


                                        //newRock.transform.localScale *= 0.5f;

                                        RaycastHit hit;
                                        if (Physics.Raycast(rockShootPos.transform.position, dir, out hit, 60)) {
                                            //Debug.LogError("RockHits");
                                            //GameObject G = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                            //G.transform.position = hit.point;
                                            //G.transform.localScale *= 0.1f;

                                            GameObject loadedRock = Resources.Load("RockProjectile") as GameObject;
                                            GameObject newRock = PlayerLocomotion.Instantiate(loadedRock, hit.point, Quaternion.LookRotation(dir));
                                            newRock.GetComponent<RockProj>().sendMSG = hit.transform.gameObject;
                                            newRock.transform.localScale *= Random.Range(0.82f, 0.99f) * 1.2f;
                                        }
                                    }
                                    PlayerGraphics.extraHandIntensity = 30f;
                                    CamShake.instance.Shake(0.15f, 0.5f);
                                }
                            }
                            if (currentWeapon == Holding.Shortsword) {

                            }
                            if (currentWeapon == Holding.Broadsword) {

                            }
                            if (currentWeapon == Holding.Shield) {

                            }
                            if (currentWeapon == Holding.Bow) {

                            }

                            releaseMagicAction = false;
                            currentReleaseMagicActionTime = 0f;
                        }

                        if (magicReleaseTime >= minMagicReleaseTime) {
                            isChargingMagic = false;
                            isReleasingMagic = false;
                            magicChargeTime = 0;
                            magicReleaseTime = 0;
                            releaseMagicAction = true;
                            atkSubstate = 1;
                            ChangeArmsAnimationState("LeftHandEmpty", 0.2f, 1);
                        }
                    } else {
                        if(magicID != 0) {
                            ChangeArmsAnimationState("MagicCharge_" + magicID.ToString(), 0.2f, 1);
                        } else {
                            ChangeArmsAnimationState("BlockCharge", 0.075f, 0);
                        }
                    }
                } else {
                    if (magicID != 0) {
                        ChangeArmsAnimationState("MagicCharge_" + magicID.ToString(), 0.2f, 1);
                    } else {
                        ChangeArmsAnimationState("BlockCharge", 0.075f, 0);
                    }
                }

                if (magicID == 1) {
                    PlayerGraphics.searchingBlink = Inputs.blockHeld;
                    PlayerGraphics.blinkTargetPos = cam.transform.position + cam.transform.TransformDirection(Vector3.forward * 10f);
                    if (Inputs.blockHeld) {
                        PlayerGraphics.finderTargetPos = PlayerGraphics.blinkTargetPos;
                    } else {
                        PlayerGraphics.finderTargetPos = Vector3.zero;

                    }
                }
                if(magicID == 2 && TKCooldown <= 0) {
                    if (hasGrabbed) {
                        PlayerGraphics.finderTargetPos = currentGrabbed.transform.GetChild(0).position;
                        holdDist += TKpushDirLerp * Time.deltaTime * 250f;//50f
                        holdDist = Mathf.Clamp(holdDist, 2f, 20f);
                        TK_VFX.position = currentGrabbed.transform.GetChild(0).position;
                        Vector3 dest = cam.transform.position + (cam.transform.forward * holdDist) + grabOffset;

                        float camForce = Vector3.Distance(dest, cam.transform.position) * 40f;
                        float force = Vector3.Distance(dest, currentGrabbed.transform.position) * 30f * camForce;//10f
                        //Debug.Log(force);
                        force = Mathf.Clamp(force, 5f, 150f);


                        currentGrabbed.GetComponent<Rigidbody>().AddForce((dest - currentGrabbed.transform.position) * force);
                        currentGrabbed.GetComponent<Rigidbody>().drag = 10f;
                    } else {
                        RaycastHit grabHit;
                        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out grabHit, 20)) {
                            if (grabHit.transform.gameObject.GetComponent<Rigidbody>() && grabHit.transform.childCount > 0) {
                                if(grabHit.transform.GetChild(0).gameObject.name == "TK_Pivot") {
                                    currentGrabbed = grabHit.transform.gameObject;
                                    currentGrabbed.GetComponent<Rigidbody>().useGravity = false;
                                    startDrag = currentGrabbed.GetComponent<Rigidbody>().drag;
                                    PlayerGraphics.grabbedVFX = true;
                                    PlayerGraphics.finderLerpPos = (cam.transform.position + grabHit.point) / 2;
                                    holdDist = Vector3.Distance(cam.transform.position, grabHit.point);
                                    hasGrabbed = true;
                                    grabOffset = currentGrabbed.transform.position - grabHit.point;
                                }
                            }
                        }
                    }
                }
            }
            

            


            //ChangeArmsAnimationState("AttackRelease", 0.02f, 0);
        } else {
            ResetAttackBlockParams();
        }

        if (TKCooldown > 0) {
            TKCooldown -= Time.deltaTime;

            if (currentGrabbed != null) {
                currentGrabbed.GetComponent<Rigidbody>().useGravity = true;
                currentGrabbed.GetComponent<Rigidbody>().drag = startDrag;
                currentGrabbed = null;
            }

            PlayerGraphics.grabbedVFX = false;
            PlayerGraphics.finderTargetPos = Vector3.zero;

            forceLetGo = false;

            //Debug.LogError("LG");
            hasGrabbed = false;
        }


        CamShake.instance.SetTargetFOV(BowString.bowDrawn ? 80f : 75f);

        if (Keyboard.current.hKey.wasPressedThisFrame)
            CamShake.instance.Shake(2,1);
        if (Keyboard.current.jKey.wasPressedThisFrame)
            CamShake.instance.SetTargetFOV(90);
        if (Keyboard.current.kKey.wasPressedThisFrame)
            CamShake.instance.SetTargetFOV(75);
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            ChangeWeapon(Holding.Shortsword);
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            ChangeWeapon(Holding.Bow);
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            ChangeWeapon(Holding.Shield);
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            ChangeWeapon(Holding.Broadsword);

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            ChangeMagic(1);
        if (Keyboard.current.digit9Key.wasPressedThisFrame)
            ChangeMagic(2);
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
            ChangeMagic(3);
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
            ChangeMagic(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame)
            ChangeMagic(5);




        if (currentState == PlayerState.Blocking)
            ChangeArmsAnimationState("LowerShoulder", 0.2f, 2);
        else
            ChangeArmsAnimationState("ShoulderEmpty", 0.2f, 2);

        if (weaponSwitchTime > 0)
            weaponSwitchTime -= Time.deltaTime * 2.5f;
        else if (weaponSwitchTime <= 0) {
            weaponSwitchTime = 0;
            hasSwitchedWeapon = false;
        }
        armsConceal.localEulerAngles = new Vector3(weaponSwitchCurve.Evaluate(weaponSwitchTime) * 50, 0, weaponSwitchCurve.Evaluate(weaponSwitchTime) * -25);
        armsConceal.localPosition = new Vector3(0, weaponSwitchCurve.Evaluate(weaponSwitchTime) * -1, 0);
        if(weaponSwitchTime <= 0.5f) {
            currentWeapon = newWeapon;

            shortswords.gameObject.SetActive(currentWeapon == Holding.Shortsword || currentWeapon == Holding.Shield);
            broadswords.gameObject.SetActive(currentWeapon == Holding.Broadsword);
            shields.gameObject.SetActive(currentWeapon == Holding.Shield);
            bows.gameObject.SetActive(currentWeapon == Holding.Bow);
            
            oneHanded_Pos.gameObject.SetActive(currentWeapon == Holding.Shortsword || currentWeapon == Holding.Shield);
            twoHanded_Pos.gameObject.SetActive(currentWeapon == Holding.Broadsword);
            shield_Pos.gameObject.SetActive(currentWeapon == Holding.Shield);
            bow_Pos.gameObject.SetActive(currentWeapon == Holding.Bow);
            
            magic.gameObject.SetActive(magicID != 0);
            hasSwitchedWeapon = true;
        }



        //slideVector = new Vector3(0, -9.8f, 0);

        if (currentState == PlayerState.Sliding)
            cc.height = Mathf.Lerp(cc.height, 0.51f, Time.deltaTime * 9f);
        else {
            cc.height = Mathf.Lerp(cc.height, 2f, Time.deltaTime * 9f);
            if (cc.height < 1.99f)
                cc.Move(new Vector3(0, 10.2f, 0.01f) * Time.deltaTime);
        }

            if (keyboardOrGamepad) {
            //GAMEPAD
            float deadzone = 0.1f;
            if (Mathf.Abs(Gamepad.current.leftStick.ReadValue().x) > deadzone || Mathf.Abs(Gamepad.current.leftStick.ReadValue().y) > deadzone) {
                Inputs.inputDirRaw = Gamepad.current.leftStick.ReadValue();
            } else {
                Inputs.inputDirRaw = Vector2.zero;
            }
            
            Inputs.inputDirSmoothed.x = Mathf.Lerp(Inputs.inputDirSmoothed.x, Inputs.inputDirRaw.normalized.x, Time.deltaTime * 15f);
            Inputs.inputDirSmoothed.y = Mathf.Lerp(Inputs.inputDirSmoothed.y, Inputs.inputDirRaw.normalized.y, Time.deltaTime * 15f);

            Inputs.forwardPressed = Inputs.inputDirRaw.y > deadzone && lastFrameInputDir.y <= deadzone;
            Inputs.forwardReleased = Inputs.inputDirRaw.y == 0 && lastFrameInputDir.y > deadzone;
            Inputs.forwardHeld = Inputs.inputDirRaw.y > deadzone;

            Inputs.backPressed = Inputs.inputDirRaw.y < -deadzone && lastFrameInputDir.y >= -deadzone;
            Inputs.backReleased = Inputs.inputDirRaw.y >= -deadzone && lastFrameInputDir.y < -deadzone;
            Inputs.backHeld = Inputs.inputDirRaw.y < -deadzone;

            Inputs.leftPressed = Inputs.inputDirRaw.x < -deadzone && lastFrameInputDir.x >= -deadzone;
            Inputs.leftReleased = Inputs.inputDirRaw.x >= -deadzone && lastFrameInputDir.x < -deadzone;
            Inputs.leftHeld = Inputs.inputDirRaw.x < -deadzone;

            Inputs.rightPressed = Inputs.inputDirRaw.x > deadzone && lastFrameInputDir.x <= deadzone;
            Inputs.rightReleased = Inputs.inputDirRaw.x == 0 && lastFrameInputDir.x > deadzone;
            Inputs.rightHeld = Inputs.inputDirRaw.x > deadzone;

            if (Mathf.Abs(Gamepad.current.rightStick.ReadValue().x) > deadzone || Mathf.Abs(Gamepad.current.rightStick.ReadValue().y) > deadzone) {
                Inputs.look = Gamepad.current.rightStick.ReadValue() * 2;
            } else
                Inputs.look = Vector2.zero;

            Inputs.lookSmoothed.x = Mathf.Lerp(Inputs.lookSmoothed.x, Inputs.look.x, Time.deltaTime * 10);
            Inputs.lookSmoothed.y = Mathf.Lerp(Inputs.lookSmoothed.y, Inputs.look.y, Time.deltaTime * 10);

            Inputs.sprintPressed = Gamepad.current.leftStickButton.wasPressedThisFrame;
            Inputs.sprintHeld = Gamepad.current.leftStickButton.ReadValue() == 1;
            Inputs.sprintReleased = Gamepad.current.leftStickButton.wasReleasedThisFrame;

            Inputs.jumpPressed = Gamepad.current.buttonSouth.wasPressedThisFrame;
            Inputs.jumpReleased = Gamepad.current.buttonSouth.wasReleasedThisFrame;
            Inputs.jumpHeld = Gamepad.current.buttonSouth.ReadValue() == 1;


            Inputs.crouchPressed = Gamepad.current.buttonEast.wasPressedThisFrame;
            Inputs.crouchReleased = Gamepad.current.buttonEast.wasReleasedThisFrame;
            Inputs.crouchHeld = Gamepad.current.buttonEast.ReadValue() == 1;


            Inputs.slidePressed = Gamepad.current.buttonEast.wasPressedThisFrame;
            Inputs.slideReleased = Gamepad.current.buttonEast.wasReleasedThisFrame;
            Inputs.slideHeld = Gamepad.current.buttonEast.ReadValue() == 1;


            Inputs.attackPressed = Gamepad.current.rightTrigger.ReadValue() > 0.1f && lastFrame_RTrigger <= 0.1f;
            Inputs.attackReleased = Gamepad.current.rightTrigger.ReadValue() <= 0.1f && lastFrame_RTrigger > 0.1f;
            Inputs.attackHeld = Gamepad.current.rightTrigger.ReadValue() > 0.1f;


            Inputs.blockPressed = Gamepad.current.leftTrigger.ReadValue() > 0.1f && lastFrame_LTrigger <= 0.1f;
            Inputs.blockReleased = Gamepad.current.leftTrigger.ReadValue() <= 0.1f && lastFrame_LTrigger > 0.1f;
            Inputs.blockHeld = Gamepad.current.leftTrigger.ReadValue() > 0.1f;


            Inputs.kickPressed = Gamepad.current.buttonWest.wasPressedThisFrame;
            Inputs.kickReleased = Gamepad.current.buttonWest.wasReleasedThisFrame;
            Inputs.kickHeld = Gamepad.current.buttonWest.ReadValue() == 1;


            Inputs.interactPressed = Gamepad.current.buttonNorth.wasPressedThisFrame;
            Inputs.interactReleased = Gamepad.current.buttonNorth.wasReleasedThisFrame;
            Inputs.interactHeld = Gamepad.current.buttonNorth.ReadValue() == 1;


            Inputs.pausePressed = Gamepad.current.startButton.wasPressedThisFrame;
            Inputs.pauseReleased = Gamepad.current.startButton.wasReleasedThisFrame;
            Inputs.pauseHeld = Gamepad.current.startButton.ReadValue() == 1;

            Inputs.weaponWheelPressed = Gamepad.current.leftShoulder.wasPressedThisFrame;
            Inputs.weaponWheelReleased = Gamepad.current.leftShoulder.wasReleasedThisFrame;
            Inputs.weaponWheelHeld = Gamepad.current.leftShoulder.ReadValue() == 1;
            
            Inputs.weaponScrollUp = Gamepad.current.leftShoulder.wasPressedThisFrame;
            //Inputs.weaponScrollDown = Mouse.current.scroll.ReadValue().y < 0;

            Inputs.debug_1_Pressed = Gamepad.current.selectButton.wasPressedThisFrame;
            //Inputs.debug_1_Pressed = Gamepad.current.;
            
        }
        else {
            //KEYBOARD
            Inputs.inputDirRaw = new Vector2(
            (Keyboard.current.aKey.ReadValue() == 1 ? -1 : 0) + (Keyboard.current.dKey.ReadValue() == 1 ? 1 : 0),
            (Keyboard.current.sKey.ReadValue() == 1 ? -1 : 0) + (Keyboard.current.wKey.ReadValue() == 1 ? 1 : 0));
            Inputs.inputDirSmoothed.x = Mathf.Lerp(Inputs.inputDirSmoothed.x, Inputs.inputDirRaw.normalized.x, Time.deltaTime * 15);
            Inputs.inputDirSmoothed.y = Mathf.Lerp(Inputs.inputDirSmoothed.y, Inputs.inputDirRaw.normalized.y, Time.deltaTime * 15);

            Inputs.forwardPressed = Keyboard.current.wKey.wasPressedThisFrame;
            Inputs.forwardReleased = Keyboard.current.wKey.wasReleasedThisFrame;
            Inputs.forwardHeld = Keyboard.current.wKey.ReadValue() == 1;

            Inputs.backPressed = Keyboard.current.sKey.wasPressedThisFrame;
            Inputs.backReleased = Keyboard.current.sKey.wasReleasedThisFrame;
            Inputs.backHeld = Keyboard.current.sKey.ReadValue() == 1;

            Inputs.leftPressed = Keyboard.current.aKey.wasPressedThisFrame;
            Inputs.leftReleased = Keyboard.current.aKey.wasReleasedThisFrame;
            Inputs.leftHeld = Keyboard.current.aKey.ReadValue() == 1;

            Inputs.rightPressed = Keyboard.current.dKey.wasPressedThisFrame;
            Inputs.rightReleased = Keyboard.current.dKey.wasReleasedThisFrame;
            Inputs.rightHeld = Keyboard.current.dKey.ReadValue() == 1;


            Inputs.look = Mouse.current.delta.ReadValue();
            Inputs.lookSmoothed.x = Mathf.Lerp(Inputs.lookSmoothed.x, Inputs.look.x, Time.deltaTime * 10);
            Inputs.lookSmoothed.y = Mathf.Lerp(Inputs.lookSmoothed.y, Inputs.look.y, Time.deltaTime * 10);

            Inputs.sprintPressed = Keyboard.current.leftShiftKey.wasPressedThisFrame;
            Inputs.sprintHeld = Keyboard.current.leftShiftKey.ReadValue() == 1;
            Inputs.sprintReleased = Keyboard.current.leftShiftKey.wasReleasedThisFrame;

            Inputs.jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
            Inputs.jumpReleased = Keyboard.current.spaceKey.wasReleasedThisFrame;
            Inputs.jumpHeld = Keyboard.current.spaceKey.ReadValue() == 1;


            Inputs.crouchPressed = Keyboard.current.cKey.wasPressedThisFrame;
            Inputs.crouchReleased = Keyboard.current.cKey.wasReleasedThisFrame;
            Inputs.crouchHeld = Keyboard.current.cKey.ReadValue() == 1;


            Inputs.slidePressed = Keyboard.current.cKey.wasPressedThisFrame;
            Inputs.slideReleased = Keyboard.current.cKey.wasReleasedThisFrame;
            Inputs.slideHeld = Keyboard.current.cKey.ReadValue() == 1;


            Inputs.attackPressed = Mouse.current.leftButton.wasPressedThisFrame;
            Inputs.attackReleased = Mouse.current.leftButton.wasReleasedThisFrame;
            Inputs.attackHeld = Mouse.current.leftButton.ReadValue() == 1;


            Inputs.blockPressed = Mouse.current.rightButton.wasPressedThisFrame;
            Inputs.blockReleased = Mouse.current.rightButton.wasReleasedThisFrame;
            Inputs.blockHeld = Mouse.current.rightButton.ReadValue() == 1;


            Inputs.kickPressed = Keyboard.current.fKey.wasPressedThisFrame;
            Inputs.kickReleased = Keyboard.current.fKey.wasReleasedThisFrame;
            Inputs.kickHeld = Keyboard.current.fKey.ReadValue() == 1;


            Inputs.interactPressed = Keyboard.current.eKey.wasPressedThisFrame;
            Inputs.interactReleased = Keyboard.current.eKey.wasReleasedThisFrame;
            Inputs.interactHeld = Keyboard.current.eKey.ReadValue() == 1;


            Inputs.pausePressed = Keyboard.current.cKey.wasPressedThisFrame;
            Inputs.pauseReleased = Keyboard.current.cKey.wasReleasedThisFrame;
            Inputs.pauseHeld = Keyboard.current.cKey.ReadValue() == 1;

            Inputs.weaponWheelPressed = Mouse.current.middleButton.wasPressedThisFrame;
            Inputs.weaponWheelReleased = Mouse.current.middleButton.wasReleasedThisFrame;
            Inputs.weaponWheelHeld = Mouse.current.middleButton.ReadValue() == 1;

            Inputs.weaponScrollUp = Mouse.current.scroll.ReadValue().y > 0;
            Inputs.weaponScrollDown = Mouse.current.scroll.ReadValue().y < 0;

            Inputs.debug_1_Pressed = Keyboard.current.digit1Key.wasPressedThisFrame;
        }
    }

    void ChangeWeapon(Holding newHolding) {

        weaponSwitchTime = 1;
        newWeapon = newHolding;
        //Swap over weapon details, speed, dmg, time, altswing...

        armsAnim.SetFloat("ChargeSpeed", chargeSpeed);
        armsAnim.SetFloat("ReleaseSpeed", releaseSpeed);
        handAnim.SetFloat("ChargeSpeed", chargeSpeed);
        handAnim.SetFloat("ReleaseSpeed", releaseSpeed);

        switch (newHolding) {
            case Holding.Shortsword:
                hasAltSwing = true;
                break;
            case Holding.Broadsword:
                hasAltSwing = false;
                break;
            case Holding.Shield:
                hasAltSwing = true;
                break;
            case Holding.Bow:
                hasAltSwing = false;
                break;
            default:
                break;
        }

    }

    void ChangeMagic(int newMagicID) {
        magicID = newMagicID;

        switch (newMagicID) {
            case 0:
                //No Magic
                minMagicChargeTime = 0.75f;
                minMagicReleaseTime = 0.55f;
                releaseMagicActionTime = 0.05f;
                break;
            case 1:
                //No Magic
                minMagicChargeTime = 0.75f;
                minMagicReleaseTime = 0.55f;
                releaseMagicActionTime = 0.05f;
                break;
            case 2:
                //No Magic
                minMagicChargeTime = 0.5f;
                minMagicReleaseTime = 0.05f;
                releaseMagicActionTime = 0.05f;
                break;
            case 3:
                //No Magic
                minMagicChargeTime = 0.75f;
                minMagicReleaseTime = 0.55f;
                releaseMagicActionTime = 0.05f;
                break;
            case 4:
                //No Magic
                minMagicChargeTime = 0.75f;
                minMagicReleaseTime = 0.55f;
                releaseMagicActionTime = 0.05f;
                break;
            case 5:
                //No Magic
                minMagicChargeTime = 0.75f;
                minMagicReleaseTime = 0.55f;
                releaseMagicActionTime = 0.05f;
                break;
        }
    }

    public void ChangeArmsAnimationState(string newState, float transitionTime, int layer) {
        string suffix = "_SS";
        switch (currentWeapon) {
            case Holding.Shortsword:
                suffix = "_SS";
                break;
            case Holding.Broadsword:
                suffix = "_BS";
                break;
            case Holding.Shield:
                suffix = "_S";
                break;
            case Holding.Bow:
                suffix = "_B";
                break;
            default:
                break;
        }
        //if (magicID != 0 && currentState == PlayerState.Blocking)
        //    suffix = "";

        transitionTime /= animSpeed;

        if (layer == 0) {
            if (currentArmsState == newState + suffix) return;

            armsAnim.CrossFadeInFixedTime(newState + suffix, transitionTime, 0);
            handAnim.CrossFadeInFixedTime(newState + suffix, transitionTime, 0);
            currentArmsState = newState + suffix;
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

        //Debug.Log(currentArmsState);
    }

    private void LateUpdate() {
        lastFrameInputDir = Inputs.inputDirRaw;
        lastFrameLook = Inputs.look;
        lastFrame_RTrigger = Inputs.attackHeld ? 1 : 0;
        lastFrame_LTrigger = Inputs.blockHeld ? 1 : 0;

        tiltLerp = Vector2.Lerp(tiltLerp, Inputs.inputDirSmoothed, Time.deltaTime * 8);
        tiltLR.localRotation = Quaternion.Lerp(tilts[0].localRotation, tilts[1].localRotation, (tiltLerp.x + 1) / 2);
        tiltFB.localRotation = Quaternion.Lerp(tilts[3].localRotation, tilts[2].localRotation, (tiltLerp.y + 1) / 2);
        armsTiltLerp = Vector2.Lerp(armsTiltLerp, Inputs.inputDirSmoothed, Time.deltaTime * 11);
        armsTiltLR.localRotation = Quaternion.Lerp(armsTilts[0].localRotation, armsTilts[1].localRotation, (armsTiltLerp.x + 1) / 2);
        armsTiltFB.localRotation = Quaternion.Lerp(armsTilts[3].localRotation, armsTilts[2].localRotation, (armsTiltLerp.y + 1) / 2);
        //lookTiltLerp = Vector2.Lerp(lookTiltLerp, Inputs.look / 10, Time.deltaTime * 2);
        lookTiltLerp = Vector2.Lerp(Vector2.ClampMagnitude(lookTiltLerp, 1), Inputs.look / 10, Time.deltaTime * 2);
        //lookTiltLerp = Vector2.ClampMagnitude(lookTiltLerp, 1);
        lookTiltLR.localRotation = Quaternion.Lerp(lookTilts[0].localRotation, lookTilts[1].localRotation, (lookTiltLerp.x + 1) / 2);
        lookTiltFB.localRotation = Quaternion.Lerp(lookTilts[3].localRotation, lookTilts[2].localRotation, (lookTiltLerp.y + 1) / 2);
        tiltHolder.transform.rotation = tiltFB.rotation;
        armsTiltHolder.transform.rotation = tiltHolder.rotation;

        if(lastPosInterval <= 0) {
            lastPlayerPos = transform.position;
            lastPosInterval = 0.51f;
        } else {
            lastPosInterval -= Time.deltaTime;
            playerVelDir = (transform.position - lastPlayerPos).normalized;
            slideDir.position = transform.position;
        }
    }

    public void TouchPad() {
        //Debug.LogError("Touchpad");
    }

    public void AttackChargeUpdate() {
        if (currentWeapon == Holding.Shortsword) {

        }
        if (currentWeapon == Holding.Broadsword) {

        }
        if (currentWeapon == Holding.Shield) {

        }
        if (currentWeapon == Holding.Bow) {
            BowString.bowDrawn = Inputs.attackHeld;
        }
    }

    public void ResetAttackBlockParams() {
        isChargingAttack = false;
        isReleasingAttack = false;
        attackChargeTime = 0;
        attackReleaseTime = 0;
        releaseAction = false;
        currentReleaseActionTime = 0f;

        isChargingMagic = false;
        isReleasingMagic = false;
        magicChargeTime = 0;
        magicReleaseTime = 0;
        releaseMagicAction = false;
        currentReleaseMagicActionTime = 0f;

        BowString.bowDrawn = false;

        atkSubstate = 0;

        isOverhead = false;
        needsToAltSwing = false;
        needsToSlam = false;

        TKCooldown = 0.3f;
        ChangeArmsAnimationState("LeftHandEmpty", 0.2f, 1);
    }
}

public class Inputs
{
    //Move
    public static Vector2 inputDirRaw;
    public static Vector2 inputDirSmoothed;

    public static bool forwardPressed;
    public static bool forwardReleased;
    public static bool forwardHeld;

    public static bool backPressed;
    public static bool backReleased;
    public static bool backHeld;

    public static bool leftPressed;
    public static bool leftReleased;
    public static bool leftHeld;

    public static bool rightPressed;
    public static bool rightReleased;
    public static bool rightHeld;

    //Look
    public static Vector2 look;
    public static Vector2 lookSmoothed;

    //Sprint
    public static bool sprintPressed;
    public static bool sprintReleased;
    public static bool sprintHeld;

    //Jump (Up Motion)
    public static bool jumpPressed;
    public static bool jumpReleased;
    public static bool jumpHeld;

    //Crouch (Down Motion)
    public static bool crouchPressed;
    public static bool crouchReleased;
    public static bool crouchHeld;

    //Slide & Dodoge
    public static bool slidePressed;
    public static bool slideReleased;
    public static bool slideHeld;

    //Attack
    public static bool attackPressed;
    public static bool attackReleased;
    public static bool attackHeld;

    //Block & Magic
    public static bool blockPressed;
    public static bool blockReleased;
    public static bool blockHeld;

    //Kick
    public static bool kickPressed;
    public static bool kickReleased;
    public static bool kickHeld;

    //Interact
    public static bool interactPressed;
    public static bool interactReleased;
    public static bool interactHeld;

    //Pause
    public static bool pausePressed;
    public static bool pauseReleased;
    public static bool pauseHeld;
    
    //Weapon Wheel
    public static bool weaponWheelPressed;
    public static bool weaponWheelReleased;
    public static bool weaponWheelHeld;

    //Weapon Scroll
    public static bool weaponScrollUp;
    public static bool weaponScrollDown;

    //DEBUG KEYS--------
    public static bool debug_1_Pressed;
}


