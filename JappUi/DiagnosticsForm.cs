using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Japp;

namespace JappUI
{
    public partial class DiagnosticsForm : Form
    {
        private Comm comm;
        private Timer timer;


        public DiagnosticsForm()
        {
            this.InitializeComponent();
            this.UpdateCOMPortList();
            this.UpdateCommandList();
            this.comm = new Comm();
            this.timer = new Timer();
            this.timer.Interval = 500;
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Enabled = true;
            this.timer.Start();
        }

        public DiagnosticsForm(Comm comm)
        {
            this.InitializeComponent();
            this.UpdateCOMPortList();
            this.UpdateCommandList();
            this.comm = comm;
            this.timer = new Timer();
            this.timer.Interval = 500;
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Enabled = true;
            this.timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {

            if (!this.comm.IsConnected)
                return;

            foreach (string port in SerialPort.GetPortNames())
            {
                if (port == this.comboPort.Text)
                    return;
            }

            this.btnConnect_Click(this.btnConnect, EventArgs.Empty);

        }

        private void UpdateCOMPortList()
        {
            string selectedItem = this.comboPort.Text.Trim();
            
            this.comboPort.Items.Clear();
            this.comboPort.Text = string.Empty;
            foreach (string port in SerialPort.GetPortNames())
            {
                int index = this.comboPort.Items.Add(port);
                if (port == selectedItem)
                    this.comboPort.Text = port;
            }
        }

        void UpdateCommandList()
        {
            comboCmd.Items.Clear();
            foreach (string cmd in Enum.GetNames(typeof(JappCommand)))
            {
                comboCmd.Items.Add(cmd);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (this.comm.IsConnected)
            {
                try
                {
                    this.comm.Close();
                }
                catch (CommException ex)
                {
                    this.txtOutput.AppendText(ex.Message);
                    this.txtOutput.AppendText("\r\n");
                }
                finally
                {
                    this.comboPort.Enabled = true;
                    this.comboCmd.Enabled = false;
                    this.txtCmdData.Enabled = false;
                    this.btnSendCmd.Enabled = false;
                    this.btnConnect.Text = "Connect";
                    this.txtOutput.AppendText("Disconnected.\r\n");
                }
            }
            else
            {
                try
                {
                    this.comm.Open(this.comboPort.Text);
                    this.comboPort.Enabled = false;
                    this.comboCmd.Enabled = true;
                    this.txtCmdData.Enabled = true;
                    this.btnSendCmd.Enabled = true;
                    this.btnConnect.Text = "Disconnect";
                    this.txtOutput.AppendText("Connected.\r\n");
                }
                catch (CommException ex)
                {
                    this.txtOutput.AppendText(ex.Message);
                    this.txtOutput.AppendText("\r\n");
                }
            }
        }

        private void btnSendCmd_Click(object sender, EventArgs e)
        {
            try
            {
                CommResponse response;

                if (txtCmdData.Text.Trim().Length > 0)
                {
                    string cmdData = txtCmdData.Text.Trim();

                    if (cmdData.Length % 2 != 0)
                    {
                        txtOutput.AppendText("Data length must be divisible by 2.\r\n");
                        return;
                    }

                    byte[] cmdBytes = new byte[cmdData.Length / 2];

                    for (int i = 0; i < cmdData.Length; i += 2)
                    {
                        cmdBytes[i / 2] = byte.Parse(cmdData.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                    }

                    response = comm.SendCommand(
                        (JappCommand)Enum.Parse(typeof(JappCommand), this.comboCmd.Items[this.comboCmd.SelectedIndex].ToString()),
                        cmdBytes);

                }
                else
                {

                    response = comm.SendCommand(
                        (JappCommand)Enum.Parse(typeof(JappCommand), this.comboCmd.Items[this.comboCmd.SelectedIndex].ToString()));
                }

                this.txtOutput.AppendText(response.Response.ToString());
                this.txtOutput.AppendText(" ");

                for (int i = 0; i < response.Data.Count(); i++)
                {
                    if (i == 0)
                        this.txtOutput.AppendText("(0x");
                    this.txtOutput.AppendText(response.Data[i].ToString("X2"));
                    if (i == response.Data.Count() - 1)
                        this.txtOutput.AppendText(")");
                }


                this.txtOutput.AppendText("\r\n");

            }
            catch (Exception ex)
            {
                this.txtOutput.AppendText(ex.Message);
                this.txtOutput.AppendText("\r\n");
            }
        }

        private void comboPort_DropDown(object sender, EventArgs e)
        {
            this.UpdateCOMPortList();
        }

        private void txtOutput_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
