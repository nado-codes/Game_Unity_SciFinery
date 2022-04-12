using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogPeriodicTable : MonoBehaviour
{
    private Element selectedElementData
        => selectedGridItem.GetGridItemForType(Editor.DesignType).elementData;
    private GridItem selectedGridItem;

    private List<GridItem> page1GridItems = new List<GridItem>();
    private List<GridItem> page2GridItems = new List<GridItem>();

    private Button btnLoad, btnDelete, btnIsotopes;

    private Transform stdLayoutTransform, particleLayoutTransform;

    // TODO: need to get or add the GridItem<T> to the page grid items here
    private void VerifyInitialize()
    {
        stdLayoutTransform = transform.Find("Layout_Std");
        particleLayoutTransform = transform.Find("Layout_Isotope");

        var page1GridTransform = stdLayoutTransform.Find("grid");
        page1GridItems = page1GridTransform.GetComponentsInChildren<GridItem>().ToList();
        page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        var page2AtomGridItem = particleLayoutTransform.GetComponentInChildren<AtomGridItem>();
        var page2GridTransform = particleLayoutTransform.Find("grid");
        var page2GridTransforms = page2GridTransform.GetComponentsInChildren<RectTransform>();
        // page2GridItems = page2GridTransform.GetComponentsInChildren<ElementGridItem>().ToList();
        // page2GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        btnLoad = transform.Find("btnLoad").GetComponent<Button>();
        btnDelete = transform.Find("btnDelete").GetComponent<Button>();
        btnIsotopes = stdLayoutTransform.Find("btnIsotopes").GetComponent<Button>();

        OpenPage1();
        Close();
    }
    void Start()
    {
        VerifyInitialize();
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

        var loadedElements = loadElementsOfType(Editor.DesignType);

        foreach (Element elementData in loadedElements)
        {
            try
            {
                // TODO: Create a grid item if the atom won't fit in the table
                var index = (elementData?.Id ?? 0) - 1;

                if (index == -1)
                    throw new ApplicationException($"Invalid or missing index for element {elementData.Name} Id in call to DialogPeriodicTable.Open");

                var gridItem = page1GridItems[index];

                if (gridItem == null)
                    throw new ApplicationException($"Expected a gridItem for element with Id {elementData.Id} in call to DialogPeriodicTable.Open, got null");

                gridItem.SetData(elementData);
                gridItem.GetGridItemForType(elementData.ElementType).OnClick += (Element data) =>
                {
                    selectedGridItem = gridItem;
                };
            }
            catch
            {
                continue;
            }
        }

        HUD.LockedFocus = true;
        OpenPage1();
    }
    private IEnumerable<Element> loadElementsOfType(ElementType elementType) =>
        elementType switch
        {
            ElementType.Atom
                => FileSystemLoader.LoadElementsOfType<Atom>().Where((a) => !atomIsIsotope(a)),
            _ => FileSystemLoader.LoadElementsOfType(elementType)
        };
    private bool atomIsIsotope(Atom el) => el.ParentName != string.Empty && el.ParentName != null;

    // TODO: need to get or load the elements into the grid items here ... maybe only need to load them once
    private void handleOpen()
    {

    }

    public void OpenPage1()
    {
        particleLayoutTransform.gameObject.SetActive(false);
        stdLayoutTransform.gameObject.SetActive(true);

        if (Editor.DesignType == ElementType.Atom)
            btnIsotopes.gameObject.SetActive(true);
        else
            btnIsotopes.gameObject.SetActive(false);
    }

    public void OpenPage2()
    {
        particleLayoutTransform.gameObject.SetActive(true);
        stdLayoutTransform.gameObject.SetActive(false);

        var atomGridItem = particleLayoutTransform.Find("gridItem").GetComponent<AtomGridItem>();
        atomGridItem.SetData(selectedElementData);

        var allAtoms = FileSystemLoader.LoadElementsOfType<Atom>();
        var allIsotopes = allAtoms.Where(a => a.ParentName != null);
        var selectedAtomName = $"{selectedElementData.ShortName.ToLower()}{selectedElementData.Id}";
        var selectedAtomIsotopes = allIsotopes.Where(i => i.ParentName == selectedAtomName);

        var isotopeGridItems = page2GridItems.Select(gi =>
        {
            var atomGridItem = gi.GetGridItemForType<AtomGridItem>();

            if (atomGridItem == null)
                throw new ApplicationException("Expected an AtomGridItem in call to OpenPage2, got null");

            return atomGridItem;
        });

        if (isotopeGridItems.Count() < selectedAtomIsotopes.Max(i => i.Id))
            throw new ApplicationException(
                @$"Not enough grid items to store number of isotopes. 
                Expected: ${selectedAtomIsotopes.Max(i => i.Id)}
                Got: ${isotopeGridItems.Count()}
        ");

        foreach (Atom isotope in selectedAtomIsotopes)
        {
            var isotopeGridItem = isotopeGridItems.ElementAt(isotope.Id);

            if (isotopeGridItem == null)
                throw new ApplicationException("Expected an AtomGridItem to store isotope in call to OpenPage2, got null");

            isotopeGridItem.SetData(isotope);
        }
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
        if (Editor.HasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, () => FileSystem.SaveActiveElement(Editor.SubElements.Select(el => el.Data)), null,
            () => HandleLoadSelectedItem());
        }
        else
            HandleLoadSelectedItem();
    }

    private void HandleLoadSelectedItem()
    {
        if (selectedElementData == null)
            throw new ApplicationException("Expected selectedElementData in call to DialogPeriodicTable.HandleLoadSelectedItem, got null");

        Editor.LoadElement(selectedElementData);
        Close();
    }

    public void HandleDeleteSelectedItemClicked()
    {
        var dialogBody = "Deleting an element means that it is gone forever! Are you sure?";
        DialogYesNo.Open("Delete Element", dialogBody, HandleDeleteSelectedItem);
    }
    private void HandleDeleteSelectedItem()
    {
        selectedGridItem.GetGridItemForType(selectedElementData.ElementType).SetActive(false);
        FileSystem.DeleteElement(selectedElementData);
    }
}
