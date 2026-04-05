using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneAuditor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Museum/Audit Scene In Front Of Player")]
    static void AuditFrontOfPlayer()
    {
        Vector3 playerPos = Vector3.zero;
        GameObject xr = GameObject.Find("Custom XR Origin");
        if (xr != null) playerPos = xr.transform.position;

        float minZ = playerPos.z - 2f;
        float maxZ = playerPos.z + 15f;
        float minX = playerPos.x - 10f;
        float maxX = playerPos.x + 10f;

        Debug.Log("=== Audit: objects at Z=" + minZ + " to " + maxZ + ", X=" + minX + " to " + maxX + " ===");

        var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots)
        {
            AuditRecursive(root.transform, minX, maxX, minZ, maxZ, 0);
        }
        Debug.Log("=== Audit complete ===");
    }

    static void AuditRecursive(Transform t, float minX, float maxX, float minZ, float maxZ, int depth)
    {
        if (depth > 3) return;
        Vector3 pos = t.position;
        if (pos.x >= minX && pos.x <= maxX && pos.z >= minZ && pos.z <= maxZ)
        {
            Renderer r = t.GetComponent<Renderer>();
            if (r != null)
            {
                string matName = r.sharedMaterial != null ? r.sharedMaterial.name : "NULL";
                Debug.Log("  OBJ: " + t.gameObject.name + " pos=" + pos + " scale=" + t.localScale + " mat=" + matName + " active=" + t.gameObject.activeInHierarchy);
            }
        }
        foreach (Transform child in t)
            AuditRecursive(child, minX, maxX, minZ, maxZ, depth + 1);
    }

    [MenuItem("Tools/Museum/List All Renderers In Scene")]
    static void ListAllRenderers()
    {
        var renderers = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var results = new List<string>();
        int total = 0;
        foreach (var r in renderers)
        {
            total++;
            Vector3 pos = r.transform.position;
            string matName = r.sharedMaterial != null ? r.sharedMaterial.name : "NULL";
            if (matName == "NULL" || matName == "Default-Material" || matName.ToLower().Contains("default"))
                results.Add("  DEFAULT/NULL: " + r.gameObject.name + " pos=" + pos + " mat=[" + matName + "] active=" + r.gameObject.activeInHierarchy);
        }
        Debug.Log("Total renderers: " + total + ". Default/null material objects: " + results.Count);
        foreach (var s in results) Debug.Log(s);
        if (results.Count == 0) Debug.Log("No default/null material objects found.");
    }

    [MenuItem("Tools/Museum/Apply Grotto Materials To Existing Geometry")]
    static void ApplyGrottoMaterials()
    {
        Material caveWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveWall.mat");
        Material caveFloor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveFloor.mat");
        Material caveCeiling = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/CaveCeiling.mat");
        Material water = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/Water.mat");
        Material darkAlcove = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/DarkAlcove.mat");
        Material pedestal = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/Pedestal.mat");

        GameObject grottoRoom = GameObject.Find("GrottoRoom");
        if (grottoRoom == null) { Debug.LogError("GrottoRoom not found."); return; }

        int applied = 0;
        ApplyToAll(grottoRoom.transform, ref applied, caveWall, caveFloor, caveCeiling, water, darkAlcove, pedestal);
        Debug.Log("Applied grotto materials to " + applied + " objects.");
        EditorUtility.SetDirty(grottoRoom);
    }

    static void ApplyToAll(Transform t, ref int count, Material wall, Material floor, Material ceiling, Material water, Material dark, Material ped)
    {
        string n = t.gameObject.name;
        MeshRenderer mr = t.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            if (n.Contains("Floor") || n.Contains("Corridor") && !n.Contains("Wall") && !n.Contains("Ceil"))
            { mr.sharedMaterial = floor; count++; }
            else if (n.Contains("Ceiling") || n.Contains("Ceil"))
            { mr.sharedMaterial = ceiling; count++; }
            else if (n.Contains("Wall"))
            { mr.sharedMaterial = wall; count++; }
            else if (n.Contains("Stream") || n.Contains("Water"))
            { mr.sharedMaterial = water; count++; }
            else if (n.Contains("Alcove"))
            { mr.sharedMaterial = dark; count++; }
            else if (n.Contains("Pedestal"))
            { mr.sharedMaterial = ped; count++; }
            EditorUtility.SetDirty(t.gameObject);
        }
        foreach (Transform child in t)
            ApplyToAll(child, ref count, wall, floor, ceiling, water, dark, ped);
    }
#endif
}

