using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SlimeScript : MonoBehaviour
{
    //public Color slimeColour;
    Color randomSlimeColour;
    public Renderer rend1;
    public Renderer rend2;
    public Renderer eyesRend;

    public Transform slimeGraphics;

    public Rigidbody rb;
    Vector3 target;

    float h = 2;
    float gravity = -18;

    float landTime = 0;
    float jumpTime = 0;
    bool isGrounded = true;

    bool anticipateJump = false;
    bool landed = false;
    bool hasJumped = false;

    public AnimationCurve anticipateJumpScale;
    public AnimationCurve landedScale;
    public AnimationCurve jumpScale;
    public AnimationCurve jumpTilt;
    int scaleID = 0; //1 = anticipate, 2 = jump, 3 = land
    float currentScaleY;
    Vector3 startScale;
    Vector3 startLocalEuler;
    float t = 0;
    float tiltX = 0;

    public Transform graphicsHolder;
    Vector3 targetEuler;

    public Color slimeColour;

    float blinkInterval = 0f;
    float currentBlinkTime = 0;

    public GameObject splatVFX;


    float animSpeed = 1;
    float maxLandTime = 0.75f;
    float jumpHeight = 1.5f;
    float jumpDist = 2f;
    float slimeScale = 1;

    void Start()
    {
        animSpeed = Random.Range(0.91f,1.2f);
        maxLandTime = Random.Range(0.67f,1.02f);
        jumpHeight = Random.Range(0.97f,1.92f);
        jumpDist = Random.Range(1.47f,2.92f);
        slimeScale = Random.Range(0.65f,1.1f);

        transform.localScale *= slimeScale;
        blinkInterval = Random.Range(0.1f, 1.43f);

        startScale = graphicsHolder.transform.localScale;
        startLocalEuler = graphicsHolder.transform.localEulerAngles;
        //_MainColor
        //_RimLightColor

        randomSlimeColour = Color.HSVToRGB(Random.Range(0.0f, 0.99f), 0.7f, 0.57f);//0.47f
        slimeColour = randomSlimeColour;
        rend1.material.SetColor("_MainColor", randomSlimeColour);
        rend1.material.SetColor("_RimLightColor", randomSlimeColour);
        rend2.material.SetColor("_MainColor", randomSlimeColour);
        rend2.material.SetColor("_RimLightColor", randomSlimeColour);
    }

    void Update()
    {
        tiltX = scaleID == 2 ? (jumpTilt.Evaluate(t) * 0.45f) : (Mathf.Lerp(tiltX, 0, Time.deltaTime * 19));
        graphicsHolder.localEulerAngles = new Vector3(tiltX, startLocalEuler.y, startLocalEuler.z);

        if (scaleID == 1) {
            graphicsHolder.localScale = new Vector3(startScale.x, startScale.y * anticipateJumpScale.Evaluate(t), startScale.z);
            //graphicsHolder.localEulerAngles = Vector3.Lerp(graphicsHolder.localEulerAngles, startLocalEuler, Time.deltaTime * 5);
        } else if(scaleID == 2) {
            graphicsHolder.localScale = new Vector3(startScale.x, startScale.y * jumpScale.Evaluate(t), startScale.z);
            //graphicsHolder.localEulerAngles = new Vector3(jumpTilt.Evaluate(t) * 0.5f, startLocalEuler.y, startLocalEuler.z);
        } else if(scaleID == 3) {
            graphicsHolder.localScale = new Vector3(startScale.x, startScale.y * landedScale.Evaluate(t), startScale.z);
            //graphicsHolder.localEulerAngles = new Vector3(startLocalEuler.x, startLocalEuler.y, startLocalEuler.z);
            //graphicsHolder.localEulerAngles = Vector3.Lerp(graphicsHolder.localEulerAngles, startLocalEuler, Time.deltaTime * 5);
        }
        //graphicsHolder.localEulerAngles = Vector3.Lerp(graphicsHolder.localEulerAngles, targetEuler, Time.deltaTime * 5);

        if (t < 10)
            t += Time.deltaTime * animSpeed;

        //if (Input.GetKeyDown(KeyCode.N)) {

        //    Jump();
        //}

        RaycastHit hit;
        if(Physics.Raycast(rb.transform.position, rb.transform.TransformDirection(Vector3.down), out hit, 0.95f) && jumpTime <= 0) {
            isGrounded = true;
            if (!landed) {
                landed = true;
                t = 0;
                hasJumped = false;
            }
        } else {
            isGrounded = false;
        }

        Vector3 dirToMovePosition = (Camera.main.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToMovePosition);
        float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, transform.TransformDirection(Vector3.up));

        if (isGrounded) {
            landTime += Time.deltaTime;
            if(landTime > (maxLandTime - 0.25f) && !anticipateJump) {
                anticipateJump = true;
                t = 0;
            }
            if(landTime >= maxLandTime) {
                isGrounded = false;
                jumpTime = 0.75f;
                Jump();
            }
            

            //rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, Quaternion.LookRotation(new Vector3(Camera.main.transform.position.x, rb.transform.position.y, Camera.main.transform.position.z)), Time.deltaTime * 2);
            if(angleToDir > 0) {
                transform.Rotate(new Vector3(0,100,0) * Time.deltaTime);
            } else {
                transform.Rotate(new Vector3(0, -100, 0) * Time.deltaTime);
            }


        } else {
            landTime = 0;
            if(jumpTime > 0) {
                jumpTime -= Time.deltaTime;
            }
            anticipateJump = false;
            landed = false;
        }

        if (anticipateJump)
            scaleID = 1;
        else if (hasJumped && !landed)
            scaleID = 2;
        else if (landed)
            scaleID = 3;


        if(blinkInterval >= 1.5f) {
            currentBlinkTime += Time.deltaTime;
            if(currentBlinkTime > 0.1f) {
                blinkInterval = 0;
                currentBlinkTime = 0;
            } else {
                eyesRend.enabled = false;
            }
        } else {
            blinkInterval += Time.deltaTime;
            eyesRend.enabled = true;
        }
    }

    private void FixedUpdate() {
        rb.velocity += new Vector3(0,-18,0) * Time.fixedDeltaTime;
    }

    void Jump() {
        target = rb.transform.position + rb.transform.TransformDirection(Vector3.forward) * jumpDist;
        h = jumpHeight;//1.5f
        rb.velocity = CalculateJumpVel();
        t = 0;
        hasJumped = true;
        landed = false;
    }

    Vector3 CalculateJumpVel() {
        float displacementY = target.y - rb.position.y;
        Vector3 displacementXZ = new Vector3(target.x - rb.position.x,0,target.z - rb.position.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity));

        return velocityXZ + velocityY;
    }

    public void Attacked(Vector3 hitPoint) {
        GameObject newVFX = Instantiate(splatVFX, hitPoint, transform.rotation);
        newVFX.GetComponent<VisualEffect>().SetVector4("Colour",slimeColour);
        newVFX.transform.parent = transform;
        newVFX.transform.localScale *= 2;
    }
}
