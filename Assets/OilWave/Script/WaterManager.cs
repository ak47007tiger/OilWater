using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterManager : MonoBehaviour
{
    public static WaterManager Instance;

    public Material mShowSurfaceMaterial;

    private ConfigManager mConfigManager = new ConfigManager();

    CustomRenderTexture mCustomRenderTexture;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Adapt();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDestroy()
    {
        if (this == Instance && null != mCustomRenderTexture)
        {
#if UNITY_EDITOR
            DestroyImmediate(mCustomRenderTexture);
#else
            Destroy(mCustomRenderTexture);
#endif
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.I))
        {
            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Loading")
            {
                SceneManager.LoadScene("Demo");
            }
            else if (sceneName == "Demo")
            {
                SceneManager.LoadScene("Demo1");
            }
            else if (sceneName == "Demo1")
            {
                SceneManager.LoadScene("Demo2");
            }
            else
            {
                SceneManager.LoadScene("Demo");
            }
        }
    }
#endif

    void OnActiveSceneChanged(Scene a, Scene b)
    {
        Debug.Log(string.Format("scene change {0} -> {1}",a.name, b.name));
        if (mCustomRenderTexture == null)
        {
            Adapt();
        }
        else
        {
            UpdateMaterial();
        }
    }

    public void Adapt()
    {
        var cfg = Camera.main.GetComponent<ProcessingConfig>();
        if (null == cfg)
        {
            return;
        }

        var targetResolution = mConfigManager.LoadTargetResolution(cfg.mWidthKey, cfg.mHeightKey);
        var width = targetResolution.x;
        var height = targetResolution.y;

        Debug.Log(string.Format("target resolution: {0},{1}", width, height));

        var adapt = Camera.main.GetComponent<DemoForResolutionAdapt>();
        if (adapt == null)
        {
            return;
        }
        mCustomRenderTexture = adapt.CreateTexture(width, height);

        mShowSurfaceMaterial.SetTexture("_WaveTex", mCustomRenderTexture);

        var waveMat = Resources.Load<Material>(cfg.mMaterialPath);
        Attach(mCustomRenderTexture, waveMat);

        var processing = Camera.main.GetComponent<OilWavePostProcessing>();
        if (processing == null)
        {
            return;
        }
        processing.UpdateRenderInformation(mCustomRenderTexture, waveMat, mShowSurfaceMaterial);
    }

    public void Attach(CustomRenderTexture customRenderTexture, Material waveMat)
    {
        customRenderTexture.material = waveMat;
        customRenderTexture.shaderPass = waveMat.FindPass("Update");
    }

    public void UpdateMaterial()
    {
        var cfg = Camera.main.GetComponent<ProcessingConfig>();
        if (cfg == null)
        {
            return;
        }

        var waveMat = Resources.Load<Material>(cfg.mMaterialPath);

        if (mCustomRenderTexture == null)
        {
            return;
        }

        Attach(mCustomRenderTexture, waveMat);

        var processing = Camera.main.GetComponent<OilWavePostProcessing>();
        if (processing == null)
        {
            return;
        }

        processing.UpdateRenderInformation(mCustomRenderTexture, waveMat, mShowSurfaceMaterial);
    }
}