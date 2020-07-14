using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pMC : MonoBehaviour
{
    //Player Controller
    public GameObject player;
    public CapsuleCollider playerHB;
    public Rigidbody rig;
    public float moveSpeed, jumpForce;
    public bool okToJump;

    //Camera Controller
    public GameObject fPC, tPC, cC;
    public float camCooldown, mSpeedX, mSpeedY;
    public bool fBool, tBool;
    private float yaw, pitch = 0.0f;

    //Audio
    [Header("Audio")]
    public AudioSource audio;
    public AudioClip running;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
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
        try
        {
            cC = GameObject.Find("Camera Case");
        }
        catch { Debug.LogWarning("I couldn't find a camera case!"); }

        fPC.SetActive(true);
        tPC.SetActive(false);
        tBool = true;
        fBool = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * mSpeedX, 0)));
        //rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(Input.GetAxis("Mouse Y") * -mSpeedY, 0, 0)));
        rig.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * moveSpeed) + (transform.right * Input.GetAxis("Horizontal") * moveSpeed));
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && okToJump && audio.isPlaying == false)
        {
            audio.volume = Random.Range(0.6f, 1);
            audio.PlayOneShot(running);
        }
        if (Input.GetKeyDown(KeyCode.Space) && okToJump)
        {
            rig.AddForce(transform.up * jumpForce);
        }
        yaw += mSpeedX * Input.GetAxis("Mouse X");
        pitch -= mSpeedY * Input.GetAxis("Mouse Y");

        cC.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        switch (Input.GetMouseButtonDown(2) && tBool && camCooldown <= Time.time)
        {
            case true:
                tPC.SetActive(true);
                fPC.SetActive(false);
                camCooldown = Time.time + 2.0f;
                tBool = false;
                fBool = true;
                break;
        }
        switch (Input.GetMouseButtonDown(2) && fBool && camCooldown <= Time.time)
        {
            case true:
                fPC.SetActive(true);
                tPC.SetActive(false);
                camCooldown = Time.time + 2.0f;
                fBool = false;
                tBool = true;
                break;
        }
        switch (Input.GetKeyDown(KeyCode.Escape))
        {
            case true:
                switch (Cursor.lockState)
                {
                    case CursorLockMode.Locked:
                        Cursor.lockState = CursorLockMode.None;
                        break;
                    case CursorLockMode.None:
                        Cursor.lockState = CursorLockMode.Locked;
                        break;
                }
                break;
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        
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
        
    }

}
