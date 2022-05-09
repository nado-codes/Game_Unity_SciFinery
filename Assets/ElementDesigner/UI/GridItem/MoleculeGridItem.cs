using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeGridItem : ElementGridItem
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // TODO: add ShortName set for molecules (basically works the same as an AtomGridItem)
    /*
        shortNameText = ActiveLayout.Find("ShortName")?.GetComponent<Text>();
        Assertions.AssertNotNull(shortNameText, "shortNameText");
        nameText.text = elementData?.ShortName ?? string.Empty;
    */
}
