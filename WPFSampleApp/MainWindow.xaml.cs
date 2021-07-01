using System.Windows;
using TelerikTogether.Barcode;
using TelerikTogether.Math;

namespace WPFSampleApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double pet = RadMath.DaiPet();
            MessageBox.Show(pet.ToString());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string barcodeImage = BarcodeEngine.GetFakeBarcodeImageFromText("https://www.telerik.com/");
            MessageBox.Show(barcodeImage);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var w = new Window() { Width = 400, Height = 400 };
            w.Content = new Sunburst() { Margin = new Thickness(15) };
            w.Show();
        }
    }
}
