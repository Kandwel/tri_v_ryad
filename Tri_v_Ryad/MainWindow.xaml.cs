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



namespace Tri_v_Ryad
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int razm = 35;
        const int w = 8; //константа размера поля
        const int pustota = -99; //константа пустого типа ячеек
        const int pogreshnost = 5 * ((w - 2) * 3 * w * 2); //константа погрешности при подсчете очков
        const int moves = 3; //константа числа ходов
        int scr = 0;

        BitmapImage[] typedpic = new BitmapImage[]
        {
            new BitmapImage(new Uri(@"pack://application:,,,/pics/0.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/pics/1.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/pics/2.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/pics/3.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/pics/4.png", UriKind.Absolute)),
        };

        Knopka[,] knop = new Knopka[w, w]; //массив игровых ячеек
        GameLogic gl;

        Igrok igr; //переменная класса игрока
        List<Igrok> ratelist = new List<Igrok>(); //список рейтинга игроков
        SaveLoad sl = new SaveLoad();

        public MainWindow()
        {
            InitializeComponent();
            //указыается количество строк и столбцов в сетке
            ugr.Rows = w;
            ugr.Columns = w;

            //указываются размеры сетки (число ячеек * (размер кнопки в ячейки + толщина её границ))
            ugr.Width = w * (razm + 4);
            ugr.Height = w * (razm + 4);

            //толщина границ сетки
            ugr.Margin = new Thickness(5, 50, 150, 50);
            updateKnop();

            gl = new GameLogic(knop);
        }

        // генерирует поле
        void Generate()
        {
            for (int i = 0; i < w; i++)
                for (int j = 0; j < w; j++)
                {
                    knop[i, j] = new Knopka(pustota, i + j * w);
                    StackPanel sp = new StackPanel();
                    sp.Margin = new Thickness(1);
                    knop[i, j].b.Click += Knop_Click;
                    ugr.Children.Add(knop[i, j].b);
                }

        }

        // обновляет поле
        void updateKnop()
        {
            for (int i = 0; i < w; i++)
                for (int j = 0; j < w; j++)
                {

                    if (knop[i, j] != null)
                    {
                        StackPanel sp = new StackPanel();

                        int typeel = knop[i, j].typeofpic;
                        if (typeel != pustota)
                        {
                            BitmapImage image = typedpic[typeel];
                            sp = getPanel(image);
                        }
                        knop[i, j].b.Content = sp;
                    }
                }
            if (gl != null)
            {
                score.Content = Convert.ToString(gl.getScore() - pogreshnost);
                scr = gl.getScore() - pogreshnost;
                mv.Content = gl.moveleft;
                if (gl.getMoveLf() == 0)
                {
                    for (int i = 0; i < w; i++)
                        for (int j = 0; j < w; j++)
                            knop[i, j].b.IsEnabled = false;
                    itog1.Content = "Сохраните результат!!!";
                }
            }
        }


        // анимация падения
        private void drop(object sender, EventArgs args)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                updateKnop();
            });
        }


        StackPanel getPanel(BitmapImage pic)
        {
            StackPanel sp = new StackPanel();

            //создание контейнера для хранения изображения
            Image img = new Image();
            img.Source = pic;
            sp.Children.Add(img);

            //толщина границ кнопки
            sp.Margin = new Thickness(1);

            return sp;
        }

        // запучк игры
        private void str_Click(object sender, RoutedEventArgs e)
        {
            ugr.Children.Clear();
            Generate();
            gl.setScore(0);
            gl.setMoveLf(moves);
            gl.drop += drop;
            updateKnop();
            gl.StartDrop();
            itog1.Content = "";
            itog2.Content = "";
        }

        
        void Knop_Click(object sender, RoutedEventArgs e)
        {
            int ind = (int)((Button)sender).Tag;
            int i = ind % w;
            int j = ind / w;

            gl.uslovie(i, j);
            updateKnop();
        }

        private void SaveRez_Click(object sender, RoutedEventArgs e)
        {
            NameAdd win2 = new NameAdd();
            if (win2.ShowDialog() == true)
            {
                igr = new Igrok(win2.PlayerName.Text, scr);
                records.Items.Add(igr.name + ":     " + igr.score);
                PlayerName.Content = igr.name;
                finscore.Content = igr.score;
                itog1.Content = "Перед запуском новой игрой";
                itog2.Content = "сохраните таблицу!";
            }

        }

        private void SaveTab_Click(object sender, RoutedEventArgs e)
        {
            records.Items.Clear();
            igr.setScore(Convert.ToInt32(finscore.Content));

            ratelist.Add(igr);

            var sortedPlayers = from r in ratelist
                                orderby r.score descending
                                select r;

            foreach (Igrok igr in sortedPlayers)
                records.Items.Add(igr.name + ":     " + igr.score);


            score.Content = "0";

            sl.SaveFile(ratelist);

        }

        private void LoadTab_Click(object sender, RoutedEventArgs e)
        {
            records.Items.Clear();

            ratelist = sl.LoadFile();

            //сортировка списка рейтинга по убыванию числа набранных очков
            var sortedPlayers = from r in ratelist
                                orderby r.score descending
                                select r;

            foreach (Igrok igr in sortedPlayers)
                records.Items.Add(igr.name + ":     " + igr.score);

        }
    }
}
