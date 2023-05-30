using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurveEditor 
{
   public static Vector2 QuadraticCurve(Vector2 pointA,Vector2 pointB, Vector2 pointC,float t)
    {
        Vector2 p0 = Vector2.Lerp(pointA, pointB, t);
        Vector2 p1 = Vector2.Lerp(pointB, pointC, t);
        return Vector2.Lerp(p0, p1, t);
    }
    public static Vector2 CubicCurve(Vector2 pointA,Vector2 pointB, Vector2 pointC, Vector2 pointD,float t)
    {
        Vector2 p0 = QuadraticCurve(pointA, pointB, pointC, t);
        Vector2 p1 = QuadraticCurve(pointB, pointC, pointD, t);
        return Vector2.Lerp(p0, p1, t);
    }
}
