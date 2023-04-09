// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using UnityEngine;

namespace Kaleb.TweenAnimator
{
	public static class Extensions
	{
		//	Vector setters

		public static Vector2 SetX(this Vector2 vector, float x)
		{
			vector.x = x;
			return vector;
		}

		public static Vector2 SetY(this Vector2 vector, float y)
		{
			vector.y = y;
			return vector;
		}
		
		public static Vector3 SetX(this Vector3 vector, float x)
		{
			vector.x = x;
			return vector;
		}

		public static Vector3 SetY(this Vector3 vector, float y)
		{
			vector.y = y;
			return vector;
		}

		public static Vector3 SetZ(this Vector3 vector, float z)
		{
			vector.z = z;
			return vector;
		}

		//	Color setters

		public static Color SetR(this Color color, float r)
		{
			color.r = r;
			return color;
		}

		public static Color SetG(this Color color, float g)
		{
			color.g = g;
			return color;
		}

		public static Color SetB(this Color color, float b)
		{
			color.b = b;
			return color;
		}

		public static Color SetA(this Color color, float a)
		{
			color.a = a;
			return color;
		}
	}
}