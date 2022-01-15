using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class FileSystem : MonoBehaviour
{
    public static Atom ActiveAtom { get; set; }

    private static bool hasLoadedAtoms = false;

    public static List<Atom> LoadedAtoms { get; private set; } = new List<Atom>();

    public static Atom NewActiveAtom()
    {
        ActiveAtom = new Atom();
        ActiveAtom.Number = 1;
        ActiveAtom.Name = "NewAtom";
        ActiveAtom.ShortName = "NE";
        ActiveAtom.ProtonCount = 1;
        ActiveAtom.ElectronCount = 1;

        return ActiveAtom;
    }


    public static void SaveActiveAtom()
    {
        if(!Directory.Exists("./Elements"))
            Directory.CreateDirectory("./Elements");
        
        var activeAtomJSON = JsonUtility.ToJson(ActiveAtom);
        File.WriteAllText($"./Elements/{ActiveAtom.Number}.ed",activeAtomJSON);
        
        Debug.Log($"Saved active atom {ActiveAtom.Name} at {DateTime.Now}");

        LoadedAtoms.Insert(ActiveAtom.Number-1,ActiveAtom);
    }

    public static void LoadAtoms()
    {
        if(!Directory.Exists("./Elements"))
            return;

        var files = Directory.GetFiles("./Elements/","*.ed");

        foreach(string file in files)
        {
            string atomJSON = File.ReadAllText(file);
            var atomFromJSON = JsonUtility.FromJson<Atom>(atomJSON);

            LoadedAtoms.Add(atomFromJSON);
        }
    }

    public static void DeleteAtom(Atom atom)
    {
        LoadedAtoms.Remove(atom);
        File.Delete($"./Elements/{atom.Number}.ed");
    }
}
