using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrottoSceneSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Grotto/3 - Setup Grotto Scene Objects")]
    static void SetupGrottoScene()
    {
        GameObject grottoRoom = GameObject.Find("GrottoRoom");
        if (grottoRoom == null)
        {
            Debug.LogError("GrottoRoom not found. Run 'Build Grotto Room' first.");
            return;
        }

        Vector3 grottoOrigin = grottoRoom.transform.position;

        SetupHapticManager();
        SetupWaterStreams(grottoRoom.transform, grottoOrigin);
        SetupBlindObjects(grottoRoom.transform, grottoOrigin);
        SetupCrystals(grottoRoom.transform, grottoOrigin);
        SetupEntranceTrigger(grottoRoom.transform, grottoOrigin);
        SetupGrottoGameManager(grottoRoom.transform);
        SetupGrottoUI(grottoRoom.transform);
        SetupGrottoFloorTeleport(grottoRoom.transform);

        EditorUtility.SetDirty(grottoRoom);
        Debug.Log("Grotto scene objects created.");
    }

    static void SetupHapticManager()
    {
        if (GameObject.Find("HapticManager") != null) return;

        GameObject hm = new GameObject("HapticManager");
        hm.AddComponent<HapticManager>();
        EditorUtility.SetDirty(hm);
    }

    static void SetupWaterStreams(Transform parent, Vector3 origin)
    {
        GameObject existing = GameObject.Find("WaterStreams");
        if (existing != null) DestroyImmediate(existing);

        GameObject streams = new GameObject("WaterStreams");
        streams.transform.parent = parent;
        streams.transform.localPosition = Vector3.zero;

        Material waterMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/Water.mat");

        string[] names = { "GentleStream", "MediumStream", "StrongStream" };
        WaterStreamHaptic.StreamIntensity[] intensities = {
            WaterStreamHaptic.StreamIntensity.Gentle,
            WaterStreamHaptic.StreamIntensity.Medium,
            WaterStreamHaptic.StreamIntensity.Strong
        };

        for (int i = 0; i < 3; i++)
        {
            float xPos = -4f + i * 4f;
            GameObject stream = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stream.name = names[i];
            stream.transform.parent = streams.transform;
            stream.transform.localPosition = new Vector3(xPos, 1.5f, -6f);
            stream.transform.localScale = new Vector3(1.5f, 3f, 1.5f);
            stream.GetComponent<MeshRenderer>().sharedMaterial = waterMat;
            stream.GetComponent<BoxCollider>().isTrigger = true;

            WaterStreamHaptic wsh = stream.AddComponent<WaterStreamHaptic>();
            wsh.intensity = intensities[i];

            AudioSource audio = stream.AddComponent<AudioSource>();
            audio.loop = true;
            audio.playOnAwake = false;
            audio.spatialBlend = 1f;

            GameObject selector = new GameObject(names[i] + "_Selector");
            selector.transform.parent = stream.transform;
            selector.transform.localPosition = new Vector3(0, -1.5f, 1.5f);
            BoxCollider selCol = selector.AddComponent<BoxCollider>();
            selCol.size = new Vector3(2f, 0.5f, 2f);
            selCol.isTrigger = true;
            WaterStreamSelector wss = selector.AddComponent<WaterStreamSelector>();
            wss.myStream = wsh;

            EditorUtility.SetDirty(stream);
        }
    }

    static void SetupBlindObjects(Transform parent, Vector3 origin)
    {
        GameObject existing = GameObject.Find("BlindObjects");
        if (existing != null) DestroyImmediate(existing);

        GameObject blindGroup = new GameObject("BlindObjects");
        blindGroup.transform.parent = parent;
        blindGroup.transform.localPosition = Vector3.zero;

        Material smoothMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/SmoothStone.mat");
        Material roughMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/RoughRock.mat");
        Material heavyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Grotto/RidgedCylinder.mat");

        GameObject alcove = null;
        Transform grottoT = parent;
        for (int c = 0; c < grottoT.childCount; c++)
        {
            if (grottoT.GetChild(c).name == "DarkAlcove")
            {
                alcove = grottoT.GetChild(c).gameObject;
                break;
            }
        }
        Vector3 alcovePos = alcove != null ? alcove.transform.localPosition : new Vector3(6f, 0, -16f);

        PrimitiveType[] shapes = { PrimitiveType.Sphere, PrimitiveType.Cube, PrimitiveType.Cylinder };
        Material[] mats = { smoothMat, roughMat, heavyMat };
        HapticObject.TextureType[] types = {
            HapticObject.TextureType.Smooth,
            HapticObject.TextureType.Rough,
            HapticObject.TextureType.Heavy
        };
        string[] objNames = { "SmoothSphere", "RoughCube", "HeavyCylinder" };

        for (int i = 0; i < 3; i++)
        {
            float xOff = (i - 1) * 1.2f;
            GameObject obj = GameObject.CreatePrimitive(shapes[i]);
            obj.name = objNames[i];
            obj.transform.parent = blindGroup.transform;
            obj.transform.localPosition = alcovePos + new Vector3(xOff, 1.2f, -0.75f);
            obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            obj.GetComponent<MeshRenderer>().sharedMaterial = mats[i];

            Collider col = obj.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            HapticObject ho = obj.AddComponent<HapticObject>();
            ho.textureType = types[i];

            GameObject selectorGO = new GameObject(objNames[i] + "_Selector");
            selectorGO.transform.parent = obj.transform;
            selectorGO.transform.localPosition = Vector3.zero;
            SphereCollider selCol = selectorGO.AddComponent<SphereCollider>();
            selCol.radius = 2f;
            selCol.isTrigger = true;
            BlindObjectSelector bos = selectorGO.AddComponent<BlindObjectSelector>();
            bos.myObject = ho;

            EditorUtility.SetDirty(obj);
        }
    }

    static void SetupCrystals(Transform parent, Vector3 origin)
    {
        GameObject existing = GameObject.Find("GrottoCrystals");
        if (existing != null) DestroyImmediate(existing);

        GameObject crystalGroup = new GameObject("GrottoCrystals");
        crystalGroup.transform.parent = parent;
        crystalGroup.transform.localPosition = Vector3.zero;

        string[] matPaths = {
            "Assets/Materials/Grotto/CrystalTeal.mat",
            "Assets/Materials/Grotto/CrystalPurple.mat",
            "Assets/Materials/Grotto/CrystalBlue.mat",
            "Assets/Materials/Grotto/CrystalGreen.mat"
        };

        Vector3[] positions = {
            new Vector3(-5f, 0.5f, 3f),
            new Vector3(4f, 0.5f, -3f),
            new Vector3(-3f, 0.5f, -6f),
            new Vector3(6f, 0.5f, 2f)
        };

        Color[] lightColors = {
            new Color(0.1f, 0.9f, 0.8f),
            new Color(0.6f, 0.1f, 0.9f),
            new Color(0.1f, 0.3f, 1f),
            new Color(0.1f, 1f, 0.3f)
        };

        for (int i = 0; i < 4; i++)
        {
            Material crystalMat = AssetDatabase.LoadAssetAtPath<Material>(matPaths[i]);

            GameObject crystal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crystal.name = "Crystal_" + i;
            crystal.transform.parent = crystalGroup.transform;
            crystal.transform.localPosition = positions[i];
            crystal.transform.localScale = new Vector3(0.3f, 0.6f, 0.3f);
            crystal.transform.localRotation = Quaternion.Euler(0, 45, 15);
            crystal.GetComponent<MeshRenderer>().sharedMaterial = crystalMat;

            SphereCollider col = crystal.AddComponent<SphereCollider>();
            col.radius = 3f;
            col.isTrigger = true;
            DestroyImmediate(crystal.GetComponent<BoxCollider>());

            CrystalHuntTarget cht = crystal.AddComponent<CrystalHuntTarget>();

            AudioSource audio = crystal.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.spatialBlend = 1f;

            GameObject lightObj = new GameObject("CrystalLight_" + i);
            lightObj.transform.parent = crystal.transform;
            lightObj.transform.localPosition = Vector3.up * 0.5f;
            Light l = lightObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = lightColors[i];
            l.intensity = 2f;
            l.range = 5f;
            l.shadows = LightShadows.None;
            lightObj.AddComponent<UniversalAdditionalLightData>();

            cht.glowLight = l;

            EditorUtility.SetDirty(crystal);
        }
    }

    static void SetupEntranceTrigger(Transform parent, Vector3 origin)
    {
        GameObject existing = GameObject.Find("GrottoEntrance");
        if (existing != null) DestroyImmediate(existing);

        GameObject entrance = new GameObject("GrottoEntrance");
        entrance.transform.parent = parent;
        entrance.transform.localPosition = new Vector3(0, 1.5f, 7f);

        BoxCollider col = entrance.AddComponent<BoxCollider>();
        col.size = new Vector3(4f, 3f, 1f);
        col.isTrigger = true;

        entrance.AddComponent<GrottoEntranceTrigger>();
        EditorUtility.SetDirty(entrance);
    }

    static void SetupGrottoGameManager(Transform parent)
    {
        GameObject existing = GameObject.Find("GrottoGameManager");
        if (existing != null) DestroyImmediate(existing);

        GameObject gm = new GameObject("GrottoGameManager");
        gm.transform.parent = parent;
        gm.transform.localPosition = Vector3.zero;

        GrottoGameManager mgr = gm.AddComponent<GrottoGameManager>();

        WaterStreamHaptic[] streams = Object.FindObjectsByType<WaterStreamHaptic>(FindObjectsSortMode.None);
        mgr.waterStreams = streams;
        foreach (var s in streams)
        {
            if (s.intensity == WaterStreamHaptic.StreamIntensity.Strong)
                mgr.correctStream = s;
        }

        mgr.blindObjects = Object.FindObjectsByType<HapticObject>(FindObjectsSortMode.None);
        mgr.crystals = Object.FindObjectsByType<CrystalHuntTarget>(FindObjectsSortMode.None);

        GameObject xrOrigin = GameObject.Find("Custom XR Origin");
        if (xrOrigin != null)
            mgr.playerTransform = xrOrigin.transform;

        EditorUtility.SetDirty(gm);
    }

    static void SetupGrottoUI(Transform parent)
    {
        GameObject existing = GameObject.Find("GrottoCanvas");
        if (existing != null) DestroyImmediate(existing);

        GameObject canvasObj = new GameObject("GrottoCanvas");
        canvasObj.transform.parent = parent;
        canvasObj.transform.localPosition = new Vector3(0, 3.5f, -3f);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(400, 200);
        canvasObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        GameObject instrObj = new GameObject("InstructionText");
        instrObj.transform.parent = canvasObj.transform;
        RectTransform instrRect = instrObj.AddComponent<RectTransform>();
        instrRect.anchoredPosition = new Vector2(0, 50);
        instrRect.sizeDelta = new Vector2(380, 100);
        TMP_Text instrText = instrObj.AddComponent<TextMeshProUGUI>();
        instrText.fontSize = 24;
        instrText.alignment = TextAlignmentOptions.Center;
        instrText.text = "Welcome to the Waterfall Grotto";

        GameObject timerObj = new GameObject("GrottoTimerText");
        timerObj.transform.parent = canvasObj.transform;
        RectTransform timerRect = timerObj.AddComponent<RectTransform>();
        timerRect.anchoredPosition = new Vector2(0, -50);
        timerRect.sizeDelta = new Vector2(200, 50);
        TMP_Text timerTextComp = timerObj.AddComponent<TextMeshProUGUI>();
        timerTextComp.fontSize = 30;
        timerTextComp.alignment = TextAlignmentOptions.Center;
        timerTextComp.text = "3:00";

        GameObject resultObj = new GameObject("GrottoResultText");
        resultObj.transform.parent = canvasObj.transform;
        RectTransform resultRect = resultObj.AddComponent<RectTransform>();
        resultRect.anchoredPosition = Vector2.zero;
        resultRect.sizeDelta = new Vector2(380, 80);
        TMP_Text resultText = resultObj.AddComponent<TextMeshProUGUI>();
        resultText.fontSize = 36;
        resultText.alignment = TextAlignmentOptions.Center;
        resultText.enabled = false;

        GrottoGameManager mgr = Object.FindFirstObjectByType<GrottoGameManager>();
        if (mgr != null)
        {
            mgr.instructionText = instrText;
            mgr.timerText = timerTextComp;
            mgr.resultText = resultText;
            EditorUtility.SetDirty(mgr);
        }

        EditorUtility.SetDirty(canvasObj);
    }

    static void SetupGrottoFloorTeleport(Transform parent)
    {
        GameObject floor = null;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name == "GrottoFloor")
            {
                floor = parent.GetChild(i).gameObject;
                break;
            }
        }
        if (floor == null) return;

        if (floor.GetComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>() == null)
            floor.AddComponent<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea>();

        EditorUtility.SetDirty(floor);
    }
#endif
}
