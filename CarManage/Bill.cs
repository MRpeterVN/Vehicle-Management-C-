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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Globalization;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using System.IO;
using iText.Layout.Borders;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Action;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Diagnostics;








namespace CarManage
{
    public partial class Bill : Form
    {
        private List<BillItem> billItems;
        public Bill()
        {
            InitializeComponent();
            Con = new Functions();
            this.Load += Car_Load;
            this.Load += Bill_Load;
            this.Load += ListBill_Load;

            billItems = new List<BillItem>();
        }
        Functions Con;

        private void Car_Load(object sender, EventArgs e) => ShowCar();
        private void Bill_Load(object sender, EventArgs e) => LoadSellers();
        private void ListBill_Load(object sender, EventArgs e) => LoadListBillDetails();


        private void LoadListBillDetails()
        {
            try
            {
                // Truy vấn lấy tất cả dữ liệu từ bảng BillDetails và các thông tin liên quan
                string query = @"
        SELECT b.BillID, b.BillDate, b.TotalPrice, c.CarName, bd.Quantity, bd.Price, s.FullName AS SellerName
        FROM BillDetails bd
        JOIN Bill b ON bd.BillID = b.BillID
        JOIN Car c ON bd.CarID = c.CarID
        JOIN Seller s ON b.SellerID = s.SellerID";

                // Gọi hàm GetData từ Functions để lấy dữ liệu
                Functions dbFunctions = new Functions();
                DataTable data = dbFunctions.GetData(query);

                // Gán dữ liệu cho DataGridView
                ListBillDGV.DataSource = data;

                // Format lại tên cột hiển thị cho rõ ràng (nếu cần)
                ListBillDGV.Columns["BillID"].HeaderText = "Mã Hóa Đơn";
                ListBillDGV.Columns["BillDate"].HeaderText = "Ngày";
                ListBillDGV.Columns["TotalPrice"].HeaderText = "Tổng Tiền";
                ListBillDGV.Columns["CarName"].HeaderText = "Tên Xe";
                ListBillDGV.Columns["Quantity"].HeaderText = "Số Lượng";
                ListBillDGV.Columns["Price"].HeaderText = "Giá";
                ListBillDGV.Columns["SellerName"].HeaderText = "Tên Người Bán";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load danh sách hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void CarDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Kiểm tra xem người dùng có nhấp vào một dòng hợp lệ không
                if (e.RowIndex >= 0)
                {
                    // Lấy hàng được chọn
                    DataGridViewRow selectedRow = CarDGV.Rows[e.RowIndex];

                    string carName = selectedRow.Cells["CarName"].Value.ToString();
                    string price = selectedRow.Cells["Price"].Value.ToString();

                    // Gắn giá trị vào các TextBox hoặc Label tương ứng
                    CarNametxt.Text = carName;
                    Pricetxt.Text = price;

                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void LoadSellers()
        {
            try
            {
                // Query để lấy danh sách người bán
                string query = "SELECT SellerID, FullName FROM Seller";
                DataTable sellerData = Con.GetData(query);

                // Kiểm tra dữ liệu
                if (sellerData.Rows.Count > 0)
                {
                    Sellertxt.DataSource = sellerData;
                    Sellertxt.DisplayMember = "FullName";  // Hiển thị tên người bán (FullName)
                    Sellertxt.ValueMember = "SellerID";   // Lưu trữ giá trị là SellerID
                }
                else
                {
                    MessageBox.Show("Không có người bán nào để hiển thị!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách người bán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


      




        // Cập nhật BillItem để thêm trường Quantity
        public class BillItem
        {
            public string CarName { get; set; }
            public decimal Price { get; set; }
            public string SellerName { get; set; }
            public DateTime Date { get; set; }
            public int Quantity { get; set; }  // Thêm trường Quantity
        }
        public class BillDetail
        {
            public int CarID { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }

        private void Addbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu hợp lệ
                if (string.IsNullOrEmpty(CarNametxt.Text) || string.IsNullOrEmpty(Pricetxt.Text) || Sellertxt.SelectedIndex == -1 || string.IsNullOrEmpty(Quantitytxt.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thông báo");
                    return;
                }

                // Lấy dữ liệu từ các điều khiển
                string carName = CarNametxt.Text;  // Lấy CarName từ TextBox
                decimal price;
                int quantity;

                if (!decimal.TryParse(Pricetxt.Text, out price))
                {
                    MessageBox.Show("Giá không hợp lệ!", "Lỗi");
                    return;
                }

                if (!int.TryParse(Quantitytxt.Text, out quantity))
                {
                    MessageBox.Show("Số lượng không hợp lệ!", "Lỗi");
                    return;
                }

                // Lấy SellerID từ ComboBox
                string sellerID = Sellertxt.SelectedValue.ToString();  // SellerID
                string sellerName = Sellertxt.Text; // Tên người bán

                DateTime date = DateTime.Now;

                // Tạo một mục hóa đơn
                BillItem item = new BillItem
                {
                    CarName = carName,
                    Price = price,
                    SellerName = sellerName,  // Lưu SellerName
                    Date = date,
                    Quantity = quantity
                };

                // Thêm mục hóa đơn vào danh sách
                billItems.Add(item);

                // Hiển thị danh sách hóa đơn lên DataGridView
                BillDGV.DataSource = null; // Reset dữ liệu
                BillDGV.DataSource = billItems;  // Liên kết lại danh sách với DataGridView

                // Cập nhật lại tên cột trong DataGridView
                BillDGV.Columns["SellerName"].HeaderText = "Tên Người Bán";  // Đảm bảo cột SellerName hiển thị tên người bán

                // Xóa dữ liệu trong các TextBox sau khi thêm
                CarNametxt.Clear();
                Pricetxt.Clear();
                Quantitytxt.Clear();
                LoadListBillDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm vào hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        private void Save_Click_1(object sender, EventArgs e)
        {
            if (billItems.Count == 0) throw new Exception("Không có xe nào trong hóa đơn!");

            using (SqlConnection conn = new SqlConnection(Con.ConStr))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    decimal totalPrice = billItems.Sum(item => item.Price * item.Quantity);

                    // Lấy giá trị BillDate từ DateTimePicker
                    DateTime billDate = Datetxt.Value;

                    string insertBillQuery = @"INSERT INTO Bill (BillDate, SellerID, TotalPrice) OUTPUT INSERTED.BillID 
                                       VALUES (@BillDate, @SellerID, @TotalPrice)";
                    var billCmd = new SqlCommand(insertBillQuery, conn, transaction);
                    billCmd.Parameters.AddWithValue("@BillDate", billDate);
                    billCmd.Parameters.AddWithValue("@SellerID", GetSellerIDByName(billItems[0].SellerName, conn, transaction));
                    billCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);

                    int billID = (int)billCmd.ExecuteScalar();

                    foreach (var item in billItems)
                    {
                        string insertDetails = "INSERT INTO BillDetails (BillID, CarID, Quantity, Price) VALUES (@BillID, @CarID, @Quantity, @Price)";
                        var detailsCmd = new SqlCommand(insertDetails, conn, transaction);
                        detailsCmd.Parameters.AddWithValue("@BillID", billID);
                        detailsCmd.Parameters.AddWithValue("@CarID", GetCarIDByName(item.CarName, conn, transaction));
                        detailsCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        detailsCmd.Parameters.AddWithValue("@Price", item.Price);
                        detailsCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Lưu hóa đơn thành công!");
                    billItems.Clear();
                    RefreshBillDataGrid();
                    LoadListBillDetails();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Lỗi khi lưu hóa đơn: " + ex.Message);
                }
            }
        }


        private int GetSellerIDByName(string sellerName, SqlConnection conn, SqlTransaction transaction)
        {
            var cmd = new SqlCommand("SELECT SellerID FROM Seller WHERE FullName = @Name", conn, transaction);
            cmd.Parameters.AddWithValue("@Name", sellerName);
            return (int)cmd.ExecuteScalar();
        }

        private int GetCarIDByName(string carName, SqlConnection conn, SqlTransaction transaction)
        {
            var cmd = new SqlCommand("SELECT CarID FROM Car WHERE CarName = @CarName", conn, transaction);
            cmd.Parameters.AddWithValue("@CarName", carName);
            return (int)cmd.ExecuteScalar();
        }

        private void RefreshBillDataGrid()
        {
            BillDGV.DataSource = null;
            BillDGV.DataSource = billItems;
        }

        // Hàm lấy SellerID theo tên người bán





        private void ListBillDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    // Lấy dòng hiện tại từ DataGridView
                    DataGridViewRow selectedRow = ListBillDGV.Rows[e.RowIndex];

                    // Lấy thông tin từ dòng được chọn
                    string billID = selectedRow.Cells["BillID"].Value?.ToString() ?? "N/A";
                    string billDate = selectedRow.Cells["BillDate"].Value?.ToString() ?? "N/A";
                    string totalPrice = selectedRow.Cells["TotalPrice"].Value?.ToString() ?? "0";

                    // Truy vấn lấy tất cả thông tin chi tiết hóa đơn
                    Functions dbFunctions = new Functions();
                    string billDetailsQuery = $@"
                SELECT c.CarName, bd.Quantity, bd.Price, s.FullName AS SellerName
                FROM BillDetails bd
                JOIN Car c ON bd.CarID = c.CarID
                JOIN Bill b ON bd.BillID = b.BillID
                JOIN Seller s ON b.SellerID = s.SellerID
                WHERE bd.BillID = {billID}";

                    // Lấy tất cả chi tiết hóa đơn
                    DataTable billDetailsData = dbFunctions.GetData(billDetailsQuery);

                    // Hiển thị thông tin hóa đơn chung
                    var vietnamCulture = new CultureInfo("vi-VN");
                    BillNumberlbl.Text = $"BillNumber: {billID}";
                    Datelbl.Text = $"Date: {billDate}";

                    // Duyệt qua chi tiết hóa đơn để nhóm thông tin
                    StringBuilder carDetails = new StringBuilder();
                    HashSet<string> displayedSellers = new HashSet<string>(); // Để kiểm tra seller đã hiển thị
                    decimal totalPriceCalculated = 0;
                    string sellerName = "";

                    foreach (DataRow row in billDetailsData.Rows)
                    {
                        string carName = row["CarName"].ToString();
                        int quantity = int.Parse(row["Quantity"].ToString());
                        decimal price = decimal.Parse(row["Price"].ToString());
                        string currentSellerName = row["SellerName"].ToString();

                        // Tính giá tiền từng xe
                        decimal totalCarPrice = quantity * price;
                        totalPriceCalculated += totalCarPrice;

                        // Thêm thông tin chi tiết xe
                        carDetails.AppendLine($"Car: {carName}, Quantity: {quantity}, Price: {string.Format(vietnamCulture, "{0:C0}", totalCarPrice)}");

                        // Lấy seller (chỉ hiển thị một lần)
                        if (!displayedSellers.Contains(currentSellerName))
                        {
                            sellerName = currentSellerName;
                            displayedSellers.Add(currentSellerName);
                        }
                    }

                    // Hiển thị thông tin trong các Label
                    CarNamelbl.Text = $"Cars:\n{carDetails.ToString()}";
                    Sellerlbl.Text = $"Seller: {sellerName}";
                    Pricelbl.Text = $"Total Price: {string.Format(vietnamCulture, "{0:C0}", totalPriceCalculated)}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void Printbtb_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(BillNumberlbl.Text) ||
                    string.IsNullOrEmpty(CarNamelbl.Text) ||
                    string.IsNullOrEmpty(Datelbl.Text) ||
                    string.IsNullOrEmpty(Sellerlbl.Text) ||
                    string.IsNullOrEmpty(Pricelbl.Text))
                {
                    MessageBox.Show("Invoice details are incomplete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Extract data from labels
                string billNumber = BillNumberlbl.Text.Replace("BillNumber: ", "");
                string date = Datelbl.Text.Replace("Date: ", "");
                string sellerName = Sellerlbl.Text.Replace("Seller: ", "");
                string totalPrice = Pricelbl.Text.Replace("Total Price: ", "");

                // Get car details as lines
                string carDetailsText = CarNamelbl.Text.Replace("Cars:\n", "");
                string[] carDetailsLines = carDetailsText.Split('\n', (char)StringSplitOptions.RemoveEmptyEntries);

                // Open Save File Dialog
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Save Invoice",
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"Invoice_{billNumber}.pdf"
                };

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string pdfFilePath = saveFileDialog.FileName;

                // Create and write the PDF
                using (PdfWriter writer = new PdfWriter(pdfFilePath))
                using (PdfDocument pdf = new PdfDocument(writer))
                using (Document document = new Document(pdf))
                {
                    // Set fonts
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    // Add Title
                    document.Add(new Paragraph("SALES INVOICE")
                        .SetFont(boldFont)
                        .SetFontSize(18)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(20));

                    // Create a table with two columns for general info
                    Table generalInfoTable = new Table(2);
                    generalInfoTable.SetWidth(UnitValue.CreatePercentValue(100));

                    generalInfoTable.AddCell(CreateCell("Invoice Number:", boldFont));
                    generalInfoTable.AddCell(CreateCell(billNumber, regularFont));

                    generalInfoTable.AddCell(CreateCell("Date:", boldFont));
                    generalInfoTable.AddCell(CreateCell(date, regularFont));

                    generalInfoTable.AddCell(CreateCell("Seller Name:", boldFont));
                    generalInfoTable.AddCell(CreateCell(sellerName, regularFont));

                    // Add the general info table
                    document.Add(generalInfoTable);

                    // Add a space before car details
                    document.Add(new Paragraph("\nCar Details:").SetFont(boldFont).SetFontSize(14));

                    // Create a table for car details (3 columns: Car Name, Quantity, Price)
                    Table carDetailsTable = new Table(3);
                    carDetailsTable.SetWidth(UnitValue.CreatePercentValue(100));

                    carDetailsTable.AddHeaderCell(CreateCell("Car Name", boldFont));
                    carDetailsTable.AddHeaderCell(CreateCell("Quantity", boldFont));
                    carDetailsTable.AddHeaderCell(CreateCell("Price", boldFont));

                    foreach (string carDetail in carDetailsLines)
                    {
                        // Each carDetail line is formatted as: "Car: [CarName], Quantity: [Quantity], Price: [Price]"
                        string[] parts = carDetail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < parts.Length; i++)
                        {
                            parts[i] = parts[i].Trim(); // Loại bỏ khoảng trắng thừa
                        }


                        if (parts.Length >= 3)
                        {
                            string carName = parts[0].Replace("Car: ", "");
                            string quantity = parts[1].Replace("Quantity: ", "");
                            string price = parts[2].Replace("Price: ", "");

                            carDetailsTable.AddCell(CreateCell(carName, regularFont));
                            carDetailsTable.AddCell(CreateCell(quantity, regularFont));
                            carDetailsTable.AddCell(CreateCell(price, regularFont));
                        }
                    }

                    // Add car details table to document
                    document.Add(carDetailsTable);

                    // Add a space before total price
                    document.Add(new Paragraph("\nTotal Price:").SetFont(boldFont).SetFontSize(14));

                    // Add total price
                    document.Add(new Paragraph(totalPrice)
                        .SetFont(regularFont)
                        .SetFontSize(12)
                        .SetTextAlignment(TextAlignment.RIGHT));

                    // Add a thank you message
                    document.Add(new Paragraph("\nThank you for your purchase!")
                        .SetItalic()
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                // Show success message
                MessageBox.Show($"Invoice has been saved to: {pdfFilePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open the saved PDF
                Process.Start(new ProcessStartInfo(pdfFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Handle errors
                MessageBox.Show($"Error generating invoice: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to create a table cell
        private Cell CreateCell(string content, PdfFont font)
        {
            return new Cell()
                .Add(new Paragraph(content).SetFont(font).SetFontSize(12))
                .SetBorder(Border.NO_BORDER)
                .SetPadding(5);
        }



        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // Tạo một instance của form Car
            Cars carForm = new Cars();
            carForm.Show();
            this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Seller sellerForm = new Seller();
            sellerForm.Show();
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
