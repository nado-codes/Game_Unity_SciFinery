using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;

public class FileSystemLoader
{
    public static IEnumerable<Element> LoadElementsOfType(ElementType elementType)
     => elementType switch
     {
         ElementType.Particle => LoadElementsOfType<Particle>(),
         ElementType.Atom => LoadElementsOfType<Atom>(),
         ElementType.Molecule => LoadElementsOfType<Molecule>(),
         _ => throw new NotImplementedException($"Element type \"{elementType.ToString()}\" is not implemented in call to LoadElementsOfType")
     };

    public static IEnumerable<T> LoadElementsOfType<T>() where T : Element
    {
        if (!Enum.TryParse(typeof(T).FullName, out ElementType elementType))
            throw new ApplicationException($"Element \"{typeof(T).FullName}\" is not a valid ElementType in call to LoadElements<T : Element>");

        var elements = elementType switch
        {
            ElementType.Particle => loadParticles().Cast<T>(),
            _ => loadElementsOfType<T>()
        };

        if (elements.Any(el => el.ElementType.ToString() != typeof(T).FullName))
            throw new ApplicationException($"All elements must be of type {typeof(T).FullName} in call to LoadElementsOfType");

        return elements;
    }


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

        T element = elements.FirstOrDefault(e => e.Id == id);

        return element != null ? element : getUnknownForType<T>();
    }
    private static T getUnknownForType<T>() where T : Element
    {
        if (Enum.TryParse(typeof(T).FullName, out ElementType elementType))
        {
            var newElementId = loadElementsOfType<T>().Count();
            return (elementType) switch
            {
                ElementType.Particle => getUnknownParticle(newElementId) as T,
                _ => new Element()
                {
                    Id = newElementId,
                    Name = $"Unknown{typeof(T).FullName}",
                    Weight = 1
                } as T
            };
        }

        throw new NotImplementedException("Element of type {typeof(T).FullName} is not a valid ElementType in call to getUnknownForType");
    }
    private static Particle getUnknownParticle(int id) => new Particle()
    {
        Id = id,
        Name = "UnknownParticle",
        Charge = 0,
        ColorHex = "#FFF",
        Weight = 1,
        Size = 1
    };

    private static IEnumerable<Particle> loadParticles()
    {
        var protonParticle = new Particle()
        {
            Id = 1,
            Name = "Proton",
            Weight = .001f,
            ElementType = ElementType.Particle,
            Charge = 1,
            Size = 1,
            ColorHex = "#00FFFA"
        };
        var neutronParticle = new Particle()
        {
            Id = 2,
            Name = "Neutron",
            Weight = 1,
            ElementType = ElementType.Particle,
            Charge = 0,
            Size = 1,
            ColorHex = "#006F05"
        };
        var electronParticle = new Particle()
        {
            Id = 3,
            Name = "Electron",
            Weight = 10f,
            ElementType = ElementType.Particle,
            Charge = -1,
            Size = .5f,
            ColorHex = "#FF0000"
        };

        var defaultParticles = new Particle[] { protonParticle, neutronParticle, electronParticle };
        return defaultParticles.Concat(loadElementsOfType<Particle>());
    }
}