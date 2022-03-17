using System;
using UnityEngine;

[Serializable]
public class Particle : Element
{
    [SerializeField]
    private Color baseColor;

    ///<summary>Hexadecimal value representing the baseColor of a particle in world space</summary>
    public string BaseColor => "#" + ColorUtility.ToHtmlStringRGBA(baseColor);
}