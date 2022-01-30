using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class DialogPeriodicTable : MonoBehaviour
{
    private InputField inputName;

    private PeriodicTableGridItem selectedItem;

    public AtomGridItem gridItemPrefab;
    private List<AtomGridItem> page1GridItems = new List<AtomGridItem>();
    private List<IsotopeGridItem> page2GridItems = new List<IsotopeGridItem>();

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
    }

    public void OpenPage1()
    {
        page2Transform.gameObject.SetActive(false);
        page1Transform.gameObject.SetActive(true);
    }

    public void OpenPage2()
    {
        page2Transform.gameObject.SetActive(true);
        page1Transform.gameObject.SetActive(false);
    }

    public AtomGridItem CreateGridItem(int index)
    {
        if(gridItemPrefab == null)
            return null;

        var newGridItem = Instantiate(gridItemPrefab);
        newGridItem.transform.parent = transform;
        newGridItem.transform.localScale = Vector3.one;
        
        page1GridItems.Add(newGridItem);

        return newGridItem;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HUD.LockedFocus = false;
    }

    private void HandleItemSelected(PeriodicTableGridItem item)
    {
        btnLoad.interactable = (item != null);
        btnDelete.interactable = (item != null);
        btnIsotopes.interactable = (item != null);

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
