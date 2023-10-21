using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GoalManager : MonoBehaviour
{

    [Header("Components")]
    public TextMeshProUGUI GoalText;
    public GameObject GoalUI;
    [SerializeField] private AnimationClip goalUIAnimation;
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private AudioSource goalAudioSource;
    public Goal[] goals;

    private bool goalsCompleted = false;
    private int currentIndex = 0;

    void Start()
    {
        currentIndex = 0;
        goalsCompleted = false;
    }

    public void NextGoal()
    {
        if (!goalsCompleted)
        {
            SetGoalCompleted();
            if (currentIndex < goals.Length)
            {
                if (goals[currentIndex] != null)
                {
                    SetGoal(goals[currentIndex]);
                }
                else
                {
                    goalsCompleted = true;
                }
            }
            else
            {
                goalsCompleted = true;
            }
        }
    }

    public void SetGoal(Goal goal)
    {
        if (goal != null)
        {
            if (!GoalUI.activeSelf)
                GoalUI.SetActive(true);

            goalAudioSource.clip = goalSound;
            goalAudioSource.Play();
            GoalText.text = goal.nameGoal;
            GoalUI.GetComponent<Animator>().Rebind();
            GoalUI.GetComponent<Animator>().Play(goalUIAnimation.name);
        }
    }

    public void SetFirstGoal()
    {
        SetGoal(goals[0]);
    }

    public void SetGoalCompleted()
    {
        goals[currentIndex].completed = true;
        currentIndex++;
    }

    public Goal GetCurrentGoal() 
    {
        return goals[currentIndex];
    }

}
