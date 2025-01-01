public static class FilerLoaderService
{
    public static IEnumerable<string> GetInputFile(AocTask task)
    {
        var inputLists = File.ReadAllLines($"Assets\\{task.Filename}");

        return inputLists;
    }
}