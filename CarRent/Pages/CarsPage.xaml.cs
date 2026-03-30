using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CarRent.Pages
{
    /// <summary>
    /// Логика взаимодействия для CarsPage.xaml
    /// </summary>
    public partial class CarsPage : Page
    {
        public CarsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var cars = App.Context.Cars.ToList();

            foreach (var car in cars)
            {
                // Карточка — это Border с текстом внутри
                var card = new Border
                {
                    Width = 200,
                    Height = 250,
                    Margin = new Thickness(10),
                    Background = Brushes.LightGray,
                    CornerRadius = new CornerRadius(8),
                    VerticalAlignment = VerticalAlignment.Center
                };

                if (car.Available == true)
                {
                    card.Background = Brushes.LightGreen;
                }
                else
                {
                    card.Background = Brushes.LightCoral;
                }

                var text = new TextBlock
                {
                    Text = car.Brand + " " + car.Model + "\n" + car.Price.ToString("0") + " руб/день",
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                card.Child = text;
                CarsPanel.Children.Add(card);
            }
        }
    }
}
