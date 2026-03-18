using System.Globalization;

namespace ExpenseTracker;

internal class Program
{
    static void Main(string[] args)
    {
        string separatorCaracter = ",";
        string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
        string fileName = Path.Combine(projectDir, "expenses.csv");
        string[] csvHeader = { "Description", "Amount", "Date" };
        string csvHeaderText = string.Join(separatorCaracter, csvHeader);

        var choice = 0;

        try
        {
            if (!File.Exists(fileName))
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine(csvHeaderText);
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating the file: {ex.Message}");
            return;
        }

        while (choice != 99)
        {
            Console.WriteLine("Expense Tracker");
            Console.WriteLine("1. Add Expense");
            Console.WriteLine("2. Update Expense");
            Console.WriteLine("3. Delete Expense");
            Console.WriteLine("4. List Expenses");
            Console.WriteLine("5. View Summary");
            Console.WriteLine("6. View Monthly Expenses");
            Console.WriteLine("99. Exit");
            choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    AddExpense(fileName, separatorCaracter);
                    break;
                case 2:
                    UpdateExpense(fileName, separatorCaracter);
                    break;
                case 3:
                    DeleteExpense(fileName);
                    break;
                case 4:
                    ListExpenses(fileName);
                    break;
                case 5:
                    ViewSummary(fileName);
                    break;
                case 6:
                    ViewMonthlyExpenses(fileName);
                    break;
                case 99:
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    public static void AddExpense(string fileName, string separatorCaracter)
    {
        Console.WriteLine("Expense Description:");
        var description = Console.ReadLine();
        Console.WriteLine("Expense Amount:");
        int amount = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Expense Date (yyyy-MM-dd):");
        DateTime date = Convert.ToDateTime(Console.ReadLine());
        string expenseData = $"{description}{separatorCaracter}{amount}{separatorCaracter}{date:yyyy-MM-dd}";
        try
        {
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine(expenseData);
            }
            Console.WriteLine("Expense added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while adding the expense: {ex.Message}");
        }
    }

    public static void UpdateExpense(string fileName, string separatorCaracter)
    {
        var lines = File.ReadAllLines(fileName).ToList();

        if (lines.Count <= 1)
        {
            Console.WriteLine("No expenses to update.");
            return;
        }

        for (int i = 1; i < lines.Count; i++)
        {
            Console.WriteLine($"{i}. {lines[i]}");
        }

        Console.WriteLine("Enter the number of the expense to update:");
        int index = Convert.ToInt32(Console.ReadLine());

        if (index < 1 || index >= lines.Count)
        {
            Console.WriteLine("Invalid number.");
            return;
        }

        string[] fields = lines[index].Split(separatorCaracter);
        string currentDescription = fields[0];
        string currentAmount = fields[1];
        string currentDate = fields[2];

        Console.WriteLine($"Description ({currentDescription}):");
        string newDescription = Console.ReadLine();
        if (string.IsNullOrEmpty(newDescription)) newDescription = currentDescription;

        Console.WriteLine($"Amount ({currentAmount}):");
        string newAmountInput = Console.ReadLine();
        string newAmount = string.IsNullOrEmpty(newAmountInput) ? currentAmount : newAmountInput;

        Console.WriteLine($"Date ({currentDate}):");
        string newDateInput = Console.ReadLine();
        string newDate = string.IsNullOrEmpty(newDateInput) ? currentDate : newDateInput;

        lines[index] = $"{newDescription}{separatorCaracter}{newAmount}{separatorCaracter}{newDate}";
        File.WriteAllLines(fileName, lines);
        Console.WriteLine("Expense updated successfully.");
    }

    public static void DeleteExpense(string fileName)
    {
        var lines = File.ReadAllLines(fileName).ToList();

        if (lines.Count <= 1)
        {
            Console.WriteLine(" No expenses to delete");
            return;
        }
        for (int i = 1; i < lines.Count; i++)
        {
            Console.WriteLine($"{i}. {lines[i]}");
        }

        Console.WriteLine("Enter the number of the expense to delete:");
        int index = Convert.ToInt32(Console.ReadLine());

        if (index < 1 || index >= lines.Count)
        {
            Console.WriteLine("Invalid number.");
            return;
        }

        lines.RemoveAt(index);
        File.WriteAllLines(fileName, lines);

        Console.WriteLine("Expense successfully deleted");
    }

    public static void ListExpenses(string fileName)
    {
        using (var reader = new StreamReader(fileName))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }

    public static void ViewSummary(string fileName)
    {
        var expenseSum = File.ReadAllLines(fileName)
            .Skip(1)
            .Select(line =>
            {
                var columns = line.Split(',');

                return decimal.Parse(columns[1], CultureInfo.InvariantCulture);
            })
            .Sum();

        Console.WriteLine($"Total amount: {expenseSum}");
        
    }

    public static void ViewMonthlyExpenses(string fileName)
    {
        int currentYear = DateTime.Now.Year;

        var monthlyTotals = File.ReadAllLines(fileName)
            .Skip(1)
            .Select(line => line.Split(','))
            .Where(fields => fields.Length >= 3 && DateTime.TryParse(fields[2], out _))
            .Select(fields => new
            {
                Date = DateTime.Parse(fields[2]),
                Amount = decimal.Parse(fields[1], CultureInfo.InvariantCulture)
            })
            .Where(e => e.Date.Year == currentYear)
            .GroupBy(e => e.Date.Month)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        Console.WriteLine($"Monthly Expenses for {currentYear}:");
        for (int month = 1; month <= 12; month++)
        {
            decimal total = monthlyTotals.ContainsKey(month) ? monthlyTotals[month] : 0;
            Console.WriteLine($"{CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month)}: {total}");
        }
    }
}
