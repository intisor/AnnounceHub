# AnnounceHub

## Overview

AnnounceHub is a real-time announcement broadcasting system built with ASP.NET Core 8.0 and SignalR. It enables administrators to send instant announcements to all connected users through a web interface, with announcements being delivered in real-time using WebSocket technology. The application features a complete authentication and authorization system using ASP.NET Core Identity with role-based access control.

## Key Features

- **Real-time Announcements**: Instant message broadcasting using SignalR WebSocket connections
- **User Authentication**: Secure login and registration system with ASP.NET Core Identity
- **Role-Based Authorization**: Admin-only announcement creation with role-based access control
- **Persistent Storage**: MySQL database integration for storing users and announcements
- **Responsive UI**: Bootstrap-based interface for cross-device compatibility
- **Cookie-based Sessions**: Secure authentication with configurable session expiration
- **Automatic Database Seeding**: Default admin user creation on first run

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 (MVC pattern)
- **Real-time Communication**: ASP.NET Core SignalR 1.1.0
- **Authentication**: ASP.NET Core Identity 8.0.10
- **ORM**: Entity Framework Core 8.0.10
- **Database Provider**: Pomelo.EntityFrameworkCore.MySql 8.0.2

### Frontend
- **UI Framework**: Bootstrap (via CDN and local libs)
- **JavaScript**: SignalR Client Library 3.1.18
- **View Engine**: Razor (.cshtml)
- **Client Libraries**: jQuery, jQuery Validation

### Database
- **DBMS**: MySQL 8.0.21+
- **Migrations**: Entity Framework Core Migrations

## Architecture

### Application Structure

```
AnnounceHub/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs     # Main page and announcement sending
│   ├── AdminController.cs    # Admin-specific announcement functionality
│   └── AccountController.cs  # Authentication (login, register, logout)
├── Models/               # View Models
│   ├── IndexViewModel.cs
│   ├── SendAnnouncementViewModel.cs
│   └── ErrorViewModel.cs
├── Views/                # Razor Views
│   ├── Home/
│   ├── Admin/
│   ├── Account/
│   └── Shared/
├── Data/                 # Data Layer
│   └── AppDbContext.cs       # EF Core DbContext, entities, and DB initializer
├── AppHub/              # SignalR Hubs
│   └── AnnouncementHub.cs    # Real-time announcement hub
├── Migrations/          # EF Core migrations
├── wwwroot/            # Static files (CSS, JS, libraries)
├── Auth.cs             # Custom authorization attribute
├── Program.cs          # Application entry point and configuration
└── appsettings.json    # Configuration file
```

### Data Model

**User** (extends IdentityUser)
- UserName (string)
- Email (string)
- Joined (DateTime)
- Plus all standard Identity properties

**Announcement**
- Id (int, primary key)
- Message (string)
- CreatedAt (DateTime)

**Roles**
- Admin: Can send announcements and access admin pages

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server 8.0.21+](https://dev.mysql.com/downloads/)
- A MySQL client (optional, for manual database management)
- A code editor (Visual Studio 2022, VS Code, or Rider)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/intisor/AnnounceHub.git
cd AnnounceHub
```

### 2. Configure the Database

Edit the `appsettings.json` file to configure your MySQL connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AnnounceHub;User=your_username;Password=your_password"
  }
}
```

**Important**: 
- Replace `your_username` and `your_password` with your actual MySQL credentials
- For security best practices, create a dedicated MySQL user for the application instead of using the root account
- Example: `CREATE USER 'announcehub_user'@'localhost' IDENTIFIED BY 'secure_password';`
- Grant only necessary permissions: `GRANT SELECT, INSERT, UPDATE, DELETE ON AnnounceHub.* TO 'announcehub_user'@'localhost';`

### 3. Create the Database

The application uses Entity Framework Core migrations. Run the following commands:

```bash
# Restore dependencies
dotnet restore

# Apply migrations (creates database and tables)
dotnet ef database update
```

Alternatively, the application will automatically create the database on first run using `EnsureCreatedAsync()`.

### 4. Run the Application

```bash
dotnet run
```

The application will start on `https://localhost:5001` (HTTPS) or `http://localhost:5000` (HTTP).

## Default Admin Account

On first run, the application automatically creates a default admin user:

- **Username**: `Intitech`
- **Email**: `admin@example.com`
- **Role**: Admin
- **Password**: See `Data/AppDbContext.cs` in the `DbInitializer.Initialize()` method

**⚠️ SECURITY WARNING**: The default password is hardcoded in the source code for development purposes. You **MUST** change this password before deploying to any production or publicly accessible environment. Consider using environment variables or a secure configuration system for production deployments.

## Usage Guide

### For End Users

1. **View Announcements**
   - Navigate to the home page (`/`)
   - All announcements are displayed with timestamps
   - New announcements appear automatically in real-time (no refresh needed)

2. **User Registration**
   - Go to `/Account/Register`
   - Provide email and password
   - New users are created without admin privileges

3. **Login**
   - Go to `/Account/Login` or `/home/login`
   - Enter username and password
   - Check "Remember Me" for persistent login (1 day session)

### For Administrators

1. **Send Announcements (Admin Role)**
   - Login as a user with Admin role
   - Navigate to `/Admin/SendAnnouncement`
   - Enter your message
   - Submit to broadcast to all connected users

2. **Send Announcements (Special Username)**
   - Login as user "Intitech"
   - Navigate to `/Home/SendAnnouncement`
   - Uses custom `[Auth(Username = "Intitech")]` attribute
   - Submit to broadcast announcement

**Note**: There are two announcement endpoints for demonstration purposes - one uses standard role-based authorization, the other uses a custom username-based authorization attribute.

## Authentication & Authorization

### Authentication Flow

1. **Cookie-based Authentication**
   - Authentication scheme: `CookieAuthenticationDefaults.AuthenticationScheme`
   - Cookie name: "Announce"
   - Session duration: 1 day (sliding expiration)
   - Login path: `/home/login`
   - Access denied path: `/home/privacy`

2. **ASP.NET Core Identity**
   - User management via `UserManager<User>`
   - Password sign-in via `SignInManager<User>`
   - Role management via `RoleManager<IdentityRole>`

### Authorization Methods

1. **Role-Based**: `[Authorize(Roles = "Admin")]`
   - Used on AdminController
   - Requires user to be in "Admin" role

2. **Custom Attribute**: `[Auth(Username = "Intitech")]`
   - Custom authorization attribute (Auth.cs)
   - Checks specific username
   - Applied to Home/SendAnnouncement action

3. **Anonymous Access**: `[AllowAnonymous]`
   - Used on public pages like Home/Index

## Real-time Features (SignalR)

### Hub Configuration

The application uses SignalR for real-time communication:

```csharp
// Hub endpoint: /announcementhub
app.MapHub<AnnouncementHub>("/announcementhub");
```

### Client-Side Integration

JavaScript code on the home page connects to the SignalR hub:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/announcementhub")
    .build();

connection.on("ReceiveAnnouncement", function (message) {
    // Dynamically add announcement to the page
});

connection.start();
```

### How It Works

1. Client connects to SignalR hub on page load
2. Admin sends announcement via HTTP POST
3. Server saves announcement to database
4. Server broadcasts to all connected clients via `Clients.All.SendAsync()`
5. Clients receive and display the announcement instantly

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AnnounceHub;User=root;Password=your_password"
  },
  "AllowedHosts": "*"
}
```

### Environment-Specific Configuration

- `appsettings.Development.json`: Development environment settings
- Production settings should be configured via environment variables or Azure App Configuration

## Development Workflow

### Adding New Migrations

When you modify entities (User, Announcement):

```bash
# Create a new migration
dotnet ef migrations add YourMigrationName

# Apply the migration
dotnet ef database update
```

### Database Initialization

The `DbInitializer` class in `Data/AppDbContext.cs` handles:
- Database creation
- Admin role creation
- Default admin user creation

This runs automatically on application startup in `Program.cs`.

### Running in Development Mode

```bash
# Set environment to Development
export ASPNETCORE_ENVIRONMENT=Development  # Linux/Mac
set ASPNETCORE_ENVIRONMENT=Development     # Windows

# Run with hot reload
dotnet watch run
```

## Deployment Considerations

### Before Deploying to Production

1. **Change Default Credentials**
   - Update the default admin password in `DbInitializer.Initialize()`
   - Or remove auto-creation and manually create admin accounts

2. **Update Connection String**
   - Use environment variables for connection strings
   - Never commit production credentials to source control

3. **Enable HTTPS**
   - The app uses `app.UseHttpsRedirection()`
   - Configure proper SSL certificates

4. **Update Cookie Security**
   - Consider setting `SameSite`, `Secure`, and `HttpOnly` flags
   - Review session expiration time

5. **Error Handling**
   - Production mode uses `/Home/Error` for exceptions
   - Configure appropriate logging

6. **Database**
   - Use `dotnet ef database update` to apply migrations
   - Or use SQL scripts generated from migrations
   - Consider removing `EnsureCreatedAsync()` in production

### Deployment Options

- **Azure App Service**: Native support for ASP.NET Core
- **Docker**: Create a Dockerfile for containerization
- **IIS**: Configure IIS with ASP.NET Core Module
- **Linux Server**: Use Nginx or Apache as reverse proxy with Kestrel

## Security Notes

### Current Implementation

1. **Password Requirements**: Uses ASP.NET Core Identity defaults
2. **Cookie Authentication**: Configured with 1-day expiration
3. **HTTPS Redirection**: Enabled via `UseHttpsRedirection()`
4. **HSTS**: Enabled in production mode (30-day default)

### Security Recommendations

1. **Strengthen Password Policy**: Configure Identity options for stronger passwords
2. **Enable Two-Factor Authentication**: Implement 2FA for admin accounts
3. **Input Validation**: Add validation to announcement messages (XSS prevention)
4. **Rate Limiting**: Implement rate limiting on announcement endpoints
5. **Secure Connection String**: Use Azure Key Vault or environment variables
6. **Update Dependencies**: Regularly update NuGet packages for security patches
7. **CORS Policy**: Configure CORS if accessed from different origins
8. **SQL Injection**: Already protected by Entity Framework parameterization

## License

This project is licensed under the MIT License. See the `LICENSE.txt` file for full details.

The MIT License is a permissive license that allows for reuse with minimal restrictions. Key points:
- Commercial use allowed
- Modification allowed
- Distribution allowed
- Private use allowed
- Warranty and liability limitations apply

For the complete license text, refer to the `LICENSE.txt` file in the repository root.

## Contributing

Contributions are welcome! Here's how you can help:

1. **Fork the Repository**
2. **Create a Feature Branch**
   ```bash
   git checkout -b feature/YourFeature
   ```
3. **Commit Your Changes**
   ```bash
   git commit -m "Add some feature"
   ```
4. **Push to Your Branch**
   ```bash
   git push origin feature/YourFeature
   ```
5. **Open a Pull Request**

### Code Standards

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML comments for public APIs
- Ensure Entity Framework migrations are included

## Troubleshooting

### Common Issues

#### Database Connection Fails
```
Error: Unable to connect to MySQL server
```
**Solution**: 
- Verify MySQL server is running
- Check connection string in `appsettings.json`
- Ensure MySQL user has proper permissions

#### SignalR Connection Fails
```
Error: Failed to start connection: Error: WebSocket failed to connect
```
**Solution**:
- Check that the application URL matches the SignalR hub URL
- Verify no firewall is blocking WebSocket connections
- Ensure HTTPS is properly configured if using SSL

#### Migration Errors
```
Error: Unable to create an object of type 'AppDbContext'
```
**Solution**:
```bash
dotnet ef migrations add InitialCreate --project AnnounceHub.csproj
dotnet ef database update --project AnnounceHub.csproj
```

#### Login Redirects to Home Without Authenticating
**Solution**:
- Check that the password meets Identity requirements
- Verify the user exists in the database
- Check that cookies are enabled in the browser

#### Announcements Not Appearing in Real-time
**Solution**:
- Open browser console and check for JavaScript errors
- Verify SignalR connection is established
- Check that the SignalR hub endpoint is correctly configured

## Project Status

This is a demonstration/educational project showcasing:
- ASP.NET Core MVC architecture
- Real-time web applications with SignalR
- ASP.NET Core Identity authentication
- Entity Framework Core with MySQL
- Role-based authorization

## Future Enhancements

Potential improvements for this project:

- [ ] Announcement categories and filtering
- [ ] User profile management
- [ ] Announcement edit and delete functionality
- [ ] Rich text editor for announcements
- [ ] File attachment support
- [ ] Announcement history pagination
- [ ] User notification preferences
- [ ] Email notifications for announcements
- [ ] Multi-tenancy support
- [ ] API endpoints for mobile apps
- [ ] Unit and integration tests
- [ ] Docker containerization
- [ ] CI/CD pipeline

## Contact & Support

For questions, issues, or contributions, please:
- Open an issue on GitHub
- Submit a pull request
- Contact the repository owner

---

**Built with ❤️ using ASP.NET Core and SignalR**