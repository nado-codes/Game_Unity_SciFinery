using System.Collections.Generic;
using System.IO;
using System;

public class FileSystemAtomLoader : FileSystemElementLoader
{
    public static IEnumerable<Atom> LoadAtoms()
    {
        // TODO: Implement loading isotopes
        return loadElements<Atom>();
    }
    // TODO: Implement loading isotopes
    private static IEnumerable<Atom> loadAtomIsotopes(string path)
    {
        if (!File.Exists(path))
            throw new ArgumentException($"No atom exists at path {path} in call to FileSystem.loadAtom");

        // string elementJSON = File.ReadAllText(file);
        // var elementFromJSON = JsonUtility.FromJson<T>(elementJSON);

        /* var isotopeDirectoryName = getElementFilePath(elementFromJSON).Split(new string[1] { $".{fileExtension}" }, StringSplitOptions.None)[0];

        // .. TODO: If an atom has isotopes, we can probably just save them as part of the atom file itself
        if (Directory.Exists($"{isotopeDirectoryName}/"))
        {
            var isotopes = Directory.GetFiles($"{isotopeDirectoryName}/", $"*.{fileExtension}");
            var isotopeAtoms = isotopes.Select(isotope =>
            {
                var isotopeJSON = File.ReadAllText(isotope);
                var isotopeAtom = JsonUtility.FromJson<Atom>(isotopeJSON);
                isotopeAtom.IsIsotope = true;
                return isotopeAtom;
            });

            // .. TODO: this might break because we're casting an Atom to it's base type so it may lose the "IsIsotope" property
            // .. Need to QA this
            loadedElements.AddRange(isotopeAtoms.Cast<T>());
        }

        loadedElements.Add(elementFromJSON); */

        return new List<Atom>();
    }

    /* private string GetActiveAtomIsotopeFileName()
    {
        if (activeElement is Atom)
        {
            var mainAtomPath = $"{elementsRoot}/{activeElementFileName}";
            var activeAtom = activeElement as Atom;

            var allNeutronParticles = loadParticles().Where(p => p.Charge == 0);
            var allNeutronParticleIds = allNeutronParticles.Select(p => p.Id);
            var activeAtomNeutronCount = activeAtom.ParticleIds.Count(id => allNeutronParticleIds.Contains(id));
            var isotopeNumber = activeAtomNeutronCount < 0 ? "m" + (activeAtomNeutronCount * -1) : activeAtomNeutronCount.ToString();
            return $"{mainAtomPath}/{activeElementFileName}{isotopeNumber}.{fileExtension}";
        }
        else
            throw new ApplicationException($"ActiveElement must be an atom in call to FileSystem.GetActiveAtomIsotopeFileName, got {activeElement.GetType().FullName}");
    } */
}