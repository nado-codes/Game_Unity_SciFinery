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
    public static Editor Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Editor>();

            return instance;
        }
    }
    public static ElementType DesignType => Instance.designType;
    public static bool HasUnsavedChanges = false;
    // PREFABS
    [Header("Prefabs")]
    public GameObject particlePrefab;
    public GameObject atomPrefab;
    public GameObject moleculePrefab;
    [Header("Other")]
    private Button btnSave;
    // PARTICLES
    public static GameObject elementGameObject;
    public static List<WorldElement> SubElements { get => Instance.subElements; }

    private static Editor instance;
    private ElementType designType = ElementType.Atom;
    private List<WorldElement> subElements = new List<WorldElement>();
    // CAMERA
    private Vector3 cameraStartPos;
    private Quaternion cameraStartAngle;

    void Start()
    {
        if (instance == null)
            instance = this;

        if (particlePrefab == null)
            throw new ArgumentNullException("particlePrefab must be set in Editor");

        SubElements.AddRange(FindObjectsOfType<WorldParticle>());

        var designTypeTabs = GameObject.Find("tabsDesignType").GetComponent<Tabs>();
        designTypeTabs.OnSelectedTabChanged += (int designTypeId) => HandleChangeDesignTypeClicked((ElementType)designTypeId);

        cameraStartPos = Camera.main.transform.position;
        cameraStartAngle = Camera.main.transform.rotation;

        var hudTransform = GameObject.Find("HUD")?.transform;
        var fileButtonsTransform = hudTransform.Find("FileButtons");
        btnSave = fileButtonsTransform?.Find("btnSave")?.GetComponent<Button>();
        Assertions.AssertNotNull(btnSave, "btnSave");

        // NOTE: Start the Editor in an initial state, also setting up the UI
        // with the correct elements and displays
        HandleChangeDesignTypeClicked(ElementType.Atom);
        designTypeTabs.SelectTab((int)ElementType.Atom);
    }
    public static void LoadElement<T>(T element) where T : Element
    {
        if (element == null)
            throw new ArgumentException("Expected elementData in call to Editor.LoadElement, got null");

        Editor.Instance.clearSubElements();
        FileSystem.ActiveElement = element;
        loadElementOfType(element);

        // .. Make atom particles orbit eccentrically on the first load
        if (element.ElementType == ElementType.Atom)
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

        Camera.main.transform.position = SubElements.Select(e => e.transform.position).Average() - (Vector3.forward * 30);
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);

        PanelName.SetElementData(element);
        TextNotification.Show($"Loaded \"{element.Name}\"");
        HasUnsavedChanges = false;
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
    public void HandleDeleteSubElementClicked(WorldElement element)
    {
        EditorSelect.Deselect(element);
        if (!validateDeletion(element.Data))
            return;

        RemoveSubElement(element);
        HasUnsavedChanges = true;
    }
    public static void RemoveSubElement(WorldElement element)
    {
        SubElements.Remove(element);
        FileSystem.UpdateActiveElement();
        PanelName.SetElementData(FileSystem.ActiveElement);

        var orbitCam = Camera.main.GetComponent<OrbitCam>();
        if (orbitCam != null && orbitCam?.TrackedObject == element.transform)
            orbitCam.TrackedObject = null;

        EditorSelect.Deselect(element);
        EditorSelect.RemoveHover(element);

        GameObject.Destroy(element.gameObject);
    }
    private static bool validateDeletion(Element elementToDelete)
    {
        // .. if deleting a nucleic element (Proton/Neutron), and it's an atom, validate
        if (DesignType == ElementType.Atom && elementToDelete.Charge >= 0)
        {
            // .. make sure there's at least one other nucleic element
            var nuclei = SubElements.Where(c => c.Charge >= 0);
            var canDelete = nuclei.Count() > 1;

            if (!canDelete)
                TextNotification.Show("You can't delete the last nuclei!");

            return canDelete;
        }

        return true;
    }
    public async void HandleSave()
    {
        if (!HasUnsavedChanges)
        {
            TextNotification.Show("No changes to save");
            return;
        }

        var saved = await FileSystem.SaveActiveElement(subElements.Select(el => el.Data));
        HasUnsavedChanges = !saved;
    }

    public static async Task CheckUnsaved()
    {
        if (HasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            var dialogResult = await DialogYesNo.OpenForResult("Save Changes?", dialogBody);

            if (dialogResult == YesNo.Yes)
                Instance.HandleSave();
        }
    }
    public async void HandleNewElementClicked()
    {
        await CheckUnsaved();
        clearSubElements();
        createNewElementOfType(Editor.DesignType);
    }
    public async void HandleClearSubElementsClicked()
    {
        await CheckUnsaved();
        clearSubElements();
    }
    public async void HandleChangeDesignTypeClicked(ElementType newDesignType)
    {
        await CheckUnsaved();
        createNewElementOfType(newDesignType);
        PanelCreate.SetDesignType(newDesignType);
        designType = newDesignType;
        TextNotification.Show("Design Type: " + newDesignType);
    }
    private void createNewElementOfType(ElementType type)
    {
        switch (type)
        {
            case ElementType.Atom:
                createNewElementOfType<Atom>();
                break;
            case ElementType.Molecule:
                createNewElementOfType<Molecule>();
                break;
            default:
                throw new NotImplementedException($"Element of type ${type} is not yet implemented in call to HandleChangeDesignTypeClicked");
        }
    }
    private void createNewElementOfType<T>() where T : Element, new()
    {
        var typeName = typeof(T).FullName;
        elementGameObject = GameObject.Find($"{typeName}") ?? new GameObject();
        elementGameObject.name = $"{typeName}New{typeName}";

        var newElement = FileSystem.CreateElementOfType<T>();
        LoadElement(newElement);

        HasUnsavedChanges = true;
    }
    private static void loadElementOfType<T>(T element) where T : Element
    {
        if (element == null)
            throw new NullReferenceException("Expected an element in call to loadElementOfType, got null");

        foreach (Element subElement in element.Children)
        {
            try
            {
                // TODO: later, positions will be able to be saved and re-loaded the next time an element loads
                var radius = subElement.Charge >= 0 ? 1 : 1.1f;
                var randPos = UnityEngine.Random.insideUnitSphere * radius;
                var worldElement = CreateSubElement(subElement, randPos);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                continue;
            }
        }
    }
    private void clearSubElements()
    {
        var elementsToDelete = new List<WorldElement>(SubElements);
        elementsToDelete.ForEach(p => RemoveSubElement(p));
    }
}
