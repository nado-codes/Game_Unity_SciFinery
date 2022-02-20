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
    private static string activeAtomFileName => ActiveAtom.ShortName.ToLower()+ActiveAtom.Number;

    public static AtomWithIsotopes ActiveAtom { get; set; }

    private static bool hasLoadedAtoms = false;

    public static List<AtomWithIsotopes> LoadedAtoms { get; private set; } = new List<AtomWithIsotopes>();

    public static Atom NewAtom()
    {
        ActiveAtom = new AtomWithIsotopes();
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
        if(!Directory.Exists(elementsRoot))
            Directory.CreateDirectory(elementsRoot);

        // .. if the main atom doesn't exist, create it
        var mainAtomPath = $"{elementsRoot}/{activeAtomFileName}";
        var atomExists = File.Exists(GetMainAtomFilePath(ActiveAtom));

        if(atomExists)
        {
            var existingAtomJSON = File.ReadAllText($"{mainAtomPath}.{fileExtension}");
            var existingAtom = JsonUtility.FromJson<AtomWithIsotopes>(existingAtomJSON);
            var activeAtomIsIsotope = existingAtom.Name == ActiveAtom.Name && existingAtom.Charge != ActiveAtom.Charge;

            if(activeAtomIsIsotope)
            {
                // .. make sure the atom's isotope directory exists
                if(!Directory.Exists(mainAtomPath))
                    Directory.CreateDirectory(mainAtomPath);

                var isotopePath = GetActiveAtomIsotopeFileName();
                var isotopeExists = File.Exists(isotopePath);

                if(!isotopeExists) // .. confirm create isotope
                    DialogConfirmSaveIsotope.Open();
                else
                    ConfirmSaveIsotope(); // .. overwrite isotope

                Debug.Log($"Saved isotope {ActiveAtom.Name} with charge {ActiveAtom.Charge} at {DateTime.Now}");
            }
            else // .. overwrite the main atom
            {
                var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);
                LoadedAtoms[ActiveAtom.Number-1] = ActiveAtom;
                File.WriteAllText($"{mainAtomPath}.{fileExtension}",activeAtomJSON);
                Debug.Log($"Saved active atom {ActiveAtom.Name} at {DateTime.Now}");
            }
        }
        else
        {
            var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);
            LoadedAtoms.Insert(ActiveAtom.Number-1,ActiveAtom);
            File.WriteAllText($"{mainAtomPath}.{fileExtension}",activeAtomJSON);
            Debug.Log($"Saved active atom {ActiveAtom.Name} at {DateTime.Now}");
        }
    }

    // .. Create an isotope for a particlar atom, by creating a directory to store isotopes and then saving the file inside it
    public static void ConfirmSaveIsotope()
    {
        var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);

        var isotopePath = GetActiveAtomIsotopeFileName();
        var isotopeExists = File.Exists(isotopePath);
        
        if(!isotopeExists)
            LoadedAtoms[ActiveAtom.Number-1].isotopes.Add(ActiveAtom);
        else
        {
            var isotopeCount = LoadedAtoms[ActiveAtom.Number-1].isotopes.Count;
            var indexInIsotopes = LoadedAtoms[ActiveAtom.Number-1].isotopes.FindIndex(0,isotopeCount,a => a.Charge == ActiveAtom.Charge);

            LoadedAtoms[ActiveAtom.Number-1].isotopes[indexInIsotopes] = ActiveAtom;
        }
        File.WriteAllText(isotopePath,activeAtomJSON);
    }

    private static string GetMainAtomFilePath(Atom atom)
    {
        var atomFileName = ActiveAtom.ShortName.ToLower()+ActiveAtom.Number;
        return $"{elementsRoot}/{atomFileName}.{fileExtension}";
    }
    private static string GetIsotopeFilePath(Atom atom)
    {
        var mainAtomFilePath = GetMainAtomFilePath(atom);
        var mainAtomDirectoryName = mainAtomFilePath.Split($".{fileExtension}")[0];

        var isotopeFileName = ActiveAtom.ShortName.ToLower()+ActiveAtom.Number;
        var isotopeNumber = ActiveAtom.Charge < 0 ? "m"+(ActiveAtom.Charge*-1) : ActiveAtom.Charge.ToString();
        return $"{mainAtomDirectoryName}/{isotopeFileName}_{isotopeNumber}.{fileExtension}";
    }

    private static string GetActiveAtomIsotopeFileName()
    {
        var mainAtomPath = $"{elementsRoot}/{activeAtomFileName}";
        var isotopeNumber = ActiveAtom.Charge < 0 ? "m"+(ActiveAtom.Charge*-1) : ActiveAtom.Charge.ToString();
        return $"{mainAtomPath}/{activeAtomFileName}_{isotopeNumber}.{fileExtension}";
    }

    public static void LoadAtoms()
    {
        if(!Directory.Exists(elementsRoot))
            return;

        var files = Directory.GetFiles($"{elementsRoot}/",$"*.{fileExtension}");

        foreach(string file in files)
        {
            string atomJSON = File.ReadAllText(file);
            var atomFromJSON = JsonUtility.FromJson<AtomWithIsotopes>(atomJSON);
            var mainAtomDirectoryName = GetMainAtomFilePath(atomFromJSON).Split($".{fileExtension}")[0];
            var isotopes = Directory.GetFiles($"{mainAtomDirectoryName}/",$"*.{fileExtension}");
            var isotopeAtoms = isotopes.Select(isotope => {
                var isotopeJSON = File.ReadAllText(isotope);
                var isotopeAtom = JsonUtility.FromJson<AtomWithIsotopes>(isotopeJSON);
                isotopeAtom.IsIsotope = true;
                return isotopeAtom;
            });

            LoadedAtoms.Add(atomFromJSON);
            LoadedAtoms.AddRange(isotopeAtoms);
        }
    }

    public static void DeleteAtom(AtomWithIsotopes atom)
    {
        LoadedAtoms.Remove(atom);
        File.Delete($"./Elements/{atom.ShortName.ToLower()}.{fileExtension}");
    }
}
