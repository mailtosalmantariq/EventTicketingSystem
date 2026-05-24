# EventTicketing API

A modular .NET API for managing events, tickets, reservations, and purchases. Built with Clean Architecture and Entity Framework Core.

## Overview
The EventTicketing API provides functionality for:
- Creating and retrieving events
- Viewing event tickets
- Reserving tickets
- Purchasing tickets
- Ensuring safe updates using EF Core optimistic concurrency

## Project Structure
EventTicketingSystem/
|
|-- EventTickets.API/          # API layer (controllers, requests, responses)
|-- EventTickets.Application/  # Business logic, interfaces, exceptions
|-- EventTickets.Domain/       # Entities, enums, domain models
|-- EventTickets.Data/         # EF Core DbContext, repositories, migrations
|-- EventTickets.Middleware/   # Global exceptions, extensions
|-- EventTickets.Tests/        # Unit tests for controllers & repositories

## Architecture

### API Layer
- Controllers
- Request/Response DTOs
- Validation

### Application Layer
- Repository interfaces
- Unit of Work
- Exceptions
- Time provider abstraction

### Domain Layer
- Entities: Event, Ticket
- Enums: TicketStatus
- Concurrency token: RowVersion

### Data Layer
- EF Core DbContext
- Repository implementations
- SQLite database

## Ticket Lifecycle
| Status    | Description                |
|-----------|----------------------------|
| Available | Ticket can be reserved     |
| Reserved  | Ticket is temporarily held |
| Sold      | Ticket is purchased        |

## Concurrency Handling
The API uses EF Core optimistic concurrency:

[Timestamp]
public byte[]? RowVersion { get; set; }

This ensures:
- Two users cannot reserve the same ticket
- EF Core throws a concurrency exception
- API returns a 409 Conflict

## API Endpoints

### Events
| Method | Endpoint                | Description            |
|--------|--------------------------|------------------------|
| GET    | /api/events/{eventId}   | Get event with tickets |

### Tickets
| Method | Endpoint                                             | Description               |
|--------|-------------------------------------------------------|---------------------------|
| POST   | /api/events/{eventId}/tickets/reserve                | Reserve a ticket          |
| POST   | /api/events/{eventId}/tickets/{ticketId}/purchase    | Purchase a reserved ticket |

## Example Requests

### Reserve Ticket
POST /api/events/1/tickets/reserve
{
  "holderName": "John Doe"
}

### Purchase Ticket
POST /api/events/1/tickets/5/purchase
{
  "holderName": "John Doe"
}

## Testing
The solution includes:
- Controller tests
- Repository tests
- Concurrency tests

Run tests:
dotnet test

## Running the API

Restore packages:
dotnet restore

Apply migrations:
dotnet ef database update

Run the API:
dotnet run --project EventTickets.API

Swagger UI:
https://localhost:5001/swagger

## Technologies Used
- .NET 10
- Entity Framework Core
- SQLite
- NUnit + FluentAssertions + Moq
- Clean Architecture


