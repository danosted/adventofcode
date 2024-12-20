public class AocTask
{
    public string Filename;
    private AocTask(string filename)
    {
        Filename = filename;
    }
    public static AocTask Part_1 = new("input_1.txt");
    public static AocTask Part_2 = new("input_2.txt");

}