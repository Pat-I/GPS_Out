using System;
using System.IO.Ports;

namespace GPS_Out
{
    public class SerialComm
    {
        private readonly frmStart mf;
        private SerialPort Sport;

        public SerialComm(frmStart CalledFrom)
        {
            this.mf = CalledFrom;
            Sport = new SerialPort(Properties.Settings.Default.Port, Properties.Settings.Default.Baud);
            Sport.WriteTimeout = 500;
            Sport.Parity = Parity.None;
            Sport.DataBits = 8;
            Sport.StopBits = StopBits.One;

            if (Properties.Settings.Default.AutoConnect && Properties.Settings.Default.SerialSuccessful) Open();
        }

        public int Baud
        {
            get { return Sport.BaudRate; }
            set
            {
                if (!Sport.IsOpen && value > 0 && value < 115201)
                {
                    Sport.BaudRate = value;
                    Properties.Settings.Default.Baud = Sport.BaudRate;
                }
            }
        }

        public string PortNm
        {
            get { return Sport.PortName; }
            set
            {
                if (!Sport.IsOpen && value != "")
                {
                    Sport.PortName = value;
                    Properties.Settings.Default.Port = Sport.PortName;
                }
            }
        }

        public void Close()
        {
            try
            {
                if (Sport.IsOpen) Sport.Close();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/CloseRCport: " + ex.Message);
            }
        }

        public bool IsOpen()
        {
            return Sport.IsOpen;
        }

        public bool Open()
        {
            bool Result = false;
            try
            {
                if (SerialPortExists(Sport.PortName))
                {
                    if (!Sport.IsOpen) Sport.Open();

                    if (Sport.IsOpen)
                    {
                        Sport.DiscardOutBuffer();
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("SerialComm/OpenRCport: " + ex.Message);
            }
            Properties.Settings.Default.SerialSuccessful = Result;
            return Result;
        }

        public void SendStringData(String data)
        {
            if (Sport.IsOpen)
            {
                try
                {
                    Sport.WriteLine(data + "\r\n");
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("SerialComm/SendStringData: " + ex.Message);
                }
            }
        }

        private bool SerialPortExists(string Name)
        {
            bool Result = false;
            foreach (string s in SerialPort.GetPortNames())
            {
                if (s == Name)
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }
    }
}