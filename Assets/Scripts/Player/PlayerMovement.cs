using UnityEngine;

public class PlayerMovement
{
    private Transform playerTransform;
    private Transform cameraTransform;
    private float velocity;

    public PlayerMovement(Transform transform, Transform cameraTransform, float velocity)
    {
        playerTransform = transform;
        this.cameraTransform = cameraTransform;
        this.velocity = velocity; 
    }
    
    public void Move(Vector3 dir)
    {
        playerTransform.Translate(dir * velocity);
    }

    public void MoveCamera(Vector2 dir)
    {
        playerTransform.rotation = Quaternion.Euler(0, dir.y, 0f);
        cameraTransform.rotation = Quaternion.Euler(dir.x, dir.y, 0);
    }

}
