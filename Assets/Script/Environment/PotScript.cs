using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotScript : MonoBehaviour
{
    public bool physicsBasedOnDistance = false;
    public float kinematicDistance;

    public bool randomColour = true;
    Color chosenColour;

    public GameObject basePot;
    public GameObject shatteredPot;
    public Transform shatteredPotTransform;

    public bool randomOrbID = true;
    public int orbID = 1; //1 Health,2 Speed,3 Mana
    public GameObject potDust;
    public Transform orbSpawnPoint;
    public Light potLight;
    public AnimationCurve lightBrightnessOverTime;
    bool broken = false;
    bool enabledShatteredPot = false;
    int behaviourID;
    Vector3 playerPos;

    public bool projectileBreak = false;
    bool hasProjectileBreak = false;

    float t = 0;
    float projectile_t = 0;

    void Start()
    {
        //_MainColor
        chosenColour = Color.HSVToRGB(Random.Range(0, 0.99f), .3f, .3f);
        basePot.GetComponent<Renderer>().material.SetColor("_MainColor", chosenColour);
        
        if(randomOrbID)
            orbID = Random.Range(1, 4);
        if (physicsBasedOnDistance)
            transform.GetChild(1).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    void Update()
    {
        if (physicsBasedOnDistance)
            if (Vector3.Distance(transform.position, Camera.main.transform.position) < kinematicDistance)
                transform.GetChild(1).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if (broken) {
            t += Time.deltaTime;

            if (!enabledShatteredPot) {
                if (behaviourID != 2) {
                    potLight.gameObject.SetActive(true);
                    potLight.transform.parent = null;
                    orbSpawnPoint.parent = null;
                }
                basePot.SetActive(false);
                shatteredPot.SetActive(true);
                

                    if (orbID == 1) //health
                    potLight.color = new Color(1, 0, 0.2306142f);
                if (orbID == 2) //speed
                    potLight.color = new Color(1, 0.8610413f, 0);
                if (orbID == 3) //mana
                    potLight.color = new Color(0.1320786f, 0, 1);
                if(behaviourID != 2)
                    Instantiate(potDust, orbSpawnPoint.position, orbSpawnPoint.rotation);

                for (int i = 0; i < shatteredPot.transform.childCount; i++) {
                    shatteredPot.transform.GetChild(i).GetComponent<Renderer>().material.SetColor("_MainColor", chosenColour);
                    if (behaviourID == 0) {
                        shatteredPot.transform.GetChild(i).gameObject.layer = 10;
                        shatteredPot.transform.GetChild(i).GetComponent<Rigidbody>().AddForce((shatteredPot.transform.GetChild(i).position - playerPos) * 130);
                    }
                    if (behaviourID == 3) {
                        shatteredPot.transform.GetChild(i).gameObject.layer = 10;
                        shatteredPot.transform.GetChild(i).GetComponent<Rigidbody>().AddForce((shatteredPot.transform.GetChild(i).position - playerPos) * 100);
                        shatteredPot.transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(200, orbSpawnPoint.position, 1);
                    }
                    if(behaviourID == 2) {
                        //shatteredPot.transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = true;
                        shatteredPot.transform.GetChild(i).gameObject.layer = 10;
                        Destroy(shatteredPot.transform.GetChild(i).GetComponent<Rigidbody>());
                        //shatteredPot.transform.GetChild(i).GetComponent<MeshCollider>().enabled = false;---------------------------
                        //shatteredPot.transform.GetChild(i).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                    if (behaviourID == 1) {
                        shatteredPot.transform.GetChild(i).gameObject.layer = 10;
                        shatteredPot.transform.GetChild(i).GetComponent<Rigidbody>().AddForce((shatteredPot.transform.GetChild(i).position - playerPos) * 170);
                    }
                }
                if(behaviourID == 2) {
                    gameObject.AddComponent<Rigidbody>();
                    //shatteredPot.GetComponent<Rigidbody>().velocity = (playerPos - shatteredPot.transform.position).normalized * -10;
                    gameObject.GetComponent<Rigidbody>().velocity = GameObject.Find("_MyPlayer").transform.Find("Player").TransformDirection(Vector3.forward * 25) + (Vector3.up * 6);
                    //GetComponent<SphereCollider>().enabled = true;
                }

                if(orbID == 1 && behaviourID != 2)
                    Instantiate(Resources.Load("HealthOrbs") as GameObject, orbSpawnPoint.position, orbSpawnPoint.rotation);
                if (orbID == 2 && behaviourID != 2)
                    Instantiate(Resources.Load("SpeedOrbs") as GameObject, orbSpawnPoint.position, orbSpawnPoint.rotation);
                if (orbID == 3 && behaviourID != 2)
                    Instantiate(Resources.Load("ManaOrbs") as GameObject, orbSpawnPoint.position, orbSpawnPoint.rotation);


                enabledShatteredPot = true;
            }

            if (projectileBreak) {
                projectile_t += Time.deltaTime;

                if (!hasProjectileBreak) {

                    potLight.gameObject.SetActive(true);
                    potLight.transform.parent = null;
                    orbSpawnPoint.parent = null;

                    Instantiate(potDust, orbSpawnPoint.position, orbSpawnPoint.rotation);

                    for (int i = 0; i < shatteredPot.transform.childCount; i++) {
                        //give rigi
                        shatteredPot.transform.GetChild(i).GetComponent<MeshCollider>().enabled = true;
                        shatteredPot.transform.GetChild(i).gameObject.AddComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
                        shatteredPot.transform.GetChild(i).GetComponent<Rigidbody>().AddExplosionForce(350, orbSpawnPoint.position, 1);
                    }

                    if (orbID == 1)
                        Instantiate(Resources.Load("HealthOrbs") as GameObject, orbSpawnPoint.position, orbSpawnPoint.rotation);
                    if (orbID == 2)
                        Instantiate(Resources.Load("SpeedOrbs") as GameObject, orbSpawnPoint.position, orbSpawnPoint.rotation);
                    if (orbID == 3)
                        Instantiate(Resources.Load("ManaOrbs") as GameObject, orbSpawnPoint.position, orbSpawnPoint.rotation);

                    Destroy(GetComponent<Rigidbody>());

                    hasProjectileBreak = true;
                }


            }

            if(t > 0.5f && behaviourID != 2) {
                for (int i = 0; i < shatteredPot.transform.childCount; i++) {
                    shatteredPot.transform.GetChild(i).localScale = Vector3.Lerp(shatteredPot.transform.GetChild(i).localScale, new Vector3(0.01f, 0.01f, 0.01f), Time.deltaTime * 8);
                }
            }
            else if (projectile_t > 0.5f) {
                for (int i = 0; i < shatteredPot.transform.childCount; i++) {
                    shatteredPot.transform.GetChild(i).localScale = Vector3.Lerp(shatteredPot.transform.GetChild(i).localScale, new Vector3(0.01f, 0.01f, 0.01f), Time.deltaTime * 8);
                }
            }

            if(projectileBreak)
                potLight.intensity = lightBrightnessOverTime.Evaluate(projectile_t * 3) * 400000;
            else
                potLight.intensity = lightBrightnessOverTime.Evaluate(t * 3) * 400000;
            //Debug.LogError(potLight.intensity);
        }
    }

    public void BreakPot(int newBehaviourID, Vector3 newPlayerPos) //0 stationary kick, 1 slide into, 2 slide kick, 3 melee
    {
        shatteredPot.transform.position = shatteredPotTransform.transform.position;
        shatteredPot.transform.rotation = shatteredPotTransform.transform.rotation;
        behaviourID = newBehaviourID;
        playerPos = newPlayerPos;
        broken = true;

        if (behaviourID == 1)
            basePot.layer = 10;
        if (behaviourID == 2)
            CombatMovement.isKicking = false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if(t > 0.2f) {
            //Hit!
            
            projectileBreak = true;
        }

        Debug.LogError(70);

        if(basePot.GetComponent<Rigidbody>().velocity.magnitude > 10) {
            BlockingState.forceLetGo = true;
            projectileBreak = true;
        }
    }
}
