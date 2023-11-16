dotnet ef migrations add "UpdatedMigration1116" -p Infrastructure -s API -c StoreContext -o Data/Migrations

dotnet ef migrations add "UpdatedMigration1116" -p Infrastructure -s API -c AppIdentityDbContext -o Identity/Migrations