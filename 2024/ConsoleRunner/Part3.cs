using System.Collections.Specialized;
using System.Dynamic;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

public class Part_3_Service
{
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private bool _istTest = false;
    record MulClass(int v1, int v2);
    // private MulClass GetMulClassFromRegexGroup(List<string> group)
    // {

    // }

    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        string[] testString = ["xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"];

        var inputLists = testRun ? testString : FilerLoaderService.GetInputFile(AocTask.GetPart(3));

        // _runnerService.WithTest(_istTest).RunWithLogging(GetType().Name, () =>
        // {
        //     var result = GetRunPart_1_MulOperations(inputLists);
        //     var sum = result.Sum(m => m.v1 * m.v2);
        //     return sum;
        // });

        _runnerService.WithTest(_istTest).RunWithLogging(GetType().Name, () =>
        {
            var result = GetRunPart_2_MulOperations(inputLists);
            var sum = result.Sum(m => m.v1 * m.v2);
            return sum;
        });

    }

    IEnumerable<MulClass> GetRunPart_1_MulOperations(IEnumerable<string> inputLists)
    {
        var regexPairs = """(mul\(([0-9]{1,3}),([0-9]{1,3})\))""";
        var inputRowGroupDebug = inputLists.SelectMany(row => Regex.Matches(row, regexPairs).Select(p => p.Groups.Values.FirstOrDefault()));
        List<Task> tasks = [];
        foreach (var g in inputRowGroupDebug)
        {
            var t = Task.Run(() =>
            {
                Console.WriteLine(g.Value);
            });
            tasks.Add(t);
        }
        Task.WhenAll(tasks).Wait();
        Console.WriteLine($"Found {inputRowGroupDebug.Count()} mul operations.");
        var inputRowGroupMatches = inputLists.SelectMany(row => Regex.Matches(row, regexPairs).Select(p => p.Groups.Values.Where(v => !v.Value.Contains("mul", StringComparison.OrdinalIgnoreCase)))).ToList();

        Console.WriteLine($"Found {inputRowGroupMatches.Count()} mul operations filtered.");

        var hello = inputRowGroupMatches?.Select(g => new MulClass(int.Parse(g.FirstOrDefault()?.Value!), int.Parse(g.LastOrDefault()?.Value!)));
        return hello ?? [];
    }

    IEnumerable<MulClass> GetRunPart_2_MulOperations(IEnumerable<string> inputLists)
    {
        // \_.\{-}
        // Regex explanation: find the string part which is preceded by none or one do()|don't() until the next do()|dont't() or end of line
        var regexPairs = """(do\(\)|don't\(\)){0,1}(.+?)(?=do\(\)|don't\(\)|$)""";
        var matches = inputLists.SelectMany(row => Regex.Matches(row, regexPairs)).Where(m => m.Success).ToList();


        // var enabledParts = matches.Where(m => !m.Value.StartsWith("do()") && !m.Value.StartsWith("don't()")).ToList();

        // Console.WriteLine();
        // Console.WriteLine("Enabled Parts");
        // Console.WriteLine();
        // DebugMatches(enabledParts);

        // var doParts = matches.Where(m => m.Value.StartsWith("do()")).ToList();

        // Console.WriteLine();
        // Console.WriteLine("Do Parts");
        // Console.WriteLine();
        // DebugMatches(doParts);
        List<Match> enabledParts = [];
        List<Match> doParts = [];
        var state = 1;
        var isEnabled = true;
        foreach (var match in matches)
        {
            if (state == 1)
            {
                if (match.Value.StartsWith("do()") || match.Value.StartsWith("don't()"))
                {
                    state = 2;
                }
                else
                {
                    enabledParts.Add(match);
                    continue;
                }
            }
            if (state == 2)
            {
                if (match.Value.StartsWith("do()"))
                {
                    isEnabled = true;
                    doParts.Add(match);
                }
                else if (match.Value.StartsWith("don't()"))
                {
                    isEnabled = false;
                }
                else
                {
                    if (isEnabled)
                    {
                        doParts.Add(match);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        
        return GetRunPart_1_MulOperations([.. enabledParts.Select(m => m.Value), .. doParts.Select(m => m.Value)]);
    }

    void DebugMatches(IEnumerable<Match> matches)
    {
        foreach (var m in matches)
        {
            Console.WriteLine();
            Console.WriteLine(m.Index);
            Console.WriteLine(m.Value);
            Console.WriteLine();
        }
    }
}