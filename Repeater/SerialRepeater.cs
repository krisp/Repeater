using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Collections;
using System.Threading;

namespace Repeater
{
    class SerialRepeater
    {
        private SerialPort port;
        private ArrayList listeners;
        private Thread readThread;
        private bool threadStop = false;

        public String PortName
        {
            get { return port.PortName; }
        }

        public ArrayList Listeners { get { return listeners; } }

        public SerialRepeater(SerialPort Serial_Port)
        {
            try
            {
                this.port = Serial_Port;
                this.listeners = new ArrayList();                          
                this.port.Open();

                readThread = new Thread(new ThreadStart(ReadThread));
                readThread.Start();
            }
            catch(Exception e)
            {
                throw new Exception("Unable to open serial port " + this.port.PortName + ": " + e.Message);
            }
        }

        public void ReadThread()
        {
            while (!threadStop)
            {
                try
                {
                    byte b = (byte)port.ReadByte();
                    byte[] c = new byte[1];
                    c[0] = b;
                    foreach(SerialRepeater sr in listeners)
                        sr.Write(c, 0, 1);
                }
                catch { }
            }
        }

        public void AddListener(SerialRepeater ChildSR)
        {
            listeners.Add(ChildSR);
        }

        public bool RemoveListener(SerialRepeater ChildSR)
        {
            listeners.Remove(ChildSR);
            if (listeners.Count == 0)
            {
                port.Close();
                port = null;
                return true;
            }
            return false;
        }

        public void WriteLine(String data)
        {
            lock(port) port.WriteLine(data);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            lock (port) port.Write(buffer, offset, count);
        }

        public void Write(char[] buffer, int offset, int count)
        {
            lock (port) port.Write(buffer, offset, count);
        }

        public void Write(String data)
        {
            lock (port) port.Write(data);
        }

        public void Close()
        {
            try
            {
                threadStop = true;
                readThread = null;
                listeners = new ArrayList();
                port.Close();
                listeners = null;
            }
            catch { }
        }
    }
}
