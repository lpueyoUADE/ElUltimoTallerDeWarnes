using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowController : MonoBehaviour
{
    public Transform pointAt;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pointAt != null)
        {
            Vector3 direction = pointAt.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }
    }
}
