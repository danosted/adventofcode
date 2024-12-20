using System.Diagnostics;

public class RunnerService
{
    private bool _isTest { get; set; } = false;
    public RunnerService WithTest(bool isTest = false)
    {
        _isTest = isTest;
        return this;
    }

    public void RunWithLogging(string title, Func<int> methodDelegate)
    {
        var watch = new Stopwatch();
        title = _isTest ? $"TESTRUN_{title}" : title;
        Console.WriteLine($"Starting {title}");
        watch.Start();
        var result = methodDelegate();
        watch.Stop();
        Console.WriteLine($"Completed {title}: {result}");
        Console.WriteLine($"runtime: {watch.ElapsedMilliseconds}ms");

    }
}