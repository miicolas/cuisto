using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Camera cam;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Déplacement seulement si le curseur est caché
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float x = 0f, z = 0f;
            if (Keyboard.current.dKey.isPressed) x = 1f;
            if (Keyboard.current.qKey.isPressed) x = -1f;
            if (Keyboard.current.zKey.isPressed) z = 1f;
            if (Keyboard.current.sKey.isPressed) z = -1f;

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);

            float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * 0.1f;
            float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * 0.1f;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            if (cam != null)
                cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        // Tab pour ouvrir/fermer le menu
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}