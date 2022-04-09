using System;
using UnityEngine;

[Serializable]
public class Molecule : Element
{
    public Molecule(int id) : base(id)
    {

    }
    public int[] atomIds = new int[0];
}

