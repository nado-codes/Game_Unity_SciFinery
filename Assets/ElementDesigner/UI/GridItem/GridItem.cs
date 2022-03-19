using System;
using UnityEngine;

public class GridItem : MonoBehaviour
{

    public T UseGridItemOfType<T, U>() where U : Element where T : ElementGridItem<U>
        => (GetComponent<ElementGridItem<U>>() ?? gameObject.AddComponent<ElementGridItem<U>>()) as T;

    public bool HasDataOfType(ElementType elementType)
    {
        if (elementType == ElementType.Atom)
            return UseGridItemOfType<AtomGridItem, Atom>().elementData != null;
        else if (elementType == ElementType.Molecule)
            return UseGridItemOfType<MoleculeGridItem, Molecule>().elementData != null;
        else
            throw new NotImplementedException($"Element of type \"{elementType}\" is not yet implemented in call to GridItem.SetData");
    }
    public void SetData(Element elementData)
    {
        if (elementData is Atom)
            UseGridItemOfType<AtomGridItem, Atom>().SetAtomData(elementData as Atom);
        else
            throw new NotImplementedException($"Element of type \"{elementData.GetType()}\" is not yet implemented in call to GridItem.SetData");
    }
}
