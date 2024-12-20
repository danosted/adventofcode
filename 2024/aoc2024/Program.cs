var runNumber = args.FirstOrDefault() ?? "1";

var isTest = args.Count() == 2 && args[1] == "true";
// #if DEBUG
// Console.WriteLine("DEBUG");
// isTest = true;
// runNumber = "2";
// #endif
if (runNumber == "1")
{
    var service = Activator.CreateInstance<PartOneService>();
    service.Run(isTest);
}

if (runNumber == "2")
{
    var service = Activator.CreateInstance<PartTwoService>();
    service.Run(isTest);
}


