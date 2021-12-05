using UnityEngine;
using UdonSharp;

namespace TrailBoids
{
    public class BoidController : UdonSharpBehaviour
    {
        #region Editable properties
        [SerializeField] private Transform boidsParentTransform;

        [Space]
        [SerializeField] float _spawnRadius = 4;

        [SerializeField] float _velocity = 6;
        [SerializeField, Range(0, 1)] float _velocityVariance = 0.5f;
        [SerializeField] Vector3 _scroll = Vector3.zero;

        [SerializeField] float _rotationSpeed = 4;
        [SerializeField] float _neighborDistance = 2;

        [SerializeField] private Boid[] _boids;

        #endregion

        #region Boid behavior

        // Calculates a separation vector from a boid with another boid.
        Vector3 GetSeparationVector(Boid self, Boid target)
        {
            var diff = target.position - self.position;
            var diffLen = diff.magnitude;
            var scaler = Mathf.Clamp01(1 - diffLen / _neighborDistance);
            return diff * scaler / diffLen;
        }

        // Reynolds' steering behavior
        void SteerBoid(Boid self)
        {
            // Steering vectors
            var separation = Vector3.zero;
            var alignment = transform.forward;
            var cohesion = transform.position;

            // Looks up nearby boids.
            var neighborCount = 0;
            foreach (var neighbor in _boids)
            {
                if (neighbor == self) continue;

                var dist = Vector3.Distance(self.position, neighbor.position);
                if (dist > _neighborDistance) continue;

                // Influence from this boid
                separation += GetSeparationVector(self, neighbor);
                alignment += neighbor.rotation * Vector3.forward;
                cohesion += neighbor.position;

                neighborCount++;
            }

            // Normalization
            var div = 1.0f / (neighborCount + 1);
            alignment *= div;
            cohesion = (cohesion * div - self.position).normalized;

            // Calculate the target direction and convert to quaternion.
            var direction = separation + alignment * 0.667f + cohesion;
            var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);

            // Applys the rotation with interpolation.
            if (rotation != self.rotation)
            {
                var ip = Mathf.Exp(-_rotationSpeed * Time.deltaTime);
                self.rotation = Quaternion.Slerp(rotation, self.rotation, ip);
            }
        }

        // Position updater
        void AdvanceBoid(Boid self)
        {
            var noise = Mathf.PerlinNoise(Time.time * 0.5f, self.noiseOffset) * 2 - 1;
            var velocity = _velocity * (1 + noise * _velocityVariance);
            var forward = self.rotation * Vector3.forward;
            self.position += (forward * velocity + _scroll) * Time.deltaTime;
        }

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            int childCount = boidsParentTransform.childCount;

            _boids = new Boid[childCount];

            for (int i = 0; i < childCount; i++)
            {
                var g = boidsParentTransform.GetChild(i).gameObject;
                Boid boid = g.GetComponent<Boid>();
                boid.position = transform.position + Random.insideUnitSphere * _spawnRadius;
                boid.rotation = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
                boid.noiseOffset = Random.value * 10;
                boid.gameObject = g;

                _boids[i] = boid;
            }
        }

        void Update()
        {
            foreach (var boid in _boids) SteerBoid(boid);
            foreach (var boid in _boids) AdvanceBoid(boid);

            foreach (var boid in _boids)
            {
                var tr = boid.gameObject.transform;
                tr.position = boid.position;
                tr.rotation = boid.rotation;
            }
        }

        #endregion
    }
}
