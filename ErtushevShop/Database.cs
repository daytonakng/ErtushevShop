using Npgsql;
using System.Data;
namespace ErtushevShop
{
    internal class Database
    {
        string psqlStrConnection = "Server=localhost; port=5432; user id=postgres; password=admin; database=ErtushevShop;";

        NpgsqlConnection psqlConnection;
        NpgsqlCommand psqlCommand;

        public void getPsqlConnection()
        {
            psqlConnection = new NpgsqlConnection();
            psqlConnection.ConnectionString = psqlStrConnection;

            if (psqlConnection.State == ConnectionState.Closed)
            {
                psqlConnection.Open();
            }
        }

        public DataTable GetData(string sql)
        {
            DataTable dt = new DataTable();
            getPsqlConnection();
            psqlCommand = new NpgsqlCommand();
            psqlCommand.Connection = psqlConnection;
            psqlCommand.CommandText = sql;

            NpgsqlDataReader dr = psqlCommand.ExecuteReader();
            dt.Load(dr);

            return dt;
        }
    }
}
