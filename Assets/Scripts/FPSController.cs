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

    [SerializeField] private float health;
    public float maxO2;
    private float currentO2;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private Camera cam;
    private Rigidbody rb;
    public Dictionary<string, int> fishCollection = new Dictionary<string, int>();


    void Start()
    {
        Application.targetFrameRate = 15;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        RefreshO2();
        InitialiseFishCollection();
    }

    private void InitialiseFishCollection()
    {
        fishCollection.Add("Chin-Chin", 0);
        fishCollection.Add("Dst_Byster", 0);
        fishCollection.Add("Eyeguy", 0);
        fishCollection.Add("Fishman", 0);
        fishCollection.Add("Halihustur", 0);
        fishCollection.Add("Longtailedjonrus", 0);
        fishCollection.Add("Seaboneraven", 0);
        fishCollection.Add("Shelpus", 0);
        fishCollection.Add("Shol-gyth", 0);
        fishCollection.Add("Shuk-tukhu", 0);
        fishCollection.Add("Steinclover", 0);
        fishCollection.Add("Steinraus", 0);
        fishCollection.Add("Trapastavoid", 0);
        fishCollection.Add("Wohl-oth", 0);
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
            rb.velocity = moveDirection * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void RefreshO2()
    {
        currentO2 = maxO2;
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

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.collider.CompareTag("Fish") && other.gameObject.GetComponent<FishBehaviour>().myStats.canBeCaught)
        {
            fishCollection.Add(other.gameObject.GetComponent<FishBehaviour>().myStats.fishType, 1);
        }
    }
}