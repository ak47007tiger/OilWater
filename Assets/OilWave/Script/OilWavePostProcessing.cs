using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OilWavePostProcessing : MonoBehaviour
{
    public CustomRenderTexture mCrTexture;
    public Material mWaveMat;
    public Material mSurfaceMat;

    /// <summary>
    /// 是否响应控制鼠标或者手指的点击
    /// </summary>
    public bool handleInput = true;

    public Vector2 center = new Vector2(0.5f, 0.5f);
    public Vector2 range = new Vector2(1, 1);

    private CustomRenderTextureUpdateZone[] updateZones;

    private Queue<Vector2> pointQueue;

    private Vector2 lastPoint;

    public void UpdateRenderInformation(CustomRenderTexture cTexture, Material wave, Material show)
    {
        mCrTexture = cTexture;
        mWaveMat = wave;
        mSurfaceMat = show;
    }

    public void UpdateRenderInformation(CustomRenderTexture cTexture, Material wave)
    {
        mCrTexture = cTexture;
        mWaveMat = wave;
    }

    public void UpdateRenderInformation(Material wave)
    {
        mWaveMat = wave;
    }

    void Start()
    {
        pointQueue = new Queue<Vector2>();

        updateZones = new CustomRenderTextureUpdateZone[] {CreateUpdateZone()};
        mCrTexture.Initialize();
        mCrTexture.SetUpdateZones(updateZones);
        // InvokeRepeating("Test", 0, 1);
    }

    void Test()
    {
        var points = new Vector2[10];
        for (var i = 0; i < points.Length; i++)
        {
            points[i] = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        PushPoints(points);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mSurfaceMat != null)
        {
            Graphics.Blit(src, dest, mSurfaceMat);
        }
        else
        {
            Graphics.Blit(src,dest);
        }
    }

    void Update()
    {
        if (handleInput)
        {
            CreatePoints();
        }

        if (null == mCrTexture || mWaveMat == null)
        {
            return;
        }

        mCrTexture.ClearUpdateZones();
        if (pointQueue.Count > 0)
        {
            var point = pointQueue.Dequeue();
            mWaveMat.SetVector("_ClickPoint", new Vector4(point.x, point.y, 0, 0));
            mCrTexture.SetUpdateZones(updateZones);
            if (point == lastPoint)
            {
                mCrTexture.Update();
            }

            lastPoint = point;
        }
        else
        {
            mCrTexture.Update();
        }
    }

    bool CreatePoints()
    {
        var points = EffectInput.GetAllPoints();
        if (points.Length == 0)
        {
            return false;
        }

        PushPoints(points);
        return true;
    }

    public void PushPoints(Vector2[] points)
    {
        if (points == null || points.Length == 0)
        {
            return;
        }

        for (var i = 0; i < points.Length; i++)
        {
            pointQueue.Enqueue(points[i]);
        }
    }

    CustomRenderTextureUpdateZone CreateUpdateZone()
    {
        var clickZone = new CustomRenderTextureUpdateZone();
        clickZone.needSwap = true;
        clickZone.passIndex = 1;
        clickZone.rotation = 0f;
        clickZone.updateZoneCenter = new Vector2(center.x, center.y);
        clickZone.updateZoneSize = new Vector2(range.x, range.y);
        return clickZone;
    }
}