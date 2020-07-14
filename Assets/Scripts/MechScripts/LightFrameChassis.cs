using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFrameChassis : Chassis
{
    void Start()
    {
        chassisCam.SetActive(false);

        chasEnum = CHASSIS.Light;


        //mSpeedX = pilot.GetComponent<pMCTESTING>().mSpeedX;
        //mSpeedY = pilot.GetComponent<pMCTESTING>().mSpeedY;

        rig = GetComponent<Rigidbody>();
        isPlayerPiloting = false;
    }

    private void FixedUpdate()
    {
        if (isPlayerPiloting == true)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                moveVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
                //moveVec = new Vector3((transform.right * Input.GetAxisRaw("Horizontal")), rig.velocity.y, (transform.forward * Input.GetAxisRaw("Vertical")))
                moveVec = new Vector3(moveVec.x * movementSpeed * Time.deltaTime, rig.velocity.y, moveVec.z * movementSpeed * Time.deltaTime);
                //rig.velocity = moveVec * moveSpeed * Time.deltaTime;
                rig.velocity = moveVec;
            }
            else
            {
                rig.velocity *= .8f;
            }
            
        }
        else
        {

        }

    }
    void Update()
    {
        if (isPlayerPiloting == true)
        {
            timeSinceLastShot += Time.deltaTime;

            rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * mSpeedX, 0)));

            rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            yaw += mSpeedX * Input.GetAxis("Mouse X");
            pitch -= mSpeedY * Input.GetAxis("Mouse Y");
            chassisCam.transform.eulerAngles = (new Vector3(pitch, yaw, 0.0f));

            //Shooting next pls
            if (Input.GetKey(KeyCode.Mouse0) && primaryAmmo > 0)
            {
                if (timeSinceLastShot >= fireRate)
                {

                    if (Physics.Raycast(rayOrigin, out hit))
                    {
                        if (hit.collider.tag != null)
                        {
                            Instantiate(bulletHoles, hit.point, Quaternion.LookRotation(hit.normal));
                            primaryAmmo--;
                            timeSinceLastShot = 0;
                        }
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.R) || primaryAmmo <= 0)
            {
                //Maybe do anim.Play("Reload"); here or something, and have chassisReload() be triggered by an animTrigger.
                chassisReload();
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                PilotDisembarking(pilot);
            }
        }
    }
}
