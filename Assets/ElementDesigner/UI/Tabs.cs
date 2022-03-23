using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public delegate void TabClick(int tabId);
public class Tabs : MonoBehaviour
{
    private List<Tab> tabs = new List<Tab>();

    public int selectedTabIndex = 0;

    public TabClick OnSelectedTabChanged;


    // Start is called before the first frame update
    void Start()
    {
        tabs = GetComponentsInChildren<Tab>().ToList();

        var firstTab = tabs.FirstOrDefault();

        tabs.ForEach((tab) =>
        {
            tab.OnClick += () => handleTabClicked(tab.index);

            var currentTabIndex = tabs.IndexOf(tab);
            if (currentTabIndex == selectedTabIndex)
                tab.Select();
            else
                tab.Deselect();
        });
    }

    void handleTabClicked(int index)
    {
        Debug.Log("selected tab " + index);
        var tabToSelect = tabs.FirstOrDefault(tab => tabs.IndexOf(tab) == index);
        tabToSelect.Select();

        var otherTabs = tabs.Where((_, i) => i != index).ToList();
        otherTabs.ForEach(tab => tab.Deselect());

        OnSelectedTabChanged?.Invoke(index);
    }
}
