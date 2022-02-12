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
    public static List<Atom[]> LoadedIsotopes {get; private set; } = new List<Atom[]>();

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
        var atomExists = File.Exists(mainAtomPath);

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

                var isotopeNumber = ActiveAtom.Charge > 0 ? "m"+ActiveAtom.Charge : ActiveAtom.Charge.ToString();
                var isotopePath = $"{mainAtomPath}/{activeAtomFileName}_{isotopeNumber}.{fileExtension}";
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
                File.WriteAllText(mainAtomPath,activeAtomJSON);
                Debug.Log($"Saved active atom {ActiveAtom.Name} at {DateTime.Now}");
            }
        }
        else
        {
            var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);
            LoadedAtoms.Insert(ActiveAtom.Number-1,ActiveAtom);
            File.WriteAllText(mainAtomPath,activeAtomJSON);
            Debug.Log($"Saved active atom {ActiveAtom.Name} at {DateTime.Now}");
        }
    }

    // .. Create an isotope for a particlar atom, by creating a directory to store isotopes and then saving the file inside it
    public static void ConfirmSaveIsotope()
    {
        var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);

        var mainAtomPath = $"{elementsRoot}/{activeAtomFileName}";
        var isotopeNumber = ActiveAtom.Charge > 0 ? "m"+ActiveAtom.Charge : ActiveAtom.Charge.ToString();
        var isotopePath = $"{mainAtomPath}/{activeAtomFileName}_{isotopeNumber}.{fileExtension}";
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

    public static void LoadAtoms()
    {
        if(!Directory.Exists(elementsRoot))
            return;

        var files = Directory.GetFiles($"{elementsRoot}/",$"*.{fileExtension}");

        foreach(string file in files)
        {
            string atomJSON = File.ReadAllText(file);
            var atomFromJSON = JsonUtility.FromJson<AtomWithIsotopes>(atomJSON);

            // .. TODO: Get isotopes of this atom (if there are any) and load them into the "isotopes" property
            // of the AtomWithIsotopes instance

            LoadedAtoms.Add(atomFromJSON);
        }
    }

    public static void DeleteAtom(AtomWithIsotopes atom)
    {
        LoadedAtoms.Remove(atom);
        File.Delete($"./Elements/{atom.ShortName.ToLower()}.{fileExtension}");
    }
}
