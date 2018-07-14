using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousServer
{
    class StateObject
    {
        //Client socket
        public Socket workSocket = null;

        //Size of recieve buffer.
        public const int BufferSize = 1024;

        //Recieve buffer.
        public byte[] buffer = new byte[BufferSize];

        //Recieved data string.
        public StringBuilder sb = new StringBuilder();
    }

}
