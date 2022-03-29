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

    private static FileSystem _instance;
    public static FileSystem instance
    {
        get
        {
            if (_instance == null)
            {
                var newFileSystem = FindObjectOfType<FileSystem>();

                if (newFileSystem == null)
                    newFileSystem = Camera.main.gameObject.AddComponent<FileSystem>();

                _instance = newFileSystem;
            }

            return _instance;
        }
    }

    // ACTIVE ELEMENT
    private string activeElementFileName => ActiveElementAs<Atom>().ShortName.ToLower() + "_" + ActiveElementAs<Atom>().Id;
    public Element ActiveElement { get; set; }

    // LOADED ELEMENTS

    public List<Atom> LoadedAtoms { get; private set; } = new List<Atom>();

    public static IEnumerable<Element> LoadElementsOfType(ElementType elementType)
    {
        if (elementType == ElementType.Particle)
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
                Weight = 3.5f,
                Charge = -1,
                Size = .5f,
                Type = ElementType.Particle,
                Color = "#AD0005"
            };

            var defaultParticles = new Particle[] { protonParticle, neutronParticle, electronParticle };
            return defaultParticles.Concat(loadElements<Particle>());
        }
        else if (elementType == ElementType.Atom)
            return loadElements<Atom>();
        else if (elementType == ElementType.Molecule)
            return loadElements<Molecule>();
        else
            throw new NotImplementedException($"Element type \"{elementType.ToString()}\" is not implemented in call to FileSystem.LoadElementsOfType");
    }
    public U ActiveElementAs<U>() where U : Element => ActiveElement as U;
    public static T CreateNewElementOfType<T>() where T : Element
    {
        if (typeof(T) == typeof(Atom))
        {
            var newAtom = new Atom();
            newAtom.Number = 1;
            newAtom.Name = "NewAtom";
            newAtom.ShortName = "NE";
            newAtom.ProtonCount = 1;
            newAtom.ElectronCount = 1;
            newAtom.ParticleIds = new int[] { 1, 3 };
            newAtom.Type = ElementType.Atom;

            instance.ActiveElement = newAtom;

            return newAtom as T;
        }
        else
            throw new NotImplementedException($"Element of type ${typeof(T)} is not yet implemented in call to Editor.CreateNewElementOfType");
    }
    public void SaveActiveElement()
    {
        // .. make sure the elements directory exists
        var elementsOfTypeDir = $"{elementsRoot}/{ActiveElement.Type}/";
        if (!Directory.Exists(elementsOfTypeDir))
            Directory.CreateDirectory(elementsOfTypeDir);

        // .. if the main atom doesn't exist, create it
        var mainElementPath = $"{elementsOfTypeDir}/{activeElementFileName}";

        var elementExists = File.Exists(GetMainElementFilePath(ActiveElement));

        if (elementExists)
        {
            var existingElementJSON = File.ReadAllText($"{mainElementPath}.{fileExtension}");
            var existingElement = JsonUtility.FromJson<Atom>(existingElementJSON);

            
            // var activeAtomIsIsotope = existingElement.Name == ActiveElementAs<Atom>().Name && existingElement.NeutronCount != ActiveElementAs<Atom>().NeutronCount;

            /* if (activeAtomIsIsotope)
            {
                // .. make sure the atom's isotope directory exists
                if (!Directory.Exists(mainElementPath))
                    Directory.CreateDirectory(mainElementPath);

                var isotopePath = GetActiveAtomIsotopeFileName();
                var isotopeExists = File.Exists(isotopePath);

                if (!isotopeExists) // .. confirm create isotope
                {
                    var dialogBody = @"You're about to create the isotope {isotopeShortName} for {mainAtomName}, 
                    with {neutronCount} neutrons. Do you wish to continue?";

                    DialogYesNo.Open("Confirm Create Isotope", dialogBody, ConfirmSaveIsotope);
                }
                else
                    ConfirmSaveIsotope(); // .. overwrite isotope


            }
            else // .. overwrite the main atom
            {
              var activeAtomJSON = JsonUtility.ToJson(ActiveElement);
                // instance.LoadedAtoms[ActiveElementAs<Atom>().Id - 1] = ActiveElement as Atom;
                File.WriteAllText($"{mainElementPath}.{fileExtension}", activeAtomJSON);
                Debug.Log($"Saved active atom {ActiveElementAs<Atom>().Name} at {DateTime.Now}");
                TextNotification.Show("Save Successful");  
            } */
        }
        else
        {
            var activeAtomJSON = JsonUtility.ToJson(ActiveElement);
            // instance.LoadedAtoms.Insert(ActiveElementAs<Atom>().Id - 1, ActiveElement);
            File.WriteAllText($"{mainElementPath}.{fileExtension}", activeAtomJSON);
            Debug.Log($"Saved active atom {ActiveElementAs<Atom>().Name} at {DateTime.Now}");
            TextNotification.Show("Save Successful");
        }
    }
    private void saveAtom(Atom atomData)
    {
        var elementExists = File.Exists(GetMainElementFilePath(atomData));
    }
    // .. Create an isotope for a particlar atom, by creating a directory to store isotopes and then saving the file inside it
    public void ConfirmSaveIsotope()
    {
        var activeAtomJSON = JsonUtility.ToJson(ActiveElement);

        var isotopePath = GetActiveAtomIsotopeFileName();
        var isotopeExists = File.Exists(isotopePath);

        // TODO: implement saving isotopes
        /* if(!isotopeExists)
            LoadedAtoms[ActiveAtom.Number-1].isotopes.Add(ActiveAtom);
        else
        {
            var isotopeCount = LoadedAtoms[ActiveAtom.Number-1].isotopes.Count;
            var indexInIsotopes = LoadedAtoms[ActiveAtom.Number-1].isotopes.FindIndex(0,isotopeCount,a => a.NeutronCount == ActiveAtom.NeutronCount);

            LoadedAtoms[ActiveAtom.Number-1].isotopes[indexInIsotopes] = ActiveAtom;
        } */
        File.WriteAllText(isotopePath, activeAtomJSON);
        Debug.Log($"Saved isotope {ActiveElementAs<Atom>().Name} with neutrons {ActiveElementAs<Atom>().NeutronCount} at {DateTime.Now}");
        TextNotification.Show("Save Successful");
    }
    public void DeleteAtom(Atom atom)
    {
        LoadedAtoms.Remove(atom);
        File.Delete(!atom.IsIsotope ? GetMainElementFilePath(atom) : GetIsotopeFilePath(atom));
        TextNotification.Show("Delete Successful");
    }

    private static IEnumerable<T> loadElements<T>() where T : Element
    {
        var typeName = typeof(T).FullName;

        var elementsOfTypeDir = $"{elementsRoot}/{typeName}/";
        if (!Directory.Exists(elementsOfTypeDir))
            return new List<T>();

        var files = Directory.GetFiles(elementsOfTypeDir, $"*.{fileExtension}");
        var loadedElements = new List<T>();

        foreach (string file in files)
        {
            string elementJSON = File.ReadAllText(file);
            var elementFromJSON = JsonUtility.FromJson<T>(elementJSON);

            if (typeof(T) == typeof(Atom))
            {
                var mainAtomDirectoryName = GetMainElementFilePath(elementFromJSON).Split(new string[1] { $".{fileExtension}" }, StringSplitOptions.None)[0];

                // .. TODO: If an atom has isotopes, we can probably just save them as part of the atom file itself
                if (Directory.Exists($"{mainAtomDirectoryName}/"))
                {
                    var isotopes = Directory.GetFiles($"{mainAtomDirectoryName}/", $"*.{fileExtension}");
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

                loadedElements.Add(elementFromJSON);
            }
        }

        return loadedElements;
    }
    private static string GetMainElementFilePath(Element element)
    {
        var elementFileName = element.ShortName.ToLower() + "_" + element.Id;
        return $"{elementsRoot}/{elementFileName}.{fileExtension}";
    }
    private static string GetIsotopeFilePath(Atom atom)
    {
        var mainAtomFilePath = GetMainElementFilePath(atom);
        var mainAtomDirectoryName = mainAtomFilePath.Split(new string[1] { $".{fileExtension}" }, StringSplitOptions.None)[0];

        var isotopeFileName = atom.ShortName.ToLower() + atom.Number;
        var isotopeNumber = atom.NeutronCount < 0 ? "m" + (atom.NeutronCount * -1) : atom.NeutronCount.ToString();
        return $"{mainAtomDirectoryName}/{isotopeFileName}_{isotopeNumber}.{fileExtension}";
    }
    private string GetActiveAtomIsotopeFileName()
    {
        var mainAtomPath = $"{elementsRoot}/{activeElementFileName}";
        var isotopeNumber = ActiveElementAs<Atom>().NeutronCount < 0 ? "m" + (ActiveElementAs<Atom>().NeutronCount * -1) : ActiveElementAs<Atom>().NeutronCount.ToString();
        return $"{mainAtomPath}/{activeElementFileName}_{isotopeNumber}.{fileExtension}";
    }
}
