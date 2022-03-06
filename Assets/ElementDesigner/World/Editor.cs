using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum DragState { Init, Active, None }
public enum DesignType { Atom = 0, Molecule = 1, Product = 2 }

public class Editor : MonoBehaviour
{
    private static Editor instance;
    private DesignType designType = DesignType.Atom;

    // PREFABS
    public GameObject protonPrefab, neutronPrefab, electronPrefab;

    // UI
    public Text textClassification, textStability, textCharge;

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
    public static List<Particle> Particles = new List<Particle>();

    public static IEnumerable<Particle> OtherParticles(Particle particle) =>
        Particles.Where(x => x != particle);

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            throw new ApplicationException("There may be only one instance of Editor");

        if (protonPrefab == null)
            throw new ArgumentNullException("protonPrefab must be set in Editor");
        if (neutronPrefab == null)
            throw new ArgumentNullException("neutronPrefab must be set in Editor");
        if (electronPrefab == null)
            throw new ArgumentNullException("electronPrefab must be set in Editor");

        Particles.AddRange(FindObjectsOfType<Particle>());

        textClassification = GameObject.Find("Classification")?.transform.Find("Value").GetComponent<Text>();
        textStability = GameObject.Find("TextStability")?.GetComponent<Text>();
        textCharge = GameObject.Find("TextCharge")?.GetComponent<Text>();

        var designTypeTabs = GameObject.Find("tabsDesignType").GetComponent<Tabs>();
        designTypeTabs.OnSelectedTabChanged += HandleChangeDesignType;

        cameraStartPos = Camera.main.transform.position;
        cameraStartAngle = Camera.main.transform.rotation;

        dragSelectCollider = gameObject.AddComponent<BoxCollider>();
        dragSelectCollider.enabled = false;

        var rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        NewAtom();
    }

    void HandleChangeDesignType(int designTypeTabId)
    {
        if (FileSystem.hasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, FileSystem.SaveAtom, null,
            () => ChangeDesignType((DesignType)designTypeTabId));
        }
        else
            ChangeDesignType((DesignType)designTypeTabId);
    }

    void ChangeDesignType(DesignType newDesignType)
    {
        ClearParticles();
        panelCreate.SetDesignType(newDesignType);

        if (newDesignType == DesignType.Atom)
            NewAtom();
        else if (newDesignType == DesignType.Molecule)
            NewMolecule();
        else if (newDesignType == DesignType.Product)
            NewProduct();

        designType = newDesignType;
        TextNotification.Show("Design Type: " + newDesignType);
    }

    public void HandleNewElementClicked()
    {
        if (FileSystem.hasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, FileSystem.SaveAtom, null,
            () => NewElement());
        }
        else
            NewElement();
    }
    public void NewElement()
    {
        if (designType == DesignType.Atom)
            NewAtom();
        else if (designType == DesignType.Molecule)
            NewMolecule();
        else if (designType == DesignType.Product)
            NewProduct();
    }
    public void NewAtom()
    {
        atomGameObject = GameObject.Find("Atom") ?? new GameObject();
        atomGameObject.name = "AtomNewAtom";

        FileSystem.NewAtom();
        LoadAtomData(FileSystem.ActiveAtom);

        FileSystem.LoadAtoms();
        FileSystem.hasUnsavedChanges = false;
    }

    public void NewMolecule()
    {
        FileSystem.hasUnsavedChanges = false;
    }

    public void NewProduct()
    {
        FileSystem.hasUnsavedChanges = false;
    }

    void Update()
    {
        if (!HUD.LockedFocus)
            UpdateInputs();

        UpdateActiveAtom();

        var chargeRound = Mathf.RoundToInt(FileSystem.ActiveAtom.Charge);
        textCharge.text = $"Charge: {chargeRound} ({FileSystem.ActiveAtom.Charge})";

        if (_selectedObjects.Any())
        {
            var selectionCenter = _selectedObjects.Count() > 1 ?
                _selectedObjects.Aggregate(Vector3.zero, (total, next) => total += next.transform.position * .5f) :
                _selectedObjects.FirstOrDefault().transform.position;

            Translate.Instance.transform.position = selectionCenter;
        }
    }

    void UpdateActiveAtom()
    {
        var protons = Particles.Where(p => p.charge == Particle.Charge.Positive);
        var neutrons = Particles.Where(p => p.charge == Particle.Charge.None);
        var electrons = Particles.Where(p => p.charge == Particle.Charge.Negative);

        FileSystem.ActiveAtom.Number = protons.Count();
        FileSystem.ActiveAtom.ProtonCount = protons.Count();
        FileSystem.ActiveAtom.NeutronCount = neutrons.Count();
        FileSystem.ActiveAtom.ElectronCount = electrons.Count();
        FileSystem.ActiveAtom.Weight = protons.Count() + neutrons.Count();

        //var charges = Particles.Select(p => (int)p.charge);
        //FileSystem.ActiveAtom.Charge = charges.Aggregate((totalCharge,charge) => totalCharge += charge);
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
                RemoveParticle(s.GetComponent<Particle>());
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

        bool canDrag = (
            Vector3.Distance(dragSelectStartWorld.origin, dragSelectEndWorld.origin) > .05f &&
            !Translate.IsActive
        );

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

        Translate.SetActive((objectsToSelect.Count() > 0));

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

    public void LoadAtomData(Atom atomData)
    {
        ClearParticles();
        Camera.main.transform.position = instance.cameraStartPos;
        Camera.main.transform.rotation = instance.cameraStartAngle;

        var particlesToCreate = new List<ParticleType>();

        particlesToCreate.AddRange(Enumerable.Range(0, atomData.ProtonCount).Select(n => ParticleType.Proton));
        particlesToCreate.AddRange(Enumerable.Range(0, atomData.NeutronCount).Select(n => ParticleType.Neutron));
        particlesToCreate.AddRange(Enumerable.Range(0, atomData.ElectronCount).Select(n => ParticleType.Electron));

        // .. create each particle in a random position, with electrons further away than protons and neutrons
        particlesToCreate.ForEach(particleToCreate =>
        {
            var radius = particleToCreate != ParticleType.Electron ? 1 : 20;
            var randPos = UnityEngine.Random.insideUnitSphere * radius;

            CreateParticle(particleToCreate, randPos);
        });

        TextNotification.Show($"Loaded \"{FileSystem.ActiveAtom.Name}\"");
    }

    public static Particle CreateParticle(ParticleType type)
    {
        GameObject particleGameObject = null;

        if (type == ParticleType.Proton)
            particleGameObject = Instantiate(instance.protonPrefab);
        else if (type == ParticleType.Neutron)
            particleGameObject = Instantiate(instance.neutronPrefab);
        else
            particleGameObject = Instantiate(instance.electronPrefab);

        var newParticle = particleGameObject.GetComponent<Particle>();
        newParticle.transform.parent = atomGameObject.transform;
        FileSystem.ActiveAtom.Charge += (int)newParticle.charge;

        if (newParticle.type != type)
            Debug.LogWarning($"Failed to create a particle of type {type}. Created an electron by default");

        Particles.Add(newParticle);

        FileSystem.hasUnsavedChanges = true;

        return newParticle;
    }

    public Particle CreateParticle(ParticleType type, Vector3 position)
    {
        var particle = CreateParticle(type);
        particle.transform.position = position;

        return particle;
    }

    public static bool RemoveParticle(Particle particle)
    {
        Particles.Remove(particle);
        GameObject.Destroy(particle.gameObject);

        FileSystem.ActiveAtom.Charge -= (int)particle.charge;

        return true;
    }

    public static void RemoveParticles(IEnumerable<Particle> particlesToRemove)
        => particlesToRemove.Select(p => RemoveParticle(p));

    public static void RemoveParticles(IEnumerable<Interact> particlesToRemove)
        => particlesToRemove.Select(p => RemoveParticle(p.GetComponent<Particle>()));

    public static void HandleClearParticlesClicked()
    {
        if (FileSystem.hasUnsavedChanges)
        {
            var dialogBody = "You have unsaved changes in the editor. Would you like to save before continuing?";
            DialogYesNo.Open("Save Changes?", dialogBody, FileSystem.SaveAtom, null,
            () => ClearParticles());
        }
        else
            ClearParticles();
    }

    public static void ClearParticles()
    {
        var particlesToDelete = new List<Particle>(Particles);
        particlesToDelete.ForEach(p => RemoveParticle(p));

        TextNotification.Show("All Particles Cleared");
    }
}
