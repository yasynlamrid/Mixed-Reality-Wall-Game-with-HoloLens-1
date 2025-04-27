using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{

    public float xsize = 0.3f;
    public float ysize = 0.3f;
    public float zsize = 0.3f;
    private void Start()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true; 
        boxCollider.size = new Vector3(xsize, ysize, zsize); 
    }
}
