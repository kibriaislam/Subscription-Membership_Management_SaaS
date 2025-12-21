# Architecture Documentation

## Overview

This is a **Clean Architecture** implementation of a Subscription & Membership Management SaaS MVP. The architecture strictly follows SOLID principles and Domain-Driven Design (DDD-lite) patterns.

## Layer Responsibilities

### Domain Layer (`src/Domain`)
**Purpose**: Core business logic and entities. No dependencies on other layers.

**Contents**:
- **Entities**: Core domain models (User, Business, Member, SubscriptionPlan, Membership, Payment, AuditLog)
- **Enums**: Domain enumerations (MembershipStatus, PaymentMethod)
- **Interfaces**: Repository and service contracts
- **ValueObjects**: (Ready for future expansion)

**Key Principles**:
- Entities are rich domain models with business logic
- No infrastructure dependencies
- Pure C# classes with no external libraries (except base .NET)

### Application Layer (`src/Application`)
**Purpose**: Application use cases and business logic orchestration.

**Contents**:
- **Features**: CQRS handlers organized by feature (Auth, Business, Members, Plans, etc.)
- **DTOs**: Data Transfer Objects for API communication
- **Interfaces**: Application service contracts
- **Behaviors**: Cross-cutting concerns (Validation, Logging)
- **Validators**: FluentValidation validators

**Key Principles**:
- Uses MediatR for CQRS pattern
- Commands for write operations, Queries for read operations
- All validation happens here via FluentValidation
- No direct database access (uses repositories through interfaces)

### Infrastructure Layer (`src/Infrastructure`)
**Purpose**: External concerns and implementations.

**Contents**:
- **Persistence**: Entity Framework Core DbContext and configurations
- **Repositories**: Concrete implementations of domain repository interfaces
- **Identity**: JWT service, password hashing, current user service
- **Notifications**: Notification service (interface-based, ready for SMS/WhatsApp)
- **Receipts**: Receipt generation service
- **BackgroundJobs**: Hosted services (membership expiry job)

**Key Principles**:
- Implements interfaces defined in Domain/Application layers
- Handles all external dependencies (database, external APIs)
- No business logic here

### API Layer (`src/API`)
**Purpose**: HTTP API entry point.

**Contents**:
- **Controllers**: RESTful API endpoints
- **Middleware**: Exception handling, logging
- **Program.cs**: Application startup and configuration

**Key Principles**:
- Thin controllers that delegate to MediatR
- No business logic in controllers
- Handles HTTP concerns only

## Data Flow

```
HTTP Request
    ↓
API Controller (thin)
    ↓
MediatR Command/Query
    ↓
Application Handler (business logic)
    ↓
Repository Interface (Domain)
    ↓
Repository Implementation (Infrastructure)
    ↓
DbContext (Infrastructure)
    ↓
Database
```

## Multi-Tenancy

**Strategy**: Business-level isolation

- Every entity has a `BusinessId` foreign key
- All queries filter by `BusinessId` from JWT token
- `ICurrentUserService` extracts `BusinessId` from authenticated user's claims
- Data isolation enforced at repository level

## Security

1. **Authentication**: JWT-based
   - Token contains UserId, BusinessId, and role
   - Validated on every request via middleware

2. **Authorization**: Role-based (currently "Owner" only)

3. **Data Isolation**: 
   - All queries automatically filter by BusinessId
   - No cross-business data access possible

4. **Password Security**: BCrypt with work factor 12

5. **Input Validation**: FluentValidation on all commands

## CQRS Pattern

**Commands** (Write Operations):
- `CreateMemberCommand`
- `UpdateMemberCommand`
- `CreateMembershipCommand`
- `CreatePaymentCommand`
- etc.

**Queries** (Read Operations):
- `GetMembersQuery`
- `GetPlansQuery`
- `GetDashboardStatsQuery`
- etc.

**Benefits**:
- Clear separation of read/write operations
- Easy to optimize queries independently
- Scalable architecture

## Repository Pattern

**Generic Repository** (`IRepository<T>`):
- Basic CRUD operations
- Works with any entity

**Specific Repositories**:
- `IUserRepository` - User-specific queries (GetByEmail)
- `IMemberRepository` - Member-specific queries (GetPagedByBusinessId)
- `IMembershipRepository` - Membership-specific queries (GetExpiringWithinDays)
- etc.

**Unit of Work**:
- Manages transactions
- Coordinates multiple repositories
- Ensures data consistency

## Background Jobs

**MembershipExpiryJob**:
- Runs daily (configurable interval)
- Marks expired memberships automatically
- Idempotent (safe to run multiple times)

**Future Jobs**:
- Renewal reminder notifications
- Payment reminders
- Report generation

## Database Design

**Key Features**:
- GUID primary keys for all entities
- Soft delete support (`IsDeleted` flag)
- Audit fields (`CreatedAt`, `UpdatedAt`)
- Proper indexing on:
  - `BusinessId` (multi-tenancy)
  - `ExpiryDate` (membership queries)
  - `MemberId` (member lookups)
  - `Email` (user authentication)

**Relationships**:
- User → Business (1:1)
- Business → Members (1:Many)
- Business → SubscriptionPlans (1:Many)
- Member → Memberships (1:Many)
- Membership → Payments (1:Many)
- Membership → SubscriptionPlan (Many:1)

## Extension Points

### Adding New Features

1. **Domain**: Add entity, enum, or interface
2. **Application**: Create DTOs, Command/Query, Handler, Validator
3. **Infrastructure**: Implement repository if needed
4. **API**: Add controller endpoint

### Adding External Services

1. Define interface in `Application.Interfaces`
2. Implement in `Infrastructure`
3. Register in `Program.cs`

Example: SMS/WhatsApp notifications
- Interface already exists: `INotificationService`
- Implement in `Infrastructure.Notifications`
- Replace placeholder implementation

## Testing Strategy

**Unit Tests**:
- Domain entities and business logic
- Application handlers (with mocked repositories)
- Validators

**Integration Tests**:
- Repository implementations
- End-to-end API tests
- Database operations

**Test Structure** (Recommended):
```
/tests
  /Domain.Tests
  /Application.Tests
  /Infrastructure.Tests
  /API.Tests
```

## Performance Considerations

1. **Database Indexing**: All foreign keys and frequently queried fields indexed
2. **Pagination**: All list endpoints support pagination
3. **Async/Await**: All I/O operations are async
4. **Query Optimization**: Use Include() for eager loading when needed
5. **Caching**: Ready for Redis integration (add caching layer in Infrastructure)

## Deployment Checklist

- [ ] Update JWT key in production config
- [ ] Configure production database connection
- [ ] Set up database migrations
- [ ] Configure CORS for production domains
- [ ] Enable HTTPS
- [ ] Set up logging/monitoring
- [ ] Configure notification service (SMS/WhatsApp)
- [ ] Set up PDF generation library for receipts
- [ ] Configure background job scheduling
- [ ] Set up health checks
- [ ] Configure error tracking (e.g., Sentry)

## Future Enhancements

1. **Caching Layer**: Redis for frequently accessed data
2. **Event Sourcing**: For audit trail and event replay
3. **API Versioning**: Support multiple API versions
4. **Rate Limiting**: Prevent abuse
5. **Webhooks**: Notify external systems of events
6. **Multi-language Support**: i18n for receipts and notifications
7. **Advanced Reporting**: Analytics and insights
8. **Payment Gateway Integration**: Stripe, PayPal, etc.
9. **Email Notifications**: SMTP integration
10. **File Storage**: Receipt storage in cloud storage

