using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class DialogPeriodicTable : MonoBehaviour
{
    private PeriodicTableGridItem selectedItem;

    private List<AtomGridItem> page1GridItems = new List<AtomGridItem>();
    private List<IsotopeGridItem> page2GridItems = new List<IsotopeGridItem>();

    private List<PeriodicTableGridItem> pageGridItems = new List<PeriodicTableGridItem>();

    private Button btnLoad, btnDelete, btnIsotopes;

    private Transform page1Transform, page2Transform;
    
    void Start()
    {
        page1Transform = transform.Find("page1");
        page2Transform = transform.Find("page2");

        var page1GridTransform = page1Transform.Find("grid");
        var page1GridTransforms = page1GridTransform.GetComponentsInChildren<RectTransform>();
        page1GridItems = page1GridTransform.GetComponentsInChildren<AtomGridItem>().ToList();
        page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));
        page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        var page2GridTransform = page2Transform.Find("grid");
        var page2GridTransforms = page2GridTransform.GetComponentsInChildren<RectTransform>();
        page2GridItems = page2GridTransform.GetComponentsInChildren<IsotopeGridItem>().ToList();
        page2GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        btnLoad = transform.Find("btnLoad").GetComponent<Button>();
        btnDelete = transform.Find("btnDelete").GetComponent<Button>();
        btnIsotopes = page1Transform.Find("btnIsotopes").GetComponent<Button>();

        Close();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Close();

        if(Input.GetKeyDown(KeyCode.Delete))
            HandleDeleteSelectedItem();
    }

    public void Open()
    {
        gameObject.SetActive(true);

        foreach(Atom atom in FileSystem.LoadedAtoms)
        {
            // TODO: Create a grid item if the atom won't fit in the table
            var gridItem = page1GridItems[atom.Number-1]; // ??
                // CreateGridItem(atom.Number);

            if(gridItem == null)
            {
                Debug.LogWarning("Couldn't fit the atom in the table, skipping");
                continue;
            }

            gridItem.SetAtomData(atom);
        }

        HUD.LockedFocus = true;
        OpenPage1();
    }

    public void OpenPage1()
    {
        page2Transform.gameObject.SetActive(false);
        page1Transform.gameObject.SetActive(true);
    }

    public void OpenPage2()
    {
        var atomGridItem = page2Transform.Find("gridItem").GetComponent<AtomGridItem>();
        atomGridItem.SetAtomData(selectedItem.atom);

        page2GridItems.ForEach(item => item.SetName(selectedItem.atom.Name));

        page2Transform.gameObject.SetActive(true);
        page1Transform.gameObject.SetActive(false);   
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HUD.LockedFocus = false;
    }

    private void HandleItemSelected(PeriodicTableGridItem item)
    {
        btnLoad.interactable = (item.atom != null);
        btnDelete.interactable = (item.atom != null);
        btnIsotopes.interactable = (item.atom != null);

        selectedItem = item;
    }

    public void HandleLoadSelectedItem()
    {
        FileSystem.ActiveAtom = selectedItem.atom;
        Editor.LoadAtomData(selectedItem.atom);
        Close();
    }

    public void HandleDeleteSelectedItem()
    {
        selectedItem.SetActive(false);
        FileSystem.DeleteAtom(selectedItem.atom);
    }
}
