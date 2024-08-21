namespace NotesAppAPI.Services;

public class NoteService
{
    private readonly HttpClient _httpClient;

    public NoteService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CheckGrammarAsync(string text)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.languagetool.org/v2/check")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["text"] = text,
                ["language"] = "en-US"
            })
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    public async Task<string> UploadFile(IFormFile file)
    {
        var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;

        if (Directory.Exists("uploads") == false)
        {
            Directory.CreateDirectory("uploads");
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", uniqueFileName);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return uniqueFileName;
    }

    public List<string> GetAllFileNames()
    {
        if (!Directory.Exists("uploads"))
        {
            return new List<string>();
        }

        var files = Directory.GetFiles("uploads")
            .Select(Path.GetFileName)
            .ToList();

        return files;
    }
}