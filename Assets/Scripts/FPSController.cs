using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{   
    [Header("Movement")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    [HideInInspector] public bool canMove = true;

    [Header("Weapons")]
    [HideInInspector] public bool canShoot = true;
    [SerializeField] private float shrinkRayDistance, suctionRayDistance;
    [SerializeField] private float shrinkRayMagnitude, suctionRayMagnitude;
    [SerializeField] private LayerMask hitLayerMask;
    private FishBehaviour fishTarget;
    public Light flashlight;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private Camera cam;
    private Rigidbody rb;

    void Start()
    {
        Application.targetFrameRate = 15;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (canMove)
        {
            MoveInput();
            LookInput();     
        }

        if(canShoot)
        {  
            if(Input.GetMouseButton(1))
            {
                Shrink();
            }
            else if(Input.GetMouseButton(0))
            {
                Suck();
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    void MoveInput()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedZ = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        
        moveDirection = (cam.transform.forward * curSpeedZ) + (transform.right * curSpeedX);
    }

    void LookInput()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    void FixedUpdate() 
    {
        if(moveDirection != Vector3.zero && canMove)
        {
            transform.position += moveDirection * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void Shrink()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, cam.transform.forward, out hit, shrinkRayDistance, hitLayerMask))
        {
            fishTarget = hit.transform.parent.GetComponent<FishBehaviour>();
            fishTarget.ShrinkMe(shrinkRayMagnitude);
        }
        
    }

    void Suck()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, cam.transform.forward, out hit, suctionRayDistance, hitLayerMask))
        {
            fishTarget = hit.transform.parent.GetComponent<FishBehaviour>();
            fishTarget.SuckMe(suctionRayMagnitude);
        }
    }

}