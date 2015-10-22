using UnityEngine;
using System.Collections;

public class Player2Control : MonoBehaviour
{
    public GameObject explosion;
    public GameObject playerExplosion;
    private float rotationSpeed = 90.0f;
    public float acceleration = 0.15f;
    private Vector3 speed = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 gravityPull;   
    private float gravity = 106.20f; //upper part of gravity force equation
    private float maxspeed = 1.5f; 
    public GameObject shot;
    public Transform P2Cannon; 
    public float reloadTimer = 0; //adjust rate of rire
    float charge = 5f; //alternative wincon
    public bool isAway = false;
    public GameObject engine;

    public void AdjustGravity(float rate)
    {//gravity rate is adjustable through controller code
        gravity *= rate;
    }

    public bool FTLReady()
    {
        if (charge > 99f)
            return true;
        return false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) && charge > 0)
        {
            //apply acceleration based on ship's position 
            if (!engine.activeSelf)
                engine.SetActive(true); //thruster animation only when used
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
        }
        else
        {
            if (engine.activeSelf)
                engine.SetActive(false);
            if (speed.z > 0.05)
                speed.z -= 0.05f;
            if (speed.x > 0.01)
                speed.x -= 0.05f;
            if (speed.z < -0.01)
                speed.z += 0.05f;
            if (speed.x < -0.01)
                speed.x += 0.01f;
        }
        if (Input.GetKey(KeyCode.KeypadEnter) && Time.time > reloadTimer && charge >= 3)
        { // shoot projectile if have enough charge
            reloadTimer = Time.time + 0.5f;
            charge -= 3;
            Instantiate(shot, P2Cannon.position, P2Cannon.rotation);
            GetComponent<AudioSource>().Play();
        }
        //calculate velocity vector & gravity pull vector
        Vector3 position = GetComponent<Transform>().position;
        float pull = gravity / (Mathf.Pow((1.496f * Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2))), 2) * 8); //calculate pull
        float gravangle = GravityAngle(position);
        gravityPull.x = Mathf.Sin(gravangle) * pull;
        gravityPull.z = Mathf.Cos(gravangle) * pull;
        //rotate ship 
        float rotation = (Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0) * rotationSpeed;
        transform.Rotate(0, rotation * Time.deltaTime, 0);
        position.y = GravityAngle(position);
        transform.Translate(speed * Time.deltaTime, Space.World);
        transform.Translate(gravityPull * Time.deltaTime, Space.World);
        //alternative wincon if charge = 100% you can leave system and win
        if(charge < 110f)
            charge += (10 / Mathf.Sqrt(Mathf.Pow(position.x, 2) + Mathf.Pow(position.z, 2)))*Time.deltaTime;
        if (FTLReady() && Input.GetKey(KeyCode.DownArrow))
            FTL();
    }

    public float GetCharge()
    {
        return (charge > 100)? 100f : Mathf.RoundToInt(charge);
    }

    private void FTL() //play animation and win the game
    {
        if (charge > 99)
        {
            this.gameObject.SetActive(false);
            isAway = true;
            Instantiate(playerExplosion, transform.position, transform.rotation);
        }
    }
    private float GravityAngle(Vector3 position)
    {// calculate angle from ship to star
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
                return Mathf.Asin(AB / AC) + (180 * Mathf.Deg2Rad);
            else
                return (360 - (Mathf.Asin(Mathf.Abs(AB) / AC) * Mathf.Rad2Deg)) * Mathf.Deg2Rad;
        }
        else
        {
            if (BC > 0)
                return (180 - (Mathf.Acos(BC / AC) * Mathf.Rad2Deg)) * Mathf.Deg2Rad;
            else
                return Mathf.Acos(Mathf.Abs(BC) / AC);
        }
    }

    void OnTriggerEnter(Collider other) //collision handling
    {
        if (other.tag != "Player2")
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
