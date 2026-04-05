using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrottoCorridor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Grotto/4 - Build Corridor To Grotto")]
    static void BuildCorridor()
    {
        GameObject existing = GameObject.Find("GrottoCorridor");
        if (existing != null) DestroyImmediate(existing);

        GameObject corridor = new GameObject("GrottoCorridor");
        corridor.transform.position = Vector3.zero;

        Material wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveWall.mat");
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveFloor.mat");
        Material ceilMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveCeiling.mat");

        if (wallMat == null) wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MuseumWall.mat");
        if (floorMat == null) floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MuseumFloor.mat");
        if (ceilMat == null) ceilMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MuseumCeiling.mat");

        float startX = -10f;
        float endX = -22f;
        float corridorWidth = 3f;
        float corridorHeight = 6f;
        float corridorZ = 0f;
        float length = Mathf.Abs(endX - startX);

        float midX = (startX + endX) / 2f;

        MakeBox("CorridorFloor", corridor.transform, new Vector3(midX, 0, corridorZ), new Vector3(length, 0.1f, corridorWidth), floorMat, true);
        MakeBox("CorridorCeiling", corridor.transform, new Vector3(midX, corridorHeight, corridorZ), new Vector3(length, 0.1f, corridorWidth), ceilMat, false);
        MakeBox("CorridorWallFront", corridor.transform, new Vector3(midX, corridorHeight / 2f, corridorZ + corridorWidth / 2f), new Vector3(length, corridorHeight, 0.2f), wallMat, false);
        MakeBox("CorridorWallBack", corridor.transform, new Vector3(midX, corridorHeight / 2f, corridorZ - corridorWidth / 2f), new Vector3(length, corridorHeight, 0.2f), wallMat, false);

        GameObject signObj = new GameObject("GrottoSign");
        signObj.transform.parent = corridor.transform;
        signObj.transform.position = new Vector3(startX - 1f, 4f, corridorZ);
        signObj.transform.rotation = Quaternion.Euler(0, 90, 0);

        Canvas signCanvas = signObj.AddComponent<Canvas>();
        signCanvas.renderMode = RenderMode.WorldSpace;
        RectTransform signRect = signObj.GetComponent<RectTransform>();
        signRect.sizeDelta = new Vector2(300, 60);
        signObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        GameObject textObj = new GameObject("SignText");
        textObj.transform.parent = signObj.transform;
        textObj.transform.localPosition = Vector3.zero;
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(300, 60);
        TMPro.TextMeshProUGUI tmp = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = "Waterfall Grotto →";
        tmp.fontSize = 28;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.color = new Color(0.7f, 0.9f, 1f);

        CreateLight("CorridorLight1", corridor.transform, new Vector3(startX - 3f, 5f, corridorZ), new Color(0.4f, 0.6f, 0.9f), 2f, 10f);
        CreateLight("CorridorLight2", corridor.transform, new Vector3(midX, 5f, corridorZ), new Color(0.3f, 0.5f, 0.8f), 2f, 10f);
        CreateLight("CorridorLight3", corridor.transform, new Vector3(endX + 3f, 5f, corridorZ), new Color(0.2f, 0.7f, 0.8f), 2f, 10f);

        EditorUtility.SetDirty(corridor);
        Debug.Log("Corridor built from X=" + startX + " to X=" + endX);
    }

    static void MakeBox(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool addTeleport)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.parent = parent;
        box.transform.position = pos;
        box.transform.localScale = scale;
        box.GetComponent<MeshRenderer>().sharedMaterial = mat;
        box.isStatic = true;

        if (addTeleport)
            box.AddComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>();
    }

    static void CreateLight(string name, Transform parent, Vector3 pos, Color color, float intensity, float range)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        go.transform.position = pos;
        Light l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = color;
        l.intensity = intensity;
        l.range = range;
        l.shadows = LightShadows.None;
        go.AddComponent<UniversalAdditionalLightData>();
    }
#endif
}
