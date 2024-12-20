using System.Diagnostics;
using System.Text.RegularExpressions;

public class Part_1_Service
{
    private NumberDistanceService _numberService = Activator.CreateInstance<NumberDistanceService>();
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private IEnumerable<int> _firstColumn = [];
    private IEnumerable<int> _secondColumn = [];
    private Stopwatch _watch = new Stopwatch();
    private bool _istTest = false;

    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        List<string> testStringList = [
        "3   4",
        "4   3",
        "2   5",
        "1   3",
        "3   9",
        "3   3",
        ];

        var inputLists = _istTest ? testStringList : FilerLoaderService.GetInputFile(AocTask.GetPart(1));

        var regexPairs = """\d+""";
        var inputMatchPairs = inputLists.Select(row => Regex.Matches(row, regexPairs));

        _firstColumn = inputMatchPairs.Select(p => int.Parse(p.First().Value)).Order().ToList();
        _secondColumn = inputMatchPairs.Select(p => int.Parse(p.Last().Value)).Order().ToList();


        Console.WriteLine($"firstColumns Rows: {_firstColumn.Count()}");
        Console.WriteLine($"secondColumns Rows: {_secondColumn.Count()}");


        _runnerService.WithTest(_istTest).RunWithLogging("Part 1_1", () =>
        {
            return _numberService.GetDistanceSum(_firstColumn, _secondColumn);
        });
        _runnerService.WithTest(_istTest).RunWithLogging("Part 1_2", () =>
        {
            return _numberService.GetSimilarityScore(_firstColumn, _secondColumn);
        });
    }

}