using AdventOfCode2022.InputHelper;
using FluentAssertions;

namespace Tests.Common.InputHelper;

public class Tests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task GetStringFromUrlShouldReturnString()
    {
        // Arrange
        var url = "https://www.google.com";

        // Act
        var result = await InputHandler.GetStringFromUrl(url);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetStringFromFileShouldReturnTestText()
    {
		// Arrange
		string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
		string sFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\Common\InputHelper\testFile");
		string sFilePath = Path.GetFullPath(sFile);

        // Act
        var result = await InputHandler.GetStringFromFile(sFilePath);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Be("this is a test file");
    }

	[Test]
	public async Task GetStringFromInputFileShouldReturnTestText()
	{
		// Act
		var result = await InputHandler.GetStringFromInputFile("");

		// Assert
		result.Should().NotBeEmpty();
		result.Should().Be("this is a input file");
	}

	[Test]
	public void GetStringLinesReturnStringArraySplitByLines()
	{
        // Arrange
        var inputString = "This\nIs\nSome\nString";

		// Act
		var result = InputHandler.GetStringLines(inputString);

		// Assert
		result.Should().NotBeEmpty();
        result.Length.Should().Be(4);
        result.ElementAt(0).Should().Be("This");
        result.ElementAt(1).Should().Be("Is");
        result.ElementAt(2).Should().Be("Some");
        result.ElementAt(3).Should().Be("String");
	}
}