using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;

namespace EDFileSystem.Loader
{
    public class FileSystemLoader
    {
        private readonly Dictionary<ElementType, Element[]> cache = new Dictionary<ElementType, Element[]>();
        public IEnumerable<T> GetOrLoadElementsOfType<T>() where T : Element
        {
            if (Enum.TryParse(typeof(T).FullName, out ElementType elementType))
                return GetOrLoadElementsOfType(elementType).Cast<T>();
            else
                throw new ApplicationException($"Element \"{typeof(T).FullName}\" is not a valid ElementType in call to FileSystemLoader.LoadElements<T : Element>");
        }
        public IEnumerable<Element> GetOrLoadElementsOfType(ElementType elementType)
         => elementType switch
         {
             ElementType.Particle => loadParticles(),
             ElementType.Atom => getOrLoadElementsOfType<Atom>(),
             _ => throw new NotImplementedException($"Element type \"{elementType.ToString()}\" is not implemented in call to FileSystem.LoadElementsOfType")
         };

        public Element GetOrLoadElementOfTypeById(ElementType elementType, int id)
           => elementType switch
           {
               ElementType.Atom => getOrLoadElementOfTypeById<Atom>(id),
               _ => throw new NotImplementedException(
                   $"Element type \"{elementType.ToString()}\" is not implemented in call to FileSystem.LoadElementOfTypeById"
                )
           };

        private static IEnumerable<T> getOrLoadElementsOfType<T>() where T : Element
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

        private static T getOrLoadElementOfTypeById<T>(int id) where T : Element
        {
            var elementsOfType = getOrLoadElementsOfType<T>();
            var elementById = elementsOfType.FirstOrDefault(e => e.Id == id);

            if (elementById == null)
            {
                throw new ApplicationException(
                    $"{typeof(T).FullName} with Id {id} doesn't exist in call to FileSystemLoader.loadElementOfTypeById"
                );
            }

            return elementById;
        }

        // TODO: Implement loading isotopes
        private static IEnumerable<Atom> loadAtomIsotopes(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException($"No atom exists at path {path} in call to FileSystem.loadAtom");

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
            return defaultParticles.Concat(getOrLoadElementsOfType<Particle>());
        }
    }
}