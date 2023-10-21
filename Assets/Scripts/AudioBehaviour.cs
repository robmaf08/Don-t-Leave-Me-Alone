using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBehaviour : MonoBehaviour
{
    
    private AudioSource m_AudioSource;
    private bool wasPlaying = false; 

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Time.timeScale > 0f)
        {
            if (wasPlaying && !m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            } else  if (wasPlaying && m_AudioSource.isPlaying)
            {
                wasPlaying = false;
            }
        } 

        if (Time.timeScale == 0f)
        {
            if (m_AudioSource != null && m_AudioSource.isPlaying)
            {
                wasPlaying = true;
                m_AudioSource.Pause();
            }
        }
       
    }
}
