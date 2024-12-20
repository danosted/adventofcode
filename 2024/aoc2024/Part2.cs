using System.Diagnostics;
using System.Text.RegularExpressions;

public class PartTwoService
{
    private SafeRowService _taskService = Activator.CreateInstance<SafeRowService>();
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private IEnumerable<int> _firstColumn = [];
    private IEnumerable<int> _secondColumn = [];
    private bool _istTest = false;

    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        IEnumerable<string> testStringList = [
            "7 6 4 2 1",
            "1 2 7 8 9",
            "9 7 6 2 1",
            "1 3 2 4 5",
            "8 6 4 4 1",
            "1 3 6 7 9",
        ];

        var inputLists = testRun ? testStringList : FilerLoaderService.GetInputFile(AocTask.Part_2);

        var regexPairs = """\d+""";
        var inputRowMatches = inputLists.Select(row => Regex.Matches(row, regexPairs).Select(p => int.Parse(p.Value)));


        _runnerService.WithTest(_istTest).RunWithLogging("Part 2_1", () =>
        {
            return inputRowMatches.Where(row => _taskService.IsSafeRow(row)).Count();
        });
        _runnerService.WithTest(_istTest).RunWithLogging("Part 2_2", () =>
        {
            return inputRowMatches.Where(row => _taskService.WithRuleSkips(1).IsSafeRow(row)).Count();
        });
    }
}