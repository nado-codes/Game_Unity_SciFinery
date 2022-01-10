using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HUD : MonoBehaviour
{
    public static bool LockedFocus = false;
    public static void LockFocus() => LockedFocus = true;
    public static void ClearFocus() => LockedFocus = false;
}
