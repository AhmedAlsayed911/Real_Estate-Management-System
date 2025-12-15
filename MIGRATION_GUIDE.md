# Clean Architecture Migration Guide

## Step-by-Step Migration Instructions

### Step 1: Restore NuGet Packages
```bash
cd c:\Users\ka\Desktop\CS\RentSystem\RentApi
dotnet restore
```

### Step 2: Build the Solution
```bash
# Build all projects
dotnet build

# Or build individual projects
dotnet build src/RentSystem.Domain/RentSystem.Domain.csproj
dotnet build src/RentSystem.Application/RentSystem.Application.csproj
dotnet build src/RentSystem.Infrastructure/RentSystem.Infrastructure.csproj
dotnet build src/RentSystem.API/RentSystem.API.csproj
```

### Step 3: Update Database Connection String
Edit `src/RentSystem.API/appsettings.json` and update the connection string to match your SQL Server instance.

### Step 4: Create New Migration
```bash
cd src/RentSystem.API
dotnet ef migrations add InitialMigration --project ../RentSystem.Infrastructure --context ApplicationDbContext
```

### Step 5: Update Database
```bash
dotnet ef database update --project ../RentSystem.Infrastructure --context ApplicationDbContext
```

### Step 6: Run the Application
```bash
dotnet run --project src/RentSystem.API
```

### Step 7: Test the API
Open browser and navigate to:
- Swagger UI: `https://localhost:5001/swagger`
- API: `https://localhost:5001/api`

## Troubleshooting

### Issue: Build Errors
**Solution**: Ensure all project references are correct and NuGet packages are restored.

### Issue: Migration Errors
**Solution**: 
1. Delete existing migrations in old Migrations folder
2. Create new migration from scratch
3. Ensure connection string is correct

### Issue: Authentication Not Working
**Solution**: 
1. Check JWT settings in appsettings.json
2. Ensure SigningKey is at least 32 characters
3. Verify token is being passed in Authorization header

## Key Changes from Old Structure

### 1. Namespace Changes
- Old: `RentApi.Models` → New: `RentSystem.Domain.Entities`
- Old: `RentApi.Controllers` → New: `RentSystem.API.Controllers`
- Old: `RentApi.Dtos` → New: `RentSystem.Application.DTOs`

### 2. Database Context
- Old: Direct DbContext in Models folder
- New: Clean DbContext in Infrastructure layer with proper configurations

### 3. Controllers
- Old: Direct DbContext injection in controllers
- New: Service injection following dependency inversion

### 4. Authentication
- Old: Direct password storage
- New: Proper password hashing with PBKDF2

### 5. Data Access
- Old: Direct EF Core queries in controllers
- New: Repository pattern with Unit of Work

## Verification Checklist

- [ ] All projects build successfully
- [ ] Database migrations run without errors
- [ ] Swagger UI loads correctly
- [ ] Authentication endpoints work
- [ ] CRUD operations work for all entities
- [ ] JWT tokens are generated correctly
- [ ] Authorization rules are enforced
- [ ] CORS is configured properly

## Next Steps After Migration

1. **Copy Data** (if needed):
   - Export data from old database
   - Import into new database structure

2. **Update Old Migrations**:
   - Keep old migrations for reference
   - Mark them as applied if database already exists

3. **Test All Endpoints**:
   - Use Swagger UI or Postman
   - Test each endpoint with different roles
   - Verify error handling

4. **Deploy**:
   - Update deployment scripts
   - Update environment variables
   - Test in staging environment

## Directory Structure Comparison

### Old Structure
```
RentApi/
├── Controllers/
│   ├── UserController.cs
│   ├── PropertyController.cs
│   ├── BookingController.cs
│   └── ReviewController.cs
├── Models/
│   ├── User.cs
│   ├── Property.cs
│   ├── Booking.cs
│   ├── Review.cs
│   └── ApplicationDbContext.cs
├── Dtos/
│   └── [Various DTOs]
├── Helpers/
│   └── [Helper classes]
└── Program.cs
```

### New Structure (Clean Architecture)
```
RentSystem/
├── src/
│   ├── RentSystem.Domain/
│   │   ├── Entities/
│   │   ├── Interfaces/
│   │   └── Common/
│   ├── RentSystem.Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Services/
│   ├── RentSystem.Infrastructure/
│   │   ├── Data/
│   │   ├── Repositories/
│   │   └── Services/
│   └── RentSystem.API/
│       ├── Controllers/
│       ├── Program.cs
│       └── appsettings.json
└── RentSystem.sln
```

## Benefits of New Architecture

1. **Better Testability**: Each layer can be tested independently
2. **Flexibility**: Easy to swap implementations (e.g., change database)
3. **Maintainability**: Clear separation of concerns
4. **Scalability**: Easy to add new features without breaking existing code
5. **Team Collaboration**: Different teams can work on different layers
6. **Technology Independence**: Core business logic is framework-agnostic

## Common Patterns Used

### Repository Pattern
```csharp
// Interface in Domain
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

// Implementation in Infrastructure
public class UserRepository : Repository<User>, IUserRepository
{
    // Implementation
}
```

### Unit of Work Pattern
```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IPropertyRepository Properties { get; }
    Task<int> SaveChangesAsync();
}
```

### Service Layer Pattern
```csharp
// Interface in Application
public interface IPropertyService
{
    Task<PropertyDetailsDto?> GetPropertyByIdAsync(int id);
}

// Implementation in Application
public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;
    // Implementation
}
```

## Performance Considerations

1. **Async/Await**: All operations are asynchronous
2. **EF Core Tracking**: Proper use of AsNoTracking for read operations
3. **Eager Loading**: Include related entities when needed
4. **Connection Pooling**: Enabled by default in EF Core
5. **Transaction Management**: Unit of Work handles transactions

## Security Enhancements

1. **Password Hashing**: PBKDF2 with salt
2. **JWT Tokens**: Secure token-based authentication
3. **HTTP-Only Cookies**: Refresh tokens stored securely
4. **Role-Based Authorization**: Proper role enforcement
5. **CORS Configuration**: Controlled cross-origin access

---

For questions or issues, please refer to the main README or create an issue.
