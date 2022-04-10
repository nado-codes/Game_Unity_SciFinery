using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;

public class FileSystemLoader
{
    public static IEnumerable<T> LoadElementsOfType<T>() where T : Element
    {
        if (Enum.TryParse(typeof(T).FullName, out ElementType elementType))
            return LoadElementsOfType(elementType).Cast<T>();
        else
            throw new ApplicationException($"Element \"{typeof(T).FullName}\" is not a valid ElementType in call to LoadElements<T : Element>");
    }
    public static IEnumerable<Element> LoadElementsOfType(ElementType elementType)
     => elementType switch
     {
         ElementType.Particle => loadParticles(),
         ElementType.Atom => loadElementsOfType<Atom>(),
         _ => throw new NotImplementedException($"Element type \"{elementType.ToString()}\" is not implemented in call to LoadElementsOfType")
     };

    public static Element LoadElementOfTypeById<T>(int id) where T : Element
        => loadElementOfTypeById<T>(id);

    public static IEnumerable<T> LoadElementsOfTypeByIds<T>(IEnumerable<int> ids) where T : Element
        => loadElementsOfTypeByIds<T>(ids);

    public static Element LoadElementOfTypeById(ElementType elementType, int id)
       => elementType switch
       {
           ElementType.Particle => loadElementOfTypeById<Particle>(id),
           ElementType.Atom => loadElementOfTypeById<Atom>(id),
           _ => throw new NotImplementedException(
               $"Element type \"{elementType.ToString()}\" is not implemented in call to LoadElementOfTypeById"
            )
       };

    private static IEnumerable<T> loadElementsOfType<T>() where T : Element
    {
        var typeName = typeof(T).FullName;
        var elementsOfTypeDirPath = FileSystem.GetElementDirectoryPathForTypeName(typeName);

        if (!Directory.Exists(elementsOfTypeDirPath))
            return new List<T>();

        var files = Directory.GetFiles(elementsOfTypeDirPath, $"*.{FileSystem.fileExtension}");

        var loadedElements = files.Select(elementFilePath =>
        {
            string elementJSON = File.ReadAllText(elementFilePath);
            var elementFromJSON = JsonUtility.FromJson<T>(elementJSON);

            return elementFromJSON;
        });

        return loadedElements;
    }
    private static IEnumerable<T> loadElementsOfTypeByIds<T>(IEnumerable<int> ids) where T : Element
    {
        var elementsOfType = LoadElementsOfType<T>();
        return ids.Select(id => getElementById<T>(id, elementsOfType));
    }
    private static T loadElementOfTypeById<T>(int id) where T : Element
    {
        var elementsOfType = LoadElementsOfType<T>();
        return getElementById<T>(id, elementsOfType);
    }
    private static T getElementById<T>(int id, IEnumerable<T> elements) where T : Element
    {
        if (id < 1) throw new ArgumentException("id must be greater than 0 in call to getElementById");

        T element = elements.ElementAt(id - 1);

        if (element == null)
            throw new NullReferenceException("Element with id {id} doesn't exist in call to getElementById");

        return element;
    }

    // TODO: Implement loading isotopes
    private static IEnumerable<Atom> loadAtomIsotopes(string path)
    {
        if (!File.Exists(path))
            throw new ArgumentException($"No atom exists at path {path} in call to loadAtom");

        // TODO: Implement loading isotopes

        return new List<Atom>();
    }
    private static IEnumerable<Particle> loadParticles()
    {
        var protonParticle = new Particle()
        {
            Id = 1,
            Name = "Proton",
            Weight = .001f,
            Charge = 1,
            Size = 1,
            ElementType = ElementType.Particle,
            Color = "#00FFFA"
        };
        var neutronParticle = new Particle()
        {
            Id = 2,
            Name = "Neutron",
            Weight = 1,
            Charge = 0,
            Size = 1,
            ElementType = ElementType.Particle,
            Color = "#006F05"
        };
        var electronParticle = new Particle()
        {
            Id = 3,
            Name = "Electron",
            Weight = 10f,
            Charge = -1,
            Size = .5f,
            ElementType = ElementType.Particle,
            Color = "#FF0000"
        };

        var defaultParticles = new Particle[] { protonParticle, neutronParticle, electronParticle };
        return defaultParticles.Concat(loadElementsOfType<Particle>());
    }
}