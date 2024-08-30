using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RoutePlaner_Rafael_elias
{
    public partial class AddLogWindow : Window
    {
        public AddLogWindow()
        {
            InitializeComponent();

            // Set default placeholder texts
            SetPlaceholderText(DistanceTextBox, "Enter distance...");
            SetPlaceholderText(DurationTextBox, "Enter duration...");
            SetPlaceholderText(StepsTextBox, "Enter steps...");
            SetPlaceholderText(DifficultyTextBox, "Enter difficulty...");
            SetPlaceholderText(WeatherTextBox, "Enter weather...");
            SetPlaceholderText(CommentTextBox, "Enter comment...");
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetPlaceholderText(textBox, textBox.Tag.ToString());
            }
        }

        private void SetPlaceholderText(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.Foreground = Brushes.Gray;
            textBox.Tag = placeholder; // Store the placeholder text in the Tag property
        }
    }
}