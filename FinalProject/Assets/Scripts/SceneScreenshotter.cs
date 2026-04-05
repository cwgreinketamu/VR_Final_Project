using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneScreenshotter : MonoBehaviour
{
#if UNITY_EDITOR
    static readonly (string name, Vector3 pos, Vector3 euler)[] shots = new[]
    {
        ("01_spawn",           new Vector3(0f, 1.6f, 0f),      new Vector3(0f, 0f, 0f)),
        ("02_lobby_right",     new Vector3(10f, 1.6f, 5f),     new Vector3(0f, 180f, 0f)),
        ("03_maze_entrance",   new Vector3(0f, 1.6f, 12f),     new Vector3(0f, 0f, 0f)),
        ("04_maze_inside",     new Vector3(0f, 1.6f, 25f),     new Vector3(0f, 0f, 0f)),
        ("05_corridor_entry",  new Vector3(-5f, 1.6f, -7f),    new Vector3(0f, 180f, 0f)),
        ("06_grotto_entry",    new Vector3(-5f, 1.6f, -12f),   new Vector3(0f, 180f, 0f)),
        ("07_grotto_interior", new Vector3(-5f, 1.6f, -18f),   new Vector3(0f, 0f, 0f)),
        ("08_grotto_streams",  new Vector3(-5f, 1.6f, -22f),   new Vector3(0f, 0f, 0f)),
        ("09_manip_entry",     new Vector3(18f, 1.6f, 0f),     new Vector3(0f, 90f, 0f)),
        ("10_manip_room",      new Vector3(28f, 1.6f, 0f),     new Vector3(0f, 90f, 0f)),
    };

    [MenuItem("Tools/Museum/Take Scene Screenshots")]
    static void TakeScreenshots()
    {
        string folder = Path.Combine(Application.dataPath, "../SceneShots");
        Directory.CreateDirectory(folder);

        int width = 1280;
        int height = 720;

        GameObject camGO = new GameObject("__SceneShotter_Camera__");
        Camera cam = camGO.AddComponent<Camera>();
        cam.fieldOfView = 80f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 200f;
        cam.clearFlags = CameraClearFlags.Skybox;

        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;

        foreach (var shot in shots)
        {
            camGO.transform.position = shot.pos;
            camGO.transform.eulerAngles = shot.euler;

            cam.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            byte[] bytes = tex.EncodeToPNG();
            string path = Path.Combine(folder, shot.name + ".png");
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(tex);

            Debug.Log("Saved: " + path);
        }

        cam.targetTexture = null;
        RenderTexture.active = null;
        rt.Release();
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(camGO);

        Debug.Log("All scene screenshots saved to: " + folder);
    }
#endif
}
