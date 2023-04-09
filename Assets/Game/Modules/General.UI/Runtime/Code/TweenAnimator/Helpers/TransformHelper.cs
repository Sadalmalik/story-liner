// Source code by Kaleb Sadalmalik
// Link: https://github.com/Sadalmalik/DoTweenAnimator

using UnityEngine;

namespace Kaleb.TweenAnimator
{
	public static class TransformHelper
	{
#region Getters

		public static FloatGetter GetTransformFloatGetter(
			TransformDataType type,
			Transform         target,
			bool              local)
		{
			FloatGetter getter = null;

			if (local)
			{
				switch (type)
				{
					case TransformDataType.PositionX:
						getter = () => target.localPosition.x;
						break;
					case TransformDataType.PositionY:
						getter = () => target.localPosition.y;
						break;
					case TransformDataType.PositionZ:
						getter = () => target.localPosition.z;
						break;

					case TransformDataType.RotationX:
						getter = () => target.localRotation.eulerAngles.x;
						break;
					case TransformDataType.RotationY:
						getter = () => target.localRotation.eulerAngles.y;
						break;
					case TransformDataType.RotationZ:
						getter = () => target.localRotation.eulerAngles.z;
						break;
				}
			}
			else
			{
				switch (type)
				{
					case TransformDataType.PositionX:
						getter = () => target.position.x;
						break;
					case TransformDataType.PositionY:
						getter = () => target.position.y;
						break;
					case TransformDataType.PositionZ:
						getter = () => target.position.z;
						break;

					case TransformDataType.RotationX:
						getter = () => target.rotation.eulerAngles.x;
						break;
					case TransformDataType.RotationY:
						getter = () => target.rotation.eulerAngles.y;
						break;
					case TransformDataType.RotationZ:
						getter = () => target.rotation.eulerAngles.z;
						break;
				}
			}

			switch (type)
			{
				case TransformDataType.ScaleX:
					getter = () => target.localScale.x;
					break;
				case TransformDataType.ScaleY:
					getter = () => target.localScale.y;
					break;
				case TransformDataType.ScaleZ:
					getter = () => target.localScale.z;
					break;
			}

			return getter;
		}
		

		public static Vector3Getter GetTransformVectorGetter(
			TransformDataType type,
			Transform         target,
			bool              local)
		{
			Vector3Getter getter = null;

			switch (type)
			{
				case TransformDataType.Position:
					if (local)
						getter = () => target.localPosition;
					else
						getter = () => target.position;
					break;

				case TransformDataType.Rotation:
					if (local)
						getter = () => target.localRotation.eulerAngles;
					else
						getter = () => target.rotation.eulerAngles;
					break;

				case TransformDataType.Scale:
					getter = () => target.localScale;
					break;
			}

			return getter;
		}

		public static FloatGetter GetRectTransformFloatGetter(
			RectTransformDataType type,
			RectTransform         target,
			bool                  local)
		{
			FloatGetter getter = null;

			switch (type)
			{
				case RectTransformDataType.RectSizeX:
					getter = () => target.sizeDelta.x;
					break;
				case RectTransformDataType.RectSizeY:
					getter = () => target.sizeDelta.y;
					break;
				case RectTransformDataType.RectPivotX:
					getter = () => target.pivot.x;
					break;
				case RectTransformDataType.RectPivotY:
					getter = () => target.pivot.y;
					break;
				case RectTransformDataType.RectAnchorMinX:
					getter = () => target.anchorMin.x;
					break;
				case RectTransformDataType.RectAnchorMinY:
					getter = () => target.anchorMin.y;
					break;
				case RectTransformDataType.RectAnchorMaxX:
					getter = () => target.anchorMax.x;
					break;
				case RectTransformDataType.RectAnchorMaxY:
					getter = () => target.anchorMax.y;
					break;
			}

			if (getter == null)
				getter = GetTransformFloatGetter((TransformDataType) type, target, local);

			return getter;
		}

		public static Vector2Getter GetRectTransformVector2Getter(
			RectTransformDataType type,
			RectTransform         target,
			bool                  local)
		{
			Vector2Getter getter = null;

			switch (type)
			{
				case RectTransformDataType.RectSize:
					getter = () => target.sizeDelta;
					break;
				case RectTransformDataType.RectPivot:
					getter = () => target.pivot;
					break;
				case RectTransformDataType.RectAnchorMin:
					getter = () => target.anchorMin;
					break;
				case RectTransformDataType.RectAnchorMax:
					getter = () => target.anchorMax;
					break;
			}

			return getter;
		}
		

#endregion


#region Setters

		public static FloatSetter GetTransformFloatSetter(
			TransformDataType type,
			Transform         target,
			bool              local)
		{
			FloatSetter setter = null;

			if (local)
			{
				switch (type)
				{
					case TransformDataType.PositionX:
						setter = value => target.localPosition = target.localPosition.SetX(value);
						break;
					case TransformDataType.PositionY:
						setter = value => target.localPosition = target.localPosition.SetY(value);
						break;
					case TransformDataType.PositionZ:
						setter = value => target.localPosition = target.localPosition.SetZ(value);
						break;

					case TransformDataType.RotationX:
						setter = value =>
							target.localRotation = Quaternion.Euler(target.localRotation.eulerAngles.SetX(value));
						break;
					case TransformDataType.RotationY:
						setter = value =>
							target.localRotation = Quaternion.Euler(target.localRotation.eulerAngles.SetY(value));
						break;
					case TransformDataType.RotationZ:
						setter = value =>
							target.localRotation = Quaternion.Euler(target.localRotation.eulerAngles.SetZ(value));
						break;
				}
			}
			else
			{
				switch (type)
				{
					case TransformDataType.PositionX:
						setter = value => target.position = target.position.SetX(value);
						break;
					case TransformDataType.PositionY:
						setter = value => target.position = target.position.SetY(value);
						break;
					case TransformDataType.PositionZ:
						setter = value => target.position = target.position.SetZ(value);
						break;

					case TransformDataType.RotationX:
						setter = value =>
							target.rotation = Quaternion.Euler(target.rotation.eulerAngles.SetX(value));
						break;
					case TransformDataType.RotationY:
						setter = value =>
							target.rotation = Quaternion.Euler(target.rotation.eulerAngles.SetY(value));
						break;
					case TransformDataType.RotationZ:
						setter = value =>
							target.rotation = Quaternion.Euler(target.rotation.eulerAngles.SetZ(value));
						break;
				}
			}

			switch (type)
			{
				case TransformDataType.ScaleX:
					setter = value => target.localScale = target.localScale.SetX(value);
					break;
				case TransformDataType.ScaleY:
					setter = value => target.localScale = target.localScale.SetY(value);
					break;
				case TransformDataType.ScaleZ:
					setter = value => target.localScale = target.localScale.SetZ(value);
					break;
			}

			return setter;
		}

		public static Vector3Setter GetTransformVectorSetter(
			TransformDataType type,
			Transform         target,
			bool              local)
		{
			Vector3Setter setter = null;

			switch (type)
			{
				case TransformDataType.Position:
					if (local)
						setter = value => target.localPosition = value;
					else
						setter = value => target.position = value;
					break;

				case TransformDataType.Rotation:
					if (local)
						setter = value => target.localRotation = Quaternion.Euler(value);
					else
						setter = value => target.rotation = Quaternion.Euler(value);
					break;

				case TransformDataType.Scale:
					setter = value => target.localScale = value;
					break;
			}

			return setter;
		}

		public static FloatSetter GetRectTransformFloatSetter(
			RectTransformDataType type,
			RectTransform         target,
			bool                  local)
		{
			FloatSetter setter = null;

			switch (type)
			{
				case RectTransformDataType.RectSizeX:
					setter = value => target.sizeDelta = target.sizeDelta.SetX(value);
					break;
				case RectTransformDataType.RectSizeY:
					setter = value => target.sizeDelta = target.sizeDelta.SetY(value);
					break;
				case RectTransformDataType.RectPivotX:
					setter = value => target.pivot = target.pivot.SetX(value);
					break;
				case RectTransformDataType.RectPivotY:
					setter = value => target.pivot = target.pivot.SetY(value);
					break;
				case RectTransformDataType.RectAnchorMinX:
					setter = value => target.anchorMin = target.anchorMin.SetX(value);
					break;
				case RectTransformDataType.RectAnchorMinY:
					setter = value => target.anchorMin = target.anchorMin.SetY(value);
					break;
				case RectTransformDataType.RectAnchorMaxX:
					setter = value => target.anchorMax = target.anchorMax.SetX(value);
					break;
				case RectTransformDataType.RectAnchorMaxY:
					setter = value => target.anchorMax = target.anchorMax.SetY(value);
					break;
			}

			if (setter == null)
				setter = GetTransformFloatSetter((TransformDataType) type, target, local);

			return setter;
		}

		public static Vector2Setter GetRectTransformVector2Setter(
			RectTransformDataType type,
			RectTransform         target,
			bool                  local)
		{
			Vector2Setter setter = null;

			switch (type)
			{
				case RectTransformDataType.RectSize:
					setter = value => target.sizeDelta = value;
					break;
				case RectTransformDataType.RectPivot:
					setter = value => target.pivot = value;
					break;
				case RectTransformDataType.RectAnchorMin:
					setter = value => target.anchorMin = value;
					break;
				case RectTransformDataType.RectAnchorMax:
					setter = value => target.anchorMax = value;
					break;
			}

			return setter;
		}
		
#endregion
	}
}