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
public partial class MainWindow : Window
{
    ChatService chatService = null!;
    StringBuilder chatHistory = new StringBuilder();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        chatService = new ChatService(NotifyMessageReceived);
    }

    private async void btnSend_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            btnSend.IsEnabled = false;
            btnSend.Content = "Sending...";

            var reponse = await chatService.GetChatResponse(txtUserInput.Text);

            chatHistory.AppendLine(reponse);
            txtUserInput.Text = string.Empty;
            txtResponse.Text = chatHistory.ToString();
        }
        catch (Exception ex)
        {
            txtResponse.Text = $"An error occurred while processing your request. Error : {ex.Message}";
        }
        finally
        {
            btnSend.IsEnabled = true; 
            btnSend.Content = "Send";
        }
    }

    private void NotifyMessageReceived(string message)
    {
        txtResponse.Text += message;
        scrollView.ScrollToBottom();
    }

    private void btnClear_Click(object sender, RoutedEventArgs e)
    {
        txtResponse.Text = string.Empty;
        chatHistory.Clear();
    }
}