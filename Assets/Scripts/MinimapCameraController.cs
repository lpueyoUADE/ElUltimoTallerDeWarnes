using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    public GameObject target;


    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);
    }
}
