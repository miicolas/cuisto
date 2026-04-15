using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VRUIPointer : MonoBehaviour
{
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public OVRInput.Button clickButton = OVRInput.Button.PrimaryIndexTrigger;
    public float maxDistance = 10f;
    public LineRenderer line;
    public Color idleColor = new Color(0.2f, 0.8f, 1f, 0.6f);
    public Color hoverColor = new Color(1f, 0.9f, 0.2f, 0.9f);

    GameObject lastHover;
    PointerEventData pointerData;

    void Awake()
    {
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.widthMultiplier = 0.005f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.positionCount = 2;
            line.useWorldSpace = true;
        }
        if (EventSystem.current == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
        pointerData = new PointerEventData(EventSystem.current);
    }

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 dir = transform.forward;
        Vector3 end = origin + dir * maxDistance;

        GameObject hover = null;
        float bestDist = maxDistance;

        foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (canvas.renderMode != RenderMode.WorldSpace) continue;
            var gr = canvas.GetComponent<GraphicRaycaster>();
            if (gr == null || !gr.enabled) continue;

            var plane = new Plane(-canvas.transform.forward, canvas.transform.position);
            if (!plane.Raycast(new Ray(origin, dir), out float enter)) continue;
            if (enter > bestDist) continue;

            Vector3 worldPoint = origin + dir * enter;
            Vector2 screenPt = Camera.main != null
                ? (Vector2)Camera.main.WorldToScreenPoint(worldPoint)
                : (Vector2)worldPoint;

            pointerData.position = screenPt;
            var results = new List<RaycastResult>();
            gr.Raycast(pointerData, results);
            if (results.Count > 0)
            {
                hover = results[0].gameObject;
                bestDist = enter;
                end = worldPoint;
            }
        }

        line.SetPosition(0, origin);
        line.SetPosition(1, end);
        line.startColor = line.endColor = hover != null ? hoverColor : idleColor;

        if (hover != lastHover)
        {
            if (lastHover != null) ExecuteEvents.Execute(lastHover, pointerData, ExecuteEvents.pointerExitHandler);
            if (hover != null) ExecuteEvents.Execute(hover, pointerData, ExecuteEvents.pointerEnterHandler);
            lastHover = hover;
        }

        if (hover != null && OVRInput.GetDown(clickButton, controller))
        {
            var btn = hover.GetComponentInParent<Button>();
            if (btn != null) btn.onClick.Invoke();
        }
    }
}
