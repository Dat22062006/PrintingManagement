using System;
using System.Data;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace PrintingManagement
{
    internal class DatabaseHelper
    {
        public static SqlConnection GetConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["MyConnection"];
            if (cs == null)
                throw new Exception("Không tìm thấy 'MyConnection' trong App.config.");
            return new SqlConnection(cs.ConnectionString);
        }

        // SELECT — dùng ExecuteReader thay SqlDataAdapter
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader); // ✅ Thay adapter.Fill(dt)
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi DB:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }
        // GỌI STORED PROCEDURE — TRẢ VỀ DataTable
        public static DataTable ExecuteStoredProcedure(string spName, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure; // ← quan trọng
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi SP [" + spName + "]:\n\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }
        // INSERT / UPDATE / DELETE
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi DB:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}