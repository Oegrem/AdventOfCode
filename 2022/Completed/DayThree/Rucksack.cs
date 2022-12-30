namespace Puzzles.Completed.DayThree;
internal class Rucksack
{
	public string SerialString { get; set; }

	public List<Item> FirstCompartmentItems { get; set; }

	public List<Item> SecondCompartmentItems { get; set; }

	public Item? SharedItemType { get; set; }

	public List<Item> AllItems => FirstCompartmentItems.Concat(SecondCompartmentItems).ToList();

	public Rucksack(string inputString)
	{
		SerialString = inputString;
		FirstCompartmentItems = new List<Item>();
		SecondCompartmentItems = new List<Item>();
		SharedItemType = null;

		ParseSerialString();
	}

	public static Item GetCommonItem(List<Rucksack> rucksacks)
	{
		var intersections = rucksacks.Take(1).First().AllItems;
		foreach(var rucksack in rucksacks.Skip(1))
		{
			intersections = intersections.Intersect(rucksack.AllItems).ToList();
		}
		return intersections.First();
	}

	private void ParseSerialString()
	{
		var firstCompartmentString = SerialString[..(SerialString.Length / 2)];
		var secondCompartmentString = SerialString[(SerialString.Length / 2)..];

		FirstCompartmentItems = ParseCopartmentString(firstCompartmentString);
		SecondCompartmentItems = ParseCopartmentString(secondCompartmentString);

		SharedItemType = FirstCompartmentItems.Intersect(SecondCompartmentItems).First();
	}

	private static List<Item> ParseCopartmentString(string compartmentString)
	{
		var compartmentItems = new List<Item>();

		var itemCharsWithIndex = compartmentString.ToCharArray().Select((item, index) => (item, index));

		foreach (var (itemChar, index) in itemCharsWithIndex)
		{
			compartmentItems.Add(new Item(itemChar, index));
		}

		return compartmentItems;
	}
}

public class Item
{
	public char Identifier { get; set; }

	public int Index { get; set; }

	public int Priority { get; set; }


	public Item(char identifier, int index)
	{
		Identifier = identifier;
		Index = index;

		SetPriority();
	}

	private void SetPriority()
	{
		var isUpperCase = Char.IsUpper(Identifier);
		var asciiValue = (int)Char.ToLower(Identifier) - 96;

		Priority = isUpperCase ? asciiValue + 26 : asciiValue;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null || obj.GetType() != this.GetType())
		{
			return false;
		}

		return Identifier == ((Item)obj).Identifier;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Identifier);
	}
}
