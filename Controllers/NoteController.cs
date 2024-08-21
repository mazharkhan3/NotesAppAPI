using Microsoft.AspNetCore.Mvc;
using NotesAppAPI.Services;

namespace NotesAppAPI.Controllers;

[ApiController]
[Route("api/notes")]
public class NoteController : ControllerBase
{
    private readonly NoteService _noteService;

    public NoteController(NoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpPost("check-file")]
    public async Task<IActionResult> CheckGrammar([FromForm] IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("file is not provided or empty");
        }
        
        string markdownText;
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            markdownText = await reader.ReadToEndAsync();
        }
        
        // var plainText = Markdig

        var result = await _noteService.CheckGrammarAsync(markdownText);
        
        return Ok(result);
    }
    
    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("file is not provided or empty");
        }
        
        var result = await _noteService.UploadFile(file);
        
        return Ok("The file " + result + " uploaded successfully");
    }
    
    [HttpGet("files")]
    public IActionResult GetAllFileNames()
    {
        var files = _noteService.GetAllFileNames();
        return Ok(files);
    }
    
    [HttpGet("file/{fileName}")]
    public async Task<IActionResult> GetFileContent(string fileName)
    {
        var filePath = Path.Combine("uploads", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File not found.");
        }

        string markdownContent;
        using (var reader = new StreamReader(filePath))
        {
            markdownContent = await reader.ReadToEndAsync();
        }

        // Convert Markdown to HTML
        var htmlContent = Markdig.Markdown.ToHtml(markdownContent);

        return Content(htmlContent, "text/html");
    }
}