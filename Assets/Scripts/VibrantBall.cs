using UnityEngine;
using System.Collections;
using System;

public class VibrantBall : IComparable<VibrantBall> {

    public int id;
    public GameObject ball;
    public Vector3 initialPosition;
    public float yOffset;

    public VibrantBall(int id, GameObject ball, Vector3 initialPosition, float yOffset)
    {
        this.id = id;
        this.ball = ball;
        this.initialPosition = initialPosition;
        this.yOffset = yOffset;
    }

    public int CompareTo(VibrantBall other)
    {
        if (other == null)
        {
            return 1;
        }

        return id - other.id;
    }
}
