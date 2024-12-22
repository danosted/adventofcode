public class Part_22_Service
{
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private bool _istTest = false;

    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        string[] testString = [
            "1",
            "10",
            "100",
            "2024",
        ];

        var inputLists = testRun ? testString : FilerLoaderService.GetInputFile(AocTask.GetPart(22));

        _runnerService.WithTest(_istTest).RunWithLogging(GetType().Name, () =>
        {
            var tasks = new List<Task>();
            long sum = 0;
            foreach (var line in inputLists)
            {
                var lineValue = long.Parse(line);
                var t = Task.Run(() =>
                {
                    var secretNumber = lineValue;
                    for (int i = 0; i < 2000; i++)
                    {
                        secretNumber = GenerateSecretNumber(secretNumber);
                    }
                    Interlocked.Add(ref sum, secretNumber);
                });
                tasks.Add(t);
            }
            Task.WhenAll(tasks).Wait();
            return sum;
        });
    }

    long GenerateSecretNumber(long secretNumber)
    {
        var step_1 = secretNumber * 64;
        var mix_1 = Mix(secretNumber, step_1);
        var result_1 = Prune(mix_1);

        var (step_2, _) = long.DivRem(result_1, 32);
        var mix_2 = Mix(result_1, step_2);
        var result_2 = Prune(mix_2);

        var step_3 = result_2 * 2048;
        var mix_3 = Mix(result_2, step_3);
        var result_3 = Prune(mix_3);
        return result_3;
    }

    static long Mix(long secretNumber, long mixee)
    {
        return secretNumber ^ mixee;
    }

    static long Prune(long secretNumber)
    {
        var modResult = secretNumber % 16777216;
        return modResult;
    }
}
