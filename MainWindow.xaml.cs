using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Wpf_Task4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        public void AddDate()
        {
            var date = DateTime.Now.ToLongDateString();
            var lbi = new ListBoxItem();
            lbi.Content = date;
            lbi.HorizontalAlignment = HorizontalAlignment.Center;
            lbi.Foreground = (Brush)(new BrushConverter().ConvertFrom("#D0F4EA"));
            lbi.FontWeight = FontWeights.Bold;

            // Space
            Label lbl = new Label();
            lbl.Height = 10;

            Messages_lbox.Items.Add(lbi);
            Messages_lbox.Items.Add(lbl);
        }

        private void Message_tbox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TypeMessage_tbox.Text == "Type a message")
            {
                TypeMessage_tbox.Text = String.Empty;
                TypeMessage_tbox.Foreground = Brushes.Black;
            }
        }

        private void Message_tbox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (TypeMessage_tbox.Text == String.Empty)
            {
                TypeMessage_tbox.Text = "Type a message";
                TypeMessage_tbox.Foreground = Brushes.Gray;
            }
        }

        public Border CreateMessageBox(string message, int margin = 190)
        {
            int charactherCount = message.Length;
            int size = (charactherCount - 20) / 17 + 1;
            Border border = new Border()
            {
                CornerRadius = new CornerRadius(10),
                Background = (Brush)(new BrushConverter().ConvertFrom("#D0F4EA")),
                Width = 170,
                Height = 25 + size * 17,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(margin, 5, 0, 5),

            };

            TextBlock messageBlock = new TextBlock();
            messageBlock.Text = message;
            messageBlock.FontSize = 15;
            messageBlock.Background = (Brush)(new BrushConverter().ConvertFrom("#D0F4EA"));
            messageBlock.TextWrapping = TextWrapping.Wrap;
            messageBlock.TextTrimming = TextTrimming.None;
            messageBlock.HorizontalAlignment = HorizontalAlignment.Left;
            messageBlock.Margin = new Thickness(10, 0, 0, 0);

            border.Child = messageBlock;
   
            return border;
        }

        public bool firstMessageOfDay = true;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (TypeMessage_tbox.Foreground != Brushes.Gray)
            {
                if (firstMessageOfDay)
                {
                    AddDate();
                    firstMessageOfDay = false;
                }

                Message = TypeMessage_tbox.Text;
                var border = CreateMessageBox(Message, 190);

                Messages_lbox.Items.Add(border);
                Messages_lbox.Items.Refresh();

                GetAnswer(Message);

                if (VisualTreeHelper.GetChildrenCount(Messages_lbox) > 0)
                {
                    Border border2 = (Border)VisualTreeHelper.GetChild(Messages_lbox, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border2, 0);
                    scrollViewer.ScrollToBottom();
                }

                TypeMessage_tbox.Text = "Type a message";
                TypeMessage_tbox.Foreground = Brushes.Gray;
            }
        }

        public async void GetAnswer(string message)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://aeona3.p.rapidapi.com/?text={message}&userId=12312312312"),
                Headers =
    {
        { "X-RapidAPI-Key", "aad495b4cdmshbaa5f68a2f317e7p100b56jsn8cde16be11a0" },
        { "X-RapidAPI-Host", "aeona3.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var aiAnswerBox = CreateMessageBox(body.ToString(), 10);
                Messages_lbox.Items.Add(aiAnswerBox);
                Messages_lbox.Items.Refresh();

            }
        }
        public string Message { get; set; }
        private void Message_tbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
                return;
            }

            if (TypeMessage_tbox.Text == "Type a message" && TypeMessage_tbox.Foreground == Brushes.Gray)
            {
                TypeMessage_tbox.Text = String.Empty;
                TypeMessage_tbox.Foreground = Brushes.Black;
            }
        }
    }
}
