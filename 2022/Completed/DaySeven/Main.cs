using AdventOfCode2022.InputHelper;
using System.Xml.Linq;

namespace Puzzles.Completed.DaySeven;

public class Main
{
	const string FOLDER = "DaySeven";

	public static async Task SolvePuzzle()
	{
		var inputStrings = InputHandler.GetStringLines(await InputHandler.GetStringFromInputFile(FOLDER));

		var fileSystem = new FileSystem();

		foreach (var inputString in inputStrings)
		{
			fileSystem.ParseOutputString(inputString);
		}
		
		Console.WriteLine(fileSystem.BaseFolder.GetSize());

		var combinedDirSize = fileSystem.GetAllFolders().Where(x => x.GetSize() <= 100000).Sum(x => x.GetSize());
		Console.WriteLine(combinedDirSize);

		var neededSpace = 30000000;

		var deleteSize = neededSpace - fileSystem.UnusedSpace;
		var deletionSize = fileSystem.GetAllFolders().Where(x => x.GetSize() >= deleteSize).Min(x => x.GetSize());

		Console.WriteLine(deletionSize);
	}
}

public class FileSystem
{
	public int Space = 70000000;

	public Folder BaseFolder = new("/");

	public Folder CurrentFolder;

	public int UsedSpace => BaseFolder.GetSize();

	public int UnusedSpace => Space - UsedSpace;

	public FileSystem()
	{
		CurrentFolder = BaseFolder;
	}

	public void ParseOutputString(string output)
	{
		// Command
		if(output.StartsWith('$')) 
		{
			ExecuteCommand(output[1..].Trim());
		}
		// Data
		else
		{
			ReadData(output);
		}
	}

	public void ReadData(string data)
	{
		if(data.StartsWith("dir"))
		{
			var folderName = data.Split(' ')[1];
			AddFolder(folderName);
		} else
		{
			var fileData = data.Split(' ');

			AddFile(fileData[1], int.Parse(fileData[0]));
		}
	}

	public void ExecuteCommand(string command)
	{
		if(command == "ls" || !command.StartsWith("cd"))
			return;
		
		var targetFolder = command.Split(' ')[1];
		switch(targetFolder)
		{
			case "/":
				GoToBaseFolder();
				break;
			case "..":
				GoToParenFolder();
				break;
			default:
				GoToFolderByName(targetFolder);
				break;
		}		
	}

	public void AddFolder(string name)
	{
		CurrentFolder.AddFolder(name);
	}

	public void AddFile(string name, int size)
	{
		CurrentFolder.AddFile(name, size);
	}

	public void GoToFolderByName(string name)
	{
		var targetFolder = CurrentFolder.GetDirectFolderByName(name);
		if (targetFolder != null)
			CurrentFolder = targetFolder;
	}

	public void GoToBaseFolder()
	{
		CurrentFolder = BaseFolder;
	}

	public void GoToParenFolder()
	{ 
		if(CurrentFolder.ParentFolder != null)
			CurrentFolder = CurrentFolder.ParentFolder; 
	}

	public List<Folder> GetAllFolders()
	{
		return BaseFolder.GetAllFolders();
	}
}

public class Folder
{
	public Folder? ParentFolder;

	public List<Folder> SubFolders = new();

	public List<File> Files = new();

	public string Name;

	public bool IsBaseFolder;

	public Folder(string name)
	{
		IsBaseFolder = true;
		Name = name;
	}

	public Folder(Folder parentFolder, string name)
	{
		IsBaseFolder = false;
		ParentFolder = parentFolder;
		Name = name;
	}

	public Folder? GetDirectFolderByName(string name)
	{
		return SubFolders.FirstOrDefault(x => x.Name == name);
	}

	public Folder AddFolder(string name)
	{
		var newFolder = new Folder(this, name);
		SubFolders.Add(newFolder);
		return newFolder;
	}

	public File AddFile(string name, int size)
	{
		var newFile = new File(name, size);
		Files.Add(newFile);
		return newFile;
	}

	public int GetSize()
	{
		var directFileSize = Files.Sum(x => x.Size);
		var indirectFileSize = SubFolders.Sum(x => x.GetSize());

		return directFileSize + indirectFileSize;
	}

	public List<Folder> GetAllFolders()
	{
		var newList = new List<Folder>();
		newList.Add(this);
		newList.AddRange(SubFolders.SelectMany(x => x.GetAllFolders()).ToList());
		return newList;
	}

	public override string ToString()
	{
		return $"{Name}";
	}
}

public class File
{
	public int Size = 0;

	public string Name;

	public File(string name, int size)
	{
		Name = name;
		Size = size;
	}
}