using System.Collections.Concurrent;
using System.Diagnostics;

public class Part_22_Service
{
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private bool _istTest = false;

    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        string[] testString = [
            // "123",
            "1",
            "2",
            "3",
            "2024",
// "8945015",
// "12837429",
// "9210031",
// "1866170",
// "13626549",
// "5759740",
// "8110456",
// "16550935",
// "15494709",
// "8840159",
// "4366810",
// "11646429",
// "16054706",
// "1318639",
// "6286402",
// "6293852",
// "9250312",
// "12437685",
// "13525420",
// "15715428",
// "232907",
// "7437620",
// "470598",
// "8469997",
// "8556311",
// "11203336",
// "2942972",
// "14194947",
// "3397980",
// "13887710",
// "280416",
// "3436954",
// "14716460",
// "10310831",
// "5282448",
// "14551259",
// "3054496",
// "13344449",
// "3546404",
// "3059012",
// "9526934",
// "5670448",
// "15622777",
// "238626",
// "11827453",
// "7220002",
// "1840308",
// "11455420",
// "8684421",
// "6337853",
// "10937948",
// "6049030",
// "6503030",
// "904437",
// "2351439",
// "9688737",
// "4051453",
// "2647402",
// "8888347",
// "4413091",
// "3149576",
// "12745822",
// "299487",
// "16102077",
// "6217203",
// "3796857",
// "10387429",
// "7677002",
// "11773775",
// "2186133",
// "16724778",
// "14779635",
// "16207036",
// "7425348",
// "7377445",
// "9092856",
// "4010846",
// "5542622",
// "14931960",
// "7917127",
// "9844318",
// "12849555",
// "446795",
// "16333879",
// "7596569",
// "5013959",
// "6833121",
// "2425615",
// "11261672",
// "224484",
// "9301996",
// "12152637",
// "1822963",
// "10733897",
// "10342654",
// "1529556",
// "13457517",
// "2281853",
// "6454070",
// "5122713",
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
            // var tasks = new List<Task>();
            // var priceChangeList = new ConcurrentDictionary<long, List<PriceChangePair>>();
            // var sequenceDict = new ConcurrentDictionary<long, List<Sequence>>();
            // var sequenceListShared = new ConcurrentBag<Sequence>();
            var sequenceDictionaryShared = new ConcurrentDictionary<string, List<Sequence>>();
            // foreach (var line in inputLists)
            var counter = 0;
            Parallel.ForEach(inputLists, line =>
            {
                var sw = new Stopwatch();

                var originalSecretNumber = long.Parse(line);
                // var t = Task.Run(() =>
                // {
                var priceChangeListLocal = new List<PriceChangePair>();
                var sequenceListLocal = new Dictionary<string, Sequence>();
                var secretNumber = originalSecretNumber;
                var priceChangePair = new PriceChangePair(GetPrice(originalSecretNumber), 0);
                sw.Start();
                for (int i = 0; i < 2000; i++)
                {
                    var innerSw = new Stopwatch();
                    innerSw.Start();
                    var lastFourPriceChangePair = new PriceChangePair?[4];
                    secretNumber = GenerateSecretNumber(secretNumber);
                    priceChangePair = GetPriceChangePair(secretNumber, priceChangePair);
                    priceChangeListLocal.Add(priceChangePair);

                    // TIMER
                    var one = innerSw.Elapsed.Seconds;
                    innerSw.Restart();

                    if (i < 3) continue;
                    for (int i2 = 0; i2 < 4; i2++)
                    {
                        lastFourPriceChangePair[i2] = priceChangeListLocal.ElementAt(i - i2);
                    }

                    // TIMER
                    var two = innerSw.Elapsed.Seconds;
                    innerSw.Restart();
                    var sequence = new Sequence
                    {
                        OriginSecretNumber = originalSecretNumber,
                        SecretNumber = secretNumber,
                        SequenceIndexStart = i,
                        SequenceList = lastFourPriceChangePair,
                        Price = priceChangePair.Price
                    };

                    if (!sequenceListLocal.TryAdd(sequence.Key, sequence))
                    {
                        lock (this)
                        {
                            if (sequenceListLocal[sequence.Key].Price < sequence.Price)
                            {
                                sequenceListLocal[sequence.Key] = sequence;
                            }
                        }
                    }
                    // TIMER
                    var three = innerSw.Elapsed.Seconds;
                    innerSw.Restart();
                    if (one + two + three > 10)
                    {
                        Console.WriteLine($"inner loop => 1: {one}. 2: {two}. 3: {three}");
                    }
                }

                var generatedTime = sw.Elapsed;
                sw.Restart();
                // Console.WriteLine($"{originalSecretNumber} => Finished Generating");
                foreach (var sequence in sequenceListLocal)
                {
                    if (!sequenceDictionaryShared.TryAdd(sequence.Key, [sequence.Value]))
                    {
                        sequenceDictionaryShared[sequence.Key].Add(sequence.Value);
                    }
                    // sequenceListShared.Add(sequence);
                }
                sw.Stop();
                var syncTime = sw.Elapsed;
                var c = Interlocked.Increment(ref counter);
                // Console.WriteLine($"{originalSecretNumber} => Finished Synchronizing. Counter: {c}. GTime: {generatedTime.Seconds}. STime: {syncTime.Seconds}");

                // var wasAdded = priceChangeList.TryAdd(originalSecretNumber, priceChangeListLocal);
                // if (!wasAdded)
                // {
                //     Console.WriteLine($"SecretNumber already in dictionary.  {originalSecretNumber}");
                // }

                // var wasAdded2 = sequenceDict.TryAdd(originalSecretNumber, sequenceListLocal);
                // if (!wasAdded)
                // {
                //     Console.WriteLine($"SecretNumber already in dictionary.  {originalSecretNumber}");
                // }

                // });
                // tasks.Add(t);
            });
            // Task.WhenAll(tasks).Wait();

            Console.WriteLine("Done preparing data.");

            // sequence, list<sequence>
            // var keyGroups = sequenceListShared.GroupBy(s => s.Key);
            var highestSum = 0;
            var sequenceHighestSum = string.Empty;
            var highestSet = new Dictionary<long, int>();
            Parallel.ForEach(sequenceDictionaryShared, entry =>
            {
                var priceBySecretNumber = new Dictionary<long, int>();
                foreach (var sequence in entry.Value)
                {
                    if (!priceBySecretNumber.TryAdd(sequence.OriginSecretNumber, sequence.Price))
                    {
                        Console.WriteLine($"Duplicate. {sequence.OriginSecretNumber} => {sequence.Price}");
                        if (priceBySecretNumber[sequence.OriginSecretNumber] < sequence.Price)
                        {
                            priceBySecretNumber.Add(sequence.OriginSecretNumber, sequence.Price);
                        }
                    }

                }
                var myGroupSum = priceBySecretNumber.Sum(d => d.Value);
                
                if (myGroupSum > highestSum)
                {
                    highestSum = myGroupSum;
                    sequenceHighestSum = entry.Key;
                    highestSet = priceBySecretNumber;
                }
            });

            return $"{sequenceHighestSum} => {highestSum} ({highestSet.Values.Count()} buyers)";
        });
    }

    static long GenerateSecretNumber(long secretNumber)
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
        public required long SecretNumber { get; init; } // secret number for sequence
        public required int SequenceIndexStart { get; init; } // where in the number of secretnumber was this found
        public required int Price { get; init; }
        public required IEnumerable<PriceChangePair?> SequenceList { get; init; } // sequence indexed [0,1,2,3] where 0 is the beginning of the sequence
        public string Key => SequenceList.Count() != 4 ? string.Empty : string.Join(",", SequenceList.Reverse().Select(s => s?.Change.ToString())); // string.join(",",each number in the sequence)
        public List<int?> ChangeSequence => SequenceList.Select(t => t?.Change).ToList();

    }
    public record PriceChangePair(int Price, int Change);

    static PriceChangePair GetPriceChangePair(long secretNumber, PriceChangePair previousChangePair)
    {
        var curPrice = GetPrice(secretNumber);
        return new PriceChangePair(curPrice, curPrice - previousChangePair.Price);
    }

    static int GetPrice(long secretNumber)
    {
        return int.Parse(secretNumber.ToString().Last().ToString());
    }


    #endregion
}
