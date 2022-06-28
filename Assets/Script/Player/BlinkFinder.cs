using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkFinder : MonoBehaviour
{
    public Transform startPoint;
    //public Transform midPoint;
    public Vector3 midPoint;
    public Transform endPoint;

    public Transform bone_HandEnd;
    public Transform bone_Middle;
    public Transform bone_Destination;

    public Transform blinkBoneRotation_1;
    public Transform blinkBoneRotation_2;
    public Transform blinkBoneRotation_3;

    public float rotationValue_1 = 5;
    public float rotationValue_2 = 5;
    public float rotationValue_3 = 5;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //startPoint.position = bone_HandEnd.position;
        //endPoint.position = bone_Destination.position;
        bone_HandEnd.position = startPoint.position;
        bone_Destination.position = endPoint.position + (Camera.main.transform.position - endPoint.position).normalized;

        bone_Middle.position = Vector3.Lerp(bone_Middle.position, (bone_HandEnd.position + bone_Destination.position) / 2, Time.deltaTime * 6) + (Vector3.up * 0.04f) + (Camera.main.transform.TransformDirection(Vector3.left) * 0.04f);
        //bone_Middle.position = Vector3.MoveTowards(bone_Middle.position, (bone_HandEnd.position + bone_Destination.position) / 2, Time.deltaTime * 10.5f);

        //bone_Middle.position = (bone_HandEnd.position + bone_Destination.position) / 2;
    }

    void Update()
    {
        //bone_HandEnd.LookAt(endPoint.position, Vector3.right);
        //bone_Middle.LookAt(endPoint.position, Vector3.right);
        //bone_Destination.LookAt(endPoint.position, Vector3.right);

        blinkBoneRotation_1.Rotate(new Vector3(0, rotationValue_1, 0) * Time.deltaTime * 20);
        blinkBoneRotation_2.Rotate(new Vector3(0, rotationValue_2, 0) * Time.deltaTime * 20);
        blinkBoneRotation_3.Rotate(new Vector3(0, rotationValue_3, 0) * Time.deltaTime * 20);

        bone_HandEnd.rotation = blinkBoneRotation_1.rotation;
        bone_Middle.rotation = blinkBoneRotation_2.rotation;
        bone_Destination.rotation = blinkBoneRotation_3.rotation;

        /*
        bone_HandEnd.Rotate(new Vector3(0, 5, 0) * Time.deltaTime * 15);
        bone_Middle.Rotate(new Vector3(0, 5.7f, 0) * Time.deltaTime * 20);
        bone_Destination.Rotate(new Vector3(0, -5, 0) * Time.deltaTime * 30);
        */
    }

    public void SetUpMidpoint()
    {
        bone_HandEnd.position = startPoint.position;
        bone_Destination.position = endPoint.position + (Camera.main.transform.position - endPoint.position).normalized;
        bone_Middle.position = ((bone_HandEnd.position + bone_Destination.position) / 2) + (Vector3.up * 0.04f) + (Camera.main.transform.TransformDirection(Vector3.left) * 0.04f);
    }
}
