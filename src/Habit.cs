namespace HabitTracker;

public class Habit
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? Quantity { get; set; }
    public DateTime? Date { get; set; }

    public Habit(int? id = null, string? name = null, int? quantity = null, DateTime? date = null)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Date = date; 
    }

    public override string ToString()
    {
        return $"Habit n°{Id} : {Name}, {Quantity} times per week{(Date == null ? "" : $", due {Date}")}";
    }
}