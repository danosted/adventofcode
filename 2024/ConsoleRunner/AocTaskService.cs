public class AocTask
{
    public string Filename;
    private AocTask(string filename)
    {
        Filename = filename;
    }

    public static AocTask GetPart(int partNum) => new ($"input_{partNum}.txt");

}