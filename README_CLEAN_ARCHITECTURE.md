# RentSystem - Clean Architecture

A modern, scalable Rent/Property Management System built with .NET 9.0 following Clean Architecture principles.

## 🏗️ Architecture Overview

This project follows Clean Architecture (Onion Architecture) with clear separation of concerns:

```
RentSystem/
├── src/
│   ├── RentSystem.Domain/          # Enterprise Business Rules
│   │   ├── Entities/               # Domain entities
│   │   ├── Interfaces/             # Repository interfaces
│   │   └── Common/                 # Base classes & interfaces
│   │
│   ├── RentSystem.Application/     # Application Business Rules
│   │   ├── DTOs/                   # Data Transfer Objects
│   │   ├── Interfaces/             # Service interfaces
│   │   └── Services/               # Business logic services
│   │
│   ├── RentSystem.Infrastructure/  # External Concerns
│   │   ├── Data/                   # Database context
│   │   ├── Repositories/           # Repository implementations
│   │   └── Services/               # Infrastructure services
│   │
│   └── RentSystem.API/             # Presentation Layer
│       ├── Controllers/            # API endpoints
│       ├── Program.cs              # DI configuration
│       └── appsettings.json        # Configuration
```

## 🎯 Key Architectural Principles

### 1. **Dependency Inversion**
- Inner layers define interfaces
- Outer layers implement them
- Dependencies flow inward
- Domain has zero dependencies

### 2. **Separation of Concerns**
- **Domain Layer**: Pure business entities and rules
- **Application Layer**: Use cases and business logic
- **Infrastructure Layer**: Data access and external services
- **API Layer**: HTTP endpoints and request/response handling

### 3. **SOLID Principles**
- ✅ Single Responsibility Principle
- ✅ Open/Closed Principle
- ✅ Liskov Substitution Principle
- ✅ Interface Segregation Principle
- ✅ Dependency Inversion Principle

## 📦 Layers Description

### Domain Layer (`RentSystem.Domain`)
**Purpose**: Contains enterprise-wide business rules and entities

**Components**:
- **Entities**: Core business objects (User, Property, Booking, Review)
- **Interfaces**: Repository contracts (IUserRepository, IPropertyRepository, etc.)
- **Common**: Base classes (BaseEntity, IAuditableEntity)

**Rules**:
- No dependencies on other layers
- No framework-specific code
- Pure C# classes
- Contains only business logic

### Application Layer (`RentSystem.Application`)
**Purpose**: Contains application-specific business rules

**Components**:
- **DTOs**: Request/Response models for data transfer
- **Interfaces**: Service contracts (IAuthenticationService, IPropertyService, etc.)
- **Services**: Business logic implementation

**Responsibilities**:
- Orchestrates domain objects
- Implements use cases
- Validates business rules
- Transforms domain entities to DTOs

**Dependencies**: Only depends on Domain layer

### Infrastructure Layer (`RentSystem.Infrastructure`)
**Purpose**: Implements interfaces defined in inner layers

**Components**:
- **Data**: EF Core DbContext and configurations
- **Repositories**: Data access implementations
- **Services**: External service implementations (TokenService, PasswordHasher)

**Responsibilities**:
- Database operations
- External API integrations
- File system operations
- Third-party service integrations

**Dependencies**: Depends on Domain and Application layers

### API Layer (`RentSystem.API`)
**Purpose**: HTTP API endpoints and request handling

**Components**:
- **Controllers**: API endpoints
- **Program.cs**: Dependency injection setup
- **Middleware**: Request pipeline configuration

**Responsibilities**:
- HTTP request/response handling
- Authentication/Authorization
- API documentation (Swagger)
- CORS configuration

**Dependencies**: Depends on all inner layers

## 🔧 Technologies Used

- **.NET 9.0**: Latest framework features
- **Entity Framework Core 9.0**: ORM for data access
- **SQL Server**: Relational database
- **JWT Authentication**: Secure token-based auth
- **Swagger/OpenAPI**: API documentation
- **Repository Pattern**: Data access abstraction
- **Unit of Work Pattern**: Transaction management

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the repository**
   ```bash
   cd c:\Users\ka\Desktop\CS\RentSystem\RentApi
   ```

2. **Update connection string**
   Edit `src/RentSystem.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Your-Connection-String"
   }
   ```

3. **Run migrations**
   ```bash
   cd src/RentSystem.API
   dotnet ef migrations add InitialCreate --project ../RentSystem.Infrastructure
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/RentSystem.API
   ```

5. **Access Swagger UI**
   Navigate to: `https://localhost:5001/swagger`

## 📚 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - User logout
- `POST /api/auth/request-password-reset` - Request password reset
- `POST /api/auth/reset-password` - Reset password

### Properties
- `GET /api/properties` - Get all properties
- `GET /api/properties/{id}` - Get property by ID
- `GET /api/properties/owner/{ownerId}` - Get properties by owner
- `GET /api/properties/location/{location}` - Search by location
- `POST /api/properties` - Create property (Owner/Admin)
- `PUT /api/properties/{id}` - Update property (Owner/Admin)
- `DELETE /api/properties/{id}` - Delete property (Owner/Admin)

### Bookings
- `GET /api/bookings/{id}` - Get booking by ID
- `GET /api/bookings/property/{propertyId}` - Get bookings by property
- `GET /api/bookings/renter/{renterId}` - Get bookings by renter
- `GET /api/bookings/my-bookings` - Get current user's bookings
- `POST /api/bookings` - Create booking (Renter)
- `PUT /api/bookings/{id}` - Update booking
- `DELETE /api/bookings/{id}` - Cancel booking

### Reviews
- `GET /api/reviews/{id}` - Get review by ID
- `GET /api/reviews/property/{propertyId}` - Get reviews for property
- `GET /api/reviews/user/{userId}` - Get reviews by user
- `POST /api/reviews` - Create review
- `DELETE /api/reviews/{id}` - Delete review

## 🔐 Authentication & Authorization

The system uses JWT (JSON Web Tokens) for authentication:

### User Roles
- **Renter**: Can book properties and leave reviews
- **Owner**: Can create and manage properties
- **Admin**: Full system access

### Token Management
- **Access Token**: Short-lived (60 minutes)
- **Refresh Token**: Long-lived (7 days), stored in HTTP-only cookie

## 🎨 Design Patterns Used

1. **Repository Pattern**: Abstracts data access
2. **Unit of Work Pattern**: Manages transactions
3. **Dependency Injection**: Loose coupling
4. **Service Layer Pattern**: Business logic separation
5. **DTO Pattern**: Data transfer abstraction
6. **Factory Pattern**: Object creation (implicitly through DI)

## 📊 Database Schema

### Core Entities
- **Users**: User accounts with roles
- **Properties**: Rental properties
- **Bookings**: Property reservations
- **Reviews**: Property reviews
- **RefreshTokens**: Token management
- **PasswordResetCodes**: Password recovery

## 🧪 Testing Strategy

### Unit Tests (Recommended)
- Domain layer: Entity validation
- Application layer: Service logic
- Infrastructure layer: Repository operations

### Integration Tests (Recommended)
- API endpoints
- Database operations
- Authentication flows

## 📈 Best Practices Implemented

1. ✅ **Clean Architecture**: Clear layer separation
2. ✅ **SOLID Principles**: Maintainable, extensible code
3. ✅ **Repository Pattern**: Data access abstraction
4. ✅ **Unit of Work**: Transaction management
5. ✅ **DTOs**: API contract separation
6. ✅ **Dependency Injection**: Loose coupling
7. ✅ **Async/Await**: Non-blocking operations
8. ✅ **JWT Authentication**: Secure API access
9. ✅ **Password Hashing**: Secure password storage
10. ✅ **API Documentation**: Swagger/OpenAPI

## 🔄 Migration from Old Structure

The original monolithic structure has been refactored into Clean Architecture:

**Before**:
```
RentApi/
├── Controllers/
├── Models/
├── Dtos/
├── Helpers/
└── Migrations/
```

**After**:
```
RentSystem/
├── Domain/ (Pure business logic)
├── Application/ (Use cases)
├── Infrastructure/ (Implementation details)
└── API/ (Presentation)
```

## 🚦 Next Steps & Improvements

1. **Add Unit Tests**: Test services and repositories
2. **Add Integration Tests**: Test API endpoints
3. **Implement CQRS**: Separate read and write operations
4. **Add MediatR**: Request/response pipeline
5. **Add FluentValidation**: Enhanced input validation
6. **Implement Caching**: Redis for performance
7. **Add Logging**: Serilog for structured logging
8. **Add Health Checks**: Monitor API health
9. **Implement Rate Limiting**: Protect against abuse
10. **Add API Versioning**: Support multiple API versions

## 📝 License

This project is for educational purposes.

## 👥 Contributing

Contributions are welcome! Please follow Clean Architecture principles.

---

**Built with ❤️ using Clean Architecture and .NET 9.0**
