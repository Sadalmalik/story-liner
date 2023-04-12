using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Self.Architecture.IOC;
using UnityEngine;

namespace Self.Story
{
	public enum Op
	{
		Equal,
		Less,
		LessOrEqual,
		Greater,
		GreaterOrEqual,
		NotEqual,
		Value
	}
	
	public static class OpExtensions
	{
		public static readonly Dictionary<string, Op> ExpressionToOperation = new Dictionary<string, Op>()
		{
			{"==", Op.Equal},
			{"<", Op.Less},
			{"<=", Op.LessOrEqual},
			{">", Op.Greater},
			{">=", Op.GreaterOrEqual},
			{"!=", Op.NotEqual}
		};
	}
	[Serializable]
	public abstract class Condition
	{
		public abstract bool Evaluate(Container context);
	}

	[Serializable]
	public class AndCondition : Condition
	{
		public List<Condition> childs;

		public override bool Evaluate(Container context)
		{
			foreach (var child in childs)
				if (!child.Evaluate(context))
					return false;
			return true;
		}
	}

	[Serializable]
	public class OrCondition : Condition
	{
		public List<Condition> childs;

		public override bool Evaluate(Container context)
		{
			foreach (var child in childs)
				if (child.Evaluate(context))
					return true;
			return false;
		}
	}

	[Serializable]
	public abstract class VariableCondition : Condition
	{
		public string variableId;
	}

	[Serializable]
	public class StringCondition : VariableCondition
	{
		public string expectedValue;

		public override bool Evaluate(Container context)
		{
			var controller = context.Get<StoryController>();

			var value = controller.CurrentChapter.book.variables.GetValue(variableId);

			return value.Equals(expectedValue);
		}
	}
	
	[Serializable]
	public class IntCondition : VariableCondition
	{
		public Op  op;
		public int expectedValue;

		public override bool Evaluate(Container context)
		{
			var controller = context.Get<StoryController>();

			var variable = controller.CurrentChapter.book.variables.Get(variableId) as IntVariable;
			if (variable == null)
				return false;
			
			switch (op)
			{
				case Op.Equal:          return variable.value == expectedValue;
				case Op.Less:           return variable.value < expectedValue;
				case Op.LessOrEqual:    return variable.value <= expectedValue;
				case Op.Greater:        return variable.value > expectedValue;
				case Op.GreaterOrEqual: return variable.value >= expectedValue;
				case Op.NotEqual:       return variable.value != expectedValue;
			}

			return false;
		}
	}
	
	[Serializable]
	public class BoolCondition : VariableCondition
	{
		public Op  op;
		public bool expectedValue;

		public override bool Evaluate(Container context)
		{
			var controller = context.Get<StoryController>();

			var variable = controller.CurrentChapter.book.variables.Get(variableId) as BoolVariable;
			if (variable == null)
				return false;
			
			switch (op)
			{
				case Op.Equal:    return variable.value == expectedValue;
				case Op.NotEqual: return variable.value != expectedValue;
				case Op.Value:    return variable.value;
			}

			return false;
		}
	}

	[NodeMetadata(customOutput: true)]
	public class ConditionNode : BaseNode
	{
		private static readonly Regex _conditionReg =
			new Regex(@"(?<variable>[\w\.]*) *(?<expression>\>=|\<=|==|\!=) *(?:(?<value>true|false|\+{0,1}\d+)|""(?<string>[^""]*)"")");

		public string    rawCondition;
		//public Condition condition;

		public Condition LoadCondition()
		{
			var cond  = (Condition)null;
			var match = _conditionReg.Match(rawCondition);
			if (match.Success)
			{
				var _var = match.Groups["variable"].Value;
				var _exp = match.Groups["expression"].Value;
				var _val = match.Groups["value"].Value;
				var _str = match.Groups["string"].Value;

				if (int.TryParse(_val, out var intValue))
				{
					cond = new IntCondition
					{
						variableId    = _var,
						op            = OpExtensions.ExpressionToOperation[_exp],
						expectedValue = intValue
					};
				}
				else if (bool.TryParse(_val, out var boolValue))
				{
					cond = new BoolCondition
					{
						variableId    = _var,
						op            = OpExtensions.ExpressionToOperation[_exp],
						expectedValue = boolValue
					};
				}
				else if (!string.IsNullOrEmpty(_str))
				{
					cond = new StringCondition()
					{
						variableId    = _var,
						expectedValue = _str
					};
				}
			}
			else
			{
				Debug.LogWarning($"Can't parse condition:\n{rawCondition}");
			}

			return cond;
		}
	}
}