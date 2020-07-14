using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pMCTESTING : MonoBehaviour
{
    //Player Controller
    [Header("Player Movement")]

    public GameObject player;
    public CapsuleCollider playerHB;
    public Rigidbody rig;
    public float moveSpeed, airstrafeSpeed, jumpForce, steepestAngle, jumpCoolDown;
    public bool okToJump, okToDoubleJump, piloting;
    private Vector3 moveVec = Vector3.zero, airstrafeVec = Vector3.zero;
    private float groundAngle;
    private int groundedLoop;

    //Shooting
    [Header("Combat")]
    public GameObject bulletHoles;
    public Ray rayOrigin;
    public RaycastHit hit;

    //Wallrunning
    [Header("Wallrunning")]
    public float maxRCDistance;
    public float maxRCRadius, overlapSphereOffset, hoverFloat, jumpBuffer;
    public bool isWallR, isWallL, okToWallJump, okToWallRun;
    private RaycastHit hitL, hitR, hitB, hitF;

    private Vector3 closestVector3;
    private Collider closestCollision;
    private int jumpCount = 0;
    float currentDistance;

    //Camera Controller
   [Header("Camera Controller")]

    public GameObject fPC;
    public GameObject tPC, cC;
    public float camCooldown, mSpeedX, mSpeedY;
    public bool fBool, tBool;
    private float yaw, pitch = 0.0f;

    //Audio
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip running;
    public float runningPitch;
    public AudioClip wallRunning;
    public float wallRunningPitch;

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
            //cC = GameObject.Find("Head");
            cC = GameObject.Find("Camera Case");
        }
        catch
        {
            Debug.LogWarning("I couldn't find a camera case!");
        }

        fPC.SetActive(true);
        tPC.SetActive(false);
        piloting = false;
        tBool = true;
        fBool = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (!piloting)
        {
            switch (okToJump || okToWallJump)
            {
                case true:

                    if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                    {
                        moveVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
                        //moveVec = new Vector3((transform.right * Input.GetAxisRaw("Horizontal")), rig.velocity.y, (transform.forward * Input.GetAxisRaw("Vertical")))
                        moveVec = new Vector3(moveVec.x * moveSpeed * Time.deltaTime, rig.velocity.y, moveVec.z * moveSpeed * Time.deltaTime);
                        //rig.velocity = moveVec * moveSpeed * Time.deltaTime;
                        rig.velocity = moveVec;
                    }
                    else
                    {
                        rig.velocity *= .8f;
                    }

                    break;
                case false:
                    airstrafeVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
                    rig.AddForce(airstrafeVec * airstrafeSpeed * Time.deltaTime);


                    //rig.velocity = airstrafeVec * airstrafeSpeed * Time.deltaTime;
                    break;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!piloting)
        {
            rig.MoveRotation(rig.rotation * Quaternion.Euler(new Vector3(0, Input.GetAxis("Mouse X") * mSpeedX, 0)));

            rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Physics.Raycast(cC.transform.position, cC.transform.forward, out hit);

            //Physics.SphereCast(player.transform.position, maxRCRadius, -player.transform.up, out hit, maxRCDistance);

            //Physics.SphereCast(player.transform.position, maxRCDistance, -player.transform.up, out hit);

            Physics.Raycast(player.transform.position, -player.transform.up, out hit, maxRCDistance);
            groundAngle = Vector3.Angle(player.transform.up, hit.normal);
            okToJump = steepestAngle >= groundAngle && hit.transform != null;

            if (okToJump && groundedLoop == 0)
            {
                okToDoubleJump = true;
                //rig.velocity = new Vector3(0,rig.velocity.y,0);
                groundedLoop++;
            }
            else if (!okToJump && groundedLoop > 0)
            {
                groundedLoop = 0;
            }

            if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && okToJump && audioSource.isPlaying == false)
            {
                audioSource.pitch = runningPitch;
                audioSource.volume = Random.Range(0.6f, 1);
                audioSource.PlayOneShot(running);
            }

            yaw += mSpeedX * Input.GetAxis("Mouse X");
            pitch -= mSpeedY * Input.GetAxis("Mouse Y");

            cC.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

            switch (Input.GetMouseButtonDown(2) && tBool && camCooldown <= Time.time)
            {
                case true:
                    tPC.SetActive(true);
                    fPC.SetActive(false);
                    camCooldown = Time.time + 0.5f;
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

            if (Input.GetKey(KeyCode.E))
            {
                if (Physics.Raycast(rayOrigin, out hit, maxRCDistance))
                {
                    if (hit.collider.tag == "Chassis")
                    {
                        Debug.Log("Successful Chassis Detection");
                        hit.collider.GetComponent<Chassis>().PilotEmbarking(this.gameObject);

                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Physics.Raycast(rayOrigin, out hit))
                {
                    if (hit.collider.tag != null)
                    {
                        Instantiate(bulletHoles, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }
            }

            switch (Input.GetKeyDown(KeyCode.Space) && okToJump)
            {
                case true:
                    rig.AddForce(transform.up * jumpForce);
                    //switch (Input.GetKey(KeyCode.W))
                    //{
                    //    case true:
                    //        rig.AddForce(transform.forward * jumpForce);
                    //        break;
                    //}
                    //switch (Input.GetKey(KeyCode.S))
                    //{
                    //    case true:
                    //        rig.AddForce(-transform.forward * jumpForce);
                    //        break;
                    //}
                    //switch (Input.GetKey(KeyCode.A))
                    //{
                    //    case true:
                    //        rig.AddForce(-transform.right * jumpForce);
                    //        break;
                    //}
                    //switch (Input.GetKey(KeyCode.D))
                    //{
                    //    case true:
                    //        rig.AddForce(transform.right * jumpForce);
                    //        break;
                    //}
                    break;
            }
            switch (Input.GetKeyDown(KeyCode.Space) && okToDoubleJump && !okToJump && !okToWallJump)
            {
                case true:
                    if (rig.velocity.y > 0)
                    {
                        rig.AddForce(transform.up * jumpForce);
                    }
                    else
                    {
                        //rig.velocity.y = 0;
                        rig.velocity = new Vector3(rig.velocity.x, 0, rig.velocity.z);
                        rig.AddForce(transform.up * jumpForce);
                    }

                    okToDoubleJump = false;

                    switch (Input.GetKey(KeyCode.W))
                    {
                        case true:
                            rig.AddForce(transform.forward * jumpForce);
                            break;
                    }
                    switch (Input.GetKey(KeyCode.S))
                    {
                        case true:
                            rig.AddForce(-transform.forward * jumpForce);
                            break;
                    }
                    switch (Input.GetKey(KeyCode.A))
                    {
                        case true:
                            rig.AddForce(-transform.right * jumpForce);
                            break;
                    }
                    switch (Input.GetKey(KeyCode.D))
                    {
                        case true:
                            rig.AddForce(transform.right * jumpForce);
                            break;
                    }
                    break;
            }



            switch (jumpBuffer <= Time.time)
            {
                case true:
                    okToWallRun = true;
                    break;
                case false:
                    okToWallRun = false;
                    break;

            }

            if (!okToJump && okToWallRun)
            {
                #region Original Wallrun
                //switch (Physics.Raycast(player.transform.position, player.transform.forward, out hitF, maxRCDistance / 2))
                //{
                //    case true:
                //        if (hitF.transform.tag == "Wall" && okToWallRun)
                //        {
                //            isWallL = false;
                //            isWallR = false;
                //            okToWallJump = true;
                //            okToDoubleJump = true;
                //            //rig.velocity = Vector3.zero;
                //            rig.AddForce(new Vector3(0, hoverFloat, 0f));
                //            switch (!audioSource.isPlaying && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                //            {
                //                case true:
                //                    audioSource.pitch = wallRunningPitch;
                //                    audioSource.volume = Random.Range(0.6f, 1);
                //                    audioSource.PlayOneShot(wallRunning);
                //                    break;
                //                case false:
                //                    break;
                //            }
                //            //rb.useGravity = false;
                //            if (Input.GetKeyDown(KeyCode.Space))
                //            {
                //                rig.AddForce(-player.transform.forward * jumpForce);
                //                rig.AddForce(player.transform.up * jumpForce);
                //                //rig.AddForce(player.transform.forward * jumpForce);
                //                jumpBuffer = Time.time + 0.2f;
                //            }
                //        }
                //        break;
                //    case false:

                //        switch (Physics.Raycast(player.transform.position, -player.transform.forward, out hitB, maxRCDistance / 2))
                //        {
                //            case true:
                //                if (hitB.transform.tag == "Wall" && okToWallRun)
                //                {
                //                    isWallL = true;
                //                    isWallR = false;
                //                    okToWallJump = true;
                //                    okToDoubleJump = true;
                //                    //rig.velocity = Vector3.zero;
                //                    rig.AddForce(new Vector3(0, hoverFloat, 0f));
                //                    switch (!audioSource.isPlaying && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                //                    {
                //                        case true:
                //                            audioSource.pitch = wallRunningPitch;
                //                            audioSource.volume = Random.Range(0.6f, 1);
                //                            audioSource.PlayOneShot(wallRunning);
                //                            break;
                //                        case false:
                //                            break;
                //                    }
                //                    //rb.useGravity = false;
                //                    if (Input.GetKeyDown(KeyCode.Space))
                //                    {
                //                        rig.AddForce(player.transform.forward * jumpForce);
                //                        rig.AddForce(player.transform.up * jumpForce);
                //                        //rig.AddForce(player.transform.forward * jumpForce);
                //                        jumpBuffer = Time.time + 0.2f;
                //                    }
                //                }
                //                break;
                //            case false:
                //                switch (Physics.Raycast(player.transform.position, player.transform.right, out hitR, maxRCDistance / 2))
                //                {
                //                    case true:
                //                        switch (hitR.transform.tag == "Wall" && okToWallRun)
                //                        {
                //                            case true:
                //                                //TODO: add camera rotation while on walls
                //                                isWallR = true;
                //                                isWallL = false;
                //                                okToWallJump = true;
                //                                okToDoubleJump = true;
                //                                //rig.velocity = Vector3.zero;
                //                                rig.AddForce(new Vector3(0, hoverFloat, 0f));
                //                                switch (!audioSource.isPlaying && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                //                                {
                //                                    case true:
                //                                        audioSource.pitch = wallRunningPitch;
                //                                        audioSource.volume = Random.Range(0.6f, 1);
                //                                        audioSource.PlayOneShot(wallRunning);
                //                                        break;
                //                                    case false:
                //                                        break;
                //                                }
                //                                //rb.useGravity = false;
                //                                if (Input.GetKeyDown(KeyCode.Space))
                //                                {
                //                                    rig.AddForce(-player.transform.right * jumpForce);
                //                                    rig.AddForce(player.transform.up * jumpForce);
                //                                    //rig.AddForce(player.transform.forward * jumpForce);
                //                                    jumpBuffer = Time.time + 0.2f;
                //                                }
                //                                break;
                //                        }
                //                        break;
                //                    case false:
                //                        switch (Physics.Raycast(player.transform.position, -player.transform.right, out hitL, maxRCDistance / 2))
                //                        {
                //                            case true:
                //                                if (hitL.transform.tag == "Wall" && okToWallRun)
                //                                {
                //                                    isWallL = true;
                //                                    isWallR = false;
                //                                    okToWallJump = true;
                //                                    okToDoubleJump = true;
                //                                    //rig.velocity = Vector3.zero;
                //                                    rig.AddForce(new Vector3(0, hoverFloat, 0f));
                //                                    switch (!audioSource.isPlaying && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                //                                    {
                //                                        case true:
                //                                            audioSource.pitch = wallRunningPitch;
                //                                            audioSource.volume = Random.Range(0.6f, 1);
                //                                            audioSource.PlayOneShot(wallRunning);
                //                                            break;
                //                                        case false:
                //                                            break;
                //                                    }
                //                                    //rb.useGravity = false;
                //                                    if (Input.GetKeyDown(KeyCode.Space))
                //                                    {
                //                                        rig.AddForce(player.transform.right * jumpForce);
                //                                        rig.AddForce(player.transform.up * jumpForce);
                //                                        //rig.AddForce(player.transform.forward * jumpForce);
                //                                        jumpBuffer = Time.time + 0.2f;
                //                                    }
                //                                }
                //                                break;
                //                            case false:
                //                                isWallL = false;
                //                                isWallR = false;
                //                                okToWallJump = false;
                //                                rig.useGravity = true;
                //                                break;
                //                        }
                //                        break;
                //                }
                //                break;
                //        }
                //        break;
                //}
                #endregion

                #region Spherecast attempt

                Collider[] hitColliders = Physics.OverlapSphere(new Vector3(
                    player.transform.position.x, 
                    player.transform.position.y - overlapSphereOffset, 
                    player.transform.position.z), maxRCRadius, ~LayerMask.GetMask("Ground"));
                int wallCheckLoopCounter = 0;
                float closestDistance = maxRCRadius;
                closestVector3 = Vector3.zero;

                ///<summary>
                /// The overlapSphere is an actual sphere equivalent to a raycast, and doesn't discriminate direction.
                /// This iterates through it and finds the closest position data.
                /// If there isn't any by the end of the loop, then it isn't touching anything.
                /// </summary>
                while (wallCheckLoopCounter < hitColliders.Length)
                {

                    //if(hitColliders[i].tag == ("Wall"))
                    //{
                    //    Debug.Log("Wall Detected");
                    //
                    //}
                    for (int i = 0; i < hitColliders.Length - 1; i++)
                    {
                        currentDistance = Vector3.Distance(player.transform.position, hitColliders[i].transform.position);
                        if (currentDistance / 5 < closestDistance && hitColliders[i].tag == "Wall")
                        {
                            closestVector3 = hitColliders[i].transform.position;
                            closestDistance = Vector3.Distance(player.transform.position, hitColliders[i].transform.position);
                            closestCollision = hitColliders[i];
                            Debug.Log($"\nClosest V3: {closestVector3} ||||| Closest float: {closestDistance}");

                        }

                    }
                    wallCheckLoopCounter++;


                }

                if (closestDistance != maxRCRadius)
                {
                    okToWallJump = true;
                    okToDoubleJump = true;
                    //rig.velocity = Vector3.zero;
                    rig.AddForce(new Vector3(0, hoverFloat, 0f));

                    //rig.rotation = Quaternion.Euler(-hit.point);


                    switch (!audioSource.isPlaying && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
                    {
                        case true:
                            audioSource.pitch = wallRunningPitch;
                            audioSource.volume = Random.Range(0.6f, 1);
                            audioSource.PlayOneShot(wallRunning);
                            break;
                        case false:
                            break;
                    }
                    //rb.useGravity = false;
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        //rig.AddForce(-player.transform.right * jumpForce / 2);
                        //rig.AddForce(-Vector3.MoveTowards(rig.transform.position, closestCollision.transform.position, 0) * jumpForce);

                        //rig.AddForce()
                        //TODO: Calculate the direction the player would jump;
                        Vector3 posComp = Vector3.Normalize( new Vector3(closestCollision.transform.position.x- rig.transform.position.x,
                            player.transform.position.y, closestCollision.transform.position.z - rig.transform.position.z));

                        //moveVec = closestCollision.transform.position.normalized * jumpForce;

                        //rig.AddForce((player.transform.forward + posComp) * jumpForce);

                        ///This is just here for reference, delete it when you're done.
                        //moveVec = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
                        //moveVec = new Vector3(moveVec.x * moveSpeed * Time.deltaTime, rig.velocity.y, moveVec.z * moveSpeed * Time.deltaTime);
                        //rig.velocity = moveVec;
                        ///

                        rig.AddForce(transform.right * (posComp.x * jumpForce) + transform.forward * (posComp.z * jumpForce));
                         
                        rig.AddForce(player.transform.up * jumpForce);
                        //rig.AddForce(player.transform.forward * jumpForce);
                        jumpBuffer = Time.time + jumpCoolDown;
                    }
                }
                else
                {
                    okToWallJump = false;
                    rig.useGravity = true;
                }
               
                #endregion
            }


        }
        else
        {

        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(player.transform.position, maxRCDistance);
    //}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3( 
            player.transform.position.x, 
            player.transform.position.y - overlapSphereOffset, 
            player.transform.position.z), maxRCRadius);
       
    
    }
}
