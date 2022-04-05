using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class FileSystem : MonoBehaviour
{
    const string fileExtension = "ed";
    const string elementsRoot = "./Elements";

    private static FileSystem instance;
    public static FileSystem Instance
    {
        get
        {
            if (instance == null)
            {
                var newFileSystem = FindObjectOfType<FileSystem>();

                if (newFileSystem == null)
                    newFileSystem = Camera.main.gameObject.AddComponent<FileSystem>();

                instance = newFileSystem;
            }

            return instance;
        }
    }

    // ACTIVE ELEMENT
    private string activeElementFileName => $"{activeElement.ShortName.ToLower()}{activeElement.Id}";

    private Element activeElement { get; set; }
    public static Element ActiveElement
    {
        get => Instance.activeElement;
        set => Instance.activeElement = value;
    }

    // LOADED ELEMENTS

    private List<Element> loadedElements = new List<Element>();
    public static List<Element> LoadedElements
    {
        get => Instance.loadedElements;
    }

    public static IEnumerable<Element> LoadElementsOfType(ElementType elementType)
     => elementType switch
     {
         ElementType.Particle => loadParticles(),
         ElementType.Atom => loadElements<Atom>(),
         ElementType.Molecule => loadElements<Molecule>(),
         _ => throw new NotImplementedException($"Element type \"{elementType.ToString()}\" is not implemented in call to FileSystem.LoadElementsOfType")
     };

    private static IEnumerable<Particle> loadParticles()
    {
        var protonParticle = new Particle()
        {
            Id = 1,
            Name = "Proton",
            Weight = .001f,
            Charge = 1,
            Size = 1,
            Type = ElementType.Particle,
            Color = "#00FFFA"
        };
        var neutronParticle = new Particle()
        {
            Id = 2,
            Name = "Neutron",
            Weight = 1,
            Charge = 0,
            Size = 1,
            Type = ElementType.Particle,
            Color = "#006F05"
        };
        var electronParticle = new Particle()
        {
            Id = 3,
            Name = "Electron",
            Weight = 10f,
            Charge = -1,
            Size = .5f,
            Type = ElementType.Particle,
            Color = "#FF0000"
        };

        var defaultParticles = new Particle[] { protonParticle, neutronParticle, electronParticle };
        return defaultParticles.Concat(loadElements<Particle>());
    }

    public static T CreateElementOfType<T>() where T : Element
    {
        if (typeof(T) == typeof(Atom))
        {
            var newAtom = new Atom();
            newAtom.Id = 1;
            newAtom.Name = "NewAtom";
            newAtom.ShortName = "NE";
            newAtom.ProtonCount = 1;
            newAtom.ElectronCount = 1;
            newAtom.ParticleIds = new int[] { 1, 3 };
            newAtom.Type = ElementType.Atom;

            Instance.activeElement = newAtom;

            return newAtom as T;
        }
        else
            throw new NotImplementedException($"Element of type ${typeof(T)} is not yet implemented in call to Editor.CreateNewElementOfType");
    }

    // TODO: Use ActiveElementAs to e.g. convert ActiveElement to Atom (where possible) and access "charge" to increase charge
    // when a particle is added during Atom design
    public static T ActiveElementAs<T>() where T : Element
    {
        if (ActiveElement.GetType() != typeof(T))
            throw new ApplicationException("Cannot convert object of type {ActiveElement.GetType()} to {typeof(T)}");

        return ActiveElement as T;
    }

    public static void UpdateActiveElement()
    {
        if (Editor.SubElements.Any(el => el.Data == null))
            throw new ApplicationException("At least one WorldElement is missing data in call to FileSystem.UpdateActiveElement");

        if (ActiveElement is Atom)
            updateActiveAtom(ActiveElement as Atom);
        else
            throw new NotImplementedException($"Element of type ${ActiveElement.GetType().FullName} is not yet implemented in call to Editor.UpdateActiveElement");
    }
    private static void updateActiveAtom(Atom activeAtomData)
    {
        if (Editor.DesignType != ElementType.Atom)
            throw new ApplicationException($"Editor DesignType must be Atom in call to FileSystem.updateActiveAtom, got {Editor.DesignType}");

        var newParticleIds = Editor.SubElements.Select(el => el.Data.Id);
        activeAtomData.ParticleIds = newParticleIds.ToArray();
    }
    public void SaveActiveElement()
    {
        // .. make sure the elements directory exists
        var elementDirectoryPath = getElementDirectoryPathForType(activeElement.Type);
        if (!Directory.Exists(elementDirectoryPath))
            Directory.CreateDirectory(elementDirectoryPath);

        // .. if the main atom doesn't exist, create it
        var elementFilePath = $"{elementDirectoryPath}/{activeElementFileName}";

        if (activeElement is Atom)
            saveAtom(activeElement as Atom, elementFilePath);
        else
        {
            var activeElementJSON = JsonUtility.ToJson(activeElement);
            // instance.LoadedAtoms.Insert(ActiveElement.Id - 1, ActiveElement);
            File.WriteAllText($"{elementFilePath}.{fileExtension}", activeElementJSON);
            Debug.Log($"Saved active element {activeElement.Name} at {DateTime.Now}");
            TextNotification.Show("Save Successful");
        }
    }
    private void saveAtom(Atom atomData, string mainElementPath)
    {
        var activeAtom = activeElement as Atom;

        // .. In this context, a "Neutron" is any neutral particle, or a particle with Charge=0
        var allParticles = loadParticles();
        var activeAtomParticles = allParticles.Where(particle => activeAtom.ParticleIds.Any(id => id == particle.Id));
        var atomDataParticles = allParticles.Where(particle => atomData.ParticleIds.Any(id => id == particle.Id));
        var activeAtomNeutronsCount = activeAtomParticles.Where(particle => particle.Charge == 0).Count();
        var atomDataNeutronsCount = atomDataParticles.Where(particle => particle.Charge == 0).Count();

        var hasDifferentNeutronCount = atomDataNeutronsCount != activeAtomNeutronsCount;
        var activeAtomIsIsotope = atomData.Name == activeAtom.Name && hasDifferentNeutronCount;

        var elementExists = File.Exists(getElementFilePath(atomData));

        if (elementExists)
        {
            var existingElementJSON = File.ReadAllText($"{mainElementPath}.{fileExtension}");
            var existingElement = JsonUtility.FromJson<Atom>(existingElementJSON);

            if (activeAtomIsIsotope)
            {
                // TODO: implement saving isotopes
                // .. make sure the atom's isotope directory exists
                /* if (!Directory.Exists(mainElementPath))
                    Directory.CreateDirectory(mainElementPath);

                var isotopePath = GetActiveAtomIsotopeFileName();
                var isotopeExists = File.Exists(isotopePath); */

                // TODO: 
                /* if (!isotopeExists) // .. confirm create isotope
                {
                    var dialogBody = @"You're about to create the isotope {isotopeShortName} for {mainAtomName}, 
                    with {neutronCount} neutrons. Do you wish to continue?";
                    DialogYesNo.Open("Confirm Create Isotope", dialogBody, confirmSaveIsotope);
                }
                else
                    confirmSaveIsotope(); // .. overwrite isotope */
            }
            else // .. overwrite the main atom
            {
                // TODO: tidy up this duplicate code (same as line 205-209)
                var activeAtomJSON = JsonUtility.ToJson(activeElement);
                File.WriteAllText($"{mainElementPath}.{fileExtension}", activeAtomJSON);
                Debug.Log($"Saved active atom {activeElement.Name} at {DateTime.Now}");
                TextNotification.Show("Save Successful");
            }
        }
        else
        {
            // TODO: tidy up this duplicate code (same as line 205-209)
            var activeAtomJSON = JsonUtility.ToJson(activeElement);
            File.WriteAllText($"{mainElementPath}.{fileExtension}", activeAtomJSON);
            Debug.Log($"Saved active atom {activeElement.Name} at {DateTime.Now}");
            TextNotification.Show("Save Successful");
        }
    }
    // TODO: implement saving isotopes
    // .. Create an isotope for a particlar atom, by creating a directory to store isotopes and then saving the file inside it
    private void confirmSaveIsotope()
    {
        /* var activeAtomJSON = JsonUtility.ToJson(activeElement);
        var isotopePath = GetActiveAtomIsotopeFileName();
        var isotopeExists = File.Exists(isotopePath);

        if (!isotopeExists)
            LoadedAtoms[ActiveAtom.Number - 1].isotopes.Add(ActiveAtom);
        else
        {
            var isotopeCount = LoadedAtoms[ActiveAtom.Number - 1].isotopes.Count;
            var indexInIsotopes = LoadedAtoms[ActiveAtom.Number - 1].isotopes.FindIndex(0, isotopeCount, a => a.NeutronCount == ActiveAtom.NeutronCount);

            LoadedAtoms[ActiveAtom.Number - 1].isotopes[indexInIsotopes] = ActiveAtom;
        }

        File.WriteAllText(isotopePath, activeAtomJSON);
        Debug.Log($"Saved isotope {ActiveElement.Name} with neutrons {ActiveElement.NeutronCount} at {DateTime.Now}");
        TextNotification.Show("Save Successful"); */
    }
    public static void DeleteElement(Element elementData)
    {
        LoadedElements.Remove(elementData);

        var mainElementFilePath = getElementFilePath(elementData);

        if (!File.Exists(mainElementFilePath))
            throw new ApplicationException($"The file at path \"{mainElementFilePath}\" doesn't exist in call to FileSystem.DeleteElement");

        File.Delete(mainElementFilePath);

        // TODO: implement deleting isotopes
        // File.Delete(!elementData.IsIsotope ? GetMainElementFilePath(elementData) : GetIsotopeFilePath(elementData));
        TextNotification.Show("Delete Successful");
    }

    private static IEnumerable<T> loadElements<T>() where T : Element
    {
        var typeName = typeof(T).FullName;
        var elementsOfTypeDirPath = getElementDirectoryPathForTypeName(typeName);

        if (!Directory.Exists(elementsOfTypeDirPath))
            return new List<T>();

        var files = Directory.GetFiles(elementsOfTypeDirPath, $"*.{fileExtension}");
        var loadedElements = files.Select(elementFilePath =>
        {
            string elementJSON = File.ReadAllText(elementFilePath);
            var elementFromJSON = JsonUtility.FromJson<T>(elementJSON);

            return elementFromJSON;

            // TODO: Isotopes can probably be loaded seperately as will be too difficult to do here... we'll end up returning an array of arrays
            /* if (typeof(T) == typeof(Atom))
                return new Element[] { elementFromJSON }.Concat(loadAtomIsotopes(elementFilePath));
            else */
        });

        return loadedElements;
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
    private static string getElementDirectoryPathForType(ElementType type)
      => $"{elementsRoot}/{type}";
    private static string getElementDirectoryPathForTypeName(string typeName)
    {
        if (!Enum.TryParse(typeName, out ElementType type))
            throw new ArgumentException($"Element with typename {typeName} doesn't exist in call to FileSystem.getElementDirectoryPathForTypeName");

        return $"{elementsRoot}/{typeName}";
    }
    private static string getElementFilePath(Element element)
    {
        var elementFileName = $"{element.ShortName.ToLower()}{element.Id}";
        return $"{getElementDirectoryPathForType(element.Type)}/{elementFileName}.{fileExtension}";
    }

    /* private static string GetIsotopeFilePath(Atom atom)
    {
        var mainAtomFilePath = getElementFilePath(atom);
        var mainAtomDirectoryName = mainAtomFilePath.Split(new string[1] { $".{fileExtension}" }, StringSplitOptions.None)[0];

        var isotopeFileName = atom.ShortName.ToLower() + atom.Id;
        var isotopeNumber = atom.NeutronCount < 0 ? "m" + (atom.NeutronCount * -1) : atom.NeutronCount.ToString();
        return $"{mainAtomDirectoryName}/{isotopeFileName}{isotopeNumber}.{fileExtension}";
    } */
    private string GetActiveAtomIsotopeFileName()
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
    }
}
