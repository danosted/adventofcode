var numberFinderServce = new NumberFinderService();

var inputStrings = File.ReadAllLines("aoc2023.txt");

// inputStrings= [
// "two1nine",
// "eightwothree",
// "abcone2threexyz",
// "xtwone3four",
// "4nineeightseven2",
// "zoneight234",
// "7pqrstsixteen",
// ];

var sum = 0;
foreach (var input in inputStrings)
{
    var result = numberFinderServce.GetFirstAndLastNumberAsNumber2(input);
    Console.WriteLine($"'{input}'={result}");
    sum += result;
}

Console.WriteLine(sum);