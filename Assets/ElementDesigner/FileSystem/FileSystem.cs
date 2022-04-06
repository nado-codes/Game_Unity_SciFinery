using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class FileSystem : MonoBehaviour
{
    public const string fileExtension = "ed";
    public const string elementsRoot = "./Elements";

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

    public static string GetElementDirectoryPathForType(ElementType type)
      => $"{FileSystem.elementsRoot}/{type}";
    public static string GetElementDirectoryPathForTypeName(string typeName)
    {
        if (!Enum.TryParse(typeName, out ElementType type))
            throw new ArgumentException($"Element with typename {typeName} doesn't exist in call to FileSystem.getElementDirectoryPathForTypeName");

        return $"{elementsRoot}/{typeName}";
    }
    public static string GetElementFilePath(Element element)
    {
        var elementFileName = $"{element.ShortName.ToLower()}{element.Id}";
        return $"{GetElementDirectoryPathForType(element.Type)}/{elementFileName}.{FileSystem.fileExtension}";
    }

    public static IEnumerable<Element> LoadElementsOfType(ElementType elementType) => elementType switch
    {
        ElementType.Particle => FileSystemParticleLoader.LoadParticles(),
        ElementType.Atom => FileSystemAtomLoader.LoadAtoms(),
        _ => FileSystemElementLoader.LoadElementsOfType(elementType)
    };
    public static Element LoadElementOfTypeById(ElementType elementType, int id) => FileSystemElementLoader.LoadElementOfTypeById(elementType, id);
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
        var elementDirectoryPath = GetElementDirectoryPathForType(activeElement.Type);
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
        var allParticles = FileSystemParticleLoader.LoadParticles().ToArray();
        var activeAtomParticles = new Particle[0];/* activeAtom.ParticleIds.Select(id =>
        {
            var particle = allParticles[id - 1];

            // if (particle)
            return null;
        }); */
        var atomDataParticles = allParticles.Where(particle => atomData.ParticleIds.Any(id => id == particle.Id));
        var activeAtomNeutronsCount = activeAtomParticles.Where(particle => particle.Charge == 0).Count();
        var atomDataNeutronsCount = atomDataParticles.Where(particle => particle.Charge == 0).Count();

        var hasDifferentNeutronCount = atomDataNeutronsCount != activeAtomNeutronsCount;
        var activeAtomIsIsotope = atomData.Name == activeAtom.Name && hasDifferentNeutronCount;

        var elementExists = File.Exists(GetElementFilePath(atomData));

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

        var mainElementFilePath = GetElementFilePath(elementData);

        if (!File.Exists(mainElementFilePath))
            throw new ApplicationException($"The file at path \"{mainElementFilePath}\" doesn't exist in call to FileSystem.DeleteElement");

        File.Delete(mainElementFilePath);

        // TODO: implement deleting isotopes
        // File.Delete(!elementData.IsIsotope ? GetMainElementFilePath(elementData) : GetIsotopeFilePath(elementData));
        TextNotification.Show("Delete Successful");
    }



    /* private static string GetIsotopeFilePath(Atom atom)
    {
        var mainAtomFilePath = getElementFilePath(atom);
        var mainAtomDirectoryName = mainAtomFilePath.Split(new string[1] { $".{fileExtension}" }, StringSplitOptions.None)[0];

        var isotopeFileName = atom.ShortName.ToLower() + atom.Id;
        var isotopeNumber = atom.NeutronCount < 0 ? "m" + (atom.NeutronCount * -1) : atom.NeutronCount.ToString();
        return $"{mainAtomDirectoryName}/{isotopeFileName}{isotopeNumber}.{fileExtension}";
    } */

}
