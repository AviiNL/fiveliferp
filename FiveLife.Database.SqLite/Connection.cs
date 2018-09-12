using CitizenFX.Core;
using System.Data.SQLite;
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

            Connect();
        }

        internal static void Connect()
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
