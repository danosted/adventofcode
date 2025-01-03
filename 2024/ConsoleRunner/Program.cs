﻿var runNumber = args.FirstOrDefault() ?? "1";

var isTest = args.Count() == 2 && args[1] == "test";
// #if DEBUG
// Console.WriteLine("DEBUG");
// isTest = true;
// runNumber = "22";
// #endif
if (runNumber == "1")
{
    var service = Activator.CreateInstance<Part_1_Service>();
    service.Run(isTest);
}

if (runNumber == "2")
{
    var service = Activator.CreateInstance<Part_2_Service>();
    service.Run(isTest);
}

if (runNumber == "3")
{
    var service = Activator.CreateInstance<Part_3_Service>();
    service.Run(isTest);
}

if (runNumber == "4")
{
    var service = Activator.CreateInstance<Part_4_Service>();
    service.Run(isTest);
}

if (runNumber == "22")
{
    var service = Activator.CreateInstance<Part_22_Service>();
    service.Run(isTest);
}