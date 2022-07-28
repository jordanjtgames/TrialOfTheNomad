using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    float currentInterval = 0.1f;
    public GameObject slimePrefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.N)) {
            currentInterval -= Time.deltaTime;
            if(currentInterval <= 0) {
                GameObject newSlime = Instantiate(slimePrefab, transform.position, transform.rotation);
                currentInterval = 0.3f;
            }
        }
    }
}
