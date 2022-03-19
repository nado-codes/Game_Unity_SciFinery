using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogPeriodicTable : MonoBehaviour
{
    private static DialogPeriodicTable instance;
    private GridItem selectedItem;

    private List<GridItem> page1GridItems = new List<GridItem>();
    private List<GridItem> page2GridItems = new List<GridItem>();

    private Button btnLoad, btnDelete, btnIsotopes;

    private Transform page1Transform, page2Transform;

    // TODO: need to get or add the GridItem<T> to the page grid items here
    private void VerifyInitialize()
    {
        var periodicTableGameObject = GameObject.Find("dialogPeriodicTable");

        /* if (instance == null)
            instance = periodicTableGameObject.AddComponent<DialogPeriodicTable<T>>(); */

        page1Transform = transform.Find("page1");
        page2Transform = transform.Find("page2");

        var page1GridTransform = page1Transform.Find("grid");
        // var otherGridItems = page1GridTransform.GetComponentsInChildren<GridItem>().Where(i => !(i is ElementGridItem<T>)).ToList();
        // otherGridItems.ForEach(i => i.enabled = false);
        page1GridItems = page1GridTransform.GetComponentsInChildren<GridItem>().ToList();
        page1GridItems.ForEach(i => i.enabled = true);
        // page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));
        // page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        var page2AtomGridItem = page2Transform.GetComponentInChildren<AtomGridItem>();
        // page2AtomGridItem.Init(); // .. initialise the atom grid item

        var page2GridTransform = page2Transform.Find("grid");
        var page2GridTransforms = page2GridTransform.GetComponentsInChildren<RectTransform>();
        // page2GridItems = page2GridTransform.GetComponentsInChildren<ElementGridItem>().ToList();
        // page2GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        btnLoad = transform.Find("btnLoad").GetComponent<Button>();
        btnDelete = transform.Find("btnDelete").GetComponent<Button>();
        btnIsotopes = page1Transform.Find("btnIsotopes").GetComponent<Button>();

        OpenPage1();
        Close();
    }
    void Start()
    {
        VerifyInitialize();
        // instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();

        if (Input.GetKeyDown(KeyCode.Delete))
            HandleDeleteSelectedItem();
    }

    public void HandleOpenClicked()
    {
        VerifyInitialize();
        gameObject.SetActive(true);

        var loadedElements = FileSystem.LoadElementsOfType(Editor.DesignType);

        foreach (Element elementData in loadedElements)
        {
            // TODO: Create a grid item if the atom won't fit in the table
            var gridItem = page1GridItems[elementData.Id - 1];

            if (gridItem == null)
                throw new ApplicationException($"Expected a gridItem for element with Id {elementData.Id} in call to Open, got null");

            gridItem.SetData(elementData);
        }

        HUD.LockedFocus = true;
        OpenPage1();
    }
    // TODO: need to get or load the elements into the grid items here ... maybe only need to load them once
    private void handleOpen()
    {

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

        // var selectedAtomIsotopes = FileSystem.instance.LoadedAtoms.Where(atom => atom.Name == selectedItem.elementData.Name && atom.IsIsotope);

        /* foreach (ElementGridItem gridItem in page2GridItems)
        {
            var gridItemIndex = page2GridItems.IndexOf(gridItem);
            /* var isotopeAtom = selectedAtomIsotopes.FirstOrDefault(atom =>
                atom.NeutronCount - selectedItem.atom.NeutronCount - 1
            == gridItemIndex); 

            /* if (isotopeAtom != null)
                gridItem.SetAtomData(isotopeAtom);
            else
                gridItem.SetActive(false);
        }*/
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HUD.LockedFocus = false;
    }

    private void HandleItemSelected(GridItem item)
    {
        var isInteractable = item.HasDataOfType(Editor.DesignType);
        btnLoad.interactable = isInteractable;
        btnDelete.interactable = isInteractable;
        btnIsotopes.interactable = isInteractable;
    }

    public void HandleLoadSelectedItemClicked()
    {
        /* if (FileSystem.instance.hasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, () => FileSystem.instance.SaveActiveElement(), null,
            () => HandleLoadSelectedItem());
        }
        else
            HandleLoadSelectedItem(); */
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
        // selectedItem.SetActive(false);
        // FileSystem.instance.DeleteAtom(selectedItem.atom);
    }
}
