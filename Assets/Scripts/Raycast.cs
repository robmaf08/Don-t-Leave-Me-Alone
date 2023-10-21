using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Raycast : MonoBehaviour
{
    //Defaut interactions text
    public const string DEFAULT_USE_COMMAND_TEXT = "Usa";
    public const string DEFAULT_EXAMINE_COMMAND_TEXT = "Esamina";
    
    //Singleton
    public static Raycast Instance;

    [Header("Raycast Crosshair components")]
    public Sprite crosshairNotColliding;
    public Sprite crosshairColliding;
    public Sprite crosshairAiming;
    [SerializeField] private Camera playerCamera;

    [Header("Interaction UI Components")]
    [SerializeField] private GameObject useCommandUI;
    [SerializeField] private GameObject examineCommandUI;

    public float raycastRange = 5f;
    private Collider lastCollider = new Collider();
    private StarterAssets.StarterAssetsInputs _input;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _input = StarterAssets.StarterAssetsInputs.Instance;
       //Physics.queriesHitTriggers = false;
        useCommandUI.SetActive(false);
        examineCommandUI.SetActive(false);
    }

    public Image GetCrosshair()
    {
        return GetComponent<Image>();
    }

    public void SetCrosshair(Sprite crosshair) 
    {
        GetComponent<Image>().sprite = crosshair;
    }

    void Update()
    {
        /// Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        if (playerCamera != null)
        {
           
            //Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(transform.position);
            Ray ray = playerCamera.GetComponent<Camera>().ScreenPointToRay(transform.position);
            RaycastHit hit;

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 5f, layerMask))
            {
      

                //Checking if colliding with an interactable object
                if (hit.collider.gameObject.tag == "InteractableObject")
                {
                    GetComponent<Image>().sprite = crosshairColliding;
                    GetComponent<Image>().color = Color.red;
                    lastCollider = hit.collider;
                    if (hit.collider.gameObject.GetComponent<InteractObject>() != null)
                    {
                        hit.collider.gameObject.GetComponent<InteractObject>().OnHitCollision();
                        
                        //Check if use command
                        if (_input.interact)
                        {
                            _input.interact = false;
                            hit.collider.gameObject.GetComponent<InteractObject>().OnInteract();
                        }

                        //Check if examine command
                        if (_input.examine)
                        {
                            _input.examine = false;
                            _input.backButton = false;
                            hit.collider.gameObject.GetComponent<InteractObject>().OnExamine();
                        }
                    }
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
                }
                else if (hit.collider.GetComponent<Enemy>() != null)
                {
                    IsColliding(true);
                }
                else
                {
                    if (lastCollider != null && lastCollider.gameObject.GetComponent<InteractObject>() != null)
                    {
                        lastCollider.gameObject.GetComponent<InteractObject>().OnNotHitCollision();
                        lastCollider = null;
                    }
                    GetComponent<Image>().sprite = crosshairNotColliding;
                    GetComponent<Image>().color = Color.white;
                }
                _input.interact = false;
                _input.examine = false;
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
                if (lastCollider != null && lastCollider.gameObject.GetComponent<InteractObject>() != null)
                {
                    lastCollider.gameObject.GetComponent<InteractObject>().OnNotHitCollision();
                    lastCollider = null;
                    GetComponent<Image>().sprite = crosshairNotColliding;
                    GetComponent<Image>().color = Color.white;
                }
                _input.interact = false;
                _input.examine = false;
            }
            
        }
    }

    public void IsColliding(bool colliding) {
        if (colliding)
        {
            GetComponent<Image>().sprite = crosshairColliding;
            GetComponent<Image>().color = Color.red;
        } else
        {
            GetComponent<Image>().sprite = crosshairNotColliding;
            GetComponent<Image>().color = Color.white;
        }
    } 

    public static void ShowCommandUse(string commandText, bool show)
    {
        if (Instance.useCommandUI != null)
        {
            Instance.useCommandUI.SetActive(show);
            if (show)
            {
                if (commandText != string.Empty)
                    Instance.useCommandUI.GetComponentInChildren<TextMeshProUGUI>().text = commandText;
                else
                    Instance.useCommandUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            }
            else
            {
                Instance.useCommandUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            }
        }
    }

    public void RaycastEnable(bool enable)
    {
        if (enable)
            enabled = true;
        else
            enabled = false;

        if (!enable)
        {
            ShowCommandExamine(string.Empty, false);
            ShowCommandUse(string.Empty, false);
        }
    }

    public void ShowCrosshair(bool show)
    {    
        Image image = GetComponent<Image>();
        if (show && image != null)
            image.color = new Color(image.color.r, image.color.g, image.color.b, 255);
        else
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }

    public static void ShowCommandExamine(string commandText, bool show)
    {
        if (Instance.examineCommandUI != null)
        {
            Instance.examineCommandUI.SetActive(show);
            if (show)
            {
                if (commandText != string.Empty)
                    Instance.examineCommandUI.GetComponentInChildren<TextMeshProUGUI>().text = commandText;
                else
                    Instance.examineCommandUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            }
            else
            {
                Instance.examineCommandUI.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            }
        }
    }

}
