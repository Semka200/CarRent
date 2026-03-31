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
    /// Логика взаимодействия для EditCarPage.xaml
    /// </summary>
    public partial class EditCarPage : Page
    {
        private Entities.Cars _car;

        public EditCarPage(Entities.Cars car)
        {
            InitializeComponent();
            _car = car;

            BrandBox.Text = car.Brand;
            ModelBox.Text = car.Model;
            YearBox.Text = car.Year.ToString();
            BodyTypeBox.Text = car.BodyType;
            ColorBox.Text = car.Color;
            PriceBox.Text = car.Price.ToString();
            DescriptionBox.Text = car.Description;
            AvailableBox.IsChecked = car.Available;
            ImagePathBox.Text = car.Image;
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image files|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                ImagePathBox.Text = dialog.FileName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            BrandBox.BorderBrush = Brushes.Gray;
            ModelBox.BorderBrush = Brushes.Gray;
            YearBox.BorderBrush = Brushes.Gray;
            PriceBox.BorderBrush = Brushes.Gray;
      
            bool hasError = false;
            if (string.IsNullOrWhiteSpace(BrandBox.Text)) { BrandBox.BorderBrush = Brushes.Red; hasError = true; }
            if (string.IsNullOrWhiteSpace(ModelBox.Text)) { ModelBox.BorderBrush = Brushes.Red; hasError = true; }
            if (!int.TryParse(YearBox.Text, out _)) { YearBox.BorderBrush = Brushes.Red; hasError = true; }
            if (!decimal.TryParse(PriceBox.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _)) { PriceBox.BorderBrush = Brushes.Red; hasError = true; }

            if (hasError)
            {
                MessageBox.Show("Проверьте выделенные поля!");
                return;
            }

            try
            {
                _car.Brand = BrandBox.Text;
                _car.Model = ModelBox.Text;
                _car.Year = int.Parse(YearBox.Text);
                _car.BodyType = BodyTypeBox.Text;
                _car.Color = ColorBox.Text;
                _car.Price = decimal.Parse(PriceBox.Text.Replace(",", "."),
                             System.Globalization.CultureInfo.InvariantCulture);
                _car.Description = DescriptionBox.Text;
                _car.Available = AvailableBox.IsChecked == true;

                if (!string.IsNullOrEmpty(ImagePathBox.Text) && System.IO.File.Exists(ImagePathBox.Text))
                {
                    string fileName = System.IO.Path.GetFileName(ImagePathBox.Text);
                    string destFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sourse");
                    string destFile = System.IO.Path.Combine(destFolder, fileName);
                    System.IO.File.Copy(ImagePathBox.Text, destFile, true);
                    _car.Image = fileName;
                }


                App.Context.SaveChanges(); 
                MessageBox.Show("Изменения сохранены!");
                NavigationService.Navigate(new CarsPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
