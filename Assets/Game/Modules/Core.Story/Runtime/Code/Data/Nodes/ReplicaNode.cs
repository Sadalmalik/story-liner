﻿using Self.Story;
using UnityEngine;

namespace Self.Story
{
	/// <summary>
	/// A node that contains only character dialogue
	/// </summary>
	[NodeMetadata]
	public class ReplicaNode : ActiveNode
	{
		public Character character;
		[SerializeReference] public EmotionReference   emotion;

		public string localized;
	}
}