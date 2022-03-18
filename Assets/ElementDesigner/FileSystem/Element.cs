using System;

[Serializable]
public class Element
{
    public int Id;

    public string ShortName;
    public string Name;
    public float Weight;
    public float Charge;
    public ElementType Type;


    // TODO: elements should have a list or dictionary for their properties, so all the properties above will be serialized/deserialized
    // seperately when saving/loading
}