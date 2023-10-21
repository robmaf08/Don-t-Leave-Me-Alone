using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class LevelComplete : MonoBehaviour
{

    public void CompleteLevel() 
    {
        StateMachine.Instance.TurnOffAllSound();
        if (StateMachine.Instance != null)
        {
            GetComponent<Animator>().Play("LevelComplete");
            Time.timeScale = 0;
            enabled = true;
        }
   
    }

    public void CompleteLevel(float delay) 
    {
        StartCoroutine(Delay(delay));   
    }

    private IEnumerator Delay(float delay) 
    {
        yield return new WaitForSeconds(delay);
        CompleteLevel();
    }

    private void Start() 
    {
        enabled = false;
    }

    private void Update() 
    {
        if (StarterAssets.StarterAssetsInputs.Instance.jump)
            SceneManager.LoadScene(0);
    }
}
