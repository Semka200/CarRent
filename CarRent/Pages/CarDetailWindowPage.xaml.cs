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
    /// Логика взаимодействия для CarDetailWindowPage.xaml
    /// </summary>
    public partial class CarDetailWindowPage : Page
    {
        public CarDetailWindowPage()
        {
            InitializeComponent();
        }

        public CarDetailWindowPage(Entities.Cars car)
        {
            InitializeComponent();
            DataContext = car;
            if (App.CurrentUser == null || App.CurrentUser.Role != "Admin")
            {
                Edid.Visibility = Visibility.Collapsed;
                Del.Visibility = Visibility.Collapsed;
            }
            StatusText.Text = car.Available ? "Доступен" : "Занят";

            if (App.CurrentUser == null)
            {
                Rent.Visibility = Visibility.Collapsed;
            }

            if (car.Available == false)
            {
                Rent.Visibility = Visibility.Collapsed;
            }

            // Загружаем фото
            string imagePath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Sourse",
                car.Brand + ".jpg");

            CarImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
        }
    }
}
