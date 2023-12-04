using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        // 1つ目のC#プログラムを実行
        ExecuteCSharpProgram("../lib/volume/call.cs");

        // 2つ目のC#プログラムを実行
        ExecuteCSharpProgram("../lib/pitch/pitch2.cs");
    }

    static void ExecuteCSharpProgram(string filePath)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = $"run {filePath}"
        };

        using (Process process = new Process { StartInfo = psi })
        {
            process.Start();
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            string error = process.StandardError.ReadToEnd();
            Console.WriteLine(error);
        }
    }
}
