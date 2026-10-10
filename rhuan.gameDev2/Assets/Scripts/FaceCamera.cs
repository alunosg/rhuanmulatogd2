using UnityEngine;

public class FaceCamaera : MonoBehaviour
{
    public float baseScale = 0.1f;
    private Camera mainCamera;
    private Vector3 startScale;

    void Start()
    {
        mainCamera = Camera.main;
        startScale = transform.localScale;
    }

   
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
        transform.localScale = startScale * (baseScale *
            Vector3.Distance(transform.position, mainCamera.transform.position));
    }
}
