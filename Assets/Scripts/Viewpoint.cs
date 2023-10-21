using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Viewpoint : MonoBehaviour
{
    [Header("Viewpoint")]

    [SerializeField] private Canvas viewpointContainerUI;
    
    [SerializeField, TextArea] private string PointText = "Press E";
    
    [Space, SerializeField] Camera cam;
    
    [SerializeField] GameObject PlayerController;
    
    [SerializeField] Image ImagePrefab;
    
    [Space ,SerializeField, Range(0.1f, 20)] float MaxViewRange = 8;
    
    [SerializeField, Range(0.1f, 20)] float MaxTextViewRange = 3;
    
    float Distance;
    
    Text ImageText;
    
    Image ImageUI;

    public void OnDisable()
    {
        if (ImageUI != null)
        {
            ImageUI.color = new Color(ImageUI.color.r, ImageUI.color.g, ImageUI.color.b, 0f);
            ImageText.color = new Color(ImageText.color.r, ImageText.color.g, ImageText.color.b, 0f);
        }
        enabled = false;
    }

    public void OnEnable()
    {
        ImageUI = Instantiate(ImagePrefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        ImageText = ImageUI.GetComponentInChildren<Text>();
        ImageText.text = PointText;
        ImageUI.color = new Color(ImageUI.color.r, ImageUI.color.g, ImageUI.color.b, 0f);
        ImageText.color = new Color(ImageText.color.r, ImageText.color.g, ImageText.color.b, 0f);
        ImageUI.transform.SetParent(viewpointContainerUI.transform);
    }

    public void SetPointText(string text)
    {
        PointText = text;
        ImageText.text = PointText;
    }

    void Update()
    {
        if (cam.enabled && viewpointContainerUI.GetComponent<Canvas>().enabled)
        {
            if (!ImageUI.gameObject.activeSelf)
            {
                ImageUI.gameObject.SetActive(true);
                OnEnable();
            }

            ImageUI.transform.position = cam.WorldToScreenPoint(CalculateWorldPosition(transform.position, cam));
            Distance = Vector3.Distance(PlayerController.transform.position, transform.position);

            if (Distance < MaxTextViewRange)
            {
                Color OpacityColor = ImageText.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 1, 10 * Time.deltaTime);
                ImageText.color = OpacityColor;
            }
            else
            {
                Color OpacityColor = ImageText.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 0, 10 * Time.deltaTime);
                ImageText.color = OpacityColor;
            }
            if (Distance < MaxViewRange)
            {
                Color OpacityColor = ImageUI.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 1, 10 * Time.deltaTime);
                ImageUI.color = OpacityColor;
            }
            else
            {
                Color OpacityColor = ImageUI.color;
                OpacityColor.a = Mathf.Lerp(OpacityColor.a, 0, 10 * Time.deltaTime);
                ImageUI.color = OpacityColor;
            }
            
        } else
        {
            ImageUI.gameObject.SetActive(false);
        }
    }

    private Vector3 CalculateWorldPosition(Vector3 position, Camera camera)
    {
        Vector3 camNormal = camera.transform.forward;
        Vector3 vectorFromCam = position - camera.transform.position;
        float camNormDot = Vector3.Dot(camNormal, vectorFromCam.normalized);
        if (camNormDot <= 0f)
        {
            float camDot = Vector3.Dot(camNormal, vectorFromCam);
            Vector3 proj = (camNormal * camDot) * 1.01f;
            position = camera.transform.position + (vectorFromCam - proj);
        }
        return position;
    }

 
    
}
