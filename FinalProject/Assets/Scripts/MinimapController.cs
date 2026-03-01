using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public Transform playerTransform;
    public RawImage minimapImage;
    public RectTransform playerIcon;

    public Vector3 mapCenterWorldPos = new Vector3(0, 0, 25);
    public float mapWorldSizeX = 24f;
    public float mapWorldSizeZ = 24f;
    public int textureSize = 128;
    public float raycastHeight = 8f;

    private Texture2D mapTexture;

    void Start()
    {
        GenerateMapTexture();
        if (minimapImage != null)
        {
            minimapImage.texture = mapTexture;
            minimapImage.color = Color.white;
        }

        SetupPlayerArrow();
    }

    void SetupPlayerArrow()
    {
        if (playerIcon == null) return;

        Image img = playerIcon.GetComponent<Image>();
        if (img == null) return;

        int size = 32;
        Texture2D arrowTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = new Color(0, 0, 0, 0);
        Color arrowColor = new Color(0f, 1f, 0.4f, 1f);

        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = clear;

        int cx = size / 2;
        for (int y = 0; y < size; y++)
        {
            float t = (float)y / (size - 1);
            int halfWidth = Mathf.RoundToInt(Mathf.Lerp(size * 0.4f, 0, t));
            for (int x = cx - halfWidth; x <= cx + halfWidth; x++)
            {
                if (x >= 0 && x < size)
                    pixels[y * size + x] = arrowColor;
            }
        }

        arrowTex.SetPixels(pixels);
        arrowTex.Apply();

        Sprite arrowSprite = Sprite.Create(arrowTex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        img.sprite = arrowSprite;
        img.type = Image.Type.Simple;
        img.preserveAspect = true;
        img.color = Color.white;
    }

    void GenerateMapTexture()
    {
        mapTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        mapTexture.filterMode = FilterMode.Bilinear;

        Color floorColor = new Color(0.92f, 0.88f, 0.8f);
        Color wallColor = new Color(0.18f, 0.15f, 0.12f);
        Color bgColor = new Color(0.08f, 0.08f, 0.12f);

        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = bgColor;

        float probeY = 1.0f;
        float probeRadius = 0.45f;
        Vector3[] dirs = {
            Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
            (Vector3.forward + Vector3.left).normalized,
            (Vector3.forward + Vector3.right).normalized,
            (Vector3.back + Vector3.left).normalized,
            (Vector3.back + Vector3.right).normalized
        };

        for (int px = 0; px < textureSize; px++)
        {
            for (int py = 0; py < textureSize; py++)
            {
                float worldX = mapCenterWorldPos.x + ((float)px / textureSize - 0.5f) * mapWorldSizeX;
                float worldZ = mapCenterWorldPos.z + ((float)py / textureSize - 0.5f) * mapWorldSizeZ;
                Vector3 origin = new Vector3(worldX, probeY, worldZ);

                bool hitFloor = Physics.Raycast(origin, Vector3.down, probeY + 0.5f);
                if (!hitFloor)
                    continue;

                bool nearWall = false;
                foreach (Vector3 dir in dirs)
                {
                    if (Physics.Raycast(origin, dir, probeRadius))
                    {
                        nearWall = true;
                        break;
                    }
                }

                pixels[py * textureSize + px] = nearWall ? wallColor : floorColor;
            }
        }

        mapTexture.SetPixels(pixels);
        mapTexture.Apply();
    }

    void LateUpdate()
    {
        if (playerTransform == null || playerIcon == null || minimapImage == null) return;

        RectTransform mapRect = minimapImage.rectTransform;

        Vector3 offset = playerTransform.position - mapCenterWorldPos;
        float normX = offset.x / mapWorldSizeX + 0.5f;
        float normZ = offset.z / mapWorldSizeZ + 0.5f;

        normX = Mathf.Clamp01(normX);
        normZ = Mathf.Clamp01(normZ);

        float iconX = (normX - 0.5f) * mapRect.rect.width;
        float iconY = (normZ - 0.5f) * mapRect.rect.height;
        playerIcon.anchoredPosition = new Vector2(iconX, iconY);

        float yRot = playerTransform.eulerAngles.y;
        playerIcon.localRotation = Quaternion.Euler(0f, 0f, -yRot);
    }
}
