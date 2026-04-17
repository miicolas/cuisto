using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddObjectMenu : MonoBehaviour
{
    public SpawnManager spawnManager;
    public PauseMenu pauseMenu;
    public Button buttonTemplate;
    public Transform contentRoot;
    public bool resumeAfterSpawn = true;

    readonly List<GameObject> spawnedButtons = new();

    void OnEnable()
    {
        if (spawnManager == null) spawnManager = FindAnyObjectByType<SpawnManager>();
        if (pauseMenu == null) pauseMenu = FindAnyObjectByType<PauseMenu>();
        Rebuild();
    }

    void OnDisable()
    {
        Clear();
    }

    void Clear()
    {
        foreach (var go in spawnedButtons)
            if (go != null) Destroy(go);
        spawnedButtons.Clear();
    }

    void Rebuild()
    {
        Clear();
        if (spawnManager == null || buttonTemplate == null || contentRoot == null) return;

        buttonTemplate.gameObject.SetActive(false);

        for (int i = 0; i < spawnManager.spawnables.Count; i++)
        {
            var entry = spawnManager.spawnables[i];
            if (entry == null || entry.prefab == null) continue;

            Button btn = Instantiate(buttonTemplate, contentRoot);
            btn.gameObject.SetActive(true);
            btn.gameObject.name = $"Btn_{entry.displayName}";

            var label = btn.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = entry.displayName;
            var legacyLabel = btn.GetComponentInChildren<Text>(true);
            if (legacyLabel != null) legacyLabel.text = entry.displayName;

            var iconT = btn.transform.Find("Icon");
            if (iconT != null)
            {
                var iconImg = iconT.GetComponent<Image>();
                if (iconImg != null && entry.icon != null)
                {
                    iconImg.sprite = entry.icon;
                    iconImg.enabled = true;
                }
                else if (iconImg != null)
                {
                    iconImg.enabled = false;
                }
            }

            int capturedIndex = i;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnEntryClicked(capturedIndex));

            spawnedButtons.Add(btn.gameObject);
        }
    }

    void OnEntryClicked(int index)
    {
        if (spawnManager != null) spawnManager.Spawn(index);
        if (resumeAfterSpawn && pauseMenu != null) pauseMenu.Resume();
    }
}
