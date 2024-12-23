using System.Collections.Concurrent;

public class Part_22_Service
{
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private bool _istTest = false;

    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        string[] testString = [
            "123",
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
                var originalSecretNumber = long.Parse(line);
                var t = Task.Run(() =>
                {
                    var secretNumber = originalSecretNumber;
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

        _runnerService.WithTest(_istTest).RunWithLogging(GetType().Name, () =>
        {
            var tasks = new List<Task>();
            var priceChangeList = new ConcurrentDictionary<long, List<PriceChangePair>>();
            var sequenceList = new ConcurrentDictionary<long, List<Sequence>>();
            foreach (var line in inputLists)
            {
                var originalSecretNumber = long.Parse(line);
                var t = Task.Run(() =>
                {
                    var priceChangeListLocal = new List<PriceChangePair>();
                    var sequenceListLocal = new List<Sequence>();
                    var secretNumber = originalSecretNumber;
                    var priceChangePair = new PriceChangePair(GetPrice(originalSecretNumber), 0);
                    var lastFourPriceChangePair = new PriceChangePair[4];
                    for (int i = 0; i < 2000; i++)
                    {
                        secretNumber = GenerateSecretNumber(secretNumber);
                        priceChangePair = GetPriceChangePair(secretNumber, priceChangePair);
                        priceChangeListLocal.Add(priceChangePair);
                        if (i < 3) continue;
                        for (int i2 = 0; i2 < 4; i2++)
                        {
                            lastFourPriceChangePair[3 - i2] = priceChangeListLocal.ElementAt(i - i2);
                        }
                        sequenceListLocal.Add(new Sequence
                        {
                            OriginSecretNumber = originalSecretNumber,
                            SequenceIndexStart = i,
                            SequenceList = lastFourPriceChangePair
                        });
                    }
                    var wasAdded = priceChangeList.TryAdd(originalSecretNumber, priceChangeListLocal);
                    if (!wasAdded)
                    {
                        Console.WriteLine($"SecretNumber already in dictionary.  {originalSecretNumber}");
                    }

                    var wasAdded2 = sequenceList.TryAdd(originalSecretNumber, sequenceListLocal);
                    if (!wasAdded)
                    {
                        Console.WriteLine($"SecretNumber already in dictionary.  {originalSecretNumber}");
                    }

                });
                tasks.Add(t);
            }
            Task.WhenAll(tasks).Wait();

            return 0;
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


    #region Part 2 stuff
    class Sequence
    {
        public required long OriginSecretNumber { get; init; } // originating secretnumber
        public required int SequenceIndexStart { get; init; } // where in the number of secretnumber was this found
        public required IEnumerable<PriceChangePair> SequenceList { get; init; } // sequence indexed [0,1,2,3] where 0 is the beginning of the sequence
        public string Key => string.Join(",", SequenceList.Select(s => s.Change.ToString())); // string.join(",",each number in the sequence)
        public List<int> ChangeSequence => SequenceList.Select(t => t.Change).ToList();
        public int Price => SequenceList.Last().Price;

    }
    public record PriceChangePair(int Price, int Change);

    public PriceChangePair GetPriceChangePair(long secretNumber, PriceChangePair previousChangePair)
    {
        var curPrice = GetPrice(secretNumber);
        return new PriceChangePair(curPrice, curPrice - previousChangePair.Price);
    }

    public int GetPrice(long secretNumber)
    {
        return int.Parse(secretNumber.ToString().Last().ToString());
    }


    #endregion
}
