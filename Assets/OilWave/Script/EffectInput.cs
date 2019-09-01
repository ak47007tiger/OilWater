using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EffectInput : MonoBehaviour
{
  public static bool Touched()
  {
    if (Application.isMobilePlatform)
    {
      return Input.touchCount > 0 && Input.touches[0].pressure > 1;
    }
    return Input.GetMouseButton(0);
  }

  public static Vector2[] GetAllPoints()
  {
    if (Application.isMobilePlatform)
    {
      return GetAllPointsMobile();
    }
    else
    {
      return GetAllPointsPC();
    }
  }

  public static Vector2[] GetAllPointsPC()
  {
    if (!Input.GetMouseButton(0))
    {
      return new Vector2[0];
    }
    var pos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
    return new Vector2[] { pos };
  }

  public static Vector2[] GetAllPointsMobile()
  {
    var tc = Input.touchCount;
    var screen = new Vector2(Screen.width, Screen.height);
    var touchPoints = new Vector2[tc];
    for (var i = 0; i < tc; i++)
    {
      var touch = Input.touches[i];
      touchPoints[i] = new Vector2(touch.position.x / screen.x, touch.position.y / screen.y);
    }
    return touchPoints;
  }
}
