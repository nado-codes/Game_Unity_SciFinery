using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class FileSystemCache : MonoBehaviour
{
    private static FileSystemCache instance;
    public static FileSystemCache Instance
    {
        get
        {
            if (instance == null)
            {
                var newFileSystem = FindObjectOfType<FileSystemCache>();

                if (newFileSystem == null)
                    newFileSystem = Camera.main.gameObject.AddComponent<FileSystemCache>();

                instance = newFileSystem;
            }

            return instance;
        }
    }
    private List<Element> elements = new List<Element>();
    private List<Element> subElements = new List<Element>();


    // TODO: "FileSystemCache" should work like a Repository
    public IEnumerable<Element> GetAll()
    {
        throw new NotImplementedException();
    }
    public Element GetById(int id)
    {
        throw new NotImplementedException();
    }
    public int Add(Element element)
    {
        throw new NotImplementedException();
    }

    public static void AddElement(Element element)
    {
        var allElementsOfType = FileSystemCache.GetOrLoadElementsOfType(element.ElementType);
        element.Id = allElementsOfType.Count() + 1;
        instance.elements.Add(element);
    }

    public static void AddSubElement(Element element)
    {
        instance.subElements.Add(element);
    }

    // TODO
    /* public void Update(Element element)
    {

    } */

    public void Delete()
    {

    }

    public static IEnumerable<Element> GetOrLoadElementsOfType(ElementType type)
    {
        var firstElement = Instance.elements.FirstOrDefault();

        if (firstElement?.ElementType != type || firstElement == null)
            Instance.elements = FileSystemLoader.LoadElementsOfType(type).ToList();

        return Instance.elements;
    }
    public static IEnumerable<T> GetOrLoadElementsOfType<T>() where T : Element
    {
        if (!Enum.TryParse(typeof(T).FullName, out ElementType type))
            throw new ArgumentException($"Element of type \"{typeof(T).FullName}\" is not parsable to ElementType");

        var firstElement = Instance.elements.FirstOrDefault();

        if (firstElement?.ElementType != type || firstElement == null)
            Instance.elements = FileSystemLoader.LoadElementsOfType<T>().ToList<Element>();

        return Instance.elements.Cast<T>();
    }
    public static T GetOrLoadElementOfTypeById<T>(int id) where T : Element
        => getOrLoadElementOfTypeById<T>(id);
    public static IEnumerable<T> GetOrLoadElementsOfTypeByIds<T>(IEnumerable<int> ids) where T : Element
        => getOrLoadElementsOfTypeByIds<T>(ids);
    public static Element GetOrLoadElementOfTypeById(ElementType elementType, int id)
       => elementType switch
       {
           ElementType.Particle => getOrLoadElementOfTypeById<Particle>(id),
           ElementType.Atom => getOrLoadElementOfTypeById<Atom>(id),
           _ => throw new NotImplementedException(
               $"Element type \"{elementType.ToString()}\" is not implemented in call to LoadElementOfTypeById"
            )
       };
    public static IEnumerable<Element> GetOrLoadSubElementsOfType(ElementType type)
    {
        var firstElement = Instance.subElements.FirstOrDefault();

        if (firstElement?.ElementType != type || firstElement == null)
            Instance.subElements = FileSystemLoader.LoadElementsOfType(type).ToList();

        return Instance.subElements;
    }
    public static IEnumerable<T> GetOrLoadSubElementsOfType<T>() where T : Element
    {
        if (!Enum.TryParse(typeof(T).FullName, out ElementType type))
            throw new ArgumentException($"Element of type \"{typeof(T).FullName}\" is not parsable to ElementType");

        var firstElement = Instance.subElements.FirstOrDefault();

        if (firstElement?.ElementType != type || firstElement == null)
            Instance.subElements = FileSystemLoader.LoadElementsOfType(type).ToList();

        return Instance.subElements.Cast<T>();
    }
    public static Element GetOrLoadSubElementOfTypeById<T>(int id) where T : Element
        => getOrLoadSubElementOfTypeById<T>(id);
    public static IEnumerable<T> GetOrLoadSubElementsOfTypeByIds<T>(IEnumerable<int> ids) where T : Element
        => getOrLoadSubElementsOfTypeByIds<T>(ids);
    public static Element GetOrLoadSubElementOfTypeById(ElementType elementType, int id)
       => elementType switch
       {
           ElementType.Particle => getOrLoadSubElementOfTypeById<Particle>(id),
           ElementType.Atom => getOrLoadSubElementOfTypeById<Atom>(id),
           _ => throw new NotImplementedException(
               $"Element type \"{elementType.ToString()}\" is not implemented in call to LoadElementOfTypeById"
            )
       };
    public static void ReloadElementOfTypeById<T>(int id) where T : Element
    {
        var oldElement = Instance.elements.FirstOrDefault(el => el.Id == id);
        var indexToUpdate = Instance.elements.IndexOf(oldElement);
        var updatedElement = FileSystemLoader.LoadElementOfTypeById<T>(id);

        if (indexToUpdate == -1 || Instance.elements.Count() <= indexToUpdate)
            throw new NullReferenceException($"Element with id {id} does not exist in elements array");

        Instance.elements[indexToUpdate] = updatedElement;
    }
    public static void UpdateElement(Element element)
    {
        var oldElement = Instance.elements.FirstOrDefault(el => el.Id == element.Id);
        var indexToUpdate = Instance.elements.IndexOf(oldElement);
        Instance.elements[indexToUpdate] = element;
    }
    public static void RemoveElementOfTypeById<T>(int id) where T : Element
    {
        if (!Enum.TryParse(typeof(T).FullName, out ElementType type))
            throw new ArgumentException($"Element of type \"{typeof(T).FullName}\" is not parsable to ElementType");

        RemoveElementOfTypeById(id, type);
    }
    public static void RemoveElementOfTypeById(int id, ElementType type)
    {
        if (!Instance.elements.ContainsElementsOfType(type))
            throw new ApplicationException($"Elements list does not match type ${type}");

        var elementToRemove = Instance.elements.FirstOrDefault(el => el.Id == id);
        if (elementToRemove == null)
            throw new NullReferenceException($"Element with id {id} doesn't exist");

        Instance.elements.Remove(elementToRemove);
    }

    private static IEnumerable<T> getOrLoadElementsOfTypeByIds<T>(IEnumerable<int> ids) where T : Element
    {
        var shouldReload = !Instance.elements.ContainsElementsOfType<T>();
        if (shouldReload)
            Instance.elements = FileSystemLoader.LoadElementsOfType<T>().ToList<Element>();

        var elementsById = Instance.elements.Where(el => ids.Contains(el.Id));
        return elementsById.Cast<T>();
    }
    private static T getOrLoadElementOfTypeById<T>(int id) where T : Element
    {
        var shouldReload = !Instance.elements.ContainsElementsOfType<T>();
        if (shouldReload)
            Instance.elements = FileSystemLoader.LoadElementsOfType<T>().ToList<Element>();

        var elementById = Instance.elements.FirstOrDefault(el => el.Id == id);

        if (elementById == null)
            throw new NullReferenceException($"Element with id {id} doesn't exist in call to getOrLoadElementOfTypeById");

        return elementById as T;
    }
    private static IEnumerable<T> getOrLoadSubElementsOfTypeByIds<T>(IEnumerable<int> ids) where T : Element
    {
        var shouldReload = !Instance.subElements.ContainsElementsOfType<T>();
        if (shouldReload)
            Instance.subElements = FileSystemLoader.LoadElementsOfType<T>().ToList<Element>();

        var elementsById = Instance.subElements.Where(el => ids.Contains(el.Id));
        return elementsById.Cast<T>();
    }
    private static T getOrLoadSubElementOfTypeById<T>(int id) where T : Element
    {
        var shouldReload = Instance.subElements.ContainsElementsOfType<T>();
        if (shouldReload)
            Instance.subElements = FileSystemLoader.LoadElementsOfType<T>().ToList<Element>();

        var elementById = Instance.subElements.FirstOrDefault(el => el.Id == id);
        return elementById as T;
    }
}

public static class ElementListExtension
{
    public static bool ContainsElementsOfType<T>(this List<Element> list)
    {
        if (!Enum.TryParse(typeof(T).FullName, out ElementType type))
            throw new ArgumentException($"Element of type \"{typeof(T).FullName}\" is not parsable to ElementType");

        return list.ContainsElementsOfType(type);
    }
    public static bool ContainsElementsOfType(this List<Element> list, ElementType type)
        => list.Any(el => el.ElementType == type);
}