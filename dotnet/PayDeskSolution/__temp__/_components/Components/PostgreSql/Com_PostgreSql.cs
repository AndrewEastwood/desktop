using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using System.Data;

namespace components.Components.PostgreSql
{
    public class Com_PostgreSql
    {

        string query = "SELECT * FROM c01y2009.artgroups";

        public DataTable Test()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=postgres;Password=1111;Database=Market;");
            conn.Open();

            DataTable dtsource = new DataTable("articles");
            NpgsqlDataAdapter NpAdapter = new NpgsqlDataAdapter();
            NpAdapter.SelectCommand = new NpgsqlCommand("SELECT * FROM c01y2009.articles", conn);
            NpAdapter.Fill(dtsource);

            conn.Close();


            return dtsource;
        }

    }
}
