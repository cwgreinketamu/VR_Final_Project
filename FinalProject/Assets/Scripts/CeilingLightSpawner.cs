using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CeilingLightSpawner : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Spawn Ceiling Lights")]
    static void SpawnCeilingLights()
    {
        GameObject parent = new GameObject("CeilingLights");

        float y = 6.5f;
        float startX = -8f;
        float endX = 8f;
        float startZ = 16f;
        float endZ = 34f;
        float spacing = 4f;

        int index = 0;
        for (float x = startX; x <= endX; x += spacing)
        {
            for (float z = startZ; z <= endZ; z += spacing)
            {
                GameObject lightObj = new GameObject("CeilingLight_" + index);
                lightObj.transform.parent = parent.transform;
                lightObj.transform.position = new Vector3(x, y, z);
                lightObj.transform.rotation = Quaternion.Euler(90, 0, 0);

                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = new Color(1f, 0.95f, 0.85f);
                light.intensity = 2.5f;
                light.range = 10f;
                light.shadows = LightShadows.Soft;

                lightObj.AddComponent<UniversalAdditionalLightData>();

                index++;
            }
        }

        EditorUtility.SetDirty(parent);
        Debug.Log("Spawned " + index + " ceiling lights.");
    }
#endif
}
