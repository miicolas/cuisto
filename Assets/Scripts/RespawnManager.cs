using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public float fallThresholdY = -10f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private CharacterController controller;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (transform.position.y < fallThresholdY)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (controller != null) controller.enabled = false;
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        if (controller != null) controller.enabled = true;
    }
}
