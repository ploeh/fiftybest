# FiftyBest - World's 50 Best Restaurants Rankings

FiftyBest is an ASP.NET Core web application that displays "The World's 50 Best Restaurants" rankings data from various years (2002-2025). The application provides interactive views to browse restaurants by year, country, city, and name, with detailed ranking history for each restaurant.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Bootstrap, Build, and Test the Repository

Install .NET 9.0 SDK (required):
- `curl -L https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 9.0`
- `export PATH="/home/runner/.dotnet:$PATH"`
- Verify installation: `dotnet --version` (should show 9.0.x)

Build the solution:
- `dotnet build --configuration Release` -- **NEVER CANCEL**: Takes 45+ seconds. Set timeout to 90+ minutes.
- Build may take 45-50 seconds on first run due to package restore and compilation

Run tests:
- `dotnet test --configuration Release --no-build` -- Takes ~3 seconds. Set timeout to 10+ minutes.
- Alternative: `./build.sh` -- Runs tests in Release configuration

### Run the Web Application

Development mode:
- `cd FiftyBest && dotnet run --configuration Release`
- Application runs on http://localhost:5298 (configured in launchSettings.json)
- **NEVER CANCEL**: Application startup takes 5-10 seconds initially

Production deployment:
- `dotnet publish FiftyBest/FiftyBest.csproj -c Release -o [output-directory]` -- Takes ~15 seconds
- Run published app: `dotnet [output-directory]/Ploeh.FiftyBest.dll`

## Validation

### Manual Testing Requirements
**CRITICAL**: Always manually validate the application by running through complete user scenarios after making changes.

Required test scenarios:
1. **Homepage Navigation**: Navigate to http://localhost:5298 and verify "Welcome" page loads
2. **Year Rankings View**: Click "By year" and verify 2025 rankings table displays all 50 restaurants
3. **Restaurant Detail View**: Click on any restaurant name (e.g., "Maido") and verify ranking history table appears
4. **Cross-Navigation**: Test navigation between Years, Countries, Cities, and Restaurants views
5. **Data Integrity**: Verify restaurant names, cities, countries, and rankings display correctly

### Build Validation
Always run these commands before committing changes:
- `dotnet build --configuration Release` -- Ensures code compiles without warnings (TreatWarningsAsErrors=True)
- `dotnet test --configuration Release --no-build` -- Ensures all 7 tests pass
- The GitHub Actions workflow (.github/workflows/master_fiftybest.yml) will fail if warnings exist

## Architecture and Key Components

### Project Structure
- **FiftyBest/**: Main web application (ASP.NET Core 9.0 Razor Pages)
- **FiftyBest.Tests/**: xUnit test project
- **FiftyBest/Data/**: CSV files containing restaurant rankings by year (2002-2025)

### Core Classes
- `FiftyBestData.cs`: Loads and processes restaurant ranking data from CSV files
- `Restaurant.cs`, `RestaurantId.cs`, `RestaurantName.cs`: Restaurant domain models
- `CityId.cs`, `Ranking.cs`, `FiftyBestEntry.cs`: Supporting domain models
- `Pages/`: Razor Pages for different views (Years, Countries, Cities, Restaurants)

### Data Format
CSV files in `FiftyBest/Data/` follow format: `Rank,Restaurant,City,Country`
Example: `1,Maido,Lima,Peru`

## Common Tasks

### Adding New Year Data
1. Create new CSV file: `FiftyBest/Data/YYYY.csv`
2. Follow existing CSV format: `Rank,Restaurant,City,Country`
3. Test by running application and navigating to `/Years/YYYY`

### Debugging Data Issues
- Check CSV file format in `FiftyBest/Data/`
- Verify `FiftyBestData.Load("Data")` in `Program.cs`
- Restaurant rankings are automatically aggregated across years

### Performance Considerations
- Application loads all data into memory at startup from CSV files
- Data is processed into dictionaries for fast lookups
- No database required - all data is file-based

## Deployment Information

### Azure Web App Deployment
The application deploys automatically to Azure Web App "fiftybest" via GitHub Actions:
- Target framework: .NET 9.0
- Deployment trigger: Push to master branch
- Build artifacts: Published to Azure Web App

### Local Development
- Development environment: Uses `appsettings.Development.json`
- Production environment: Uses `appsettings.json`
- No external dependencies required - fully self-contained

## Timing Expectations

| Operation | Expected Time | Timeout Setting |
|-----------|---------------|-----------------|
| Build (first time) | 45-50 seconds | 90+ minutes |
| Build (subsequent) | 10-20 seconds | 30+ minutes |
| Test execution | 2-3 seconds | 10+ minutes |
| Publish | 15 seconds | 30+ minutes |
| Application startup | 5-10 seconds | 5+ minutes |

**CRITICAL**: NEVER CANCEL builds or long-running commands. Wait for completion even if it takes the full expected time.

## Common File Locations

### Repository Root
```
.
├── FiftyBest/                    # Main web application
├── FiftyBest.Tests/              # Unit tests
├── FiftyBest.sln                 # Solution file
├── build.sh                      # Test runner script
├── .github/workflows/            # CI/CD pipeline
└── COPYING                       # GPL v3 license
```

### Key Configuration Files
- `FiftyBest/FiftyBest.csproj`: Project file with .NET 9.0 target
- `FiftyBest/appsettings.json`: Application configuration
- `FiftyBest/Properties/launchSettings.json`: Development server settings
- `.github/workflows/master_fiftybest.yml`: Azure deployment pipeline

## Troubleshooting

### .NET Version Issues
- **Error**: "The current .NET SDK does not support targeting .NET 9.0"
- **Solution**: Install .NET 9.0 SDK using the bootstrap commands above

### Build Warnings as Errors
- **Error**: Build fails due to warnings
- **Solution**: Fix all warnings - project has `TreatWarningsAsErrors=True`

### Data Loading Issues
- **Error**: Restaurant data not displaying
- **Solution**: Verify CSV files exist in `FiftyBest/Data/` and follow correct format

### Application Not Starting
- **Error**: Application fails to start
- **Solution**: Check port 5298 availability, verify .NET 9.0 runtime installed