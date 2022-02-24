using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public delegate void TabClick(int tabId);
public class Tabs : MonoBehaviour
{
    private List<Button> tabs = new List<Button>();

    public int selectedTabIndex = 0;

    private const int tabSelectedAlpha = 255, tabNormalAlpha = 123;
    private ColorBlock tabSelectedColors, tabDeselectedColors;

    public TabClick onTabChanged;

    // Start is called before the first frame update
    void Start()
    {
        tabs = GetComponentsInChildren<Button>().ToList();
        Debug.Log("tab count="+tabs.Count);

        var firstTab = tabs.FirstOrDefault();
        var normalColor = firstTab.colors.normalColor;
        var selectedColor = firstTab.colors.selectedColor;
        var newNormalColor = new Color(normalColor.r,normalColor.g,normalColor.b,tabNormalAlpha);
        var newSelectedColor = new Color(selectedColor.r,selectedColor.g,selectedColor.b,tabSelectedAlpha);

        tabDeselectedColors = firstTab.colors;
        tabDeselectedColors.normalColor = newNormalColor;
        tabSelectedColors = firstTab.colors;
        tabSelectedColors.normalColor = newSelectedColor;

        Debug.Log("tabDeselectedColors="+tabDeselectedColors.normalColor);
        Debug.Log("tabSelectedColors="+tabSelectedColors.normalColor);

        tabs.ForEach((tab) => {
            var currentTabIndex = tabs.IndexOf(tab);

            tab.onClick.AddListener(() => handleTabClicked(currentTabIndex)); 
            tab.colors = (currentTabIndex == selectedTabIndex) ? tabSelectedColors : tabDeselectedColors;
        });
    }

    void handleTabClicked(int index)
    {
        Debug.Log("selected tab "+index);
        var tabToSelect = tabs.FirstOrDefault(tab => tabs.IndexOf(tab) == index);
        tabToSelect.colors = tabSelectedColors;

        var otherTabs = tabs.Where((_,i) => i != index).ToList();
        otherTabs.ForEach(tab => tab.colors = tabDeselectedColors);

        onTabChanged(selectedTabIndex);
    }
}
