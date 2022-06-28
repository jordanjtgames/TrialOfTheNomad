using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockProj : MonoBehaviour
{
    bool connected = false;
    public Renderer rockRend;
    public Transform smallRocks;
    public GameObject sendMSG;

    void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        rockRend.transform.localPosition = new Vector3(0, 0, -20);
        rockRend.transform.localScale = Vector3.back * 0.1f;
        rockRend.enabled = true;

        transform.Rotate(new Vector3(0, 0, Random.Range(0,360)));
    }

    void Update()
    {
        /*
        if (!connected)
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, 2129f)) * Time.fixedDeltaTime;
        //transform.Translate(new Vector3(0, 0, 10) * Time.deltaTime * 2f);

        
        */
        rockRend.transform.localScale = Vector3.Lerp(rockRend.transform.localScale, new Vector3(20, 20, 50), Time.deltaTime * 15f);
        rockRend.transform.localPosition = Vector3.MoveTowards(rockRend.transform.localPosition, new Vector3(0, 0, -0.2f), Time.deltaTime * 115f);
        if(Vector3.Distance(rockRend.transform.localPosition, new Vector3(0,0,-0.2f)) < 0.5f && connected == false) {
            GameObject loadedDust = Resources.Load("RockDust") as GameObject;
            GameObject newDust = Instantiate(loadedDust, transform.position, Quaternion.Euler(0, 0, 0), transform);
            //Destroy(GetComponent<RockProj>());
            connected = true;
            //sendMSG.SendMessage("RockHit", SendMessageOptions.DontRequireReceiver);
            if(sendMSG != null)
                sendMSG.SendMessage("RockHit", gameObject, SendMessageOptions.DontRequireReceiver);
        }
        if (Vector3.Distance(rockRend.transform.localPosition, new Vector3(0, 0, -0.2f)) < 0.5f) {
            rockRend.material.SetColor("_Colour", Color.Lerp(rockRend.material.GetColor("_Colour"), Color.black, Time.deltaTime * 15f));
            rockRend.material.SetFloat("_Power", Mathf.Lerp(rockRend.material.GetFloat("_Power"), 10f, Time.deltaTime * 5f));
            if(smallRocks != null)
                Destroy(smallRocks.gameObject);
        }
        if (smallRocks != null) {
            smallRocks.localPosition = Vector3.Lerp(smallRocks.localPosition, new Vector3(0, 0, 2), Time.deltaTime * 1.5f);
            smallRocks.localScale = Vector3.Lerp(smallRocks.localScale, Vector3.one * 0.75f, Time.deltaTime * 1.5f);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        /*
        connected = true;
        GetComponent<Rigidbody>().isKinematic = true;
        transform.position = collision.GetContact(0).point;
        */
    }
}
