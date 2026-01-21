using UnityEngine;

public class CameraZoneController : MonoBehaviour
{
    public Transform player;
    public float screenHeight = 18f;
    public float smoothSpeed = 3f;

    private int currentZone;
    private Vector3 targetPosition;

    void Start()
    {
        currentZone = Mathf.FloorToInt(player.position.y / screenHeight);
        SetTargetPosition();
        transform.position = targetPosition;
    }

    void LateUpdate()
    {
        int playerZone = Mathf.FloorToInt(player.position.y / screenHeight);

        if (playerZone != currentZone)
        {
            currentZone = playerZone;
            SetTargetPosition();
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * smoothSpeed
        );
    }

    void SetTargetPosition()
    {
        targetPosition = new Vector3(
            transform.position.x,
            currentZone * screenHeight + screenHeight / 2f,
            transform.position.z
        );
    }
}