using System.Collections.Specialized;
using System.Dynamic;
using System.Text.RegularExpressions;

public class Part_3_Service
{
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private bool _istTest = false;
    record MulClass(int v1, int v2);
    private MulClass GetMulClassFromRegexGroup(List<string> group)
    {

    }
    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        string[] testString = ["xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"];

        var inputLists = testRun ? testString : FilerLoaderService.GetInputFile(AocTask.GetPart(3));

        var regexPairs = """(mul\(([0-9]{1,3}),([0-9]{1,3})\))""";
        // var inputRowGroupDebug = inputLists.SelectMany(row => Regex.Matches(row, regexPairs).Select(p => p.Groups.Values.FirstOrDefault()));
        // foreach (var g in inputRowGroupDebug)
        // {
        //     Task.Run(() =>
        //     {
        //         Console.WriteLine(g.Value);
        //     });
        // }
        var inputRowGroupMatches = inputLists.SelectMany(row => Regex.Matches(row, regexPairs).Select(p => p.Groups.Values.Where(v => !v.Value.Contains("mul", StringComparison.OrdinalIgnoreCase))));
        var hello = inputRowGroupMatches?.Select(g => new MulClass(int.Parse(g.FirstOrDefault()?.Value!), int.Parse(g.LastOrDefault()?.Value!)));
        var sum = hello?.Sum(m => m.v1 * m.v2);


        _runnerService.WithTest(_istTest).RunWithLogging(GetType().Name, () =>
        {
            return sum.GetValueOrDefault();
        });

    }

}