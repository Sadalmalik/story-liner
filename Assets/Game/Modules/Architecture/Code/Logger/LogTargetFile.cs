using System.IO;

namespace Self.Architecture.Logger
{
	public class LogTargetFile : LogTarget
	{
		private TextWriter _writer;
		
		public LogTargetFile(string path)
		{
			_writer = new StreamWriter(File.OpenWrite(path));
		}
		
		public override void LogTemp(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}

		public override void LogInfo(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}

		public override void LogWarning(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}

		public override void LogError(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}
	}
}