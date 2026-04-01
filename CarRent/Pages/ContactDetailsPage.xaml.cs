using CarRent.Entities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace CarRent.Pages
{
    /// <summary>
    /// Логика взаимодействия для ContactDetailsPage.xaml
    /// </summary>
    public partial class ContactDetailsPage : Page
    {
        private Cars _selectedCar;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _rentalId;

        public ContactDetailsPage(Cars selectedCar, DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            _selectedCar = selectedCar;
            _startDate = startDate;
            _endDate = endDate;
        }

        // Метод для генерации QR-кода прямо в странице
        private BitmapImage GenerateQRCode(string content, int pixelSize = 10)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (Bitmap bitmap = qrCode.GetGraphic(pixelSize))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        bitmap.Save(memory, ImageFormat.Png);
                        memory.Position = 0;

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();

                        return bitmapImage;
                    }
                }
            }
        }

        private async void ConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем заполнение полей
            if (string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем корректность email
            if (!IsValidEmail(EmailBox.Text))
            {
                MessageBox.Show("Введите корректный email адрес!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверяем корректность телефона
            if (!IsValidPhone(PhoneBox.Text))
            {
                MessageBox.Show("Введите корректный номер телефона!\nФормат: +7XXXXXXXXXX или 8XXXXXXXXXX",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Создаем бронирование
                var rental = new Rentals
                {
                    UserID = App.CurrentUser?.ID,
                    CarID = _selectedCar.ID,
                    StartDate = _startDate,
                    EndDate = _endDate,
                    TotalPrice = (_endDate - _startDate).Days * _selectedCar.Price,
                    Status = "Active"
                };

                App.Context.Rentals.Add(rental);
                await App.Context.SaveChangesAsync();

                _rentalId = rental.ID;

                // Обновляем статус автомобиля
                _selectedCar.Available = false;
                await App.Context.SaveChangesAsync();

                // Формируем информацию для QR-кода
                int days = (_endDate - _startDate).Days;
                decimal totalPrice = days * _selectedCar.Price;

                string userName = App.CurrentUser != null ?
                    $"{App.CurrentUser.FirstName} {App.CurrentUser.LastName}" : "Гость";

                string qrContent = $@"
АРЕНДА АВТОМОБИЛЯ
━━━━━━━━━━━━━━━━━━━━
Номер брони: {_rentalId}
Автомобиль: {_selectedCar.Brand} {_selectedCar.Model}
Год выпуска: {_selectedCar.Year}
Период аренды: {_startDate:dd.MM.yyyy} - {_endDate:dd.MM.yyyy}
Количество дней: {days}
Стоимость: {totalPrice:C}
Арендатор: {userName}
Email: {EmailBox.Text}
Телефон: {PhoneBox.Text}
Дата бронирования: {DateTime.Now:dd.MM.yyyy HH:mm}
";

                // Генерируем QR-код
                var qrImage = GenerateQRCode(qrContent);
                QRCodeImage.Source = qrImage;

                // Показываем информацию о бронировании
                QRInfoText.Text = $"Номер бронирования: #{_rentalId}\n\n" +
                                 $"Автомобиль: {_selectedCar.Brand} {_selectedCar.Model}\n" +
                                 $"Период: {_startDate:dd.MM.yyyy} - {_endDate:dd.MM.yyyy}\n" +
                                 $"Стоимость: {totalPrice:C}\n\n" +
                                 $"Покажите этот QR-код при получении автомобиля.";

                // Скрываем форму и показываем QR-код
                FormPanel.Visibility = Visibility.Collapsed;
                QRPanel.Visibility = Visibility.Visible;

                MessageBox.Show($"Бронирование успешно создано!\nНомер бронирования: #{_rentalId}",
                    "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании бронирования: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToMainPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CarsPage());
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Убираем все нецифровые символы
            string digits = Regex.Replace(phone, @"[^\d]", "");
            // Проверяем длину (10 или 11 цифр)
            return digits.Length == 10 || digits.Length == 11;
        }
    }
}