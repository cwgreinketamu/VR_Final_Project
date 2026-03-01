using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MinimapSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Setup Player Arrow Icon")]
    static void SetupArrow()
    {
        TextureImporter importer = AssetImporter.GetAtPath("Assets/Materials/PlayerArrow.png") as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 32;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
            Debug.Log("Reimported as Sprite. Run this menu item AGAIN to assign.");
            return;
        }

        GameObject icon = GameObject.Find("PlayerIcon");
        if (icon == null)
        {
            Debug.LogError("PlayerIcon not found");
            return;
        }

        Image img = icon.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("No Image component on PlayerIcon");
            return;
        }

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("Assets/Materials/PlayerArrow.png");
        Sprite arrow = null;
        foreach (Object a in assets)
        {
            if (a is Sprite s)
            {
                arrow = s;
                break;
            }
        }

        if (arrow == null)
        {
            Debug.LogError("No Sprite sub-asset found in PlayerArrow.png");
            return;
        }

        img.sprite = arrow;
        img.type = Image.Type.Simple;
        img.preserveAspect = true;
        EditorUtility.SetDirty(img);

        Debug.Log("Arrow sprite assigned to PlayerIcon.");
    }

    [MenuItem("Tools/Cleanup Minimap Size")]
    static void CleanupMinimap()
    {
        GameObject border = GameObject.Find("MinimapBorder");
        if (border != null)
        {
            RectTransform rt = border.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(210, 210);
            EditorUtility.SetDirty(rt);
        }

        GameObject label = GameObject.Find("MinimapLabel");
        if (label != null)
        {
            RectTransform rt = label.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(110, 220);
            rt.sizeDelta = new Vector2(210, 24);
            EditorUtility.SetDirty(rt);
        }

        GameObject minimapImg = GameObject.Find("MinimapImage");
        if (minimapImg != null)
        {
            RectTransform rt = minimapImg.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(110, 108);
            rt.sizeDelta = new Vector2(196, 196);
            EditorUtility.SetDirty(rt);
        }

        Debug.Log("Minimap layout cleaned up.");
    }
#endif
}
