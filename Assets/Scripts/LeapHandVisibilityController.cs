using UnityEngine;
using System;
using System.Collections;

public class LeapHandVisibilityController : MonoBehaviour {

    [SerializeField] private GameObject capsuleHand_L;
    [SerializeField] private GameObject capsuleHand_R;

    public void ShowHands()
    {
        ShowRightHand();
        ShowLeftHand();
    }

    public void HideHands()
    {
        HideRightHand();
        HideLeftHand();
    }

    public void ShowRightHand()
    {
        capsuleHand_R.GetComponent<CapsuleHand>()._showHand = true;
        capsuleHand_R.GetComponent<CapsuleHand>()._showArm = true;
        capsuleHand_R.GetComponent<CapsuleHand>().UpdateVisibility();
    }

    public void HideRightHand()
    {
        capsuleHand_R.GetComponent<CapsuleHand>()._showHand = false;
        capsuleHand_R.GetComponent<CapsuleHand>()._showArm = false;
        capsuleHand_R.GetComponent<CapsuleHand>().UpdateVisibility();
    }

    public void ShowLeftHand()
    {
        capsuleHand_L.GetComponent<CapsuleHand>()._showHand = true;
        capsuleHand_L.GetComponent<CapsuleHand>()._showArm = true;
        capsuleHand_R.GetComponent<CapsuleHand>().UpdateVisibility();
    }

    public void HideLeftHand()
    {
        capsuleHand_L.GetComponent<CapsuleHand>()._showHand = false;
        capsuleHand_L.GetComponent<CapsuleHand>()._showArm = false;
        capsuleHand_R.GetComponent<CapsuleHand>().UpdateVisibility();
    }

}
