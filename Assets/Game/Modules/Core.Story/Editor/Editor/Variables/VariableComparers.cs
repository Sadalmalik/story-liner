using System.Collections.Generic;
using UnityEditor;

namespace Self.Story.Editors
{
	// The idea is that you only compare values of the same type
	// so everything should always be sorted by type beforehand

	public class ComparerById : IComparer<SerializedProperty>
	{
		public static int CompareById(SerializedProperty x, SerializedProperty y)
		{
			var idPropX = x.objectReferenceValue as Variable;
			var idpropY = y.objectReferenceValue as Variable;

			return idPropX.id.CompareTo(idpropY.id);
		}

		public int Compare(SerializedProperty x, SerializedProperty y)
		{
			var idPropX = x.objectReferenceValue as Variable;
			var idpropY = y.objectReferenceValue as Variable;

			return idPropX.id.CompareTo(idpropY.id);
		}
	}

	public class ComparerByValue : IComparer<SerializedProperty>
	{
		public int Compare(SerializedProperty x, SerializedProperty y)
		{
			int comparison = 0;

			if (x.objectReferenceValue is IntVariable intVar1)
			{
				var value1 = intVar1.value;
				var value2 = (y.objectReferenceValue as IntVariable).value;

				comparison = value1.CompareTo(value2);
			}
			else if (x.objectReferenceValue is FloatVariable floatVar1)
			{
				var value1 = floatVar1.value;
				var value2 = (y.objectReferenceValue as FloatVariable).value;

				comparison = value1.CompareTo(value2);
			}
			else if (x.objectReferenceValue is BoolVariable boolVar1)
			{
				var value1 = boolVar1.value;
				var value2 = (y.objectReferenceValue as BoolVariable).value;

				comparison = value1.CompareTo(value2);
			}
			else if (x.objectReferenceValue is StringVariable stringVar1)
			{
				var value1 = stringVar1.value;
				var value2 = (y.objectReferenceValue as StringVariable).value;

				comparison = value1.CompareTo(value2);
			}

			if (comparison == 0)
			{
				return ((Variable) x.objectReferenceValue).id.CompareTo(((Variable) y.objectReferenceValue).id);
			}

			return comparison;
		}
	}

	public class ComparerByMinValue : IComparer<SerializedProperty>
	{
		public int Compare(SerializedProperty x, SerializedProperty y)
		{
			var comparison = 0;

			if (x.objectReferenceValue is IntVariable intVar1)
			{
				var value1 = intVar1.minValue;
				var value2 = (y.objectReferenceValue as IntVariable).minValue;

				comparison = value1.CompareTo(value2);
			}
			else if (x.objectReferenceValue is FloatVariable floatVar1)
			{
				var value1 = floatVar1.minValue;
				var value2 = (y.objectReferenceValue as FloatVariable).minValue;

				comparison = value1.CompareTo(value2);
			}

			if (comparison == 0)
			{
				return ((Variable) x.objectReferenceValue).id.CompareTo(((Variable) y.objectReferenceValue).id);
			}

			return comparison;
		}
	}

	public class ComparerByMaxValue : IComparer<SerializedProperty>
	{
		public int Compare(SerializedProperty x, SerializedProperty y)
		{
			var comparison = 0;

			if (x.objectReferenceValue is IntVariable intVar1)
			{
				var value1 = intVar1.maxValue;
				var value2 = (y.objectReferenceValue as IntVariable).maxValue;

				comparison = value1.CompareTo(value2);
			}
			else if (x.objectReferenceValue is FloatVariable floatVar1)
			{
				var value1 = floatVar1.maxValue;
				var value2 = (y.objectReferenceValue as FloatVariable).maxValue;

				comparison = value1.CompareTo(value2);
			}

			if (comparison == 0)
			{
				return ((Variable) x.objectReferenceValue).id.CompareTo(((Variable) y.objectReferenceValue).id);
			}

			return comparison;
		}
	}

	public class ComparerByType : IComparer<SerializedProperty>
	{
		public int Compare(SerializedProperty x, SerializedProperty y)
		{
			var value1 = x.objectReferenceValue.GetType().ToString();
			var value2 = y.objectReferenceValue.GetType().ToString();

			return value1.CompareTo(value2);
		}
	}
}