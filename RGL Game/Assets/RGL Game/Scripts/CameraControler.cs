using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, 
            target.transform.position.y, transform.position.z);
    }
}
