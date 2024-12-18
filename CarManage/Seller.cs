using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarManage
{
    public partial class Seller : Form
    {
        public Seller()
        {
            InitializeComponent();
            Con = new Functions();
            this.Load += Seller_Load; 
        }
        Functions Con;
        private void Seller_Load(object sender, EventArgs e)
        {
            ShowSellers();
            guna2ComboBox1.Items.AddRange(new string[] { "Đang làm việc", "Đã nghỉ việc" });

            // Đăng ký sự kiện CellFormatting
            SellerDGV.CellFormatting += SellerDGV_CellFormatting;
        }



        public void ShowSellers()
        {
            try
            {
                string Query = "SELECT * FROM Seller";
                DataTable data = Con.GetData(Query);
                if (data.Rows.Count > 0)
                {
                    SellerDGV.DataSource = data;
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu nào để hiển thị!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SellerDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Kiểm tra nếu cột là WorkStatus (giả sử cột này có tên là "WorkStatus")
            if (SellerDGV.Columns[e.ColumnIndex].Name == "WorkStatus")
            {
                if (e.Value != null)
                {
                    // Kiểm tra nếu giá trị là 1 (Đang làm việc) hoặc 0 (Đã nghỉ việc)
                    if (e.Value.ToString() == "1")
                    {
                        e.Value = "Đang làm việc"; // Hiển thị "Đang làm việc"
                    }
                    else if (e.Value.ToString() == "0")
                    {
                        e.Value = "Đã nghỉ việc"; // Hiển thị "Đã nghỉ việc"
                    }
                }
            }
        }



        private void pictureBox2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit(); 
            }
        }

        


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            
            DialogResult result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Confirm logout",              
                MessageBoxButtons.YesNo,            
                MessageBoxIcon.Question);        

         
            if (result == DialogResult.Yes)
            {
               
                this.Close(); // Đóng form hiện tại

                
                Login loginForm = new Login();
                loginForm.Show();
            }
            else
            {
                return;
            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            string name = Nametxt.Text;
            string phone = Phonetxt.Text;
            string email = Emailtxt.Text;
            string hireDate = Datetxt.Text;

            // Lấy giá trị WorkStatus từ ComboBox và chuyển thành 0 hoặc 1
            string workStatus = guna2ComboBox1.SelectedItem.ToString();
            int workStatusValue = workStatus == "Đang làm việc" ? 1 : 0; // 1 = Đang làm việc, 0 = Đã nghỉ việc

            // Kiểm tra xem các TextBox có rỗng không
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(hireDate))
            {
                MessageBox.Show("Missing Data");
            }
            else
            {
                try
                {
                    // Kiểm tra xem ngày có hợp lệ không
                    if (DateTime.TryParse(hireDate, out DateTime parsedDate))
                    {
                        // Kiểm tra xem tên hoặc email có tồn tại trong cơ sở dữ liệu không
                        using (SqlConnection con = new SqlConnection(Con.ConStr))
                        {
                            string checkQuery = "SELECT COUNT(*) FROM Seller WHERE FullName = @FullName OR Email = @Email";
                            SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                            checkCmd.Parameters.AddWithValue("@FullName", name);
                            checkCmd.Parameters.AddWithValue("@Email", email);

                            con.Open();
                            int existingRecords = (int)checkCmd.ExecuteScalar();

                            // Nếu có ít nhất 1 bản ghi trùng tên hoặc email
                            if (existingRecords > 0)
                            {
                                MessageBox.Show("Tên hoặc email đã tồn tại. Vui lòng nhập lại.");
                            }
                            else
                            {
                                // Nếu không có bản ghi trùng, thực hiện thêm dữ liệu mới
                                string insertQuery = "INSERT INTO Seller (FullName, Phone, Email, HireDate, WorkStatus) VALUES (@FullName, @Phone, @Email, @HireDate, @WorkStatus)";
                                SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                                insertCmd.Parameters.AddWithValue("@FullName", name);
                                insertCmd.Parameters.AddWithValue("@Phone", phone);
                                insertCmd.Parameters.AddWithValue("@Email", email);
                                insertCmd.Parameters.AddWithValue("@HireDate", parsedDate);
                                insertCmd.Parameters.AddWithValue("@WorkStatus", workStatusValue);

                                int rowsAffected = insertCmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Dữ liệu đã được thêm thành công!");
                                    ShowSellers(); // Cập nhật lại danh sách Seller
                                    Nametxt.Text = "";
                                    Phonetxt.Text = "";
                                    Emailtxt.Text = "";
                                    Datetxt.Text = ""; // Đặt lại ô nhập ngày
                                }
                                else
                                {
                                    MessageBox.Show("Có lỗi xảy ra khi thêm dữ liệu.");
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ngày không hợp lệ. Vui lòng nhập lại.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }




        private void SellerDGV_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = SellerDGV.Rows[e.RowIndex];
                Nametxt.Text = row.Cells["FullName"].Value.ToString();
                Phonetxt.Text = row.Cells["Phone"].Value.ToString();
                Emailtxt.Text = row.Cells["Email"].Value.ToString();
                Datetxt.Text = row.Cells["HireDate"].Value.ToString();

                // Lấy giá trị WorkStatus từ DataGridView (0 hoặc 1)
                string workStatus = row.Cells["WorkStatus"].Value?.ToString() ?? "0";

                // Chuyển đổi WorkStatus thành trạng thái tương ứng trong ComboBox
                if (workStatus == "1")
                {
                    guna2ComboBox1.SelectedItem = "Đang làm việc";
                }
                else
                {
                    guna2ComboBox1.SelectedItem = "Đã nghỉ việc";
                }
            }
        }



        private void EditBtn_Click(object sender, EventArgs e)
        {
            string name = Nametxt.Text;
            string phone = Phonetxt.Text;
            string email = Emailtxt.Text;
            string hireDate = Datetxt.Text;

            // Lấy giá trị WorkStatus từ ComboBox và chuyển thành 0 hoặc 1
            string workStatus = guna2ComboBox1.SelectedItem.ToString();
            int workStatusValue = workStatus == "Đang làm việc" ? 1 : 0;

            // Kiểm tra xem các TextBox có rỗng không
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(hireDate) || string.IsNullOrEmpty(workStatus))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
            }
            else
            {
                // Kiểm tra xem ngày có hợp lệ không
                if (DateTime.TryParse(hireDate, out DateTime parsedDate))
                {
                    // Kiểm tra xem người dùng có chọn dòng nào trong DataGridView không
                    if (SellerDGV.SelectedRows.Count > 0)
                    {
                        // Lấy ID của người bán từ dòng đã chọn (giả sử "SellerID" là tên cột ID của người bán)
                        int sellerID = Convert.ToInt32(SellerDGV.SelectedRows[0].Cells["SellerID"].Value);

                        try
                        {
                            // Kiểm tra xem email có tồn tại trong cơ sở dữ liệu hay không
                            using (SqlConnection con = new SqlConnection(Con.ConStr))
                            {
                                string checkEmailQuery = "SELECT COUNT(*) FROM Seller WHERE Email = @Email AND SellerID != @SellerID";
                                SqlCommand checkEmailCmd = new SqlCommand(checkEmailQuery, con);
                                checkEmailCmd.Parameters.AddWithValue("@Email", email);
                                checkEmailCmd.Parameters.AddWithValue("@SellerID", sellerID);

                                con.Open();
                                int emailExists = (int)checkEmailCmd.ExecuteScalar();

                                if (emailExists > 0)
                                {
                                    MessageBox.Show("Email đã tồn tại. Vui lòng nhập email khác.");
                                }
                                else
                                {
                                    // Cập nhật thông tin người bán trong cơ sở dữ liệu
                                    string query = "UPDATE Seller SET FullName = @FullName, Phone = @Phone, Email = @Email, HireDate = @HireDate, WorkStatus = @WorkStatus WHERE SellerID = @SellerID";

                                    SqlCommand cmd = new SqlCommand(query, con);
                                    cmd.Parameters.AddWithValue("@FullName", name);
                                    cmd.Parameters.AddWithValue("@Phone", phone);
                                    cmd.Parameters.AddWithValue("@Email", email);
                                    cmd.Parameters.AddWithValue("@HireDate", parsedDate);
                                    cmd.Parameters.AddWithValue("@WorkStatus", workStatusValue);  // Thêm tham số WorkStatus
                                    cmd.Parameters.AddWithValue("@SellerID", sellerID);

                                    int rowsAffected = cmd.ExecuteNonQuery();

                                    // Kiểm tra nếu có dữ liệu được cập nhật thành công
                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("Thông tin người bán đã được cập nhật thành công!");
                                        ShowSellers(); // Cập nhật lại danh sách Seller trong DataGridView
                                    }
                                    else
                                    {
                                        MessageBox.Show("Có lỗi xảy ra khi cập nhật dữ liệu.");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi: " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn một dòng để chỉnh sửa.");
                    }
                }
                else
                {
                    MessageBox.Show("Ngày không hợp lệ. Vui lòng nhập lại.");
                }
            }
        }



        private void DeleBtn_Click(object sender, EventArgs e)
        {
            Nametxt.Text = string.Empty;
            Phonetxt.Text = string.Empty;
            Emailtxt.Text = string.Empty;
            Datetxt.Text = string.Empty;
            guna2ComboBox1.SelectedIndex = -1;
        }

        //private void DeleBtn_Click(object sender, EventArgs e)
        //{
        //    if (SellerDGV.SelectedRows.Count > 0)
        //    {
        //        try
        //        {
        //            int sellerID = Convert.ToInt32(SellerDGV.SelectedRows[0].Cells["SellerID"].Value);

        //            DialogResult result = MessageBox.Show(
        //                "Bạn có chắc chắn muốn xóa người bán này không?",
        //                "Xác nhận xóa",
        //                MessageBoxButtons.YesNo,
        //                MessageBoxIcon.Warning
        //            );

        //            if (result == DialogResult.Yes)
        //            {
        //                using (SqlConnection con = new SqlConnection(Con.ConStr))
        //                {
        //                    con.Open();

        //                    // Xóa tất cả các dòng liên quan trong bảng Bill trước
        //                    string deleteBillsQuery = "DELETE FROM Bill WHERE SellerID = @SellerID";
        //                    SqlCommand deleteBillsCmd = new SqlCommand(deleteBillsQuery, con);
        //                    deleteBillsCmd.Parameters.AddWithValue("@SellerID", sellerID);
        //                    deleteBillsCmd.ExecuteNonQuery();

        //                    // Sau đó xóa Seller
        //                    string deleteSellerQuery = "DELETE FROM Seller WHERE SellerID = @SellerID";
        //                    SqlCommand deleteSellerCmd = new SqlCommand(deleteSellerQuery, con);
        //                    deleteSellerCmd.Parameters.AddWithValue("@SellerID", sellerID);
        //                    int rowsAffected = deleteSellerCmd.ExecuteNonQuery();

        //                    if (rowsAffected > 0)
        //                    {
        //                        MessageBox.Show("Người bán đã được xóa thành công!");
        //                        ShowSellers();
        //                    }
        //                    else
        //                    {
        //                        MessageBox.Show("Có lỗi xảy ra khi xóa dữ liệu.");
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Lỗi: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Vui lòng chọn một dòng để xóa.");
        //    }
        //}


        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // Tạo đối tượng của form Cars
            Cars carsForm = new Cars();

            // Hiển thị form Cars
            carsForm.Show();

            // Đóng form hiện tại
            this.Close();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Bill BillForm = new Bill();
            BillForm.Show();
            this.Hide();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Dashboard dashboardForm = new Dashboard();
            dashboardForm.Show();
            this.Hide();
        }

       
    }
}
