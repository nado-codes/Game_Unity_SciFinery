using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
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
    private static string activeElementFileName => $"{ActiveElement.ShortName.ToLower()}{ActiveElement.Id}";

    public static Element ActiveElement { get; set; }

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
    public static string GetIsotopeFilePath(Atom atom, Atom parentAtom)
    {
        var parentAtomFileName = GetElementFileName(parentAtom);
        var atomNeutronCount = atom.Particles.Count(p => p.Charge == 0);

        return $"{GetElementDirectoryPathForType(ElementType.Atom)}/{parentAtomFileName}n{atomNeutronCount}.{fileExtension}";
    }

    public static T CreateElementOfType<T>() where T : Element, new()
    {
        var newElement = new T();
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
        if (ActiveElement == null)
            throw new ApplicationException("ActiveElement cannot be null in call to UpdateActiveElement");
        if (Editor.SubElements.Any(el => el.Data == null))
            throw new ApplicationException("At least one WorldElement is missing data");

        var newChildIds = Editor.SubElements.Select(el => el.Data.Id).Where(i => i > -1);
        ActiveElement.ChildIds = newChildIds.ToArray();
    }
    private static Element updateElement(Element element)
    {
        if (element == null)
            throw new ApplicationException("Expected an element in call to updateActiveAtom, got null");

        var cacheRef = FileSystemCache.GetOrLoadElementOfTypeById(element.ElementType, element.Id) ?? element;
        var newChildIds = Editor.SubElements.Select(el => el.Data.Id);
        cacheRef.ChildIds = newChildIds.ToArray();

        return cacheRef;
    }
    public static async Task<bool> SaveActiveElement(IEnumerable<Element> subElements)
        => await saveElement(ActiveElement, subElements);
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

    private static async Task<bool> saveElement(Element element, IEnumerable<Element> subElements)
    {
        if (element == null)
            throw new NullReferenceException("Expected an Element, got null");

        assertValidSubElements(element.ElementType, subElements);

        var elementDirectoryPath = GetElementDirectoryPathForType(element.ElementType);
        if (!Directory.Exists(elementDirectoryPath))
            Directory.CreateDirectory(elementDirectoryPath);

        if (element.ElementType == ElementType.Atom)
            return await saveAtom(element, subElements);
        else // .. applies to any other element like Molecules, Products and Particles
        {
            var elementFilePath = GetElementFilePath(element);
            var isUpdate = File.Exists(elementFilePath);

            var allElementsOfType = FileSystemCache.GetOrLoadElementsOfType(element.ElementType);
            element.Id = allElementsOfType.Max(a => a.Id) + 1;
            var elementJSON = JsonUtility.ToJson(element);
            File.WriteAllText(elementFilePath, elementJSON);

            // .. if the file already exists, we know we're updating an existing element
            if (isUpdate)
                FileSystemCache.UpdateElement(element);

            TextNotification.Show($"Saved {element.Name}");
            return true;
        }
    }

    private static async Task<bool> saveAtom(Element element, IEnumerable<Element> subElements)
    {
        // .. if this atom doesn't exist, there's no need to check for isotopes. Just save it.
        var allAtoms = FileSystemCache.GetOrLoadElementsOfType<Atom>();
        var atomToSave = element as Atom;
        var parentAtom = allAtoms.FirstOrDefault(a => a.Number == atomToSave.Number);

        if (parentAtom == null)
        {
            // .. this might loop infinitely...
            return await saveElement(element, subElements);
        }

        var atomParticles = subElements.Cast<Particle>();
        var atomNeutronCount = atomParticles.Where(particle => particle.Charge == 0).Count();
        var parentAtomIsotopes = FileSystemCache.GetOrLoadElementsOfTypeByIds<Atom>(parentAtom.IsotopeIds).Where(i => !i.IsDeleted);
        var existingIsotope = parentAtomIsotopes.FirstOrDefault(iso =>
        {
            var isotopeParticles = FileSystemCache.GetOrLoadSubElementsOfTypeByIds<Particle>(iso.ChildIds);
            var isotopeNeutronCount = isotopeParticles.Where(p => p.Charge == 0).Count();

            return (isotopeNeutronCount == atomNeutronCount);
        });

        // .. TODO: there's an issue here. since the "ActiveElement" will point to the cache, this means that
        // if we load a pre-existing atom and then try to make an isotope of it, the new particles will
        // get added to the cache which means that the neutron counts will always be the same and it wont 
        // detect that we're creating an isotope.
        //
        // the solution here is to pass in a raw atom as the ActiveElement and update the local reference only
        // - only update the cache when we actually click "save".
        var parentAtomParticles = FileSystemCache.GetOrLoadSubElementsOfTypeByIds<Particle>(parentAtom.ChildIds);
        var parentAtomNeutronCount = parentAtomParticles.Where(particle => particle.Charge == 0).Count();
        var atomIsIsotope = atomNeutronCount != parentAtomNeutronCount;

        if (atomIsIsotope && existingIsotope == null)
        {
            var createIsotopeResult = await DialogYesNo.OpenForResult(
                "Create Isotope?",
                $"You're about to create an isotope for \"{parentAtom.Name}\". Do you want to do that?"
            );

            if (createIsotopeResult == YesNo.Yes)
            {
                // .. save the isotope
                atomToSave.ParentId = parentAtom.Id;
                atomToSave.Id = allAtoms.Max(a => a.Id) + 1;
                var isotopeFilePath = GetIsotopeFilePath(atomToSave, parentAtom);
                var atomToSaveJSON = JsonUtility.ToJson(atomToSave);
                File.WriteAllText(isotopeFilePath, atomToSaveJSON);
                FileSystemCache.AddElement<Atom>(atomToSave);

                // .. add the isotope to the parent and save it
                parentAtom.IsotopeIds = parentAtom.IsotopeIds.Concat(new int[] { atomToSave.Id }).ToArray();
                var parentAtomJSON = JsonUtility.ToJson(parentAtom);
                File.WriteAllText(GetElementFilePath(parentAtom), parentAtomJSON);
                FileSystemCache.ReloadElementOfTypeById<Atom>(parentAtom.Id);

                TextNotification.Show($"Created {parentAtom.Name} isotope \"{atomToSave.Name}\"");
                return true;
            }
        }
        else
        {
            var isIsotope = existingIsotope != null;

            var atomText = $"Are you sure you want to overwrite \"{parentAtom.Name}\"?";
            var isotopeText = $"Are you sure you want to overwrite {parentAtom.Name} isotope \"{existingIsotope?.Name}\"?";
            var dialogText = !isIsotope ? atomText : isotopeText;

            var overwriteResult = await DialogYesNo.OpenForResult("Overwrite?", dialogText);

            if (overwriteResult == YesNo.Yes)
            {
                var atomFilePath = !isIsotope ? GetElementFilePath(parentAtom) : GetIsotopeFilePath(atomToSave, parentAtom);

                var atomJSON = JsonUtility.ToJson(atomToSave);
                File.WriteAllText(atomFilePath, atomJSON);
                FileSystemCache.ReloadElementOfTypeById<Atom>(atomToSave.Id);

                TextNotification.Show($"Saved {atomToSave.Name}");
                return true;
            }
        }

        return false;
    }

    public static void RecycleRestoreElement(Element elementData, bool isDeleted)
    {
        if (elementData == null)
            throw new ArgumentException("Expected elementData in call to DeleteElement, got null");

        switch (elementData.ElementType)
        {
            case ElementType.Atom:
                recycleRestoreAtom(elementData as Atom, isDeleted);
                break;
            default:
                recycleElement(elementData, isDeleted);
                break;
        }
        FileSystemCache.RemoveElementOfTypeById(elementData.Id, elementData.ElementType);
        TextNotification.Show($"Deleted \"{elementData.Name}\"");
    }

    private static void recycleElement(Element element, bool isDeleted)
    {
        var elementFilePath = GetElementFilePath(element);
        recycleRestoreElement(elementFilePath, isDeleted);
    }

    private static void recycleRestoreElement(string elementFilePath, bool isDeleted)
    {
        if (string.IsNullOrEmpty(elementFilePath))
            throw new ArgumentException("elementFilePath cannot be null or empty in call to deleteElement");
        if (!File.Exists(elementFilePath))
            throw new ArgumentException($"The file at path \"{elementFilePath}\" doesn't exist in call to deleteElement");

        var elementJSON = File.ReadAllText(elementFilePath);
        var element = JsonUtility.FromJson<Element>(elementJSON);

        element.IsDeleted = isDeleted;

        elementJSON = JsonUtility.ToJson(element);
        File.WriteAllText(elementFilePath, elementJSON);
    }

    private static void recycleRestoreAtom(Atom atom, bool isDeleted)
    {
        if (atom == null)
            throw new ArgumentException("Expected an atom in call to deleteAtom, got null");

        var isIsotope = atom.ParentId != -1;

        var parentAtom = FileSystemCache.GetOrLoadElementOfTypeById<Atom>(atom.ParentId);
        var elementFilePath = isIsotope ? GetIsotopeFilePath(atom, parentAtom) : GetElementFilePath(atom);

        recycleRestoreElement(elementFilePath, isDeleted);
    }
}
