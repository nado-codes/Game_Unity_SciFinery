using System;
using UnityEngine;

public class FileSystemCachev2<T> : MonoBehaviour where T : Element
{
    private static FileSystemCachev2<T> instance;
    public static FileSystemCachev2<T> Instance
    {
        get
        {
            if (instance == null)
                instance = Camera.main.gameObject.AddComponent<FileSystemCachev2<T>>();
            if (instance == null)
                throw new ApplicationException($"Unable to setup cache for type {typeof(T).FullName}. Make sure there is a Main Camera in the scene.");

            return instance;
        }
    }
}