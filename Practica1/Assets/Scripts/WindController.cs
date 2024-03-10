using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    HingeJoint joint;

    public Vector3 windDirection = Vector3.forward; // Dirección del viento
    public float windStrength = 10f; // Intensidad del viento

    void Start()
    {
        joint = GetComponent<HingeJoint>();
    }

    void Update()
    {
        var motor = joint.motor;
        Vector3 windVector = new Vector3(Mathf.Sin(Time.time), Mathf.Cos(Time.time), Mathf.Sin(Time.time + Mathf.PI / 2f));

        // Normalizar la dirección del viento y el vector del viento
        Vector3 normalizedWindDirection = windDirection.normalized;
        Vector3 normalizedWindVector = windVector.normalized;

        // Calcular el producto punto entre la dirección del viento y el vector del viento
        float dotProduct = Vector3.Dot(normalizedWindDirection, normalizedWindVector);

        // Calcular la componente de la fuerza del viento en la dirección del viento y escalarla por windStrength
        Vector3 appliedWindForce = dotProduct * normalizedWindDirection * windStrength;

        // Establecer la velocidad objetivo del motor en la magnitud de esta fuerza del viento aplicada
        motor.targetVelocity = appliedWindForce.magnitude;
        joint.motor = motor; //Se li torna a dir que el motor és el que hem modificat
    }
}
