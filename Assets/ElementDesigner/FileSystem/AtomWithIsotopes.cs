using System;
using System.Collections.Generic;

public class AtomWithIsotopes : Atom
{
    public List<Atom> isotopes { get; private set; } = new List<Atom>();
}