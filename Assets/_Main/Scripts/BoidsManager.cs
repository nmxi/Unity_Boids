using UdonSharp;
using UnityEngine;

public class BoidsManager : UdonSharpBehaviour
{
    [SerializeField] private Transform boidsElementParent;
    [SerializeField] private GameObject[] boidsElements;

    [Range(0.1f, 20.0f)]
    public float velocity = 6.0f;

    [Range(0.0f, 0.9f)]
    public float velocityVariation = 0.5f;

    [Range(0.1f, 20.0f)]
    public float rotationCoeff = 4.0f;

    [Range(0.1f, 10.0f)]
    public float neighborDist = 2.0f;

    public LayerMask SearchLayer;

    private void Start()
    {
        boidsElements = new GameObject[boidsElementParent.childCount];

        for (int i = 0; i < boidsElementParent.childCount; i++)
        {
            boidsElements[i] = boidsElementParent.GetChild(i).gameObject;

            var rotation = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
            boidsElements[i].transform.rotation = rotation;
            boidsElements[i].GetComponent<BoidsElement>().BoidsManager = this;
        }
    }
}