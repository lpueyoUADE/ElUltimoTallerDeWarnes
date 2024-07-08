using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate360Controller : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,15 * Time.deltaTime,0);
    }
}
