using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogPeriodicTable : MonoBehaviour
{
    private Element selectedElementData
        => selectedGridItem.GetGridItemForType(Editor.DesignType).elementData;
    private GridItem selectedGridItem;

    private List<GridItem> page1GridItems = new List<GridItem>();
    private GridItem page2AtomGridItem;
    private List<GridItem> page2GridItems = new List<GridItem>();

    private Button btnLoad, btnDelete, btnIsotopes;

    private Transform stdLayoutTransform, isotopeLayoutTransform;

    // TODO: need to get or add the GridItem<T> to the page grid items here
    private void VerifyInitialize()
    {
        stdLayoutTransform = transform.Find("Layout_Std");
        AssertNotNull(stdLayoutTransform, "stdLayoutTransform");
        isotopeLayoutTransform = transform.Find("Layout_Isotope");
        AssertNotNull(isotopeLayoutTransform, "isotopeLayoutTransform");

        var page1GridTransform = stdLayoutTransform.Find("grid");
        AssertNotNull(page1GridTransform, "page1GridTransform");
        page1GridItems = page1GridTransform.GetComponentsInChildren<GridItem>().ToList();
        AssertNotEmpty(page1GridItems, "page1GridItems");
        page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        var page2AtomGridItemTransform = isotopeLayoutTransform.transform.Find("gridItem");
        AssertNotNull(page2AtomGridItemTransform, "page2AtomGridItem");
        page2AtomGridItem = page2AtomGridItemTransform.GetComponent<GridItem>();
        AssertNotNull(page2AtomGridItem, "page2AtomGridItem");
        var page2GridTransform = isotopeLayoutTransform.Find("grid");
        AssertNotNull(page2GridTransform, "page2GridTransform");
        page2GridItems = page2GridTransform.GetComponentsInChildren<GridItem>().ToList();
        AssertNotEmpty(page2GridItems, "page2GridItems");
        page2GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => HandleItemSelected(item)));

        btnLoad = transform.Find("btnLoad").GetComponent<Button>();
        AssertNotNull(btnLoad, "btnLoad");
        btnDelete = transform.Find("btnDelete").GetComponent<Button>();
        AssertNotNull(btnDelete, "btnDelete");
        btnIsotopes = stdLayoutTransform.Find("btnIsotopes").GetComponent<Button>();
        AssertNotNull(btnIsotopes, "btnIsotopes");

        OpenPage1();
        Close();
    }
    private void AssertNotNull<T>(T obj, string propertyName, [CallerMemberName] string callerName = "")
    {
        if (obj == null)
            throw new NullReferenceException($"Expected {propertyName} in call to {callerName}, got null");
    }
    private void AssertNotEmpty<T>(IEnumerable<T> obj, string propertyName, [CallerMemberName] string callerName = "")
    {
        if (obj.Count() < 1)
            throw new ApplicationException($"{propertyName} is not allowed to be empty in call to {callerName}");
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
                => FileSystemCache.GetOrLoadElementsOfType<Atom>().Where((a) => a.ParentId == -1),
            _ => FileSystemCache.GetOrLoadElementsOfType(elementType)
        };

    // TODO: need to get or load the elements into the grid items here ... maybe only need to load them once
    private void handleOpen()
    {

    }

    public void OpenPage1()
    {
        isotopeLayoutTransform.gameObject.SetActive(false);
        stdLayoutTransform.gameObject.SetActive(true);

        if (Editor.DesignType == ElementType.Atom)
            btnIsotopes.gameObject.SetActive(true);
        else
            btnIsotopes.gameObject.SetActive(false);
    }

    public void OpenPage2()
    {
        isotopeLayoutTransform.gameObject.SetActive(true);
        stdLayoutTransform.gameObject.SetActive(false);

        page2AtomGridItem.SetData(selectedElementData);

        var allAtoms = FileSystemCache.GetOrLoadElementsOfType<Atom>();
        var allIsotopes = allAtoms.Where(a => a.ParentId != -1);
        var selectedAtomIsotopes = allIsotopes.Where(i => i.ParentId == selectedElementData.Id);

        if (!selectedAtomIsotopes.Any())
            return;

        var isotopeGridItems = page2GridItems.Select(gi =>
        {
            var atomGridItem = gi.GetGridItemForType<AtomGridItem>();

            if (atomGridItem == null)
                atomGridItem = gi.gameObject.AddComponent<AtomGridItem>();

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

        selectedGridItem = item;
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
