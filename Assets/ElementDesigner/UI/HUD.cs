using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HUD : MonoBehaviour
{
    public static bool LockedFocus = false;
    public static void LockFocus()
    {
        var flyCam = Camera.main.GetComponent<FlyCam>();

        if (flyCam != null)
            flyCam.LockAim = true;
    }
    public static void ClearFocus()
    {
        var flyCam = Camera.main.GetComponent<FlyCam>();

        if (flyCam != null)
            flyCam.LockAim = false;
    }
}
