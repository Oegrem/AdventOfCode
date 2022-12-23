namespace Common.Algorithm;
public class BFS
{
	public IBFSObject Root;

	public Queue<IBFSObject> Queue;

	public BFS(IBFSObject root)
	{
		Root = root;
		Queue = new Queue<IBFSObject>();
	}

	public IBFSObject Search()
	{
		Root.IsExplored = true;
		Queue.Enqueue(Root);
		while (Queue.Any())
		{
			var v = Queue.Dequeue();

			if (v.IsGoal) return v;

			foreach (var w in v.GetNext())
			{
				if (!w.IsExplored)
				{
					w.ExecuteLogic();
					w.IsExplored = true;
					w.Parent = v;
					Queue.Enqueue(w);
				}
			}
		}
		return Root;
	}
}

public interface IBFSObject
{
	public bool IsExplored { get; set; }

	public bool IsGoal { get; set; }

	public IBFSObject? Parent { get; set; }

	public void ExecuteLogic();

	public List<IBFSObject> GetNext();

}