using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Context : DbContext
    {

        public DbSet<Shared.Entity.Player> Players { get; set; }
        public DbSet<Shared.Entity.Character> Characters { get; set; }
        public DbSet<Shared.Entity.Room> Rooms { get; set; }

        public Context()
            : base()
        {
        }

        public Context(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Types().Configure(entity =>
            {
                entity.ToTable("fl_" + entity.ClrType.Name.ToLower());
            });

            modelBuilder.Properties<string>().Configure(c =>
            {
                c.HasColumnType("varchar");
                c.HasMaxLength(255);
            });

            // Blurgh!
            modelBuilder.Entity<Shared.Entity.Room>()
               .HasMany(x => x.Allowed)
               .WithMany(x => x.Rooms)
               .Map(x =>
               {
                   x.MapLeftKey("RoomId");
                   x.MapRightKey("CharacterId");
                   x.ToTable("RoomCharacters");
               });

            base.OnModelCreating(modelBuilder);
        }
    }
}
