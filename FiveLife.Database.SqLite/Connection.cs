using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Database.SqLite
{
    public class Connection : BaseScript
    {
        internal static MyContext context;

        private static bool firstTick = true;

        public Connection()
        {
            Tick += Connection_Tick;
        }

        private async Task Connection_Tick()
        {
            if (!firstTick) return;
            firstTick = false;
            Tick -= Connection_Tick;

            await Connect();
        }

        internal static async Task Connect()
        {
            if (context == null)
            {
                var connection = new SQLiteConnection("Data Source=./fivelife.db;Trusted_connection=yes");

                context = new MyContext(connection);
                context.Configuration.LazyLoadingEnabled = true;
                TriggerEvent("fivelife.database.connected");
            }
        }
    }
}
