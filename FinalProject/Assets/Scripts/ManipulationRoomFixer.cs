using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ManipulationRoomFixer : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Museum/Fix Manipulation Room UI")]
    static void FixManipulationRoomUI()
    {
        SortingManager mgr = Object.FindFirstObjectByType<SortingManager>();
        if (mgr == null) { Debug.LogError("SortingManager not found."); return; }

        GameObject existing = GameObject.Find("ManipulationResultCanvas");
        if (existing != null) DestroyImmediate(existing);

        GameObject canvasObj = new GameObject("ManipulationResultCanvas");
        canvasObj.transform.position = new Vector3(28f, 3f, 0f);
        canvasObj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        canvasObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();

        RectTransform cr = canvasObj.GetComponent<RectTransform>();
        cr.sizeDelta = new Vector2(500f, 150f);

        GameObject bgObj = new GameObject("ResultBackground");
        bgObj.transform.SetParent(canvasObj.transform, false);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(500f, 150f);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.8f);
        bgObj.SetActive(false);

        GameObject textObj = new GameObject("ResultText");
        textObj.transform.SetParent(canvasObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(490f, 140f);
        TMP_Text resultText = textObj.AddComponent<TextMeshProUGUI>();
        resultText.fontSize = 36;
        resultText.alignment = TextAlignmentOptions.Center;
        resultText.color = Color.white;
        resultText.enabled = false;

        mgr.gameOverText = resultText;
        mgr.gameOverBackground = bgObj;
        EditorUtility.SetDirty(mgr);
        EditorUtility.SetDirty(canvasObj);

        Canvas descCanvas = null;
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (c.gameObject.name == "Canvas (4)")
            { descCanvas = c; break; }
        }
        if (descCanvas != null)
        {
            descCanvas.transform.position = new Vector3(27f, 2.5f, 0f);
            descCanvas.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
            EditorUtility.SetDirty(descCanvas.gameObject);
        }

        Debug.Log("Manipulation room UI fixed and SortingManager wired.");
    }
#endif
}
