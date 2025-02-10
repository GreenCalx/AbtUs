using UnityEngine;

public class SplineWalker : MonoBehaviour {
    
    public enum SplineWalkerMode {
	    Once,
	    Loop,
	    PingPong
    }
    ///
	public BezierSpline spline;
	public SplineWalkerMode mode;

	public float duration;
    public bool lookForward;
	public float progressOffset = 0f;
    ///
	private float progress;
	private bool goingForward = true;

	void Start()
	{
		progress = progressOffset;
		Vector3 position = spline.GetPoint(progress);
		transform.position = position;
	}

	private void Update () 
    {
		if (goingForward) {
			progress += Time.deltaTime / duration;
			if (progress > 1f) {
				if (mode == SplineWalkerMode.Once) {
					progress = 1f;
				}
				else if (mode == SplineWalkerMode.Loop) {
					progress -= 1f;
				}
				else {
					progress = 2f - progress;
					goingForward = false;
				}
			}
		}
		else {
			progress -= Time.deltaTime / duration;
			if (progress < 0f) {
				progress = -progress;
				goingForward = true;
			}
		}

		Vector3 position = spline.GetPoint(progress);
		transform.position = position;
		if (lookForward) {
			transform.LookAt(position + spline.GetDirection(progress));
		}
	}
}