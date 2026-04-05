using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CorridorAuditor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Museum/Audit Corridor Opening")]
    static void AuditCorridorOpening()
    {
        float corridorMinX = -7f;
        float corridorMaxX = -3f;
        float corridorMinZ = -11f;
        float corridorMaxZ = 1f;
        float corridorMaxY = 4f;

        List<string> blocking = new List<string>();
        List<int> blockingIDs = new List<int>();

        GameObject wallsParent = GameObject.Find("Walls");
        if (wallsParent != null)
        {
            foreach (Transform child in wallsParent.transform)
            {
                Bounds b = GetBounds(child.gameObject);
                if (b.min.x < corridorMaxX && b.max.x > corridorMinX &&
                    b.min.z < corridorMaxZ && b.max.z > corridorMinZ &&
                    b.min.y < corridorMaxY)
                {
                    blocking.Add(child.name + " pos=" + child.position + " scale=" + child.localScale + " id=" + child.gameObject.GetInstanceID());
                    blockingIDs.Add(child.gameObject.GetInstanceID());
                }
            }
        }

        GameObject mazeParent = GameObject.Find("Maze");
        if (mazeParent != null)
        {
            foreach (Transform child in mazeParent.transform)
            {
                Bounds b = GetBounds(child.gameObject);
                if (b.min.x < corridorMaxX && b.max.x > corridorMinX &&
                    b.min.z < corridorMaxZ && b.max.z > corridorMinZ &&
                    b.min.y < corridorMaxY)
                {
                    blocking.Add("[MAZE] " + child.name + " pos=" + child.position + " scale=" + child.localScale + " id=" + child.gameObject.GetInstanceID());
                    blockingIDs.Add(child.gameObject.GetInstanceID());
                }
            }
        }

        if (blocking.Count == 0)
        {
            Debug.Log("No walls found in corridor zone. Corridor is clear.");
        }
        else
        {
            Debug.Log("Found " + blocking.Count + " objects in corridor zone:");
            foreach (var s in blocking)
                Debug.Log("  BLOCKING: " + s);
        }
    }

    static Bounds GetBounds(GameObject go)
    {
        Renderer r = go.GetComponent<Renderer>();
        if (r != null) return r.bounds;
        Collider c = go.GetComponent<Collider>();
        if (c != null) return c.bounds;
        return new Bounds(go.transform.position, Vector3.zero);
    }

    [MenuItem("Tools/Museum/Remove Blocking Corridor Walls")]
    static void RemoveBlockingWalls()
    {
        float corridorMinX = -6.8f;
        float corridorMaxX = -3.2f;
        float corridorMinZ = -10.5f;
        float corridorMaxZ = 0.5f;
        float corridorMinY = 0f;
        float corridorMaxY = 3.5f;

        List<GameObject> toDelete = new List<GameObject>();

        GameObject wallsParent = GameObject.Find("Walls");
        if (wallsParent != null)
        {
            foreach (Transform child in wallsParent.transform)
            {
                Vector3 pos = child.position;
                Vector3 scale = child.localScale;

                bool xOverlap = (pos.x - scale.x / 2f) < corridorMaxX && (pos.x + scale.x / 2f) > corridorMinX;
                bool zOverlap = (pos.z - scale.z / 2f) < corridorMaxZ && (pos.z + scale.z / 2f) > corridorMinZ;
                bool yOverlap = pos.y > corridorMinY && pos.y < corridorMaxY;

                if (xOverlap && zOverlap && yOverlap && scale.x < 5f && scale.z < 5f)
                {
                    toDelete.Add(child.gameObject);
                }
            }
        }

        if (toDelete.Count == 0)
        {
            Debug.Log("No small blocking walls found in corridor zone.");
            return;
        }

        Debug.Log("Removing " + toDelete.Count + " blocking walls in corridor:");
        foreach (var go in toDelete)
        {
            Debug.Log("  Removing: " + go.name + " at " + go.transform.position);
            DestroyImmediate(go);
        }
    }
#endif
}
