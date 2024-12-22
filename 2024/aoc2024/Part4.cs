using System.Collections.Frozen;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

public class Part_4_Service
{
    private RunnerService _runnerService = Activator.CreateInstance<RunnerService>();
    private bool _istTest = false;

    record Dimensions(int x, int y);

    // TL T TR
    // L  i  R
    // BL B BR

    // i-w-1  i-w  i-w+1
    // i-1     i    i+1
    // i+w-1  i+w  i+w+1

    // 140 x 140

    class Cell(string l, int index)
    {
        public Letter LetterCode = GetLetterFromChar(l);
        public string LetterString = l;
        public int Index = index;

        static Letter GetLetterFromChar(string l)
        {
            if (string.Equals(l, "x", StringComparison.OrdinalIgnoreCase))
            {
                return Letter.X;
            }

            if (string.Equals(l, "m", StringComparison.OrdinalIgnoreCase))
            {
                return Letter.M;
            }

            if (string.Equals(l, "a", StringComparison.OrdinalIgnoreCase))
            {
                return Letter.A;
            }

            if (string.Equals(l, "s", StringComparison.OrdinalIgnoreCase))
            {
                return Letter.S;
            }

            throw new InvalidOperationException($"Letter not recognized. {l}");
        }
        public enum Letter
        {
            X,
            M,
            A,
            S
        }
    }

    bool IsXmasWord(List<Cell> word)
    {
        var wordResult = string.Empty;
        foreach (var str in word)
        {
            wordResult += str.LetterString;
        }

        var isXmas = wordResult.Equals("xmas", StringComparison.OrdinalIgnoreCase);
        if (isXmas)
        {
            // Console.WriteLine($"""Found xmas: {string.Join(":", word.Select(w => w.Index))}""");
        }
        return isXmas;
    }

    List<Cell> GetNextCellLetterList(int index, int width, int modifier, Cell? prevLetter, List<Cell> wordResult, List<Cell> cells)
    {
        var cell = cells.ElementAtOrDefault(index);
        if (cell is null) return wordResult;
        var nextIndex = index + width + modifier;
        if (prevLetter is null)
        {
            if (cell.LetterCode == 0)
            {
                return GetNextCellLetterList(nextIndex, width, modifier, cell, [cell], cells);
            }
            else
            {
                return wordResult;
            }
        }
        if (cell.LetterCode == prevLetter.LetterCode + 1) return GetNextCellLetterList(nextIndex, width, modifier, cell, [.. wordResult, cell], cells);
        return wordResult;
    }

    public void Run(bool testRun = false)
    {
        _istTest = testRun;

        string[] testString = [
            "MMMSXXMASM",
            "MSAMXMSMSA",
            "AMXSXMAAMM",
            "MSAMASMSMX",
            "XMASAMXAMM",
            "XXAMMXXAMA",
            "SMSMSASXSS",
            "SAXAMASAAA",
            "MAMMMXMMMM",
            "MXMXAXMASX"
        ];

        var inputLists = testRun ? testString : FilerLoaderService.GetInputFile(AocTask.GetPart(4));

        var height = inputLists.Count();
        var width = inputLists.FirstOrDefault()?.Count() ?? 0;

        // 140 x 140
        Console.WriteLine($"width: {width}. height: {height}");

        // Construct cell set
        // List<Cell> cells = inputLists.SelectMany((r, x) => r.Select((c, y) => new Cell(c.ToString()))).ToFrozenSet();
        List<Cell> cells = [];
        var index = 0;
        foreach (var row in inputLists)
        {
            foreach (var s in row)
            {
                cells.Add(new Cell(s.ToString(), index));
                index++;
            }
        }

        _runnerService.WithTest(_istTest).RunWithLogging(GetType().Name, () =>
        {
            var xmasCount = 0;
            var debug = new List<string>();
            var debugLine = string.Empty;
            for (int i = 0; i < cells.Count(); i++)
            {
                // i-w-1  i-w  i-w+1
                // i-1     i    i+1
                // i+w-1  i+w  i+w+1
                var words = new List<List<Cell>>();
                words = [
                    GetNextCellLetterList(i, -width, -1, null, [], cells),
                    GetNextCellLetterList(i, -width, 0, null, [], cells),
                    GetNextCellLetterList(i, -width, 1, null, [], cells),

                    GetNextCellLetterList(i, 0, -1, null, [], cells),
                    GetNextCellLetterList(i, 0, 1, null, [], cells),

                    GetNextCellLetterList(i, width, -1, null, [], cells),
                    GetNextCellLetterList(i, width, 0, null, [], cells),
                    GetNextCellLetterList(i, width, 1, null, [], cells)
                ];
                var xmasWords = words.Where(w => IsXmasWord(w)).ToList();
                xmasCount += xmasWords.Count;
                // debugLine += $"[{i}]";
                debugLine += $"[{xmasWords.Count}]";
                
                if (i > 0 && (i + 1) % width == 0)
                {
                    debug.Add(debugLine);
                    debugLine = string.Empty;
                }
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("./out/", "WriteLines.txt")))
            {
                foreach (string line in debug)
                    outputFile.WriteLine(line);
            }
            return xmasCount;
        });
    }




}