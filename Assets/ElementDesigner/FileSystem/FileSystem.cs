using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using EDFileSystem.Loader;

namespace EDFileSystem
{
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

        private FileSystemLoader loader;
        private FileSystemLoader Loader
        {
            get
            {
                if (loader == null)
                    loader = new FileSystemLoader();

                return loader;
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

        // LOADED ELEMENTS

        private List<Element> loadedElements = new List<Element>();
        public static List<Element> LoadedElements
        {
            get => Instance.loadedElements;
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

        public static IEnumerable<Element> LoadElementsOfType(ElementType elementType) =>
            Instance.Loader.LoadElementsOfType(elementType);
        public static Element LoadElementOfTypeById(ElementType elementType, int id) =>
            Instance.loader.LoadElementOfTypeById(elementType, id);
        public static T CreateElementOfType<T>() where T : Element
        {
            if (typeof(T) == typeof(Atom))
            {
                var atomsCount = Instance.Loader.LoadElementsOfType<Atom>().Count();
                var newAtom = new Atom();


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
        public static void SaveActiveElement(Element[] subElements)
        {
            AssertValidSubElements(Instance.activeElement.ElementType, subElements);

            if (File.Exists(GetElementFilePath(ActiveElement)))
                DialogYesNo.Open("Overwrite Element",
                    $"An Element with name {GetElementFileName(ActiveElement)} already exists. Do you want to overwrite it?",
                    () => saveElement(ActiveElement, subElements)
                );
            else
                saveElement(ActiveElement, subElements);
        }
        private static void AssertValidSubElements(ElementType elementType, Element[] subElements)
        {
            switch (elementType)
            {
                case ElementType.Atom:
                    subElements.Select(el => AssertValidSubElement(ElementType.Particle, el));
                    break;
            };
        }
        private static bool AssertValidSubElement(ElementType elementType, Element element, [CallerMemberName] string callerName = "")
            => element.ElementType == elementType ? true :
        throw new ArgumentException($"Element must be of type {elementType} in call to {callerName}, got {element.ElementType}");

        private static void saveElement(Element elementData, Element[] subElements)
        {
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

        }
        private static string saveAtom(Element elementData, Element[] subElements)
        {
            // .. if this atom doesn't exist, there's no need to check for isotopes. Just save it.
            var atomFilePath = GetElementFilePath(elementData);
            if (!File.Exists(atomFilePath))
                return atomFilePath;

            // .. if the atom exists, we should check if the new data is an isotope or not
            var atomData = elementData as Atom;
            var particles = subElements.Cast<Particle>();

            var existingAtomJSON = File.ReadAllText(atomFilePath);
            var existingAtom = JsonUtility.FromJson<Atom>(existingAtomJSON);

            var allParticles = Instance.Loader.LoadElementsOfType<Particle>();
            var existingAtomParticles = Instance.Loader.LoadElementsOfTypeByIds<Particle>(existingAtom.ParticleIds);
            var atomNeutronCount = particles.Where(particle => particle.Charge == 0).Count();
            var existingAtomNeutronCount = existingAtomParticles.Where(particle => particle.Charge == 0).Count();
            var activeAtomIsIsotope = atomNeutronCount != existingAtomNeutronCount;

            if (activeAtomIsIsotope)
            {
                DialogYesNo<string>.Open("Create Isotope?", $"You're about to create an isotope for \"{existingAtom.Name}\". Do you want to do that?",
                    () =>
                    {

                    }
                );
            }

            var elementExists = File.Exists(GetElementFilePath(atomData));

            if (elementExists)
            {
                var existingElementJSON = File.ReadAllText($"{mainElementPath}.{fileExtension}");
                var existingElement = JsonUtility.FromJson<Atom>(existingElementJSON);

                // TODO: can probably split this out into a factory. double-nested if statement
                if (activeAtomIsIsotope)
                {
                    // TODO: implement saving isotopes
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
}
