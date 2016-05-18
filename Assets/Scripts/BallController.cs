using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

    private Animator anim;
    private Plane[] cameraPlanes;
    private int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");


    void Start () {
        anim = GetComponent<Animator>();

        if (anim.GetFloat(horizontalSpeedHash) != 1.0F)
            anim.SetFloat(horizontalSpeedHash, 1.0F);
    }

    //public bool CheckInBounds(GameObject otherObj)
    //{
    //    Bounds otherBounds = otherObj.GetComponent<CapsuleCollider>().bounds;
    //    bool inBounds = GetComponent<BoxCollider>().bounds.Intersects(otherBounds);
    //    return inBounds;
    //}
}
