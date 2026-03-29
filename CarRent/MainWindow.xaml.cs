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

namespace CarRent
    {
        /// <summary>
        /// Логика взаимодействия для MainWindow.xaml
        /// </summary>
        public partial class MainWindow : Window
        {
            public MainWindow()
            {
                InitializeComponent();
                //Перенаправляем на страницу авторизации
                FrameMain.Navigate(new Pages.LoginPage());
                // инициализируем обработку события ContentRendered сразу после загрузки страницы:
                FrameMain.ContentRendered += new EventHandler(MainFrame_ContentRendered);

        }
            //кнопка назад
            private void BtnBack_Click(object sender, RoutedEventArgs e)
            {
                if (FrameMain.CanGoBack)
                    FrameMain.GoBack();
            }
            //Скрываем кнопку назад на первой странице
            private void MainFrame_ContentRendered(object sender, EventArgs e)
            {
                if (!FrameMain.CanGoBack || this.FrameMain.Content.GetType() == typeof(Pages.LoginPage))
                {
                    BtnBack.Visibility = Visibility.Collapsed;
                }
                else
                {
                    BtnBack.Visibility = Visibility.Visible;
                }
            }
        }
    }
