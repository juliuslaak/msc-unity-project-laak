using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour {

    public GameObject ball;
    public GameObject ExperimentPlane;

    [SerializeField] MainController mainController;

    public int gridHorizontalSize;
    public int gridVerticalSize;
    public float gridDistance;
    public float gridHorizontalSpace;
    public float gridVerticalSpace;
    [HideInInspector] public float gridTopLeftHorizontal;
    [HideInInspector] public float gridTopLeftVertical;

    private int oscillateHash;
    private int cycleOffsetHash = Animator.StringToHash("cycleOffset");

    public List<VibrantBall> balls;

    void Start()
    {
        ExperimentPlane.transform.position = new Vector3(ExperimentPlane.transform.position.x, ExperimentPlane.transform.position.y, gridDistance);

        if (mainController.task != 2)
        {
            oscillateHash = Animator.StringToHash("OscillateHorizontal");
        }
        else
        {
            oscillateHash = Animator.StringToHash("OscillateVertical");
        }
    }

    public List<VibrantBall> MakeGrid()
    {
        balls = new List<VibrantBall>();
        int row = 0;
        int col = 0;

        // Compute the position of the topleft corner of the grid
        findTopCorner(true, gridHorizontalSize, gridVerticalSize, gridHorizontalSpace, gridVerticalSpace, out gridTopLeftHorizontal, out gridTopLeftVertical);

        for (int i = 0; i < gridHorizontalSize * gridVerticalSize; i++, col++)
        {
            // Add new line
            if (i != 0 && i % gridHorizontalSize == 0)
            {
                row++;
                col = 0;
            }

            float yOffset = UnityEngine.Random.Range(-1 * (gridVerticalSpace / 3), (gridVerticalSpace / 3));
            float xOffset = UnityEngine.Random.Range(-1 * (gridHorizontalSpace / 3), (gridHorizontalSpace / 3));
            //float yOffset = UnityEngine.Random.Range(-1 * (gridVerticalSpace / 2 - ball.transform.localScale.y / 2), (gridVerticalSpace / 2 - ball.transform.localScale.y / 2));

            Vector3 ballPosition = new Vector3(
                gridTopLeftHorizontal + col * gridHorizontalSpace + xOffset,
                gridTopLeftVertical + row * gridVerticalSpace + yOffset,
                0);

            GameObject newBall = (GameObject)Instantiate(ball, ballPosition, Quaternion.identity);
            newBall.transform.SetParent(ExperimentPlane.transform, false);
            VibrantBall newVibrantBall = new VibrantBall(i, newBall, ballPosition, yOffset);
            balls.Add(newVibrantBall);
        }

        return balls;
    }

    public void DeleteGrid()
    {
        foreach (VibrantBall b in balls)
        {
            Destroy(b.ball);
        }
    }

    public void startOscillation()
    {
        foreach (VibrantBall b in balls)
        {
            Animator ballAnimator = b.ball.GetComponent<Animator>();
            ballAnimator.SetFloat(cycleOffsetHash, UnityEngine.Random.Range(0F, 1F));
            ballAnimator.SetTrigger(oscillateHash);
        }
    }

    public void stopOscillation()
    {
        foreach (VibrantBall b in balls)
        {
            b.ball.GetComponent<Animator>().ResetTrigger(oscillateHash);
        }
    }

    private void findTopCorner(bool left, int gridHorizontalSize, int gridVerticalSize, float gridHorizontalSpace, float gridVerticalSpace,
        out float gridTopHorizontal, out float gridTopVertical)
    {
        int ballsToTheLeft = gridHorizontalSize / 2;
        int ballsToTheTop = gridVerticalSize / 2;

        int shift;

        if (left)
            shift = -1;
        else
            shift = 1;

        gridTopHorizontal = shift * (ballsToTheLeft * gridHorizontalSpace - gridHorizontalSpace / 2 * (1 - gridHorizontalSize % 2));
        gridTopVertical = shift * (ballsToTheTop * gridVerticalSpace - gridVerticalSpace / 2 * (1 - gridVerticalSize % 2));
    }


}
