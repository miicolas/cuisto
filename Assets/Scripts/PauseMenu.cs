using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public Transform followAnchor;
    public float distance = 1.2f;

    public OVRInput.Button vrToggleButton = OVRInput.Button.Start;

    bool isPaused;
    GraphicRaycaster myRaycaster;
    readonly List<GraphicRaycaster> disabledRaycasters = new();

    void Start()
    {
        myRaycaster = GetComponent<GraphicRaycaster>();
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        bool esc = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool vrBtn = OVRInput.GetDown(vrToggleButton);
        if (esc || vrBtn)
        {
            if (isPaused) Resume(); else Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (followAnchor != null)
        {
            var t = transform;
            t.position = followAnchor.position + followAnchor.forward * distance;
            t.rotation = Quaternion.LookRotation(t.position - followAnchor.position);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        disabledRaycasters.Clear();
        foreach (var gr in Object.FindObjectsByType<GraphicRaycaster>(FindObjectsSortMode.None))
        {
            if (gr == myRaycaster || !gr.enabled) continue;
            gr.enabled = false;
            disabledRaycasters.Add(gr);
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var gr in disabledRaycasters)
            if (gr != null) gr.enabled = true;
        disabledRaycasters.Clear();
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
