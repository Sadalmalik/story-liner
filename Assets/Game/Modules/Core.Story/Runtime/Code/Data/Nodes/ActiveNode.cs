using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Self.Story
{
	public class ActiveNode : BaseNode
	{
		[FormerlySerializedAs("behaviours")]
		public List<BaseAction> actions;
	}
}