using CommonHelper;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace TcpServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder sb;List<string> stringList;
        public MainWindow()
        {
            InitializeComponent();
            sb = new StringBuilder();
            stringList = new List<string>();
            HelperUtility.Instance.Initialize();
            HelperUtility.TCPReceivedMessageFromEvent += HelperUtility_TCPReceivedMessageFromEvent;
        }

        private void HelperUtility_TCPReceivedMessageFromEvent(object? sender, EventMessage e)
        {
            if (e != null)
            {
                sb.Append(e.DispatchedMessage);
                if (e.DispatchedMessage.Contains("Server Started"))
                {
                    TxtBlckInformation.Text = sb.ToString();
                }
                else
                {
                    //if (string.IsNullOrEmpty(TxtbxMessageReceived.Text))
                    //{
                        sb = new StringBuilder();
                    //}
                    //sb.Append(e.DispatchedMessage+Environment.NewLine);
                    stringList.Add(e.DispatchedMessage + Environment.NewLine);
                    string concatenatedString = string.Join(" ", stringList);
                    TxtbxMessageReceived.Text = concatenatedString;
                }
            }
        }

        private async void BtnServerStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await HelperUtility.Instance.StartTcpServer();
            }
            catch (Exception)
            {

            }
        }
    }
}