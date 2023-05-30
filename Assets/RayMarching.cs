using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RayMarching : MonoBehaviour
{
    public float SignedDstToCircle(Vector2 p, Vector2 center,float radious)
    {
        return lenght(center-p)-radious;
    }
    public float SignedDstToBox(Vector2 p, Vector2 center, Vector2 size)
    {
        Vector2 offset = abs(p-center)-size;
        float unsignedDst = lenght(max(offset,Vector2.zero));
        float dstInsideBox = lenght(min(offset, Vector2.zero));
        return unsignedDst + dstInsideBox;

    }
    float lenght(Vector2 a)
    {
        return Mathf.Sqrt(a.x * a.x+a.y*a.y);
    }
    Vector2 abs(Vector2 a)
    {
        return new Vector2(Mathf.Abs(a.x), Mathf.Abs(a.y));
    }
    Vector2 max(Vector2 a,Vector2 b)
    {
        return new Vector2(Mathf.Max(a.x,b.x),Mathf.Max(a.y, b.y));
    }
    Vector2 min(Vector2 a, Vector2 b)
    {
        return new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
    }
}
