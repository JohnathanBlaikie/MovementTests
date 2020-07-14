using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pCam : MonoBehaviour
{
    public GameObject fPC, tPC;
    public float mSpeedX, mSpeedY, camCooldown;
    public bool fBool, tBool;
    private float yaw, pitch = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            fPC = GameObject.Find("First-Person Camera");
        }
        catch { }
        try
        {
            tPC = GameObject.Find("Third-Person Camera");
        }
        catch { }
        fPC.SetActive(true);
        tPC.SetActive(false);
        tBool = true;
        fBool = false;
    }

    // Update is called once per frame
    void Update()
    {
        yaw += mSpeedX * Input.GetAxis("Mouse X");
        pitch -= mSpeedY * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        if (Input.GetMouseButtonDown(2) && tBool &&camCooldown <= Time.time)
        {
            tPC.SetActive(true);
            fPC.SetActive(false);
            camCooldown = Time.time + 2.0f;
            tBool = false;
            fBool = true;
        }
        if (Input.GetMouseButtonDown(2) && fBool && camCooldown <= Time.time)
        {
            fPC.SetActive(true);
            tPC.SetActive(false);
            camCooldown = Time.time + 2.0f;
            fBool = false;
            tBool = true;
        }
    }
}
