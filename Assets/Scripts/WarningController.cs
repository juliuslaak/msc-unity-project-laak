using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class WarningController : MonoBehaviour {
    
    [SerializeField] private FixationPointController m_FixationPointController;

	[SerializeField] private UIFader m_HandNoticeUI;
	[SerializeField] private UIFader m_HigherNoticeUI;
	[SerializeField] private UIFader m_ReactionNoticeUI;
	[SerializeField] private UIFader m_FasterNoticeUI;
	[SerializeField] private UIFader m_SlowerNoticeUI;
	[SerializeField] private UIFader m_NoTargetNoticeUI;
	[SerializeField] private UIFader m_PausNoticeUI;
    [Range(0, 2F)]
    [SerializeField] private float m_showTime;

    public IEnumerator ShowHandNoticeUI()
    {
        m_FixationPointController.DisableFixationPoint();

        yield return StartCoroutine(m_HandNoticeUI.InteruptAndFadeIn());
        yield return new WaitForSeconds(m_showTime);
        StartCoroutine(HideHandNoticeUI());

        m_FixationPointController.EnableFixationPoint();
    }
    public IEnumerator HideHandNoticeUI()
    {
        yield return StartCoroutine(m_HandNoticeUI.InteruptAndFadeOut());
    }


    public IEnumerator ShowNoReactionNoticeUI()
    {
        m_FixationPointController.DisableFixationPoint();

        yield return StartCoroutine(m_ReactionNoticeUI.InteruptAndFadeIn());
        yield return new WaitForSeconds(m_showTime);
        StartCoroutine(HideReactionNoticeUI());

        m_FixationPointController.EnableFixationPoint();
    }
    public IEnumerator HideReactionNoticeUI()
    {
        yield return StartCoroutine(m_ReactionNoticeUI.InteruptAndFadeOut());
    }


    public IEnumerator ShowFasterNoticeUI()
    {
        m_FixationPointController.DisableFixationPoint();

        yield return StartCoroutine(m_FasterNoticeUI.InteruptAndFadeIn());
        yield return new WaitForSeconds(m_showTime);
        StartCoroutine(HideFasterNoticeUI());

        m_FixationPointController.EnableFixationPoint();
    }
    public IEnumerator HideFasterNoticeUI()
    {
        yield return StartCoroutine(m_FasterNoticeUI.InteruptAndFadeOut());
    }


    public IEnumerator ShowSlowerNoticeUI()
    {
        m_FixationPointController.DisableFixationPoint();

        yield return StartCoroutine(m_SlowerNoticeUI.InteruptAndFadeIn());
        yield return new WaitForSeconds(m_showTime);
        StartCoroutine(HideSlowerNoticeUI());

        m_FixationPointController.EnableFixationPoint();
    }
    public IEnumerator HideSlowerNoticeUI()
    {
        yield return StartCoroutine(m_SlowerNoticeUI.InteruptAndFadeOut());
    }


    public IEnumerator ShowNoTargetNoticeUI()
    {
        m_FixationPointController.DisableFixationPoint();

        yield return StartCoroutine(m_NoTargetNoticeUI.InteruptAndFadeIn());
        yield return new WaitForSeconds(m_showTime);
        StartCoroutine(HideNoTargetNoticeUI());

        m_FixationPointController.EnableFixationPoint();
    }
    public IEnumerator HideNoTargetNoticeUI()
    {
        yield return StartCoroutine(m_NoTargetNoticeUI.InteruptAndFadeOut());
    }


    public IEnumerator ShowHigherNoticeUI()
    {
        m_FixationPointController.DisableFixationPoint();

        yield return StartCoroutine(m_HigherNoticeUI.InteruptAndFadeIn());
        yield return new WaitForSeconds(m_showTime);
        StartCoroutine(HideHigherNoticeUI());

        m_FixationPointController.EnableFixationPoint();
    }
    public IEnumerator HideHigherNoticeUI()
    {
        yield return StartCoroutine(m_HigherNoticeUI.InteruptAndFadeOut());
    }


    public IEnumerator ShowPausNoticeUI(float s)
    {
        m_FixationPointController.DisableFixationPoint();

        yield return StartCoroutine(m_PausNoticeUI.InteruptAndFadeIn());

        if (s == 0)
            s = m_showTime;

        yield return new WaitForSeconds(s);
        StartCoroutine(m_PausNoticeUI.InteruptAndFadeOut());

        m_FixationPointController.EnableFixationPoint();
    }
}
