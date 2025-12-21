# Subscription & Membership Management SaaS MVP

A production-ready, multi-tenant SaaS application built with ASP.NET Core 8, following Clean Architecture principles.

## Architecture

This solution follows Clean Architecture with clear separation of concerns:

- **Domain**: Core business entities, interfaces, and domain logic
- **Application**: Use cases, DTOs, validators, and business logic
- **Infrastructure**: Data access, external services, and implementations
- **API**: Controllers, middleware, and API configuration

## Tech Stack

- **.NET 8** - Latest LTS version
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core** - ORM
- **PostgreSQL/MySQL** - Database (configurable)
- **JWT** - Authentication
- **MediatR** - CQRS pattern
- **FluentValidation** - Input validation
- **BCrypt** - Password hashing

## Features

### EPIC 1: Authentication ✅
- User registration
- Login with JWT token
- Password hashing

### EPIC 2: Business Profile ✅
- Create/Update business profile
- Currency support

### EPIC 3: Member Management ✅
- Add/Edit/Deactivate members
- Search & pagination

### EPIC 4: Subscription Plans ✅
- Create/Edit plans
- Price and duration management

### EPIC 5: Membership Assignment ✅
- Assign plans to members
- Auto-calculate expiry dates
- Prevent overlapping memberships

### EPIC 6: Payments ✅
- Manual payment entry
- Partial payments support
- Payment history

### EPIC 7: Renewal & Expiry Logic ✅
- Auto-expire memberships (background job)
- Upcoming renewals tracking

### EPIC 8: Notifications ✅
- Interface-based notification service
- Ready for SMS/WhatsApp integration

### EPIC 9: Dashboard ✅
- Active vs expired members
- Renewals due
- Monthly collection summary

### EPIC 10: Receipts ✅
- HTML receipt generation
- PDF generation (placeholder)
- Shareable links

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL or MySQL
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Subscription&Membership_Management
   ```

2. **Configure database**
   - Update `appsettings.json` with your database connection string
   - Set `Database:Provider` to "PostgreSQL" or "MySQL"

3. **Configure JWT**
   - Update `Jwt:Key` in `appsettings.json` with a secure key (at least 32 characters)

4. **Run migrations**
   ```bash
   cd src/API
   dotnet ef migrations add InitialCreate --project ../Infrastructure --startup-project .
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access Swagger**
   - Navigate to `https://localhost:5001/swagger` (or your configured port)

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token

### Business
- `GET /api/business` - Get business profile
- `PUT /api/business` - Update business profile

### Members
- `GET /api/members` - Get paginated members (with search)
- `POST /api/members` - Create member
- `PUT /api/members/{id}` - Update member
- `DELETE /api/members/{id}` - Deactivate member

### Plans
- `GET /api/plans` - Get all subscription plans
- `POST /api/plans` - Create plan
- `PUT /api/plans/{id}` - Update plan

### Memberships
- `GET /api/memberships` - Get memberships (optional memberId filter)
- `POST /api/memberships` - Create membership

### Payments
- `GET /api/payments` - Get payments (optional membershipId filter)
- `POST /api/payments` - Create payment

### Dashboard
- `GET /api/dashboard/stats` - Get dashboard statistics

### Renewals
- `GET /api/renewals/expiring` - Get expiring memberships

## Security

- JWT-based authentication
- Business data isolation (multi-tenant)
- Password hashing with BCrypt
- Input validation with FluentValidation
- Soft delete for data retention

## Background Jobs

- **MembershipExpiryJob**: Runs daily to mark expired memberships

## Database

The application uses Entity Framework Core with:
- Proper indexing on BusinessId, ExpiryDate, MemberId
- Soft delete support
- Migration-ready structure

## Testing

The architecture supports unit and integration testing:
- Domain logic is testable in isolation
- Application handlers can be tested with mocked repositories
- Infrastructure can be tested with in-memory database

## Production Considerations

Before deploying to production:

1. **Security**
   - Change JWT key to a secure random value
   - Use environment variables for sensitive configuration
   - Enable HTTPS
   - Configure CORS properly

2. **Database**
   - Use connection pooling
   - Set up database backups
   - Configure proper indexes

3. **Notifications**
   - Implement actual SMS/WhatsApp provider
   - Configure notification templates

4. **Receipts**
   - Implement PDF generation library (QuestPDF, iTextSharp)
   - Set up secure shareable link generation

5. **Monitoring**
   - Add logging and monitoring
   - Set up health checks
   - Configure error tracking

## License

This is a production-ready MVP for commercial use.

