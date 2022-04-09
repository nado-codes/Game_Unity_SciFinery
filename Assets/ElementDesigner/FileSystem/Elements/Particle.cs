using System;
using UnityEngine;

///<summary>[Data] Any component element which is used to construct an Atom</summary>
[Serializable]
public class Particle : Element
{
    public Particle() : base(1)
    {

    }
    [SerializeField]
    private Color color;
    ///<summary>Hexadecimal value representing the baseColor of a particle in world space</summary>
    public string Color
    {
        get => "#" + ColorUtility.ToHtmlStringRGBA(color);
        set
        {
            ColorUtility.TryParseHtmlString(value, out Color newColor);
            color = newColor;
        }
    }

    ///<summary>How physically big the particle is in world space</summary>
    public float Size = 1;
}