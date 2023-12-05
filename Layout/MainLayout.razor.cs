using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace dotnet_hackathon.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private String? imageSource = null;
        private String? calre = null;
        private bool hasFile = false;

        // protected override async Task OnInitializedAsync()
        // {
        //     await Action();
        // }

        private async Task MakeDot(float volume)
        {
            await JSRuntime.InvokeVoidAsync("createDotWithGrowingAnimation", volume);
        }

        private async Task Action()
        {
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(1000);
                await MakeDot(1f);
                var source = (await GetImageStream()).Split(",")[1];
                var image = await StabilityAPI.Call(Convert.FromBase64String(source));
                imageSource = $"data:image/png;base64,{Convert.ToBase64String(image)}";
                StateHasChanged();
            }
        }

        private async Task<string> GetImage(float volume)
        {
            await MakeDot(volume);
            var source = (await GetImageStream()).Split(",")[1];
            var image = await StabilityAPI.Call(Convert.FromBase64String(source));
            return $"data:image/png;base64,{Convert.ToBase64String(image)}";
        }

        private async Task<String> GetImageStream()
        {
            return await JSRuntime.InvokeAsync<String>("exportCanvasAsImage");
        }

        // async Task ReadAudio()
        // {
        //     // var audioBuffer = File.ReadAllBytes("piano.wav");

        //     // 1つ目のC#プログラムを実行
        //     // ExecuteCSharpProgram("../lib/volume/call.cs");
        //     // await Calculate(audioBuffer);

        //     // 2つ目のC#プログラムを実行
        //     // ExecuteCSharpProgram("../lib/pitch/pitch2.cs");
        //     // Pitch2.Action();
        // }

        private async Task<String[][]> Calculate(byte[] audioBuffer)
        {
            var resultCsv = await JSRuntime.InvokeAsync<String>("calculate", audioBuffer);
            return resultCsv.Split("\n").Select(x => x.Split(",")).ToArray();
        }

        private async Task OnFileSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file != null)
            {
                hasFile = true;
                // 最大サイズを増やす（例：30MB）
                long maxFileSize = 30 * 1024 * 1024;

                // 音楽ファイル読み込み
                using var stream = file.OpenReadStream(maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                byte[] audioBuffer = memoryStream.ToArray();

                // 音量計算
                var result = await Calculate(audioBuffer);
                var resultOverThreshold = result
                .Where(x => x.Length == 2)
                .Select(x => (int.Parse(x[0]), float.Parse(x[1])))
                .Where(t => t.Item2 > 0.1f)
                .ToList();

                // 画像取得
                var images = new Dictionary<int, String>();
                resultOverThreshold.ForEach(async tuple =>
                {
                    int frame = tuple.Item1;
                    float volume = tuple.Item2;
                    var image = await GetImage(volume);
                    images.Add(frame, image);
                });

                // 音に合わせて画像を表示
                // 音楽再生
                // var audio = new Audio("piano.wav");
                resultOverThreshold.ForEach(async tuple =>
                {
                    int frame = tuple.Item1;
                    float volume = tuple.Item2;
                    var image = images[frame];
                    if (image != "" && image != null)
                    {
                        imageSource = image;
                        StateHasChanged();
                        await Task.Delay(NextDelay(frame, resultOverThreshold));
                    }
                });
            }
        }

        private int NextDelay(int currentFrame, List<(int, float)> list)
        {
            var nextFrame = list.FirstOrDefault(x => x.Item1 > currentFrame);
            int next = nextFrame != default ? nextFrame.Item1 : list.Last().Item1;
            return next - currentFrame;
        }
    }
}