using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class CelestialBody : GravityObject {

    // Enum to define the type of celestial body
    public enum BodyType { Planet, Moon, Sun }
    public BodyType bodyType;

    // Radius of the celestial body
    public float radius;
    // Surface gravity of the celestial body
    public float surfaceGravity;
    // Initial velocity of the celestial body
    public Vector3 initialVelocity;
    // Name of the celestial body
    public string bodyName = "Unnamed";
    Transform meshHolder;

    // Current velocity of the celestial body
    public Vector3 velocity { get; private set; }
    // Mass of the celestial body
    public float mass { get; private set; }
    Rigidbody rb;

    // Method called when the script instance is being loaded
    void Awake() {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        // Set the initial velocity
        velocity = initialVelocity;
        // Recalculate the mass of the celestial body
        RecalculateMass();
    }

    // Method to update the velocity of the celestial body based on gravitational forces
    public void UpdateVelocity(CelestialBody[] allBodies, float timeStep) {
        foreach (var otherBody in allBodies) {
            if (otherBody != this) {
                float sqrDst = (otherBody.rb.position - rb.position).sqrMagnitude;
                Vector3 forceDir = (otherBody.rb.position - rb.position).normalized;

                Vector3 acceleration = forceDir * Universe.gravitationalConstant * otherBody.mass / sqrDst;
                velocity += acceleration * timeStep;
            }
        }
    }

    // Method to update the velocity of the celestial body based on provided acceleration
    public void UpdateVelocity(Vector3 acceleration, float timeStep) {
        velocity += acceleration * timeStep;
    }

    // Method to update the position of the celestial body
    public void UpdatePosition(float timeStep) {
        rb.MovePosition(rb.position + velocity * timeStep);
    }

    // Method called when any value in the inspector is changed
    void OnValidate() {
        // Recalculate the mass and update the scale of the celestial body generator
        RecalculateMass();
        if (GetComponentInChildren<CelestialBodyGenerator>()) {
            GetComponentInChildren<CelestialBodyGenerator>().transform.localScale = Vector3.one * radius;
        }
        // Set the name of the game object to the body name
        gameObject.name = bodyName;
    }

    // Method to recalculate the mass of the celestial body
    public void RecalculateMass() {
        mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
        Rigidbody.mass = mass;
    }

    // Property to get the Rigidbody component
    public Rigidbody Rigidbody {
        get {
            if (!rb) {
                rb = GetComponent<Rigidbody>();
            }
            return rb;
        }
    }

    // Property to get the position of the celestial body
    public Vector3 Position {
        get {
            return rb.position;
        }
    }

}
