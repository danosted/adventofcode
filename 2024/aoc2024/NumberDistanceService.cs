using System.Collections.Frozen;

public class NumberDistanceService
{
    // AOC2024_1_1
    public int GetDistanceSum(IEnumerable<int> list1, IEnumerable<int> list2)
    {
        var distSum = 0;
        for (int i = 0; i < list1.Count(); i++)
        {
            var n1 = list1.ElementAt(i);
            var n2 = list2.ElementAt(i);
            var dist = n1 > n2 ? n1 - n2 : n2 - n1;
            distSum += dist;
        }
        return distSum;
    }

    // AOC2024_1_2
    public int GetSimilarityScore(IEnumerable<int> list1, IEnumerable<int> list2)
    {
        var similarityScore = list1.Sum(i => GetScore(i));
        return similarityScore;

        int GetScore(int inputFromList1)
        {
            var count = list2.Count(i => i == inputFromList1);
            return inputFromList1 * count;
        }
    }
}