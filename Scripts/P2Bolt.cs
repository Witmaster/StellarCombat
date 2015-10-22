using UnityEngine;
using System.Collections;

public class P2Bolt : MonoBehaviour {

    public GameObject explosion;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, 1);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Indestructible" && other.tag != "Boundary" && other.tag != "Player2")
        {
            Instantiate(explosion, other.transform.position, other.transform.rotation);
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

    }
}
