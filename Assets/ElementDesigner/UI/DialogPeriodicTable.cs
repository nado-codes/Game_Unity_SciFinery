using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class DialogPeriodicTable : MonoBehaviour
{
    public Editor editor;
    private GridItem<Element> selectedItem;

    private List<ElementGridItem> page1GridItems = new List<ElementGridItem>();
    private List<ElementGridItem> page2GridItems = new List<ElementGridItem>();

    private List<GridItem<Element>> pageGridItems = new List<GridItem<Element>>();

    private Button btnLoad, btnDelete, btnIsotopes;

    private Transform page1Transform, page2Transform;

    void Start()
    {
        page1Transform = transform.Find("page1");
        page2Transform = transform.Find("page2");

        var page1GridTransform = page1Transform.Find("grid");
        var page1GridTransforms = page1GridTransform.GetComponentsInChildren<RectTransform>();
        page1GridItems = page1GridTransform.GetComponentsInChildren<ElementGridItem>().ToList();
        page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));
        page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        var page2AtomGridItem = page2Transform.GetComponentInChildren<AtomGridItem>();
        page2AtomGridItem.Init(); // .. initialise the atom grid item

        var page2GridTransform = page2Transform.Find("grid");
        var page2GridTransforms = page2GridTransform.GetComponentsInChildren<RectTransform>();
        page2GridItems = page2GridTransform.GetComponentsInChildren<ElementGridItem>().ToList();
        page2GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        btnLoad = transform.Find("btnLoad").GetComponent<Button>();
        btnDelete = transform.Find("btnDelete").GetComponent<Button>();
        btnIsotopes = page1Transform.Find("btnIsotopes").GetComponent<Button>();

        OpenPage1();
        Close();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();

        if (Input.GetKeyDown(KeyCode.Delete))
            HandleDeleteSelectedItem();
    }

    public void Open()
    {
        gameObject.SetActive(true);

        // .. atoms which are not isotopes
        var mainAtoms = FileSystem.instance.LoadedAtoms.Where(atom => !atom.IsIsotope);

        foreach (Atom atom in mainAtoms)
        {
            // TODO: Create a grid item if the atom won't fit in the table
            var gridItem = page1GridItems[atom.Number - 1];

            if (gridItem == null)
            {
                Debug.LogWarning("Couldn't fit the atom in the table, skipping");
                continue;
            }

            // gridItem.SetAtomData(atom);
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
        page2Transform.gameObject.SetActive(true);
        page1Transform.gameObject.SetActive(false);

        var atomGridItem = page2Transform.Find("gridItem").GetComponent<AtomGridItem>();
        // atomGridItem.SetAtomData(selectedItem?.elementData);

        var selectedAtomIsotopes = FileSystem.instance.LoadedAtoms.Where(atom => atom.Name == selectedItem.elementData.Name && atom.IsIsotope);

        foreach (ElementGridItem gridItem in page2GridItems)
        {
            var gridItemIndex = page2GridItems.IndexOf(gridItem);
            /* var isotopeAtom = selectedAtomIsotopes.FirstOrDefault(atom =>
                atom.NeutronCount - selectedItem.atom.NeutronCount - 1
            == gridItemIndex); */

            /* if (isotopeAtom != null)
                gridItem.SetAtomData(isotopeAtom);
            else
                gridItem.SetActive(false); */
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HUD.LockedFocus = false;
    }

    private void HandleItemSelected(ElementGridItem item)
    {
        var isInteractable = (item.elementData != null);
        btnLoad.interactable = isInteractable;
        btnDelete.interactable = isInteractable;
        btnIsotopes.interactable = isInteractable;

        selectedItem = item;
    }

    public void HandleLoadSelectedItemClicked()
    {
        if (FileSystem.instance.hasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, () => FileSystem.instance.SaveActiveElement(), null,
            () => HandleLoadSelectedItem());
        }
        else
            HandleLoadSelectedItem();
    }

    private void HandleLoadSelectedItem()
    {
        // editor.LoadAtomData(selectedItem.elementData);
        Close();
    }

    public void HandleDeleteSelectedItemClicked()
    {
        var dialogBody = "Deleting an element means that it is gone forever! Are you sure?";
        DialogYesNo.Open("Delete Element", dialogBody, HandleDeleteSelectedItem);
    }
    private void HandleDeleteSelectedItem()
    {
        selectedItem.SetActive(false);
        // FileSystem.instance.DeleteAtom(selectedItem.atom);
    }
}
