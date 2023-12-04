using System;
using System.Diagnostics;
using System.IO;

class Program
{
    public static int frameNumber = 1;
  // breader と bwriter の定義を追加
    static StreamReader breader;
    static StreamWriter bwriter;

    // TONES 配列をクラスのメンバーとして定義
    static string[] TONES = {
        "A", "Bb", "B", "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab"
    };
    static double[] freq; 
    static void Main()
    {
        // Pythonスクリプトのパス
        string pythonScriptPath = "./f0.py";

        // 外部プロセスとしてPythonスクリプトを実行
        ExecutePythonScript(pythonScriptPath);

        // ファイル変換処理
        ConvertFile();
    }

    static void ExecutePythonScript(string scriptPath)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "python", // Pythonのコマンド
            RedirectStandardInput = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = $"\"{scriptPath}\"" // 引数は不要
        };

        using (Process process = new Process { StartInfo = psi })
        {
            process.Start();
            process.WaitForExit();

            // 標準出力を取得
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            // エラー出力を取得
            string error = process.StandardError.ReadToEnd();
            Console.WriteLine(error);
        }
    }


    static void ConvertFile()
    {
        
        bool isValue = false;
        string linebuf = "";

        double MINTONE = 110.0;

        // freq の初期化をここで行う
        freq = new double[48];
        for (int i = 0; i < freq.Length; i++)
            freq[i] = MINTONE * Math.Pow(2.0, ((double)i / 12.0));

        string pyinfile = "./b.txt";

        // file open
        OpenReader(pyinfile);
        if (breader == null) Environment.Exit(-1);

        // read the file
        try
        {
            while (true)
            {
                string line = breader.ReadLine();
                if (line == null) break;

                // filename
                if (isValue == false && line.IndexOf('[') < 0)
                {
                    WriteLine(line);
                    continue;
                }

                // start reading
                if (line.IndexOf('[') >= 0)
                {
                    isValue = true;
                }

                if (line.IndexOf(']') >= 0)
                {
                    isValue = false;
                }

                line = line.Replace("[", "");
                line = line.Replace("]", "");
                string[] tokens = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var token in tokens)
                {
                    string tone = SpecifyTone(token);
                    linebuf += (tone + ",");
                }

                if (isValue == false)
                {
                    WriteLine(linebuf);
                    linebuf = "";
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        // file close
        CloseReader();
    }


    static string SpecifyTone(string token)
    {
      double MINTONE = 110.0;
        string tone = "";
        if (token.IndexOf("nan") >= 0) return "nan";
        double f0 = double.Parse(token);

        // freq の初期化をここで行う
        freq = new double[48];
        for (int i = 0; i < freq.Length; i++)
            freq[i] = MINTONE * Math.Pow(2.0, ((double)i / 12.0));

        for (int i = 1; i < freq.Length; i++)
        {
            if (freq[i] < f0) continue;
            int octave = i / 12;
            int fid = i - octave * 12;
            if (fid >= 3) octave++;
            tone = TONES[fid] + octave;
            break;
        }

        Console.WriteLine($"{frameNumber},{tone}");
        frameNumber++; 

        return tone;
    }




      static void OpenReader(string filename)
      {
          try
          {
              breader = new StreamReader(filename);
          }
          catch (Exception e)
          {
              Console.WriteLine(e);
          }
          Console.WriteLine("  open " + filename);
      }

      static void CloseReader()
      {
          try
          {
              breader.Close();
          }
          catch (Exception e)
          {
              Console.WriteLine(e);
          }
      }


    static void WriteLine(string word)
      {
          try
          {
              bwriter.Write(word);
              bwriter.Flush();
              bwriter.WriteLine();
          }
          catch (Exception e)
          {
              Console.WriteLine(e);
          }
      }

}

