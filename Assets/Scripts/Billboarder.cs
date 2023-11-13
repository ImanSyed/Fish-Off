using UnityEngine;

public class Billboarder : MonoBehaviour
{
    Camera mainCamera;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        transform.rotation = mainCamera.transform.rotation;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(Vector3.Distance(transform.position, mainCamera.transform.position) * -100);
    }
}
