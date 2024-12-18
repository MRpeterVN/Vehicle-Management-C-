using System;
using System.Data;
using System.Data.SqlClient;

namespace CarManage
{
    internal class Functions
    {
        public readonly string ConStr;

        public Functions()
        {
            ConStr = @"Data Source=DESKTOP-N9S7F24\SQLEXPRESS;Initial Catalog=ManageCar;Integrated Security=True";
        }
        public string GetConnectionString()
        {
            return ConStr;
        }

        // Lấy dữ liệu (SELECT)
        public DataTable GetData(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConStr))
                using (SqlDataAdapter sda = new SqlDataAdapter(query, con))
                {
                    sda.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy dữ liệu: " + ex.Message);
            }
            return dt;
        }

        // Thực thi câu lệnh (INSERT, UPDATE, DELETE) với tham số
        public int SetData(string query, params SqlParameter[] parameters)
        {
            int affectedRows = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    // Thêm tham số vào câu lệnh
                    cmd.Parameters.AddRange(parameters);

                    con.Open();
                    affectedRows = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi thực thi câu lệnh: " + ex.Message);
            }
            return affectedRows;
        }
    }
}