using UnityEngine;
using System.Collections;

public class MissileAI : MonoBehaviour {

    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 force;
    public float mass = 1.0f;

    public float maxSpeed = 30.0f;
    public float maxForce = 10.0f;

    public bool pursueEnabled;
    public GameObject pursueTarget;
    Vector3 pursueTargetPos;

    public Vector3 Pursue(GameObject target)
    {
        Vector3 toTarget = target.transform.position - transform.position;
        float lookAhead = toTarget.magnitude / maxSpeed;
        pursueTargetPos = target.transform.position
           + (target.GetComponent<PathFollower>().velocity * lookAhead);

        return Seek(pursueTargetPos);
    }

    Vector3 Seek(Vector3 target)
    {
        Vector3 toTarget = target - transform.position;
        toTarget.Normalize();
        Vector3 desired = toTarget * maxSpeed;
        return desired - velocity;
    }

    void OnCollisionEnter(Collision mycol)
    {      
        if (mycol.gameObject.CompareTag("enemy")) 
        {
            print("Collision");
            Destroy(mycol.gameObject);
            Destroy(this.gameObject);
        }
    }

        // Use this for initialization
        void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        force = Vector3.zero;

        if (pursueEnabled)
        {
            force += Pursue(pursueTarget);
        }

        force = Vector3.ClampMagnitude(force, maxForce);
        acceleration = force / mass;
        velocity += acceleration * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;
        if (velocity.magnitude > float.Epsilon)
        {
            transform.forward = velocity;
        }

    }
}
