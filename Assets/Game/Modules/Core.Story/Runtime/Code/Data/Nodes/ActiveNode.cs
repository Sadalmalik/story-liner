using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Self.Story
{
	/// <summary> 
	/// Acts as an Actions Container. 
	/// Only executes a list of actions and continues the story
	/// </summary>
	[NodeMetadata]
	public class ActiveNode : BaseNode
	{
		[SerializedChild]
		[FormerlySerializedAs("behaviours")]
		public List<BaseAction> actions;
	}
}