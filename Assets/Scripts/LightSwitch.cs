using UnityEngine;
using UnityEngine.InputSystem; // ← nouveau

public class LightSwitch : MonoBehaviour
{
    public Light myLight;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame) 
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                if (hit.collider.gameObject == this.gameObject)
                    myLight.enabled = !myLight.enabled;
            }
        }
    }
}