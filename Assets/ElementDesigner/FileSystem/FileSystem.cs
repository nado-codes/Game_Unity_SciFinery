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

    public static AtomWithIsotopes ActiveAtom { get; set; }

    private static bool hasLoadedAtoms = false;

    public static List<AtomWithIsotopes> LoadedAtoms { get; private set; } = new List<AtomWithIsotopes>();

    public static Atom NewActiveAtom()
    {
        ActiveAtom = new AtomWithIsotopes();
        ActiveAtom.Number = 1;
        ActiveAtom.Name = "NewAtom";
        ActiveAtom.ShortName = "NE";
        ActiveAtom.ProtonCount = 1;
        ActiveAtom.ElectronCount = 1;

        return ActiveAtom;
    }


    public static void SaveActiveAtom()
    {
        if(!Directory.Exists(elementsRoot))
            Directory.CreateDirectory(elementsRoot);
        
        var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);

        var mainAtomPath = $"{elementsRoot}/{ActiveAtom.ShortName.ToLower()}";
        var existingAtomJSON = File.ReadAllText($"{mainAtomPath}.{fileExtension}");
        var existingAtom = JsonUtility.FromJson<AtomWithIsotopes>(existingAtomJSON);
        var isOverwriteOrSave = existingAtom == null || existingAtom.Charge == ActiveAtom.Charge;

        if(!isOverwriteOrSave)
        {
            // .. atoms which exist, but have a different charge (isotopes)
            
            // TODO: confirm the creation of the isotope with a popup dialog, and then run "ConfirmSaveIsotope()"
        }
        else // .. atoms which don't exist or have the same charge
            File.WriteAllText(mainAtomPath,activeAtomJSON);
        
        if(!isOverwriteOrSave)
        {
            // .. atoms which exist, but have a different charge (isotopes)
            if(!Directory.Exists(mainAtomPath))
                Directory.CreateDirectory(mainAtomPath);

            var isotopeNumber = ActiveAtom.Charge > 0 ? "m"+ActiveAtom.Charge : ActiveAtom.Charge.ToString();
            var isotopePath = $"{mainAtomPath}/{ActiveAtom.ShortName.ToLower()}_{isotopeNumber}.{fileExtension}";

            File.WriteAllText(isotopePath,activeAtomJSON);
        }
        else
        {
            Debug.Log($"Saved active atom {ActiveAtom.Name} at {DateTime.Now}");
        }

        LoadedAtoms.Insert(ActiveAtom.Number-1,ActiveAtom);
    }

    // .. Create an isotope for a particlar atom, by creating a directory to store isotopes and then saving the file inside it
    public static void ConfirmSaveIsotope()
    {
        var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);
        var mainAtomPath = $"{elementsRoot}/{ActiveAtom.ShortName.ToLower()}";

        if(!Directory.Exists(mainAtomPath))
            Directory.CreateDirectory(mainAtomPath);

        var isotopeNumber = ActiveAtom.Charge > 0 ? "m"+ActiveAtom.Charge : ActiveAtom.Charge.ToString();
        var isotopePath = $"{mainAtomPath}/{ActiveAtom.ShortName.ToLower()}_{isotopeNumber}.{fileExtension}";

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
