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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем подсветку
            BrandBox.BorderBrush = Brushes.Gray;
            ModelBox.BorderBrush = Brushes.Gray;
            YearBox.BorderBrush = Brushes.Gray;
            PriceBox.BorderBrush = Brushes.Gray;

            // Проверяем поля
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
                string brand = BrandBox.Text;
                string model = ModelBox.Text;
                string bodyType = BodyTypeBox.Text;
                string color = ColorBox.Text;
                string description = DescriptionBox.Text;
                string sourceFile = ImagePathBox.Text;
                string fileName = System.IO.Path.GetFileName(sourceFile);
                string destFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sourse");
                string destFile = System.IO.Path.Combine(destFolder, fileName);

                if (!string.IsNullOrEmpty(sourceFile))
                {
                    System.IO.File.Copy(sourceFile, destFile, true);
                }

                string imagePath = fileName;
                int year = int.Parse(YearBox.Text);
                decimal price = decimal.Parse(PriceBox.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                var newCar = new Entities.Cars
                {
                    Brand = brand,
                    Model = model,
                    Year = year,
                    BodyType = bodyType,
                    Color = color,
                    Price = price,
                    Description = description,
                    Image = imagePath,
                    Available = true
                };

                App.Context.Cars.Add(newCar);
                App.Context.SaveChanges();

                MessageBox.Show("Автомобиль успешно добавлен!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
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
    }
}
