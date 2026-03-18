# Expense Tracker

A simple command-line expense tracker built with C# and .NET 8. Expenses are stored locally in a CSV file.

## Features

- Add, update, and delete expenses
- List all recorded expenses
- View total spending summary
- View monthly expenses breakdown for the current year

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Running

```bash
dotnet run --project ExpenseTracker
```

## Usage

After launching, select an option from the menu:

```
1. Add Expense          – Record a new expense (description, amount, date)
2. Update Expense       – Edit an existing expense
3. Delete Expense       – Remove an expense
4. List Expenses        – Show all expenses
5. View Summary         – Show total amount spent
6. View Monthly Expenses – Show spending totals per month for the current year
99. Exit
```

Dates must be entered in `yyyy-MM-dd` format (e.g. `2026-03-17`).

## Data Storage

Expenses are saved in `ExpenseTracker/expenses.csv`:

```
Description,Amount,Date
Lunch,15,2026-03-17
```

https://roadmap.sh/projects/expense-tracker