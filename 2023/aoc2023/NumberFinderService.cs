using System.Text.RegularExpressions;

public class NumberFinderService
{
    public int GetFirstAndLastNumberAsNumber(string input)
    {
        var regex = """([1-9])""";
        var matches = Regex.Matches(input, regex);
        return GetNumber(matches);
    }

    private int GetNumber(MatchCollection matches)
    {
        var first = matches.First().Value;
        var last = matches.Last().Value;
        var combined = $"{GetNumberAsText(first)}{GetNumberAsText(last)}";
        return int.Parse(combined);

        string GetNumberAsText(string t)
        {
            if (int.TryParse(t, out var n))
            {
                return t;
            }
            if (numberStrings.ContainsKey(t))
            {
                return numberStrings[t];
            }
            throw new InvalidOperationException($"{t} could not be converted to number.");
        }
    }

    static Dictionary<string, string> numberStrings = new Dictionary<string, string>{
        {"one","1"},
        {"two","2"},
        {"three","3"},
        {"four","4"},
        {"five","5"},
        {"six","6"},
        {"seven","7"},
        {"eight","8"},
        {"nine","9"}
    };

    public int GetFirstAndLastNumberAsNumber2(string input)
    {
        var regex = """([1-9]|one|two|three|four|five|six|seven|eight|nine)""";
        var matches = Regex.Matches(input, regex, RegexOptions.IgnoreCase);
        return GetNumber(matches);
    }
}