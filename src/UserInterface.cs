namespace HabitTracker;

public static class UserInterface
{
    private static bool _isSpeechRecognitionActivated = false;
    
    public static void Intro()
    {
        Console.WriteLine("Welcome to my Habit Tracker app !");
    }
    public static void GoodBye()
    {
        Console.WriteLine("Good bye !");
    }

    public static void SpeechToggle()
    {
        Console.WriteLine($"Currently speech recognition is {(_isSpeechRecognitionActivated? "on" : "off")}");
    }
    
    public static async Task<UserOption> AskUserOption()
    {
        return await AskUser.Ask<UserOption>(
        ["Choose which operation you want to do:",
        "\tI - Insert a new habit",
        "\tD - Delete an existing habit",
        "\tU - Update an existing habit",
        "\tV - View your habits",
        "\tS - Toggle speech control",
        "\tQ - Leave the app"], 
            _isSpeechRecognitionActivated);
    }

    public static async Task AskInsert()
    {
        string name = await AskUser.Ask<string>(["Enter the name of the Habit you want to create"], _isSpeechRecognitionActivated);
        int quantity = await AskUser.Ask<int>(["Enter the amount of times this Habit should be done each week"], _isSpeechRecognitionActivated);
        DateTime? date = await AskUser.AskOptional<DateTime>(["Enter the date (dd/mm/yyyy) this Habit is due, nothing for today's date"], _isSpeechRecognitionActivated);

        Habit habit = new Habit(name: name, quantity: quantity, date: date);
        bool? confirm = await AskUser.AskOptional<bool>([$"You are trying to insert {habit}","Enter False to dismiss it or nothing if you want to confirm"], _isSpeechRecognitionActivated);

        // We consider True and null as a confirmation
        if (confirm ?? true)
        {
            DataBaseInterface.InsertHabit(habit);
        }
    }

    public static async Task AskDelete()
    {
        int? index = await AskUser.AskOptional<int>(["Which habit to you want to delete ? Enter the index of the habit. Nothing to cancel"], _isSpeechRecognitionActivated);
        if (index == null) return;
        try
        {
            // If the index exist in db, it returns only 1
            Habit habit = DataBaseInterface.ViewHabits(id: index)[0];
            bool? confirm = await AskUser.AskOptional<bool>([$"You are trying to delete {habit}","Enter False to dismiss the deletion or nothing if you want to confirm"], _isSpeechRecognitionActivated);
            if ((confirm ?? true) && habit.Id.HasValue)
            {
                DataBaseInterface.DeleteHabit(habit.Id!.Value);
            }
        }
        catch (IndexOutOfRangeException)
        {
            // Means the index didn't exist
            Console.WriteLine($"The {index} Habit doesn't exist");
        }
    }

    public static async Task AskUpdate()
    {
        int? index = await AskUser.AskOptional<int>(["Which habit to you want to update ? Enter the index of the habit. Nothing to cancel"], _isSpeechRecognitionActivated);
        if (index == null) return;
        Habit oldHabit;
        try
        {
            oldHabit = DataBaseInterface.ViewHabits(id: index)[0];
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine($"The {index} Habit doesn't exist");
            return;
        }
        
        Console.WriteLine($"You are trying to update {oldHabit}");
        
        string? newName = await AskUser.AskOptional([$"Enter the new name of the Habit, or nothing to keep {oldHabit.Name}"], _isSpeechRecognitionActivated);
        int? newQuantity = await AskUser.AskOptional<int>([$"Enter the new amount of times this Habit should be done each week, or nothing to keep {oldHabit.Quantity}"], _isSpeechRecognitionActivated);
        DateTime? newDate = await AskUser.AskOptional<DateTime>([$"Enter the new date (YYYY/mm/dd HH:mm:ss) this Habit is due, nothing to keep {oldHabit.Date}"], _isSpeechRecognitionActivated);

        Habit newHabit = new Habit(
            oldHabit.Id, 
            newName ?? oldHabit.Name, 
            newQuantity ?? oldHabit.Quantity,
            newDate ?? oldHabit.Date);
        
        bool? confirm = await AskUser.AskOptional<bool>(
            [$"This is the updated Habit: {newHabit}", 
                "Enter False to dismiss the update or nothing if you want to confirm"], _isSpeechRecognitionActivated);
        if ((confirm ?? true))
        {
            DataBaseInterface.UpdateHabit(newHabit);
        }
        
    }

    public static async Task AskView()
    {
        Console.WriteLine("For the next filed, enter nothing to not filter by the corresponding parameter");
        int? index = await AskUser.AskOptional<int>(["Enter the index of the habit you want to see"], _isSpeechRecognitionActivated);
        string? name = await AskUser.AskOptional([$"Enter the name of the habit you want to see"], _isSpeechRecognitionActivated);
        DateTime? startDate = await AskUser.AskOptional<DateTime>([$"Enter the date (YYYY/mm/dd HH:mm:ss) that are due AFTER it"], _isSpeechRecognitionActivated);
        DateTime? endDate = await AskUser.AskOptional<DateTime>([$"Enter the date (YYYY/mm/dd HH:mm:ss) that are due BEFORE it"], _isSpeechRecognitionActivated);
        
        Console.WriteLine("This is the list of Habits you wanted to see:");
        Console.WriteLine(string.Join("\n", DataBaseInterface.ViewHabits(index, name, startDate, endDate)));
    }

    public static void ToggleSpeech()
    {
        if (ConfigSettings.IsConfigValid())
        {
            _isSpeechRecognitionActivated = !_isSpeechRecognitionActivated;
        }
        else
        {
            Console.WriteLine("The configuration is not right, can't activate speech. See README for more info");
        }
    }
}