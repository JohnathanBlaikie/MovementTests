using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pMovement : MonoBehaviour
{
    public GameObject player;
    public CapsuleCollider playerHB;
    public Rigidbody rig;
    public float moveSpeed, mSpeedX, mSpeedY, jumpForce;
    public bool okToJump, okToWallJump;
    
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * mSpeedX, 0)));
        //rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(Input.GetAxis("Mouse Y") * -mSpeedY, 0, 0)));
        rig.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * moveSpeed) + (transform.right * Input.GetAxis("Horizontal") * moveSpeed));

        if (Input.GetKeyDown("space") && (okToJump || okToWallJump))
        {
            rig.AddForce(transform.up * jumpForce);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            okToWallJump = true;
            rig.MovePosition(transform.position + (transform.right * Input.GetAxis("Horizontal") * moveSpeed));
        }
        if (collision.gameObject.tag == "Ground")
        {
            okToJump = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            okToJump = false;
        }
        if (collision.gameObject.tag == "Wall")
        {
            okToWallJump = false;
        }
    }
}
