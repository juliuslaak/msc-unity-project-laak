using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// This class is used to display a red (or any chosen colour)
// plane (UI->Image) for a short time interval.
public class VRPlaneShow : MonoBehaviour {

    public event Action OnShowComplete;

    public Image m_VisiblePlane;
    public Color m_Color = Color.red;
    public float m_VisibilityDuration = 1.0F;
    [Range(0.0F, 1.0F)]
    public float m_Transparency = 0.5F; 
    
    private bool m_IsVisible;
    private Color m_OpaqueColor;

    public bool IsShowing { get { return m_IsVisible; } }


    private void Awake()
    {
        m_VisiblePlane.enabled = false;
        m_OpaqueColor = new Color(m_Color.r, m_Color.g, m_Color.b, m_Transparency);
    }

    public IEnumerator ShowPlane(float duration)
    {
        yield return StartCoroutine(Show(duration));
    }

    public IEnumerator ShowPlane()
    {
        yield return StartCoroutine(Show(m_VisibilityDuration));
    }

    private IEnumerator Show(float duration)
    {
        m_IsVisible = true;

        m_VisiblePlane.color = m_OpaqueColor;
        m_VisiblePlane.enabled = true;

        yield return new WaitForSeconds(duration);

        m_VisiblePlane.enabled = false;

        m_IsVisible = false;

        // If anything is subscribed to OnShowComplete call it.
        if (OnShowComplete != null)
            OnShowComplete();
    }
}
