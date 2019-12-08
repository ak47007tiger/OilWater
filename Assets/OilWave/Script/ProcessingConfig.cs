using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingConfig : MonoBehaviour
{
    public string mWidthKey = "PR_WIDTH";
    public string mHeightKey = "PR_HEIGHT";
    public string mMaterialPath = "wave_01";

    public bool IsRight()
    {
        if (string.IsNullOrEmpty(mWidthKey))
        {
            return false;
        }

        if (string.IsNullOrEmpty(mHeightKey))
        {
            return false;
        }

        if (string.IsNullOrEmpty(mMaterialPath))
        {
            return false;
        }

        return true;
    }
}
