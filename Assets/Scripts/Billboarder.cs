using UnityEngine;

public class Billboarder : MonoBehaviour
{
    Camera mainCamera;
    SpriteRenderer spriteRenderer;
    [SerializeField] private bool billboard, sorting;

    void Start()
    {
        mainCamera = Camera.main;
        if(sorting)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if(billboard && transform.rotation != mainCamera.transform.rotation)
        {
            transform.rotation = mainCamera.transform.rotation;
        }
        if(sorting)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(Vector3.Distance(transform.position, mainCamera.transform.position) * -100);
        }
    }
}