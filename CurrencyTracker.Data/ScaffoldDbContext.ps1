$ConnectionString = "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=True;TrustServerCertificate=True;"
$Provider = "Microsoft.EntityFrameworkCore.SqlServer"
$OutputDir = "Entities"
$ContextName = "CurrencyDbContext"
$ContextDir = "Context"
$Project = "CurrencyTracker.Data"
$StartupProject = "CurrencyTracker.API"

# --- Scaffold command ---
Scaffold-DbContext $ConnectionString $Provider `
    -OutputDir $OutputDir `
    -Context $ContextName `
    -ContextDir $ContextDir `
    -DataAnnotations `
    -Project $Project `
    -StartupProject $StartupProject `
    -Force `
    -NoOnConfiguring

    
ef migrations add InitIdentity --context AuthDbContext --project CurrencyTracker.Data --startup-project CurrencyTracker.API