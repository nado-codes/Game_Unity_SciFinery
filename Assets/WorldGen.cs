using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
    public GameObject tilePrefab;
    private GameObject worldContainer;

    // Start is called before the first frame update
    void Start()
    {
        MakeWorld();
    }

    void MakeWorld()
    {
        worldContainer = new GameObject();

        for(int x = 0; x < 10; ++x)
        {
            for(int y = 0; y < 10; ++y)
            {
                MakeTile(x,y);
            }
        }
    }

    private void MakeTile(float x, float y)
    {
        var newTile = GameObject.Instantiate(tilePrefab);

        newTile.transform.position = new Vector3(x*1.025f,0,y*1.025f);
        newTile.transform.parent = worldContainer.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
