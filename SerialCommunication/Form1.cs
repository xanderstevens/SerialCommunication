using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SerialCommunication
{
    public partial class Form1 : Form
    {
        // --- Globale variabelen voor Oefening 6 (Toestandsmachine) ---
        enum AlarmToestand { OK, ALARM, BEVESTIGD }
        AlarmToestand huidigeToestand = AlarmToestand.OK;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();
                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;

                comboBoxBaudrate.SelectedIndex = comboBoxBaudrate.Items.IndexOf("115200");
            }
            catch (Exception)
            { }
        }

        private void cboPoort_DropDown(object sender, EventArgs e)
        {
            try
            {
                string selected = (string)comboBoxPoort.SelectedItem;
                string[] portNames = SerialPort.GetPortNames().Distinct().ToArray();

                comboBoxPoort.Items.Clear();
                comboBoxPoort.Items.AddRange(portNames);

                comboBoxPoort.SelectedIndex = comboBoxPoort.Items.IndexOf(selected);
            }
            catch (Exception)
            {
                if (comboBoxPoort.Items.Count > 0) comboBoxPoort.SelectedIndex = 0;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    // ik heb een verbinding -> de gebruiker wil deze verbreken
                    serialPortArduino.Close();
                    radioButtonVerbonden.Checked = false;
                    buttonConnect.Text = "Connect";
                    labelStatus.Text = "Status: Disconnected";

                    // STOP de algemene verbindingsmonitor
                    timerVerbindingMonitor.Enabled = false;
                }
                else
                {
                    //ik heb geen verbinding -> de gebruiker wil deze maken
                    serialPortArduino.PortName = (string)comboBoxPoort.SelectedItem;
                    serialPortArduino.BaudRate = Int32.Parse((string)comboBoxBaudrate.SelectedItem);
                    serialPortArduino.DataBits = (int)numericUpDownDatabits.Value;

                    if (radioButtonParityEven.Checked) serialPortArduino.Parity = Parity.Even;
                    else if (radioButtonParityOdd.Checked) serialPortArduino.Parity = Parity.Odd;
                    else if (radioButtonParityNone.Checked) serialPortArduino.Parity = Parity.None;
                    else if (radioButtonParityMark.Checked) serialPortArduino.Parity = Parity.Mark;
                    else if (radioButtonParitySpace.Checked) serialPortArduino.Parity = Parity.Space;

                    if (radioButtonStopbitsNone.Checked) serialPortArduino.StopBits = StopBits.None;
                    else if (radioButtonStopbitsOne.Checked) serialPortArduino.StopBits = StopBits.One;
                    else if (radioButtonStopbitsOnePointFive.Checked) serialPortArduino.StopBits = StopBits.OnePointFive;
                    else if (radioButtonStopbitsTwo.Checked) serialPortArduino.StopBits = StopBits.Two;

                    if (radioButtonHandshakeNone.Checked) serialPortArduino.Handshake = Handshake.None;
                    else if (radioButtonHandshakeRTS.Checked) serialPortArduino.Handshake = Handshake.RequestToSend;
                    else if (radioButtonHandshakeRTSXonXoff.Checked) serialPortArduino.Handshake = Handshake.RequestToSendXOnXOff;
                    else if (radioButtonHandshakeXonXoff.Checked) serialPortArduino.Handshake = Handshake.XOnXOff;

                    serialPortArduino.RtsEnable = checkBoxRtsEnable.Checked;
                    serialPortArduino.DtrEnable = checkBoxDtrEnable.Checked;

                    serialPortArduino.Open();
                    string commando = "ping";
                    serialPortArduino.WriteLine(commando);
                    string antwoord = serialPortArduino.ReadLine();
                    antwoord = antwoord.TrimEnd();

                    if (antwoord == "pong")
                    {
                        radioButtonVerbonden.Checked = true;
                        buttonConnect.Text = "Disconnect";
                        labelStatus.Text = "Connected";

                        // START de algemene verbindingsmonitor
                        timerVerbindingMonitor.Enabled = true;
                    }
                    else
                    {
                        serialPortArduino.Close();
                        labelStatus.Text = "Error: verkeerd antwoord";
                    }
                }
            }
            catch (Exception exception)
            {
                labelStatus.Text = "Error: " + exception.Message;
                serialPortArduino.Close();
                radioButtonVerbonden.Checked = false;
                buttonConnect.Text = "Connect";
                timerVerbindingMonitor.Enabled = false;
            }
        }

        private void checkBoxDigital2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando;
                    if (checkBoxDigital2.Checked) commando = "set d2 high";
                    else commando = "set d2 low";
                    serialPortArduino.WriteLine(commando);
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        private void checkBoxDigital3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando;
                    if (checkBoxDigital3.Checked) commando = "set d3 high";
                    else commando = "set d3 low";
                    serialPortArduino.WriteLine(commando);
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        private void checkBoxDigital4_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando;
                    if (checkBoxDigital4.Checked) commando = "set d4 high";
                    else commando = "set d4 low";
                    serialPortArduino.WriteLine(commando);
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        private void trackBarPWM9_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando = String.Format("set pwm9 {0}", trackBarPWM9.Value);
                    serialPortArduino.WriteLine(commando);
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        private void trackBarPWM10_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando = String.Format("set pwm10 {0}", trackBarPWM10.Value);
                    serialPortArduino.WriteLine(commando);
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        private void trackBarPWM11_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    string commando = String.Format("set pwm11 {0}", trackBarPWM11.Value);
                    serialPortArduino.WriteLine(commando);
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerOefening3.Enabled = tabControl.SelectedIndex == 3;
            timerOefening4.Enabled = tabControl.SelectedIndex == 4;
            timerOefening5.Enabled = tabControl.SelectedIndex == 5;

            // Controleer of jouw Oefening 6 daadwerkelijk index 6 heeft. 
            // Als Oefening 6 het 6e tabblad is i.p.v. 7e, zet dit dan op == 5.
            timerOefening6.Enabled = tabControl.SelectedIndex == 6;
        }

        private void timerOefening3_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    serialPortArduino.ReadExisting();
                    string commando = "get d5";
                    serialPortArduino.WriteLine(commando);
                    string antwoord = serialPortArduino.ReadLine();
                    antwoord = antwoord.TrimEnd();
                    antwoord = antwoord.Substring(4);
                    radioButtonDigital5.Checked = (antwoord == "1");

                    commando = "get d6";
                    serialPortArduino.WriteLine(commando);
                    antwoord = serialPortArduino.ReadLine();
                    antwoord = antwoord.TrimEnd();
                    antwoord = antwoord.Substring(4);
                    radioButtonDigital6.Checked = (antwoord == "1");

                    commando = "get d7";
                    serialPortArduino.WriteLine(commando);
                    antwoord = serialPortArduino.ReadLine();
                    antwoord = antwoord.TrimEnd();
                    antwoord = antwoord.Substring(4);
                    radioButtonDigital7.Checked = (antwoord == "1");
                }
                else
                {
                    AfhandelenVerbrokenVerbinding("Status: Verbinding verbroken (kabel ontkoppeld)");
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        private void timerOefening4_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    serialPortArduino.ReadExisting();
                    string commando = "get a0";
                    serialPortArduino.WriteLine(commando);
                    string antwoord = serialPortArduino.ReadLine();
                    antwoord = antwoord.TrimEnd();
                    antwoord = antwoord.Substring(4);
                    int value = Int32.Parse(antwoord);
                    labelAnalog0.Text = value.ToString();
                }
                else
                {
                    AfhandelenVerbrokenVerbinding("Status: Verbinding verbroken (kabel ontkoppeld)");
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        private void timerOefening5_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    // 1. Wis oude buffer
                    serialPortArduino.ReadExisting();

                    // 2. Lees en herschaal potentiometer (Gewenste Temperatuur)
                    serialPortArduino.WriteLine("get a0");
                    string antwoordA0 = serialPortArduino.ReadLine().TrimEnd();
                    int analog0 = Int32.Parse(antwoordA0.Substring(4));
                    double gewensteTemp = (40.0 / 1023.0) * analog0 + 5.0;
                    labelGewensteTemp.Text = gewensteTemp.ToString("0.0") + " °C";

                    // 3. Lees en herschaal LM35 (Huidige Temperatuur)
                    serialPortArduino.WriteLine("get a1");
                    string antwoordA1 = serialPortArduino.ReadLine().TrimEnd();
                    int analog1 = Int32.Parse(antwoordA1.Substring(4));
                    double huidigeTemp = (500.0 / 1023.0) * analog1;
                    labelHuidigeTemp.Text = huidigeTemp.ToString("0.0") + " °C";

                    // 4. Stuur de LED aan op basis van de temperaturen
                    if (huidigeTemp < gewensteTemp)
                    {
                        serialPortArduino.WriteLine("set d2 high"); // Te koud -> Verwarming/LED AAN
                    }
                    else
                    {
                        serialPortArduino.WriteLine("set d2 low");  // Warm genoeg -> Verwarming/LED UIT
                    }
                }
                else
                {
                    AfhandelenVerbrokenVerbinding("Status: Verbinding verbroken (kabel ontkoppeld)");
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        // --- OEFENING 6: Temperatuur Alarm ---
        private void timerOefening6_Tick(object sender, EventArgs e)
        {
            try
            {
                if (serialPortArduino.IsOpen)
                {
                    // 1. Wis oude buffer
                    serialPortArduino.ReadExisting();

                    // 2. Lees en herschaal Potentiometer (A0 - Alarm Drempel)
                    // Range: -10°C tot +60°C (bereik van 70 graden)
                    serialPortArduino.WriteLine("get a0");
                    string antwoordA0 = serialPortArduino.ReadLine().TrimEnd();
                    int analog0 = Int32.Parse(antwoordA0.Substring(4));
                    double alarmTemp = (70.0 / 1023.0) * analog0 - 10.0;
                    labelOef6AlarmTemp.Text = alarmTemp.ToString("0.0") + " °C";

                    // 3. Lees en herschaal LM35 (A1 - Huidige Temperatuur)
                    serialPortArduino.WriteLine("get a1");
                    string antwoordA1 = serialPortArduino.ReadLine().TrimEnd();
                    int analog1 = Int32.Parse(antwoordA1.Substring(4));
                    double huidigeTemp = (500.0 / 1023.0) * analog1; // Datasheet LM35 (bij 5V)
                    labelOef6HuidigeTemp.Text = huidigeTemp.ToString("0.0") + " °C";

                    // 4. Lees de drukknop uit (D5 - Bevestigingsknop)
                    serialPortArduino.WriteLine("get d5");
                    string antwoordD5 = serialPortArduino.ReadLine().TrimEnd();
                    bool knopIngedrukt = (antwoordD5.Substring(4) == "1"); // Pas aan naar == "0" indien pull-up wordt gebruikt

                    // 5. TOESTANDSMACHINE LOGICA
                    switch (huidigeToestand)
                    {
                        case AlarmToestand.OK:
                            if (huidigeTemp > alarmTemp)
                            {
                                huidigeToestand = AlarmToestand.ALARM;
                            }
                            break;

                        case AlarmToestand.ALARM:
                            if (knopIngedrukt)
                            {
                                if (huidigeTemp < alarmTemp)
                                {
                                    // Als de temperatuur alweer gedaald was, ga direct naar OK
                                    huidigeToestand = AlarmToestand.OK;
                                }
                                else
                                {
                                    // Als het nog steeds te warm is, ga naar BEVESTIGD
                                    huidigeToestand = AlarmToestand.BEVESTIGD;
                                }
                            }
                            break;

                        case AlarmToestand.BEVESTIGD:
                            if (huidigeTemp < alarmTemp)
                            {
                                // Temperatuur is eindelijk onder het alarm gezakt
                                huidigeToestand = AlarmToestand.OK;
                            }
                            break;
                    }

                    // 6. STUUR HARDWARE AAN EN UPDATE UI OP BASIS VAN DE TOESTAND
                    labelOef6Toestand.Text = huidigeToestand.ToString();

                    if (huidigeToestand == AlarmToestand.OK)
                    {
                        serialPortArduino.WriteLine("set d2 low");  // LED UIT
                        serialPortArduino.WriteLine("set d3 low");  // Buzzer UIT
                    }
                    else if (huidigeToestand == AlarmToestand.ALARM)
                    {
                        serialPortArduino.WriteLine("set d2 high"); // LED AAN
                        serialPortArduino.WriteLine("set d3 high"); // Buzzer AAN
                    }
                    else if (huidigeToestand == AlarmToestand.BEVESTIGD)
                    {
                        serialPortArduino.WriteLine("set d2 high"); // LED AAN
                        serialPortArduino.WriteLine("set d3 low");  // Buzzer UIT
                    }
                }
                else
                {
                    AfhandelenVerbrokenVerbinding("Status: Verbinding verbroken (kabel ontkoppeld)");
                }
            }
            catch (Exception exception)
            {
                AfhandelenVerbrokenVerbinding("Error: " + exception.Message);
            }
        }

        // --- DE ACHTERGROND MONITOR ---
        private void timerVerbindingMonitor_Tick(object sender, EventArgs e)
        {
            if (radioButtonVerbonden.Checked && !serialPortArduino.IsOpen)
            {
                AfhandelenVerbrokenVerbinding("Status: Verbinding verbroken (kabel ontkoppeld)");
            }
        }

        private void AfhandelenVerbrokenVerbinding(string foutmelding)
        {
            timerOefening3.Enabled = false;
            timerOefening4.Enabled = false;
            timerOefening5.Enabled = false;
            timerOefening6.Enabled = false; // Zorg dat de nieuwe timer ook stopt

            timerVerbindingMonitor.Enabled = false;

            if (serialPortArduino.IsOpen)
            {
                serialPortArduino.Close();
            }

            labelStatus.Text = foutmelding;
            radioButtonVerbonden.Checked = false;
            buttonConnect.Text = "Connect";
        }
    }
}