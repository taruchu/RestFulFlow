using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsynchronousServer
{
    public class AsynchronousSocketListener
    {
        //Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public string _request { get; set; }
        public bool _stop { get; set; }
        private int _port { get; set; }
        private string _ipAddress { get; set; }
        private Socket _socket { get; set; }
        private const string EOF = "<EOF>";

        public AsynchronousSocketListener(string ipAddress = "127.0.0.1", int port = 11000)
        {
            _stop = false;
            _port = port;
            _ipAddress = ipAddress;
        }
        
        public bool Connected()
        {
            return _socket != null && _socket.Connected;
        }
        public void StartListening()
        {
            //Establish the local endpoint for the socket.
            //The DNS name of the computer
            //running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse(_ipAddress);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);

            //Create a TCP/IP socket.
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (_stop == false)
                {
                    //Set the event to nonsignaled state.
                    allDone.Reset();

                    //Start an asynchronous socket to listen for connections.
                    //Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    //Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            } 
        }

        public void AcceptCallback(IAsyncResult ar)
        { 
            //Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            _socket = listener.EndAccept(ar);
            _stop = true;
            allDone.Set();
        }

        public string GetClientRequest()
        {
            allDone.Reset();
            StateObject state = new StateObject();
            state.workSocket = _socket;
            _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            allDone.WaitOne();
            return _request;
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            //Retrieve the state object and the handler socket
            //from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            //Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                //There might be more data, so store the data recieved so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                //Check for end-of-file tag. If it is not there, read more data.
                content = state.sb.ToString();
                int eofIndex = content.IndexOf(EOF);
                if ( eofIndex > -1)
                {
                    _request = content.Remove(eofIndex);
                    //All the data has been read from the client. Display it on the console.
                    //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", request.Length, request);

                    if(handler.Connected)
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    

                     
                    //Signal the main thread to continue.
                    allDone.Set();
                    //Echo the data back to the client.
                    //Send(handler, content);
                }
                else
                {
                    //Not all data recieved. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
            else
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close(); 
                _stop = true; 
                allDone.Set();
            }
        }

        public void Send(String data)
        {
            //Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data + EOF);

            //Begin sending the data to the remote device.
            _socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), _socket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                //Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                //Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message, e);
            }
        }

        public void Close()
        {
            _socket.Close();
            _stop = true;
            allDone.Set();
        }
    }
}
