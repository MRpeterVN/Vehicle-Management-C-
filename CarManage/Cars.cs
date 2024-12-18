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
    public partial class Cars : Form
    {
        public Cars()
        {
            InitializeComponent();
            Con = new Functions();
            this.Load += Car_Load;
        }
        Functions Con;
        private void Car_Load(object sender, EventArgs e)
        {
            ShowCar();
        }

        public void ShowCar()
        {
            try
            {
                string Query = "SELECT * FROM Car";
                DataTable data = Con.GetData(Query);
                if (data.Rows.Count > 0)
                {
                    CarDGV.DataSource = data;
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

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            // Tạo đối tượng của form Seller
            Seller sellerForm = new Seller();

            // Hiển thị form Seller
            sellerForm.Show();

            // Đóng form hiện tại
            this.Close();
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void CarDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có nhấp vào hàng hợp lệ không
            if (e.RowIndex >= 0)
            {
                // Lấy hàng hiện tại mà người dùng đã nhấp vào
                DataGridViewRow row = CarDGV.Rows[e.RowIndex];

                // Lấy dữ liệu từ các ô và đưa vào các TextBox
                CarNametxt.Text = row.Cells["CarName"].Value.ToString();
                Brandtxt.Text = row.Cells["Brand"].Value.ToString();
                Modeltxt.Text = row.Cells["Model"].Value.ToString();
                Pricetxt.Text = row.Cells["Price"].Value.ToString();
                Quantitytxt.Text = row.Cells["Quantity"].Value.ToString();
                Desciptiontxt.Text = row.Cells["Description"].Value.ToString();
                Yeartxt.Text = row.Cells["CarYear"].Value.ToString();
                Colortxt.Text = row.Cells["CarColor"].Value.ToString();
            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ các TextBox
            string carname = CarNametxt.Text;
            string brand = Brandtxt.Text;
            string model = Modeltxt.Text;
            string price = Pricetxt.Text;
            string quantity = Quantitytxt.Text;
            string description = Desciptiontxt.Text;
            string caryear = Yeartxt.Text;
            string carcolor = Colortxt.Text;

            // Kiểm tra xem các TextBox có bị bỏ trống không
            if (string.IsNullOrEmpty(carname) || string.IsNullOrEmpty(brand) ||
                string.IsNullOrEmpty(model) || string.IsNullOrEmpty(price) ||
                string.IsNullOrEmpty(quantity) || string.IsNullOrEmpty(description) ||
                string.IsNullOrEmpty(caryear) || string.IsNullOrEmpty(carcolor))
            {
                MessageBox.Show("Missing Data");
            }
            else
            {
                try
                {
                    // Kiểm tra xem giá và năm có hợp lệ không
                    if (decimal.TryParse(price, out decimal parsedPrice) &&
                        int.TryParse(caryear, out int parsedYear) &&
                        int.TryParse(quantity, out int parsedQuantity))
                    {
                        // Câu truy vấn SQL
                        string Query = "INSERT INTO Car (CarName, Brand, Model, Price, Quantity, Description, CarYear, CarColor) " +
                                       "VALUES (@CarName, @Brand, @Model, @Price, @Quantity, @Description, @CarYear, @CarColor)";

                        // Tạo kết nối và thực thi câu lệnh SQL
                        using (SqlConnection con = new SqlConnection(Con.ConStr))
                        {
                            SqlCommand cmd = new SqlCommand(Query, con);
                            cmd.Parameters.AddWithValue("@CarName", carname);
                            cmd.Parameters.AddWithValue("@Brand", brand);
                            cmd.Parameters.AddWithValue("@Model", model);
                            cmd.Parameters.AddWithValue("@Price", parsedPrice);
                            cmd.Parameters.AddWithValue("@Quantity", parsedQuantity);
                            cmd.Parameters.AddWithValue("@Description", description);
                            cmd.Parameters.AddWithValue("@CarYear", parsedYear);
                            cmd.Parameters.AddWithValue("@CarColor", carcolor);

                            con.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            // Kiểm tra nếu thêm thành công
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Dữ liệu đã được thêm thành công!");
                                ShowCar(); 
                                CarNametxt.Text = "";
                                Brandtxt.Text = "";
                                Modeltxt.Text = "";
                                Pricetxt.Text = "";
                                Quantitytxt.Text = "";
                                Desciptiontxt.Text = "";
                                Yeartxt.Text = "";
                                Colortxt.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("Có lỗi xảy ra khi thêm dữ liệu.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng nhập giá, số lượng và năm hợp lệ.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ các TextBox
            string carname = CarNametxt.Text;
            string brand = Brandtxt.Text;
            string model = Modeltxt.Text;
            string price = Pricetxt.Text;
            string quantity = Quantitytxt.Text;
            string description = Desciptiontxt.Text;
            string caryear = Yeartxt.Text;
            string carcolor = Colortxt.Text;

            // Kiểm tra xem các TextBox có rỗng không
            if (string.IsNullOrEmpty(carname) || string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(model) ||
                string.IsNullOrEmpty(price) || string.IsNullOrEmpty(quantity) || string.IsNullOrEmpty(description) ||
                string.IsNullOrEmpty(caryear) || string.IsNullOrEmpty(carcolor))
            {
                MessageBox.Show("Missing Data");
            }
            else
            {
                try
                {
                    // Kiểm tra nếu có dòng nào được chọn
                    if (CarDGV.SelectedRows.Count > 0)
                    {
                        // Lấy ID của chiếc xe từ dòng đã chọn
                        int carID = Convert.ToInt32(CarDGV.SelectedRows[0].Cells["CarID"].Value);

                        // Kiểm tra xem giá và năm có hợp lệ không
                        if (decimal.TryParse(price, out decimal parsedPrice) && int.TryParse(caryear, out int parsedYear) &&
                            int.TryParse(quantity, out int parsedQuantity))
                        {
                            // Câu lệnh SQL để cập nhật dữ liệu
                            string Query = "UPDATE Car SET CarName = @CarName, Brand = @Brand, Model = @Model, Price = @Price, " +
                                           "Quantity = @Quantity, Description = @Description, CarYear = @CarYear, CarColor = @CarColor " +
                                           "WHERE CarID = @CarID";

                            using (SqlConnection con = new SqlConnection(Con.ConStr))
                            {
                                SqlCommand cmd = new SqlCommand(Query, con);
                                cmd.Parameters.AddWithValue("@CarID", carID); // Dùng carID từ dòng đã chọn
                                cmd.Parameters.AddWithValue("@CarName", carname);
                                cmd.Parameters.AddWithValue("@Brand", brand);
                                cmd.Parameters.AddWithValue("@Model", model);
                                cmd.Parameters.AddWithValue("@Price", parsedPrice);
                                cmd.Parameters.AddWithValue("@Quantity", parsedQuantity);
                                cmd.Parameters.AddWithValue("@Description", description);
                                cmd.Parameters.AddWithValue("@CarYear", parsedYear);
                                cmd.Parameters.AddWithValue("@CarColor", carcolor);

                                // Mở kết nối và thực thi câu lệnh
                                con.Open();
                                int rowsAffected = cmd.ExecuteNonQuery();

                                // Kiểm tra nếu dữ liệu được cập nhật thành công
                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Dữ liệu đã được cập nhật thành công!");
                                    ShowCar(); // Cập nhật lại danh sách Car
                                }
                                else
                                {
                                    MessageBox.Show("Có lỗi xảy ra khi cập nhật dữ liệu.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Giá hoặc năm sản xuất không hợp lệ. Vui lòng kiểm tra lại.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn một chiếc xe để chỉnh sửa.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void DeleBtn_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có dòng nào được chọn trong DataGridView không
            if (CarDGV.SelectedRows.Count > 0)
            {
                // Lấy ID của chiếc xe từ dòng đã chọn
                int carID = Convert.ToInt32(CarDGV.SelectedRows[0].Cells["CarID"].Value);

                // Hiển thị hộp thoại xác nhận xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa chiếc xe này?", "Xác nhận xóa", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // Câu lệnh SQL để xóa chiếc xe
                        string query = "DELETE FROM Car WHERE CarID = @CarID";

                        using (SqlConnection con = new SqlConnection(Con.ConStr))
                        {
                            SqlCommand cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@CarID", carID); // Dùng carID từ dòng đã chọn

                            // Mở kết nối và thực thi câu lệnh
                            con.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();

                            // Kiểm tra nếu dữ liệu được xóa thành công
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Chiếc xe đã được xóa thành công!");
                                ShowCar(); // Cập nhật lại danh sách Car
                            }
                            else
                            {
                                MessageBox.Show("Có lỗi xảy ra khi xóa dữ liệu.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một chiếc xe để xóa.");
            }
        }

        private void CarNametxt_TextChanged(object sender, EventArgs e)
        {

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
    }

}
