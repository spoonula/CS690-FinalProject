namespace InventoryApp.UI;

using Spectre.Console;

public static class PromptHelpers
{
    public static String PromptNotEmpty(String prompt)
    {
        String response = "";
        do {
            response = AnsiConsole.Ask<string>(prompt);
            if (string.IsNullOrWhiteSpace(response))
            {
                AnsiConsole.MarkupLine("[red]Your response must not be empty[/]");
            }
        } while (string.IsNullOrWhiteSpace(response));
        return response;
    }

    public static decimal PromptDecimal(String prompt, Boolean allowNegative)
    {
        decimal value;
        while (true)
        {
            String input = AnsiConsole.Ask<string>(prompt);

            if (string.IsNullOrWhiteSpace(input))
            {
                AnsiConsole.MarkupLine("[red]Your response must not be empty[/]");
                continue;
            }

            // remove curreny formatting
            String cleaned = input.Replace("$", "").Replace(",", "").Trim();

            if (!decimal.TryParse(cleaned, out value))
            {
                AnsiConsole.MarkupLine("[red]Please enter a valid number[/]");
                continue;
            }

            if (!allowNegative && value < 0)
            {
                AnsiConsole.MarkupLine("[red]Value must not be negative[/]");
                continue;
            }
            return value;
        }
    }

}
