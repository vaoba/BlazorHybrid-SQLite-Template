using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SampleProject.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items { get; set; }
}

// DesignTimeContextFactory to set design time filepath for SQLite database.
// This is for migrations, places data.db into /Resources/Raw/ in order to be automatically deployed.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Raw/data.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        
        return new AppDbContext(optionsBuilder.Options);
    }
}

// Then we use DbInitializer at runtime to figure out what path to initialize our DbContext with according to the system we run on, and if the debug mode is active.
public static class DbInitializer
{
    public static string InitializeDbPath()
    {
        // If we are in debug mode on windows, use bin/debug/windows data.db file because of insufficient access rights to AppData
        // If debug android or not debug, use AppData directory, and copy data.db from the build to AppData if it doesn't exist yet
        #if DEBUG
            #if WINDOWS
                var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "data.db");
            #else
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "data.db");
                            
                if (!File.Exists(dbPath))
                {
                    using var stream = FileSystem.OpenAppPackageFileAsync("data.db").Result;
                    using var newFile = File.Create(dbPath);
                    stream.CopyTo(newFile);
                }
            #endif
        #else
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "data.db");
                    
            if (!File.Exists(dbPath))
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("data.db").Result;
                using var newFile = File.Create(dbPath);
                stream.CopyTo(newFile);
            }
        #endif

        return dbPath;
    }
}