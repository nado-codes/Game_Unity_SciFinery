using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum DragState { Init, Active, None }
public enum ElementType { Particle = 0, Atom = 1, Molecule = 2, Product = 3 }

public class Editor : MonoBehaviour
{
    private static Editor instance;
    public static Editor Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Editor>();

            return instance;
        }
    }

    private ElementType designType = ElementType.Atom;
    public static ElementType DesignType => Instance.designType;
    public static bool HasUnsavedChanges = false;

    // PREFABS
    [Header("Prefabs")]
    public GameObject particlePrefab;
    public GameObject atomPrefab;
    public GameObject moleculePrefab;

    [Header("Other")]
    // UI
    public Text textClassification;
    public Text textStability;
    public Text textCharge;

    // CAMERA
    private Vector3 cameraStartPos;
    private Quaternion cameraStartAngle;

    // PARTICLES
    public static GameObject elementGameObject;

    private List<WorldElement> subElements = new List<WorldElement>();
    public static List<WorldElement> SubElements { get => Instance.subElements; }

    void Start()
    {
        if (instance == null)
            instance = this;

        if (particlePrefab == null)
            throw new ArgumentNullException("particlePrefab must be set in Editor");

        SubElements.AddRange(FindObjectsOfType<WorldParticle>());

        textClassification = GameObject.Find("Classification")?.transform.Find("Value").GetComponent<Text>();
        textStability = GameObject.Find("TextStability")?.GetComponent<Text>();
        textCharge = GameObject.Find("TextCharge")?.GetComponent<Text>();

        var designTypeTabs = GameObject.Find("tabsDesignType").GetComponent<Tabs>();
        designTypeTabs.OnSelectedTabChanged += (int designTypeId) => HandleChangeDesignTypeClicked((ElementType)designTypeId);

        cameraStartPos = Camera.main.transform.position;
        cameraStartAngle = Camera.main.transform.rotation;

        // NOTE: Start the Editor in an initial state, also setting up the UI
        // with the correct elements and displays
        HandleChangeDesignTypeClicked(ElementType.Atom);
        designTypeTabs.SelectTab((int)ElementType.Atom);
    }
    public async void HandleChangeDesignTypeClicked(ElementType newDesignType)
    {
        switch (newDesignType)
        {
            case ElementType.Atom:
                await handleChangeDesignType<Atom>();
                break;
            case ElementType.Molecule:
                await handleChangeDesignType<Molecule>();
                break;
            default:
                throw new NotImplementedException($"Element of type ${newDesignType} is not yet implemented in call to HandleChangeDesignTypeClicked");
        }

        panelCreate.SetDesignType(newDesignType);
        designType = newDesignType;
        TextNotification.Show("Design Type: " + newDesignType);
    }
    public void HandleSave()
    {
        FileSystem.SaveActiveElement(subElements.Select(el => el.Data));
    }
    private async Task handleChangeDesignType<T>() where T : Element, new()
    {
        if (HasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            var dialogResult = await DialogYesNo.OpenForResult("Save Changes?", dialogBody);

            if (dialogResult == YesNo.Yes)
                HandleSave();
        }

        Editor.Instance.clearSubElements();
        createNewElementOfType<T>();
    }

    public void HandleNewElementClicked()
    {
        if (DesignType == ElementType.Atom)
            handleCreateNewElementOfType<Atom>();
        else
            throw new NotImplementedException($"Design type {DesignType} is not yet implemented in call to Editor.HandleNewElementClicked");
    }
    private void handleCreateNewElementOfType<T>() where T : Element, new()
    {
        if (HasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, HandleSave, null,
            createNewElementOfType<T>);
        }
        else
            createNewElementOfType<T>();
    }
    private void createNewElementOfType<T>() where T : Element, new()
    {
        var typeName = typeof(T).FullName;
        elementGameObject = GameObject.Find($"{typeName}") ?? new GameObject();
        elementGameObject.name = $"{typeName}New{typeName}";

        FileSystem.CreateElementOfType<T>();
        LoadElement(FileSystem.ActiveElement);

        HasUnsavedChanges = false;
    }
    public static void LoadElement<T>(T elementData) where T : Element
    {
        if (elementData == null)
            throw new ArgumentException("Expected elementData in call to Editor.LoadElement, got null");

        // Camera.main.transform.position = instance.cameraStartPos;
        // Camera.main.transform.rotation = instance.cameraStartAngle;
        switch (elementData.ElementType)
        {
            case ElementType.Atom:
                loadAtom(elementData as Atom);
                break;
            case ElementType.Molecule:
                loadMolecule(elementData as Molecule);
                break;
            default:
                throw new NotImplementedException($"Element of type \"{elementData.GetType().FullName}\" is not yet implemented in call to Editor.LoadElementData");
        }

        // .. Make atom particles orbit eccentrically on the first load
        if (elementData.ElementType == ElementType.Atom)
        {
            SubElements.ForEach(el =>
            {
                var nuclei = SubElements.Where(otherEl => otherEl.Data.Id != el.Data.Id && otherEl.Data.Charge > 0);
                var forces = nuclei.Select(otherEl => el.ForceBetween(otherEl));
                var effectiveForce = forces.Average();
                var perpForce = Vector3.Cross(effectiveForce, Vector3.up).normalized * effectiveForce.magnitude;
                Debug.DrawRay(el.transform.position, effectiveForce, Color.red, 30);
                Debug.DrawRay(el.transform.position, perpForce, Color.yellow, 30);
                var motor = el.GetComponent<WorldElementMotor>();
                motor.AddVelocity(perpForce);
            });
        }

        PanelName.SetElementData(elementData);
        TextNotification.Show($"Loaded \"{elementData.Name}\"");
    }

    private static void loadAtom(Atom atomData)
    {
        foreach (Particle particle in atomData.Particles)
        {
            try
            {
                // TODO: later, positions will be able to be saved and re-loaded the next time an element loads
                var radius = particle.Charge >= 0 ? 1 : 20;
                var randPos = UnityEngine.Random.insideUnitSphere * radius;
                var worldElement = CreateSubElement(particle, randPos);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                continue;
            }
        }
    }

    private static void loadMolecule(Molecule moleculeData)
    {
        var atoms = FileSystemCache.GetOrLoadSubElementsOfType(ElementType.Atom);

        foreach (int atomId in moleculeData.AtomIds)
        {
            try
            {
                var atomToCreateData = atoms.FirstOrDefault(p => p.Id == atomId);

                if (atomToCreateData == null)
                    throw new ApplicationException($"No atom found for Id {atomId} in call to Editor.LoadMolecule");

                // TODO: later, positions will be able to be saved and re-loaded the next time an element loads
                var randPos = UnityEngine.Random.insideUnitSphere * 20;
                CreateSubElement(atomToCreateData, randPos);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                continue;
            }
        }
    }

    // .. NOTE: A "sub-element" is any component element of a parent Element, e.g. a Particle is
    // a sub-element of an Atom, and an Atom is a sub-element of a Molecule.
    public static WorldElement CreateSubElement(Element elementData)
    {
        if (elementData == null)
            throw new ApplicationException("elementData cannot be null in call to CreateWorldElement");

        // TODO: later, prefabs for particles, atoms and molecules will be loaded in at runtime using
        // Unity "Addressables" (like AssetBundles) so we don't have to pass them into the component as variables

        WorldElement newWorldElement = null;
        GameObject newWorldElementGO = null;
        var elementType = elementData.ElementType;

        try
        {
            if (elementType == ElementType.Particle)
            {
                newWorldElementGO = Instantiate(Instance.particlePrefab);

                var particleData = elementData as Particle;
                var newWorldParticle = newWorldElementGO.GetComponent<WorldParticle>();
                newWorldParticle.SetParticleData(particleData);

                newWorldElement = newWorldParticle;
                SubElements.Add(newWorldParticle);
            }
            else if (elementType == ElementType.Atom)
            {
                newWorldElementGO = Instantiate(Instance.atomPrefab);

                var atomData = elementData as Atom;
                var newWorldAtom = newWorldElementGO.GetComponent<WorldElement>();

                if (newWorldAtom == null)
                    throw new ApplicationException("The WorldAtom needs a WorldElement component!");

                newWorldAtom.SetData(atomData);

                newWorldElement = newWorldAtom;
                SubElements.Add(newWorldAtom);
            }
            else
                throw new NotImplementedException($"Element of type {elementType} is not yet implemented in call to Editor.CreateWorldElement");

            newWorldElementGO.transform.parent = elementGameObject.transform;
            newWorldElement.SetData(elementData);

            FileSystem.UpdateActiveElement();
            PanelName.SetElementData(FileSystem.ActiveElement);
            HasUnsavedChanges = true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return newWorldElement;
    }

    public static WorldElement CreateSubElement(Element elementData, Vector3 position)
    {
        var element = CreateSubElement(elementData);
        element.transform.position = position;

        return element;
    }

    // TODO: implement removing world elements
    public static void RemoveSubElement(WorldElement element)
    {
        SubElements.Remove(element);
        FileSystem.UpdateActiveElement();
        PanelName.SetElementData(FileSystem.ActiveElement);
        EditorSelect.Deselect(element);
        GameObject.Destroy(element.gameObject);
    }

    public async void HandleClearSubElementsClicked()
    {
        if (HasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            var result = await DialogYesNo.OpenForResult("Save Changes?", dialogBody);

            if (result == YesNo.Yes)
                HandleSave();
        }

        clearSubElements();
    }

    private void clearSubElements()
    {
        var elementsToDelete = new List<WorldElement>(SubElements);
        elementsToDelete.ForEach(p => RemoveSubElement(p));

        TextNotification.Show("All Sub-Elements Cleared");
    }
}
