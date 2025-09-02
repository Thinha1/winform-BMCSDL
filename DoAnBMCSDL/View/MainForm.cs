﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace OracleConnect.View
{
    public partial class MainForm : Form
    {
        private LoginForm loginForm;
        public MainForm()
        {
            InitializeComponent();
            loginForm = new LoginForm();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            OracleConnection o = DatabaseInit.GetConnection();
            string name = Test.username;
            if (!string.IsNullOrWhiteSpace(name))
            {
                lbl_user.Text = $"Welcome {Test.username}";
                lbl_user.Refresh();
            }
            else
            {
                lbl_user.Text = "Welcome guest";
                lbl_user.Refresh();
            }
            if (Test.username.ToUpper() == "SYS")
            {
                btn_logout_all.Visible = true;
            }
            else
            {
                btn_logout_all.Visible = false;
            }
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            if (DatabaseInit.Connect())
            {
                try
                {
                    //Chạy lệnh kill hết session
                    using (OracleCommand omd = new OracleCommand("SYS.LOGOUT_USER", DatabaseInit.GetConnection()))
                    {
                        omd.CommandType = System.Data.CommandType.StoredProcedure;
                        omd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = Test.username;
                        omd.ExecuteNonQuery();
                    }

                    //Logout ra sau khi kill
                    MessageBox.Show("Bạn đã thực thi việc đăng xuất, form sẽ thoát.");
                    this.Hide();
                    loginForm.ShowDialog();
                    return;
                }
                catch (OracleException)
                {
                    MessageBox.Show("Không tồn tại procedure này!.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }
        }

        private void btn_logout_all_Click(object sender, EventArgs e)
        {
            DatabaseInit.init(Test.host, Test.port, Test.sid, Test.username, Test.password);

            if (DatabaseInit.Connect())
            {
                try
                {
                    //Chạy lệnh kill hết session
                    using (OracleCommand omd = new OracleCommand("LOGOUT_USER_ALL", DatabaseInit.GetConnection()))
                    {
                        omd.CommandType = System.Data.CommandType.StoredProcedure;
                        omd.ExecuteNonQuery();
                    }

                    //Logout ra sau khi kill
                    MessageBox.Show("Bạn đã thực thi việc đóng connection của tất cả user, form sẽ thoát.");
                    this.Hide();
                    loginForm.ShowDialog();
                    return;
                }
                catch (OracleException)
                {
                    MessageBox.Show("Không tồn tại procedure này!");
                    this.Hide();
                    loginForm.ShowDialog();
                    return;

                }
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }
        }
    }
}
