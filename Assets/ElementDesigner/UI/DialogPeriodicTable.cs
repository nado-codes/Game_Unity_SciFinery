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
    private GridItem page2AtomGridItem;
    private List<GridItem> page2GridItems = new List<GridItem>();

    private Button btnLoad, btnDelete, btnIsotopes;

    private Transform stdLayoutTransform, isotopeLayoutTransform;

    // TODO: need to get or add the GridItem<T> to the page grid items here
    private void VerifyInitialize()
    {
        stdLayoutTransform = transform.Find("Layout_Std");
        Assertions.AssertNotNull(stdLayoutTransform, "stdLayoutTransform");
        isotopeLayoutTransform = transform.Find("Layout_Isotope");
        Assertions.AssertNotNull(isotopeLayoutTransform, "isotopeLayoutTransform");

        var page1GridTransform = stdLayoutTransform.Find("grid");
        Assertions.AssertNotNull(page1GridTransform, "page1GridTransform");
        page1GridItems = page1GridTransform.GetComponentsInChildren<GridItem>().ToList();
        Assertions.AssertNotEmpty(page1GridItems, "page1GridItems");
        page1GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => handleItemSelected(item)));

        var page2AtomGridItemTransform = isotopeLayoutTransform.transform.Find("ElementButton");
        Assertions.AssertNotNull(page2AtomGridItemTransform, "page2AtomGridItemTransform");
        page2AtomGridItem = page2AtomGridItemTransform.GetComponent<GridItem>();
        Assertions.AssertNotNull(page2AtomGridItem, "page2AtomGridItem");
        var page2GridTransform = isotopeLayoutTransform.Find("grid");
        Assertions.AssertNotNull(page2GridTransform, "page2GridTransform");
        page2GridItems = page2GridTransform.GetComponentsInChildren<GridItem>().ToList();
        Assertions.AssertNotEmpty(page2GridItems, "page2GridItems");
        page2GridItems.ForEach(item => item.GetComponent<Button>().onClick.AddListener(() => handleItemSelected(item)));

        btnLoad = transform.Find("btnLoad").GetComponent<Button>();
        Assertions.AssertNotNull(btnLoad, "btnLoad");
        btnDelete = transform.Find("btnDelete").GetComponent<Button>();
        Assertions.AssertNotNull(btnDelete, "btnDelete");
        btnIsotopes = stdLayoutTransform.Find("btnIsotopes").GetComponent<Button>();
        Assertions.AssertNotNull(btnIsotopes, "btnIsotopes");

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
            handleDeleteSelectedItem();
    }

    public void HandleOpenClicked()
    {
        VerifyInitialize();
        gameObject.SetActive(true);

        var loadedElements = loadElementsOfType(Editor.DesignType).Where(el => !el.IsDeleted);

        page1GridItems.ForEach(gi => gi.SetActive(false));

        foreach (Element elementData in loadedElements)
        {
            try
            {
                // TODO: Create a grid item if the atom won't fit in the table
                var isAtom = elementData is Atom;
                var index = (!isAtom ? elementData.Id : (elementData as Atom).Number) - 1;

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

        HUD.LockFocus();
        OpenPage1();
    }
    public async void HandleLoadSelectedItemClicked()
    {
        await Editor.CheckUnsaved();
        handleLoadSelectedItem();
    }
    public void HandleDeleteSelectedItemClicked()
    {
        var dialogBody = "A deleted element can't be used, but you can get it back later if you want. Continue with deletion?";
        DialogYesNo.Open("Delete Element", dialogBody, handleDeleteSelectedItem);
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
        var selectedAtomIsotopes = allIsotopes.Where(i => i.ParentId == selectedElementData.Id && !i.IsDeleted);

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
            var neutronCount = isotope.Particles.Count(p => p.Charge == 0);
            var isotopeGridItem = isotopeGridItems.ElementAt(neutronCount);

            if (isotopeGridItem == null)
                throw new ApplicationException("Expected an AtomGridItem to store isotope in call to OpenPage2, got null");

            isotopeGridItem.SetData(isotope);
        }
    }
    public void Close()
    {
        gameObject.SetActive(false);
        HUD.ClearFocus();

        page1GridItems.ForEach(gi => gi.SetData(null));
        page2GridItems.ForEach(gi => gi.SetData(null));
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
    private void handleItemSelected(GridItem item)
    {
        var isInteractable = item.HasDataOfType(Editor.DesignType);
        btnLoad.interactable = isInteractable;
        btnDelete.interactable = isInteractable;
        btnIsotopes.interactable = isInteractable;

        selectedGridItem = item;
    }
    private void handleLoadSelectedItem()
    {
        var selectedElementCopy = selectedElementData.Copy();
        Editor.LoadElement(selectedElementCopy);
        Close();
    }
    private void handleDeleteSelectedItem()
    {
        selectedGridItem.GetGridItemForType(selectedElementData.ElementType).SetActive(false);
        FileSystem.RecycleRestoreElement(selectedElementData, true);
    }
}
