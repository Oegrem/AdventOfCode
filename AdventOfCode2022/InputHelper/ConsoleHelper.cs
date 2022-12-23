using System.Collections.Concurrent;

namespace Common.InputHelper;
public static class ConsoleHelper
{

	public static BlockingCollection<string> m_Queue = new BlockingCollection<string>();

	static ConsoleHelper()
	{
		var thread = new Thread(
		  () =>
		  {
			  while (true) Console.Write(m_Queue.Take());
		  });
		thread.IsBackground = true;
		thread.Start();
	}

	public static void WriteLine(string line = "")
	{
		m_Queue.Add(line + '\n');
	}

	public static void WriteLine(int line)
	{
		m_Queue.Add(line.ToString() + '\n');
	}

	public static void Write(string stuff)
	{
		m_Queue.Add(stuff);
	}

	public static void Write(char stuff)
	{
		m_Queue.Add(stuff.ToString());
	}
}
