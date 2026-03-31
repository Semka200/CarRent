using CarRent.Entities;
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
    /// Логика взаимодействия для BookingWindowPage.xaml
    /// </summary>
    public partial class BookingWindowPage : Page
    {
        private Cars _selectedCar;

        // Конструктор, который принимает выбранный автомобиль
        public BookingWindowPage(Cars selectedCar)
        {
            InitializeComponent();
            _selectedCar = selectedCar;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            // Устанавливаем даты по умолчанию
            StartDate.SelectedDate = DateTime.Today;
            EndDate.SelectedDate = DateTime.Today.AddDays(1);

            // Минимальная дата - сегодня
            StartDate.DisplayDateStart = DateTime.Today;
            EndDate.DisplayDateStart = DateTime.Today.AddDays(1);
        }


        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateRentalCost();
        }

        private void CalculateRentalCost()
        {
            // Проверяем, что даты выбраны
            if (StartDate.SelectedDate == null || EndDate.SelectedDate == null)
            {
                DaysCount.Text = "0";
                TotalCost.Text = "0";
                return;
            }

            DateTime start = StartDate.SelectedDate.Value;
            DateTime end = EndDate.SelectedDate.Value;

            // Проверяем, что дата окончания позже даты начала
            if (end <= start)
            {
                DaysCount.Text = "0";
                TotalCost.Text = "0";
                MessageBox.Show("Дата окончания должна быть позже даты начала!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Считаем количество дней
            int days = (end - start).Days;
            DaysCount.Text = days.ToString();

            // Считаем итоговую стоимость
            decimal total = days * _selectedCar.Price;
            TotalCost.Text = total.ToString("N0");
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что даты выбраны
            if (StartDate.SelectedDate == null || EndDate.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите даты аренды",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime start = StartDate.SelectedDate.Value;
            DateTime end = EndDate.SelectedDate.Value;

            // Проверяем корректность дат
            if (end <= start)
            {
                MessageBox.Show("Дата окончания должна быть позже даты начала!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _selectedCar.Available = false;
            NavigationService.Navigate(new ContactDetailsPage(_selectedCar, start, end));
        }
    }
}