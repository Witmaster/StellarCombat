using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour {

	void OnTriggerExit(Collider other)
    {
        if (other.tag == "Indestructible")
            return;
        Destroy(other.gameObject);
    }
}
