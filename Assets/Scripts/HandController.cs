using UnityEngine;
using System.Linq;
using System.Collections;
using Leap;
using System;

public class HandController : MonoBehaviour {

    private LeapProvider provider;

    [SerializeField] MainController mainController;

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>() as LeapProvider;
    }

    internal Vector3 getPalmVelocity()
    {
        foreach (Hand hand in provider.CurrentFrame.Hands)
        {
            if (hand.IsLeft)
            {
                return hand.PalmVelocity.ToVector3();
            }
        }

        return default(Vector3);
    }

    internal Vector3 getPalmPosition()
    {
        foreach (Hand hand in provider.CurrentFrame.Hands)
        {
            if (hand.IsLeft)
            {
                return hand.PalmPosition.ToVector3();
            }
        }

        return default(Vector3);
    }

    internal Vector3 getMiddleFingerPosition()
    {
        foreach (Hand hand in provider.CurrentFrame.Hands)
        {
            if (hand.IsLeft)
            {
                return hand.Fingers.First(f => f.Type == Finger.FingerType.TYPE_MIDDLE).TipPosition.ToVector3();
            }
        }

        return default(Vector3);
    }

    internal Vector3 getThumbPosition()
    {
        foreach (Hand hand in provider.CurrentFrame.Hands)
        {
            if (hand.IsLeft)
            {
                return hand.Fingers.First(f => f.Type == Finger.FingerType.TYPE_THUMB).TipPosition.ToVector3();
            }
        }

        return default(Vector3);
    }

    internal Vector3 getMiddleFingerHitPoint()
    {
        Vector3 centerEye = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye);
        Vector3 fingerPosition = getMiddleFingerPosition();
        Vector3 fingerDirection = fingerPosition - centerEye;

        RaycastHit fingerHitOnPlane;
        Physics.Raycast(centerEye, fingerDirection, out fingerHitOnPlane, 1000, 1 << 8);

        return fingerHitOnPlane.point;
    }

    internal Vector3 getThumbHitPoint()
    {
        Vector3 centerEye = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye);
        Vector3 fingerPosition = getThumbPosition();
        Vector3 fingerDirection = fingerPosition - centerEye;

        RaycastHit fingerHitOnPlane;
        Physics.Raycast(centerEye, fingerDirection, out fingerHitOnPlane, 1000, 1 << 8);

        return fingerHitOnPlane.point;
    }

    internal Vector3 getPalmHitPoint()
    {
        Vector3 centerEye = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.CenterEye);
        Vector3 palmPosition = getPalmPosition();
        Vector3 palmDirection = palmPosition - centerEye;

        RaycastHit hitOnPlane;
        Physics.Raycast(centerEye, palmDirection, out hitOnPlane, 1000, 1 << 8);

        return hitOnPlane.point;
    }
}
