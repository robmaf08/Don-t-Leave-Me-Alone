using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using TMPro;

public class InspectManager : MonoBehaviour
{

    //Item information
    Vector3 originalPos;
    Quaternion originalRot;
    Vector3 originalScale;
    bool onInspect = false;
    GameObject inspected;
    private Transform parent;

    [Header("Components for inspecting")]
    public GameObject crosshair;
    public Transform playerSocket;
    public Animator playerAnimator;
    public float distance;
    public ThirdPersonController playerScript;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera inspectionCamera;
    [SerializeField] private GameObject inspectionUI;

    [Header("Components for item description")]
    [SerializeField] private GameObject nameItemUI;
    [SerializeField] private TextMeshProUGUI nameItemText;
    [SerializeField] private TextMeshProUGUI descriptionItemText;

    [Header("Components for note item description")]
    [SerializeField] private GameObject inspectionNoteUI;
    [SerializeField] private TextMeshProUGUI descriptionNoteText;
    [SerializeField] private GameObject examineButton;
    [SerializeField] private GameObject backButton;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioItemNameAppears;

    private bool canInspect = true;
    private bool hasParent = false;
    private Rigidbody itemRigibody;
    private StarterAssets.StarterAssetsInputs _input;

    private void Start()
    {
        onInspect = false;
        _input = StarterAssetsInputs.Instance;
        nameItemUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (onInspect)
        {
            playerAnimator.enabled = false;
            inspected.transform.position = Vector3.Lerp(inspected.transform.position, playerSocket.position, 2.5f * Time.deltaTime);
            if (canInspect)
            {
                //CheckIfNote();
                if (itemRigibody != null) 
                {
                    itemRigibody.isKinematic = true;
                    itemRigibody.detectCollisions = false;
                }
                CheckIfRotating();
                CheckIfZoom();
            }
        }
        else if (inspected != null && !onInspect)
        {
            inspected.transform.parent = null;
            inspected.transform.position = Vector3.Lerp(inspected.transform.position, originalPos, 8f * Time.deltaTime);
            inspected.transform.rotation = Quaternion.Lerp(inspected.transform.rotation, originalRot, 8f * Time.deltaTime);
            inspected.transform.localScale = Vector3.Lerp(inspected.transform.localScale, originalScale, 8f * Time.deltaTime);
        }

        //Check if close inspecting
        if (onInspect)
        {
            if (_input.backButton)
            {
                StateMachine.Instance.ShowCursor(false);
                StartCoroutine(DropItem());
                onInspect = false;
                _input.Reset();
            }
        }
       

    }

    private void SetInspectedObject(GameObject hit)
    {
        if  (inspected != null)
        {
            inspected.gameObject.tag = "InteractableObject";
            inspected = null;
            itemRigibody = null;
        }
        
        if (hit.GetComponent<InteractiveItem>().isChildren)
        {
            inspected = hit.transform.parent.gameObject;
            inspected.GetComponent<InteractObject>().OnDisable();
            hasParent = true;
        }
        else
        {
            inspected = hit;
            inspected.GetComponent<InteractObject>().OnDisable();
        }
        parent = hit.transform.parent;
        originalPos = hit.transform.position;
        originalRot = hit.transform.rotation;
        originalScale = hit.transform.lossyScale;

        /* Check if item have rigibody attached */
        if (inspected.GetComponent<Rigidbody>() != null)
        {
            itemRigibody = inspected.GetComponent<Rigidbody>();
            itemRigibody.isKinematic = true;
            itemRigibody.detectCollisions = false;
        }

        onInspect = true;
        StateMachine.Instance.ShowCursor(true);
        _input.Reset();

    }

    /* This method will set the object's scale when it gets examinated
     * by adding or decreasiing the scale using the class InteractiveItem */
    private void SetScaleExamine()
    {
        if (inspected.GetComponent<Note>() != null)
        {
            if (inspected.GetComponent<InteractiveItem>() != null
                               && inspected.GetComponent<InteractiveItem>().AddScaleIfExamine != 0)
            {
                float scale = inspected.GetComponent<InteractiveItem>().AddScaleIfExamine;
                inspected.transform.localScale += new Vector3(scale, scale, scale);
            }
            else if (inspected.GetComponentInChildren<InteractiveItem>() != null)
            {
                float scale = inspected.GetComponentInChildren<InteractiveItem>().AddScaleIfExamine;
                inspected.transform.localScale += new Vector3(scale, scale, scale);
            }

            if (inspected.GetComponent<InteractiveItem>() != null
                && inspected.GetComponent<InteractiveItem>().ReduceScaleIfExamine != 0)
            {
                float scale = inspected.GetComponent<InteractiveItem>().ReduceScaleIfExamine;
                inspected.transform.localScale -= new Vector3(scale, scale, scale);
            }
        }
        else
        {
            float scale = inspected.GetComponent<InteractiveItem>().AddScaleIfExamine;
            Vector3 inspectedLocalScale = inspected.transform.localScale + new Vector3(scale * inspected.transform.localScale.x,
                scale * inspected.transform.localScale.y, scale * inspected.transform.localScale.z);

            inspected.transform.localScale = Vector3.Lerp(inspected.transform.localScale, inspectedLocalScale, 0.2f);
        }
    }

    private void CheckIfRotating()
    {
        /* Check if rotating */
        if (Input.GetKey(KeyCode.Mouse0))
        {
            playerSocket.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * 150f);
            //playerSocket.Rotate((Input.GetAxis("Mouse Y") * RotationSpeed * Time.deltaTime), (Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime), 0, Space.World);
        } else
        {
            playerSocket.Rotate(new Vector3(_input.look.y, _input.look.x, 0) * Time.deltaTime * 1.2f);
        }
    }

    private void CheckIfZoom()
    {
        /* Check if zooming */
        float ZoomAmount = 0; //With Positive and negative values
        ZoomAmount += Input.GetAxis("Mouse ScrollWheel");
        Vector3 inspectedLocalScale;
        InteractiveItem item;
        if (!hasParent)
            item = inspected.GetComponent<InteractiveItem>();
        else
            item = inspected.GetComponentInChildren<InteractiveItem>();

        if (ZoomAmount > 0 || _input.move.y > 0)
        {
            inspectedLocalScale = inspected.transform.localScale + new Vector3(0.2f * inspected.transform.localScale.x,
                0.2f * inspected.transform.localScale.y, 0.2f * inspected.transform.localScale.z);


            if (item != null
                && inspectedLocalScale.x <= item.maxScale.x
                && inspectedLocalScale.y <= item.maxScale.y
                && inspectedLocalScale.z <= item.maxScale.z)
            {
                inspected.transform.localScale = Vector3.Lerp(inspected.transform.localScale, inspectedLocalScale, 0.2f);
            }
        }
        else if (ZoomAmount < 0 || _input.move.y < 0)
        {
            inspectedLocalScale = inspected.transform.localScale - new Vector3(0.2f * inspected.transform.localScale.x,
                0.2f * inspected.transform.localScale.y, 0.2f * inspected.transform.localScale.z);

            if (item != null
                && inspectedLocalScale.x >= item.minScale.x
                && inspectedLocalScale.y >= item.minScale.y
                && inspectedLocalScale.z >= item.minScale.z)
            {
                inspected.transform.localScale = Vector3.Lerp(inspected.transform.localScale, inspectedLocalScale, 0.2f);
            }
        }
    }

    private void OnNotInspect()
    {
        canInspect = false;
        nameItemUI.SetActive(false);
        descriptionItemText.text = string.Empty;
        nameItemText.text = string.Empty;
        canInspect = true;
    }

    public void InspectOnly(GameObject hit)
    {
        inspectionUI.SetActive(true);
        inspectionCamera.gameObject.SetActive(true);
        SetInspectedObject(hit);
        SetScaleExamine();
        StartCoroutine(PickUpItem());
        Input.ResetInputAxes();
    }

    public IEnumerator PickUpItem()
    {
        StateMachine.Instance.InspectingState(true);
        playerScript.enabled = false;
        inspected.transform.SetParent(playerSocket);
        
        /* If GameObject have children then set them too */
        inspected.layer = 9;
        int children = inspected.transform.childCount;
        for (int i = 0; i < children; ++i)
            inspected.transform.GetChild(i).gameObject.layer = 9;

        if (inspected.GetComponent<InteractObject>() != null)
        {
            inspected.GetComponent<InteractObject>().NotInteractableAnymore();
        }

        if (inspected.GetComponentInChildren<InteractObject>() != null)
        {
            inspected.GetComponentInChildren<InteractObject>().NotInteractableAnymore();
        }

        if (inspected.GetComponent<InteractiveItem>() != null)
        {
            InteractiveItem item;
            if (hasParent)
            {
                item = inspected.GetComponentInChildren<InteractiveItem>();
            }
            else
            {
                item = inspected.GetComponent<InteractiveItem>();
            }
            StartCoroutine(ShowItemName());
        }
        yield return new WaitForSeconds(0.5f);
    }

    /* This method will drop item and store it initial position and size, 
     * disabling raycast and user interaction until this process is complete */
    public IEnumerator DropItem()
    {
        _input.backButton = false;
        _input.examine = false;
        Raycast.Instance.RaycastEnable(false);
        playerScript.enabled = true;
        inspected.gameObject.layer = 0;
        inspectionUI.SetActive(false);
        inspectionCamera.gameObject.SetActive(false);

        int children = inspected.transform.childCount;
        for (int i = 0; i < children; ++i)
            inspected.transform.GetChild(i).gameObject.layer = 0;

        StateMachine.Instance.InspectingState(false);
        yield return new WaitForSeconds(1f);
        playerAnimator.enabled = true;
        canInspect = true;
        
        if (itemRigibody != null)
            itemRigibody.isKinematic = false;
        
        if (inspected.GetComponent<InteractObject>() != null)
            inspected.GetComponent<InteractObject>().IsInteractable();

        if (hasParent)
        {
            if (inspected.GetComponentInChildren<InteractObject>() != null)
                inspected.GetComponentInChildren<InteractObject>().IsInteractable();

            hasParent = false;
        }
        if (itemRigibody != null)
            itemRigibody.detectCollisions = true;
        
        Raycast.Instance.RaycastEnable(true);
        OnNotInspect();
        _input.examine = false;

    }

    private IEnumerator ShowItemName()
    {
        yield return new WaitForSeconds(0.4f);
        nameItemUI.SetActive(true);
        if (inspected.GetComponent<StoreItem>() != null)
        {
            nameItemText.text = inspected.GetComponent<StoreItem>().nameItem;
        }
        else
        {
            if (inspected.transform.childCount > 0 &&  inspected.transform.GetChild(0) != null)
                nameItemText.text = inspected.transform.GetChild(0).gameObject.name;
            else
                nameItemText.text = inspected.gameObject.name;
        }

        if (audioItemNameAppears != null)
            audioItemNameAppears.Play();

        /* Set Item description */
        if (descriptionItemText != null 
            && inspected.GetComponent<StoreItem>() != null )
            descriptionItemText.text = inspected.GetComponent<StoreItem>().descriptionItem;
    }

}
