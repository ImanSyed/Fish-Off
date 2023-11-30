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
    [SerializeField] float waterYPoint, diveDistance;
    [HideInInspector] public bool canMove = true;

    [Header("Weapons")]
    [HideInInspector] public bool canShoot = true;
    [SerializeField] private float shrinkRayDistance, suctionRayDistance;
    [SerializeField] private float shrinkRayMagnitude, suctionRayMagnitude;
    [SerializeField] private GameObject shrinkVFX, suctionVFX;
    [SerializeField] private LayerMask hitLayerMask;
    private FishBehaviour fishTarget;
    public Light flashlight;

    public float maxO2;
    [SerializeField] float o2DecreaseRate;
    private float currentO2;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private Camera cam;
    private Rigidbody rb;
    public Dictionary<string, int> fishCollection = new Dictionary<string, int>();
    private ShopUI shopUI;


    void Start()
    {
        Application.targetFrameRate = 24;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = Camera.main;
        shopUI = FindAnyObjectByType<ShopUI>();
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
                if(!shrinkVFX.activeInHierarchy)
                {
                    shrinkVFX.SetActive(true);
                    suctionVFX.SetActive(false);
                }
                Shrink();
            }
            else if(Input.GetMouseButton(0))
            {
                if(!shrinkVFX.activeInHierarchy)
                {
                    shrinkVFX.SetActive(false);
                    suctionVFX.SetActive(true);
                }
                Suck();
            }

            if(!Input.GetMouseButton(1) && shrinkVFX.activeInHierarchy)
            {
                shrinkVFX.SetActive(false);
            }

            if(!Input.GetMouseButton(0) && suctionVFX.activeInHierarchy)
            {
                suctionVFX.SetActive(false);
            }

        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    void MoveInput()
    {
        currentO2 -= o2DecreaseRate;
        if(currentO2 <= 0)
        {
            //GAME OVER
            return;
        }

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

        if(transform.position.y > waterYPoint)
        {
            canMove = false;
            canShoot = false;
            shopUI.ShowShop(true);
        }
    }

    public void Dive()
    {
        Vector3 divePosition = transform.position;

        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, diveDistance))
        {
            divePosition.y = hit.point.y + 10; 
        }
        else
        {
            divePosition.y -= diveDistance;
        }

        transform.position = divePosition;

        canMove = true;
        canShoot = true;
    }

    public void RefreshO2()
    {
        currentO2 = maxO2;
    }

    void Shrink()
    {
        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, shrinkRayDistance, hitLayerMask))
        {
            fishTarget = hit.transform.GetComponent<FishBehaviour>();
            fishTarget.ShrinkMe(shrinkRayMagnitude);
            Debug.Log(fishTarget.gameObject.name);
        }
    }

    void Suck()
    {
        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, suctionRayDistance, hitLayerMask))
        {
            fishTarget = hit.transform.GetComponent<FishBehaviour>();
            fishTarget.SuckMe(suctionRayMagnitude);
            Debug.Log(fishTarget.gameObject.name);
        }
    }

    public void TakeDamage(float damage)
    {
        currentO2 -= damage;
        if(currentO2 <= 0)
        {
            //GAME OVER
        }
    }


    private void OnCollisionEnter(Collision other) 
    {
        if(other.collider.CompareTag("Fish"))
        {
            if(other.transform.parent.gameObject.GetComponent<FishBehaviour>().myStats.canBeCaught)
            {
                fishCollection.Add(other.gameObject.GetComponent<FishBehaviour>().myStats.fishType, 1);
            }
        }
    }
}