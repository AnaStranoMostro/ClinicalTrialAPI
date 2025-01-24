# ClinicalTrialAPI

## Prerequisites
- Docker
- .NET 8 SDK

## Technologies Used
- .NET 8
- Docker (with Docker Compose)
- Microsoft SQL Server
- Moq
- Entity Framework Core InMemory

## Running the Application

1. Clone the repository: - [GitHub Repo](https://github.com/AnaStranoMostro/ClinicalTrialAPI)
2. Build and run the Docker containers or use the `docker-compose.yml` file.
3. The API will be available at `http://localhost:5000`.

## Accessing Swagger
Swagger UI is available at `http://localhost:5000/swagger`.


## Volumes
The SQL Server data is stored in a Docker volume named `mssql-data`.

## Environment Variables
- `SA_PASSWORD`: The password for the SQL Server `sa` user.
- `ACCEPT_EULA`: Set to `Y` to accept the SQL Server EULA.
- `DBServer`: The database server name (default: `mssql`).
- `DBPort`: The database server port (default: `1433`).
- `DBUser`: The database user (default: `SA`).
- `DBPassword`: The database password (default: `StronKPassW0rd1!`).

## Docker Compose
The `docker-compose.yml` file is used to define and run multi-container Docker applications. I's been added to the GitHub repo and it includes the following services:
- `api`: The Clinical Trial API service.
- `mssql`: The Microsoft SQL Server service.



## Author
- Ana Zdravkovic
- Date of Creation: January 23, 2025

