namespace DeployerWebService.Models;

public class AppConfig
{
    public int Id { get; set; } // Unique identifier for each app
    public string AppName { get; set; } // Name of the app for reference
    public string GitHubWebhookSecret { get; set; } // Secret to validate the webhook
    public string DockerComposeFilePath { get; set; } // Path to the docker-compose file for this app
}
