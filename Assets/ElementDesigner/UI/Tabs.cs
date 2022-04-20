using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void TabClick(int tabId);
public class Tabs : MonoBehaviour
{
    private List<Tab> tabs = new List<Tab>();

    public int SelectedTabIndex = 0;

    public TabClick OnSelectedTabChanged;


    // Start is called before the first frame update
    void Start()
    {
        tabs = GetComponentsInChildren<Tab>().ToList();

        tabs.ForEach((tab) =>
        {
            tab.OnClick += () => handleTabClicked(tab.index);

            var currentTabIndex = tabs.IndexOf(tab);
            if (currentTabIndex == SelectedTabIndex)
                tab.Select();
            else
                tab.Deselect();
        });
    }

    void handleTabClicked(int index)
    {
        if (index < 0 || index > tabs.Count - 1)
            throw new ArgumentException($"index must be between 0 and {tabs.Count - 1} inclusive");
        SelectTab(index);
        OnSelectedTabChanged?.Invoke(index);
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index > tabs.Count - 1)
            throw new ArgumentException($"index must be between 0 and {tabs.Count - 1} inclusive");

        var tabToSelect = tabs.FirstOrDefault(tab => tabs.IndexOf(tab) == index);
        tabToSelect.Select();

        var otherTabs = tabs.Where((_, i) => i != index).ToList();
        otherTabs.ForEach(tab => tab.Deselect());
    }
}
