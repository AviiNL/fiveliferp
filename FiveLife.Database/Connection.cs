using CitizenFX.Core;
using CitizenFX.Core.Native;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLife.Database
{
    public class Connection : BaseScript
    {
        private MySqlConnection connection;
        internal static Context context;

        private bool firstTick = true;

        public Connection()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            Tick += Connection_Tick;
        }

        private async Task Connection_Tick()
        {
            if (firstTick)
            {
                firstTick = false;
                Tick -= Connection_Tick;

                if (connection == null)
                {
                    connection = new MySqlConnection(Migrations.Configuration.ConnectionString);
                    connection.StateChange += Connection_StateChange;
                }

                if (context == null)
                    context = new Context(connection, false);

                context.Configuration.LazyLoadingEnabled = true;
                connection.Open();
                
            }
        }

        private void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if (e.CurrentState == System.Data.ConnectionState.Open)
            {
                TriggerEvent("fivelife.database.connected");
            }
        }
    }
}
