using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Reflection;

public class GrabbedObjectInteraction : MonoBehaviour
{
    public float rotationStepDegrees = 45f;
    public GameObject spawnPanel;
    public GameObject grabPanel;
    public float releaseDelay = 2f;

    GameObject target;
    float lostHoverAt = -1f;
    bool wasTargeting;

    void Start()
    {
        UnblockNearFarUI();
        if (spawnPanel != null) spawnPanel.SetActive(true);
        if (grabPanel != null) grabPanel.SetActive(false);
    }

    void UnblockNearFarUI()
    {
        var f = typeof(NearFarInteractor).GetField("m_BlockUIOnInteractableSelection", BindingFlags.NonPublic | BindingFlags.Instance);
        if (f == null) return;
        foreach (var r in Object.FindObjectsByType<NearFarInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            f.SetValue(r, false);
    }

    void Update()
    {
        XRGrabInteractable hit = null;
        foreach (var g in Object.FindObjectsByType<XRGrabInteractable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (g.isHovered || g.isSelected) { hit = g; break; }
        }

        if (hit != null)
        {
            target = hit.gameObject;
            lostHoverAt = -1f;
        }
        else if (target != null)
        {
            if (lostHoverAt < 0f) lostHoverAt = Time.time;
            if (Time.time - lostHoverAt > releaseDelay) target = null;
        }

        bool targeting = target != null;
        if (targeting != wasTargeting)
        {
            if (spawnPanel != null) spawnPanel.SetActive(!targeting);
            if (grabPanel != null) grabPanel.SetActive(targeting);
            wasTargeting = targeting;
        }
    }

    public void RotateCurrent()
    {
        if (target == null) return;
        target.transform.Rotate(Vector3.up, rotationStepDegrees, Space.World);
        var e = target.transform.eulerAngles;
        target.transform.eulerAngles = new Vector3(0f, e.y, 0f);
        lostHoverAt = Time.time;
    }

    public void DeleteCurrent()
    {
        if (target == null) return;
        Destroy(target);
        target = null;
        lostHoverAt = -1f;
    }
}
