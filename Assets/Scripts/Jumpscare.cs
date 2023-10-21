using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using StarterAssets;

public class Jumpscare : MonoBehaviour
{
    
    [Header("Components")]
    [SerializeField] private ThirdPersonController _controller;
    [SerializeField] private AudioSource audioJumpscare;
    [SerializeField] private AudioClip audioClipJumpscare;
    [SerializeField] private GameObject UI_Jumpscare;
    public VideoPlayer videoPlayerJumpscare;
    public VideoClip videoJumpscare;

    private bool doOnce = true;

    private void Start()
    {
        enabled = false;
    }

    public void StartJumpscare()
    {
        if (doOnce)
        {
            enabled = true;
            doOnce = false;
            StartCoroutine(JumpScare());
        }
    }

    private IEnumerator JumpScare()
    {
        if (videoJumpscare != null)
        {
            UI_Jumpscare.SetActive(true);
        }
        _controller.LockCameraPosition = true;
        StateMachine.Instance.PlayerCanMove(false);
        yield return StartCoroutine(PlayVideoJumpscare());
        yield return new WaitForSeconds((float) videoJumpscare.length);
        UI_Jumpscare.SetActive(false);
        _controller.LockCameraPosition = false;
        StateMachine.Instance.PlayerCanMove(true);
        Destroy(gameObject);
    }

    private IEnumerator PlayVideoJumpscare()
    {
        if (videoPlayerJumpscare != null && !videoPlayerJumpscare.enabled)
            videoPlayerJumpscare.enabled = true;

        videoPlayerJumpscare.clip = videoJumpscare;
        videoPlayerJumpscare.Play();
        yield return new WaitForSeconds(((float)videoJumpscare.length));
        videoPlayerJumpscare.clip = null;
    }

}
