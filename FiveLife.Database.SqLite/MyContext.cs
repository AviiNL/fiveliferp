using SQLite.CodeFirst;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;

namespace FiveLife.Database.SqLite
{
    [DbConfigurationType(typeof(SqLiteDbConfiguration))]
    public class MyContext : DbContext
    {
        public MyContext(DbConnection connection)
            : base(connection, contextOwnsConnection: true)
        { }

        public MyContext(string connectionString)
        : base()
        {

        }

        public DbSet<Shared.Entity.Player> Players { get; set; }
        public DbSet<Shared.Entity.Character> Characters { get; set; }
        public DbSet<Shared.Entity.Room> Rooms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            System.Data.Entity.Database.SetInitializer(new SqliteDropCreateDatabaseWhenModelChanges<MyContext>(modelBuilder));
        }
    }
}
