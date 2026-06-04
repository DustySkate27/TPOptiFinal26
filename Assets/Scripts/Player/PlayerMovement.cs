using UnityEngine;

public class PlayerMovement
{
    private Transform playerTransform;
    private Rigidbody rb;
    private Transform cameraTransform;
    private float velocity;

    public PlayerMovement(Transform transform, Rigidbody rb, Transform cameraTransform, float velocity)
    {
        playerTransform = transform;
        this.rb = rb;
        this.cameraTransform = cameraTransform;
        this.velocity = velocity; 
    }
    
    public void Move(Vector3 dir)
    {
        rb.linearVelocity = (dir * velocity * Time.deltaTime);
        //rb.AddForce(dir * velocity * Time.deltaTime, ForceMode.Force);
    }

    public void MoveCamera(Vector2 dir)
    {
        playerTransform.rotation = Quaternion.Euler(0, dir.y, 0f);
        cameraTransform.rotation = Quaternion.Euler(dir.x, dir.y, 0);
    }

}
