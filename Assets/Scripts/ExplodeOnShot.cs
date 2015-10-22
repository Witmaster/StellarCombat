using UnityEngine;
using System.Collections;

public class ExplodeOnShot : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        Destroy(this);
    }
}
