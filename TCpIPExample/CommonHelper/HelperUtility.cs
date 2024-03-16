using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace CommonHelper
{
    public class HelperUtility
    {
        #region Events
        public static event EventHandler<EventMessage> DispatchClientAcknowledgementEvent;
        private void OnDispatchClientAcknowledgementEventTrigger(string message)
        {
            EventMessage eventMessage = new EventMessage
            {
                DispatchedMessage = message,
            };
            DispatchClientAcknowledgementEvent?.Invoke(null, eventMessage);
        }

        public static event EventHandler<EventMessage> TCPReceivedMessageFromEvent;
        private void TCPReceivedMessageFromEventTrigger(string message)
        {
            EventMessage eventMessage = new EventMessage
            {
                DispatchedMessage = message,
            };
            TCPReceivedMessageFromEvent?.Invoke(null, eventMessage);
        }
        #endregion
        private const string ServerIpAddress = "127.0.0.9";
        private const int ServerPort = 8888;
        public  TcpClient client;
        private TcpListener listener;

        private static HelperUtility instance;
        private static readonly object objLock = new object();

        private HelperUtility() { }

        public static HelperUtility Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new HelperUtility();
                        }
                    }
                }
                return instance;
            }
        }

        #region TCP CLIENT
        public void Initialize()
        {
            client = new TcpClient();
        }
        public async void ConnectClientConnection()
        {
            client = new TcpClient();
            if (!client.Connected)
            {
                await client.ConnectAsync(ServerIpAddress, ServerPort);
            }
        }
        public void CloseClientConnection()
        {
            client.Close();
        }
        public async Task SendDataToTCPServer(string sentMessage)
        {
            try
            {
                try
                {
                    if (client.Connected)
                    {
                        string message = sentMessage;
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        using (NetworkStream stream = client.GetStream())
                        {
                            //Data Sent to server end
                            await stream.WriteAsync(data, 0, data.Length);
                            byte[] responseBuffer = new byte[1024];


                            //Acknowledgement received from server by client
                            int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                            string response = Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
                            OnDispatchClientAcknowledgementEventTrigger(response);
                            CloseClientConnection();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion


        #region TCP SERVER
        private async Task AcceptClients()
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                ReceivedDataByTCPServerFromClient(client);
            }
        }
        public async Task StartTcpServer()
        {
            try
            {
                // Start listening for client requests
                listener = new TcpListener(IPAddress.Any, ServerPort);
                if (!listener.Server.Connected)
                {
                    listener.Start();
                    TCPReceivedMessageFromEventTrigger("Server Started");
                    await AcceptClients();
                }
            }
            catch (Exception)
            {
            }
        }

        public async void ReceivedDataByTCPServerFromClient(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    //Data received on server end
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    TCPReceivedMessageFromEventTrigger(dataReceived);

                    // Echo the message back to the client
                    byte[] response = Encoding.ASCII.GetBytes($"Server has received data: {dataReceived}");
                    await stream.WriteAsync(response, 0, response.Length);
                    // Close client connection
                    client.Close();
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

    }
    public class EventMessage:EventArgs
    {
        public required string DispatchedMessage { get; set; }
    }
}
