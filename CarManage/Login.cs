using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarManage
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if(UserNametxt.Text == "" || Passtxt.Text == "")
            {
                MessageBox.Show("Missing Data !!!");
            }
            else
            {
                if (UserNametxt.Text == "1" || Passtxt.Text == "1")
                {
                    MessageBox.Show("Login Succes!!!");
                    Seller obj = new Seller(); ;
                    obj.Show();
                    this.Hide();    
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Đóng form
                this.Close();
            }
            else
            {
                // Nếu người dùng chọn No, không làm gì và giữ lại form hiện tại
                return;
            }
        }

    }
}
