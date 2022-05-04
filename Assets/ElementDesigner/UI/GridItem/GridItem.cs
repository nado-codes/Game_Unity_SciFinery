using System;
using System.Linq;
using UnityEngine;

public class GridItem : MonoBehaviour
{
    public ElementGridItem UseGridItemForType(ElementType elementType)
    {
        var otherGridItems = GetComponents<ElementGridItem>().Where(gridItem => gridItem.elementDataType != elementType).ToList();
        otherGridItems.ForEach(gridItem => Destroy(gridItem));

        if (elementType == ElementType.Atom)
            return GetOrAddElementGridItem<AtomGridItem>();
        else if (elementType == ElementType.Molecule)
            return GetOrAddElementGridItem<MoleculeGridItem>();
        else
            throw new NotImplementedException($"Element of type \"{elementType}\" is not yet implemented in call to GridItem.SetData");
    }
    public bool HasDataOfType(ElementType elementType)
    {
        if (elementType == ElementType.Atom)
            return true; // GetOrAddElementGridItem<AtomGridItem, Atom>().elementData != null;
        else if (elementType == ElementType.Molecule)
            return true; // GetOrAddElementGridItem<MoleculeGridItem, Molecule>().elementData != null;
        else
            throw new NotImplementedException($"Element of type \"{elementType}\" is not yet implemented in call to GridItem.SetData");
    }
    public void SetData(Element elementData)
    {
        if (elementData == null)
        {
            var elementGridItems = GetComponents<ElementGridItem>().ToList();
            elementGridItems.ForEach(gi => gi.SetData(null));
            return;
        }

        switch (elementData.ElementType)
        {
            case ElementType.Particle:
                GetOrAddElementGridItem<ParticleGridItem>().SetData(elementData as Particle);
                break;
            case ElementType.Atom:
                GetOrAddElementGridItem<AtomGridItem>().SetData(elementData as Atom);
                break;
            default:
                GetOrAddElementGridItem<ElementGridItem>().SetData(elementData);
                break;
        }
    }

    public T GetGridItemForType<T>() where T : ElementGridItem
        => GetComponent<T>();

    public ElementGridItem GetGridItemForType(ElementType elementType) =>
    elementType switch
    {
        ElementType.Particle => GetComponent<ParticleGridItem>(),
        ElementType.Atom => GetComponent<AtomGridItem>(),
        _ => GetComponent<ElementGridItem>()
    };

    private T GetOrAddElementGridItem<T>() where T : ElementGridItem
        => (GetComponent<T>() ?? gameObject.AddComponent<T>());
}
