using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public GameObject LoadingScreenScene;
    AsyncOperation loading;
    public TextMeshProUGUI PressSpaceText;
    public Image loadIcon;
    float progressValue;
    float currentValue;

    [Header("Loading Icon")]
    public Animator waitingSpinnerIcon;
    public AnimationClip waitingSpinnerIconAnimation;

    [Header("Loading Scene Info")]
    public Image backgroundLevelImage;
    public Image pressAnyKeyImageAnimation;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI descriptionLevelText;

   private AsyncOperation operation;
    
    public void LoadMainMenu() 
    {
        SceneManager.LoadScene(0);
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadSceneById(int sceneId)
    {
        Destroy(StateMachine.Instance);
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    public void LoadSceneByName(string sceneName)
    {
        waitingSpinnerIcon.Play(waitingSpinnerIconAnimation.ToString());
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadScene(LevelScene level)
    {
        LoadingScreenScene.SetActive(true);
        levelNameText.text = level.nameLevel;
        descriptionLevelText.text = level.descriptionLevel;
        backgroundLevelImage.sprite = level.imageLevel;
        Time.timeScale = 1f;
        StartCoroutine(LoadSceneAsync(level.idLevel));   
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        /* Stop all audios and destroy all object scene */
        StopAllAudios();
        StopBehaviours();

        if (!waitingSpinnerIcon.enabled)
            waitingSpinnerIcon.enabled = true;

        /* Start loading new scene */
        operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;
        while (progressValue <= 0.9f)
        {
            progressValue = operation.progress / 0.9f;
            currentValue = Mathf.MoveTowards(currentValue, progressValue, 0.25f * Time.deltaTime);
            yield return null;
        }
       
        waitingSpinnerIcon.gameObject.SetActive(false);
        StarterAssets.StarterAssetsInputs.Instance.Reset();
        PressSpaceText.transform.gameObject.GetComponent<Animator>().enabled = true;
        yield return new WaitWhile(() => !Input.anyKeyDown);
        StartCoroutine (FadeIn(operation));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        /* Stop all audios and destroy all object scene */
        StopAllAudios();
        StopBehaviours();

        if (!waitingSpinnerIcon.enabled)
            waitingSpinnerIcon.enabled = true;

        /* Start loading new scene */
        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (progressValue <= 0.9f)
        {
            progressValue = operation.progress / 0.9f;
            currentValue = Mathf.MoveTowards(currentValue, progressValue, 0.25f * Time.deltaTime);
            yield return null;
        }

        waitingSpinnerIcon.gameObject.SetActive(false);
        StarterAssets.StarterAssetsInputs.Instance.Reset();
        PressSpaceText.transform.gameObject.GetComponent<Animator>().enabled = true;
        yield return new WaitWhile(() => !Input.anyKeyDown);
        StartCoroutine(FadeIn(operation));
    }

    private void StopAllAudios() 
    {
        foreach(AudioSource audio in GameObject.FindObjectsOfType<AudioSource>(true))
            audio.Stop();   
    }

    private void StopBehaviours() 
    {
        foreach (AudioSource audio in GameObject.FindObjectsOfType<AudioSource>(true))
            audio.Stop();

        foreach (InteractObject obj in GameObject.FindObjectsOfType<InteractObject>(true))
            obj.GetComponent<InteractObject>().enabled = false;

        foreach (Enemy obj in GameObject.FindObjectsOfType<Enemy>(true))
            obj.GetComponent<Enemy>().enabled = false;

        foreach (Viewpoint obj in GameObject.FindObjectsOfType<Viewpoint>(true))
            obj.GetComponent<Viewpoint>().enabled = false;
        
        if (GameObject.FindGameObjectWithTag("Player") != null)
            GameObject.Destroy(GameObject.FindGameObjectWithTag("Player"));
    }

    private IEnumerator FadeIn(AsyncOperation operation)
    {
        pressAnyKeyImageAnimation.gameObject.SetActive(true);
        for (float i = 0f; i <= 1f; i += 0.05f)
        {
            pressAnyKeyImageAnimation.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        pressAnyKeyImageAnimation.color = new Color(0, 0, 0, 255);
        operation.allowSceneActivation = true;
    }

}
