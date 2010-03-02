using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Collections;

namespace Repeater
{
    public class Repeater
    {
        private ArrayList repeaters;        
        private NotifyIcon notifyIcon;

        public RepeaterForm RepeaterForm { get { return new RepeaterForm(this); } }
        public ArrayList RepeaterList { get { return repeaters; } }     

        public Repeater()
        {                        
            notifyIcon = new NotifyIcon(this);
            repeaters = new ArrayList();

            ArrayList ar = Properties.Settings.Default.Repeaters;
            if (ar != null)
            {
                foreach (String pair in ar)
                {
                    String src = pair.Substring(0, pair.IndexOf(' '));
                    String dst = pair.Substring(pair.IndexOf('>') + 2, pair.Length - (pair.IndexOf('>') + 2));

                    AddSerialPairing(src, dst, Properties.Settings.Default.baudRate);
                }
            }
        }
        
        public bool AddSerialPairing(String source, String destination, int BaudRate)
        {
            SerialRepeater src = null;
            SerialRepeater dst = null;

            foreach (SerialRepeater sr in repeaters)
            {
                if (sr.PortName == source)                
                    src = sr;                
                else if (sr.PortName == destination)                
                    dst = sr;                
            }

            try
            {
                if (src == null)
                {
                    src = new SerialRepeater(new SerialPort(source, BaudRate));
                    repeaters.Add(src);
                }
            }
            catch (Exception e)
            {
                src = null;
                dst = null;
                throw new Exception("Error opening serial port " + source + ": " + e.ToString());                
            }

            try
            {
                if (dst == null)
                {
                    dst = new SerialRepeater(new SerialPort(destination, BaudRate));
                    repeaters.Add(dst);
                }
            }
            catch (Exception e)
            {
                src = null;
                dst = null;
                throw new Exception("Error opening serial port " + destination + ": " + e.ToString());
            }

            src.AddListener(dst);
            dst.AddListener(src);

            return true;
        }

        public bool RemoveSerialPairing(String source, String destination)
        {
            SerialRepeater src = null;
            SerialRepeater dst = null;

            try
            {
                foreach (SerialRepeater sr in repeaters)
                {
                    if (sr.PortName == source)
                        src = sr;
                    else if (sr.PortName == destination)
                        dst = sr;
                }

                if (src.RemoveListener(dst))
                {
                    repeaters.Remove(src);
                    src = null;
                }

                if (dst.RemoveListener(src))
                {
                    repeaters.Remove(dst);
                    dst = null;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error removing serial pairing: " + e.ToString());
            }
        }        

        public void EndProgram()
        {
            try
            {
                foreach (SerialRepeater sr in repeaters)
                {
                    sr.Close();
                }
                repeaters = null;
                notifyIcon.EndProgram();
            }
            catch { }
        }
    }
}
