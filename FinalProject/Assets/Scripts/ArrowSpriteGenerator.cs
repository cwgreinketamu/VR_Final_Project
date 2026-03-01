using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ArrowSpriteGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Generate Arrow Sprite")]
    static void GenerateArrow()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = new Color(0, 0, 0, 0);
        Color arrow = new Color(0f, 1f, 0.4f, 1f);

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = clear;

        int cx = size / 2;
        for (int y = 0; y < size; y++)
        {
            float t = (float)y / (size - 1);
            int halfWidth = Mathf.RoundToInt(Mathf.Lerp(size / 2f - 1, 0, t));
            for (int x = cx - halfWidth; x <= cx + halfWidth; x++)
            {
                if (x >= 0 && x < size)
                    pixels[y * size + x] = arrow;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        byte[] pngData = tex.EncodeToPNG();
        string path = "Assets/Materials/PlayerArrow.png";
        System.IO.File.WriteAllBytes(Application.dataPath + "/../" + path, pngData);
        AssetDatabase.Refresh();

        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 32;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        Debug.Log("Arrow sprite saved to " + path);
    }
#endif
}
