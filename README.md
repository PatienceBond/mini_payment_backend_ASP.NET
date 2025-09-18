
# Mini Payment Gateway API

A .NET 9 Web API project that implements a simple payment gateway for processing card payments with MySQL database integration.

## Features

- **Payment Processing**: Create payment requests with card validation
- **Transaction Management**: Store and retrieve payment transactions
- **Mock Bank Integration**: Simulates bank responses based on card number logic
- **MySQL Database**: Entity Framework Core with MySQL integration
- **Swagger Documentation**: Interactive API documentation
- **Validation**: Comprehensive input validation and error handling

## Prerequisites

- .NET 9 SDK
- MySQL Server (XAMPP recommended)
- Visual Studio 2022 or VS Code

## Database Setup

1. **Start MySQL Server** (XAMPP):
   - Start XAMPP Control Panel
   - Start MySQL service

2. **Create Database**:
   ```sql
   CREATE DATABASE mini_payment_gateway;
   ```

3. **Verify Connection**:
   - Database: `mini_payment_gateway`
   - Username: `root`
   - Password: `` (empty by default)
   - Host: `localhost`
   - Port: `3306`

## Project Setup

1. **Clone/Download the project** to your local machine

2. **Restore packages**:
   ```bash
   dotnet restore
   ```

3. **Create and apply migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

5. **Access the API**:
   - API Base URL: `https://localhost:7000` or `http://localhost:5000`
   - Swagger UI: `https://localhost:7000` (root URL)

## API Endpoints

### POST /api/payments
Create a new payment request.

**Request Body**:
```json
{
  "cardNumber": "4111111111111111",
  "expiryMonth": 12,
  "expiryYear": 2025,
  "cvv": "123",
  "amount": 100.00,
  "currencyCode": "USD"
}
```

**Response** (201 Created):
```json
{
  "transactionId": "12345678-1234-1234-1234-123456789012",
  "status": "Success"
}
```

### GET /api/payments/{transactionId}
Get a specific transaction by ID.

**Response** (200 OK):
```json
{
  "transactionId": "12345678-1234-1234-1234-123456789012",
  "maskedCardNumber": "************1111",
  "status": "Success",
  "amount": 100.00,
  "currencyCode": "USD",
  "bankMessage": "Payment approved",
  "processedAt": "2025-01-18T10:30:00Z"
}
```

### GET /api/payments
Get all transactions.

**Response** (200 OK):
```json
[
  {
    "transactionId": "12345678-1234-1234-1234-123456789012",
    "maskedCardNumber": "************1111",
    "status": "Success",
    "amount": 100.00,
    "currencyCode": "USD",
    "bankMessage": "Payment approved",
    "processedAt": "2025-01-18T10:30:00Z"
  }
]
```

## Business Logic

### Payment Processing
- **Card Validation**: Ensures all required fields are provided and valid
- **Expiry Check**: Validates that the card hasn't expired
- **Amount Validation**: Ensures amount is greater than 0
- **Bank Simulation**: If the last digit of the card number is even → Success, else → Failed

### Data Models

#### PaymentRequest Table
- `Id` (GUID, Primary Key)
- `CardNumber` (string, max 19 characters)
- `ExpiryMonth` (int, 1-12)
- `ExpiryYear` (int, 2024-2099)
- `Cvv` (string, max 4 characters)
- `Amount` (decimal 10,2)
- `CurrencyCode` (string, 3 characters)
- `CreatedAt` (DateTime, auto-generated)

#### Transaction Table
- `Id` (GUID, Primary Key)
- `PaymentRequestId` (GUID, Foreign Key)
- `MaskedCardNumber` (string, shows only last 4 digits)
- `Status` (string: "Success" or "Failed")
- `BankMessage` (string, bank response message)
- `ProcessedAt` (DateTime, auto-generated)

## Testing the API

### Using Swagger UI
1. Navigate to the root URL when the app is running
2. Use the interactive Swagger interface to test endpoints
3. Click "Try it out" on any endpoint to test

### Using curl
```bash
# Create a payment
curl -X POST "https://localhost:7000/api/payments" \
  -H "Content-Type: application/json" \
  -d '{
    "cardNumber": "4111111111111111",
    "expiryMonth": 12,
    "expiryYear": 2025,
    "cvv": "123",
    "amount": 100.00,
    "currencyCode": "USD"
  }'

# Get a transaction
curl -X GET "https://localhost:7000/api/payments/{transactionId}"

# Get all transactions
curl -X GET "https://localhost:7000/api/payments"
```

## Project Structure

```
MiniPaymentGateway/
├── Controllers/
│   └── PaymentsController.cs
├── Data/
│   └── PaymentDbContext.cs
├── DTOs/
│   ├── PaymentRequestDto.cs
│   ├── PaymentResponseDto.cs
│   └── TransactionDetailsDto.cs
├── Models/
│   ├── PaymentRequest.cs
│   └── Transaction.cs
├── Services/
│   ├── IAcquiringBankService.cs
│   ├── MockAcquiringBankService.cs
│   └── PaymentService.cs
├── Program.cs
├── appsettings.json
└── README.md
```

## Error Handling

The API returns appropriate HTTP status codes:
- `200 OK`: Successful GET requests
- `201 Created`: Successful payment creation
- `400 Bad Request`: Validation errors or invalid input
- `404 Not Found`: Transaction not found
- `500 Internal Server Error`: Server-side errors

Error responses include descriptive messages:
```json
{
  "error": "Card has expired"
}
```

## Development Notes

- The application automatically creates the database and tables on startup
- All timestamps are stored in UTC
- Card numbers are masked for security (only last 4 digits visible)
- The mock bank service simulates real-world processing delays
- CORS is enabled for development purposes

## Troubleshooting

### Database Connection Issues
- Ensure MySQL is running in XAMPP
- Verify connection string in `appsettings.json`
- Check that the database `mini_payment_gateway` exists

### Migration Issues
- Delete the `Migrations` folder and run `dotnet ef migrations add InitialCreate` again
- Ensure Entity Framework tools are installed: `dotnet tool install --global dotnet-ef`

### Port Issues
- The app runs on HTTPS port 7000 and HTTP port 5000 by default
- Check if ports are available or modify in `launchSettings.json`
