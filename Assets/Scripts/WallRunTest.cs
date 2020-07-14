using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunTest : MonoBehaviour
{
    public bool isWallR, isWallL;
    private RaycastHit hitL, hitR;
    private int jumpCount = 0;
    public Rigidbody cc, rb;
    public CapsuleCollider cctemp;
    public float maxRCDistance, hoverFloat;
    // Start is called before the first frame update
    void Start()
    {
        cctemp = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (cc.Grounded)
        //{
        //    jumpCount = 0;
        //}
       
       
            if (Physics.Raycast(transform.position, transform.right, out hitR, maxRCDistance))
            {
                if (hitR.transform.tag == "Wall")
                {
                    isWallR = true;
                    isWallL = false;
                    jumpCount += 1;
                    rb.velocity = Vector3.zero;
                    rb.AddForce(new Vector3(0, hoverFloat, 0f));
                //rb.useGravity = false;
            }
            }
            else if (Physics.Raycast(transform.position, -transform.right, out hitR, maxRCDistance))
            {
                if (hitR.transform.tag == "Wall")
                {
                    isWallL = true;
                    isWallR = false;
                    jumpCount += 1;
                    rb.velocity = Vector3.zero;
                    rb.AddForce(new Vector3(0, hoverFloat, 0f));
                    //rb.useGravity = false;
                }
            }
            else
            {
                isWallL = false;
                isWallR = false;
                rb.useGravity = true;
            }
        
    }
}
