using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssettoCorsaSharedMemory
{
    public class ConnectionStatusEventArgs : EventArgs
    {
        public ACC_CONNECTION_STATUS connectionStatus {get; private set;}

        public ConnectionStatusEventArgs(ACC_CONNECTION_STATUS status)
        {
            connectionStatus = status;
        }
    }
}
