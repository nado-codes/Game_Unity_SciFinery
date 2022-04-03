using UnityEngine;
using System;

public class WorldElement : MonoBehaviour
{
    public Element Data { get; protected set; }
    void Start()
    {

    }

    void Update()
    {

    }

    public void SetData(Element elementData) => Data = elementData;
}