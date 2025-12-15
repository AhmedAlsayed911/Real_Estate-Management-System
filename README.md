# RentSystem API - Clean Architecture Implementation

## �‍💻 Developer
**Ahmed Elsayed** - <a href="https://www.linkedin.com/in/ahmed-elsayed-969307375/" target="_blank" rel="noopener noreferrer" title="LinkedIn Profile">LinkedIn Profile</a>

## �🚀 Live Repository
**GitHub:** https://github.com/AhmedAlsayed911/Real_Estate-Management-System

## 📖 API Documentation & Testing

### Quick Start
1. Clone the repository
2. Create database:
   ```bash
   dotnet ef migrations add InitialCreate --project src/RentSystem.Infrastructure --startup-project src/RentSystem.API
   dotnet ef database update --project src/RentSystem.Infrastructure --startup-project src/RentSystem.API
   ```
3. Run the API:
   ```bash
   dotnet run --project src/RentSystem.API
   ```
4. Access Swagger: https://localhost:5001/swagger

### 📝 HTTP Requests Collection
All API endpoints are documented in **`API.http`** file with 42 ready-to-use requests.

**Quick Test Workflow:**
1. Register users (Admin, Owner, Renter) - Requests #1-3
2. Login to get JWT token - Requests #4-6
3. Copy the `accessToken` from login response
4. Test authenticated endpoints using the token

### 🔗 Available Endpoints

#### Authentication (4 endpoints)
- `POST /api/auth/register` - Register new user with role
- `POST /api/auth/login` - Login (returns JWT + sets refresh token cookie)
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - Logout and revoke tokens

#### Properties (7 endpoints)
- `GET /api/properties` - List all properties (Public)
- `GET /api/properties/{id}` - Get property details (Public)
- `GET /api/properties/owner/{ownerId}` - Get properties by owner (Auth)
- `GET /api/properties/location/{location}` - Search by location (Public)
- `POST /api/properties` - Create property (Owner/Admin only)
- `PUT /api/properties/{id}` - Update property (Owner/Admin only)
- `DELETE /api/properties/{id}` - Delete property (Owner/Admin only)

#### Bookings (7 endpoints)
- `GET /api/bookings/{id}` - Get booking details (Auth)
- `GET /api/bookings/property/{propertyId}` - Get bookings for property (Owner/Admin)
- `GET /api/bookings/renter/{renterId}` - Get bookings by renter (Auth)
- `GET /api/bookings/my-bookings` - Get current user's bookings (Auth)
- `POST /api/bookings` - Create booking (Renter/Admin only)
- `PUT /api/bookings/{id}` - Update booking (Auth)
- `DELETE /api/bookings/{id}` - Delete booking (Auth)

#### Reviews (5 endpoints)
- `GET /api/reviews/{id}` - Get review (Public)
- `GET /api/reviews/property/{propertyId}` - Get reviews for property (Public)
- `GET /api/reviews/user/{userId}` - Get reviews by user (Public)
- `POST /api/reviews` - Create review (Auth)
- `DELETE /api/reviews/{id}` - Delete review (Auth)

## 🎯 Example Requests

### 1. Register a User
```http
POST https://localhost:5001/api/auth/register
Content-Type: multipart/form-data

FirstName=John
LastName=Doe
Email=john@example.com
Password=Test@123456
Role=renter
```

### 2. Login
```http
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Test@123456"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 3. Create Property (Requires Auth)
```http
POST https://localhost:5001/api/properties
Authorization: Bearer {your-token-here}
Content-Type: multipart/form-data

Title=Beach House
Description=Beautiful oceanfront property
Location=Miami, FL
PricePerNight=250.00
```

### 4. Search Properties by Location
```http
GET https://localhost:5001/api/properties/location/Miami
```

### 5. Create Booking
```http
POST https://localhost:5001/api/bookings
Authorization: Bearer {your-token-here}
Content-Type: application/json

{
  "propertyId": 1,
  "startDate": "2025-12-20T14:00:00",
  "endDate": "2025-12-25T11:00:00"
}
```

### 6. Create Review
```http
POST https://localhost:5001/api/reviews
Authorization: Bearer {your-token-here}
Content-Type: application/json

{
  "propertyId": 1,
  "rating": 5,
  "comment": "Amazing property! Highly recommend."
}
```

## 🔒 Authentication Flow

1. **Register** → Get success message
2. **Login** → Receive:
   - `accessToken` (JWT) in response body - use for API calls
   - `refreshToken` (7 days) in HTTP-only cookie - automatic
3. **Use Access Token** → Add to requests: `Authorization: Bearer {token}`
4. **Token Expired?** → Call `/api/auth/refresh` (uses cookie automatically)
5. **Logout** → Call `/api/auth/logout` to revoke tokens

## 👥 User Roles & Permissions

### Admin
- Full access to all endpoints
- Can manage all properties, bookings, and reviews

### Owner
- Create, update, delete own properties
- View bookings for own properties
- View all reviews

### Renter
- Create bookings for any property
- View own bookings
- Create reviews for properties

### Agent
- View all properties
- Assist with bookings
- View reviews

## 🏗️ Architecture

### Clean Architecture (4 Layers)
```
┌─────────────────────────────────────┐
│         API Layer (HTTP)            │
│  Controllers, Middleware, Swagger   │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Application Layer              │
│  Services, DTOs, Interfaces         │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│    Infrastructure Layer             │
│ Repositories, DbContext, Services   │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│       Domain Layer (Core)           │
│   Entities, Interfaces, Rules       │
└─────────────────────────────────────┘
```

### Key Patterns
- **Repository Pattern**: Abstracts data access
- **Unit of Work Pattern**: Manages transactions
- **Dependency Inversion**: All dependencies through interfaces
- **SOLID Principles**: Applied throughout

## 🔐 Security Features

✅ **Password Security**
- PBKDF2 hashing algorithm
- 10,000 iterations
- SHA256 hash function
- 16-byte salt per password

✅ **Token Security**
- JWT access tokens (configurable expiry)
- Refresh tokens in HTTP-only cookies
- Secure and SameSite flags enabled
- Automatic token revocation on refresh

✅ **Authorization**
- Role-based access control
- Endpoint-level authorization
- Email uniqueness validation

## 📦 Technology Stack

- **.NET 9.0** - Latest framework
- **Entity Framework Core 9.0.7** - ORM
- **SQL Server** - Database
- **JWT Bearer** - Authentication
- **Swagger/OpenAPI** - API documentation
- **PBKDF2** - Password hashing

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server or SQL Server LocalDB
- Git
- VS Code or Visual Studio 2022

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/AhmedAlsayed911/Real_Estate-Management-System.git
   cd Real_Estate-Management-System
   ```

2. **Update connection string** (if needed)
   Edit `src/RentSystem.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RentSystemDb;Trusted_Connection=true;TrustServerCertificate=true"
   }
   ```

3. **Create database**
   ```bash
   dotnet ef migrations add InitialCreate --project src/RentSystem.Infrastructure --startup-project src/RentSystem.API
   dotnet ef database update --project src/RentSystem.Infrastructure --startup-project src/RentSystem.API
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run the API**
   ```bash
   dotnet run --project src/RentSystem.API
   ```

6. **Access Swagger UI**
   - HTTPS: https://localhost:5001/swagger
   - HTTP: http://localhost:5000/swagger

### Testing with API.http

1. Open `API.http` in VS Code (requires REST Client extension)
2. Click "Send Request" on any endpoint
3. For authenticated endpoints:
   - First run a login request (#4, #5, or #6)
   - Copy the `accessToken` from response
   - Update `@accessToken` variable at top of file
   - Run authenticated requests

## 📊 Project Structure

```
RentSystem/
├── src/
│   ├── RentSystem.Domain/              # Core business entities
│   │   ├── Entities/                   # Domain models
│   │   ├── Interfaces/                 # Repository contracts
│   │   └── Common/                     # Shared abstractions
│   │
│   ├── RentSystem.Application/         # Business logic
│   │   ├── Services/                   # Business services
│   │   ├── Interfaces/                 # Service contracts
│   │   └── DTOs/                       # Data transfer objects (12 DTOs)
│   │
│   ├── RentSystem.Infrastructure/      # External concerns
│   │   ├── Data/                       # DbContext
│   │   ├── Repositories/               # Data access implementations
│   │   ├── Services/                   # External service implementations
│   │   └── Configuration/              # Settings classes
│   │
│   └── RentSystem.API/                 # HTTP interface
│       ├── Controllers/                # API endpoints
│       ├── Program.cs                  # DI configuration
│       └── appsettings.json            # Configuration
│
├── API.http                            # 42 example requests
├── RentSystem.sln                      # Solution file
└── README.md                           # This file
```

## 🧪 Testing the API

### Using Swagger UI (Recommended for beginners)

1. Run the API: `dotnet run --project src/RentSystem.API`
2. Open browser: https://localhost:5001/swagger
3. Click "Authorize" button (top right)
4. Register a user via `/api/auth/register`
5. Login via `/api/auth/login` to get token
6. Copy the access token (without quotes)
7. In Swagger, click "Authorize" and paste: `Bearer {your-token}`
8. Now you can test all authenticated endpoints

### Using API.http (Recommended for developers)

1. Install "REST Client" extension in VS Code
2. Open `API.http`
3. Execute requests in order:
   - Register users (#1-3)
   - Login (#4-6) and copy token
   - Update `@accessToken` variable
   - Test features (#9-42)

### Using Postman

1. Import the API.http file or manually create requests
2. Set base URL: `https://localhost:5001`
3. For authenticated requests:
   - Add header: `Authorization: Bearer {token}`
4. Test all 23 endpoints

## 📈 API Response Examples

### Successful Login
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJqb2huQGV4YW1wbGUuY29tIiwicm9sZSI6InJlbnRlciIsImV4cCI6MTY5OTk5OTk5OX0.xxx"
}
```

### Get All Properties
```json
[
  {
    "id": 1,
    "title": "Luxury Beach House",
    "location": "Miami Beach, FL",
    "pricePerNight": 350.00,
    "mainImageUrl": "base64-encoded-image-here",
    "ownerName": "Sarah Property",
    "bookingCount": 5,
    "averageRating": 4.7
  }
]
```

### Create Booking
```json
{
  "id": 1,
  "startDate": "2025-12-20T14:00:00",
  "endDate": "2025-12-25T11:00:00",
  "propertyTitle": "Luxury Beach House",
  "propertyLocation": "Miami Beach, FL",
  "renterFullName": "Mike Renter"
}
```

### Error Response
```json
{
  "message": "Invalid email or password"
}
```

## 🐛 Troubleshooting

### Database Connection Issues
- Ensure SQL Server LocalDB is installed
- Check connection string in `appsettings.json`
- Run migrations: `dotnet ef database update`

### Authentication Issues
- Ensure token is in format: `Bearer {token}`
- Check token hasn't expired (default: 60 minutes)
- Use refresh endpoint if token expired

### Build Errors
- Ensure .NET 9.0 SDK is installed
- Run `dotnet restore`
- Clean and rebuild: `dotnet clean && dotnet build`

## 📝 Notes

- **Refresh tokens** are stored in HTTP-only cookies for security
- **Image uploads** are converted to Base64 strings
- **Passwords** must meet complexity requirements (uppercase, lowercase, digit, special char)
- **Roles** are validated against configured list: admin, owner, renter, agent
- **Public endpoints** (properties, reviews GET) don't require authentication

## 🤝 Contributing

This is an educational project demonstrating Clean Architecture principles. Feel free to fork and modify for your needs.

## 📄 License

This project is for educational purposes.

---

**Repository:** https://github.com/AhmedAlsayed911/Real_Estate-Management-System  
**All 23 endpoints fully functional** ✅ **Build Status: SUCCESS** ✅
