using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cinnamon_back.Data;
using cinnamon_back.Models;

namespace cinnamon_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public PartsController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Part>>> GetParts()
    {
        var parts = await _context.Parts.ToListAsync();
        return Ok(parts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePart([FromBody] PartDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { success = false, error = "Product Name is required." });
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            return BadRequest(new { success = false, error = "Product Description is required." });
        }

        if (string.IsNullOrWhiteSpace(dto.ImageBase64))
        {
            return BadRequest(new { success = false, error = "Product Image is required." });
        }

        try
        {
            // Generate next available ID (e.g. part-101)
            var count = await _context.Parts.CountAsync();
            var nextNum = count + 1;
            var id = $"part-{nextNum:D3}";
            while (await _context.Parts.AnyAsync(p => p.Id == id))
            {
                nextNum++;
                id = $"part-{nextNum:D3}";
            }

            // Save Base64 Image
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var partsFolder = Path.Combine(webRoot, "parts");
            if (!Directory.Exists(partsFolder))
            {
                Directory.CreateDirectory(partsFolder);
            }

            var extension = string.IsNullOrWhiteSpace(dto.ImageExtension) ? "jpg" : dto.ImageExtension.TrimStart('.');
            var filename = $"{id}.{extension}";
            var filepath = Path.Combine(partsFolder, filename);

            var base64Data = dto.ImageBase64;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Substring(base64Data.IndexOf(",") + 1);
            }

            var bytes = Convert.FromBase64String(base64Data);
            await System.IO.File.WriteAllBytesAsync(filepath, bytes);

            var imageUrl = $"/parts/{filename}";

            var newPart = new Part
            {
                Id = id,
                Name = dto.Name.Trim(),
                Category = dto.Category,
                Image = imageUrl,
                Description = dto.Description.Trim(),
                InStock = dto.InStock
            };

            _context.Parts.Add(newPart);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, part = newPart });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}
