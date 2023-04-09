using System.Collections.Generic;
using Self.Architecture.IOC;

namespace Self.Story
{
	public abstract class Condition
	{
		public abstract bool Evaluate(Container context);
	}

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

	public abstract class VariableCondition : Condition
	{
		public string variableName;
	}

	public abstract class StringCondition : VariableCondition
	{
		public string expectedValue;

		public override bool Evaluate(Container context)
		{
			var controller = context.Get<StoryController>();

			var value = controller.CurrentChapter.book.variables.GetValue(variableName);

			return value.Equals(expectedValue);
		}
	}
	
	public abstract class IntCondition : VariableCondition
	{
		public enum Op
		{
			Equal,
			Less,
			LessOrEqual,
			Greater,
			GreaterOrEqual,
			NotEqual
		}

		public Op  op;
		public int expectedValue;

		public override bool Evaluate(Container context)
		{
			var controller = context.Get<StoryController>();

			var variable = controller.CurrentChapter.book.variables.Get(variableName) as IntVariable;
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
	public abstract class BoolCondition : VariableCondition
	{
		public enum Op
		{
			Equal,
			NotEqual
		}

		public Op  op;
		public bool expectedValue;

		public override bool Evaluate(Container context)
		{
			var controller = context.Get<StoryController>();

			var variable = controller.CurrentChapter.book.variables.Get(variableName) as BoolVariable;
			if (variable == null)
				return false;
			
			switch (op)
			{
				case Op.Equal:          return variable.value == expectedValue;
				case Op.NotEqual:       return variable.value != expectedValue;
			}

			return false;
		}
	}


	public class ConditionNode : BaseNode
	{
		public Condition condition;
	}
}