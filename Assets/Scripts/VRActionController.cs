using UnityEngine;

public class VRActionController : MonoBehaviour
{
    public SpawnManager spawnManager;
    public HandGrabber leftGrabber;
    public HandGrabber rightGrabber;

    public OVRInput.Button spawnTableButton = OVRInput.Button.One;   // A (right)
    public OVRInput.Button deleteButton = OVRInput.Button.Two;       // B (right)
    public OVRInput.Button spawnChairButton = OVRInput.Button.Three; // X (left)
    public OVRInput.Button spawnDecoButton = OVRInput.Button.Four;   // Y (left)

    void Update()
    {
        if (spawnManager != null)
        {
            if (OVRInput.GetDown(spawnTableButton)) spawnManager.SpawnTable();
            if (OVRInput.GetDown(spawnChairButton)) spawnManager.SpawnChair();
            if (OVRInput.GetDown(spawnDecoButton))  spawnManager.SpawnDeco();
        }

        if (OVRInput.GetDown(deleteButton))
        {
            if (rightGrabber != null && rightGrabber.DestroyHeld()) return;
            if (leftGrabber != null) leftGrabber.DestroyHeld();
        }
    }
}
