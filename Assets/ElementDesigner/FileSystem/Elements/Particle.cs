using System;

///<summary>[Data] Any component element which is used to construct an Atom</summary>
[Serializable]
public class Particle : Element
{
    public Particle()
    {
        Name = "NewParticle";
        ElementType = ElementType.Particle;
        // TODO
        //SubElementType = ElementType.Quark;
    }

    ///<summary>How physically big the particle is in world space</summary>
    public float Size = 1;
}