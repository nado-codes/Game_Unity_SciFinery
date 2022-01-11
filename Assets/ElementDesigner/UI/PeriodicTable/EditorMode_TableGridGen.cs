using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EditorMode_TableGridGen : MonoBehaviour
{
    public bool refreshGrid = false;
    public GameObject gridItemPrefab;
    public int columns = 3;
    public int rows = 3;
    public int itemWidth = 90;

    private static List<GameObject> gridItems = new List<GameObject>();
    private Vector3 gridStartPos;
    private Vector3 gridLastStartPos;

    // Update is called once per frame
    void Update()
    {
        var gridStartObject = transform.Find("GridStartPos");
        var gridEnd = transform.Find("GridEndPos")?.GetComponent<RectTransform>();

        if(gridStartObject != null && gridEnd != null)
        {
            gridStartObject.gameObject.SetActive(!EditorApplication.isPlaying);
            gridEnd.gameObject.SetActive(!EditorApplication.isPlaying);

            if((refreshGrid || gridStartPos != gridLastStartPos) && !EditorApplication.isPlaying)
            {
                gridStartPos = gridStartObject.GetComponent<RectTransform>().localPosition;
                gridEnd.transform.localPosition = gridStartPos + new Vector3(itemWidth*(columns-1),itemWidth*-(rows-1),0);

                GenerateGrid();
                refreshGrid = false;
            }
        }
    }

    void ClearGrid()
    {
        gridItems.ForEach(i => GameObject.DestroyImmediate(i.gameObject));
        gridItems.Clear();

        if(gridItemPrefab == null) return;

        var otherGridItems = GetComponentsInChildren<PeriodicTableGridItem>().ToList();
        otherGridItems.ForEach(i => GameObject.DestroyImmediate(i.gameObject));
    }
    void GenerateGrid()
    {
        ClearGrid();
        Debug.Log("Generated grid at "+DateTime.Now);

        gridLastStartPos = gridStartPos;

        if(gridItemPrefab == null) return;

        for(var y = 0; y < rows; ++y)
        {
            for(var x = 0; x < columns; ++x)
            {
                var newGridItem = Instantiate(gridItemPrefab);
                newGridItem.name = y.ToString()+"_"+x.ToString();
                newGridItem.transform.parent = transform;
                newGridItem.transform.localScale = Vector3.one;

                // TODO: We don't need this. To be removed. Only used to visualise the numbers on the grid
                var script = newGridItem.GetComponent<PeriodicTableGridItem>();
                script.Awake();
                script.SetNumber(x+(y*columns)+1);

                var newGridItemRect = newGridItem.GetComponent<RectTransform>();
                newGridItemRect.localPosition = gridStartPos + new Vector3(itemWidth*x,itemWidth*-y,0);
                
                gridItems.Add(newGridItem);
            }
        }
    }
}
