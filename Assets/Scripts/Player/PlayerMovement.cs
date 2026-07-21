using UnityEngine;

public class PlayerMovement
{
    private Transform playerTransform;
    private Rigidbody rb;
    private Transform cameraTransform;
    private float velocity;

    private float horizontalInput, verticalInput;
    private float xRotation, yRotation;
    private float sensitivity = 400f;

    public PlayerMovement(Transform transform, Rigidbody rb, Transform cameraTransform, float velocity)
    {
        playerTransform = transform;
        this.rb = rb;
        this.cameraTransform = cameraTransform;
        this.velocity = velocity; 
    }
    
    public void Move()
    {
        Vector3 dir = MoveInputDirection();
        rb.linearVelocity = (dir * velocity);
        cameraTransform.position = playerTransform.position;
    }

    private Vector3 MoveInputDirection()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 dir = playerTransform.forward * verticalInput + playerTransform.right * horizontalInput;

        return dir;
    }

    public void MoveCamera()
    {
        Vector2 dir = MoveCameraDirection();
        playerTransform.rotation = Quaternion.Euler(0, dir.y, 0f);
        cameraTransform.rotation = Quaternion.Euler(dir.x, dir.y, 0);
    }

    private Vector3 MoveCameraDirection()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector2 dir = new Vector2(xRotation, yRotation);

        return dir;
    }
}
