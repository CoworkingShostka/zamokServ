using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zamokServ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string backup1 = Application.StartupPath + "\\zamokDB.mdb";
        // string backup2 = "C:\\zamokDB.mdb";
        string wrk = "D:\\zamokDB.mdb";
        string vhod = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            this.usersDBTableAdapter.Fill(this.usersDataSet.usersDB);
            this.historyTableAdapter.Fill(this.historyDataSet.history);
            // usersDBTableAdapter.Connection.ConnectionString = "Microsoft.Jet.OLEDB.4.0; Data Source =" + Application.StartupPath + "\\zamokDB.mdb";
            try { usersDBTableAdapter.Connection.ConnectionString = "Microsoft.Jet.OLEDB.4.0; Data Source =" + wrk; }
            catch (Exception) { }



        }







        private void button1_Click_1(object sender, EventArgs e)
        {

            save();
            File.Copy(wrk, backup1, true);
            // File.Copy(wrk, backup2, true);
        }

        public void save()
        {
            usersDBTableAdapter.Update(usersDataSet.usersDB);
            usersDataSet.AcceptChanges();
            historyTableAdapter.Update(historyDataSet.history);
            historyDataSet.AcceptChanges();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            usersDBTableAdapter.Fill(this.usersDataSet.usersDB);
            historyTableAdapter.Fill(historyDataSet.history);



        }
        string login = "";
        string pass = "";
        private void button2_Click_1(object sender, EventArgs e)
        {
            usersDBTableAdapter.Fill(this.usersDataSet.usersDB);
            historyTableAdapter.Fill(historyDataSet.history);
            vhod = textBox1.Text;


            try
            {
                string[] temp = vhod.Split(new char[] { '*' });
                login = temp[0];
                pass = temp[1];
            }
            catch (Exception) { }
            bool enter = false;
            for (int i = 0; i < usersDataSet.usersDB.Rows.Count; i++)
            {

                if (login == usersDataSet.usersDB.Rows[i][0].ToString() && pass == usersDataSet.usersDB.Rows[i][1].ToString())
                {
                    addToHist(true);
                    break;
                }
                else if (login == usersDataSet.usersDB.Rows[i][0].ToString() && pass != usersDataSet.usersDB.Rows[i][1].ToString())
                {
                    addToHist(false);
                }
                else if (login != usersDataSet.usersDB.Rows[i][0].ToString() && pass == usersDataSet.usersDB.Rows[i][1].ToString())
                {
                    addToHist(false);
                }
            }
            save();



        }
        public void addToHist(bool res)
        {
            historyDataSet.history.Rows.Add();
            historyDataSet.history.Rows[historyDataSet.history.Rows.Count - 1][0] = historyDataSet.history.Rows.Count;
            historyDataSet.history.Rows[historyDataSet.history.Rows.Count - 1][1] = login;
            historyDataSet.history.Rows[historyDataSet.history.Rows.Count - 1][2] = DateTime.Now;
            historyDataSet.history.Rows[historyDataSet.history.Rows.Count - 1][3] = res;
            if (res == true)
            {
                MessageBox.Show("Разрешено");
                
            }
            else { }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            DialogResult result;
            result = MessageBox.Show("Вы уверены, что хотите безвозвратно удалить историю посещений?", "Вы уверены", buttons);
            if (result == DialogResult.OK)
            {
                while (historyDataSet.history.Rows.Count > 0) {   historyDataSet.history.Clear();Application.DoEvents();
                }
                //  
                historyTableAdapter.Update(historyDataSet.history);
              
            }
        }
    }
}
