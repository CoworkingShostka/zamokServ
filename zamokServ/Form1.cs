using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace zamokServ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //timer1.Start();
            try
            {
                // comboBox1.DataSource = SerialPort.GetPortNames();
                //   serialPort1.Close();
                //  serialPort1.PortName = comboBox1.SelectedItem.ToString();
                // serialPort1.Open();
                // File.Copy(wrk, backup1, true);
            }
            catch (Exception) { }
            
            //настраиваем изначальный вид датагрид вью
            
       
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.RowHeadersVisible = false;
            notifyIcon1.Visible = false;
            localVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            versionToolStrip.Text += " " + localVersion;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
        }
        /// <summary>
        /// описываем расположение базы(устарело(вроде))
        /// </summary>
        string localVersion;
        string wrk = Application.StartupPath + "\\zamokDB.mdb";
        // string backup2 = "C:\\zamokDB.mdb";
        string backup1 = "D:\\zamokDB.mdb";
        /// <summary>
        /// админская карта
        /// </summary>
        string adminCard = "";
        //   string vhod = "";
        private void Form1_Load(object sender, EventArgs e)
        {//подключаемся к брокеру
            connectBrockerSub();
            //заполняем данные из базы в датагрид вью(ДГВ)
            this.historyTableAdapter.Fill(this.histDataSet.history);


            this.usersDBTableAdapter.Fill(this.usersDataSet.usersDB);
            //ранее использовалось для ком порта тепеь там сольвер
            asyncRead();

            adminCard = new WebClient().DownloadString("https://drive.google.com/uc?export=download&id=0B1PRhPmv7AwwdGlsV05pc1pFQ0k");
            //label9.Text = adminCard;
            //скрываем пароль
            dataGridView1.Columns[4].Visible = false;
            //dataGridView1.Columns[5].Visible = false;
            dataGridView1.Columns[6].Visible = false;
            visibility(false);
            //   serialPort1.ReadTimeout = 2000;
        }



        /// <summary>
        /// Разрешаем показ элементов
        /// </summary>
        /// <param name="vis"></param>
        public void visibility(bool vis)
        {
            textBox2.Visible = vis;
            textBox3.Visible = vis;
            textBox4.Visible = vis;
            textBox5.Visible = vis;
            button1.Visible = vis;
            button3.Visible = vis;
            label3.Visible = vis;
            label4.Visible = vis;
            label5.Visible = vis;
            label7.Visible = vis;
            button4.Visible = vis;
            // dataGridView1.Columns[4].Visible = vis;
            //  dataGridView1.Columns[5].Visible = vis;
            if (vis == true)
            {
                button2.Location = new Point(350, 446);

                button2.Size = new Size(220, 108);
                return;
            }
            else if (vis == false)
            {
                button2.Location = new Point(12, 446);

                button2.Size = new Size(500, 108);
                return;
            }

        }

        /// <summary>
        /// Добавляем нового пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {

            usersDataSet.usersDB.Rows.Add();
            usersDataSet.usersDB.Rows[usersDataSet.usersDB.Rows.Count - 1][0] = textBox3.Text;
            usersDataSet.usersDB.Rows[usersDataSet.usersDB.Rows.Count - 1][1] = textBox2.Text;
            usersDataSet.usersDB.Rows[usersDataSet.usersDB.Rows.Count - 1][2] = textBox5.Text;
            usersDataSet.usersDB.Rows[usersDataSet.usersDB.Rows.Count - 1][3] = textBox4.Text;
            usersDataSet.usersDB.Rows[usersDataSet.usersDB.Rows.Count - 1][4] = usersDataSet.usersDB.Count;
            saveUS();
            addToHist(true, "Добавлен новый пользователь:" + textBox2.Text);
            //File.Copy(wrk, backup1, true);
            // File.Copy(wrk, backup2, true);
        }
        /// <summary>
        /// применяем изменения
        /// </summary>
        public void saveUS()
        {
            usersDBTableAdapter.Update(usersDataSet.usersDB);
            usersDataSet.AcceptChanges();
            usersDBTableAdapter.Fill(this.usersDataSet.usersDB);
            //  historyTableAdapter.Update(historyDataSet.history);
            // historyDataSet.AcceptChanges();
        }

        /// <summary>
        /// сохраняем историю
        /// </summary>
        public void saveHist()
        {
            historyTableAdapter.Update(histDataSet.history);
            histDataSet.AcceptChanges();
            historyTableAdapter.Fill(histDataSet.history);
        }
        /// <summary>
        /// кнопка теста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            usersDBTableAdapter.Fill(this.usersDataSet.usersDB);
            this.historyTableAdapter.Fill(this.histDataSet.history);



        }
        //     string login = "";
        string pass = "";
        //    string uuid = "";
        string cache = "";

        // string adminCard = "23351217172";
        string tag;
        public string cardNo;
        string permit;
        int doors = 1;//кол-во дверей

        //    string kek = "";
        bool flag = false;
        bool flag1 = false;
        //добалвяем в базу историй новую запись
        public void addToHist(bool res, string comm)
        {
            try
            {
                //histDataSet.historyRow hist = histDataSet.history.FindByNum(histDataSet.history.Rows.Count);
                //textBox1.Text = hist.ToString();
                histDataSet.history.Rows.Add();
                //  histDataSet.history.Rows[histDataSet.history.Rows.Count - 1][0] = histDataSet.history.Rows.Count;
                histDataSet.history.Rows[histDataSet.history.Rows.Count - 1][1] = cardNo;
                histDataSet.history.Rows[histDataSet.history.Rows.Count - 1][2] = DateTime.Now;
                histDataSet.history.Rows[histDataSet.history.Rows.Count - 1][3] = comm;
                saveHist();
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
            //if (res == true)
            //{
            //    MessageBox.Show("Разрешено");
            //    //save();

            //}
            //else { }
        }
        // string passw;
        /// <summary>
        /// самый главный метод решает впустить или не
        /// закоменченные строки относятся к компорту
        /// </summary>
        /// <param name="dataIn"></param>
        /// 
        string msg;
        int ind = 0;
        public void solver(string dataIn)
        {
             //dataIn = recv;

            //  Data.Value = dataIn;

            // MessageBox.Show(dataIn);



            try
            {



                //string[] dataOut;
                //dataOut = dataIn.Split(new char[] { '#' });
                //tag = dataOut[0];
                //cardNo = dataOut[1];
                cardNo = dataIn;
                if (cardNo != null)
                {
                    textBox2.Text = cardNo;


                    if (/*tag.Contains("G") &&*/ tempTopic[1] == "admin" && cardNo == adminCard)//add new card
                    {

                        visibility(true);
                        addToHist(true, "admin");

                    }

                    for (int i = 0; i < usersDataSet.usersDB.Rows.Count; i++)
                    {

                        //if (cardNo == usersDataSet.usersDB.Rows[i][2].ToString())
                        //  if ( String.Compare(cardNo, usersDataSet.usersDB.Rows[i][2].ToString())==0) 

                        if (/*object.Equals(pass,usersDataSet.usersDB.Rows[i][0].ToString()) &&*/ object.Equals(cardNo, usersDataSet.usersDB.Rows[i][1].ToString()) /*== true*/)
                        {
                            permit = "yes";
                            ind = i;
                            flag = false;

                            // addToHist(true,tag);
                            //  label6.Text = "OK";
                            break;
                        }
                        else
                        {
                            permit = "no";
                            flag = true;
                            //   label6.Text = "NOT OK";
                        }

                    }
                    // cache = 'S' + tag + '#' + permit;
                    //  serialPort1.Write(cache);
                    if (flag == true)
                    {
                        addToHist(false, tempTopic[1] + " " + permit);
                        pubBrocker("no");
                    }
                    else
                    {
                        addToHist(true, tempTopic[1] + " " + permit);
                        //pubFIBrocker();
                        insideController();
                        pubBrocker(msg);
                        

                    }
                }

                //int m = 1;
                //for (int n = 1; n <= doors; n++)
                //{
                //    if (tag.Contains("D" + doors))
                //    {
                //        //  pass = dataOut[2];
                //        //  pass = dataOut[2];
                //        // label9.Text = pass;
                //        for (int i = 0; i < usersDataSet.usersDB.Rows.Count; i++)
                //        {

                //            //if (cardNo == usersDataSet.usersDB.Rows[i][2].ToString())
                //            //  if ( String.Compare(cardNo, usersDataSet.usersDB.Rows[i][2].ToString())==0) 

                //            if (/*object.Equals(pass,usersDataSet.usersDB.Rows[i][0].ToString()) &&*/ object.Equals(cardNo, usersDataSet.usersDB.Rows[i][1].ToString()) /*== true*/)
                //            {
                //                permit = "yes";
                //                flag = false;
                //                // addToHist(true,tag);
                //                //  label6.Text = "OK";
                //                break;
                //            }
                //            else
                //            {
                //                permit = "no";
                //                flag = true;
                //                //   label6.Text = "NOT OK";
                //            }

                //        }
                //        cache = 'S' + tag + '#' + permit;
                //        serialPort1.Write(cache);
                //        if (flag == true) { addToHist(false, tag + " " + permit); } else { addToHist(true, tag + " " + permit); }

                //    }
                //}

                //if (tag.Contains("D0"))
                //{
                //    for (int i = 0; i < usersDataSet.usersDB.Rows.Count; i++)
                //    {

                //        if (/*pass == usersDataSet.usersDB.Rows[i][0].ToString() &&*/ cardNo == usersDataSet.usersDB.Rows[i][1].ToString())
                //        {
                //            permit = "yes";
                //            flag1 = false;
                //            addToHist(true, tag);

                //        }

                //        else if (/*pass != usersDataSet.usersDB.Rows[i][0].ToString() &&*/ cardNo == usersDataSet.usersDB.Rows[i][1].ToString())
                //        {
                //            permit = "no";
                //            flag1 = true;

                //        }
                //        else if (/*pass == usersDataSet.usersDB.Rows[i][0].ToString() &&*/ cardNo != usersDataSet.usersDB.Rows[i][1].ToString())
                //        {
                //            permit = "no";
                //            flag1 = true;

                //        }
                //    }
                //    cache = 'S' + tag + '#' + permit;
                //    if (flag1 == true) { addToHist(false, tag + " " + permit); } else { addToHist(true, tag + " " + permit); }
                //    serialPort1.Write(cache);
                //}

                //cache ='S'+tag+ permit;
                //serialPort1.Write(cache); 
                //if (dataIn != "") { MessageBox.Show(dataIn); }

            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// лучше дальше не ломать методы
        /// </summary>
        public string data;

        public string param;

        public delegate void AddTextDelegate();

        public Task<string> DataRd()
        {

            return Task.Run(() =>
            {
                try
                {

                    //  data = serialPort1.ReadLine();
                    //вытаскиваем потоко-безопасно данные
                    SetTextSafe(data);
                    //Thread.CurrentThread.Abort();

                }

                catch (Exception)
                {
                    // MessageBox.Show(es.ToString());
                }

                return data;
            });

        }
        // string kek;
        /// <summary>
        /// оправляем в сольвер данные, обязательно обнуляя переменную
        /// </summary>
        public async void doRead()
        {

            while (true)
            {
                try
                {
                    solver(mqttMessageConv);
                    //solver(decodeMqttMess());
                    label9.Text = mqttTopic;
                    label10.Text = mqttMessageConv;
                    mqttMessageConv = null;

                }
                catch (Exception) { }
                // solver(recv);
                // solver(decodeMqtt());

                //solver(recv1);
                // recv = serialPort1.ReadLine();
                // solver(data);
                //solver(dataCOM.Text);
                Application.DoEvents();//шоб не висло
                NoSleep();///недаем винде заснуть пока приложение запущено
                param = await DataRd();
            }
        }

        void SetTextSafe(string newText)
        {

            BeginInvoke(new Action<string>((s) => recv = s), newText);

            Data.Value = recv;




        }
        public void asyncRead() { BeginInvoke(new AddTextDelegate(doRead)); }//вытаскиваем дданые отправляя делегата за ними
        MqttClient client = new MqttClient(IPAddress.Parse("192.168.1.2"));//коннект
        public void subBrocker()
        {
            client.Subscribe(new string[] { "AS/+/cardID" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });//подписываемся на все двери

        }
        string clientID;
        /// <summary>
        /// подкючаемся к брокеру
        /// </summary>
        public void connectBrockerSub()
        {
            client.MqttMsgPublishReceived += mqttRecv;
            clientID = Guid.NewGuid().ToString();
            client.Connect(clientID);
            subBrocker();
        }

        byte[] mqttMessage;
        string mqttTopic;
        string mqttMessageConv;
        /// <summary>
        /// считываем данные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void mqttRecv(object sender, MqttMsgPublishEventArgs e)
        {
            mqttMessage = e.Message;
            mqttTopic = e.Topic;
            tempTopic = mqttTopic.Split(new char[] { '/' });
            //sendTopic = tempTopic[0] + "/" + tempTopic[1] + "/server_response";
            mqttMessageConv = Regex.Replace(decodeMqttMess(), @"\t|\n|\r", "");

        }
        //декодируем данные (обязательно)
        public string decodeMqttMess()
        {
            Encoding enc = Encoding.GetEncoding(1251);
            return enc.GetString(mqttMessage);
        }

        private void button6_Click(object sender, EventArgs e)
        {

            //testPub("");
        }
        //public void connectBrockerPub(string value)
        //{
        //    //client.MqttMsgPublishReceived += mqttRecv;
        //  //  clientID = Guid.NewGuid().ToString();
        //    //client.Disconnect();
        //    //  client.Connect(clientID);
        //    // pubBrocker(value);
        //   // testPub(value);
        //    // client.Disconnect();
        //}

        string[] tempTopic;
        string sendTopic;
        
        /// <summary>
        /// отвечаем брокеру
        /// </summary>
        /// <param name="value"></param>
        public void pubBrocker(string value)
        {
            if (tempTopic[1] == "DoorCoworkingOut")
            {
                string strF = usersDataSet.usersDB.Rows[ind][2].ToString();
                client.Publish(tempTopic[0] + "/F", Encoding.UTF8.GetBytes(strF), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);

                string strI = usersDataSet.usersDB.Rows[ind][3].ToString();
                client.Publish(tempTopic[0] + "/I", Encoding.UTF8.GetBytes(strI), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);

                string strRez = usersDataSet.usersDB.Rows[ind][5].ToString();
                client.Publish(tempTopic[0] + "/rez", Encoding.UTF8.GetBytes(strRez), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);

                string strInd = usersDataSet.usersDB.Rows[ind][4].ToString();
                client.Publish(tempTopic[0] + "/ind", Encoding.UTF8.GetBytes(strInd), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);

            }
            else if (tempTopic[1] == "DoorCoworkingIn")
            {
                string strInd = usersDataSet.usersDB.Rows[ind][4].ToString();
                client.Publish(tempTopic[0] + "/ind", Encoding.UTF8.GetBytes(strInd), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
            }

            string strValue = Convert.ToString(value);
            //  tempTopic = mqttTopic.Split(new char[] { '/' });
            sendTopic = tempTopic[0] + "/" + tempTopic[1] + "/server_response";
            client.Publish(sendTopic, Encoding.UTF8.GetBytes(strValue), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);

        }

        public void pubFIBrocker()
        {

            //string strValue = Convert.ToString(value);
            //  tempTopic = mqttTopic.Split(new char[] { '/' });

            
        }

        public void insideController()
        {
            msg = "yes";

            if (tempTopic[1] == "DoorCoworkingOut")
            {
                

                if (object.Equals("staj", usersDataSet.usersDB.Rows[ind][5].ToString()))
                {
                    for (int i = 0; i < usersDataSet.usersDB.Rows.Count; i++)
                    {
                        if(object.Equals("rez", usersDataSet.usersDB.Rows[i][5].ToString()) && object.Equals("yes", usersDataSet.usersDB.Rows[i][6].ToString()))
                        {
                            msg = "yes";
                            usersDataSet.usersDB.Rows[ind][6] = "yes";
                            return;
                        }
                    }
                    msg = "no";
                    return;
                }

                usersDataSet.usersDB.Rows[ind][6] = "yes";

            }
            else if (tempTopic[1] == "DoorCoworkingIn" )
            {
                usersDataSet.usersDB.Rows[ind][6] = "no";
            }
        }
        //тест публикации
        //public void testPub(string value)
        //{

        //    string strValue = Convert.ToString(value);
        //    // tempTopic = mqttTopic.Split(new char[] { '/' });
        //    sendTopic = "AS/door1/server_response";

        //    client.Publish(sendTopic, Encoding.UTF8.GetBytes(strValue), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);

        //}




        private void button3_Click(object sender, EventArgs e)
        {
            //    MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            //    DialogResult result;
            //    result = MessageBox.Show("Вы уверены, что хотите безвозвратно удалить историю посещений?", "Вы уверены", buttons);
            //    if (result == DialogResult.OK)
            //    {
            //        while (historyDataSet.history.Rows.Count > 0)
            //        {
            //            historyDataSet.history.Clear(); Application.DoEvents();
            //        }
            //        //  
            //        historyTableAdapter.Update(historyDataSet.history);

            //    }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // comboBox1.Items.Clear();
            //comboBox1.DataSource = SerialPort.GetPortNames();
            //serialPort1.Close();
            //serialPort1.PortName = comboBox1.SelectedItem.ToString();
            //serialPort1.Open();
        }
        string recv;
        //  string recv1;
        private void timer1_Tick(object sender, EventArgs e) { }


        private void button2_Click_2(object sender, EventArgs e)
        {
            openHistory addN = new openHistory();
            addN.StartPosition = FormStartPosition.CenterScreen;
            addN.Show(this);
        }
        bool flagPW;
        /// <summary>
        /// скрытие полей из-под Одмена(вроед)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            //dataGridView1.Columns[5].Visible = true;
            dataGridView1.Columns[6].Visible = true;
            //if (flagPW == true) { dataGridView1.Columns[4].Visible = true; dataGridView1.Columns[5].Visible = true; flagPW = false; }
            // /*else*/ { dataGridView1.Columns[4].Visible = false; dataGridView1.Columns[5].Visible = false; flagPW = true; }
            visibility(true);


        }
        /// <summary>
        /// сложный код который нельзя удалять, он недает компу заснуть
        /// </summary>
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_USER_PRESENT = 0x00000004
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags);
        //продолжение

        private void NoSleep()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }
        //скрываем даныне из-под одмена
        private void button4_Click(object sender, EventArgs e)
        {
            //visibility(false); dataGridView1.Columns[5].Visible = false;
            visibility(false); dataGridView1.Columns[6].Visible = false;
        }
        string kasha = "copy \"" + Application.StartupPath + "\\zamokServ_new.exe\"" + "\"" + Application.StartupPath + "\\zamokServ.exe\"";
        /// <summary>
        /// апдейтер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // string ver = new WebClient().DownloadString("https://drive.google.com/uc?export=download&id=0B1PRhPmv7AwwZFFDM2VzYXJ3cFU");
                new WebClient().DownloadFile("https://drive.google.com/uc?export=download&id=0B1PRhPmv7AwweldpMTduQm9CVEk", Application.StartupPath + "\\update.bat");


                new WebClient().DownloadFile("https://drive.google.com/uc?export=download&id=0B1PRhPmv7AwwbFBqNjNBLTlKMjQ", Application.StartupPath + "\\zamokServ_new.exe");
                new WebClient().DownloadFile("https://drive.google.com/uc?export=download&id=0B1PRhPmv7AwwTG1hVkxOTHRWVE0", Application.StartupPath + "\\M2Mqtt.Net.dlll");

                ProcessStartInfo upd = new ProcessStartInfo("cmd.exe");

                Process.Start(Application.StartupPath + "\\update.bat");

                this.Close();





            }

            catch (Exception) { MessageBox.Show("В процесі оновлення сталась помилка\nзверніться до розробника", "Сталась помилка"); }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        //  DataRow[] foundRow;
        //обработчик удалений пользователей
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int numDel;
            if (e.ColumnIndex == 6)
            {

                numDel = int.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());

                usersDataSet.usersDBRow usToDel = usersDataSet.usersDB.FindByNCust(numDel);
                textBox1.Text = usToDel.ToString();
                //DataRow[] result = usersDataSet.usersDB.Select("SELECT * FROM usersDB WHERE NCust=" + numDel);

                usersDataSet.usersDB.Rows[dataGridView1.CurrentRow.Index].Delete();
                usersDBTableAdapter.Update(usersDataSet.usersDB);
                usersDataSet.usersDB.AcceptChanges();
                addToHist(true, "Картку видалено:" + dataGridView1.CurrentRow.Cells[3].Value.ToString());



            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.usersDBTableAdapter.Fill(this.usersDataSet.usersDB);
        }
        //гасим аппу по крестику иначе не гасится
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.Copy(wrk, backup1, true);
            notifyIcon1.Visible = false;
            //  client.Disconnect();
            //Close();
            Environment.Exit(0);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.usersDBTableAdapter.Fill(this.usersDataSet.usersDB);
        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            // histDataSet.historyRow hist = histDataSet.history.FindByNum(histDataSet.history.Rows.Count);

        }
        /// <summary>
        /// копирайт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/monteshot");
        }
        /// <summary>
        /// сворачиваем у трей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "Программу згорнуто";
                notifyIcon1.BalloonTipText = "Програма поміщена в трей, але продовжить роботу";
                notifyIcon1.ShowBalloonTip(7);
            }
        }
        private FormWindowState _OldFormState;
        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                notifyIcon1.ContextMenuStrip.Show();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
