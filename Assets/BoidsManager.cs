using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static BoidsManager;
using static UnityEngine.GraphicsBuffer;

public class BoidsManager : MonoBehaviour
{
    public class Boid
    {
        public GameObject visualObject;
        public Vector2 position;
        public Vector2 velocity;
        public Boid(Vector2 position, Vector2 velocity,GameObject _visual)
        {
            visualObject = _visual;
            this.position = position;
            this.velocity = velocity;
        }
    }
    
    [SerializeField] private float boidsDetectionRadious;
    [Range(0,360)]
    [SerializeField] private float boidsDetectionAngle;
    [SerializeField] private float boidSpeed;
    [SerializeField] private Vector2 simulationSpace;
    [Space]
    [SerializeField] private bool _avoid;
    [SerializeField] private bool _align;
    [SerializeField] private bool _flock;
    [SerializeField] private float alignmentMultiplier = 1;
    [Space]
    [SerializeField] GameObject _visualObject;

    [SerializeField] int boidsCount;
    List<Boid> boids = new List<Boid>();

    Vector2 boundsLeftCorrner;
    Vector2 boundsRightCorrner;
    private void Start()
    {
        boundsLeftCorrner = (Vector2)transform.position - simulationSpace / 2;
        boundsRightCorrner = (Vector2)transform.position + simulationSpace / 2;
        SpawnBoids();
        
    }
    void SpawnBoids()
    {
        for(int i = 0;i<boidsCount;i++)
        {
            Vector2 spawnPosition = new Vector2(
                Random.Range(boundsLeftCorrner.x, boundsRightCorrner.x), 
                Random.Range(boundsLeftCorrner.y, boundsRightCorrner.y));
            Vector2 forwardDirection = DirectionFromAngle(Random.Range(0,360));
            GameObject v = Instantiate(_visualObject, spawnPosition, Quaternion.identity);
            if (i == 0)
            {
                v.GetComponent<SpriteRenderer>().color = Color.red;
            }
            boids.Add(new Boid(spawnPosition,forwardDirection,v));
        }
    }
    
    private void Update()
    {
        
        foreach(Boid boid in boids)
        {
            CheckForOutOfBounds(boid);
            CheckForBoidsInRange(boid);
            boid.position += boid.velocity *boidSpeed* Time.deltaTime;
            boid.visualObject.transform.position = boid.position;
            boid.visualObject.transform.up = boid.velocity;
        }
        
        
    }
    void AdjustVelocity(Boid thisBoid, List<Boid> neighbourBoids)
    {
        Vector2 floackAdjustment = Flock(thisBoid,neighbourBoids,.003f*alignmentMultiplier);
        Vector2 alignAdjustment = Align(thisBoid,neighbourBoids,.01f * alignmentMultiplier);
        Vector2 avoidAdjustment = Avoid(thisBoid, neighbourBoids, .001f * alignmentMultiplier);
        if (_flock) thisBoid.velocity += floackAdjustment;
        if (_avoid) thisBoid.velocity += avoidAdjustment;
        if (_align) thisBoid.velocity += alignAdjustment;
        thisBoid.velocity = thisBoid.velocity.normalized;
    }
    void CheckForBoidsInRange(Boid curentBoid)
    {
        List<Boid> boidsInRange = new List<Boid>();
        foreach(Boid boid in boids)
        {
            
            if (boid == curentBoid) continue;
            if (Vector2.Distance(boid.position, curentBoid.position) < boidsDetectionRadious)
            {
                Vector2 diretionToTarget = boid.position- curentBoid.position;
                if (Vector2.Angle(curentBoid.velocity.normalized, diretionToTarget.normalized) < boidsDetectionAngle/2)
                {
                    boidsInRange.Add(boid);
                    //Debug.DrawLine(curentBoid.position,boid.position)d;
                }
            }
        }
        if(boidsInRange.Count > 0 ) AdjustVelocity(curentBoid, boidsInRange);

    }
    Vector2 Flock(Boid thisBoid, List<Boid> neighbourBoids,float power)
    {
        Vector2 mean = Vector2.zero;
        foreach (Boid boid in neighbourBoids)
        {
            mean += boid.position;
        }
        mean/=neighbourBoids.Count;
        Vector2 deltaCenter = mean - thisBoid.position;
        return deltaCenter * power;
        
    }
    Vector2 Align(Boid thisBoid, List<Boid> neighbourBoids, float power)
    {
        Vector2 meanVelocity = Vector2.zero;
        foreach (Boid boid in neighbourBoids)
        {
            meanVelocity += boid.velocity;
        }
        meanVelocity /= neighbourBoids.Count;
        Vector2 deltaVelocity = meanVelocity - thisBoid.velocity;
        return deltaVelocity * power;
    }
    Vector2 Avoid(Boid thisBoid, List<Boid> neighbourBoids, float power)
    {
        Vector2 sumClosnes = Vector2.zero;
        foreach (Boid boid in neighbourBoids)
        {
            float clossnes = boidsDetectionRadious - Vector2.Distance(thisBoid.position,boid.position);
            sumClosnes += (thisBoid.position-boid.position) * clossnes;
        }
        return (sumClosnes * power);
    }
    void CheckForOutOfBounds(Boid boid)
    {
        Vector2 teleportPosition = boid.position;
        if (boid.position.x < boundsLeftCorrner.x) teleportPosition.x = boundsRightCorrner.x;
        if (boid.position.x > boundsRightCorrner.x) teleportPosition.x = boundsLeftCorrner.x;
        if (boid.position.y < boundsLeftCorrner.y) teleportPosition.y = boundsRightCorrner.y;
        if (boid.position.y > boundsRightCorrner.y) teleportPosition.y = boundsLeftCorrner.y;
        boid.position = teleportPosition;
    }

    Vector2 DirectionFromAngle(float angleDeg)
    {
        return new Vector2( Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad));
    }
    float AngleFromDirection(Vector2 direction)
    {
        direction = direction.normalized;
        float angle = Mathf.Atan2(Mathf.Abs(direction.y), Mathf.Abs(direction.x)) *Mathf.Rad2Deg;
        if (direction.x >= 0 && direction.y >= 0) angle = angle; //First Quadrant
        else if (direction.x  < 0 && direction.y >= 0)angle = 180-angle; //Second Quadrant
        else if (direction.x < 0 && direction.y < 0)angle = 180+angle; //Third Quadrant
        else if (direction.x >= 0 && direction.y < 0)angle = 360-angle; //Fourth Quadrant
        return angle;
    }
    
    private void OnDrawGizmos()
    {
        foreach(var boid in boids)
        {
            //Gizmos.color = Color.white;
            if (boid == boids.First())
            {
                Gizmos.DrawWireSphere(boid.position, boidsDetectionRadious);
                Vector2 boidForward = boid.velocity.normalized;
                Vector2 boidRight = new Vector2(-boidForward.y, boidForward.x);
                Gizmos.color = Color.green;
                //Gizmos.DrawLine(boid.position,boid.position+boidForward*boidsDetectionRadious);
                Gizmos.color = Color.red;
                //Gizmos.DrawLine(boid.position, boid.position + boidRight * boidsDetectionRadious);
                Gizmos.color = Color.yellow;
                float forwardAngle = AngleFromDirection(boidForward);
                Gizmos.DrawLine(boid.position, boid.position + DirectionFromAngle(forwardAngle + (boidsDetectionAngle / 2)) * boidsDetectionRadious);
                Gizmos.DrawLine(boid.position, boid.position + DirectionFromAngle(forwardAngle + (-boidsDetectionAngle / 2)) * boidsDetectionRadious);
                
            }
            //Gizmos.DrawSphere(boid.position, .3f);
        }
        Gizmos.DrawWireCube(Vector2.zero,simulationSpace);
    }
}
