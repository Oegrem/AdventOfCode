namespace Puzzles.Completed.DayThree;
internal class ElfGroup
{
	public List<Rucksack> Rucksacks { get; set; }
	public Item? Badge { get; set; }

	public ElfGroup()
	{
		Rucksacks = new List<Rucksack>();
		Badge = null;
	}

	public void AddRucksack(Rucksack rucksack)
	{
		Rucksacks.Add(rucksack);
	}

	public Item GetBadge()
	{
		return Rucksack.GetCommonItem(Rucksacks);
	}

}
