using System.Linq;

public static class AtomUtils
{
    // TODO: this needs to go in a config file
    private static int[] nonMetalCounts = new int[6] { 2, 10, 28, 60, 110, 182 };
    public static Classification GetClassification(this Atom atom)
    {
        var numElectrons = atom.Children.Count(c => c.Charge < 0);
        return numElectrons > nonMetalCounts[0] && !nonMetalCounts.Any(x => x == numElectrons) ?
            Classification.Metal : Classification.NonMetal;
    }
}