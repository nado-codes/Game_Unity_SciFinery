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
        if (elementData is Particle)
            GetOrAddElementGridItem<ParticleGridItem>().SetData(elementData);
        else if (elementData is Atom)
            GetOrAddElementGridItem<AtomGridItem>().SetData(elementData);
        else
            throw new NotImplementedException($"Element of type \"{elementData.GetType().FullName}\" is not yet implemented in call to GridItem.SetData");
    }

    private T GetOrAddElementGridItem<T>() where T : ElementGridItem
        => (GetComponent<T>() ?? gameObject.AddComponent<T>());
}
