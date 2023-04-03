using Self.Architecture.IOC;

namespace Self.Story
{
	public static class StoryModule
	{
		public static void Init(Container container)
		{
			container.Add<StoryManager>();
			container.Add<StoryController>();
			
			
		}
	}
}