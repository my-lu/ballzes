using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    float deathTime = 1.0f;

    void Start()
    {
        Destroy(gameObject, deathTime);
    }
}
