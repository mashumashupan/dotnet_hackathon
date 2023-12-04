using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class StabilityAPIResponse
{
    public class Artifact {
        public string? base64 { get; set; }
        public string? finishReason { get; set; }
        public long? seed { get; set; }
    }
    public Artifact[] artifacts { get; set; }
}

class StabilityAPI
{
    static async Task Action()
    {
        var bytes = File.ReadAllBytes("../stabilityAItest/init_image.png");
        Console.WriteLine(await Call(bytes));
    }

    static async Task<byte[]> Call(byte[] initImage, 
        float imageStrength = 0.33f,
        int steps = 15,
        int seed = 0,
        int cfgScale = 5,
        int samples = 1)
    {
        using (var httpClient = new HttpClient())
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new ByteArrayContent(initImage), "init_image", "init_image.png");
            formData.Add(new StringContent("IMAGE_STRENGTH"), "init_image_mode");
            formData.Add(new StringContent(imageStrength.ToString()), "image_strength");
            formData.Add(new StringContent(steps.ToString()), "steps");
            formData.Add(new StringContent(seed.ToString()), "seed");
            formData.Add(new StringContent(cfgScale.ToString()), "cfg_scale");
            formData.Add(new StringContent(samples.ToString()), "samples");

            // テキストプロンプトを動的に追加
            var textPrompts = new List<(string text, int weight)>
            {
                ("Gradation, (blur:2), big ripples, spreading faintly,(watercolor:2), pale", 1),
                ("clear, vivid", -1)
                // 追加のテキストプロンプトはここに続けて追加
            };

            for (int i = 0; i < textPrompts.Count; i++)
            {
                formData.Add(new StringContent(textPrompts[i].text), $"text_prompts[{i}][text]");
                formData.Add(new StringContent(textPrompts[i].weight.ToString()), $"text_prompts[{i}][weight]");
            }

            var apiKey = "sk-cDbc6FpFeyGAGqMzBfku6iDKSya7TgzmZTDVt8B5lvNNrz6O";
            var requestUri = "https://api.stability.ai/v1/generation/stable-diffusion-xl-1024-v1-0/image-to-image";
            

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            using (var response = await httpClient.PostAsync(requestUri, formData))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Non-200 response: {await response.Content.ReadAsStringAsync()}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                // レスポンス内容を必要に応じて処理
                // Console.WriteLine(responseContent);
                var json = System.Text.Json.JsonSerializer.Deserialize<StabilityAPIResponse>(responseContent);
                var artifacs = json?.artifacts;
                Debug.Assert(artifacs != null);
                Debug.Assert(artifacs.Length > 0);
                var artifact = artifacs[0];
                var image = artifact?.base64;
                // saveImage(image);
                return base64ToByte(image);
            }
        }
    }

    static byte[] base64ToByte(string? base64)
    {
        if (base64 == null) return Array.Empty<byte>();
        return Convert.FromBase64String(base64);
    }

    static void saveImage(string? base64)
    {
        if (base64 == null) return;
        var base64Data = Convert.FromBase64String(base64);
        var fileStream = new FileStream($"../stabilityAItest/{DateTime.Now.ToString("yyyyMMddHHmmss")}.png", FileMode.Create);
        fileStream.Write(base64Data, 0, base64Data.Length);
        fileStream.Close();
    }
}
