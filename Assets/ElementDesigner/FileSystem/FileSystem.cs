using System.Runtime.CompilerServices;
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
    private static string activeElementFileName => $"{Instance.activeElement.ShortName.ToLower()}{Instance.activeElement.Id}";

    private Element activeElement { get; set; }
    public static Element ActiveElement
    {
        get => Instance.activeElement;
        set => Instance.activeElement = value;
    }

    public static string GetElementDirectoryPathForType(ElementType type)
      => $"{elementsRoot}/{type}";
    public static string GetElementDirectoryPathForTypeName(string typeName)
    {
        if (!Enum.TryParse(typeName, out ElementType type))
            throw new ArgumentException($"Element with typename {typeName} doesn't exist in call to FileSystem.getElementDirectoryPathForTypeName");

        return $"{elementsRoot}/{typeName}";
    }
    public static string GetElementFileName(Element element)
        => $"{element.ShortName.ToLower()}{element.Id}";
    public static string GetElementFilePath(Element element)
        => $"{GetElementDirectoryPathForType(element.ElementType)}/{GetElementFileName(element)}.{fileExtension}";

    public static T CreateElementOfType<T>() where T : Element, new()
    {
        var newElement = new T();
        var allElementsOfType = FileSystemCache.GetOrLoadElementsOfType<T>();
        newElement.Id = allElementsOfType.Count();
        Instance.activeElement = newElement;
        return newElement;
    }

    // TODO: Use ActiveElementAs to e.g. convert ActiveElement to Atom (where possible) and access "charge" to increase charge
    // when a particle is added during Atom design
    public static T ActiveElementAs<T>() where T : Element
    {
        if (ActiveElement.GetType() != typeof(T))
            throw new ApplicationException($"Cannot convert object of type {ActiveElement.GetType()} to {typeof(T)}");

        return ActiveElement as T;
    }
    public static void UpdateActiveElement()
    {
        if (Editor.SubElements.Any(el => el.Data == null))
            throw new ApplicationException("At least one WorldElement is missing data");

        if (ActiveElement is Atom)
            updateActiveAtom(ActiveElement as Atom);
        else
            throw new NotImplementedException($"Element of type ${ActiveElement.GetType().FullName} is not yet implemented");
    }
    private static void updateActiveAtom(Atom atom)
    {
        if (Editor.DesignType != ElementType.Atom)
            throw new ApplicationException($"Editor DesignType must be Atom, got {Editor.DesignType}");

        var particleIds = Editor.SubElements.Select(el => el.Data.Id);
        atom.ParticleIds = particleIds.ToArray();

        var particles = FileSystemCache.GetOrLoadElementsOfTypeByIds<Particle>(particleIds);
        var protonCount = particles.Count(p => p.Charge > 0);
        atom.Number = protonCount;
    }
    public static void SaveActiveElement(IEnumerable<Element> subElements)
    {
        saveElement(ActiveElement, subElements);
    }
    private static void assertValidSubElements(ElementType elementType, IEnumerable<Element> subElements)
    {
        switch (elementType)
        {
            case ElementType.Atom:
                subElements.Select(el => assertValidSubElement(ElementType.Particle, el));
                break;
        };
    }
    private static bool assertValidSubElement(ElementType elementType, Element element, [CallerMemberName] string callerName = "")
        => element.ElementType == elementType ? true :
    throw new ArgumentException($"Element must be of type {elementType} in call to {callerName}, got {element.ElementType}");

    private static void saveElement(Element elementData, IEnumerable<Element> subElements)
    {
        assertValidSubElements(Instance.activeElement.ElementType, subElements);

        var elementFilePath = elementData.ElementType switch
        {
            ElementType.Atom => saveAtom(elementData, subElements),
            _ => GetElementFilePath(elementData)
        };

        // .. if the save was aborted, e.g. when aborting saving of an atom isotope, elementFilePath will be null
        if (elementFilePath == null)
            return;

        var elementDirectoryPath = GetElementDirectoryPathForType(elementData.ElementType);
        if (!Directory.Exists(elementDirectoryPath))
            Directory.CreateDirectory(elementDirectoryPath);

        var elementJSON = JsonUtility.ToJson(elementData);
        File.WriteAllText(elementFilePath, elementJSON);
        TextNotification.Show($"Saved {elementData.Name}");

    }

    private static string saveAtom(Element element, IEnumerable<Element> subElements)
    {
        // .. if this atom doesn't exist, there's no need to check for isotopes. Just save it.
        var allAtoms = FileSystemCache.GetOrLoadElementsOfType<Atom>();
        var atomToSave = element as Atom;
        var parentAtom = allAtoms.FirstOrDefault(a => a.Number == atomToSave.Number);

        if (parentAtom == null)
            return GetElementFilePath(atomToSave);

        var parentAtomParticles = FileSystemCache.GetOrLoadSubElementsOfTypeByIds<Particle>(parentAtom.ParticleIds);
        var parentAtomNeutronCount = parentAtomParticles.Where(particle => particle.Charge == 0).Count();

        var parentAtomIsotopes = FileSystemCache.GetOrLoadElementsOfTypeByIds<Atom>(parentAtom.IsotopeIds);
        var existingIsotope = parentAtomIsotopes.FirstOrDefault(iso =>
        {
            var isotopeParticles = FileSystemCache.GetOrLoadSubElementsOfTypeByIds<Particle>(iso.ParticleIds);
            var isotopeNeutronCount = isotopeParticles.Where(p => p.Charge == 0).Count();

            return (isotopeNeutronCount == parentAtomNeutronCount);
        });

        var atomParticles = subElements.Cast<Particle>();
        var atomNeutronCount = atomParticles.Where(particle => particle.Charge == 0).Count();
        var atomIsIsotope = atomNeutronCount != parentAtomNeutronCount;

        if (atomIsIsotope && existingIsotope == null)
        {
            /* DialogYesNo.Open("Create Isotope?", $"You're about to create an isotope for \"{existingAtom.Name}\". Do you want to do that?",
                () =>
                {

                    
                }
            ); */

            // .. save the isotope
            atomToSave.ParentId = parentAtom.Id;
            atomToSave.Id = allAtoms.Count() + 1;
            var parentAtomFileName = GetElementFileName(parentAtom);
            var isotopeFilePath = $"{GetElementDirectoryPathForType(ElementType.Atom)}/{parentAtomFileName}n{atomNeutronCount}.{fileExtension}";
            var atomToSaveJSON = JsonUtility.ToJson(atomToSave);
            File.WriteAllText(isotopeFilePath, atomToSaveJSON);
            FileSystemCache.ReloadElementOfTypeById<Atom>(atomToSave.Id);

            // .. add the isotope to the parent and save it
            parentAtom.IsotopeIds = parentAtom.IsotopeIds.Concat(new int[] { atomToSave.Id }).ToArray();
            var parentAtomJSON = JsonUtility.ToJson(parentAtom);
            File.WriteAllText(GetElementFilePath(parentAtom), parentAtomJSON);
            FileSystemCache.ReloadElementOfTypeById<Atom>(parentAtom.Id);

            TextNotification.Show($"Created {parentAtom.Name} isotope \"{atomToSave.Name}\"");
        }
        else
        {
            if (existingIsotope != null)
                parentAtom = existingIsotope;

            DialogYesNo.Open("Overwrite?", $"Are you sure you want to overwrite \"{parentAtom.Name}\"?",
            () =>
            {
                var atomJSON = JsonUtility.ToJson(atomToSave);
                var atomFilePath = GetElementFilePath(parentAtom);
                File.WriteAllText(atomFilePath, atomJSON);
                TextNotification.Show($"Saved {atomToSave.Name}");
                FileSystemCache.ReloadElementOfTypeById<Atom>(atomToSave.Id);
            }
        );
        }

        return null;
    }

    public static void DeleteElement(Element elementData)
    {
        var elementFilePath = GetElementFilePath(elementData);
        if (!File.Exists(elementFilePath))
            throw new ApplicationException($"The file at path \"{elementFilePath}\" doesn't exist");

        File.Delete(elementFilePath);
        FileSystemCache.RemoveElementOfTypeById(elementData.Id, elementData.ElementType);
        TextNotification.Show($"Deleted \"{elementData.Name}\"");
    }
}
