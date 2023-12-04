using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace dotnet_hackathon.Layout {
public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private String? imageSource = null;

    protected override async Task OnInitializedAsync()
    {
        await Action();
        // ReadAudio();
    }

    private async Task MakeDot()
    {
        await JSRuntime.InvokeVoidAsync("createDotWithGrowingAnimation");
    }

    private async Task Action()
    {
        for (int i = 0; i < 100; i++)
        {
            await Task.Delay(1000);
            await MakeDot();
            imageSource = await GetImageStream();
            StateHasChanged(); // 通知して再レンダリングをトリガーする
        }
    }

    private async Task<String> GetImageStream()
    {
        return await JSRuntime.InvokeAsync<String>("exportCanvasAsImage");
    }

    static void ReadAudio()
    {
        // 1つ目のC#プログラムを実行
        // ExecuteCSharpProgram("../lib/volume/call.cs");
        Call.Action();

        // 2つ目のC#プログラムを実行
        // ExecuteCSharpProgram("../lib/pitch/pitch2.cs");
        Pitch2.Action();
    }

    // static void ExecuteCSharpProgram(string filePath)
    // {
    //     ProcessStartInfo psi = new ProcessStartInfo
    //     {
    //         FileName = "dotnet",
    //         RedirectStandardInput = true,
    //         RedirectStandardOutput = true,
    //         RedirectStandardError = true,
    //         UseShellExecute = false,
    //         CreateNoWindow = true,
    //         Arguments = $"run {filePath}"
    //     };

    //     using (Process process = new Process { StartInfo = psi })
    //     {
    //         process.Start();
    //         process.WaitForExit();

    //         string output = process.StandardOutput.ReadToEnd();
    //         Console.WriteLine(output);

    //         string error = process.StandardError.ReadToEnd();
    //         Console.WriteLine(error);
    //     }
    // }
}
}