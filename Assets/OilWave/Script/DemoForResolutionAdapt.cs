using System;
using UnityEngine;

public class DemoForResolutionAdapt : MonoBehaviour
{
    public float mSizeScale = 0.2f;

    public int GetSizeForMemory(int size)
    {
        return Convert.ToInt32(size * mSizeScale);
    }

    public CustomRenderTexture CreateTexture(int width, int height)
    {
        var widthInt = GetSizeForMemory(width);
        var heightInt = GetSizeForMemory(height);
        var customRenderTexture = CreateTextureForScreen(widthInt, heightInt);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(customRenderTexture, "Assets/OilWave/Temp/runtime.asset");
#endif
        return customRenderTexture;
    }

    public static CustomRenderTexture CreateTextureForScreen(int width, int height)
    {
        var texture = new CustomRenderTexture(width, height, RenderTextureFormat.RGFloat);
        texture.autoGenerateMips = true;
        texture.useDynamicScale = true;
        texture.wrapMode = TextureWrapMode.Mirror;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 0;
        texture.initializationMode = CustomRenderTextureUpdateMode.OnDemand;
        texture.initializationColor = Color.black;
        texture.updateMode = CustomRenderTextureUpdateMode.OnDemand;
        texture.doubleBuffered = true;
        texture.Create();
        return texture;
    }
}