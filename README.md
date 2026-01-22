# FunDoo Note Application

FunDoo is a feature-rich, distributed note-taking application built with **ASP.NET Core 8.0**. It is designed with a scalable **4-layer architecture** and leverages modern technologies like **Redis** for caching and **RabbitMQ** for asynchronous messaging to handle tasks like email notifications and reminders.

> **Note:** This project implements a complete backend system with Authentication, Authorization, and Background processing. The `TestingLayer` is currently initialized as a placeholder structure for future unit and integration tests.

## 🚀 Features

### 👤 User Management
*   **Secure Authentication:** JWT-based authentication using **RSA Asymmetric Encryption** (Public/Private keys).
*   **Registration & Login:** Secure signup and login endpoints.
*   **Profile Management:** Password reset capabilities.

### 📝 Note Management
*   **CRUD Operations:** Create, Read, Update, and Delete notes.
*   **Organization:**
    *   **Pin/Unpin:** Keep important notes at the top.
    *   **Archive/Unarchive:** Organized storage for older notes.
    *   **Trash/Restore:** Soft delete implementation with restore capability.
    *   **Color Coding:** personalized note styling.
*   **Reminders:** Scheduled reminders processed by a background service.
*   **Images:** Support for attaching images to notes.

### 🏷️ & 🤝 Organization & Collaboration
*   **Labels:** Create and assign custom labels to filter and organize notes.
*   **Collaboration:** Share notes with other users (Collaborators) for shared access.

### ⚡ Performance & Scalability
*   **Caching:** Implements **Redis** (StackExchange.Redis) to cache frequently accessed data.
*   **Async Messaging:** Uses **RabbitMQ** with **MassTransit** to handle:
    *   Email dispatching.
    *   Asynchronous reminder processing.

## 🏗️ Architecture

The solution follows a Clean Architecture approach separated into four distinct layers:

1.  **FunDooApp (Presentation Layer):**
    *   ASP.NET Core Web API Controllers.
    *   Middleware for Global Exception Handling.
    *   Background Services (`NoteReminderService`).
    *   DI Configuration & Extensions.
2.  **BusinessLogicLayer (BLL):**
    *   Core business logic and service implementations.
    *   Message Consumers (`EmailConsumer`, `ReminderConsumer`).
    *   DTO Mappings (AutoMapper).
    *   **Important:** Holds the `private.key` and `public.key` for JWT.
3.  **DataLogic (Data Access Layer):**
    *   Entity Framework Core setup (`ApplicationDbContext`).
    *   Repository Pattern implementations.
    *   Database Migrations.
4.  **ModelLayer (Domain Layer):**
    *   Database Entities (`User`, `Note`, `Label`, `Collaborator`).
    *   Data Transfer Objects (DTOs).

## 🛠️ Tech Stack

*   **Framework:** .NET 8.0
*   **Database:** Microsoft SQL Server
*   **ORM:** Entity Framework Core
*   **Message Broker:** RabbitMQ (MassTransit)
*   **Caching:** Redis
*   **Authentication:** JWT (RS256)
*   **Logging:** Microsoft.Extensions.Logging

## 📋 Prerequisites

Ensure the following services are installed and running locally or via Docker:

*   [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
*   [Redis](https://redis.io/download/) (Default port: `6379`)
*   [RabbitMQ](https://www.rabbitmq.com/download.html) (Default Guest/Guest access)

## ⚙️ Setup & Configuration

### 1. Database Configuration
Update the connection string in `FunDooApp/appsettings.json` or `FunDooApp/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=FunDooDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2. RSA Keys (Critical)
The application requires RSA keys for generating JWT tokens. Ensure the following files exist in the **`BusinessLogicLayer/`** directory:
*   `private.key`
*   `public.key`

### 3. Messaging & Caching
*   **RabbitMQ:** Configure credentials in `appsettings.json` under `RabbitMqSettings` if strictly required (defaults are configured for local development).
*   **Redis:** Ensure your Redis instance is reachable at `localhost`.

## 📦 Installation & Running

1.  **Clone the Repository:**
    ```bash
    git clone <repo-url>
    cd FunDoo
    ```

2.  **Restore Dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Apply Migrations:**
    Create the database and apply schema changes.
    ```bash
    dotnet ef database update --project DataLogic --startup-project FunDooApp
    ```

4.  **Run the Application:**
    ```bash
    cd FunDooApp
    dotnet run
    ```

5.  **Access API:**
    *   Swagger Documentation: `http://localhost:5127/swagger` (or port specific to your launch settings).

## 🧪 Testing

The project includes a `TestingLayer` intended for unit tests. Currently, it contains the basic project structure and reference boilerplate.

To execute tests (once added):
```bash
dotnet test TestingLayer
```
