using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class KillerFrostEntrance : MonoBehaviour
{
    public TextMeshProUGUI textGUI;
    [SerializeField] private Image holdBreathSlider;
    [SerializeField] private GameObject doorEntrance;
    [SerializeField] private GameObject doorBedroom;
    [SerializeField] private GameObject lightSwitch;
    [SerializeField] private GameObject hiddenPlace;
    [SerializeField] private HoldBreath holdBreath;
    [SerializeField] private DeathManager deathManager;
    [SerializeField] private GoalManager goalManager;
    public Jumpscare youDead;

    public static bool isHidden;
    public static bool canHide;
    public static bool blackOut;

    //Events after Killer Frost appareances
    public UnityEvent eventsBeforeAppareance;
    public UnityEvent eventsAfterAppareance;

    private void Start()
    {
        canHide = false;
        isHidden = false;
        blackOut = true;
    }

    public void KillerFrostArrives()
    {
        StartCoroutine(FoundOutKillerFrost());
    }

    private IEnumerator FoundOutKillerFrost()
    {
        eventsBeforeAppareance.Invoke();
        tag = "Untagged";
        canHide = true;
        hiddenPlace.tag = "InteractableObject";

        string[] phrases = { "Killer Frost è qui!", "Devo nascodermi sotto il letto!" };

        GameObject killerCome = new GameObject();
        killerCome.AddComponent<AudioSource>();
        killerCome.AddComponent<AudioBehaviour>();
        killerCome.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/KillerFrost/KillerFrostEntranceSound");
        killerCome.GetComponent<AudioSource>().Play();
        Destroy(killerCome, Resources.Load<AudioClip>("Audio/KillerFrost/KillerFrostEntranceSound").length);

        GameObject musicBackground = new GameObject();
        musicBackground.AddComponent<AudioSource>();
        musicBackground.AddComponent<AudioBehaviour>();
        musicBackground.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/KillerFrost/AnxietyMusic");
        musicBackground.GetComponent<AudioSource>().Play();
        Destroy(musicBackground, Resources.Load<AudioClip>("Audio/KillerFrost/AnxietyMusic").length);

        yield return StartCoroutine(GetComponent<LookObject>().Look(phrases));
        goalManager.NextGoal();

        yield return new WaitWhile(() => !isHidden);

        GetComponent<CameraLookObj>().ChangeCamera();
        string textLook = "Killer Frost è vicino! Non devo farmi sentire!";
        yield return StartCoroutine(LookObject.LookOnly(textGUI, textLook));
        Raycast.Instance.RaycastEnable(false);
        StateMachine.Instance.DialogueSate(true);

        //Start Hold Breath Mechanics
        GameObject oneShotSound = new GameObject();
        oneShotSound.AddComponent<AudioSource>();
        oneShotSound.AddComponent<AudioBehaviour>();
        oneShotSound.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/KillerFrost/KillerFrostScreaming");
        holdBreath.StartHoldBreath();
        StarterAssets.StarterAssetsInputs.Instance.Reset();
        
        /* We must wait until player press the key, then the sound can start */
        yield return new WaitWhile(() => !holdBreath.isStarted);
        oneShotSound.GetComponent<AudioSource>().Play();

        yield return new WaitWhile( () => !holdBreath.isFinished || Time.timeScale == 0);
 
        oneShotSound.GetComponent<AudioSource>().Stop();
        if (!holdBreath.isDone)
        {
            holdBreath.OnDisable();
            youDead.StartJumpscare();
            yield return new WaitForSeconds((float) youDead.videoJumpscare.length);
            deathManager.InstantDie();
            Destroy(gameObject);
        }

        Destroy(oneShotSound);
        hiddenPlace.tag = "Untagged";
        holdBreath.OnDisable();
        string[] text = { "Credo se ne sia andato...", "Ci è mancato poco, per un pelo..." };
        //yield return StartCoroutine(GeneralInteractObject.WaitForTextFinish(textGUI, text, gameObject));
        yield return StartCoroutine(GetComponent<LookObject>().Look(text));
        GetComponent<CameraLookObj>().ReturnMainCamera();
        StarterAssets.StarterAssetsInputs.Instance.Reset();
        yield return new WaitForSeconds(1f);
        Blackout.instance.StartBlackout();
        text = new string[] {"Oh no, la corrente è saltata!",
            "Devo riaccendere le luci e aspettare che Hannah venga a prendermi!",
            "Ma prima devo trovare un generatore di corrente..."};
        
        yield return StartCoroutine(GetComponent<LookObject>().Look(text));
        
        blackOut = true;
        goalManager.NextGoal();
        Destroy(musicBackground);
        StateMachine.Instance.DialogueSate(false);
        Raycast.Instance.RaycastEnable(true);
        eventsAfterAppareance.Invoke();
    }

}
