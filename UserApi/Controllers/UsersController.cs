using Microsoft.AspNetCore.Mvc;
using UserApi.DataAccess;
using UserApi.Libraries;
using UserApi.Models;

namespace UserApi.Controllers;

/// <summary>
/// Uploads data from a .csv file to the database.
/// </summary>
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UsersController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Get users sorted by username in ascending or descending order.
    /// </summary>
    /// <param name="count">Number of users to return.</param>
    /// <param name="sortDirection">Sorting direction: "asc" for ascending, "desc" for descending.</param>
    /// <returns>A list of users.</returns>
    [HttpGet]
    [Route("GetUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetUsers(int count, string sortDirection)
    {
        if (count == 0 || string.IsNullOrEmpty(sortDirection))
        {
            return BadRequest("Invalid input");
        }

        List<User> records;
        switch (sortDirection.ToLower())
        {
            case "asc":
                records = _db.Users.Take(count).OrderBy(u => u.UserName).ToList();
                break;
            case "desc":
                records = _db.Users.Take(count).OrderByDescending(u => u.UserName).ToList();
                break;
            default:
                return BadRequest();
        }

        return Ok(records);
    }

    /// <summary>
    /// Uploads a CSV file containing user data to the database.
    /// </summary>
    /// <param name="file">The CSV file to upload.</param>
    /// <returns>A response indicating the result of the upload.</returns>
    [HttpPost]
    [Route("uploadFile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UploadCsv(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest();
        }

        string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles");
        string filePath = Path.Combine(directoryPath, file.FileName);
        if (Path.GetExtension(filePath) != ".csv")
        {
            return BadRequest("Invalid file format");
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        List<User> records = ReadCsvFile.Read(filePath);
        foreach (var record in records)
        {
            if (_db.Users.Any(u => u.UserName == record.UserName))
            {
                _db.Users.Update(record);
                _db.SaveChanges();
            }
            else
            {
                _db.Users.Add(record);
                _db.SaveChanges();
            }
        }

        return Ok("Upload Successful");
    }
}