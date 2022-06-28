using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotTK : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision col) {
        
        if (GetComponent<Rigidbody>().velocity.magnitude > 1) {
            BlockingState.forceLetGo = true;
            transform.parent.GetComponent<PotScript>().BreakPot(0,transform.position);

            //Debug.LogError(GetComponent<Rigidbody>().velocity.magnitude);
        }
    }

    void RockHit(GameObject g) {
        BlockingState.forceLetGo = true;
        transform.parent.GetComponent<PotScript>().BreakPot(0, transform.position);
        Destroy(g);
    }
}
