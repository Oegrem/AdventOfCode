using System.Net;
using System.Net.Http;

namespace AdventOfCode2022.InputHelper;

public class InputHandler
{
	static readonly HttpClient client = new();

	public static async Task<string> GetStringFromUrl(string url)
	{
		var responseString = String.Empty;
		try
		{
			using HttpResponseMessage response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			responseString = await response.Content.ReadAsStringAsync();

		}
		catch (HttpRequestException e)
		{
			Console.WriteLine($"Exception while calling url: {url}");
			Console.WriteLine($"Message: {e.Message}");
		}
		return responseString;
	}

	public static async Task<string> GetStringFromFile(string path)
	{
		return await File.ReadAllTextAsync(path);
	}

	public static async Task<string> GetStringFromFile(string folderName, string fileName)
	{
		string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
		string sFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\" + @"Completed\" + folderName + @"\" + fileName);
		string sFilePath = Path.GetFullPath(sFile);

		if (!File.Exists(sFilePath))
		{
			sFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\" + folderName + @"\" + fileName);
			sFilePath = Path.GetFullPath(sFile);
		}

		return await File.ReadAllTextAsync(sFilePath);
	}

	public static async Task<string> GetStringFromInputFile(string folderName)
	{
		return await GetStringFromFile(folderName, "input.txt");
	}

	public static async Task<string> GetStringFromSample(string folderName)
	{
		return await GetStringFromFile(folderName, "sample.txt");
	}

	public static async Task<string> GetStringFromSampleSmall(string folderName)
	{
		return await GetStringFromFile(folderName, "sampleSmall.txt");
	}

	public static async Task<string> GetStringFromFileName(string folderName, string fileName)
	{
		return await GetStringFromFile(folderName, fileName);
	}

	public static string[] GetStringLines(string input, bool trim = true)
	{
		if(trim)
			input = input.Trim();

		return input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
	}

}
