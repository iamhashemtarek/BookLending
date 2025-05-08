# BookLending

BookLending is a comprehensive library management system designed to streamline the process of borrowing and lending books. It provides features for managing books, users, and borrowing activities, while ensuring robust error handling, authentication, and scalability.

## Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

---

## Features
- **Book Management**: Add, update, delete, and retrieve books.
- **Borrowing System**: Manage borrowing and returning of books with due dates and reminders.
- **User Authentication**: Secure user authentication using JWT.
- **Error Handling**: Centralized exception handling with detailed error responses.
- **Background Jobs**: Automated overdue book checks using Hangfire.
- **Logging**: Comprehensive logging with Serilog.
- **API Documentation**: Swagger integration for API exploration.

---

## Technologies Used
- **.NET 9.0**: Core framework for building the application.
- **Entity Framework Core**: ORM for database interactions.
- **Hangfire**: Background job processing.
- **Serilog**: Logging framework.
- **xUnit**: Unit testing framework.
- **AutoMapper**: Object mapping.
- **JWT Authentication**: Secure user authentication.
- **Swagger**: API documentation and testing.

---

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server
- Visual Studio (or any preferred IDE)
- Node.js (if required for front-end integration)

### Installation
1. Clone the repository:
   
```shell
   git clone https://github.com/your-repo/booklending.git
   cd booklending
   
```

2. Set up the database:
   - Update the connection string in `appsettings.json` under the `BookLending.API` project.
   - Run the migrations:
     
```shell
     dotnet ef database update --project BookLending.Infrastructure
     
```

3. Install dependencies:
   
```shell
   dotnet restore
   
```

4. Build the solution:
   
```shell
   dotnet build
   
```

5. Run the application:
   
```shell
   dotnet run --project BookLending.API
   
```

---

## Usage
- Access the API at `https://localhost:5001` (or the configured URL).
- Use Swagger UI at `https://localhost:5001/swagger` to explore and test the API.
- Background jobs (e.g., overdue book checks) are managed via Hangfire Dashboard at `https://localhost:5001/hangfire`.

---

## Project Structure

```
BookLending/
├── BookLending.API/               # API layer
├── BookLending.Application/       # Application services and business logic
├── BookLending.Domain/            # Domain entities and interfaces
├── BookLending.Infrastructure/    # Data access and persistence
├── BookLending.Tests/             # Unit and integration tests
└── README.md                      # Project documentation

```

---

## Testing
- The project uses `xUnit` for unit testing.
- Run tests using the following command:
  
```shell
  dotnet test
  
```
- Code coverage is collected using `coverlet.collector`.

---

## Contributing
Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Commit your changes and push the branch.
4. Submit a pull request.

---
