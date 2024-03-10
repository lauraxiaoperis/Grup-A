using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMill : MonoBehaviour
{
    HingeJoint joint;

    // Intensidad del viento
    public float windStrength = 100f;

    void Start()
    {
        joint = GetComponent<HingeJoint>();
    }

    void Update()
    {
        var motor = joint.motor;
        motor.targetVelocity = windStrength;
        joint.motor = motor;
    }
}
