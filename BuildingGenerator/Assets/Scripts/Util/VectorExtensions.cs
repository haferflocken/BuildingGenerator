using UnityEngine;
using System.Collections;

public static class VectorExtensions
{
	public static bool Approximately(this Vector2 vect, Vector2 other)
	{
		return vect.Approximately(ref other);
	}

	public static bool Approximately(this Vector2 vect, ref Vector2 other)
	{
		return Mathf.Approximately(vect.x, other.x) && Mathf.Approximately(vect.y, other.y);
	}

	public static bool Approximately(this Vector3 vect, Vector3 other)
	{
		return vect.Approximately(ref other);
	}

	public static bool Approximately(this Vector3 vect, ref Vector3 other)
	{
		return Mathf.Approximately(vect.x, other.x) && Mathf.Approximately(vect.y, other.y) && Mathf.Approximately(vect.z, other.z);
	}

	public static Vector3 SwizzleX0Y(this Vector2 vect)
	{
		return new Vector3(vect.x, 0.0f, vect.y);
	}

	public static Vector2 SwizzleXZ(this Vector3 vect)
	{
		return new Vector2(vect.x, vect.z);
	}

	public static bool IsLeftOf(this Vector2 vect, ref Vector2 other)
	{
		return ((vect.x * other.y) - (vect.y * other.x)) > 0.0f;
	}
}
