using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementDesignerCamera : FlyCam
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if(!HUD.LockedFocus)
            base.Update();
    }
}
