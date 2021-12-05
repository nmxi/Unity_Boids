using UdonSharp;
using UnityEngine;

public class BoidsElement : UdonSharpBehaviour
{
    // Reference to the controller.
    public BoidsManager BoidsManager;

    // Options for animation playback.
    public float animationSpeedVariation = 0.2f;

    // Random seed.
    float noiseOffset;

    // Caluculates the separation vector with a target.
    Vector3 GetSeparationVector(Transform target)
    {
        var diff = transform.position - target.transform.position;
        var diffLen = diff.magnitude;
        var scaler = Mathf.Clamp01(1.0f - diffLen / BoidsManager.neighborDist);
        return diff * (scaler / diffLen);
    }

    void Start()
    {
        noiseOffset = Random.value * 10.0f;
    }

    void Update()
    {
        if (BoidsManager == null)
            return;

        var currentPosition = transform.position;
        var currentRotation = transform.rotation;

        // Current velocity randomized with noise.
        var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
        var velocity = BoidsManager.velocity * (1.0f + noise * BoidsManager.velocityVariation);

        // Initializes the vectors.
        var separation = Vector3.zero;
        var alignment = BoidsManager.transform.forward;
        var cohesion = BoidsManager.transform.position;

        // Looks up nearby boids.
        var nearbyBoids = Physics.OverlapSphere(currentPosition, BoidsManager.neighborDist, BoidsManager.SearchLayer);

        // Accumulates the vectors.
        foreach (var boid in nearbyBoids)
        {
            if (boid.gameObject == gameObject) continue;
            var t = boid.transform;
            separation += GetSeparationVector(t);
            alignment += t.forward;
            cohesion += t.position;
        }

        var avg = 1.0f / nearbyBoids.Length;
        alignment *= avg;
        cohesion *= avg;
        cohesion = (cohesion - currentPosition).normalized;

        // Calculates a rotation from the vectors.
        var direction = separation + alignment + cohesion;
        var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);

        // Applys the rotation with interpolation.
        if (rotation != currentRotation)
        {
            var ip = Mathf.Exp(-BoidsManager.rotationCoeff * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(rotation, currentRotation, ip);
        }

        // Moves forawrd.
        transform.position = currentPosition + transform.forward * (velocity * Time.deltaTime);
    }
}
