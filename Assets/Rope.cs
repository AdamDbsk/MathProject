using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Rope : MonoBehaviour
{
    public class Point
    {
        public Point(Vector2 _pos)
        {
            position = _pos;
            prevPosition = _pos;
        }
        public Vector2 position, prevPosition;
        public bool locked;
    }
    public class Stick
    {
        public Stick(Point _pointA, Point _pointB, float _lenght)
        {
            pointA = _pointA;
            pointB = _pointB;
            lenght = _lenght;
        }
        public Point pointA, pointB;
        public float lenght;
    }
    bool simulate = false;
    public float gravity;
    public int numInterations;
    List<Point> points = new List<Point>();
    List<Stick> sticks = new List<Stick>();

    Point connectedPoint;
    bool connectPoints = false;

    private void Start()
    {
        /*
        Point previousPoint = null;
        for(int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                Point point = new Point(Vector2.zero + new Vector2(x, y));
                points.Add(point);
                if (previousPoint != null)
                {
                    if(y != 0) sticks.Add(new Stick(previousPoint, point, Vector2.Distance(previousPoint.position, point.position)));

                    if (x != 0) sticks.Add(new Stick(points[points.IndexOf(point) - 20], point, Vector2.Distance(points[points.IndexOf(point) - 20].position, point.position)));

                }
                
                previousPoint = point;
            }
        }
        */
    }
    private void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseToPointDistance = 1;
        Point closestPointToMouse= null;
        if (points.Count > 0)
        {
            closestPointToMouse = points.First();
            foreach (Point point in points)
            {
                mouseToPointDistance = Vector2.Distance(mousePosition, point.position);
                if (mouseToPointDistance < Vector2.Distance(mousePosition, closestPointToMouse.position))
                {
                    closestPointToMouse = point;
                }
            } 
        }
        if (simulate)
        {
            Simulate();
            if (Input.GetMouseButton(0))
            {
                foreach (Stick stick in sticks)
                {
                    Vector2 stickCenter = (stick.pointA.position + stick.pointB.position) / 2;
                    if (Vector2.Distance(mousePosition, stickCenter) < .5f)
                    {
                        sticks.Remove(stick);
                        return;
                    }
                }
            }
        }
        else
        {
            float accualDistance = Mathf.Infinity;
            if (closestPointToMouse != null) accualDistance = Vector2.Distance(mousePosition, closestPointToMouse.position);
            
            if (Input.GetMouseButtonDown(0))
            {
                
                if (accualDistance < .5f)
                {
                    connectPoints = true;
                    connectedPoint = closestPointToMouse;
                }
                if (!connectPoints&& accualDistance > .5f) PlacePoint(mousePosition);
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (accualDistance < .5f)
                {
                    closestPointToMouse.locked = !closestPointToMouse.locked;
                }
            }
            if (Input.GetMouseButtonUp(0)&&connectPoints)
            {

                if (accualDistance < .5f&&connectedPoint!=closestPointToMouse)
                {
                    sticks.Add(new Stick(connectedPoint, closestPointToMouse, Vector2.Distance(connectedPoint.position, closestPointToMouse.position)));
                }
                connectPoints = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space)) simulate = !simulate;
        
    }
    
    void PlacePoint(Vector2 _position)
    {
        points.Add(new Point(_position));
    }
    void Simulate()
    {
        foreach (Point p in points)
        {
            if (!p.locked)
            {
                Vector2 positionBeforeUpdate = p.position;
                p.position += p.position - p.prevPosition;
                p.position += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
                p.prevPosition = positionBeforeUpdate;
            }
        }
        for(int i = 0; i < numInterations; i++)
        {
            foreach (Stick stick in sticks)
            {
                
                Vector2 stickCenter = (stick.pointA.position + stick.pointB.position) / 2;
                Vector2 stickDir = (stick.pointA.position - stick.pointB.position).normalized;
                
                if(!stick.pointA.locked) stick.pointA.position = stickCenter + stickDir * stick.lenght / 2;

                if (!stick.pointB.locked) stick.pointB.position = stickCenter - stickDir * stick.lenght / 2;

            }
        }
        
        
    }
    private void OnDrawGizmos()
    {
        if (!simulate)
        {
            foreach (Point p in points)
            {
                Gizmos.color = Color.white;
                if (p.locked) Gizmos.color = Color.red;
                Gizmos.DrawSphere(p.position, .2f);
                Gizmos.color = Color.white;
            }
        }
        
        foreach (Stick stick in sticks)
        {
            
            Gizmos.DrawLine(stick.pointA.position, stick.pointB.position);
        }
    }
}
