using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Tri_v_Ryad
{
    public class GameLogic
    {
        const int w = 8;                // размеры игрового поля
        const int pustota = -99;        // пустой тип ячейки, не содержащий картинку
        const int move = 15;
        int knop1 = -1, knop2 = -1;     //переменные, которые нужны для перестановки ячеек
        int X = -1;
        int Y = -1;
        public int moveleft = move;

        Random rng = new Random();

        Knopka[,] knop = new Knopka[w, w];


        public int counter = 0;
        public EventHandler drop;
        List<Knopka> rem = new List<Knopka>(); // лист для записи ячеек на удаление
        bool zamena;

        public GameLogic(Knopka[,] knop)
        {
            this.knop = knop;
        }

        public int score { get; set; } //число заработанных очков
        public int getScore()
        {
            return score;
        }

        public void setScore(int score)
        {
            this.score = score;
        }

        public void setMoveLf(int moveleft)
        {
            this.moveleft = moveleft;
        }

        public int getMoveLf()
        {
            return moveleft;
        }

        private void FallCellsss()
        {
            proverka();
            if (obnulenie())
            {
                while (obnulenie())
                {
                    create();
                    drop(this, null);
                    Thread.Sleep(100); //задержка замены ячеек
                }
                StartDrop();
            }


        }

        // заполнение поля камнями
        public void create()
        {
            int typeel = -1;

            for (int j = w - 1; j >= 0; j--)
                for (int i = w - 2; i >= 0; i--)
                {
                    if (knop[i + 1, j].typeofpic == pustota)
                    {
                        typeel = knop[i, j].typeofpic;

                        knop[i + 1, j].typeofpic = typeel;
                        knop[i, j].typeofpic = pustota;
                    }
                }
            for (int j = 0; j < w; j++)
                for (int i = 0; i < w; i++)
                {
                    if (knop[i, j].typeofpic == pustota)
                    {
                        knop[i, j].typeofpic = rng.Next(0, 5);
                    }
                    break;
                }
        }


        public void StartDrop()
        {
            //осуществляет запуск автоматического сдвига ячеек вниз в отдельном потоке
            Thread newThread = new Thread(new ThreadStart(FallCellsss));
            newThread.Start();
        }


        public bool obnulenie()
        {
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    if (knop[i, j] != null && knop[i, j].typeofpic == pustota) return true;
                }
            }
            return false;
        }

        // условие всей игры
        public void uslovie(int i, int j)
        {
            rem.Clear(); // очистка списка ячееку на удаление

            if (X == -1 && Y == -1 && knop[i, j] != null)
            {
                X = i;
                Y = j;
            }
            else
            {
                if ((Math.Abs(X - i) == 0 && Math.Abs(Y - j) == 1) || (Math.Abs(X - i) == 1 && Math.Abs(Y - j) == 0))
                {
                    // обмен информации между кнопками
                    knop1 = knop[i, j].typeofpic;
                    knop2 = knop[X, Y].typeofpic;
                    knop[X, Y].typeofpic = knop1;
                    knop[i, j].typeofpic = knop2;

                    // запуск проверки
                    List<Knopka> row = proverka();
                    if (row.Count() != 0)
                    {
                        proverka();
                    }

                    // возвращение к сходным данным 
                    if (knop[i, j].typeofpic == knop2)
                    {
                        knop1 = -1;
                        knop2 = -1;

                        X = -1;
                        Y = -1;
                    }

                    moveleft--;
                    zamena = true;
                }
                else if ((Math.Abs(X - i) == 1 && Math.Abs(Y - j) == 1) || (Math.Abs(X - i) >= 2 && Math.Abs(Y - j) >= 2))
                {
                    knop1 = -1;
                    knop2 = -1;

                    X = -1;
                    Y = -1;
                }
                else
                {
                    zamena = false;
                }
                if (zamena == true)
                {
                    knop1 = -1;
                    knop2 = -1;

                    X = -1;
                    Y = -1;

                    StartDrop();
                }
            }
        }

        // проверка поля на наличие 3 и более камней в ряду (столбце)
        public List<Knopka> proverka()
        {
            rem.Clear();

            int count; //счётчик

            // проверка поля по горизонтали
            for (int i = 0; i < w; i++)
            {
                if (knop[0, i] != null)
                {
                    int type = knop[0, i].typeofpic;
                    count = 1;
                    for (int j = 0; j < w; j++)
                    {
                        if (type == knop[i, j].typeofpic && j != 0)
                            count++;
                        else
                            count = 1;

                        if (count > 2)
                        {
                            rem.Add(knop[i, j - 2]);
                            rem.Add(knop[i, j - 1]);
                            rem.Add(knop[i, j]);
                        }
                        type = knop[i, j].typeofpic;
                    }
                }
            }
            // проверка поля по вертикали
            for (int j = 0; j < w; j++)
            {
                if (knop[0, j] != null)
                {
                    int type = knop[0, j].typeofpic;
                    count = 1;
                    for (int i = 0; i < w; i++)
                    {
                        if (type == knop[i, j].typeofpic && i != 0)
                            count++;
                        else
                            count = 1;

                        if (count > 2)
                        {
                            rem.Add(knop[i - 2, j]);
                            rem.Add(knop[i - 1, j]);
                            rem.Add(knop[i, j]);
                        }
                        type = knop[i, j].typeofpic;
                    }
                }
            }

            foreach (Knopka elem in rem)
            {
                elem.typeofpic = pustota;
            }

            score += rem.Count() * 5; //подсчет очков за каждое совпадение
            return rem;
        }
    }
}
