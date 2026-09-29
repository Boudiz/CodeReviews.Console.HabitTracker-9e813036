namespace HabitTracker;
using Microsoft.Data.Sqlite;
using CrypticWizard.RandomWordGenerator;

public static class DataBaseInterface
{

    private const string DbConnectionString = 
        "Data Source=HabitTracker.db;";

    static DataBaseInterface()
    {
        CreateTable();
        PopulateTableWithRandomData();
    }
    
    private static void CreateTable()
    {
        using var connection = new SqliteConnection(DbConnectionString);
        connection.Open();

        string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Habits (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                Date TEXT
            );";

        using var command = new SqliteCommand(createTableQuery, connection);
        command.ExecuteNonQuery();
        Console.WriteLine("Table Habits created");
    }

    public static void InsertHabit(Habit habit)
    {
        using var connection = new SqliteConnection(DbConnectionString);
        connection.Open();
        string insertQuery = "INSERT INTO Habits (Name, Quantity, Date) VALUES (@Name, @Quantity, @Date)";
        
        using var command = new SqliteCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@Name", habit.Name);
        command.Parameters.AddWithValue("@Quantity", habit.Quantity);
        command.Parameters.AddWithValue("@Date", (object?)habit.Date?.ToString("yyyy-MM-dd") ?? DBNull.Value);
        
        command.ExecuteNonQuery();
    }

    public static List<Habit> ViewHabits(int? id = null, string? name = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        var habits = new List<Habit>();
        var setClauses = new List<String>();
        using var command = new SqliteCommand();

        if (id.HasValue)
        {
            setClauses.Add("Id = @Id");
            command.Parameters.AddWithValue("@Id", id.Value);
        }
        if (!string.IsNullOrEmpty(name))
        {
            setClauses.Add("Name LIKE @Name");
            command.Parameters.AddWithValue("@Name", $"%{name}%");
        }
        if (startTime.HasValue)
        {
            setClauses.Add("Date >= @StartTime");
            command.Parameters.AddWithValue("@StartTime", startTime.Value);
        }
        if (endTime.HasValue)
        {
            setClauses.Add(("Date <= @EndTime"));
            command.Parameters.AddWithValue("@EndTime", endTime.Value);
        }
        
        string baseViewQuery = "SELECT * FROM Habits";
        command.CommandText = setClauses.Count > 0
            ? $"{baseViewQuery} WHERE {string.Join(" AND ", setClauses)}"
            : baseViewQuery;
        
        using var connection = new SqliteConnection(DbConnectionString);
        command.Connection = connection;
        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            habits.Add(new Habit(
                id: reader.GetInt32(0),
                name: reader.GetString(1), 
                quantity: reader.GetInt32(2), 
                date: reader.IsDBNull(3) ? null : reader.GetDateTime(3)));
        }
        
        return habits;
    }

    public static void UpdateHabit(Habit habit)
    {
        var setClauses = new List<String>();
        using var command = new SqliteCommand();

        if (habit.Name != null)
        {
            setClauses.Add("Name = @Name");
            command.Parameters.AddWithValue("@Name",  habit.Name);
        }
        if (habit.Quantity.HasValue)
        {
            setClauses.Add("Quantity = @Quantity");
            command.Parameters.AddWithValue("@Quantity", habit.Quantity.Value);
        }
        if (habit.Date.HasValue)
        {
            setClauses.Add("Date = @Date");
            command.Parameters.AddWithValue("@Date", habit.Date.Value);
        }
        
        if (setClauses.Count == 0) return;
        
        string updateQuery = $@"
            UPDATE Habits
            SET {string.Join(", ", setClauses)}
            WHERE Id = @Id";
        command.CommandText = updateQuery;
        
        using var connection = new SqliteConnection(DbConnectionString);
        command.Connection = connection;
        connection.Open();

        command.Parameters.AddWithValue("@Id", habit.Id);

        command.ExecuteNonQuery();
    }

    public static void DeleteHabit(int habitId)
    {
        using var connection = new SqliteConnection(DbConnectionString);
        connection.Open();
        string deleteQuery = "DELETE FROM Habits WHERE Id = @Id";

        using var command = new SqliteCommand(deleteQuery, connection);
        command.Parameters.AddWithValue("@Id", habitId);

        command.ExecuteNonQuery();
    }

    private static void PopulateTableWithRandomData()
    {
        int amountToAdd = 100 - ViewHabits().Count;
        if (amountToAdd < 1) return;
        
        DateTime?[] randomDate = [null, DateTime.Now, DateTime.MaxValue, DateTime.UnixEpoch, DateTime.Parse("12/25/2025")];
        var numberGenerator = new Random();
        var wordGenerator = new WordGenerator();
        for (var i = 0; i < amountToAdd; i++)
        {
            InsertHabit(new Habit(
                name: wordGenerator.GetWord(), 
                quantity: numberGenerator.Next(100),
                date: randomDate[numberGenerator.Next(randomDate.Length)]));
        }

    }
}