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
    private EventSystem eventSystem;
    public int selectedTabIndex = 0;

    const int tabSelectedAlpha = 255;
    const int tabNormalAlpha = 123;

    public TabClick onTabChanged;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        tabs = GetComponentsInChildren<Button>().ToList();

        foreach(Button tab in tabs)
        {
            var normalColor = tab.colors.normalColor;
            var selectedColor = tab.colors.selectedColor;

            // TODO: setup normal & selected button colors for tabs
            // var newNormalColor

            // tab.colors.normalColor = Color.black; 
            //tab.colors.normalColor = new Color(normalColor.r,normalColor.g,normalColor.b,tabNormalAlpha);
            //tab.SetCol

            tab.onClick.AddListener(() => handleTabClicked(tab));
        }
    }

    void handleTabClicked(Button tab)
    {
        selectedTabIndex = tabs.IndexOf(tab);

        // TODO: when one tab is selected, deselect the other ones by changing their color
        var otherTabs = tabs.Where((_,i) => i != selectedTabIndex).ToList();
        // otherTabs.ForEach(tab => tab.

        onTabChanged(selectedTabIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
