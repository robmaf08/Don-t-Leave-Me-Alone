using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HoldBreath : MonoBehaviour
{
    [Header("Components")]
    public GameObject UI_HoldBreath;
    [SerializeField] private Image holdBreathSlider;

    [Header("Properties")]
    public float timeHoldBreaht = 10f;
    private float timeRemaining = 0f;
    private float timeRemainingSec = 0;

    //States
    [HideInInspector] public bool isStarted = false;
    [HideInInspector] public bool isDone = false;
    [HideInInspector] public bool isHolding = false;
    [HideInInspector] public bool isFinished = false;

    private void Start()
    {
        UI_HoldBreath.SetActive(false);
    }

    public void StartHoldBreath()
    {
        StartCoroutine(HoldBreathCounter());
    }

    private IEnumerator HoldBreathCounter()
    {
        StarterAssets.StarterAssetsInputs.Instance.Reset();
        Raycast.Instance.RaycastEnable(false);
        UI_HoldBreath.SetActive(true);
        bool isHoldingSpaceKey = false;
        bool doOnce = true;
        isHolding = true;
        timeRemaining = 0f;
        timeRemainingSec = 0f;
        do
        {
            if (doOnce)
            {
                /* Check if space key or right trigger of gamepad
                 * is pressed.*/
                if (InputHandler.isGamepadConnected)
                    yield return new WaitUntil(() => Gamepad.current.rightTrigger.wasPressedThisFrame || Input.GetKeyDown(KeyCode.Space));
                else
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

                isStarted = true;
                doOnce = false;
            }
            
            /* This will check if user is still pressing the same key or button */
            if ( (InputHandler.isGamepadConnected && Gamepad.current != null && Gamepad.current.rightTrigger.isPressed)  
                || Input.GetKey(KeyCode.Space))
            {
                timeRemaining += Time.deltaTime;
                timeRemainingSec += 0.1f;
                holdBreathSlider.fillAmount += Mathf.Clamp01(timeRemainingSec / 1500f * Time.deltaTime);
                isHoldingSpaceKey = true;
            } else
            {
                StarterAssets.StarterAssetsInputs.Instance.Reset();
                isFinished = true;
            }
            yield return null;
            if (holdBreathSlider.fillAmount == 1)
            {
                isHoldingSpaceKey = false;
                isFinished = true;
            }
           
        } while (isHoldingSpaceKey || !isFinished);

        isHolding = false; 
        isFinished = true;
        isStarted = false;
        Raycast.Instance.RaycastEnable(true);
        if (timeRemaining >= timeHoldBreaht || holdBreathSlider.fillAmount >= 1)
        {
            isDone = true;
            gameObject.SetActive(false);
        }
    }

    public void OnDisable()
    {
        if (UI_HoldBreath != null)
            UI_HoldBreath.gameObject.SetActive(false);
       
        holdBreathSlider.fillAmount = 0;
        gameObject.SetActive(false);
    }

}
