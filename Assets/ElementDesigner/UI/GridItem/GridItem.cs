using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GridItem : MonoBehaviour
{
    public bool StartActive = false;
    private bool initialized = false;

    protected Text numberText, shortNameText, nameText, weightText;
    private ColorBlock buttonColorsActive, buttonColorsInactive;
    protected Button button;

    protected virtual void VerifyInitialize()
    {
        if (initialized)
            return;

        initialized = true;

        numberText = transform.Find("Number")?.GetComponent<Text>();
        shortNameText = transform.Find("ShortName")?.GetComponent<Text>();
        nameText = transform.Find("Name")?.GetComponent<Text>();
        weightText = transform.Find("Weight")?.GetComponent<Text>();

        button = GetComponent<Button>();
        buttonColorsActive = button.colors;
        buttonColorsInactive.normalColor = button.colors.disabledColor;
        buttonColorsInactive.highlightedColor = button.colors.disabledColor;
        buttonColorsInactive.pressedColor = button.colors.disabledColor;
        buttonColorsInactive.selectedColor = button.colors.disabledColor;

        SetActive(StartActive);
    }

    protected virtual void Start() => VerifyInitialize();

    public T GetOrAddElementGridItem<T, U>() where U : Element where T : ElementGridItem<U>
        => (GetComponent<T>() ?? gameObject.AddComponent<T>());

    public bool HasDataOfType(ElementType elementType)
    {
        if (elementType == ElementType.Atom)
            return GetOrAddElementGridItem<AtomGridItem, Atom>().elementData != null;
        else if (elementType == ElementType.Molecule)
            return GetOrAddElementGridItem<MoleculeGridItem, Molecule>().elementData != null;
        else
            throw new NotImplementedException($"Element of type \"{elementType}\" is not yet implemented in call to GridItem.SetData");
    }
    public void SetData(Element elementData)
    {
        if (elementData is Particle)
            GetOrAddElementGridItem<ParticleGridItem, Particle>().SetParticleData(elementData as Particle);
        else if (elementData is Atom)
            GetOrAddElementGridItem<AtomGridItem, Atom>().SetAtomData(elementData as Atom);
        else
            throw new NotImplementedException($"Element of type \"{elementData.GetType()}\" is not yet implemented in call to GridItem.SetData");
    }

    public virtual void SetActive(bool active)
    {
        var allText = transform.GetComponentsInChildren<Text>().ToList();
        allText.ForEach(t => t.gameObject.SetActive(active));
        button.interactable = active;
    }
}
