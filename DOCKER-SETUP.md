# Docker Database Setup for EFCore.BulkExtensions Testing

This docker-compose file sets up the required database instances for running the EFCore.BulkExtensions tests.

## Prerequisites

- Docker Desktop installed and running
- At least 4GB of available RAM for containers

## Databases Included

1. **SQL Server 2022** - Port 1433
   - Username: `sa`
   - Password: `EFCoreBulk123!`

2. **PostgreSQL 16** - Port 5432
   - Username: `postgres`
   - Password: `EFCoreBulk123!`
   - Database: `EFCoreBulkTests`

3. **PostgreSQL with PostGIS** - Port 5433
   - Username: `postgres`
   - Password: `EFCoreBulk123!`
   - Database: `EFCoreBulkTests`
   - Includes spatial extensions for geometry tests

## Starting the Databases

From the repository root directory, run:

```bash
docker-compose up -d
```

This will start all database containers in detached mode.

## Checking Database Status

```bash
docker-compose ps
```

All services should show as "healthy" after a minute or so.

## Stopping the Databases

```bash
docker-compose down
```

To also remove the data volumes:

```bash
docker-compose down -v
```

## Connecting to Databases

### SQL Server
```
Server: localhost,1433
User: sa
Password: EFCoreBulk123!
```

### PostgreSQL
```
Host: localhost
Port: 5432
User: postgres
Password: EFCoreBulk123!
Database: EFCoreBulkTests
```

### PostgreSQL with PostGIS
```
Host: localhost
Port: 5433
User: postgres
Password: EFCoreBulk123!
Database: EFCoreBulkTests
```

## Running Tests

After starting the databases, you can run the tests:

```bash
# Run all tests
dotnet test

# Run specific database tests
dotnet test --filter "FullyQualifiedName~SqlServer"
dotnet test --filter "FullyQualifiedName~PostgreSql"
dotnet test --filter "FullyQualifiedName~Sqlite"
```

## Notes

- SQLite tests don't require Docker - they use a local file database
- MySQL and Oracle are temporarily excluded in the .NET 10 version
- The test settings in `testsettings.json` are configured to use these Docker instances
- Data persists between container restarts via Docker volumes
