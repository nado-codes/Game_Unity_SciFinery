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

    // ACTIVE ATOM
    private static string activeAtomFileName => ActiveAtom.ShortName.ToLower() + ActiveAtom.Number;

    public static Atom ActiveAtom { get; set; }
    public bool hasUnsavedChanges = false;

    // LOADED ATOMS

    private static bool hasLoadedAtoms = false;

    public static List<Atom> LoadedAtoms { get; private set; } = new List<Atom>();

    public static Atom NewAtom()
    {
        ActiveAtom = new Atom();
        ActiveAtom.Number = 1;
        ActiveAtom.Name = "NewAtom";
        ActiveAtom.ShortName = "NE";
        ActiveAtom.ProtonCount = 1;
        ActiveAtom.ElectronCount = 1;

        return ActiveAtom;
    }
    public static void SaveAtom()
    {
        // .. make sure the directory exists
        if (!Directory.Exists(elementsRoot))
            Directory.CreateDirectory(elementsRoot);

        // .. if the main atom doesn't exist, create it
        var mainAtomPath = $"{elementsRoot}/{activeAtomFileName}";
        var atomExists = File.Exists(GetMainAtomFilePath(ActiveAtom));

        if (atomExists)
        {
            var existingAtomJSON = File.ReadAllText($"{mainAtomPath}.{fileExtension}");
            var existingAtom = JsonUtility.FromJson<Atom>(existingAtomJSON);
            var activeAtomIsIsotope = existingAtom.Name == ActiveAtom.Name && existingAtom.NeutronCount != ActiveAtom.NeutronCount;

            if (activeAtomIsIsotope)
            {
                // .. make sure the atom's isotope directory exists
                if (!Directory.Exists(mainAtomPath))
                    Directory.CreateDirectory(mainAtomPath);

                var isotopePath = GetActiveAtomIsotopeFileName();
                var isotopeExists = File.Exists(isotopePath);

                var dialogBody = @"You're about to create the isotope {isotopeShortName} for {mainAtomName}, 
                with a charge of {isotopeCharge}. Do you wish to continue?";

                if (!isotopeExists) // .. confirm create isotope
                    DialogYesNo.Open("Confirm Create Isotope",dialogBody);
                else
                    ConfirmSaveIsotope(); // .. overwrite isotope

                Debug.Log($"Saved isotope {ActiveAtom.Name} with neutrons {ActiveAtom.NeutronCount} at {DateTime.Now}");
            }
            else // .. overwrite the main atom
            {
                var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);
                LoadedAtoms[ActiveAtom.Number - 1] = ActiveAtom;
                File.WriteAllText($"{mainAtomPath}.{fileExtension}", activeAtomJSON);
                Debug.Log($"Saved active atom {ActiveAtom.Name} at {DateTime.Now}");
            }
        }
        else
        {
            var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);
            LoadedAtoms.Insert(ActiveAtom.Number - 1, ActiveAtom);
            File.WriteAllText($"{mainAtomPath}.{fileExtension}", activeAtomJSON);
            Debug.Log($"Saved active atom {ActiveAtom.Name} at {DateTime.Now}");
        }
    }

    // .. Create an isotope for a particlar atom, by creating a directory to store isotopes and then saving the file inside it
    public static void ConfirmSaveIsotope()
    {
        var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);

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
    }

    private static string GetMainAtomFilePath(Atom atom)
    {
        var atomFileName = atom.ShortName.ToLower() + atom.Number;
        return $"{elementsRoot}/{atomFileName}.{fileExtension}";
    }
    private static string GetIsotopeFilePath(Atom atom)
    {
        var mainAtomFilePath = GetMainAtomFilePath(atom);
        var mainAtomDirectoryName = mainAtomFilePath.Split(new string[1] { $".{fileExtension}" }, StringSplitOptions.None)[0];

        var isotopeFileName = atom.ShortName.ToLower() + atom.Number;
        var isotopeNumber = atom.NeutronCount < 0 ? "m" + (atom.NeutronCount * -1) : atom.NeutronCount.ToString();
        return $"{mainAtomDirectoryName}/{isotopeFileName}_{isotopeNumber}.{fileExtension}";
    }

    private static string GetActiveAtomIsotopeFileName()
    {
        var mainAtomPath = $"{elementsRoot}/{activeAtomFileName}";
        var isotopeNumber = ActiveAtom.NeutronCount < 0 ? "m" + (ActiveAtom.NeutronCount * -1) : ActiveAtom.NeutronCount.ToString();
        return $"{mainAtomPath}/{activeAtomFileName}_{isotopeNumber}.{fileExtension}";
    }

    public static void LoadAtoms()
    {
        if (!Directory.Exists(elementsRoot))
            return;

        var files = Directory.GetFiles($"{elementsRoot}/", $"*.{fileExtension}");

        foreach (string file in files)
        {
            string atomJSON = File.ReadAllText(file);
            var atomFromJSON = JsonUtility.FromJson<Atom>(atomJSON);
            var mainAtomDirectoryName = GetMainAtomFilePath(atomFromJSON).Split(new string[1] { $".{fileExtension}" }, StringSplitOptions.None)[0];

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

                LoadedAtoms.AddRange(isotopeAtoms);
            }

            LoadedAtoms.Add(atomFromJSON);
        }
    }

    public static void DeleteAtom(Atom atom)
    {
        LoadedAtoms.Remove(atom);
        File.Delete(!atom.IsIsotope ? GetMainAtomFilePath(atom) : GetIsotopeFilePath(atom));
    }
}
