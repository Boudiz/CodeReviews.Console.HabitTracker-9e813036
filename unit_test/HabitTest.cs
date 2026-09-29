namespace HabitTracker.Tests;

public class HabitTest
{
    [Fact]
    public void ConstructorWithSomeNullValue()
    {
        int? id = 123;
        string? name = null;
        int? quantity = null;
        DateTime? date = DateTime.Now;

        Habit habit = new Habit(id, name, quantity, date);
        
        Assert.Equal(habit.Id, id);
        Assert.Equal(habit.Name, name);
        Assert.Equal(habit.Quantity, quantity);
        Assert.Equal(habit.Date, date);
    }
    
    [Fact]
    public void ToStringMethodwithNullAndNotNullDate()
    {
        int id = 123;
        string name = "name";
        int quantity = 321;
        Habit habit = new Habit(id, name, quantity, null);

        string habitString = $"Habit n°{id} : {name}, {quantity} times per week";
        Assert.Equal(habit.ToString(), habitString);

        DateTime? date = DateTime.UnixEpoch;
        habit.Date = date;
        Assert.Equal(habit.ToString(), habitString + $", due {date}");
    }
}