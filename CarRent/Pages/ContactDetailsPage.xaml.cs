using CarRent.Entities;
using System;
using System.Collections.Generic;
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

namespace CarRent.Pages
{
    /// <summary>
    /// Логика взаимодействия для ContactDetailsPage.xaml
    /// </summary>
    public partial class ContactDetailsPage : Page
    {
        private int _carId;

        public ContactDetailsPage(Cars selectedCar, DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            _carId = selectedCar.ID;
        }


        private void GoToMainPage_Click(object sender, RoutedEventArgs e)
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

            // Проверяем корректность телефона (простая проверка)
            if (!IsValidPhone(PhoneBox.Text))
            {
                MessageBox.Show("Введите корректный номер телефона!\nФормат: +7XXXXXXXXXX или 8XXXXXXXXXX",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            App.Context.SaveChanges();

            // Показываем сообщение об успехе
            MessageBox.Show($"Бронирование успешно оформлено!",
                "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

            // Переходим на главную страницу
            NavigationService.Navigate(new CarsPage());
            StartTimer();
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
        private void StartTimer()
        {
            // Создаем таймер, который сработает через 20 секунд
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(20);
            timer.Tick += (s, e) =>
            {
                timer.Stop(); // Останавливаем таймер
                UpdateCarStatus(); // Обновляем статус автомобиля
            };
            timer.Start();
        }
        private void UpdateCarStatus()
        {
            try
            {
                // Получаем актуальные данные автомобиля из базы
                var car = App.Context.Cars.Find(_carId);

                if (car == null)
                {
                    // Автомобиль не найден
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Ошибка: автомобиль с ID {_carId} не найден!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return;
                }

                if (car.Available == false)
                {
                    // Меняем статус автомобиля на недоступный
                    car.Available = true;

                    // Сохраняем изменения
                    App.Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Ошибка при подтверждении бронирования: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }
    }
}
