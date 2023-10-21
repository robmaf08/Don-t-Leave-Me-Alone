using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class FootstepSoundPlayer : MonoBehaviour
{
    [SerializeField] public LayerMask FloorLayer;
    [SerializeField] private TextureSound[] TextureSounds;
    [SerializeField] private bool BlendTerrainSounds;
    private CharacterController Controller;
    public AudioSource AudioSource;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        StartCoroutine(CheckForGround());
    }

    private IEnumerator CheckForGround()
    {
        RaycastHit hit;
        float offsetDistance;
        while (true)
        {
            if (Controller.isGrounded && Controller.velocity != Vector3.zero 
                && Physics.Raycast(transform.position, -Vector3.up, out hit)
                && GetComponent<ThirdPersonController>().enabled)
            {
                offsetDistance = hit.distance;
                Debug.DrawLine(transform.position, hit.point, Color.cyan);
                
                if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
                {
                    yield return StartCoroutine(PlayFootstepSoundFromTerrain(terrain, hit.point));
                } else if (hit.collider.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    //Debug.Log("Footstep from Rendern\n");
                    yield return StartCoroutine(PlayFootstepSoundFromRenderer(renderer));
                }
            } else
            {
               // Debug.Log("No footstep\n");
                CheckIfStopped();
            }
            yield return null;
        }
    }


    private IEnumerator PlayFootstepSoundFromTerrain(Terrain terrain, Vector3 hitPoint)
    {
        Vector3 terrainPosition = hitPoint - terrain.transform.position;
        Vector3 splatMapPosition = new Vector3(
            terrainPosition.x / terrain.terrainData.size.x, 
            0, 
            terrainPosition.z / terrain.terrainData.size.z);
        
        int x = Mathf.FloorToInt(splatMapPosition.x * terrain.terrainData.alphamapWidth);
        int z = Mathf.FloorToInt(splatMapPosition.z * terrain.terrainData.alphamapHeight);

        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, z, 1, 1);
        if (!BlendTerrainSounds)
        {
            int primaryIndex = 0;
            for (int i=1; i<alphaMap.Length; i++)
            {
                if (alphaMap[0, 0, i] > alphaMap[0,0, primaryIndex])
                {
                    primaryIndex = i;
                }
            }

            foreach (TextureSound sound in TextureSounds)
            {
                if (sound.Albedo == terrain.terrainData.terrainLayers[primaryIndex].diffuseTexture)
                {
                    AudioClip clip = GetClipFromTextureSounds(sound);
                    AudioSource.PlayOneShot(clip);
                    if (GetComponent<ThirdPersonController>().GetCurrentSpeed() >= GetComponent<ThirdPersonController>().SprintSpeed)
                    {
                        yield return new WaitForSeconds(clip.length - 0.25f);
                    } else
                    {
                        yield return new WaitForSeconds(clip.length);
                    }

                    break;
                }
            }
        }/* else
        {
            List<AudioClip> clips = new List<AudioClip>();
            int clipIndex = 0;
            for (int i=0; i<alphaMap.Length; i++)
            {
                if (alphaMap[0,0,i] > 0)
                {
                    foreach (TextureSound sound in TextureSounds)
                    {
                        if (sound.Albedo == terrain.terrainData.terrainLayers[i].diffuseTexture)
                        {
                            AudioClip clip = GetClipFromTextureSounds(sound);
                            AudioSource.PlayOneShot(clip, alphaMap[0, 0, i]);
                            clips.Add(clip);
                            clipIndex++;
                            break;
                        }
                    }
                }
            }
            float longestClip = clips.Max(clip => clip.length);
            yield return new WaitForSeconds(longestClip - 0.2f);
        }*/
    }

    private IEnumerator PlayFootstepSoundFromRenderer(Renderer renderer) 
    {
        foreach (TextureSound textureSound in TextureSounds)
        {
            if (textureSound.Albedo == renderer.material.GetTexture("_MainTex"))
            {
                AudioClip clip = null;
                if (textureSound.Clips.Length > 1)
                {
                    clip = GetClipFromTextureSounds(textureSound);
                    AudioSource.PlayOneShot(clip);
                } else if (textureSound.Clips.Length == 1)
                {
                    PlayFootstepOneClipSoundFromRenderer(renderer);
                }
                if (clip != null)
                {
                    yield return new WaitForSeconds(clip.length);
                } else
                {
                    yield return new WaitForSeconds(0);
                }
              
                break;
            }
        }
        yield return null;
    }

    private void CheckIfStopped()
    {
        if (Controller.velocity == Vector3.zero 
            || GetComponent<ThirdPersonController>().GetCurrentSpeed() <= 0
            || !GetComponent<ThirdPersonController>().enabled)
        {
            if (AudioSource.isPlaying)
                AudioSource.Pause();

            StopCoroutine("PlayFootstepSoundFromRenderer");
            StopCoroutine("PlayFootstepSoundFromTerrain");
        }
       
    }

    private void PlayFootstepOneClipSoundFromRenderer(Renderer renderer)
    {
        AudioClip clip = null;
        foreach (TextureSound textureSound in TextureSounds)
        {
            if (textureSound.Albedo == renderer.material.GetTexture("_MainTex"))
            {
                clip = GetClipFromTextureSounds(textureSound);
                if (!AudioSource.isPlaying) 
                {
                    AudioSource.PlayOneShot(clip);
                }
                CheckIfStopped();
                break;
            }
        }
    }

    private AudioClip GetClipFromTextureSounds(TextureSound textureSound)
    {
        int clipIndex = 0;
        if (textureSound.Clips.Length > 1)
        {
            clipIndex = Random.Range(0, textureSound.Clips.Length);
        }
        return textureSound.Clips[clipIndex];
    }

    [System.Serializable]
    private class TextureSound
    {
        public Texture Albedo;
        public AudioClip[] Clips;
    }

}
