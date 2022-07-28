using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody rb;
    public CapsuleCollider col;

    public Vector3 vel;
    float lifetime = 25f;
    float startLifetime = 5f;

    private Collider hitCol;
    bool hasHit = false;

    float dropoff = 25f;

    public Transform pierce;

    float posDelay = 0.05f;
    Vector3 lastPos;

    void Start()
    {
        startLifetime = lifetime;
        lastPos = transform.position;
        //rb.velocity = transform.TransformDirection(Vector3.forward) * speed;
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime <= 0) {
            Destroy(gameObject);
        }

        if(hasHit && hitCol == null) {
            Destroy(gameObject);
        }

        if (hasHit)
            pierce.gameObject.SetActive(false);
        else
            pierce.Rotate(Vector3.forward * 630f * Time.deltaTime);
        pierce.localScale = Vector3.Lerp(pierce.localScale, Vector3.one * 2f, Time.deltaTime * 10f);

        if (posDelay >= 0) {
            posDelay -= Time.deltaTime;
            if(lifetime < startLifetime - 0.25f)
                transform.LookAt(Vector3.LerpUnclamped(lastPos, transform.position,2f));
        } else {
            lastPos = transform.position;
            posDelay = 0.05f;
        }
    }

    private void FixedUpdate() {
        if (!hasHit) {
            rb.velocity += Vector3.down * dropoff * Time.deltaTime;
        }
    }

    public void SetArrowVel(Vector3 newVel) {
        rb.velocity = newVel;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!hasHit) {
            Vector3 dustPos = transform.position + transform.TransformDirection(Vector3.back * 1f);
            rb.isKinematic = true;
            col.enabled = false;
            hitCol = collision.collider;
            hasHit = true;
            transform.position = collision.contacts[0].point;

            collision.gameObject.SendMessage("OnAttackHit", SendMessageOptions.DontRequireReceiver);


            GameObject loadedDust = Resources.Load("RockDust") as GameObject;
            Instantiate(loadedDust, dustPos, transform.rotation);
        }
        
    }
}
