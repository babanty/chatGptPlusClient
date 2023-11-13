using Microsoft.AspNetCore.Mvc;
using TextCopy;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace GptClicker.App.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatGptController : ControllerBase
{
    public string FolderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\AutoitScripts"));

    [HttpPost()]
    public async Task<string> SendMessage([FromBody] string text)
    {
        var sendScriptPath = $@"{FolderPath}\send.exe";
        var openChromeScriptPath = $@"{FolderPath}\openChrome.exe";

        OpenChrome();
        await Task.Delay(2000);

        // Клик на строку поиска
        MouseClick(714, 63);
        await Task.Delay(2000);

        SelectAll();
        await Task.Delay(1000);

        await CopyTextToClipboardAsync("https://chat.openai.com");
        await Task.Delay(1000);

        Insert();
        await Task.Delay(1000);

        Send("{ENTER}");
        await Task.Delay(2000);

        // Клик новый чат
        MouseClick(221, 123);
        await Task.Delay(1000);

        // Клик на поле для ввода gpt
        MouseClick(711, 767);
        await Task.Delay(2000);

        await CopyTextToClipboardAsync(text);
        await Task.Delay(2000);

        // вставить текст из буфера обмена
        Insert();
        await Task.Delay(2000);

        // Клик на  кнопку "отправить"
        MouseClick(1252, 763);
        await Task.Delay(2000);

        // Клик на  кнопку "расширение"
        MouseClick(1351, 63);
        await Task.Delay(2000);

        await WaitForTheGptResponseToComplete();

        return await GetTextFromClipboardAsync();
    }

    private async Task WaitForTheGptResponseToComplete()
    {
        bool continueLoop = true;

        while (continueLoop)
        {
            await Task.Delay(250);
            // Клик на  кнопку "копировать текст"
            MouseClick(1329, 94);
            await Task.Delay(2000);

            string clipboardText = await GetTextFromClipboardAsync();
            var clipboardData = JsonConvert.DeserializeObject<ClipboardData>(clipboardText);

            if (clipboardData != null && !clipboardData.StopButtonElement)
            {
                continueLoop = false;
            }
        }
    }
    private class ClipboardData
    {
        public string Text { get; set; }
        public bool ButtonExists { get; set; }
        public string ButtonText { get; set; }
        public int SendButton { get; set; }
        public string CurrentAddress { get; set; }
        public bool StopButtonElement { get; set; }
    }
    private void MouseClick(int x, int y)
    {
        var leftMouseClickScriptPath = $@"{FolderPath}\leftMouseClick.exe";
        var proc = System.Diagnostics.Process.Start(leftMouseClickScriptPath, new string[] { x.ToString(), y.ToString() });

    }

    private void OpenChrome()
    {
        var path = $@"{FolderPath}\openChrome.exe";
        var proc = System.Diagnostics.Process.Start(path);
    }

    private void SelectAll()
    {
        var path = $@"{FolderPath}\selectAll.exe";
        var proc = System.Diagnostics.Process.Start(path);
    }
    private void Insert()
    {
        var path = $@"{FolderPath}\insert.exe";
        var proc = System.Diagnostics.Process.Start(path);
    }

    private void Send(string key)
    {
        var path = $@"{FolderPath}\send.exe";
        var proc = System.Diagnostics.Process.Start(path, key);
    }

    private async Task CopyTextToClipboardAsync(string text)
    {
        await ClipboardService.SetTextAsync(text);
    }

    private async Task<string?> GetTextFromClipboardAsync()
    {
        return await ClipboardService.GetTextAsync();
    }
}
