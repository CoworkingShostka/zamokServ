using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zamokServ
{
    public partial class openHistory : Form
    {
        public openHistory()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.RowHeadersVisible = false;
        }
        string dataIn;
        string[] dataOut;
        string cardNo;
        public void recvCard( )
        {
            if (dataIn != "")
            {
                try
                {
                    dataIn = Data.Value;
                    dataOut = dataIn.Split(new char[] { '#' });
                    cardNo = dataOut[1];
                    //textBox2.Text = cardNo;
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            recvCard();

            //risovka();
        }

        private void openHistory_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "histQUEDataSet.histQUE". При необходимости она может быть перемещена или удалена.
            //this.histQUETableAdapter.Fill(this.histQUEDataSet.histQUE);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "zamokDBDataSet1._1243234". При необходимости она может быть перемещена или удалена.
         //   this._1243234TableAdapter.Fill(this.zamokDBDataSet1._1243234);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "historyDBDataSet.history". При необходимости она может быть перемещена или удалена.
            this.historyTableAdapter.Fill(this.historyDBDataSet.history);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "usersDataSet.usersDB". При необходимости она может быть перемещена или удалена.
            //  this.usersDBTableAdapter.Fill(this.usersDataSet.usersDB);

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.historyTableAdapter.Fill(this.historyDBDataSet.history);
        }
        //public void risovka() {
        //    label1.Text = recvCard();
        //}
    }
}
