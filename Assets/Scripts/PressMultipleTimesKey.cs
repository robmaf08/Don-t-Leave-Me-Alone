using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PressMultipleTimesKey : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject pressMultipleTimeUI;
    public Image ringHealthBar;
    public TextMeshProUGUI textPress;
    
    [Header("Properties")]
    public float time = 1.8f;
    public bool complete = false;
    [Header("Custom events after complete")]
    public UnityEvent OnCompleteEvent;

    private StarterAssets.StarterAssetsInputs _input;

    private void Start()
    {
        _input = StarterAssets.StarterAssetsInputs.Instance;
        ringHealthBar.fillAmount = 0;
    }

    private void OnEnable()
    {
        ringHealthBar.gameObject.SetActive(true);
        ringHealthBar.fillAmount = 0;
        enabled = true;
        StateMachine.Instance.InspectingState(true);
    }

    private void OnDisable()
    {
        StateMachine.Instance.InspectingState(false);
        pressMultipleTimeUI.gameObject.SetActive(false);    
        complete = false;
        enabled = false;
    }

    void Update()
    {
        if (!complete)
        {
            if (ringHealthBar.fillAmount == 1)
            {
                complete = true;
                OnCompleteEvent.Invoke();
                StateMachine.Instance.InspectingState(false);
                //_input.Reset();
                OnDisable();
                Destroy(this);
            }
            if (_input.jump)
            {
                ringHealthBar.fillAmount += 0.025f;
                textPress.transform.localScale += new Vector3(0.02f, 0.02f, 0.02f);
                _input.Reset();
            }
            else
            {
                time -= Time.deltaTime;

                if (textPress.transform.localScale.x > 0.30f)
                    textPress.transform.localScale -= new Vector3(0.001f, 0.001f, 0.001f);

                if (time <= 0f)
                {
                    ringHealthBar.fillAmount -= 0.1f;
                    time = 1.8f;
                }
            }
        }
    }
}
