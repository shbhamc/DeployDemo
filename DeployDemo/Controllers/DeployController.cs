using DeployDemo.JWT;
using DeployDemo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace DeployDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeployController(
        DeployDemoContext context,
        BlobService blobService,
        ILoggerFactory loggerFactory,
        JwtService jwtService,
        HttpClient httpClient,
        IConfiguration config) : ControllerBase
    {
        private readonly DeployDemoContext _context = context;
        private readonly BlobService _blobService = blobService;
        private readonly ILogger _logger = loggerFactory.CreateLogger<DeployController>();
        private readonly JwtService _jwtService = jwtService;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _config = config;

        // LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            _logger.LogInformation("Login attempt for user {Username}", model.username);

            if (model.username == "admin" && model.password == "123")
            {
                var token = _jwtService.GenerateToken(model.username);

                _logger.LogInformation("Login successful for user {Username}", model.username);

                return Ok(new { Token = token });
            }

            _logger.LogWarning("Invalid login attempt for user {Username}", model.username);

            return Unauthorized();
        }

        // External API call
        [HttpGet("profile-validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ProfileDeatailsWithValidate(string token)
        {
            _logger.LogInformation("Calling Azure Function API");
            var funUrl = _config["AzureFunctionUrl:ProfileWithValidation"];
            var request = new HttpRequestMessage(HttpMethod.Get, funUrl);


            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
             
            request.Headers.Add("X-Custom-Header", "HeaderValue");

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Received response from Azure Function");

            return Ok(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> ProfileDeatails()
        {
            _logger.LogInformation("Calling Azure Function API");
            var funUrl = _config["AzureFunctionUrl:Profile"];

            var response = await _httpClient.GetAsync(funUrl);

            var result = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Received response from Azure Function");

            return Ok(result);
        }

        // GET ALL
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Fetching all DeployDemo records");

            var data = await _context.DeployDemos.ToListAsync();

            _logger.LogInformation("Total records fetched: {Count}", data.Count);

            return Ok(data);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Fetching record with Id {Id}", id);

            var data = await _context.DeployDemos.FindAsync(id);

            if (data == null)
            {
                _logger.LogWarning("Record not found for Id {Id}", id);
                return NotFound($"Record with Id {id} not found");
            }

            return Ok(data);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeployDTO model)
        {
            if (model == null)
            {
                _logger.LogWarning("Create request received with null model");
                return BadRequest();
            }

            _logger.LogInformation("Creating new record with Name {Name}", model.Name);

            await _context.DeployDemos.AddAsync(model);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Record created with Id {Id}", model.Id);

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // TEST EXCEPTION
        [HttpPost("exceptionaction")]
        public async Task<IActionResult> GenerateException([FromBody] DeployDTO model)
        {
            _logger.LogInformation("Exception test triggered");

            throw new Exception("This is a test exception for monitoring purposes.");
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DeployDTO model)
        {
            _logger.LogInformation("Updating record with Id {Id}", id);

            if (id != model.Id)
            {
                _logger.LogWarning("Id mismatch. Route Id {Id}, Model Id {ModelId}", id, model.Id);
                return BadRequest("Id mismatch");
            }

            var existing = await _context.DeployDemos.FindAsync(id);

            if (existing == null)
            {
                _logger.LogWarning("Update failed. Record not found for Id {Id}", id);
                return NotFound();
            }

            existing.Name = model.Name;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Record updated successfully for Id {Id}", id);

            return Ok(existing);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting record with Id {Id}", id);

            var data = await _context.DeployDemos.FindAsync(id);

            if (data == null)
            {
                _logger.LogWarning("Delete failed. Record not found for Id {Id}", id);
                return NotFound();
            }

            _context.DeployDemos.Remove(data);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Record deleted successfully for Id {Id}", id);

            return Ok("Deleted successfully");
        }

        // FILE UPLOAD
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload attempted with no file");
                return BadRequest("No file selected");
            }

            _logger.LogInformation("Uploading file {FileName}", file.FileName);

            var url = await _blobService.UploadAsync(file);

            _logger.LogInformation("File uploaded successfully. Url: {Url}", url);

            return Ok(new { FileUrl = url });
        }
    }
}