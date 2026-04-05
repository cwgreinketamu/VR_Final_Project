using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrottoFixer : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Grotto/4 - Fix Grotto References And UI")]
    static void FixAll()
    {
        FixGrottoResultBackground();
        FixGrottoCanvasPosition();
        ReWireGrottoManager();
        Debug.Log("Grotto fixes applied.");
    }

    static void FixGrottoResultBackground()
    {
        GameObject canvasObj = GameObject.Find("GrottoCanvas");
        if (canvasObj == null)
        {
            Debug.LogError("GrottoCanvas not found.");
            return;
        }

        Transform existingBg = canvasObj.transform.Find("GrottoResultBackground");
        if (existingBg != null) DestroyImmediate(existingBg.gameObject);

        GameObject bgObj = new GameObject("GrottoResultBackground");
        bgObj.transform.SetParent(canvasObj.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(400f, 120f);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.8f);
        bgObj.SetActive(false);

        EditorUtility.SetDirty(canvasObj);
        EditorUtility.SetDirty(bgObj);
    }

    static void FixGrottoCanvasPosition()
    {
        GameObject canvasObj = GameObject.Find("GrottoCanvas");
        if (canvasObj == null) return;

        canvasObj.transform.localPosition = new Vector3(0f, 2.5f, 2f);
        canvasObj.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        EditorUtility.SetDirty(canvasObj);
    }

    static void ReWireGrottoManager()
    {
        GrottoGameManager mgr = Object.FindFirstObjectByType<GrottoGameManager>();
        if (mgr == null)
        {
            Debug.LogError("GrottoGameManager not found.");
            return;
        }

        GameObject canvasObj = GameObject.Find("GrottoCanvas");
        if (canvasObj != null)
        {
            TMP_Text instrText = canvasObj.transform.Find("InstructionText")?.GetComponent<TMP_Text>();
            TMP_Text timerText = canvasObj.transform.Find("GrottoTimerText")?.GetComponent<TMP_Text>();
            TMP_Text resultText = canvasObj.transform.Find("GrottoResultText")?.GetComponent<TMP_Text>();
            GameObject resultBg = canvasObj.transform.Find("GrottoResultBackground")?.gameObject;

            if (instrText != null) mgr.instructionText = instrText;
            if (timerText != null) mgr.timerText = timerText;
            if (resultText != null) mgr.resultText = resultText;
            if (resultBg != null) mgr.resultBackground = resultBg;
        }

        WaterStreamHaptic[] streams = Object.FindObjectsByType<WaterStreamHaptic>(FindObjectsSortMode.None);
        mgr.waterStreams = streams;
        mgr.correctStream = null;
        foreach (var s in streams)
        {
            if (s.intensity == WaterStreamHaptic.StreamIntensity.Strong)
                mgr.correctStream = s;
        }

        mgr.blindObjects = Object.FindObjectsByType<HapticObject>(FindObjectsSortMode.None);
        mgr.crystals = Object.FindObjectsByType<CrystalHuntTarget>(FindObjectsSortMode.None);

        GameObject xrOrigin = GameObject.Find("Custom XR Origin");
        if (xrOrigin != null) mgr.playerTransform = xrOrigin.transform;

        EditorUtility.SetDirty(mgr);
        Debug.Log("GrottoGameManager re-wired. Streams: " + streams.Length + ", Crystals: " + mgr.crystals.Length);
    }

    [MenuItem("Tools/Museum/Add Grotto Sign")]
    static void AddGrottoSign()
    {
        GameObject existing = GameObject.Find("GrottoSignCanvas");
        if (existing != null) DestroyImmediate(existing);

        GameObject canvasObj = new GameObject("GrottoSignCanvas");
        canvasObj.transform.position = new Vector3(-5f, 2.5f, -9f);
        canvasObj.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        canvasObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(500f, 120f);

        GameObject bgObj = new GameObject("SignBackground");
        bgObj.transform.SetParent(canvasObj.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(500f, 120f);
        UnityEngine.UI.Image bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0.05f, 0.04f, 0.12f, 0.9f);

        GameObject textObj = new GameObject("SignText");
        textObj.transform.SetParent(canvasObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(490f, 110f);
        TMP_Text signText = textObj.AddComponent<TextMeshProUGUI>();
        signText.text = "Waterfall Grotto  \u2192";
        signText.fontSize = 42;
        signText.alignment = TextAlignmentOptions.Center;
        signText.color = new Color(0.4f, 0.9f, 1f);

        EditorUtility.SetDirty(canvasObj);
        Debug.Log("Grotto sign created at corridor entrance.");
    }

    [MenuItem("Tools/Museum/Fix Navigation Timer Text")]
    static void FixNavTimerText()
    {
        GameObject timerGO = GameObject.Find("GameTimerManager");
        if (timerGO == null)
        {
            Debug.LogError("GameTimerManager not found.");
            return;
        }

        GameTimerManager mgr = timerGO.GetComponent<GameTimerManager>();
        if (mgr == null) return;

        if (mgr.timerText != null)
        {
            Debug.Log("GameTimerManager timerText already assigned: " + mgr.timerText.gameObject.name);
            return;
        }

        TMP_Text[] allTexts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in allTexts)
        {
            string n = t.gameObject.name;
            if ((n.Contains("Timer") || n == "TimerText") && !n.Contains("Grotto"))
            {
                mgr.timerText = t;
                EditorUtility.SetDirty(timerGO);
                Debug.Log("GameTimerManager timerText assigned to: " + t.gameObject.name);
                return;
            }
        }
        Debug.LogWarning("No matching TimerText found for GameTimerManager — assign it manually in the Inspector.");
    }
#endif
}
