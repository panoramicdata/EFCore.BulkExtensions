# 🎉 NuGet Packages Successfully Published!

## Published Packages (5/5)

All **EFCore.BulkExtensions.Dotnet10** packages have been successfully published to NuGet.org:

### ✅ Main Package
- **EFCore.BulkExtensions.Dotnet10** - Version 10.0.0-rc.1
  - https://www.nuget.org/packages/EFCore.BulkExtensions.Dotnet10/

### ✅ Core Package
- **EFCore.BulkExtensions.Dotnet10.Core** - Version 10.0.0-rc.1
  - https://www.nuget.org/packages/EFCore.BulkExtensions.Dotnet10.Core/

### ✅ Database Providers
- **EFCore.BulkExtensions.Dotnet10.SqlServer** - Version 10.0.0-rc.1
  - https://www.nuget.org/packages/EFCore.BulkExtensions.Dotnet10.SqlServer/

- **EFCore.BulkExtensions.Dotnet10.PostgreSql** - Version 10.0.0-rc.1
  - https://www.nuget.org/packages/EFCore.BulkExtensions.Dotnet10.PostgreSql/

- **EFCore.BulkExtensions.Dotnet10.Sqlite** - Version 10.0.0-rc.1
  - https://www.nuget.org/packages/EFCore.BulkExtensions.Dotnet10.Sqlite/

## Installation

Users can now install these packages using:

```powershell
# Install main package (recommended)
dotnet add package EFCore.BulkExtensions.Dotnet10 --version 10.0.0-rc.1

# Or install specific providers
dotnet add package EFCore.BulkExtensions.Dotnet10.SqlServer --version 10.0.0-rc.1
dotnet add package EFCore.BulkExtensions.Dotnet10.PostgreSql --version 10.0.0-rc.1
dotnet add package EFCore.BulkExtensions.Dotnet10.Sqlite --version 10.0.0-rc.1
```

## Package Manager Console

```powershell
Install-Package EFCore.BulkExtensions.Dotnet10 -Version 10.0.0-rc.1
```

## Search on NuGet.org

Browse all packages:
https://www.nuget.org/packages?q=EFCore.BulkExtensions.Dotnet10

## Package Details

- **Version**: 10.0.0-rc.1 (prerelease)
- **Target Framework**: net10.0
- **EF Core Version**: 10.0.0-rc.2.25502.107
- **Repository**: https://github.com/panoramicdata/EFCore.BulkExtensions
- **Branch**: EFCore10.0
- **Published**: November 12, 2024

## Features

- ✅ Bulk Insert
- ✅ Bulk Update
- ✅ Bulk Delete
- ✅ Bulk Read
- ✅ Bulk Upsert
- ✅ Bulk Sync
- ✅ SaveChanges optimizations

## Supported Databases

- ✅ SQL Server 2016+
- ✅ PostgreSQL 9.5+
- ✅ SQLite
- ⏸️ MySQL (coming when Pomelo releases EF Core 10 support)
- ⏸️ Oracle (coming when Oracle releases EF Core 10 support)

## Notes

- Packages are marked as **prerelease** (-rc.1 suffix)
- Will be updated to stable version when EF Core 10.0 RTM is released
- MySQL and Oracle support temporarily excluded pending provider updates
- All packages are signed and include debug symbols

## Next Steps

1. **Announce** the packages on relevant channels
2. **Monitor** for any issues from early adopters
3. **Update to RTM** when EF Core 10.0 final is released
4. **Re-enable MySQL/Oracle** when providers are updated
5. **Publish documentation** and usage examples

## Support

- GitHub Issues: https://github.com/panoramicdata/EFCore.BulkExtensions/issues
- Docker setup for testing: See DOCKER-SETUP.md
- Migration documentation: See MIGRATION-SUMMARY.md

---

**Thank you for using EFCore.BulkExtensions.Dotnet10!**
