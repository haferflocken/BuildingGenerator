using UnityEngine;

public static class MathUtil
{
	public static readonly float TWOPI = Mathf.PI * 2.0f;

	public static float CounterClockwiseAngle(float y, float x)
	{
		float angle = Mathf.Atan2(y, x);
		if (angle < 0.0f)
		{
			angle += TWOPI;
		}
		return angle;
	}
}
