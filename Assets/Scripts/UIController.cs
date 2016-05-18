using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private UIFader m_CenterScreenUI;
    [SerializeField] private UIFader m_FixationPointUI;
    [SerializeField] private UIFader m_SignalToMoveHandUI;
    [SerializeField] private UIFader m_MovingObjectsUI;
    [SerializeField] private UIFader m_TargetObjectUI;
    [SerializeField] private UIFader m_ReactionIntroUI;
    [SerializeField] private UIFader m_ReactionOutroUI;
    [SerializeField] private UIFader m_HandVisibleUI;
    [SerializeField] private UIFader m_HandInvisibleUI;
    [SerializeField] private UIFader m_ExperimentIntroUI;
    [SerializeField] private UIFader m_ExperimentOutroUI;
    [SerializeField] private UIFader m_MainTutorialIntroUI;

    public void ShowText(string instructions)
    {
        switch (instructions)
        {
            case "centerScreen":
                StartCoroutine(ShowCenterScreenUI());
                break;
            case "FixationPoint":
                StartCoroutine(ShowFixationPointUI());
                break;
            case "SignalToMoveHand":
                StartCoroutine(ShowSignalToMoveHandUI());
                break;
            case "MovingObjects":
                StartCoroutine(ShowMovingObjectsUI());
                break;
            case "TargetObject":
                StartCoroutine(ShowTargetObjectUI());
                break;
            case "reactionIntro":
                StartCoroutine(ShowReactionIntroUI());
                break;
            case "reactionOutro":
                StartCoroutine(ShowReactionOutroUI());
                break;
            case "handVisible":
                StartCoroutine(ShowHandVisibleUI());
                break;
            case "handInvisible":
                StartCoroutine(ShowHandInvisibleUI());
                break;
            case "experimentIntro":
                StartCoroutine(ShowExperimentIntroUI());
                break;
            case "experimentOutro":
                StartCoroutine(ShowExperimentOutroUI());
                break;
            case "mainTutorialIntro":
                StartCoroutine(ShowMainTutorialIntroUI());
                break;
            default:
                Debug.Log("Unregistred case of SHOW UI text call!");
                break;
        }
    }

    public IEnumerator HideText(string instructions)
    {
        switch (instructions)
        {
            case "centerScreen":
                yield return StartCoroutine(HideCenterScreenUI());
                break;
            case "FixationPoint":
                yield return StartCoroutine(HideFixationPointUI());
                break;
            case "SignalToMoveHand":
                yield return StartCoroutine(HideSignalToMoveHandUI());
                break;
            case "MovingObjects":
                yield return StartCoroutine(HideMovingObjectsUI());
                break;
            case "TargetObject":
                yield return StartCoroutine(HideTargetObjectUI());
                break;
            case "reactionIntro":
                yield return StartCoroutine(HideReactionIntroUI());
                break;
            case "reactionOutro":
                yield return StartCoroutine(HideReactionOutroUI());
                break;
            case "handVisible":
                yield return StartCoroutine(HideHandVisibleUI());
                break;
            case "handInvisible":
                yield return StartCoroutine(HideHandInvisibleUI());
                break;
            case "experimentIntro":
                yield return StartCoroutine(HideExperimentIntroUI());
                break;
            case "experimentOutro":
                yield return StartCoroutine(HideExperimentOutroUI());
                break;
            case "mainTutorialIntro":
                yield return StartCoroutine(HideMainTutorialIntroUI());
                break;
            default:
                Debug.Log("Unregistred case of HIDE UI text call!");
                break;
        }
    }


    private IEnumerator ShowCenterScreenUI()
    {
        yield return StartCoroutine(m_CenterScreenUI.InteruptAndFadeIn());
    }
    private IEnumerator HideCenterScreenUI()
    {
        yield return StartCoroutine(m_CenterScreenUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowFixationPointUI()
    {
        yield return StartCoroutine(m_FixationPointUI.InteruptAndFadeIn());
    }
    private IEnumerator HideFixationPointUI()
    {
        yield return StartCoroutine(m_FixationPointUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowSignalToMoveHandUI()
    {
        yield return StartCoroutine(m_SignalToMoveHandUI.InteruptAndFadeIn());
    }
    private IEnumerator HideSignalToMoveHandUI()
    {
        yield return StartCoroutine(m_SignalToMoveHandUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowMovingObjectsUI()
    {
        yield return StartCoroutine(m_MovingObjectsUI.InteruptAndFadeIn());
    }
    private IEnumerator HideMovingObjectsUI()
    {
        yield return StartCoroutine(m_MovingObjectsUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowTargetObjectUI()
    {
        yield return StartCoroutine(m_TargetObjectUI.InteruptAndFadeIn());
    }
    private IEnumerator HideTargetObjectUI()
    {
        yield return StartCoroutine(m_TargetObjectUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowReactionIntroUI()
    {
        yield return StartCoroutine(m_ReactionIntroUI.InteruptAndFadeIn());
    }
    private IEnumerator HideReactionIntroUI()
    {
        yield return StartCoroutine(m_ReactionIntroUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowReactionOutroUI()
    {
        yield return StartCoroutine(m_ReactionOutroUI.InteruptAndFadeIn());
    }
    private IEnumerator HideReactionOutroUI()
    {
        yield return StartCoroutine(m_ReactionOutroUI.InteruptAndFadeOut());
    }



    private IEnumerator ShowHandVisibleUI()
    {
        // Wait for the outro to fade in.
        yield return StartCoroutine(m_HandVisibleUI.InteruptAndFadeIn());
    }
    private IEnumerator HideHandVisibleUI()
    {
        // Wait for the outro to fade out.
        yield return StartCoroutine(m_HandVisibleUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowHandInvisibleUI()
    {
        // Wait for the outro to fade in.
        yield return StartCoroutine(m_HandInvisibleUI.InteruptAndFadeIn());
    }
    private IEnumerator HideHandInvisibleUI()
    {
        // Wait for the outro to fade out.
        yield return StartCoroutine(m_HandInvisibleUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowExperimentIntroUI()
    {
        // Wait for the outro to fade in.
        yield return StartCoroutine(m_ExperimentIntroUI.InteruptAndFadeIn());
    }
    private IEnumerator HideExperimentIntroUI()
    {
        // Wait for the outro to fade out.
        yield return StartCoroutine(m_ExperimentIntroUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowExperimentOutroUI()
    {
        // Wait for the outro to fade in.
        yield return StartCoroutine(m_ExperimentOutroUI.InteruptAndFadeIn());
    }
    private IEnumerator HideExperimentOutroUI()
    {
        // Wait for the outro to fade out.
        yield return StartCoroutine(m_ExperimentOutroUI.InteruptAndFadeOut());
    }


    private IEnumerator ShowMainTutorialIntroUI()
    {
        // Wait for the outro to fade in.
        yield return StartCoroutine(m_MainTutorialIntroUI.InteruptAndFadeIn());
    }
    private IEnumerator HideMainTutorialIntroUI()
    {
        // Wait for the outro to fade out.
        yield return StartCoroutine(m_MainTutorialIntroUI.InteruptAndFadeOut());
    }

}