using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrottoBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Grotto/1 - Create Grotto Materials")]
    static void CreateMaterials()
    {
        string folder = "Assets/Materials/Grotto";
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets/Materials", "Grotto");

        CreateMat(folder + "/CaveWall.mat", new Color(0.18f, 0.15f, 0.13f), 0.15f);
        CreateMat(folder + "/CaveFloor.mat", new Color(0.14f, 0.12f, 0.1f), 0.2f);
        CreateMat(folder + "/CaveCeiling.mat", new Color(0.1f, 0.09f, 0.08f), 0.1f);
        CreateMat(folder + "/Water.mat", new Color(0.1f, 0.4f, 0.6f, 0.7f), 0.9f, true);
        CreateMat(folder + "/CrystalTeal.mat", new Color(0.1f, 0.9f, 0.8f), 0.8f, false, new Color(0.1f, 0.9f, 0.8f) * 2f);
        CreateMat(folder + "/CrystalPurple.mat", new Color(0.6f, 0.1f, 0.9f), 0.8f, false, new Color(0.6f, 0.1f, 0.9f) * 2f);
        CreateMat(folder + "/CrystalBlue.mat", new Color(0.1f, 0.3f, 1f), 0.8f, false, new Color(0.1f, 0.3f, 1f) * 2f);
        CreateMat(folder + "/CrystalGreen.mat", new Color(0.1f, 1f, 0.3f), 0.8f, false, new Color(0.1f, 1f, 0.3f) * 2f);
        CreateMat(folder + "/SmoothStone.mat", new Color(0.5f, 0.48f, 0.45f), 0.7f);
        CreateMat(folder + "/RoughRock.mat", new Color(0.35f, 0.3f, 0.25f), 0.1f);
        CreateMat(folder + "/RidgedCylinder.mat", new Color(0.6f, 0.55f, 0.4f), 0.3f);
        CreateMat(folder + "/DarkAlcove.mat", new Color(0.05f, 0.04f, 0.04f), 0.05f);
        CreateMat(folder + "/Pedestal.mat", new Color(0.3f, 0.28f, 0.25f), 0.4f);
        CreateMat(folder + "/WaterfallMist.mat", new Color(0.7f, 0.85f, 1f, 0.3f), 0.0f, true);

        AssetDatabase.SaveAssets();
        Debug.Log("Grotto materials created.");
    }

    static void CreateMat(string path, Color color, float smoothness, bool transparent = false, Color? emission = null)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Smoothness", smoothness);
        mat.SetFloat("_Metallic", 0f);

        if (transparent)
        {
            mat.SetFloat("_Surface", 1);
            mat.SetFloat("_Blend", 0);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetFloat("_SrcBlend", 5);
            mat.SetFloat("_DstBlend", 10);
            mat.SetFloat("_ZWrite", 0);
            mat.renderQueue = 3000;
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }

        if (emission.HasValue)
        {
            mat.SetColor("_EmissionColor", emission.Value);
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

        AssetDatabase.CreateAsset(mat, path);
    }

    [MenuItem("Tools/Grotto/2 - Build Grotto Room")]
    static void BuildGrottoRoom()
    {
        GameObject existing = GameObject.Find("GrottoRoom");
        if (existing != null) DestroyImmediate(existing);

        GameObject grotto = new GameObject("GrottoRoom");
        grotto.transform.position = new Vector3(-5, 0, -18);

        Material wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveWall.mat");
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveFloor.mat");
        Material ceilMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveCeiling.mat");
        Material darkMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/DarkAlcove.mat");

        float roomW = 14f, roomD = 14f, roomH = 5f;

        CreateWall("GrottoFloor", grotto.transform, new Vector3(0, 0, 0), new Vector3(roomW, 0.1f, roomD), floorMat);
        CreateWall("GrottoCeiling", grotto.transform, new Vector3(0, roomH, 0), new Vector3(roomW, 0.1f, roomD), ceilMat);
        CreateWall("GrottoWallLeft", grotto.transform, new Vector3(-roomW / 2f, roomH / 2f, 0), new Vector3(0.3f, roomH, roomD), wallMat);
        CreateWall("GrottoWallRight", grotto.transform, new Vector3(roomW / 2f, roomH / 2f, 0), new Vector3(0.3f, roomH, roomD), wallMat);
        CreateWall("GrottoWallBack", grotto.transform, new Vector3(0, roomH / 2f, -roomD / 2f), new Vector3(roomW, roomH, 0.3f), wallMat);

        CreateWall("GrottoWallFrontLeft", grotto.transform, new Vector3(-4.5f, roomH / 2f, roomD / 2f), new Vector3(5f, roomH, 0.3f), wallMat);
        CreateWall("GrottoWallFrontRight", grotto.transform, new Vector3(4.5f, roomH / 2f, roomD / 2f), new Vector3(5f, roomH, 0.3f), wallMat);

        GameObject alcove = new GameObject("DarkAlcove");
        alcove.transform.parent = grotto.transform;
        alcove.transform.localPosition = new Vector3(5f, 0, -5f);
        CreateWall("AlcoveBack", alcove.transform, new Vector3(0, 1.5f, -1.5f), new Vector3(4f, 3f, 0.2f), darkMat);
        CreateWall("AlcoveLeft", alcove.transform, new Vector3(-2f, 1.5f, -0.75f), new Vector3(0.2f, 3f, 1.5f), darkMat);
        CreateWall("AlcoveRight", alcove.transform, new Vector3(2f, 1.5f, -0.75f), new Vector3(0.2f, 3f, 1.5f), darkMat);
        CreateWall("AlcoveTop", alcove.transform, new Vector3(0, 3f, -0.75f), new Vector3(4f, 0.2f, 1.5f), darkMat);

        Material pedestalMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/Pedestal.mat");
        for (int i = 0; i < 3; i++)
        {
            float xOff = (i - 1) * 1.2f;
            GameObject ped = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ped.name = "Pedestal_" + i;
            ped.transform.parent = alcove.transform;
            ped.transform.localPosition = new Vector3(xOff, 0.5f, -0.75f);
            ped.transform.localScale = new Vector3(0.4f, 0.5f, 0.4f);
            ped.GetComponent<MeshRenderer>().sharedMaterial = pedestalMat;
            EditorUtility.SetDirty(ped);
        }

        AddGrottoLights(grotto.transform);

        Material corridorWall = wallMat;
        Material corridorFloor = floorMat;
        Material corridorCeil = ceilMat;
        float corrStartZ = roomD / 2f;
        float corrEndZ = 9f + 18f;
        float corrLen = corrEndZ - corrStartZ;
        float corrW = 3f;

        CreateWall("CorridorFloor", grotto.transform, new Vector3(0, 0, corrStartZ + corrLen / 2f), new Vector3(corrW, 0.1f, corrLen), corridorFloor);
        CreateWall("CorridorCeiling", grotto.transform, new Vector3(0, roomH, corrStartZ + corrLen / 2f), new Vector3(corrW, 0.1f, corrLen), corridorCeil);
        CreateWall("CorridorWallLeft", grotto.transform, new Vector3(-corrW / 2f, roomH / 2f, corrStartZ + corrLen / 2f), new Vector3(0.2f, roomH, corrLen), corridorWall);
        CreateWall("CorridorWallRight", grotto.transform, new Vector3(corrW / 2f, roomH / 2f, corrStartZ + corrLen / 2f), new Vector3(0.2f, roomH, corrLen), corridorWall);

        EditorUtility.SetDirty(grotto);
        Debug.Log("Grotto room + corridor built. Room at (-5,0,-18), corridor to Z=" + (grotto.transform.position.z + corrStartZ + corrLen));
    }

    static void AddGrottoLights(Transform parent)
    {
        Color teal = new Color(0.3f, 0.8f, 0.9f);
        Color purple = new Color(0.5f, 0.2f, 0.8f);
        Color blue = new Color(0.2f, 0.4f, 1f);

        CreateLight("GrottoLight_Main", parent, new Vector3(0, 4.5f, 0), Color.white, 1.5f, 18f);
        CreateLight("GrottoLight_Teal1", parent, new Vector3(-5, 3f, 2f), teal, 3f, 8f);
        CreateLight("GrottoLight_Purple1", parent, new Vector3(5, 3f, -4f), purple, 3f, 8f);
        CreateLight("GrottoLight_Blue1", parent, new Vector3(-3, 2f, -5f), blue, 2.5f, 8f);
        CreateLight("GrottoLight_Teal2", parent, new Vector3(3, 4f, 4f), teal, 2f, 8f);
        CreateLight("GrottoLight_Entry", parent, new Vector3(0, 4f, 10f), new Color(0.9f, 0.85f, 0.7f), 2f, 10f);
        CreateLight("GrottoLight_Corr1", parent, new Vector3(0, 4f, 16f), new Color(0.4f, 0.6f, 0.9f), 2f, 8f);
        CreateLight("GrottoLight_Corr2", parent, new Vector3(0, 4f, 22f), new Color(0.5f, 0.5f, 0.8f), 2f, 8f);
    }

    static void CreateLight(string name, Transform parent, Vector3 pos, Color color, float intensity, float range)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        go.transform.localPosition = pos;
        Light l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = color;
        l.intensity = intensity;
        l.range = range;
        l.shadows = LightShadows.None;
        go.AddComponent<UniversalAdditionalLightData>();
    }

    static void CreateWall(string name, Transform parent, Vector3 localPos, Vector3 scale, Material mat)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.parent = parent;
        wall.transform.localPosition = localPos;
        wall.transform.localScale = scale;
        wall.GetComponent<MeshRenderer>().sharedMaterial = mat;
        wall.isStatic = true;
    }
#endif
}
