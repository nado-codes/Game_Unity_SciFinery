using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Editor : MonoBehaviour
{
    private static List<Interact> selection = new List<Interact>();
    public static Interact CurrentHighlight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetMouseButtonUp(0) && !CurrentHighlight)
        {
            Debug.Log("cleared selection");
            selection.ForEach(s => s.Deselect());
            selection.Clear();
        }
    }



    public static void Select(Interact objectToSelect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);
        var isMultiDeselect = Input.GetKey(KeyCode.LeftControl);

        if(!isMultiSelect && !isMultiDeselect)
        {
            Debug.Log("cleared selection");
            selection.ForEach(s => s.Deselect());
            selection.Clear();
        }

        if(!isMultiDeselect)
        {
            Debug.Log("added one object to selection");
            if(!selection.Contains(objectToSelect))
                selection.Add(objectToSelect);
        }
        else
        {
            Debug.Log("removed one object from selection");
            objectToSelect.Deselect();
            selection.Remove(objectToSelect);
        }
    }
}
