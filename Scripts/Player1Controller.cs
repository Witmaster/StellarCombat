using UnityEngine;
using System.Collections;

public class Player1Controller : MonoBehaviour {

    public GameObject explosion;
    public GameObject playerExplosion;
    private float charge = 5.0f;
    private float rotationSpeed = 90.0f;
    public float acceleration = 0.15f;
    private Vector3 speed = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 gravityPull;
    private float gravity = 106.20f; //upper part of gravity force equation
    private float maxspeed = 1.5f; // should make these values adjustable
    public GameObject shot;
    public Transform P1Cannon;
    public float reloadTimer = 0;
    public GameObject engine;
    public bool isAway = false; // if true - ship left the system

    public void AdjustGravity(float rate)
    {//gravity rate is adjustable through controller code
        gravity *= rate;
    }

    public bool FTLReady() // check if FTL engine has enough power
    {
        if (charge > 99f)
            return true;
        return false;
    }
	void Update()
    {
        //apply acceleration based on ship's position 
        if (Input.GetKey(KeyCode.W) && charge > 0)
        {
            if(!engine.activeSelf)
            engine.SetActive(true); //play thruster animation only when accelerating
            float angle = GetComponent<Transform>().rotation.eulerAngles.y * Mathf.Deg2Rad;
            speed.x += Mathf.Sin(angle) * acceleration;
            speed.z += Mathf.Cos(angle) * acceleration;
            if (speed.x > maxspeed)
                speed.x = maxspeed;
            if (speed.x < -maxspeed)
                speed.x = -maxspeed;
            if (speed.z > maxspeed)
                speed.z = maxspeed;
            if (speed.z < -maxspeed)
                speed.z = -maxspeed;
            charge -= 1 * Time.deltaTime;
        }
        else
        {
            if(engine.activeSelf)
                engine.SetActive(false);
            if (speed.z > 0.05)
                speed.z -= 0.05f;
            if (speed.x > 0.01)
                speed.x -= 0.05f;
            if (speed.z < -0.01)
                speed.z += 0.05f;
            if (speed.x < -0.01 )
                speed.x += 0.01f;
        }
        if (Input.GetKey(KeyCode.Space) && Time.time > reloadTimer && charge >= 3)
        {//fire a projectile if sufficient charge
            reloadTimer = Time.time + 0.5f;
            charge -= 3;
            Instantiate(shot, P1Cannon.position, P1Cannon.rotation);
        }
        //calculate gravity pull & adjust ship's velocity
        Vector3 position = GetComponent<Transform>().position;
        float pull = gravity / (Mathf.Pow((1.496f * Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2))),2)*8); //calculate pull
        float gravangle = GravityAngle(position);
        gravityPull.x = Mathf.Sin(gravangle) * pull;
        gravityPull.z = Mathf.Cos(gravangle) * pull;
        //rotate ship
        float rotation = (Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0) * rotationSpeed;
        transform.Rotate(0, rotation*Time.deltaTime, 0);
        position.y = GravityAngle(position);
        transform.Translate(speed*Time.deltaTime, Space.World);
        transform.Translate(gravityPull*Time.deltaTime, Space.World);
        //alternative wincon if charge = 100% you can leave system and win
        if (charge < 110f)
            charge += (10 / Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2)))*Time.deltaTime;
        if (FTLReady() && Input.GetKey(KeyCode.S))
            FTL();
    }
    public float GetCharge()
    {
        return (charge > 100) ? 100f : Mathf.RoundToInt(charge);
    }

    private float GravityAngle(Vector3 position)
    { // calculate angle to apply gravity pull in correct direction
        float AC = Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2));
        float AB = (position.x); //x axis
        float BC = (position.z); //z axis angle B = 90deg, find angle A
        if (AB == 0)
        {
            if (BC > 0)
                return 270 * Mathf.Deg2Rad;
            if (BC < 0)
                return 90 * Mathf.Deg2Rad;
            if (BC == 0)
                return 0;
        }
        if (BC == 0)
        {
            if (AB > 0)
                return 180 * Mathf.Deg2Rad;
            if (AB < 0)
                return 0;
        }
        if (AB > 0)
        {
            if (BC > 0)
               return Mathf.Asin(AB/ AC) + (180 * Mathf.Deg2Rad);
            else
                return (360 - (Mathf.Asin(Mathf.Abs(AB) / AC)*Mathf.Rad2Deg)) * Mathf.Deg2Rad;
        }
        else
        {
            if (BC > 0)
                return (180 - (Mathf.Acos(BC / AC)*Mathf.Rad2Deg))*Mathf.Deg2Rad;
            else
                return Mathf.Acos(Mathf.Abs(BC) / AC);
        }
    }
    
    private void FTL() //play animation and win the game
    {
        if (charge > 99f)
        {
            this.gameObject.SetActive(false);
            isAway = true;
            Instantiate(playerExplosion, transform.position, transform.rotation);
        }
    }
    

    void OnTriggerEnter(Collider other) // collision handling
    {
        if (other.tag != "Player1")
        {
            if (other.tag != "Boundary")
            {
                Destroy(this.gameObject);
                if (other.tag != "Indestructible")
                {
                    Destroy(other.gameObject);
                    Instantiate(explosion, other.transform.position, other.transform.rotation);
                }
                Instantiate(playerExplosion, transform.position, transform.rotation);
            }
        }
    }
}
