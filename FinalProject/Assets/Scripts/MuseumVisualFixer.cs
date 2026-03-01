using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MuseumVisualFixer : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Fix Museum Walls")]
    static void FixMuseumWalls()
    {
        Material wallMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MuseumWall.mat");
        Material skirtMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MuseumSkirting.mat");
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MuseumFloor.mat");
        Material ceilMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MuseumCeiling.mat");

        if (wallMat == null)
        {
            wallMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            wallMat.SetColor("_BaseColor", new Color(0.88f, 0.82f, 0.73f, 1f));
            wallMat.SetFloat("_Smoothness", 0.25f);
            AssetDatabase.CreateAsset(wallMat, "Assets/Materials/MuseumWall.mat");
        }
        if (skirtMat == null)
        {
            skirtMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            skirtMat.SetColor("_BaseColor", new Color(0.45f, 0.35f, 0.28f, 1f));
            skirtMat.SetFloat("_Smoothness", 0.4f);
            AssetDatabase.CreateAsset(skirtMat, "Assets/Materials/MuseumSkirting.mat");
        }
        if (floorMat == null)
        {
            floorMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            floorMat.SetColor("_BaseColor", new Color(0.65f, 0.55f, 0.42f, 1f));
            floorMat.SetFloat("_Smoothness", 0.5f);
            AssetDatabase.CreateAsset(floorMat, "Assets/Materials/MuseumFloor.mat");
        }
        if (ceilMat == null)
        {
            ceilMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            ceilMat.SetColor("_BaseColor", new Color(0.95f, 0.93f, 0.88f, 1f));
            ceilMat.SetFloat("_Smoothness", 0.1f);
            AssetDatabase.CreateAsset(ceilMat, "Assets/Materials/MuseumCeiling.mat");
        }

        int wallCount = 0;
        int skirtCount = 0;

        string[] parentNames = { "Maze", "Walls", "temp" };
        foreach (string parentName in parentNames)
        {
            GameObject parent = GameObject.Find(parentName);
            if (parent == null) continue;

            MeshRenderer[] renderers = parent.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer r in renderers)
            {
                if (r.gameObject.name.Contains("Skirting"))
                {
                    r.sharedMaterial = skirtMat;
                    skirtCount++;
                }
                else
                {
                    r.sharedMaterial = wallMat;
                    wallCount++;
                }
                EditorUtility.SetDirty(r);
            }
        }

        GameObject floor = GameObject.Find("Floor");
        if (floor != null)
        {
            MeshRenderer[] floorRenderers = floor.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer r in floorRenderers)
            {
                r.sharedMaterial = floorMat;
                EditorUtility.SetDirty(r);
            }
        }

        GameObject ceiling = GameObject.Find("Ceiling");
        if (ceiling != null)
        {
            MeshRenderer[] ceilRenderers = ceiling.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer r in ceilRenderers)
            {
                r.sharedMaterial = ceilMat;
                EditorUtility.SetDirty(r);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Fixed " + wallCount + " walls, " + skirtCount + " skirtings, floor, and ceiling.");
    }

    [MenuItem("Tools/Boost URP Light Limit")]
    static void BoostLightLimit()
    {
        string[] paths = {
            "Assets/Settings/PC_RPAsset.asset",
            "Assets/Settings/Mobile_RPAsset.asset"
        };
        foreach (string path in paths)
        {
            UniversalRenderPipelineAsset asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);
            if (asset == null) continue;

            SerializedObject so = new SerializedObject(asset);
            SerializedProperty iter = so.GetIterator();
            iter.Next(true);
            while (iter.NextVisible(true))
            {
                if (iter.name.Contains("AdditionalLights") || iter.name.Contains("additionalLights") || iter.name.Contains("MaxAdditional"))
                {
                    if (iter.propertyType == SerializedPropertyType.Integer)
                    {
                        Debug.Log("Found: " + iter.propertyPath + " = " + iter.intValue);
                        iter.intValue = 8;
                    }
                }
            }
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);
            Debug.Log("Updated " + path);
        }
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Boost Ceiling Lights And Add Painting Spots")]
    static void BoostLightsAndAddSpots()
    {
        GameObject ceilingLights = GameObject.Find("CeilingLights");
        if (ceilingLights != null)
        {
            Light[] lights = ceilingLights.GetComponentsInChildren<Light>(true);
            foreach (Light l in lights)
            {
                l.intensity = 5f;
                l.range = 14f;
                l.shadows = LightShadows.None;
                EditorUtility.SetDirty(l);
            }
            Debug.Log("Boosted " + lights.Length + " ceiling lights to intensity 5, range 14.");
        }

        GameObject paintings = GameObject.Find("Paintings");
        if (paintings == null) return;

        GameObject spotParent = GameObject.Find("PaintingSpotlights");
        if (spotParent != null)
            DestroyImmediate(spotParent);

        spotParent = new GameObject("PaintingSpotlights");

        Transform[] children = new Transform[paintings.transform.childCount];
        for (int i = 0; i < paintings.transform.childCount; i++)
            children[i] = paintings.transform.GetChild(i);

        int spotCount = 0;
        foreach (Transform child in children)
        {
            if (!child.name.StartsWith("Paint_")) continue;

            Vector3 paintPos = child.position;
            Vector3 spotPos = new Vector3(paintPos.x, 5f, paintPos.z);

            GameObject spot = new GameObject("Spot_" + child.name);
            spot.transform.parent = spotParent.transform;
            spot.transform.position = spotPos;
            spot.transform.rotation = Quaternion.Euler(90, 0, 0);

            Light sl = spot.AddComponent<Light>();
            sl.type = LightType.Spot;
            sl.color = new Color(1f, 0.97f, 0.9f);
            sl.intensity = 15f;
            sl.range = 8f;
            sl.spotAngle = 60f;
            sl.innerSpotAngle = 40f;
            sl.shadows = LightShadows.None;

            spot.AddComponent<UniversalAdditionalLightData>();
            spotCount++;
        }

        EditorUtility.SetDirty(spotParent);
        Debug.Log("Created " + spotCount + " painting spotlights.");
    }
#endif
}
