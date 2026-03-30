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
            LoadCars(cars);
            var bodyTypes = App.Context.Cars
                .Select(c => c.BodyType)
                .Distinct()
                .ToList();

            bodyTypes.Insert(0, "Все");
            BodyTypeBox.ItemsSource = bodyTypes;
            BodyTypeBox.SelectedIndex = 0;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchBox.Text.ToLower();

            var cars = App.Context.Cars
                .ToList()
                .Where(c => c.Brand.ToLower().Contains(search) ||
                            c.Model.ToLower().Contains(search))
                .ToList();

            LoadCars(cars);
        }

        private void LoadCars(List<Entities.Cars> cars)
        {
            CarsPanel.Children.Clear();

            foreach (var car in cars)
            {
                var card = new Border
                {
                    Width = 200,
                    Height = 250,
                    Margin = new Thickness(10),
                    CornerRadius = new CornerRadius(8),
                    VerticalAlignment = VerticalAlignment.Center
                };

                if (car.Available == true)
                    card.Background = Brushes.LightGreen;
                else
                    card.Background = Brushes.LightCoral;

                string imagePath = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Sourse", car.Brand + ".jpg");

                var panel = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10)
                };

                var textName = new TextBlock
                {
                    Text = car.Brand + " " + car.Model,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var textPrice = new TextBlock
                {
                    Text = car.Price.ToString("0") + " руб/день",
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var textStatus = new TextBlock
                {
                    Text = car.Available ? "Доступен" : "Занят",
                    TextAlignment = TextAlignment.Center,
                    Foreground = car.Available ? Brushes.DarkGreen : Brushes.DarkRed
                };

                var image = new Image
                {
                    Height = 120,
                    Stretch = Stretch.UniformToFill,
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute))
                };

                panel.Children.Add(image);
                panel.Children.Add(textName);
                panel.Children.Add(textPrice);
                panel.Children.Add(textStatus);

                card.Child = panel;
                CarsPanel.Children.Add(card);

            }


        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            var cars = App.Context.Cars.ToList();

            // Фильтр по поиску
            string search = SearchBox.Text.ToLower();
            if (!string.IsNullOrEmpty(search))
                cars = cars.Where(c => c.Brand.ToLower().Contains(search) ||
                                       c.Model.ToLower().Contains(search)).ToList();

            // Фильтр по мин цене
            if (decimal.TryParse(MinPriceBox.Text, out decimal minPrice))
                cars = cars.Where(c => c.Price >= minPrice).ToList();

            // Фильтр по макс цене
            if (decimal.TryParse(MaxPriceBox.Text, out decimal maxPrice))
                cars = cars.Where(c => c.Price <= maxPrice).ToList();

            // Фильтр по типу кузова
            if (BodyTypeBox.SelectedItem != null && BodyTypeBox.SelectedItem.ToString() != "Все")
                cars = cars.Where(c => c.BodyType == BodyTypeBox.SelectedItem.ToString()).ToList();

            // Фильтр по доступности
            if (OnlyAvailable.IsChecked == true)
                cars = cars.Where(c => c.Available == true).ToList();

            LoadCars(cars);
        }
    }
}
