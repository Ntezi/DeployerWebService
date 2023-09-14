using System.Diagnostics;
using System.Security.Cryptography;
using DeployerWebService.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeployerWebService.Controllers;

[ApiController]
[Route("[controller]")]
public class DeployController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DeployerContext _dbContext;


    public DeployController(IConfiguration configuration, DeployerContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        
    }

    [HttpPost]
    [HttpPost("{appId}")]
    public IActionResult Post(int appId)
    {
        var appConfig = GetAppConfigFromDatabase(appId);
        if (appConfig == null)
        {
            return NotFound("App configuration not found.");
        }

        if (!ValidateGitHubWebhookSignature(HttpContext, appConfig.GitHubWebhookSecret))
        {
            return BadRequest("Invalid webhook signature");
        }

        Deploy(appConfig.DockerComposeFilePath);

        return Ok("Deployment triggered");
    }

    private void Deploy(string dockerComposeFilePath)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "docker-compose",
            ArgumentList = { "-f", dockerComposeFilePath, "up", "-d" }, // specifying file and running in detached mode
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using (var process = Process.Start(processInfo))
        {
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                var errorOutput = process.StandardError.ReadToEnd();
                // You might want to log this error or handle it accordingly.
                throw new InvalidOperationException($"Deployment failed: {errorOutput}");
            }
        }
    }

    private AppConfig GetAppConfigFromDatabase(int appId)
    {
        return _dbContext.AppConfigurations.FirstOrDefault(ac => ac.Id == appId);
    }
    
    private bool ValidateGitHubWebhookSignature(HttpContext context, string secret)
    {
        var payload = context.Request.Body;  // you may need to read this into a byte array depending on the state of the stream

        // Get the header signature from the request
        var headerSignature = context.Request.Headers["X-Hub-Signature-256"];

        if (string.IsNullOrWhiteSpace(headerSignature))
        {
            return false;
        }

        // Compute hash
        using var hmacsha256 = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret));
        var computedHash = hmacsha256.ComputeHash(payload);

        // Convert hash to hex string
        var computedSignature = $"sha256={BitConverter.ToString(computedHash).Replace("-", "").ToLower()}";

        // Timing-safe equal check
        return string.CompareOrdinal(headerSignature, computedSignature) == 0;
    }

}