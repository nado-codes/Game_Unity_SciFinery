using System;
using UnityEngine;

[Serializable]
public class Particle : Element
{
    [SerializeField]
    private Color color;

    ///<summary>Hexadecimal value representing the baseColor of a particle in world space</summary>
    public string Color => "#" + ColorUtility.ToHtmlStringRGBA(color);

    public float Size = 1;
}