using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ChatB0t;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    ChatService chatService = null!;
    List<ChatHistoryRecord> chatHistoryRecords = new List<ChatHistoryRecord>();

    ChatHistoryRecord currentChatData = null!;

    public List<ChatHistoryRecord> ChatHistoryRecords
    {
        get => chatHistoryRecords;
        set { chatHistoryRecords = value; OnPropertyChanged(nameof(ChatHistoryRecords)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;


    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        chatService = new ChatService(NotifyMessageReceived);
        grdChatHistory.ItemsSource = ChatHistoryRecords;
    }

    private async void btnSend_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            txtUserInput.IsReadOnly = true;
            txtUserInput.Text = txtUserInput.Text.Trim();
            btnSend.IsEnabled = false;
            btnSend.Content = "Sending...";

            currentChatData = new ChatHistoryRecord() { Message = string.Empty, RecordType = ChatHistoryRecordType.Bot};

            chatHistoryRecords.Add(new ChatHistoryRecord() { Message = txtUserInput.Text, RecordType = ChatHistoryRecordType.User });
            chatHistoryRecords.Add(currentChatData);

            grdChatHistory.Items.Refresh();

            var reponse = await chatService.GetChatResponse(txtUserInput.Text);

            txtUserInput.Text = string.Empty;
        }
        catch (Exception ex)
        {
            chatHistoryRecords.Add(new ChatHistoryRecord() { Message = $"An error occurred while processing your request. Error : {ex.Message}", RecordType = ChatHistoryRecordType.Bot });
        }
        finally
        {
            txtUserInput.IsReadOnly = false;
            btnSend.IsEnabled = true; 
            btnSend.Content = "Send";
        }
    }

    private void NotifyMessageReceived(string message)
    {
        currentChatData.Message += message;
        scrollView.ScrollToBottom();
    }

    private void btnClear_Click(object sender, RoutedEventArgs e)
    {
        grdChatHistory.ItemsSource = new List<ChatHistoryRecord>();
        chatHistoryRecords.Clear();
        chatService.ClearChatHistory();
    }
}