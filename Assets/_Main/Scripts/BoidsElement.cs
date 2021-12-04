using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;

public class BoidsElement : UdonSharpBehaviour
{
    public Vector3 Vector => vector;
    [SerializeField] private Vector3 vector;
    [SerializeField] private float speed = 0.01f;

    [Space]
    [SerializeField] public BoidsManager BoidsManager;

    private void Update()
    {
        if (BoidsManager == null)
            return;

        var nearElements = BoidsManager.GetNearObjects(this.transform.position);

        Vector3 separationVector = Vector3.zero;
        Vector3 vecBuff = Vector3.zero;
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < nearElements.Length; i++)
        {
            if (nearElements[i] == null)
                break;

            //分離
            var dif = this.transform.position - nearElements[i].transform.position;
            separationVector += dif;

            //整列
            vecBuff += nearElements[i].Vector;

            //結合
            pos += nearElements[i].transform.position;
        }

        vector += separationVector / nearElements.Length;

        vector += (vecBuff / nearElements.Length) - Vector;

        vector += (pos / nearElements.Length) - this.transform.position;

        var newPos = transform.position + vector;
        Debug.Log(newPos);

        if (newPos.x != float.NaN && newPos.y != float.NaN && newPos.z != float.NaN)
            transform.position += vector;
    }

    //private void LateUpdate()
    //{
    //    var newPos = transform.position + vector;
    //    Debug.Log(newPos);

    //    if(newPos.x != float.NaN && newPos.y != float.NaN && newPos.z != float.NaN)
    //        transform.position += vector;
    //}
}
