using System.Collections.Generic;

namespace Self.StoryV2
{
	public class ChapterSave
	{
		public HashSet<string> ReachedNodes = new HashSet<string>();

		public string currentNode;
		public string autosaveNode;
	}
}