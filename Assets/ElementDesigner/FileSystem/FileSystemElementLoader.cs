using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;

public class FileSystemElementLoader
{
    public static IEnumerable<Element> LoadElementsOfType(ElementType elementType)
     => elementType switch
     {
         ElementType.Molecule => loadElements<Molecule>(),
         _ => throw new NotImplementedException($"Element type \"{elementType.ToString()}\" is not implemented in call to FileSystem.LoadElementsOfType")
     };

    public static Element LoadElementOfTypeById(ElementType elementType, int id)
       => elementType switch
       {
           _ => throw new NotImplementedException($"Element type \"{elementType.ToString()}\" is not implemented in call to FileSystem.LoadElementOfTypeById")
       };

    protected static IEnumerable<T> loadElements<T>() where T : Element
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

            // TODO: Isotopes can probably be loaded seperately as will be too difficult to do here... we'll end up returning an array of arrays
            /* if (typeof(T) == typeof(Atom))
                return new Element[] { elementFromJSON }.Concat(loadAtomIsotopes(elementFilePath));
            else */
        });

        return loadedElements;
    }
}