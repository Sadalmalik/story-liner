using System;
using Self.Architecture.IOC;

namespace Self.Story
{
	public class NodeConditionController : SharedObject, INodeController
	{
		public Type GetTargetType() => typeof(ConditionNode);

		public string Enter(BaseNode node)
		{
			var cond = node as ConditionNode;
			
			if (cond == null) return node.NextNode;
			
			var condition = cond.LoadCondition();
			if (condition == null) return node.NextNode;
			
			var result = condition.Evaluate(container);
			
			return result ? node.nextNodes[0] : node.nextNodes[1];
		}
	}
}