using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject mainButtonsPanel;
    public GameObject addObjectPanel;
    public Transform followAnchor;
    public float distance = 1.2f;
    public InputActionReference vrToggleAction;

    bool isPaused;
    Canvas myCanvas;
    InputAction runtimeVrAction;
    readonly List<GameObject> hiddenCanvases = new();

    void OnEnable()
    {
        if (vrToggleAction != null && vrToggleAction.action != null)
        {
            vrToggleAction.action.performed += OnVrToggle;
            vrToggleAction.action.Enable();
        }
        else
        {
            runtimeVrAction = new InputAction("PauseToggle", InputActionType.Button);
            runtimeVrAction.AddBinding("<XRController>{LeftHand}/menuButton");
            runtimeVrAction.AddBinding("<XRController>{LeftHand}/menu");
            runtimeVrAction.AddBinding("<XRController>{RightHand}/secondaryButton");
            runtimeVrAction.AddBinding("<OculusTouchController>{LeftHand}/start");
            runtimeVrAction.AddBinding("<OculusTouchController>{RightHand}/secondaryButton");
            runtimeVrAction.performed += OnVrToggle;
            runtimeVrAction.Enable();
        }
    }

    void OnDisable()
    {
        if (vrToggleAction != null && vrToggleAction.action != null)
            vrToggleAction.action.performed -= OnVrToggle;
        if (runtimeVrAction != null)
        {
            runtimeVrAction.performed -= OnVrToggle;
            runtimeVrAction.Disable();
            runtimeVrAction.Dispose();
            runtimeVrAction = null;
        }
    }

    void OnVrToggle(InputAction.CallbackContext _)
    {
        if (isPaused) Resume(); else Pause();
    }

    void Start()
    {
        myCanvas = GetComponent<Canvas>();
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume(); else Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (addObjectPanel != null) addObjectPanel.SetActive(false);

        Transform anchor = followAnchor != null ? followAnchor : (Camera.main != null ? Camera.main.transform : null);
        if (anchor != null)
        {
            var t = transform;
            t.position = anchor.position + anchor.forward * distance;
            t.rotation = Quaternion.LookRotation(t.position - anchor.position);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        hiddenCanvases.Clear();
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (c == myCanvas || !c.gameObject.activeSelf) continue;
            if (myCanvas != null && c.transform.IsChildOf(myCanvas.transform)) continue;
            hiddenCanvases.Add(c.gameObject);
            c.gameObject.SetActive(false);
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (addObjectPanel != null) addObjectPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var go in hiddenCanvases)
            if (go != null) go.SetActive(true);
        hiddenCanvases.Clear();
    }

    public void ShowAddObjectMenu()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (addObjectPanel != null) addObjectPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        if (addObjectPanel != null) addObjectPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
