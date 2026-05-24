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
│
├── EventTickets.API/                # API layer (controllers, requests, responses)
├── EventTickets.Application/        # Business logic, interfaces, exceptions
├── EventTickets.Domain/             # Entities, enums, domain models
├── EventTickets.Data/               # EF Core DbContext, repositories, migrations
├── EventTickets.Middleware/         # Global exceptions, extensions
└── EventTickets.Tests/              # Unit tests for controllers & repositories

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

