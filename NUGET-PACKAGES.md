# NuGet Package Information

## Package IDs for EFCore.BulkExtensions.Dotnet10

The following NuGet packages need API keys to publish:

### Main Package (Meta-package)
- **EFCore.BulkExtensions.Dotnet10**
  - Contains references to all database-specific packages
  - Users install this for full functionality

### Core Package
- **EFCore.BulkExtensions.Dotnet10.Core**
  - Shared core functionality
  - Database-agnostic bulk operations logic

### Database-Specific Packages
- **EFCore.BulkExtensions.Dotnet10.SqlServer**
  - SQL Server 2016+ support
  - Uses SqlBulkCopy for performance

- **EFCore.BulkExtensions.Dotnet10.PostgreSql**
  - PostgreSQL 9.5+ support
  - Uses COPY BINARY with ON CONFLICT

- **EFCore.BulkExtensions.Dotnet10.Sqlite**
  - SQLite support
  - Optimized for file-based databases

## Package Details

- **Version**: 10.0.0-rc.1 (prerelease)
- **Target Framework**: net10.0
- **EF Core Version**: 10.0.0-rc.2.25502.107
- **License**: Same as original (Dual License - cFOSS)
- **Repository**: https://github.com/panoramicdata/EFCore.BulkExtensions
- **Branch**: EFCore10.0

## NuGet Package Owners

Please set the package owner to the appropriate NuGet.org account.

## Publishing

Once you have the NuGet API key:

1. Add it to `nuget-key.txt` in the repository root
2. Run the publish script:
   ```powershell
   .\publish.ps1
   ```

The script will:
- Build the solution in Release mode
- Find all .nupkg files in Nugets\net10
- Publish each package to NuGet.org
- Handle duplicate version conflicts gracefully

## Installation (After Publishing)

Users can install via:

```powershell
# Install main package (includes all database providers)
dotnet add package EFCore.BulkExtensions.Dotnet10 --version 10.0.0-rc.1

# Or install specific database packages
dotnet add package EFCore.BulkExtensions.Dotnet10.SqlServer --version 10.0.0-rc.1
dotnet add package EFCore.BulkExtensions.Dotnet10.PostgreSql --version 10.0.0-rc.1
dotnet add package EFCore.BulkExtensions.Dotnet10.Sqlite --version 10.0.0-rc.1
```

## Package Search

After publishing, packages will be available at:
- https://www.nuget.org/packages?q=EFCore.BulkExtensions.Dotnet10

## Notes

- All packages are marked as **prerelease** (10.0.0-rc.1)
- Will update to stable version when EF Core 10.0 RTM is released
- MySQL and Oracle packages are excluded until providers support EF Core 10
- Package IDs are intentionally different from original to avoid conflicts
