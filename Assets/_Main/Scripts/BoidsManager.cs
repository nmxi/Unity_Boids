using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;

public class BoidsManager : UdonSharpBehaviour
{
    [SerializeField] private Transform boidsElementParent;
    [SerializeField] private BoidsElement[] boidsElements;

    //SETTINGS
    [Space]
    [SerializeField] private float distanceThreashould;
    [SerializeField] private int boidsElementsMaxNum = 512;

    private void Start()
    {
        boidsElements = new BoidsElement[boidsElementParent.childCount];

        for (int i = 0; i < boidsElementParent.childCount; i++)
        {
            if (boidsElementsMaxNum < i)
                return;

            var g = boidsElementParent.GetChild(i).gameObject;
            var boidsElement = g.GetComponent<BoidsElement>();
            boidsElement.BoidsManager = this;
            boidsElements[i] = boidsElement;
        }
    }

    /// <summary>
    /// 与えた距離から近いオブジェクトを取得する
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public BoidsElement[] GetNearObjects(Vector3 pos)
    {
        BoidsElement[] res = new BoidsElement[boidsElementsMaxNum];
        int index = 0;

        foreach (var boidsElement in boidsElements)
        {
            if(Vector3.Distance(boidsElement.transform.position, pos) < distanceThreashould)
            {
                res[index] = boidsElement;
                index++;
            }
        }

        return res;
    }
}