using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DC
{
  public class WaterWave01 : MonoBehaviour
  {
    public CustomRenderTexture texture;

    public int countPreFrame = 1;

    public Vector2 center = new Vector2(0.5f, 0.5f);
    public Vector2 range = new Vector2(1, 1);

    public Material waveMat;

    public Material surfaceMat;

    void Start()
    {
      texture.Initialize();
    }

    void Update()
    {
      texture.ClearUpdateZones();
      UpdateZones();
      texture.Update(countPreFrame);
    }

    void UpdateZones()
    {
      bool leftClick = Input.GetMouseButton(0);
      if (!leftClick)return;

      RaycastHit hit;
      var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hit))
      {
        var clickZone = new CustomRenderTextureUpdateZone();
        clickZone.needSwap = true;
        clickZone.passIndex = 1;
        clickZone.rotation = 0f;
        clickZone.updateZoneCenter = new Vector2(center.x, center.y);
        clickZone.updateZoneSize = new Vector2(range.x, range.y);

        var click = new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0);
        Debug.Log(click);
        waveMat.SetVector("_ClickPoint", click);

        texture.SetUpdateZones(new CustomRenderTextureUpdateZone[] { clickZone });
      }
    }

  }
}
