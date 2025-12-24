# Subscription & Membership Management SaaS

A comprehensive multi-tenant SaaS application built with ASP.NET Core 8 that helps businesses manage their subscriptions, memberships, and payments. The solution follows Clean Architecture principles to ensure maintainability and scalability.

## What This Application Does

This platform is designed for businesses that need to manage recurring memberships and subscriptions. Whether you're running a gym, a subscription box service, or any business with recurring customers, this system helps you track members, manage plans, process payments, and keep everything organized.

## Architecture Overview

The solution is structured using Clean Architecture, which means we've separated concerns into distinct layers:

- **Domain Layer**: Contains the core business entities and rules. This is where your business logic lives, completely independent of any external frameworks.
- **Application Layer**: Handles use cases and business workflows. This is where we orchestrate operations using the CQRS pattern with MediatR.
- **Infrastructure Layer**: Deals with external concerns like database access, file storage, and third-party services. This is where we implement the interfaces defined in the domain.
- **API Layer**: The presentation layer that exposes RESTful endpoints and handles HTTP requests/responses.

This separation makes the codebase easier to test, maintain, and extend over time.

## Technology Stack

We're using modern .NET technologies to build a robust and performant application:

- **.NET 8** - The latest LTS version of .NET, providing excellent performance and modern language features
- **ASP.NET Core Web API** - For building RESTful APIs with built-in support for dependency injection, middleware, and more
- **Entity Framework Core** - Our ORM of choice for database operations, with support for both PostgreSQL and MySQL
- **JWT Authentication** - Secure token-based authentication for API access
- **MediatR** - Implements the CQRS pattern, making our code more organized and testable
- **FluentValidation** - For comprehensive input validation with clear error messages
- **Serilog** - Structured logging with file and console outputs for better debugging and monitoring
- **Hangfire** - Background job processing for scheduled tasks like membership expiry checks
- **SignalR** - Real-time notifications to keep users informed instantly
- **BCrypt** - Secure password hashing to protect user credentials

## Key Features

### EPIC 1: Authentication
We've implemented a complete authentication system that allows users to register and log in securely. The system uses JWT tokens for stateless authentication, and passwords are hashed using BCrypt before storage. This means even if someone gains access to the database, they can't see actual passwords.

### EPIC 2: Business Profile
Each user can create and manage their business profile. This includes business name, description, contact information, and currency settings. The system supports multi-tenant architecture, so each business's data is completely isolated.

### EPIC 3: Member Management
Managing your customer base is straightforward with this system. You can add new members, update their information, search through your member list, and deactivate members when needed. The search functionality supports filtering by name, email, or phone number, and results are paginated for better performance.

### EPIC 4: Subscription Plans
Create and manage different subscription plans for your business. Each plan has a name, description, price, and duration. You can activate or deactivate plans as needed, and the system prevents deletion of plans that are currently in use.

### EPIC 5: Membership Assignment
Assign subscription plans to members with automatic expiry date calculation. The system prevents overlapping memberships for the same member, ensuring data integrity. When you assign a plan, the expiry date is automatically calculated based on the plan's duration.

### EPIC 6: Payments
Record payments manually with support for partial payments. This is useful when customers pay in installments. The system tracks payment history for each membership, so you always know what's been paid and what's outstanding.

### EPIC 7: Renewal & Expiry Logic
Automated background jobs run daily to check for expired memberships and update their status. You can also query for memberships that are expiring soon, which helps with proactive renewal reminders.

### EPIC 8: Notifications
The system includes a notification service that can send real-time updates to users via SignalR. The architecture is designed to easily integrate with SMS or WhatsApp providers in the future. Notifications are stored in the database and can be marked as read/unread.

### EPIC 9: Dashboard
Get a quick overview of your business with the dashboard. It shows active vs expired members, upcoming renewals, monthly collection summaries, and outstanding payments. This gives you a snapshot of your business health at a glance.

### EPIC 10: Receipts
Generate receipts for payments in HTML format. The system is designed to support PDF generation (currently a placeholder) and shareable links for receipts. This makes it easy to provide customers with payment confirmations.

## API Response Format

All API endpoints return responses in a consistent format for easier client-side handling:

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Operation completed successfully",
  "data": { /* your data here */ },
  "errors": null,
  "timestamp": "2024-12-24T10:30:00Z"
}
```

This standardization makes it easier for frontend developers to handle responses and errors consistently across all endpoints.

## Getting Started

### Prerequisites

Before you begin, make sure you have the following installed:

- .NET 8 SDK (you can download it from [Microsoft's website](https://dotnet.microsoft.com/download))
- PostgreSQL or MySQL database server
- A code editor like Visual Studio 2022, VS Code, or Rider

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/kibriaislam/Subscription-Membership_Management_SaaS.git
   cd Subscription-Membership_Management
   ```

2. **Configure your database**
   Open `src/API/appsettings.json` and update the connection string with your database credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=subscriptionmanagement;User=your_user;Password=your_password"
     },
     "Database": {
       "Provider": "MySQL"  // or "PostgreSQL"
     }
   }
   ```

3. **Set up JWT authentication**
   In the same `appsettings.json` file, update the JWT key with a secure random string (at least 32 characters):
   ```json
   {
     "Jwt": {
       "Key": "YourSecureRandomKeyThatIsAtLeast32CharactersLong!",
       "Issuer": "SubscriptionManagement",
       "Audience": "SubscriptionManagement",
       "ExpiryHours": "24"
     }
   }
   ```

4. **Run database migrations** (optional)
   The application uses `EnsureCreated()` which will automatically create the database schema on first run. However, if you prefer using migrations:
   ```bash
   cd src/API
   dotnet ef migrations add InitialCreate --project ../Infrastructure --startup-project .
   dotnet ef database update
   ```

5. **Start the application**
   ```bash
   cd src/API
   dotnet run
   ```

6. **Access the API documentation**
   Once the application is running, navigate to `https://localhost:5001/swagger` in your browser. You'll see the interactive Swagger UI where you can explore all available endpoints and test them directly.

## API Endpoints Overview

### Authentication
- `POST /api/auth/register` - Create a new user account and business profile
- `POST /api/auth/login` - Authenticate and receive a JWT token

### Business Management
- `GET /api/business` - Retrieve your business profile
- `PUT /api/business` - Update business information

### Member Management
- `GET /api/members` - Get a paginated list of members (supports search)
- `POST /api/members` - Add a new member
- `PUT /api/members/{id}` - Update member information
- `DELETE /api/members/{id}` - Deactivate a member

### Subscription Plans
- `GET /api/plans` - Get all subscription plans
- `POST /api/plans` - Create a new subscription plan
- `PUT /api/plans/{id}` - Update an existing plan

### Memberships
- `GET /api/memberships` - Get memberships (optionally filtered by member)
- `POST /api/memberships` - Assign a subscription plan to a member

### Payments
- `GET /api/payments` - Get payment records (optionally filtered by membership)
- `POST /api/payments` - Record a new payment

### Dashboard
- `GET /api/dashboard/stats` - Get business statistics and metrics

### Renewals
- `GET /api/renewals/expiring` - Get memberships expiring within a specified number of days

### Notifications
- `GET /api/notifications` - Get user notifications
- `GET /api/notifications/unread/count` - Get count of unread notifications
- `POST /api/notifications/{id}/read` - Mark a notification as read
- `POST /api/notifications/read-all` - Mark all notifications as read

### Receipts
- `GET /api/receipts/{paymentId}/html` - Generate HTML receipt
- `GET /api/receipts/{paymentId}/pdf` - Generate PDF receipt (placeholder)
- `GET /api/receipts/{paymentId}/link` - Get shareable receipt link

## Security Features

The application includes several security measures to protect your data:

- **JWT-based authentication** - Secure token-based authentication that doesn't require server-side session storage
- **Multi-tenant data isolation** - Each business's data is completely isolated using BusinessId filtering
- **Password hashing** - All passwords are hashed using BCrypt before storage
- **Input validation** - Comprehensive validation using FluentValidation to prevent invalid data entry
- **Soft delete** - Records are marked as deleted rather than physically removed, maintaining data integrity and audit trails

## Logging

The application uses Serilog for structured logging. Logs are written to both the console and daily rolling log files in the `logs` directory. Each log file is named with the date (e.g., `app-2024-12-24.log`) and is retained for 30 days.

Log levels are configured per environment:
- **Development**: Information level for detailed debugging
- **Production**: Warning level to reduce noise

## Background Jobs

The system uses Hangfire for background job processing:

- **MembershipExpiryJob**: Runs daily at 2:00 AM UTC to automatically mark expired memberships. This ensures your data stays current without manual intervention.

You can monitor and manage background jobs through the Hangfire dashboard at `/hangfire` when the application is running.

## Database Design

The database is designed with performance and data integrity in mind:

- Proper indexing on frequently queried fields (BusinessId, ExpiryDate, MemberId)
- Soft delete support for data retention and audit purposes
- Migration-ready structure for easy database updates
- Support for both PostgreSQL and MySQL

## Testing Considerations

The architecture is designed to be testable:

- Domain logic can be tested in isolation without any dependencies
- Application handlers can be tested with mocked repositories
- Infrastructure layer can be tested using in-memory databases
- API controllers can be tested with integration tests

## Production Deployment Checklist

Before deploying to production, consider the following:

### Security
- Generate a strong, random JWT key (at least 32 characters)
- Use environment variables or Azure Key Vault for sensitive configuration
- Ensure HTTPS is enabled and properly configured
- Review and restrict CORS settings to only allow your frontend domains
- Regularly update dependencies to patch security vulnerabilities

### Database
- Set up connection pooling for better performance
- Configure automated database backups
- Review and optimize indexes based on query patterns
- Consider read replicas for high-traffic scenarios

### Notifications
- Integrate with an actual SMS/WhatsApp provider (Twilio, WhatsApp Business API, etc.)
- Create notification templates for different scenarios
- Set up retry logic for failed notifications

### Receipts
- Implement PDF generation using a library like QuestPDF or iTextSharp
- Set up secure, time-limited shareable links for receipts
- Consider storing generated PDFs in cloud storage (Azure Blob, AWS S3)

### Monitoring
- Set up application insights or similar monitoring tools
- Configure health checks for load balancers
- Implement error tracking (Sentry, Application Insights, etc.)
- Set up alerts for critical errors or performance issues
- Monitor background job execution and failures

### Performance
- Enable response compression
- Configure caching where appropriate
- Review and optimize database queries
- Consider implementing API rate limiting

## Additional Resources

- The application follows Clean Architecture principles as described in Robert C. Martin's book
- CQRS pattern implementation using MediatR
- Swagger UI is available at `/swagger` for API documentation and testing

## License

This is a production-ready MVP designed for commercial use. Feel free to use it as a starting point for your own subscription management system.

---

If you have questions or need help, feel free to open an issue on the repository or reach out to the development team.
