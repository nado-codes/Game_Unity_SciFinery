using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class DialogPeriodicTable : MonoBehaviour
{
    private InputField inputName;

    private PeriodicTableGridItem selectedItem;

    public PeriodicTableGridItem gridItemPrefab;
    private List<PeriodicTableGridItem> gridItems = new List<PeriodicTableGridItem>();
    private Button btnLoad, btnDelete;
    
    void Start()
    {
        var grid = transform.Find("grid");
        var transforms = grid.GetComponentsInChildren<RectTransform>();
        gridItems = grid.GetComponentsInChildren<PeriodicTableGridItem>().ToList();
        gridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        btnLoad = transform.Find("btnLoad").GetComponent<Button>();
        btnDelete = transform.Find("btnDelete").GetComponent<Button>();

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
            var gridItem = gridItems[atom.Number-1]; // ??
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

    public PeriodicTableGridItem CreateGridItem(int index)
    {
        if(gridItemPrefab == null)
            return null;

        var newGridItem = Instantiate(gridItemPrefab);
        newGridItem.transform.parent = transform;
        newGridItem.transform.localScale = Vector3.one;
        
        gridItems.Add(newGridItem);

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

        selectedItem = item;
    }

    public void HandleLoadSelectedItem()
        => FileSystem.ActiveAtom = selectedItem.atom;

    public void HandleDeleteSelectedItem()
    {
        selectedItem.SetActive(false);
        FileSystem.DeleteAtom(selectedItem.atom);
    }
}
