using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {

    public float rotationRate = 0.5f;
	// Use this for initialization
	void Start () {
        Vector3 rotation = Random.insideUnitSphere;
        rotation.y = 0.0f;
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * rotationRate;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
