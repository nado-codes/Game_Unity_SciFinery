using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum DragState { Init, Active, None }
public enum ElementType { Particle = 0, Atom = 1, Molecule = 2, Product = 3 }

public class Editor : MonoBehaviour
{
    private static Editor instance;

    private ElementType designType = ElementType.Atom;
    public static ElementType DesignType => instance.designType;
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

    // SELECTED OBJECTS
    private static List<Interact> _selectedObjects = new List<Interact>();
    public static IEnumerable<Interact> SelectedObjects => _selectedObjects;
    private static List<Interact> hoveredObjects = new List<Interact>();

    // DRAG SELECT
    public RectTransform selectionBoxRect;
    private static bool dragSelectIsEnabled = true;
    private DragState dragState = DragState.None;
    private BoxCollider dragSelectCollider;
    private Vector2 dragSelectStartPosition, endDragSelectPosition;
    private Ray dragSelectStartWorld, dragSelectEndWorld;

    // PARTICLES
    public static GameObject atomGameObject;
    public static List<WorldParticle> Particles = new List<WorldParticle>();

    public static IEnumerable<WorldParticle> OtherParticles(WorldParticle particle) =>
        Particles.Where(x => x != particle);

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            throw new ApplicationException("There may be only one instance of Editor");

        if (particlePrefab == null)
            throw new ArgumentNullException("particlePrefab must be set in Editor");

        Particles.AddRange(FindObjectsOfType<WorldParticle>());

        textClassification = GameObject.Find("Classification")?.transform.Find("Value").GetComponent<Text>();
        textStability = GameObject.Find("TextStability")?.GetComponent<Text>();
        textCharge = GameObject.Find("TextCharge")?.GetComponent<Text>();

        var designTypeTabs = GameObject.Find("tabsDesignType").GetComponent<Tabs>();
        designTypeTabs.OnSelectedTabChanged += (int designTypeId) => HandleChangeDesignTypeClicked((ElementType)designTypeId);

        cameraStartPos = Camera.main.transform.position;
        cameraStartAngle = Camera.main.transform.rotation;

        dragSelectCollider = gameObject.AddComponent<BoxCollider>();
        dragSelectCollider.enabled = false;

        var rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        var allAtomsFileNames = Directory.GetFiles("./Elements/Atom/");

        foreach (string atomFileName in allAtomsFileNames)
        {
            var atomJSON = File.ReadAllText(atomFileName);
            var atom = JsonUtility.FromJson<Atom>(atomJSON);

            Color protonColor, neutronColor, electronColor;
            ColorUtility.TryParseHtmlString("#00E0FF", out protonColor);
            ColorUtility.TryParseHtmlString("#66FF33", out neutronColor);
            ColorUtility.TryParseHtmlString("#FF0000", out electronColor);

            var protonsToAdd = Enumerable.Range(0, atom.ProtonCount).Select(i => 1);
            var neutronsToAdd = Enumerable.Range(0, atom.NeutronCount).Select(i => 2);
            var electronsToAdd = Enumerable.Range(0, atom.ElectronCount).Select(i => 3);

            atom.ParticleIds = protonsToAdd.Concat(neutronsToAdd.Concat(electronsToAdd)).ToArray();

            var newAtomJSON = JsonUtility.ToJson(atom);
            File.WriteAllText(atomFileName, newAtomJSON);
        }

        // NOTE: Start the Editor in an initial state, also setting up the UI
        // with the correct elements and displays
        HandleChangeDesignTypeClicked(ElementType.Atom);
    }

    public void HandleChangeDesignTypeClicked(ElementType newDesignType)
    {
        if (newDesignType == ElementType.Atom)
            handleChangeDesignType<Atom>();

        panelCreate.SetDesignType(newDesignType);
        designType = newDesignType;
        TextNotification.Show("Design Type: " + newDesignType);
    }

    private void handleChangeDesignType<T>() where T : Element
    {
        if (HasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, () => FileSystem.instance.SaveActiveElement(), null,
            () =>
            {
                ClearParticles();
                createNewElementOfType<T>();
            });
        }
        else
        {
            ClearParticles();
            createNewElementOfType<T>();
        }
    }

    public void HandleNewElementClicked()
    {
        if (DesignType == ElementType.Atom)
            handleCreateNewElementOfType<Atom>();
        else
            throw new NotImplementedException($"Design type {DesignType} is not yet implemented in call to Editor.HandleNewElementClicked");
    }
    private void handleCreateNewElementOfType<T>() where T : Element
    {
        if (HasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, () => FileSystem.instance.SaveActiveElement(), null,
            createNewElementOfType<T>);
        }
        else
            createNewElementOfType<T>();
    }
    private void createNewElementOfType<T>() where T : Element
    {
        var typeName = typeof(T).FullName;
        atomGameObject = GameObject.Find($"{typeName}") ?? new GameObject();
        atomGameObject.name = $"{typeName}New{typeName}";

        FileSystem.CreateNewElementOfType<T>();
        LoadElementData(FileSystem.instance.ActiveElement);

        HasUnsavedChanges = false;
    }

    void Update()
    {
        if (!HUD.LockedFocus)
            UpdateInputs();

        UpdateActiveAtom();

        // var chargeRound = Mathf.RoundToInt(FileSystem.instance.ActiveElementAs<Atom>().Charge);
        // textCharge.text = $"Charge: {chargeRound} ({FileSystem.instance.ActiveElementAs<Atom>().Charge})";

        if (_selectedObjects.Any())
        {
            var selectionCenter = _selectedObjects.Count() > 1 ?
                _selectedObjects.Aggregate(Vector3.zero, (total, next) => total += next.transform.position * .5f) :
                _selectedObjects.FirstOrDefault().transform.position;
        }
    }

    void UpdateActiveAtom()
    {
        // var protons = Particles.Where(p => p.charge == WorldParticle.Charge.Positive);
        // var neutrons = Particles.Where(p => p.charge == WorldParticle.Charge.None);
        // var electrons = Particles.Where(p => p.charge == WorldParticle.Charge.Negative);

        // FileSystem.instance.ActiveElementAs<Atom>().Number = protons.Count();
        // FileSystem.instance.ActiveElementAs<Atom>().ProtonCount = protons.Count();
        // FileSystem.instance.ActiveElementAs<Atom>().NeutronCount = neutrons.Count();
        // FileSystem.instance.ActiveElementAs<Atom>().ElectronCount = electrons.Count();
        // FileSystem.instance.ActiveElementAs<Atom>().Weight = protons.Count() + neutrons.Count();

        //var charges = Particles.Select(p => (int)p.charge);
        //FileSystem.instance.ActiveAtom.Charge = charges.Aggregate((totalCharge,charge) => totalCharge += charge);
    }

    void UpdateInputs()
    {
        if (dragSelectIsEnabled)
            HandleDragSelect();

        if (Input.GetMouseButtonUp(0))
        {
            Select(hoveredObjects.Where(sel => sel.Selectable));
            hoveredObjects.Clear();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            _selectedObjects.ForEach(s =>
            {
                //GameObject.Destroy(s.gameObject);
                RemoveParticle(s.GetComponent<WorldParticle>());
            });
            _selectedObjects.Clear();
            //RemoveParticles(_selectedObjects);
        }
    }

    public static void SetDragSelectEnabled(bool enable) => dragSelectIsEnabled = enable;

    void HandleDragSelect()
    {
        if (Input.GetMouseButtonDown(0))
            InitDragSelect();

        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragSelectEndWorld = cameraRay;

        bool canDrag = Vector3.Distance(dragSelectStartWorld.origin, dragSelectEndWorld.origin) > .05f;

        if (canDrag && dragState == DragState.Init)
            StartDragSelect();

        if (Input.GetMouseButton(0) && dragState == DragState.Active)
        {
            // .. collider
            var hudRectTransform = GameObject.Find("HUD").GetComponent<RectTransform>();

            var dragOffset = ((hudRectTransform.position - transform.position) * .9f).magnitude;
            var dragMin = dragSelectStartWorld.origin + (dragSelectStartWorld.direction * dragOffset);
            var dragMinLocal = transform.InverseTransformPoint(dragMin);
            var dragMax = dragSelectEndWorld.origin + (dragSelectEndWorld.direction * dragOffset); ;
            var dragMaxLocal = transform.InverseTransformPoint(dragMax);
            var dragCenter = dragMin + (dragMax - dragMin) * .5f;
            var dragCenterLocal = transform.InverseTransformPoint(dragCenter);

            Debug.DrawRay(dragMin, transform.forward, Color.green, .01f);
            Debug.DrawRay(dragMax, transform.forward, Color.red, .01f);
            Debug.DrawRay(dragCenter, transform.forward, Color.blue, .01f);

            var colliderSizeBase = (dragMaxLocal - dragMinLocal);
            dragSelectCollider.size = new Vector3(colliderSizeBase.x * 2, colliderSizeBase.y * 2, 20);
            dragSelectCollider.center = dragCenterLocal + (Vector3.forward * dragSelectCollider.size.z * .5f);

            // .. HUD selection box 
            float selectionBoxWidth = Input.mousePosition.x - dragSelectStartPosition.x;
            float selectionBoxHeight = Input.mousePosition.y - dragSelectStartPosition.y;
            selectionBoxRect.anchoredPosition = dragSelectStartPosition - new Vector2(-selectionBoxWidth / 2, -selectionBoxHeight / 2);
            selectionBoxRect.sizeDelta = new Vector2(Mathf.Abs(selectionBoxWidth), Mathf.Abs(selectionBoxHeight));
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (dragState == DragState.Active)
                FinishDragSelect();

            dragState = DragState.None;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        var interact = col.gameObject.GetComponent<Interact>();

        if (interact && interact.Selectable && !hoveredObjects.Contains(interact))
        {
            interact.Hover();

            hoveredObjects.Add(interact);
        }
    }

    void OnTriggerExit(Collider col)
    {
        var interact = col.gameObject.GetComponent<Interact>();

        if (interact)
        {
            interact.ClearHover();
            hoveredObjects.Remove(interact);
        }
    }

    void InitDragSelect()
    {
        dragState = DragState.Init;
        dragSelectStartWorld = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragSelectStartPosition = Input.mousePosition;
    }

    void StartDragSelect()
    {
        selectionBoxRect.gameObject.SetActive(true);
        selectionBoxRect.sizeDelta = Vector2.zero;
        dragSelectCollider.enabled = true;
        dragSelectCollider.isTrigger = true;

        dragState = DragState.Active;
    }

    void FinishDragSelect()
    {
        endDragSelectPosition = Input.mousePosition;
        selectionBoxRect.gameObject.SetActive(false);
        dragSelectCollider.enabled = false;
        dragSelectCollider.isTrigger = false;
    }

    public static void Hover(Interact objectToHover)
    {
        if (!hoveredObjects.Contains(objectToHover))
            hoveredObjects.Add(objectToHover);
    }

    public static void RemoveHover(Interact objectToDehover)
    {
        hoveredObjects.Remove(objectToDehover);
    }

    public static void Select(Interact objectToSelect) => Select(new Interact[1] { objectToSelect });

    public static void Select(IEnumerable<Interact> objectsToSelect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);
        var isMultiDeselect = Input.GetKey(KeyCode.LeftControl);

        if (!isMultiSelect && !isMultiDeselect)
        {
            var objectsToDeselect = _selectedObjects.Where(s => !objectsToSelect.Contains(s)).ToList();
            objectsToDeselect.ForEach(s => s.Deselect());
            _selectedObjects.RemoveAll(s => objectsToDeselect.Contains(s));
        }

        if (objectsToSelect.Count() == 0) return;

        if (!isMultiDeselect)
        {
            objectsToSelect.ToList().ForEach(objectToSelect => objectToSelect.Select());

            _selectedObjects.AddRange(objectsToSelect.Where(objectToSelect =>
            {
                if (!_selectedObjects.Contains(objectToSelect))
                    return true;

                return false;
            }));
        }
        else
        {
            objectsToSelect.ToList().ForEach(objectToSelect => objectToSelect.Deselect());

            objectsToSelect.ToList().ForEach(objectToDeselect =>
            {

                objectToDeselect.Deselect();
                _selectedObjects.Remove(objectToDeselect);
            });
        }
    }

    public static void Deselect(Interact objectToDeselect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);

        if (!isMultiSelect)
        {
            objectToDeselect.Deselect();
            _selectedObjects.Remove(objectToDeselect);
        }
    }

    public static void LoadElementData<T>(T elementData) where T : Element
    {
        ClearParticles();
        // Camera.main.transform.position = instance.cameraStartPos;
        // Camera.main.transform.rotation = instance.cameraStartAngle;
        var elementType = elementData.Type;
        if (elementType == ElementType.Atom)
        {
            var particlesToCreate = new List<ParticleType>();

            // TODO: get a list of particles to create by reading the particleIds from elementData

            // particlesToCreate.AddRange(Enumerable.Range(0, elementData.ProtonCount).Select(n => ParticleType.Proton));
            // particlesToCreate.AddRange(Enumerable.Range(0, elementData.NeutronCount).Select(n => ParticleType.Neutron));
            // particlesToCreate.AddRange(Enumerable.Range(0, elementData.ElectronCount).Select(n => ParticleType.Electron));

            // .. create each particle in a random position, with electrons further away than protons and neutrons
            particlesToCreate.ForEach(particleToCreate =>
            {
                var radius = particleToCreate != ParticleType.Electron ? 1 : 20;
                var randPos = UnityEngine.Random.insideUnitSphere * radius;

                instance.CreateParticle(particleToCreate, randPos);
            });
        }



        FileSystem.instance.ActiveElement = elementData;
        // TextNotification.Show($"Loaded \"{FileSystem<T>.instance.ActiveElementAs<Atom>().Name}\"");
    }

    public static WorldElement CreateWorldElement(Element elementData)
    {
        if (elementData == null)
            throw new ApplicationException("elementData cannot be null in call to CreateWorldElement");

        // TODO: later, prefabs for particles, atoms and molecules will be loaded in at runtime using
        // Unity "Addressables" (like AssetBundles)

        WorldElement newWorldElement = null;
        GameObject newWorldElementGO = null;
        var elementType = elementData.Type;

        if (elementType == ElementType.Particle)
        {
            newWorldElementGO = Instantiate(instance.particlePrefab);
            newWorldElementGO.transform.parent = atomGameObject.transform;

            var elementDataAsParticle = elementData as Particle;
            var newWorldParticle = newWorldElementGO.GetComponent<WorldParticle>();
            newWorldParticle.SetParticleData(elementDataAsParticle);

            newWorldElement = newWorldParticle;
            Particles.Add(newWorldParticle);
        }
        else if (elementType == ElementType.Atom)
        {

        }
        else if (elementType == ElementType.Molecule)
        {

        }

        HasUnsavedChanges = true;

        return newWorldElement;
    }

    public static WorldParticle CreateParticle(ParticleType type)
    {
        // TODO: later, prefabs for particles, atoms and molecules will be loaded in at runtime using
        // Unity "Addressables" (like AssetBundles)
        GameObject particleGameObject = null;

        /* if (type == ParticleType.Proton)
            particleGameObject = Instantiate(instance.protonPrefab);
        else if (type == ParticleType.Neutron)
            particleGameObject = Instantiate(instance.neutronPrefab);
        else
            particleGameObject = Instantiate(instance.electronPrefab); */

        var newParticle = particleGameObject.GetComponent<WorldParticle>();
        newParticle.transform.parent = atomGameObject.transform;
        // FileSystem.instance.ActiveElementAs<Atom>().Charge += (int)newParticle.Charge;

        Particles.Add(newParticle);

        HasUnsavedChanges = true;

        return newParticle;
    }

    public WorldParticle CreateParticle(ParticleType type, Vector3 position)
    {
        var particle = CreateParticle(type);
        particle.transform.position = position;

        return particle;
    }

    public static bool RemoveParticle(WorldParticle particle)
    {
        Particles.Remove(particle);
        GameObject.Destroy(particle.gameObject);

        // FileSystem.instance.ActiveElementAs<Atom>().Charge -= (int)particle.Charge;

        return true;
    }

    public static void RemoveParticles(IEnumerable<WorldParticle> particlesToRemove)
        => particlesToRemove.Select(p => RemoveParticle(p));

    public static void RemoveParticles(IEnumerable<Interact> particlesToRemove)
        => particlesToRemove.Select(p => RemoveParticle(p.GetComponent<WorldParticle>()));

    public void HandleClearElementsClicked()
    {
        if (designType == ElementType.Atom)
            handleClearElements<Particle>();
    }

    private void handleClearElements<T>() where T : Element
    {
        if (HasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, () => FileSystem.instance.SaveActiveElement(), null,
            clearElements<T>);
        }
        else
            clearElements<T>();
    }

    private void clearElements<T>() where T : Element
    {
        // TODO: implement clearing elements, use if statements here to clear based on design type
        // We'll need a bunch of lists to store the different types
    }

    public static void ClearParticles()
    {
        var particlesToDelete = new List<WorldParticle>(Particles);
        particlesToDelete.ForEach(p => RemoveParticle(p));

        TextNotification.Show("All Particles Cleared");
    }
}
