using UnityEngine;

public class HandGrabber : MonoBehaviour
{
    [Tooltip("Left or Right controller.")]
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;

    [Tooltip("Radius used to detect grabbable objects around this hand.")]
    public float grabRadius = 0.12f;

    [Tooltip("Layers considered for grabbing.")]
    public LayerMask grabbableLayers = ~0;

    [Tooltip("Button used to grip objects.")]
    public OVRInput.Axis1D gripAxis = OVRInput.Axis1D.PrimaryHandTrigger;

    [Range(0.1f, 1f)] public float gripThreshold = 0.55f;

    public GrabbableObject Held => held;

    public bool DestroyHeld()
    {
        if (held == null) return false;
        held.ForceRelease();
        Destroy(held.gameObject);
        held = null;
        return true;
    }

    GrabbableObject held;
    Vector3 lastPos;
    Quaternion lastRot;

    void Update()
    {
        float grip = OVRInput.Get(gripAxis, controller);

        if (held == null && grip >= gripThreshold)
            TryGrab();
        else if (held != null && grip < gripThreshold)
            ReleaseHeld();

        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    void TryGrab()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius, grabbableLayers, QueryTriggerInteraction.Ignore);
        float best = float.MaxValue;
        GrabbableObject target = null;

        foreach (var c in hits)
        {
            var g = c.GetComponentInParent<GrabbableObject>();
            if (g == null || g.IsHeld) continue;
            float d = (c.transform.position - transform.position).sqrMagnitude;
            if (d < best) { best = d; target = g; }
        }

        if (target != null)
        {
            held = target;
            held.Grab(transform);
        }
    }

    void ReleaseHeld()
    {
        Vector3 linVel = (transform.position - lastPos) / Mathf.Max(Time.deltaTime, 1e-5f);
        Quaternion dq = transform.rotation * Quaternion.Inverse(lastRot);
        dq.ToAngleAxis(out float angleDeg, out Vector3 axis);
        if (angleDeg > 180f) angleDeg -= 360f;
        Vector3 angVel = axis.normalized * (angleDeg * Mathf.Deg2Rad) / Mathf.Max(Time.deltaTime, 1e-5f);

        held.Release(linVel, angVel);
        held = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }
}
