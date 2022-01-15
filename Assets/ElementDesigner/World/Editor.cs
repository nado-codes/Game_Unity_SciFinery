using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum DragState{ Init, Active, None }

public class Editor : MonoBehaviour
{
    private static Editor instance;
    public RectTransform selectionBoxRect;
    public GameObject protonPrefab, neutronPrefab, electronPrefab;

    public Text textClassification, textStability, textCharge;

    private Vector3 cameraStartPos;
    private Quaternion cameraStartAngle;
    private static List<Interact> _selectedObjects = new List<Interact>();
    public static IEnumerable<Interact> SelectedObjects => _selectedObjects;
    private static List<Interact> hoveredObjects = new List<Interact>();
    private static bool dragSelectIsEnabled = true;
    private DragState dragState = DragState.None;
    
    private BoxCollider dragSelectCollider;
    private Vector2 dragSelectStartPosition, endDragSelectPosition;
    private Ray dragSelectStartWorld, dragSelectEndWorld;

    // PARTICLES
    private GameObject atomParent;
    public static List<Particle> Particles = new List<Particle>();

    public static IEnumerable<Particle> OtherParticles(Particle particle) =>
        Particles.Where(x => x != particle);

    void Start()
    {
        if(instance == null)
            instance = this;
        else
            throw new ApplicationException("There may be only one instance of Editor");

        if(protonPrefab == null)
            throw new ArgumentNullException("protonPrefab must be set in Editor");
        if(neutronPrefab == null)
            throw new ArgumentNullException("neutronPrefab must be set in Editor");
        if(electronPrefab == null)
            throw new ArgumentNullException("electronPrefab must be set in Editor");

        Particles.AddRange(FindObjectsOfType<Particle>());

        textClassification = GameObject.Find("Classification")?.transform.Find("Value").GetComponent<Text>();
        textStability = GameObject.Find("TextStability")?.GetComponent<Text>();
        textCharge = GameObject.Find("TextCharge")?.GetComponent<Text>();

        cameraStartPos = Camera.main.transform.position;
        cameraStartAngle = Camera.main.transform.rotation;

        dragSelectCollider = gameObject.AddComponent<BoxCollider>();
        dragSelectCollider.enabled = false;

        var rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        atomParent = GameObject.Find("Atom") ?? new GameObject();
        atomParent.name = "AtomNewAtom";

        FileSystem.NewActiveAtom();
        LoadAtomData(FileSystem.ActiveAtom);

        FileSystem.LoadAtoms();
        
    }

    void Update()
    {  
        if(!HUD.LockedFocus)
            UpdateInputs();
        
        UpdateActiveAtom();

        var chargeRound = Mathf.RoundToInt(FileSystem.ActiveAtom.Charge);
        textCharge.text = $"Charge: {chargeRound} ({FileSystem.ActiveAtom.Charge})";

        if(_selectedObjects.Any())
        {
            var selectionCenter = _selectedObjects.Count() > 1 ? 
                _selectedObjects.Aggregate(Vector3.zero,(total,next) => total += next.transform.position*.5f) :
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
        FileSystem.ActiveAtom.Weight = protons.Count()+neutrons.Count();

        //var charges = Particles.Select(p => (int)p.charge);
        //FileSystem.ActiveAtom.Charge = charges.Aggregate((totalCharge,charge) => totalCharge += charge);
    }

    void UpdateInputs()
    {
        if(dragSelectIsEnabled)
            HandleDragSelect();

        if(Input.GetMouseButtonUp(0))
        {
            Select(hoveredObjects.Where(sel => sel.Selectable));
            hoveredObjects.Clear();
        }

        if(Input.GetKeyDown(KeyCode.Delete))
        {
            _selectedObjects.ForEach(s => {
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
        if(Input.GetMouseButtonDown(0))
            InitDragSelect();

        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragSelectEndWorld = cameraRay;

        bool canDrag = (
            Vector3.Distance(dragSelectStartWorld.origin,dragSelectEndWorld.origin) > .05f &&
            !Translate.IsActive
        );

        if(canDrag && dragState == DragState.Init)
            StartDragSelect();

        if(Input.GetMouseButton(0) && dragState == DragState.Active)
        {
            // .. collider
            var hudRectTransform = GameObject.Find("HUD").GetComponent<RectTransform>();

            var dragOffset = ((hudRectTransform.position-transform.position)*.9f).magnitude;
            var dragMin = dragSelectStartWorld.origin + (dragSelectStartWorld.direction*dragOffset);
            var dragMinLocal = transform.InverseTransformPoint(dragMin);
            var dragMax = dragSelectEndWorld.origin + (dragSelectEndWorld.direction*dragOffset);;
            var dragMaxLocal = transform.InverseTransformPoint(dragMax);
            var dragCenter = dragMin+(dragMax-dragMin)*.5f;
            var dragCenterLocal = transform.InverseTransformPoint(dragCenter);

            Debug.DrawRay(dragMin,transform.forward,Color.green,.01f);
            Debug.DrawRay(dragMax,transform.forward,Color.red,.01f);
            Debug.DrawRay(dragCenter,transform.forward,Color.blue,.01f);

            var colliderSizeBase = (dragMaxLocal-dragMinLocal);
            dragSelectCollider.size = new Vector3(colliderSizeBase.x*2,colliderSizeBase.y*2,20);
            dragSelectCollider.center = dragCenterLocal + (Vector3.forward*dragSelectCollider.size.z*.5f);

            // .. HUD selection box 
            float selectionBoxWidth = Input.mousePosition.x - dragSelectStartPosition.x;
            float selectionBoxHeight = Input.mousePosition.y - dragSelectStartPosition.y;
            selectionBoxRect.anchoredPosition = dragSelectStartPosition - new Vector2(-selectionBoxWidth / 2, -selectionBoxHeight / 2);
            selectionBoxRect.sizeDelta = new Vector2(Mathf.Abs(selectionBoxWidth),Mathf.Abs(selectionBoxHeight));
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(dragState == DragState.Active)
                FinishDragSelect();

            dragState = DragState.None;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        var interact = col.gameObject.GetComponent<Interact>();
    
        if(interact && interact.Selectable && !hoveredObjects.Contains(interact))
        {
            interact.Hover();

            Debug.Log("adding "+interact.gameObject.name+" to selection");
            hoveredObjects.Add(interact);
        }
    }

    void OnTriggerExit(Collider col)
    {
        var interact = col.gameObject.GetComponent<Interact>();

        if(interact)
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
        Debug.Log("init drag select");
    }

    void StartDragSelect()
    {
        selectionBoxRect.gameObject.SetActive(true);
        selectionBoxRect.sizeDelta = Vector2.zero;
        dragSelectCollider.enabled = true;
        dragSelectCollider.isTrigger = true;

        dragState = DragState.Active;
        Debug.Log("start drag select");
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
        if(!hoveredObjects.Contains(objectToHover))
            hoveredObjects.Add(objectToHover);
    }

    public static void RemoveHover(Interact objectToDehover)
    {
        hoveredObjects.Remove(objectToDehover);
    }

    public static void Select(Interact objectToSelect) => Select(new Interact[1]{objectToSelect});

    public static void Select(IEnumerable<Interact> objectsToSelect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);
        var isMultiDeselect = Input.GetKey(KeyCode.LeftControl);

        // Debug.Log("there are "+selectedObjects.Count+" objects currently selected");
        Debug.Log("..."+objectsToSelect.Count()+" more objects will be selected/deselected");        

        if(!isMultiSelect && !isMultiDeselect)
        {
            var objectsToDeselect = _selectedObjects.Where(s => !objectsToSelect.Contains(s)).ToList();
            // if(objectsToDeselect.Count > 0) Debug.Log("cleared "+objectsToDeselect.Count+" other objects from selection");
            objectsToDeselect.ForEach(s => s.Deselect());
            _selectedObjects.RemoveAll(s => objectsToDeselect.Contains(s));
        }

        Translate.SetActive((objectsToSelect.Count() > 0));

        if(objectsToSelect.Count() == 0) return;

        if(!isMultiDeselect)
        {
            objectsToSelect.ToList().ForEach(objectToSelect => objectToSelect.Select());

            _selectedObjects.AddRange(objectsToSelect.Where(objectToSelect => {
                if(!_selectedObjects.Contains(objectToSelect))
                {
                    Debug.Log("trying to add"+objectToSelect);
                    Debug.Log("added "+objectToSelect.gameObject.name+" to selection");
                    return true;
                }
                return false;
            }));
        }
        else
        {
            objectsToSelect.ToList().ForEach(objectToSelect => objectToSelect.Deselect());

            objectsToSelect.ToList().ForEach(objectToDeselect => {
                if(_selectedObjects.Contains(objectToDeselect))
                    Debug.Log("removed "+objectToDeselect.gameObject.name+" from selection");

                objectToDeselect.Deselect();
                _selectedObjects.Remove(objectToDeselect);
            });
        }
    }

    public static void Deselect(Interact objectToDeselect)
    {
        var isMultiSelect = Input.GetKey(KeyCode.LeftShift);

        if(!isMultiSelect)
        {
            Debug.Log("removed "+objectToDeselect.gameObject.name+" from selection");
            objectToDeselect.Deselect();
            _selectedObjects.Remove(objectToDeselect);
        }
    }

    public static void LoadAtomData(Atom atomData)
    {
        ClearParticles();
        Camera.main.transform.position = instance.cameraStartPos;
        Camera.main.transform.rotation = instance.cameraStartAngle;
        
        var particlesToCreate = new List<ParticleType>();

        particlesToCreate.AddRange(Enumerable.Range(0,atomData.ProtonCount).Select(n => ParticleType.Proton));
        particlesToCreate.AddRange(Enumerable.Range(0,atomData.NeutronCount).Select(n => ParticleType.Neutron));
        particlesToCreate.AddRange(Enumerable.Range(0,atomData.ElectronCount).Select(n => ParticleType.Electron));

        Debug.Log("particlesToCreate=");
        particlesToCreate.ForEach(p => Debug.Log(" - "+p));

        // .. create each particle in a random position, with electrons further away than protons and neutrons
        particlesToCreate.ForEach(particleToCreate => {
            var radius = particleToCreate != ParticleType.Electron ? 1 : 20;
            var randPos = UnityEngine.Random.insideUnitSphere*radius;

            CreateParticle(particleToCreate,randPos);
        });
    }

    public static Particle CreateParticle(ParticleType type)
    {
        GameObject particleGameObject = null;

        if(type == ParticleType.Proton)
            particleGameObject = Instantiate(instance.protonPrefab);
        else if(type == ParticleType.Neutron)
            particleGameObject = Instantiate(instance.neutronPrefab);
        else
            particleGameObject = Instantiate(instance.electronPrefab);

        var newParticle = particleGameObject.GetComponent<Particle>();
        newParticle.transform.parent = instance.atomParent.transform;

        if(newParticle.type != type)
            Debug.LogWarning($"Failed to create a particle of type {type}. Created an electron by default");

        Particles.Add(newParticle);

        Debug.Log("particles is now ");
        Particles.ForEach(p => Debug.Log("- "+p.name));

        return newParticle;
    }

    public static Particle CreateParticle(ParticleType type, Vector3 position)
    {
        var particle = CreateParticle(type);
        particle.transform.position = position;

        

        return particle;
    }

    public static bool RemoveParticle(Particle particle)
    {
        Debug.Log("particles was ");
        Particles.ForEach(p => Debug.Log("- "+p.name));

        Particles.Remove(particle);
        GameObject.Destroy(particle.gameObject);
        Debug.Log($"removing {particle.name}");

        Debug.Log("particles is now ");
        Particles.ForEach(p => Debug.Log("- "+p.name));

        return true;
    }

    public static void RemoveParticles(IEnumerable<Particle> particlesToRemove)
        => particlesToRemove.Select(p => RemoveParticle(p));

    public static void RemoveParticles(IEnumerable<Interact> particlesToRemove)
        => particlesToRemove.Select(p => RemoveParticle(p.GetComponent<Particle>()));

    public static void ClearParticles()
    {
        var particlesToDelete = new List<Particle>(Particles);
        particlesToDelete.ForEach(p => RemoveParticle(p));
        Particles.Clear();
    }
}
