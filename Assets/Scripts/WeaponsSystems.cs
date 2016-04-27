using UnityEngine;
using System.Collections;

public class WeaponsSystems : MonoBehaviour {

    public GameObject missile;
    public AudioClip shootprojectile;
    public AudioClip shootmissile;
    public AudioClip explode;
    private AudioSource source;
    private GameObject MissileTarget;
    private RaycastHit hit;
    private float lowPitchRange = .75F;
    private float highPitchRange = 1F;
    private bool beinghandled;
    int coroutinecalls = 0;

    System.Collections.IEnumerator fireProjectile()
    {
        beinghandled = true;
        // Use a line renderer
        GameObject lazer = new GameObject();
        lazer.transform.position = transform.position;
        lazer.transform.rotation = transform.rotation;
        LineRenderer line = lazer.AddComponent<LineRenderer>();
        lazer.AddComponent<Shoot>();
        lazer.AddComponent<Rigidbody>();
        lazer.gameObject.tag = "lazer";
        lazer.gameObject.name = "CannonShot";
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.SetColors(Color.red, Color.blue);
        line.SetWidth(0.1f, 0.1f);
        line.SetVertexCount(2);
        source.pitch = Random.Range(lowPitchRange, highPitchRange);
        source.PlayOneShot(shootprojectile);
        yield return new WaitForSeconds(1.0f);
        beinghandled = false;
    }



    System.Collections.IEnumerator fireMissile()
    {
        coroutinecalls += 1;
        beinghandled = true;
        // Use a line renderer
        Instantiate(missile, this.transform.position, MissileTarget.transform.rotation);
        //LineRenderer line = missile.AddComponent<LineRenderer>();
        //missile.AddComponent<MissileAI>();
        missile.GetComponent<MissileAI>().pursueEnabled = true;
        missile.GetComponent<MissileAI>().pursueTarget = MissileTarget;
        //line.material = new Material(Shader.Find("Particles/Additive"));
        //line.SetColors(Color.red, Color.blue);
        //line.SetWidth(0.1f, 0.1f);
        //line.SetVertexCount(2);
        source.pitch = Random.Range(lowPitchRange, highPitchRange);
        source.PlayOneShot(shootmissile);
        yield return new WaitForSeconds(1.0f);
        beinghandled = false;
    }

    void OnCollisionEnter(Collision mycol)
    {
        if (mycol.gameObject.CompareTag("missile"))
        {
            source.pitch = Random.Range(lowPitchRange, highPitchRange);
            source.PlayOneShot(explode);
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

        if (coroutinecalls < 2)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, 300) && !beinghandled)
            {
                //print("There is something in front of the object!");
                MissileTarget = hit.collider.gameObject;
                StartCoroutine("fireProjectile");
                //StartCoroutine("fireMissile");
            }
            /*
            else
            {
                StopCoroutine("fireProjectile");
            }
            */
        }
    }
}
