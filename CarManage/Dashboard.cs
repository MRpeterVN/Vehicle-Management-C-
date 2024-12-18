using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CarManage
{
    public partial class Dashboard : Form
    {
        private readonly Functions _functions;

        public Dashboard()
        {
            InitializeComponent();
            _functions = new Functions();
            LoadData(); // Tải dữ liệu và hiển thị
        }

        // Tải dữ liệu và hiển thị các biểu đồ (doanh thu và thống kê khác)
        private void LoadData()
        {
            try
            {
                LoadMonthlyRevenueChart();
                LoadTotalRevenue();
                LoadTotalCarsSold();
                LoadBestSellingCar();
                LoadWorkingSellers();
                LoadCarsInStock();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMonthlyRevenueChart()
        {
            string revenueQuery = @"
        SELECT YEAR(B.BillDate) AS Year, MONTH(B.BillDate) AS Month, 
               SUM(D.Quantity * D.Price) AS TotalRevenue
        FROM Bill B
        JOIN BillDetails D ON B.BillID = D.BillID
        GROUP BY YEAR(B.BillDate), MONTH(B.BillDate)
        ORDER BY Year, Month";

            DataTable revenueData = _functions.GetData(revenueQuery);
            LoadRevenueChart(revenueData);
        }

        private void LoadTotalRevenue()
        {
            string totalRevenueQuery = "SELECT SUM(D.Quantity * D.Price) AS TotalRevenue FROM BillDetails D";
            DataTable totalRevenueData = _functions.GetData(totalRevenueQuery);

            decimal totalRevenue = totalRevenueData.Rows[0]["TotalRevenue"] != DBNull.Value
                ? Convert.ToDecimal(totalRevenueData.Rows[0]["TotalRevenue"])
                : 0;

            decimal exchangeRate = 24000m;
            decimal revenueInVND = totalRevenue * exchangeRate;

            Revenuelbl.Text = $"Revenue: {revenueInVND:N0} VND";
        }

        private void LoadTotalCarsSold()
        {
            string carsSoldQuery = "SELECT SUM(D.Quantity) AS TotalCarsSold FROM BillDetails D";
            DataTable carsSoldData = _functions.GetData(carsSoldQuery);

            int totalCarsSold = carsSoldData.Rows[0]["TotalCarsSold"] != DBNull.Value
                ? Convert.ToInt32(carsSoldData.Rows[0]["TotalCarsSold"])
                : 0;

            Carsoldlbl.Text = $"Cars Sold: {totalCarsSold}";
        }

        private void LoadBestSellingCar()
        {
            string bestCarQuery = @"
        SELECT TOP 1 C.CarName, SUM(D.Quantity) AS TotalSold
        FROM Car C
        JOIN BillDetails D ON C.CarID = D.CarID
        GROUP BY C.CarName
        ORDER BY TotalSold DESC";

            DataTable bestCarData = _functions.GetData(bestCarQuery);

            Bestcarlbl.Text = bestCarData.Rows.Count > 0
                ? $"Best Car: {bestCarData.Rows[0]["CarName"]}"
                : "Best Car: N/A";
        }

        private void LoadWorkingSellers()
        {
            string sellerQuery = "SELECT COUNT(*) AS TotalSellers FROM Seller WHERE WorkStatus = 1";
            DataTable sellerData = _functions.GetData(sellerQuery);

            int totalSellers = sellerData.Rows[0]["TotalSellers"] != DBNull.Value
                ? Convert.ToInt32(sellerData.Rows[0]["TotalSellers"])
                : 0;

            Sellerlbl.Text = $"Sellers Working: {totalSellers}";
        }

        private void LoadCarsInStock()
        {
            string carsInStockQuery = "SELECT SUM(Quantity) AS CarsInStock FROM Car";
            DataTable carsInStockData = _functions.GetData(carsInStockQuery);

            int carsInStock = carsInStockData.Rows[0]["CarsInStock"] != DBNull.Value
                ? Convert.ToInt32(carsInStockData.Rows[0]["CarsInStock"])
                : 0;

            CarsInStocklbl.Text = $"Cars in Stock: {carsInStock}";
        }



        // Hàm vẽ biểu đồ doanh thu
        private void LoadRevenueChart(DataTable revenueData)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (revenueData == null || revenueData.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để hiển thị biểu đồ doanh thu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tạo danh sách lưu giá trị doanh thu và tháng
                ChartValues<decimal> revenueValues = new ChartValues<decimal>();
                List<string> months = new List<string>();

                foreach (DataRow row in revenueData.Rows)
                {
                    if (row["TotalRevenue"] != DBNull.Value && row["Month"] != DBNull.Value && row["Year"] != DBNull.Value)
                    {
                        revenueValues.Add(Convert.ToDecimal(row["TotalRevenue"]));
                        months.Add($"{row["Month"]:00}/{row["Year"]}");
                    }
                }

                // Cập nhật dữ liệu vào biểu đồ doanh thu
                cartesianChart1.Series = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Doanh thu",
                Values = revenueValues,
                Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue), // Màu cột
                StrokeThickness = 1,
                MaxColumnWidth = 20 // Đặt độ rộng tối đa cho cột
            }
        };

                // Cập nhật trục X (Tháng)
                cartesianChart1.AxisX = new AxesCollection
        {
            new Axis
            {
                Title = "Tháng",
                Labels = months.ToArray(),
                Separator = new Separator
                {
                    Step = 1, // Khoảng cách giữa các nhãn
                    IsEnabled = false // Không hiển thị đường kẻ
                }
            }
        };

                // Cập nhật trục Y (Doanh thu)
                cartesianChart1.AxisY = new AxesCollection
        {
            new Axis
            {
                Title = "Doanh thu (VND)",
                LabelFormatter = value => value.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("vi-VN")) // Định dạng tiền tệ
            }
        };

                // Tùy chỉnh vị trí chú thích
                cartesianChart1.LegendLocation = LiveCharts.LegendLocation.Top;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi vẽ biểu đồ doanh thu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Các sự kiện click khác (giống như trước)
        private void pictureBox3_Click(object sender, EventArgs e)
        {
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

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Bill billForm = new Bill();
            billForm.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void CarsInStocklbl_Click(object sender, EventArgs e)
        {

        }

        private void Revenuelbl_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            // Show confirmation dialog
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Log out confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check if the user selected Yes or No
            if (result == DialogResult.Yes)
            {
                // Log out the user (for example, clear any user-related session information)
                // If you have a variable to store the user's information (like username, password), you can clear it
                // For example:
                // UserSession.Username = null;

                // Close the current form (main form)
                this.Close();

                // Open the Login form
                Login loginForm = new Login(); // Replace LoginForm with your actual login form's name
                loginForm.Show();
            }
            else
            {
                // If the user selects No, do nothing and keep the current form
                return;
            }
        }




    }
}
