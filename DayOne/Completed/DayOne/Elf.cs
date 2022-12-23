namespace Puzzles.Completed.DayOne;
internal class Elf : IComparable<Elf>
{
    public int Id { get; set; }
    public List<int> Calories { get; set; }

    public int CalorySum => Calories.Sum();

    public Elf(int id)
    {
        Id = id;
        Calories = new List<int>();
    }

    public void AddCalory(int calory)
    {
        Calories.Add(calory);
    }

    public override string ToString()
    {
        return $"Elf: {Id}; Calories: {CalorySum}";
    }

    public int CompareTo(Elf? other)
    {
        if (other == null)
        {
            return 0;
        }
        return other.CalorySum - CalorySum;
    }
}
