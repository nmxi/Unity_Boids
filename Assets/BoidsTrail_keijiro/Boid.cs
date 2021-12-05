using UdonSharp;
using UnityEngine;

public class Boid : UdonSharpBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
    public float noiseOffset;
    public GameObject gameObject;
}
