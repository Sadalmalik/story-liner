// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kaleb.TweenAnimator
{
	public enum ColorDataType
	{
		Color = 0b00001,
		ColorR = 0b00010,
		ColorG = 0b00100,
		ColorB = 0b01000,
		ColorA = 0b10000
	}

	public enum TransformDataType
	{
		//				= 0b00000000_00000000

		Position = 0b00000000_00000001,
		PositionX = 0b00000000_00000010,
		PositionY = 0b00000000_00000100,
		PositionZ = 0b00000000_00001000,

		Rotation = 0b00000000_00010000,
		RotationX = 0b00000000_00100000,
		RotationY = 0b00000000_01000000,
		RotationZ = 0b00000000_10000000,

		Scale = 0b00000001_00000000,
		ScaleX = 0b00000010_00000000,
		ScaleY = 0b00000100_00000000,
		ScaleZ = 0b00001000_00000000,

		Vectors = 0b00000001_00010001,
		Floats = 0b00001110_11101110
	}

	public enum RectTransformDataType
	{
		//				= 0b00000000_00000000_00000000_00000000

		Position = 0b00000000_00000000_00000000_00000001,
		PositionX = 0b00000000_00000000_00000000_00000010,
		PositionY = 0b00000000_00000000_00000000_00000100,
		PositionZ = 0b00000000_00000000_00000000_00001000,

		Rotation = 0b00000000_00000000_00000000_00010000,
		RotationX = 0b00000000_00000000_00000000_00100000,
		RotationY = 0b00000000_00000000_00000000_01000000,
		RotationZ = 0b00000000_00000000_00000000_10000000,

		Scale = 0b00000000_00000000_00000001_00000000,
		ScaleX = 0b00000000_00000000_00000010_00000000,
		ScaleY = 0b00000000_00000000_00000100_00000000,
		ScaleZ = 0b00000000_00000000_00001000_00000000,


		RectSize = 0b00000000_00000000_00010000_00000000,
		RectSizeX = 0b00000000_00000000_00100000_00000000,
		RectSizeY = 0b00000000_00000000_01000000_00000000,

		RectPivot = 0b00000000_00000001_00000000_00000000,
		RectPivotX = 0b00000000_00000010_00000000_00000000,
		RectPivotY = 0b00000000_00000100_00000000_00000000,

		RectAnchorMin = 0b00000000_00010000_00000000_00000000,
		RectAnchorMinX = 0b00000000_00100000_00000000_00000000,
		RectAnchorMinY = 0b00000000_01000000_00000000_00000000,

		RectAnchorMax = 0b00000001_00000000_00000000_00000000,
		RectAnchorMaxX = 0b00000010_00000000_00000000_00000000,
		RectAnchorMaxY = 0b00000100_00000000_00000000_00000000,

		Vectors1 = 0b00000110_01100110_01101110_11101110,
		Vectors2 = 0b00000001_00010001_00010000_00000000,
		Vectors3 = 0b00000000_00000000_00000001_00010001,
	}

	public static class DataTypesExtensions
	{
		public static bool IsColor(this ColorDataType type)
		{
			return type == ColorDataType.Color;
		}


		public static bool IsVector1(this TransformDataType type)
		{
			return type == (type & TransformDataType.Floats);
		}

		public static bool IsVector3(this TransformDataType type)
		{
			return type == (type & TransformDataType.Vectors);
		}


		public static bool IsVector1(this RectTransformDataType type)
		{
			return type == (type & RectTransformDataType.Vectors1);
		}

		public static bool IsVector2(this RectTransformDataType type)
		{
			return type == (type & RectTransformDataType.Vectors2);
		}

		public static bool IsVector3(this RectTransformDataType type)
		{
			return type == (type & RectTransformDataType.Vectors3);
		}

#if UNITY_EDITOR
		[MenuItem("Tools/Tween Animator/Check types")]
		public static void CheckFunctions()
		{
			ColorDataType ct = 0;
			ct.IsColor();

			TransformDataType td = 0;
			td.IsVector1();
			td.IsVector3();

			RectTransformDataType rtd = 0;
			rtd.IsVector1();
			rtd.IsVector2();
			rtd.IsVector3();
		}
#endif
	}
}