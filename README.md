# CMDS API Quickstart

This repository includes the source code for a C# .NET console application that demonstrates how to interact with the API using an HTTP client. 

This example shows how to authenticate, make API calls, and process responses for departments, learners, and competency profiles. The API provides much more, of course. The source code here is intended as an example only, to help you get started working with the API.

> Please note this quickstart applies to version 1 of the API for CMDS (and Shift iQ).

You can explore the OpenAPI (Swagger) specification here: [CMDS API version 1](https://dev-cmds.cmds.app/swagger)

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Usage](#usage)
- [API Client Features](#api-client-features)
- [Data Models](#data-models)
- [Examples](#examples)
- [Building and Running](#building-and-running)
- [Contributing](#contributing)
- [License](#license)

## Overview

This quickstart demonstrates:
- **Bearer token authentication** with a client secret
- **HTTP client implementation** with proper disposal patterns
- **JSON serialization/deserialization** using System.Text.Json
- **Configuration management** with appsettings.json
- **API response handling** with custom response objects
- **Simple data processing and reporting** from API responses

The application fetches data from three main endpoints:
- `/cmds/contacts/departments` - Organization departments
- `/cmds/contacts/users` - Learner user accounts
- `/cmds/templates/profiles` - Competency profiles

## Prerequisites

- **.NET 9.0 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension
- **API credentials** (client secret and base address URL)

## Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/cmds-hub/api-quickstart.git
   cd api-quickstart
   ```

2. **Configure your API credentials**
   ```bash
   # Edit appsettings.json with your actual values
   ```

3. **Build and run**
   ```bash
   dotnet build
   dotnet run
   ```

## Configuration

### appsettings.json

Update the configuration file with your API credentials:

```json
{
  "ApiQuickstart": {
    "Secret": "your-actual-client-secret-here",
    "BaseUrl": "https://your-api-domain.com/api/",
    "UserAgent": "ApiQuickstartExample/1.0 (CMDS; Windows; .NET 9.0)",
    "TimeoutSeconds": 30
  }
}
```

### Environment Variables

Alternatively, you can set configuration options using environment variables:
- `ApiQuickstart__Secret`
- `ApiQuickstart__BaseUrl`
- `ApiQuickstart__UserAgent`
- `ApiQuickstart__TimeoutSeconds`

### Configuration Properties

| Property | Description | Required |
|----------|-------------|----------|
| `Secret` | Your API client secret for authentication | Yes |
| `BaseUrl` | The base URL of your CMDS API instance | Yes |
| `UserAgent` | User agent string for API requests | Yes |
| `TimeoutSeconds` | Request timeout in seconds | No (default: 30) |

## Project Structure

```
├── ApiClient.cs          # Simple HTTP client implementation (for demo purposes only)
├── ApiResponse.cs        # Response wrapper with JSON deserialization
├── Application.cs        # Main application logic
├── ApplicationSettings.cs # Configuration model
├── Program.cs           # Entry point and configuration loading
├── appsettings.json     # Configuration file
├── Example.csproj       # Project file
├── Models/
│   ├── Competency.cs    # Competency data model
│   ├── Department.cs    # Department data model
│   ├── Learner.cs       # Learner data model
│   └── Profile.cs       # Profile data model
└── Reports/
    ├── DepartmentReport.cs # Department summary generator
    ├── LearnerReport.cs    # Learner analysis generator
    └── ProfileReport.cs    # Profile summary generator
```

## Usage

### Basic API Client Usage

```csharp
using var client = new ApiClient(clientSecret, baseUrl, userAgent);
client.SetTimeout(TimeSpan.FromSeconds(30));

// Make a GET request
var response = client.Get("v1/status");
if (response.Success)
{
    Console.WriteLine(response.Data);
}
```

### Async Operations

```csharp
// Async GET request
var response = await client.GetAsync("cmds/contacts/departments");
if (response.Success)
{
    var departments = response.GetJsonData<Department[]>();
    // Process departments...
}
```

### JSON Deserialization

```csharp
// Deserialize response data
var response = client.Get("cmds/contacts/departments");
var departments = response.GetJsonData<Department[]>();

// Or manual deserialization
var departments = JsonSerializer.Deserialize<Department[]>(response.Data);
```

## API Client Features

### Authentication
- **Bearer token authentication** using client secret
- **Automatic header management** for all requests

### Request Handling
- **GET requests** with query parameters
- **Custom headers** support
- **URL encoding** for parameters
- **Timeout configuration**

### Response Processing
- **HTTP status code** handling
- **Response headers** extraction
- **JSON deserialization** helpers
- **Error handling** with detailed messages

### Resource Management
- **IDisposable implementation** for proper cleanup
- **HttpClient disposal** following best practices

## Data Models

### Department
```csharp
public class Department
{
    public Guid Identifier { get; set; }
    public string Name { get; set; }
}
```

### Learner
```csharp
public class Learner
{
    public Guid Identifier { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Department[] Departments { get; set; }
}
```

### Profile
```csharp
public class Profile
{
    public Guid Identifier { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public List<Competency> Competencies { get; set; }
}
```

### Competency
```csharp
public class Competency
{
    public Guid Identifier { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}
```

## Examples

### Fetching and Processing Departments

```csharp
var response = client.Get("cmds/contacts/departments");
var departments = JsonSerializer.Deserialize<Department[]>(response.Data)
    .OrderBy(x => x.Name)
    .ToArray();

Console.WriteLine($"Found {departments.Length} departments:");
foreach (var dept in departments)
{
    Console.WriteLine($"  - {dept.Name}");
}
```

### Filtering Learners

```csharp
var response = client.Get("cmds/contacts/users");
var learners = JsonSerializer.Deserialize<Learner[]>(response.Data);

// Find learners with last names starting with "Ab"
var filtered = learners
    .Where(x => x.LastName.StartsWith("Ab", StringComparison.OrdinalIgnoreCase))
    .OrderBy(x => x.LastName)
    .ThenBy(x => x.FirstName)
    .ToArray();
```

### Analyzing Competencies

```csharp
var response = client.Get("cmds/templates/profiles");
var profiles = JsonSerializer.Deserialize<Profile[]>(response.Data);

var distinctCompetencies = profiles
    .SelectMany(p => p.Competencies)
    .Select(c => c.Identifier)
    .Distinct()
    .Count();

Console.WriteLine($"Total distinct competencies: {distinctCompetencies}");
```

## Building and Running

### Using .NET CLI

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Run with specific configuration
dotnet run --configuration Release
```

### Using Visual Studio

1. Open `Example.csproj` in Visual Studio
2. Set your API credentials in `appsettings.json`
3. Press F5 to build and run

### Expected Output

```
{"status":"online","version":"1.0.0"}

## DEPARTMENT SUMMARY
Your organization contains 5 departments.
  - Engineering
  - Human Resources
  - Marketing
  - Operations
  - Sales

## PROFILE SUMMARY
Your organization contains 150 distinct competencies in 12 occupation profiles.
  - Software Developer
  - Project Manager
  - ...

## LEARNER SUMMARY
Your organization contains 1,250 user accounts for learners.
15 people (1.20%) have a last name that starts with the letters `Ab`:
  - Abbott, John (john.abbott@company.com)
  - ...
```

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature-name`
3. Make your changes
4. Test your changes: `dotnet test`
5. Commit your changes: `git commit -am 'Add feature'`
6. Push to the branch: `git push origin feature-name`
7. Submit a pull request

## License

This project is licensed under the Unlicense - see the [LICENSE](LICENSE) file for details.

---

**Note**: This is a demo application. Remember to replace the placeholder values in `appsettings.json` with your actual setup values before running.
