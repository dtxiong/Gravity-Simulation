using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalBody : MonoBehaviour
{
    public float mass;
    float G;
    public Vector3 velocity;
    Vector3 acceleration;
    Vector3 position;
    float dt;
    IntegratorType integrator;
    enum IntegratorType
    {
        Euler,
        RK4
    }
    // Start is called before the first frame update
    void Start()
    {
        // mass = 10.0f;
        G = 2.0f;
        position = transform.position;
        //velocity = transform.forward;
        acceleration = Vector3.zero;
        integrator = IntegratorType.RK4;
    }

    // Update is called once per frame
    void Update()
    {

        dt = Time.deltaTime;
        acceleration = CalculateAcceleration(position);

        Vector3[] posvel = integrator switch
        {
            IntegratorType.Euler => Euler(),
            IntegratorType.RK4 => RK4()
        };
        
        transform.position = posvel[0];


    }

    Vector3 CalculateAcceleration(Vector3 position_)
    {

        Vector3 acceleration_ = Vector3.zero;
        GameObject[] gravitationalBodies;
        gravitationalBodies = GameObject.FindGameObjectsWithTag("GravitationalBody");
        int numberGravitationalBodies = gravitationalBodies.Length;
        for (int i = 0; i < numberGravitationalBodies; i++)
        {
            GameObject gravBody = gravitationalBodies[i];
            if (gravBody != this.gameObject)
            {
                float bodyMass = gravBody.GetComponent<GravitationalBody>().mass;
                Vector3 displacement = gravBody.transform.position - position_;
                float distance = displacement.magnitude;
                Vector3 direction = displacement / distance;
                acceleration_ += bodyMass * G * direction / Mathf.Pow(distance, 2.0f);
            }
        }
        return acceleration_;
    }

    Vector3[] Euler()
    {
        velocity += acceleration * dt;
        position += velocity * dt;
        return new Vector3[2] {position, velocity};
    }
    Vector3[] RK4()
    {
        //velocity += acceleration * dt;
        //position += velocity * dt;

        Vector3 kv1 = CalculateAcceleration(position);
        Vector3 kr1 = velocity;
        Vector3 kv2 = CalculateAcceleration(position + kr1 * dt/2.0f);
        Vector3 kr2 = velocity + kv1 * dt/2.0f;
        Vector3 kv3 = CalculateAcceleration(position + kr2 * dt/2.0f);
        Vector3 kr3 = velocity + kv2 * dt / 2.0f;
        Vector3 kv4 = CalculateAcceleration(position + kr3 * dt);
        Vector3 kr4 = velocity + kv3 * dt;

        velocity += dt / 6.0f * (kv1 + 2 * kv2 + 2 * kv3 + kv4);
        position += dt / 6.0f * (kr1 + 2 * kr2 + 2 * kr3 + kr4);
        return new Vector3[2] { position, velocity };
    }

}
