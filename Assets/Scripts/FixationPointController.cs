using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FixationPointController : MonoBehaviour {

    public Text m_FixationText;
    public Color m_Color = Color.red;
    public float m_VisibilityDuration = 0.5F;

    private Color m_MainColor;

    void Start ()
    {
        m_MainColor = m_FixationText.color;
    }

    public void EnableFixationPoint()
    {
        m_FixationText.enabled = true;
    }

    public void DisableFixationPoint()
    {
        m_FixationText.enabled = false;
    }

    public IEnumerator ShowCue()
    {
        m_FixationText.color = m_Color;

        yield return new WaitForSeconds(m_VisibilityDuration);

        m_FixationText.color = m_MainColor;
    }
}
