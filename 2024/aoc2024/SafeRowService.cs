using System.Collections.Frozen;

public class SafeRowService
{
    private int _min { get; set; } = 1;
    private int _max { get; set; } = 3;
    private int _ruleSkipsAllowed { get; set; } = 0;
    public SafeRowService WithMin(int min)
    {
        _min = min;
        return this;
    }

    public SafeRowService WithMax(int max)
    {
        _max = max;
        return this;
    }

    public SafeRowService WithRuleSkips(int skips)
    {
        _ruleSkipsAllowed = skips;
        return this;
    }

    // AOC2024_2_1
    public bool IsSafeRow(IEnumerable<int> row)
    {
        int? lastNum = null;
        bool? isDecreasing = null;
        int rulesBroken = 0;
        bool hasBrokenRule = false;
        var recheckDecreasing = false;
        var isFirst = true;

        foreach (var num in row)
        {
            if (lastNum is null)
            {
                lastNum = num;
                continue;
            }
            if (isDecreasing is null || recheckDecreasing)
            {
                isDecreasing = lastNum > num;
                if (recheckDecreasing) recheckDecreasing = false;
            }

            if (lastNum == num) hasBrokenRule = true;

            // If the direction is no longer the same
            if (!IsMatchingDirection(isDecreasing.Value, lastNum > num)) hasBrokenRule = true;

            if (!IsWithBoundaries(isDecreasing.Value ? lastNum.Value - num : num - lastNum.Value)) hasBrokenRule = true;

            if (hasBrokenRule)
            {
                rulesBroken++;
                recheckDecreasing = isFirst;
            }

            if (rulesBroken > _ruleSkipsAllowed) return false;

            lastNum = hasBrokenRule ? lastNum : num;

            hasBrokenRule = false;
            isFirst = false;

        }
        return true;
    }

    private bool IsWithBoundaries(int num)
    {
        return num >= _min && num <= _max;
    }
    private bool IsMatchingDirection(bool isDecreasing, bool isDecreasingCheck)
    {
        return isDecreasing == isDecreasingCheck;
    }

}