using System;
using System.Diagnostics;

class Call
{
    public static void Action()
    {
        string pythonScriptPath = "./calculate_volume.py";
        string wavFilePath = "./piano.wav";
        int frameSize = 2048;  // フレームサイズ
        int overlap = 512;     // オーバーラップ

        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{pythonScriptPath} {wavFilePath} {frameSize} {overlap}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine(output);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"エラー: {ex.Message}");
        }
    }
}
