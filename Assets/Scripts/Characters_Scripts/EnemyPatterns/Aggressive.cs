using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Aggressive : EnemyCharacter
{
    [Header("Pathfinding parameters")]
    Seeker seeker;
    // The calculated path
    public Path path;
    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;
    // How often to recalculate the path (in seconds)
    public float repathRate = 0.5f;
    private float lastRepath = 0;

    Vector3 previousDir = Vector3.zero;

    [Header("Aggression Trigger")]
    public bool triggered;
    public int RaysToShoot = 30;
    public float range;
    public float huntRange;
    public float deltaAngle;

    public override void Start()
    {
        base.Start();


        // Get a reference to the Seeker component
        seeker = GetComponent<Seeker>();

        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        //seeker.StartPath(transform.position, player.transform.position, OnPathComplete);

        // Previous direction
        while (previousDir.magnitude == 0)
            previousDir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
    }

    public override void Update()
    {
        base.Update();

        CheckTrigger();

        if (triggered)
        {
            if (player.level - level < 5)
                ProcessPathfinding();
            else
                ProcessPathfinding(-1);
        }
    }

    void CheckTrigger()
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        //Debug.Log(angle);
        float min = angle - deltaAngle / 2;
        float max = angle + deltaAngle / 2;
        for (int i = 0; i < RaysToShoot; i++)
        {
            float y = Mathf.Sin(min);
            float x = Mathf.Cos(min);
            min += deltaAngle / RaysToShoot;

            Vector3 end = transform.position + new Vector3(x, y, 0).normalized * range;
            Vector3 dir = end - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, range);
            Debug.DrawLine(transform.position, end, Color.red);
            if (hit.collider != null)
            {                
                //here is how to do your cool stuff ;)
                if (hit.collider.gameObject.name == "Player")
                {
                    triggered = true;
                }
            }
        }

        if ((transform.position - player.transform.position).magnitude > huntRange)
        {
            triggered = false;
            Stop();
        }
    }

    void ProcessPathfinding(int dirMod = 1)
    {
        if (Time.time - lastRepath > repathRate && seeker.IsDone())
        {
            lastRepath = Time.time + Random.value * repathRate * 0.5f;
            // Start a new path to the targetPosition, call the the OnPathComplete function
            // when the path has been calculated (which may take a few frames depending on the complexity)
            seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
        }
        if (path == null)
        {
            // We have no path to follow yet, so don't do anything
            return;
        }
        if (currentWaypoint > path.vectorPath.Count) return;
        if (currentWaypoint == path.vectorPath.Count)
        {
            Debug.Log("End Of Path Reached");
            currentWaypoint++;
            return;
        }
        // Direction to the next waypoint
        //Debug.Log(path.vectorPath[currentWaypoint]);
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        //if (dir.magnitude == 0)
        //    dir = previousDir;
        //else
        //    previousDir = dir;
        //while (dir.magnitude == 0)
        //dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        // Note that SimpleMove takes a velocity in meters/second, so we should not multiply by Time.deltaTime
        //controller.SimpleMove(dir);
        //Debug.Log(dir);
        if ((Vector2)dir != Vector2.zero)
            Move(dir * dirMod);
        //rb.velocity = dir * -speed;
        // The commented line is equivalent to the one below, but the one that is used
        // is slightly faster since it does not have to calculate a square root
        //if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
        if ((transform.position - path.vectorPath[currentWaypoint]).sqrMagnitude < nextWaypointDistance * nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
    public void OnPathComplete(Path p)
    {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);
        if (!p.error)
        {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            // Here I modified to 1 instead of 0 and this seems to fix a strange sudden animation state change flashing sometimes...
            currentWaypoint = 1;
        }
    }

    int DetermineEncounterLevel()
    {
        int sum = 0;
        foreach (Encounter.Enemy ee in encounter.enemies)
        {
            sum += ee.recipe.level;
        }
        return sum / encounter.enemies.Length;
    }
}



