using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzsawAnimation : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.forward, 300.0f * Time.deltaTime);
    }
}
