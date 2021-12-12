using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uplight : MonoBehaviour
{
    private Vector3 point1;
    private Vector3 point2;
    private int pointNum = 2;
    private LineRenderer selfLinerender;

    void Start()
    {
        selfLinerender = this.GetComponent<LineRenderer>() as LineRenderer;
        selfLinerender.positionCount = pointNum;
        selfLinerender.startWidth = 0.1f;
        selfLinerender.endWidth = 0.1f;
    }

    void Update()
    {
        point1 = this.GetComponentInChildren<Transform>().GetChild(0).position;
        point2 = this.GetComponentInChildren<Transform>().GetChild(1).position;
        selfLinerender.SetPosition(0,point1);
        selfLinerender.SetPosition(1, point2);
    }
}
