using UnityEngine;
using System.Collections.Generic;

public class PathFollower : MonoBehaviour {

    public Vector3 velocity = Vector3.zero;
    public List<Vector3> waypoints = new List<Vector3>();
    int currentWaypoint = 0;
    public float mass = 1.0f;
    public float maxSpeed = 5.0f;
    public AudioClip shoot;
    private AudioSource source;
    private float lowPitchRange = .75F;
    private float highPitchRange = 1F;
    private bool beinghandled;

    System.Collections.IEnumerator fireProjectile()
    {
        beinghandled = true;
        // Use a line renderer
        GameObject lazer = new GameObject();
        lazer.transform.position = transform.position;
        lazer.transform.rotation = transform.rotation;
        LineRenderer line = lazer.AddComponent<LineRenderer>();
        lazer.AddComponent<Shoot>();
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.SetColors(Color.red, Color.blue);
        line.SetWidth(0.1f, 0.1f);
        line.SetVertexCount(2);
        source.pitch = Random.Range(lowPitchRange, highPitchRange);
        source.PlayOneShot(shoot);
        yield return new WaitForSeconds(.05f);
        beinghandled = false;
    }

  
    void OnDrawGizmos()
    {
        // Draw the path
        for (int i = 1; i <= waypoints.Count; i++)
        {
            Vector3 prev = waypoints[i - 1];
            Vector3 next = waypoints[i % waypoints.Count];
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(prev, next);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(next, 0.1f);
        }
    }

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
        source.maxDistance = 15.0f;
        source.minDistance = 1.0f;
        source.dopplerLevel = 0.1f;
    }
	
	// Update is called once per frame
	void Update () {        
        // Follow the path
        if (Vector3.Distance(waypoints[currentWaypoint], transform.position) < 1.0f)
        {
            
            currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
        }
        Vector3 desired = waypoints[currentWaypoint] - transform.position;
        desired.Normalize();
        desired *= maxSpeed;
        Vector3 force = desired - velocity;

        Vector3 acceleration = force / mass;
        velocity += acceleration * Time.deltaTime;
        transform.Translate(velocity * Time.deltaTime, Space.World);
        transform.forward = velocity;

        if (Physics.Raycast(transform.position, transform.forward, 300) && !beinghandled)
        {
            print("There is something in front of the object!");
            StartCoroutine("fireProjectile");
        }
        else
        {
            StopCoroutine("fireProjectile");
        }

    }
}
