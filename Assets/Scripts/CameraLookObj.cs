using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookObj : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Camera CameraLookObject;
    [SerializeField] private Camera MainCamera;

    void Start()
    {
    }

    // Update is called once per frame
    public void ChangeCamera()
    {
        // GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        //MainCamera.gameObject.SetActive(false);
        CameraLookObject.gameObject.SetActive(true);
    }

    public void ReturnMainCamera()
    {
        //GameObject.FindGameObjectWithTag("MainCamera").SetActive(true);
        MainCamera.gameObject.SetActive(true);
        CameraLookObject.gameObject.SetActive(false);
    }

    public void DestroyCameraObj()
    {
        CameraLookObject.gameObject.SetActive(true);
        Destroy(CameraLookObject.gameObject);
    }
}
