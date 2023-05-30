using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CurveVisualiser;

public class CurveVisualiser : MonoBehaviour
{
    public class Anchor
    {
        public Vector2 position;
        public Vector2 controlPoint1;
        public Vector2 controlPoint2;
        public bool lockControlPoints =true;

        public Anchor prevAnchor;
        public Anchor(Vector2 _position,Anchor _prevAnchor)
        {
            position = _position;
            controlPoint1 = _position + Vector2.up;
            controlPoint2 = _position - Vector2.up;
            if (_prevAnchor != null) {
                prevAnchor = _prevAnchor; 

            }

        }
    }
    [System.Serializable]
    public class Path
    {
        List<Anchor> points = new List<Anchor>();
    }
    List<Anchor> Points = new List<Anchor>();
    Anchor lastBuildAnchor = null;
    [SerializeField] int numInterations;
    [SerializeField] bool closedCurve;

    bool moveAnchor = false;
    int anchorObjectIndex;
    Anchor editableAnchor;
    public void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Input.GetMouseButtonDown(1))
        {
            foreach (Anchor anchor in Points)
            {
                if (Vector2.Distance(mousePosition, anchor.position) < .5f)
                {
                    anchor.lockControlPoints = !anchor.lockControlPoints;
                }
            }  
        }
        if (Input.GetMouseButtonDown(0))
        {
            moveAnchor = false;
            foreach (Anchor anchor in Points)
            {
                if (Vector2.Distance(mousePosition, anchor.position) < .5f)
                {
                    moveAnchor = true;
                    editableAnchor = anchor;
                    anchorObjectIndex = 0;
                    break;
                }
                else if(Vector2.Distance(mousePosition, anchor.controlPoint1) < .5f)
                {
                    moveAnchor = true;
                    editableAnchor = anchor;
                    anchorObjectIndex = 1;
                    break;
                }
                else if (Vector2.Distance(mousePosition, anchor.controlPoint2) < .5f)
                {
                    moveAnchor = true;
                    editableAnchor = anchor;
                    anchorObjectIndex = 2;
                    break;
                }
            }

            if (!moveAnchor)
            {
                Anchor a = new Anchor(mousePosition, lastBuildAnchor);
                lastBuildAnchor = a;
                Points.Add(a);  
                if(Points.Count>1) {
                    if (closedCurve)
                    {
                        Points.First().prevAnchor = a;
                    }
                    else
                    {
                        Points.First().prevAnchor = null;
                    }

                }
            }
        }
        if (Input.GetMouseButton(0)) 
        {
            if (moveAnchor)
            {
                switch (anchorObjectIndex)
                {
                    case 0:
                        
                        Vector2 dir1 = editableAnchor.controlPoint1 - editableAnchor.position;
                        Vector2 dir2 = editableAnchor.controlPoint2 - editableAnchor.position;
                        editableAnchor.controlPoint1 = mousePosition + dir1;
                        editableAnchor.controlPoint2 = mousePosition + dir2;
                        editableAnchor.position = mousePosition;
                        break;
                    case 1:
                        editableAnchor.controlPoint1 = mousePosition;
                        if(editableAnchor.lockControlPoints) {
                            float distance = Vector2.Distance(editableAnchor.position, editableAnchor.controlPoint2);
                            Vector2 direction = (editableAnchor.controlPoint1 - editableAnchor.position).normalized;
                            editableAnchor.controlPoint2 = editableAnchor.position-direction*distance; 
                        }
                        break;
                    case 2:
                        editableAnchor.controlPoint2 = mousePosition;
                        if (editableAnchor.lockControlPoints)
                        {
                            float distance = Vector2.Distance(editableAnchor.position, editableAnchor.controlPoint1);
                            Vector2 direction = (editableAnchor.controlPoint2 - editableAnchor.position).normalized;
                            editableAnchor.controlPoint1 = editableAnchor.position - direction * distance;
                        }
                        break;
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) { moveAnchor = false; }
    }
    public Vector2[] calculateEvenlySpacedPoints(float spacing,Anchor a)
    {
        
        List<Vector2> spacedPoints = new List<Vector2>();
        spacedPoints.Add(Points.First().position);
        Vector2 previousPoint = Points.First().position;
        float distanceFormLastPoint = 0;
        for(int i = 0;i<numInterations;i++)
        {
            Vector2 point = CurveEditor.CubicCurve(a.prevAnchor.position, a.prevAnchor.controlPoint1, a.controlPoint2, a.position, (float)i / (float)numInterations);
            distanceFormLastPoint = Vector2.Distance(previousPoint, point);
            if(distanceFormLastPoint > spacing)
            {
                Vector2 dir = (previousPoint - point).normalized;
                spacedPoints.Add(previousPoint+dir*distanceFormLastPoint);
                previousPoint = point;
                distanceFormLastPoint = 0;
            }
            
        }
        return spacedPoints.ToArray();
    }
    private void OnDrawGizmos()
    {
        
        foreach (Anchor a in Points)
        {
            
            Gizmos.color = Color.white;
            if (a.lockControlPoints) Gizmos.color = Color.gray;
            Gizmos.DrawSphere(a.position,.2f);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(a.position, a.controlPoint1);
            Gizmos.DrawLine(a.position, a.controlPoint2);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(a.controlPoint1, .2f);
            Gizmos.DrawSphere(a.controlPoint2, .2f);
            if(a.prevAnchor!=null)
            {
                for (int i = 0; i < numInterations; i++)
                {
                    Vector2 point = CurveEditor.CubicCurve(a.prevAnchor.position, a.prevAnchor.controlPoint1, a.controlPoint2, a.position, (float)i / (float)numInterations);
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(point,.05f);
                    

                }
            }
            
        }
    }
}
