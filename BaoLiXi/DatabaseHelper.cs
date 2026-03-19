using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace PrintingManagement
{
    internal class DatabaseHelper
    {
        // 🔥 LẤY CONNECTION TỪ App.config
        public static SqlConnection GetConnection()
        {
            string connectionString = ConfigurationManager
                .ConnectionStrings["MyConnection"]
                .ConnectionString;

            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }

        // SELECT
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    conn.Open();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // INSERT / UPDATE / DELETE
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}