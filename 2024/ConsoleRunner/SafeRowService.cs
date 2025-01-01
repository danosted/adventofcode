public class SafeRowService
{
    private int _min { get; set; } = 1;
    private int _max { get; set; } = 3;
    private int? _ruleSkipsAllowed { get; set; }

    enum Direction
    {
        Increasing,
        Decreasing,
        Equal
    }
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

    private bool IsFollowingRules(int prevNum, int curNum, Direction direction)
    {
        // Must be different
        if (prevNum == curNum) return false;

        // Must be within boundaries
        var diff = prevNum > curNum ? prevNum - curNum : curNum - prevNum;
        if (diff < _min || diff > _max) return false;

        // Must follow direction
        return GetDirection(prevNum, curNum) == direction;
    }

    private bool TryGetDirection(int prevNum, int curNum, out Direction? direction)
    {
        direction = null;
        if (prevNum == curNum) return false;
        direction = GetDirection(prevNum, curNum);
        return true;
    }

    private Direction GetDirection(int prevNum, int curNum)
    {
        if (prevNum == curNum) throw new InvalidOperationException("invalid");
        if (curNum > prevNum)
        {
            return Direction.Increasing;
        }
        else
        {
            return Direction.Decreasing;
        }
    }


    public bool IsSafeRow(IEnumerable<int> row)
    {

        if (_ruleSkipsAllowed is null) return IsSafeRowInternal(row);
        var curIndex = 0;
        var maxIndex = row.Count();
        var isSafe = false;
        while (!isSafe && curIndex < maxIndex)
        {
            var modifiedRow = row.ToList();
            modifiedRow.RemoveAt(curIndex);
            isSafe = IsSafeRowInternal(modifiedRow);
            curIndex++;
        }
        return isSafe;

    }

    private bool IsSafeRowInternal(IEnumerable<int> row)
    {
        int prevNum = 0;
        Direction? direction = null;
        // State = 1 (normal)
        // State = 2 (index checking)
        var _state = 1;
        foreach (var curNum in row)
        {
            if (_state == 1)
            {
                prevNum = curNum;
                _state = 2;
                continue;
            }
            if (_state == 2)
            {
                if (!TryGetDirection(prevNum, curNum, out var dir))
                {
                    return false;
                }
                direction = dir;
                _state = 3;
            }
            if (_state == 3)
            {
                if (IsFollowingRules(prevNum, curNum, direction.GetValueOrDefault()))
                {
                    prevNum = curNum;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }


    // AOC2024_2_1
    // public bool IsSafeRow(IEnumerable<int> row)
    // {
    //     int? prevNum = null;
    //     bool? isDecreasing = null;
    //     int rulesBroken = 0;
    //     bool hasBrokenRule = false;
    //     var recheckDecreasing = false;
    //     var isFirst = true;

    //     foreach (var num in row)
    //     {
    //         if (prevNum is null)
    //         {
    //             prevNum = num;
    //             continue;
    //         }
    //         if (isDecreasing is null || recheckDecreasing)
    //         {
    //             isDecreasing = prevNum > num;
    //             if (recheckDecreasing) recheckDecreasing = false;
    //         }

    //         if (prevNum == num) hasBrokenRule = true;

    //         // If the direction is no longer the same
    //         if (!IsMatchingDirection(isDecreasing.Value, prevNum > num)) hasBrokenRule = true;

    //         if (!IsWithBoundaries(isDecreasing.Value ? prevNum.Value - num : num - prevNum.Value)) hasBrokenRule = true;

    //         if (hasBrokenRule)
    //         {
    //             rulesBroken++;
    //             recheckDecreasing = isFirst;
    //         }

    //         if (rulesBroken > _ruleSkipsAllowed) return false;

    //         prevNum = hasBrokenRule ? prevNum : num;

    //         hasBrokenRule = false;
    //         isFirst = false;

    //     }
    //     return true;
    // }


}