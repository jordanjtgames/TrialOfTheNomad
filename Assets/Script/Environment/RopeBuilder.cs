using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RopeBuilder : MonoBehaviour
{
    public Transform posA;
    public Transform posB;
    public Transform ropeGraphic;
    public Transform ropeGuide;

    public Transform triggers;

    public int currentVert = 1;

    public float val;
    public float res;

    private void Start()
    {
        
        if (Application.isPlaying) {
            for (int i = 0; i < ropeGuide.GetComponent<MeshFilter>().sharedMesh.vertices.Length - 1; i++) {
                //ropeGuide.GetComponent<MeshFilter>().mesh.vertices[i] = triggers.GetChild(i).position;
                //ropeGuide.GetComponent<MeshFilter>().mesh.vertices[i] = Vector3.zero;
                //Debug.LogError(ropeGuide.TransformPoint(ropeGuide.GetComponent<MeshFilter>().mesh.vertices[i]));

                //ropeGuide.GetComponent<MeshFilter>().mesh.vertices[i] = triggers.InverseTransformPoint(triggers.GetChild(i).position);
                Vector3[] verts = ropeGuide.GetComponent<MeshFilter>().mesh.vertices;
                Vector3 v = ropeGraphic.TransformPoint(ropeGuide.GetComponent<MeshFilter>().mesh.vertices[i]);
                //v.y += 5;
                v = triggers.GetChild(i).position;
                verts[i] = ropeGraphic.InverseTransformPoint(v);
                ropeGuide.GetComponent<MeshFilter>().mesh.vertices = verts;

                
                //GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //newObj.transform.localScale = Vector3.one * 0.3f;
                //newObj.transform.position = ropeGraphic.TransformPoint(ropeGuide.GetComponent<MeshFilter>().mesh.vertices[i]);
                

            }
        }
        
    }

    void Update()
    {
        if (!Application.isPlaying) {

            ropeGraphic.position = posA.position;
            ropeGuide.position = posA.position + Vector3.up * 0.5f;
            float dist = Vector3.Distance(posA.position, posB.position);
            ropeGraphic.LookAt(posB.position);
            ropeGuide.LookAt(posB.position + Vector3.up * 0.5f);
            ropeGraphic.localScale = new Vector3(100, 100, dist * 10);
            ropeGuide.localScale = new Vector3(100, 100, dist * 10);

            /*
            currentVert = Mathf.Clamp(currentVert, 0, ropeGuide.GetComponent<MeshFilter>().sharedMesh.vertices.Length - 1);
            float ropeDepth = Mathf.Clamp(ropeGuide.GetComponent<Renderer>().sharedMaterial.GetFloat("_RopeDepth"), -2, 2);
            //float sinDepth = Mathf.Sin()
            float prog = ((float)currentVert / ropeGuide.GetComponent<MeshFilter>().sharedMesh.vertices.Length) * val;//3.1415f
            res = Mathf.Sin(prog);
            float sinDepth = ropeDepth * Mathf.Sin(prog);
            //tester.transform.localPosition = ropeGuide.GetComponent<MeshFilter>().sharedMesh.vertices[currentVert] - Vector3.up * sinDepth / 100;
            */
            float ropeDepth = Mathf.Clamp(ropeGuide.GetComponent<Renderer>().sharedMaterial.GetFloat("_RopeDepth"), -2, 2);

            string newName1 = gameObject.name.Replace(" (", "_");
            string newName2 = newName1.Replace(")", "");
            ropeGuide.name = newName2;

            for (int i = 0; i < ropeGuide.childCount; i++) {
                float i_prog = ((float)i / ropeGuide.GetComponent<MeshFilter>().sharedMesh.vertices.Length) * val;//3.1415f
                float i_sinDepth = ropeDepth * Mathf.Sin(i_prog);
                ropeGuide.GetChild(i).localPosition = ropeGuide.GetComponent<MeshFilter>().sharedMesh.vertices[i] - Vector3.up * i_sinDepth / 100;
            }

            triggers.position = posA.position;

            for (int i = 0; i < triggers.childCount; i++) {
                float i2_prog = ((float)i / ropeGuide.GetComponent<MeshFilter>().sharedMesh.vertices.Length) * val;//3.1415f
                float i2_sinDepth = ropeDepth * Mathf.Sin(i2_prog);
                //triggers.GetChild(i).position = ropeGuide.TransformPoint(ropeGuide.GetComponent<MeshFilter>().sharedMesh.vertices[i]) - Vector3.up * i2_sinDepth / 100;
                triggers.GetChild(i).position = ropeGuide.GetChild(i).position + Vector3.up * 0.25f;
                float s = dist / 15;
                triggers.localScale = new Vector3(s, s, s);
                if (i < triggers.childCount - 1)
                    triggers.GetChild(i).LookAt(triggers.GetChild(i + 1));
                else
                    triggers.GetChild(i).rotation = triggers.GetChild(i - 1).rotation;
                triggers.GetChild(i).name = newName2 + "*";
            }
        }



        //ropeGraphic.GetComponent<Renderer>().sharedMaterial.SetFloat("_RopeDepth", posB.localScale.y);
    }
}
