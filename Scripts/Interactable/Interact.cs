using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [Header("Interact setup")]
    [SerializeField] private Camera playerCam;
    [SerializeField] private GameObject interactContainer;
    [SerializeField] private float InteractableDistance = 5f;
    [SerializeField] private float LookThreshold = 0.97f;
    [SerializeField] private LayerMask interactLayers;
    [SerializeField] private LayerMask groundLayer;

    [Header("Highlighted object")]
    [SerializeField] GameObject selectedObj;
    [SerializeField] string selectedType;

    GameDataFile GDFile;
    InputController IC;
    InteractText IT;

    private void Awake()
    {
        GDFile = FindObjectOfType<GameDataFile>();
        IC = GDFile.GetComponent<InputController>();
        IT = GetComponent<InteractText>();
        SubEvents();
    }
    void SubEvents()
    {
        IC.action += StartInteract;
        IC.item1 += StartActivate;
    }
    // Update is called once per frame
    void Update()
    {
        findObjects();
    }

    void findObjects()
    {
        float ObjPercentage = 0f;
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, InteractableDistance, groundLayer + interactLayers))
        {
            Collider[] hitObjects = Physics.OverlapSphere(hit.point, 1f, interactLayers);
            GameObject closestObj = null;
            foreach (Collider obj in hitObjects)
            {
                if (obj.transform.gameObject.layer != 8) //remove any ground layer objects
                {
                    Vector3 vector1 = ray.direction;
                    Vector3 vector2 = obj.transform.position - ray.origin;

                    float lookPercentage = Vector3.Dot(vector1.normalized, vector2.normalized);
                    if(lookPercentage > LookThreshold && lookPercentage > ObjPercentage)
                    {
                        ObjPercentage = lookPercentage;
                        closestObj = obj.gameObject;
                    }
                }
            }
            SetInteractable(closestObj);
        }
        else reset();
    }
    /// <summary>
    /// sets the selected interactable to the given gameobject.
    /// </summary>
    /// <param name="interactable"></param>
    void SetInteractable(GameObject interactable)
    {
        try
        {
            IInteractFunction objScript = interactable.GetComponent<IInteractFunction>();
            selectedType = objScript.InteractType();
            selectedObj = interactable;
            DisplayText(objScript);
        }
        catch { reset(); }
    }
    void reset()
    {
        selectedType = null;
        selectedObj = null;
        if (IT) IT.ToggleText(false);
    }
    void StartInteract()
    {
        if (!selectedObj) return;
        selectedObj.GetComponent<IInteractFunction>().Interact(); 
    }
    void StartActivate()
    {
        if (!selectedObj) return; 
        selectedObj.GetComponent<IActivateFunction>().Activate(); 
    }
    /// <summary>
    /// Gets the display text for the given interactable and calls the InteractText script to display it
    /// </summary>
    /// <param name="objScript"></param>
    void DisplayText(IInteractFunction objScript)
    {
        if (!IT) return;

        //Set screen text
        string symbol = IT.getSymbol(inputName.P_Action);
        string intText = objScript.InteractText();
        IT.SetText(symbol + " " + intText);
        IT.ToggleText(true);
    }
}
