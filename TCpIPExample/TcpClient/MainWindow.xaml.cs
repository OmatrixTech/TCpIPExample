using CommonHelper;
using System.Text;
using System.Windows;


namespace TcpClientExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder sb;
        public MainWindow()
        {
            InitializeComponent();
            HelperUtility.Instance.Initialize();
            sb = new StringBuilder();
            HelperUtility.DispatchClientAcknowledgementEvent += HelperUtility_DispatchClientAcknowledgementEvent;
        }

        private async void BtnSendClientData_Click(object sender, RoutedEventArgs e)
        {
            HelperUtility.Instance.ConnectClientConnection();
            await HelperUtility.Instance.SendDataToTCPServer(TxtBxMessage.Text);
            //HelperUtility.Instance.CloseClientConnection();
        }

        private void HelperUtility_DispatchClientAcknowledgementEvent(object? sender, EventMessage e)
        {
            if (e != null)
            {
                sb.AppendLine(e.DispatchedMessage.ToString());
                TxtBlckInformation.Text = sb.ToString();
            }
        }

        private void BtnTcpClientConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelperUtility.Instance.ConnectClientConnection();
                sb.AppendLine("Tcp Client Connected to server...");
                TxtBlckInformation.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}