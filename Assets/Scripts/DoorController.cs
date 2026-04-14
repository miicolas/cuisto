using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float pushForce = 45f;
    public float returnSpeed = 30f;
    public float maxAngle = 90f;

    [HideInInspector] public bool isBeingPushed = false;

    private float currentAngle = 0f;

    void Update()
    {
        float targetAngle = isBeingPushed ? maxAngle : 0f;

        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * 3f);
        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);

        isBeingPushed = false;
    }
}