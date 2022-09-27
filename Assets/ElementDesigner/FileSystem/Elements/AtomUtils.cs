using System.Linq;
using System.Collections.Generic;

public static class AtomUtils
{
    // TODO: this needs to go in a config file
    // .. The total number of electrons at each "shell" for non-metals. Anything outside of this is a METAL.
    private static List<int> nonMetalElectronCounts = new List<int> { 2, 10, 28, 60, 110, 182 };
    public static Classification GetClassification(this Atom atom)
    {
        var numElectrons = atom.Children.Count(c => c.Charge < 0);

        // .. automatically update the non-metal electron counts to classify an atom with more electrons
        while (numElectrons > nonMetalElectronCounts.Last())
        {
            var newShellElectrons = 2 * (nonMetalElectronCounts.Count ^ 2);
            nonMetalElectronCounts.Add(nonMetalElectronCounts.Last() + (int)newShellElectrons);
        }

        return numElectrons < 2 || nonMetalElectronCounts.Contains(numElectrons) ? Classification.NonMetal : Classification.Metal;
    }
}