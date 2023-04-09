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


	public class ConditionNode : BaseNode
	{
		public Condition condition;
	}
}