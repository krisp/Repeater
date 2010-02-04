using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Collections;

namespace Repeater
{
    class SerialRepeater
    {
        private SerialPort port;
        private ArrayList listeners;

        public String PortName
        {
            get { return port.PortName; }
        }

        public SerialRepeater(SerialPort Serial_Port)
        {
            this.port = Serial_Port;
            this.listeners = new ArrayList();
            this.port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            this.port.Open();
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

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String data = this.port.ReadExisting();
            foreach (SerialRepeater sr in listeners)
            {
                sr.Write(data);
            }
        }

        public void Close()
        {            
            listeners = new ArrayList();
            port.Close();
            listeners = null;
        }
    }
}
