using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrabbableObject : MonoBehaviour
{
    [Tooltip("If true, object uses physics while held (kinematic off on release).")]
    public bool useGravityOnRelease = true;

    Rigidbody rb;
    Transform originalParent;
    bool isHeld;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        originalParent = transform.parent;
    }

    public bool IsHeld => isHeld;

    public void ForceRelease()
    {
        isHeld = false;
        transform.SetParent(originalParent, true);
        if (rb != null) rb.isKinematic = false;
    }

    public void Grab(Transform hand)
    {
        isHeld = true;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        transform.SetParent(hand, true);
    }

    public void Release(Vector3 velocity, Vector3 angularVelocity)
    {
        isHeld = false;
        transform.SetParent(originalParent, true);
        rb.isKinematic = false;
        rb.useGravity = useGravityOnRelease;
        rb.linearVelocity = velocity;
        rb.angularVelocity = angularVelocity;
    }
}
