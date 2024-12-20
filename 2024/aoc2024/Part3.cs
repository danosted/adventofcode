using System.Text.RegularExpressions;

public class Part_3_Service
{
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private bool _istTest = false;

    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        string[] testString = ["xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"];

        var inputLists = testRun ? testString : FilerLoaderService.GetInputFile(AocTask.GetPart(3));

        var regexPairs = """\d+""";
        var inputRowMatches = inputLists.Select(row => Regex.Matches(row, regexPairs).Select(p => int.Parse(p.Value)));


        _runnerService.WithTest(_istTest).RunWithLogging(GetType().Name, () =>
        {
            return inputRowMatches.Where(row => _taskService.IsSafeRow(row)).Count();
        });

    }
}