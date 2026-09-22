using Core;

var report = EnvironmentInfo.Collect();

Console.WriteLine($"OS: {report.OsDescription}");
Console.WriteLine($".NET: {report.FrameworkDescription}");
Console.WriteLine($"Process Architecture: {report.ProcessArchitecture}");
Console.WriteLine($"Detected RID: {report.DetectedRid}");
Console.WriteLine($"Reported RID: {report.ReportedRid}");
Console.WriteLine($"Base Directory: {report.BaseDirectory}");
Console.WriteLine($"Build Note: {report.BuildNote}");