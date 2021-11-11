using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragboxTest : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;
    public Vector3 dist;
    public Vector3 center;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position - new Vector3(1,0,1);
        end = start + new Vector3(2,0,2);
    }

    // Update is called once per frame
    void Update()
    {
        dist = end-start;
        center = dist*.5f;
        transform.localScale = new Vector3(-dist.x/10,1,-dist.z/10);
        transform.position = start+center;

        Debug.DrawRay(start,transform.up,Color.green,.01f);
        Debug.DrawRay(end,transform.up,Color.red,.01f);
        Debug.DrawRay(start+center,transform.up,Color.blue,.01f);
        Debug.DrawLine(start+transform.up,end+transform.up,Color.yellow,.01f);
    }
}
