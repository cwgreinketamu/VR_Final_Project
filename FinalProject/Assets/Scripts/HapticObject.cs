using UnityEngine;

public class HapticObject : MonoBehaviour
{
    public enum TextureType { Smooth, Rough, Heavy }
    public TextureType textureType = TextureType.Smooth;

    private Coroutine activeCoroutine;
    private bool isTouched = false;

    public void OnSelect()
    {
        if (isTouched) return;
        isTouched = true;

        HapticManager hm = HapticManager.Instance;
        if (hm == null) return;

        string typeStr = textureType.ToString().ToLower();
        activeCoroutine = hm.StartContinuousTexture(typeStr);
    }

    public void OnSelectExit()
    {
        isTouched = false;

        HapticManager hm = HapticManager.Instance;
        if (hm != null && activeCoroutine != null)
            hm.StopContinuousTexture(activeCoroutine);
        activeCoroutine = null;
    }

    public void StopHaptic()
    {
        isTouched = false;
        HapticManager hm = HapticManager.Instance;
        if (hm != null && activeCoroutine != null)
            hm.StopContinuousTexture(activeCoroutine);
        activeCoroutine = null;
    }
}
