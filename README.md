# DeployerWebService

The `DeployerWebService` is a specialized tool designed to listen to GitHub actions. Once triggered, it deploys applications using `docker-compose` by pulling the necessary images from GitHub Packages.

## Prerequisites

- Docker
- Docker Compose
- .NET SDK (for local development and testing)

## Setup

### Building the Docker Image

1. Clone this repository:
    ```bash
    git clone [repository-url]
    cd DeployerWebService
    ```

2. Build the Docker image:
    ```bash
    docker build -t deployer-web-service .
    ```

### Running with Docker Compose

You can run the `DeployerWebService` alongside a PostgreSQL container using `docker-compose`.

1. Start the services:
    ```bash
    docker-compose up --build
    ```

This will start the deployer and a PostgreSQL container. The deployer will be available on port `3580` of your host machine.

### Configuration

- Ensure the PostgreSQL connection string in your deployer application uses the `postgres` service name as the host (as defined in the `docker-compose.yml`).
- Secure your PostgreSQL instance. Use strong credentials and follow best practices for securing databases.

## Usage

1. **Triggering Deployment**:
    - Send a POST request to `/deploy/{appId}` to trigger the deployment for a particular app. Replace `{appId}` with the ID of the application you want to deploy.

2. **Webhooks**:
    - The service validates GitHub webhook signatures for security. Ensure you configure your GitHub repository to send webhooks to this service and set the secret key in the deployer's configuration.

## Important Notes

- Running `docker-compose` commands from within a container requires certain considerations. We mount the Docker socket from the host into the deployer container, which allows it to run Docker commands as if they were being run on the host itself. Ensure you understand the security implications of this setup.

- Always back up important data from your PostgreSQL database.

## Support

For issues, feature requests, or assistance, please open an issue in the repository or contact the maintainers.