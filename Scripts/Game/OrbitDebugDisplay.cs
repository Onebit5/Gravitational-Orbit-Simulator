using UnityEngine;

[ExecuteInEditMode]
public class OrbitDebugDisplay : MonoBehaviour {

    // Number of steps for orbit simulation
    public int numSteps = 1000;
    // Time step for each simulation step
    public float timeStep = 0.1f;
    // Option to use the physics time step defined in the Universe class
    public bool usePhysicsTimeStep;

    // Option to draw orbits relative to a central body
    public bool relativeToBody;
    // Central body around which orbits are drawn (if relativeToBody is true)
    public CelestialBody centralBody;
    // Width of the orbit lines
    public float width = 100;
    // Option to use thick lines for drawing orbits
    public bool useThickLines;

    void Start() {
        // Hide orbits when in play mode
        if (Application.isPlaying) {
            HideOrbits();
        }
    }

    void Update() {
        // Draw orbits in edit mode
        if (!Application.isPlaying) {
            DrawOrbits();
        }
    }

    // Method to draw orbits of celestial bodies
    void DrawOrbits() {
        // Find all celestial bodies in the scene
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        // Create virtual bodies to simulate orbits without affecting actual bodies
        var virtualBodies = new VirtualBody[bodies.Length];
        // Array to store draw points for each body
        var drawPoints = new Vector3[bodies.Length][];
        // Index of the reference frame body (if relativeToBody is true)
        int referenceFrameIndex = 0;
        // Initial position of the reference body
        Vector3 referenceBodyInitialPosition = Vector3.zero;

        // Initialize virtual bodies
        for (int i = 0; i < virtualBodies.Length; i++) {
            virtualBodies[i] = new VirtualBody(bodies[i]);
            drawPoints[i] = new Vector3[numSteps];

            if (bodies[i] == centralBody && relativeToBody) {
                referenceFrameIndex = i;
                referenceBodyInitialPosition = virtualBodies[i].position;
            }
        }

        // Simulate orbits
        for (int step = 0; step < numSteps; step++) {
            // Calculate position of the reference body (if relativeToBody is true)
            Vector3 referenceBodyPosition = (relativeToBody) ? virtualBodies[referenceFrameIndex].position : Vector3.zero;
            // Update velocities and positions of virtual bodies
            for (int i = 0; i < virtualBodies.Length; i++) {
                virtualBodies[i].velocity += CalculateAcceleration(i, virtualBodies) * timeStep;
                Vector3 newPos = virtualBodies[i].position + virtualBodies[i].velocity * timeStep;
                virtualBodies[i].position = newPos;
                // Adjust position relative to the reference body
                if (relativeToBody) {
                    var referenceFrameOffset = referenceBodyPosition - referenceBodyInitialPosition;
                    newPos -= referenceFrameOffset;
                }
                if (relativeToBody && i == referenceFrameIndex) {
                    newPos = referenceBodyInitialPosition;
                }
                drawPoints[i][step] = newPos;
            }
        }

        // Draw orbit paths
        for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++) {
            var pathColour = bodies[bodyIndex].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color;

            if (useThickLines) {
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                lineRenderer.enabled = true;
                lineRenderer.positionCount = drawPoints[bodyIndex].Length;
                lineRenderer.SetPositions(drawPoints[bodyIndex]);
                lineRenderer.startColor = pathColour;
                lineRenderer.endColor = pathColour;
                lineRenderer.widthMultiplier = width;
            } else {
                for (int i = 0; i < drawPoints[bodyIndex].Length - 1; i++) {
                    Debug.DrawLine(drawPoints[bodyIndex][i], drawPoints[bodyIndex][i + 1], pathColour);
                }
                // Hide renderer
                var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
                if (lineRenderer) {
                    lineRenderer.enabled = false;
                }
            }
        }
    }

    // Method to calculate acceleration for a virtual body
    Vector3 CalculateAcceleration(int i, VirtualBody[] virtualBodies) {
        Vector3 acceleration = Vector3.zero;
        for (int j = 0; j < virtualBodies.Length; j++) {
            if (i == j) {
                continue;
            }
            Vector3 forceDir = (virtualBodies[j].position - virtualBodies[i].position).normalized;
            float sqrDst = (virtualBodies[j].position - virtualBodies[i].position).sqrMagnitude;
            acceleration += forceDir * Universe.gravitationalConstant * virtualBodies[j].mass / sqrDst;
        }
        return acceleration;
    }

    // Method to hide orbit paths
    void HideOrbits() {
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++) {
            var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
            lineRenderer.positionCount = 0;
        }
    }

    // Method to adjust time step when using physics time step option
    void OnValidate() {
        if (usePhysicsTimeStep) {
            timeStep = Universe.physicsTime

        }
    }

    // Class representing a virtual body for orbit simulation
    class VirtualBody {
        public Vector3 position;
        public Vector3 velocity;
        public float mass;

        // Constructor to initialize virtual body from a CelestialBody
        public VirtualBody(CelestialBody body) {
            position = body.transform.position;
            velocity = body.initialVelocity;
            mass = body.mass;
        }
    }
}