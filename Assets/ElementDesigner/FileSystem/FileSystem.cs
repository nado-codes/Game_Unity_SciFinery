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
    public static FileSystem instance;

    // ACTIVE ELEMENT
    private string activeElementFileName => ActiveAtomAs<Atom>().ShortName.ToLower() + "_" + ActiveAtomAs<Atom>().Guid;

    public Element ActiveElement { get; set; }
    public bool hasUnsavedChanges = false;

    // LOADED ELEMENTS

    public List<Atom> LoadedAtoms { get; private set; } = new List<Atom>();
    public List<Molecule> LoadedMolecules { get; private set; } = new List<Molecule>();
    public List<Product> LoadedProducts { get; private set; } = new List<Product>();


    void Start()
    {
        instance = this;

        LoadedAtoms = LoadElementsOfType<Atom>().ToList();
        LoadedMolecules = LoadElementsOfType<Molecule>().ToList();
        LoadedProducts = LoadElementsOfType<Product>().ToList();
    }

    public T ActiveAtomAs<T>() where T : Element => ActiveElement as T;

    public Atom NewAtom()
    {
        var newAtom = new Atom();
        newAtom.Number = 1;
        newAtom.Name = "NewAtom";
        newAtom.ShortName = "NE";
        newAtom.ProtonCount = 1;
        newAtom.ElectronCount = 1;

        ActiveElement = newAtom;

        return newAtom;
    }

    public void SaveActiveElement()
    {
        // .. make sure the elements directory exists
        if (!Directory.Exists(elementsRoot))
            Directory.CreateDirectory(elementsRoot);

        // .. if the main atom doesn't exist, create it
        var mainAtomPath = $"{elementsRoot}/{activeElementFileName}";
        var atomExists = File.Exists(GetMainElementFilePath(ActiveElement));

        if (atomExists)
        {
            var existingAtomJSON = File.ReadAllText($"{mainAtomPath}.{fileExtension}");
            var existingAtom = JsonUtility.FromJson<Atom>(existingAtomJSON);
            var activeAtomIsIsotope = existingAtom.Name == ActiveAtomAs<Atom>().Name && existingAtom.NeutronCount != ActiveAtomAs<Atom>().NeutronCount;

            if (activeAtomIsIsotope)
            {
                // .. make sure the atom's isotope directory exists
                if (!Directory.Exists(mainAtomPath))
                    Directory.CreateDirectory(mainAtomPath);

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
                instance.LoadedAtoms[ActiveAtomAs<Atom>().Id - 1] = ActiveElement as Atom;
                File.WriteAllText($"{mainAtomPath}.{fileExtension}", activeAtomJSON);
                Debug.Log($"Saved active atom {ActiveAtomAs<Atom>().Name} at {DateTime.Now}");
                TextNotification.Show("Save Successful");
            }
        }
        else
        {
            var activeAtomJSON = JsonUtility.ToJson(ActiveElement);
            instance.LoadedAtoms.Insert(ActiveAtomAs<Atom>().Id - 1, ActiveElement);
            File.WriteAllText($"{mainAtomPath}.{fileExtension}", activeAtomJSON);
            Debug.Log($"Saved active atom {ActiveAtomAs<Atom>().Name} at {DateTime.Now}");
            TextNotification.Show("Save Successful");
        }
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
        Debug.Log($"Saved isotope {ActiveAtomAs<Atom>().Name} with neutrons {ActiveAtomAs<Atom>().NeutronCount} at {DateTime.Now}");
        TextNotification.Show("Save Successful");
    }

    private static string GetMainElementFilePath(Element element)
    {
        var atomFileName = element.ShortName.ToLower() + "_" + element.Guid;
        return $"{elementsRoot}/{atomFileName}.{fileExtension}";
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
        var isotopeNumber = ActiveAtomAs<Atom>().NeutronCount < 0 ? "m" + (ActiveAtomAs<Atom>().NeutronCount * -1) : ActiveAtomAs<Atom>().NeutronCount.ToString();
        return $"{mainAtomPath}/{activeElementFileName}_{isotopeNumber}.{fileExtension}";
    }

    private static IEnumerable<T> LoadElementsOfType<T>() where T : Element
    {
        if (!Directory.Exists(elementsRoot))
            return new List<T>();

        var typeName = typeof(T).FullName;
        ElementType elementType;
        Enum.TryParse(typeName, out elementType);

        var files = Directory.GetFiles($"{elementsRoot}/{elementType.ToString()}/", $"*.{fileExtension}");
        var loadedElements = new List<T>();

        foreach (string file in files)
        {
            string elementJSON = File.ReadAllText(file);
            var elementFromJSON = JsonUtility.FromJson<T>(elementJSON);

            // TODO: Can other elements (molecule, product) have "sub-types" like isotopes? Or does it only apply to atoms?
            if (elementType == ElementType.Atom)
            {
                var mainAtomDirectoryName = GetMainElementFilePath(elementFromJSON).Split(new string[1] { $".{fileExtension}" }, StringSplitOptions.None)[0];

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

    public void DeleteAtom(Atom atom)
    {
        LoadedAtoms.Remove(atom);
        File.Delete(!atom.IsIsotope ? GetMainElementFilePath(atom) : GetIsotopeFilePath(atom));
        TextNotification.Show("Delete Successful");
    }
}
