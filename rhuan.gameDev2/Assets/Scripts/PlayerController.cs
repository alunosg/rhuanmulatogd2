using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rig;
    public Transform turret;
    public Transform cannon;
    public Transform bulletPoint;
    public GameObject bulletPrefab;
    public GameObject shootFX;

    public float bulletSpeed = 10;
    public float speed = 10;
    public Vector2 rotationSpeed = new Vector2(10, 10);
    public float minRotationX = -75f;
    public float maxRotationX = 0f;

    private Vector2 moveInput;
    private Vector2 cannonRotation;

    private void FixedUpdate()
    {
        Vector3 vX = Vector3.zero;
        Vector3 vY = rig.linearVelocity.y * transform.up;
        Vector3 vZ = moveInput.y * transform.forward * speed;
        rig.linearVelocity = vX + vY + vZ;

        float rotation = moveInput.x * Time.fixedDeltaTime * speed * 5;
        transform.Rotate(0, rotation, 0);
    } 
    
    public void OnMove(InputAction.CallbackContext context)
    {
       moveInput = context.ReadValue<Vector2>();
    }   

    public void OnLook(InputAction.CallbackContext context) 
    {
       Vector2 lookInput = context.ReadValue<Vector2>();

       cannonRotation.y += lookInput.x * rotationSpeed.y * Time.deltaTime;
       cannonRotation.x += lookInput.y * rotationSpeed.x * Time.deltaTime;

       cannonRotation.x = Mathf.Clamp(cannonRotation.x, minRotationX, maxRotationX);

       cannon.localRotation = Quaternion.Euler(cannonRotation.x, 0f, 0f);
       turret.localRotation = Quaternion.Euler(0f, cannonRotation.y, 0f);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (shootFX) Instantiate(shootFX, bulletPoint.position, bulletPoint.rotation);

            GameObject bullet = Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
            bullet.GetComponent<Rigidbody>().linearVelocity = bulletPoint.forward * bulletSpeed;
        }    
    }        
}                