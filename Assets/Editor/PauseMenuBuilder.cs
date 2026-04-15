using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public static class PauseMenuBuilder
{
    [MenuItem("Tools/Build Pause Menu")]
    public static void Build()
    {
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }

        var canvasGO = new GameObject("PauseMenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(PauseMenu));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create PauseMenuCanvas");
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);

        var panelGO = new GameObject("PausePanel", typeof(Image));
        panelGO.transform.SetParent(canvasGO.transform, false);
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        panelGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);

        var title = CreateText(panelGO.transform, "Title", "PAUSE", 60, new Vector2(0.5f, 0.8f));

        var pm = canvasGO.GetComponent<PauseMenu>();
        pm.pausePanel = panelGO;

        CreateButton(panelGO.transform, "ResumeButton", "Reprendre", new Vector2(0.5f, 0.6f), () => pm.Resume());
        CreateButton(panelGO.transform, "RestartButton", "Recommencer", new Vector2(0.5f, 0.45f), () => pm.Restart());
        CreateButton(panelGO.transform, "QuitButton", "Quitter", new Vector2(0.5f, 0.3f), () => pm.Quit());

        Selection.activeGameObject = canvasGO;
        Debug.Log("Pause Menu created. Assign PauseMenuCanvas in scene. Press Escape in Play mode.");
    }

    private static GameObject CreateText(Transform parent, string name, string text, int fontSize, Vector2 anchor)
    {
        var go = new GameObject(name, typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600, 100);
        rt.anchoredPosition = Vector2.zero;
        var t = go.GetComponent<Text>();
        t.text = text;
        t.fontSize = fontSize;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return go;
    }

    private static void CreateButton(Transform parent, string name, string label, Vector2 anchor, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject(name, typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(400, 80);
        rt.anchoredPosition = Vector2.zero;
        go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        go.GetComponent<Button>().onClick.AddListener(action);
        CreateText(go.transform, "Label", label, 32, new Vector2(0.5f, 0.5f));
    }
}
