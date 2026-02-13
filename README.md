# Acceloka - Ticket Booking API

A RESTful Web API for online ticket booking system built with ASP.NET Core 10. The system supports various ticket categories including cinema, concerts, trains, ships, flights, and hotels.

## Tech Stack

- **Framework:** ASP.NET Core 10
- **Database:** PostgreSQL 18.1
- **ORM:** Entity Framework Core
- **Architecture:** MARVEL Pattern (MediatR + CQRS)
- **Validation:** FluentValidation
- **Logging:** Serilog
- **Documentation:** Swagger / OpenAPI

## Prerequisites

- .NET 10 SDK
- PostgreSQL 18.1
- Visual Studio 2026 (or any IDE that supports .NET 10)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Budiman002/AccelokaProject.git
cd AccelokaProject
```

### 2. Configure Database Connection

Open `appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=AccelokaDb;Username=your_username;Password=your_password"
  }
}
```

### 3. Apply Migrations

```bash
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

The application will automatically seed the database with initial data on first run.

### 5. Access Swagger UI

```
https://localhost:7039/swagger
```

## API Endpoints

### GET /api/v1/get-available-ticket

Returns a list of available tickets with remaining quota.

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| namaKategori | string | Filter by category name |
| kodeTicket | string | Filter by ticket code |
| namaTicket | string | Filter by ticket name |
| harga | decimal | Filter by price (returns tickets with price <=) |
| tanggalEventMin | datetime | Filter by minimum event date |
| tanggalEventMax | datetime | Filter by maximum event date |
| orderBy | string | Column to sort by (code, name, category, eventdate, price, quota) |
| orderState | string | Sort direction: ASC or DESC (default: ASC) |
| page | int | Page number for pagination |
| pageSize | int | Items per page (default: 10) |

---

### POST /api/v1/book-ticket

Books one or more tickets.

**Request Body:**
```json
{
  "tickets": [
    {
      "kodeTicket": "C001",
      "qty": 2
    },
    {
      "kodeTicket": "H001",
      "qty": 1
    }
  ]
}
```

**Validations:**
- Ticket code must exist in the database
- Ticket quota must not be zero
- Requested quantity must not exceed available quota
- Event date must be after the booking date

---

### GET /api/v1/get-booked-ticket/{BookedTicketId}

Returns the detail of a specific booking, grouped by category.

**Path Parameter:**

| Parameter | Type | Description |
|-----------|------|-------------|
| BookedTicketId | int | The ID of the booking |

---

### DELETE /api/v1/revoke-ticket/{BookedTicketId}/{KodeTicket}/{Qty}

Revokes a specific number of tickets from a booking.

**Path Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| BookedTicketId | int | The ID of the booking |
| KodeTicket | string | The ticket code to revoke |
| Qty | int | Number of tickets to revoke |

**Validations:**
- BookedTicketId must exist
- Ticket code must exist in the booking
- Quantity to revoke must not exceed booked quantity

---

### PUT /api/v1/edit-booked-ticket/{BookedTicketId}

Updates the quantity of one or more tickets in a booking.

**Path Parameter:**

| Parameter | Type | Description |
|-----------|------|-------------|
| BookedTicketId | int | The ID of the booking |

**Request Body:**
```json
{
  "tickets": [
    {
      "ticketCode": "C001",
      "newQuantity": 5
    },
    {
      "ticketCode": "H001",
      "newQuantity": 3
    }
  ]
}
```

**Validations:**
- BookedTicketId must exist
- Ticket code must exist in the booking
- New quantity must not exceed available quota
- New quantity must be at least 1

## Error Handling

All errors follow RFC 7807 standard format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Validation Error",
  "status": 400,
  "errors": {
    "TicketCode": [
      "Ticket code C999 is not registered"
    ]
  }
}
```

## Seed Data

The application seeds the following data on startup:

**Categories:** Cinema, Transportasi Darat, Transportasi Laut, Hotel

**Tickets:**

| Code | Name | Category |
|------|------|----------|
| C001 | Ironman CGV | Cinema |
| C002 | Black Panther | Cinema |
| TD001 | Bus Jawa-Sumatra | Transportasi Darat |
| TL001 | Kapal Ferri Jawa-Sumatra | Transportasi Laut |
| H001 | Ibis Hotel Jakarta 21-23 | Hotel |

## Logging

Serilog is configured to write logs to the `/logs` directory with daily rolling files.

Log file format: `Log-{Date}.txt` (e.g., `Log-20260211.txt`)

Log level: Information

## Author

**Budi**

## Project Documentation

Full project documentation, progress log, and API test results via Postman are available at:

[Documentation - Exam Project G](https://www.notion.so/Documentation-Exam-Project-G-305d097ad30d80749459d8ec87c5a57c)
