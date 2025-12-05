using System.Text.RegularExpressions;
using Avalonia.Controls;

class Rules
{
    public static bool ruleRequiredForTextBox(string content)
    {
        return string.IsNullOrWhiteSpace(content);
    }

    public static bool ruleRequiredForComboBox(ComboBox comboBox, string defaultValue)
    {
        return comboBox.SelectedItem == null || comboBox.SelectedItem.ToString() == defaultValue;
    }

    public static bool rulePhone(string content)
    {
        return !string.IsNullOrWhiteSpace(content) && !Regex.IsMatch(content, @"^\d{10,11}$");
    }

    public static bool ruleEmail(string content)
    {
        return !string.IsNullOrWhiteSpace(content) && !Regex.IsMatch(content, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
  

   public static bool IsNumeric(string input)
{
    return !int.TryParse(input, out _);
}
}