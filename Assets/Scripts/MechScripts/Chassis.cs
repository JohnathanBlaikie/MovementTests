using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chassis : MonoBehaviour
{

    public enum CHASSIS { Light, Medium, Heavy };
    public CHASSIS chasEnum;
    public float health, movementSpeed, fireRate, timeSinceLastShot;
    public int primaryAmmo, maxPrimaryAmmo;
    public GameObject pilot;
    public GameObject chassisCam;
    public GameObject bulletHoles;
    public bool isPlayerPiloting;
    protected Ray rayOrigin;
    protected RaycastHit hit;
    protected Vector3 moveVec = Vector3.zero;
    protected Rigidbody rig;
    protected float mSpeedX, mSpeedY, pitch, yaw;

    private void Start()
    {
        
    }

    public void PilotEmbarking(GameObject _pilot)
    {
        Debug.Log("Successfully embarked");
        isPlayerPiloting = true;
        _pilot.transform.SetParent(this.transform);
        _pilot.GetComponent<pMCTESTING>().piloting = true;
        pilot = _pilot;
        pilot.GetComponent<Rigidbody>().useGravity = false;
        //Ideally this would be the part where a cool embarking animation plays, but I'm chillin.
        pilot.GetComponent<Rigidbody>().velocity = Vector3.zero;
        pilot.transform.position = transform.position;
        pilot.SetActive(false);
        mSpeedX = pilot.GetComponent<pMCTESTING>().mSpeedX;
        mSpeedY = pilot.GetComponent<pMCTESTING>().mSpeedY;
        chassisCam.SetActive(true);

    }
    public void PilotDisembarking(GameObject _pilot)
    {
        Debug.Log("Successfully Disembarked");
        isPlayerPiloting = false;
        _pilot.transform.SetParent(null);
        _pilot.GetComponent<pMCTESTING>().piloting = false;
        pilot = _pilot;
        pilot.GetComponent<Rigidbody>().useGravity = true;
        //Ideally this would be the part where a cool embarking animation plays, but I'm chillin.
        pilot.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //pilot.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        pilot.transform.position = transform.position;
        //pilot.GetComponent<Rigidbody>().MovePosition(-pilot.transform.forward * 30);
        //pilot.transform.SetPositionAndRotation(transform.forward, transform.rotation);
        pilot.GetComponent<Rigidbody>().rotation = transform.rotation;

        //pilot.transform.rotation = transform.rotation;
        chassisCam.SetActive(false);
        pilot.SetActive(true);

    }

    public void chassisReload()
    {
        //Play an animation here; maybe have an Animation Trigger activate it.
        Debug.Log(maxPrimaryAmmo - primaryAmmo);

        primaryAmmo = maxPrimaryAmmo;
    }
}

