using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Tri_v_Ryad
{
    public class Knopka
    {
        public int typeofpic { get; set; }

        public Button b = new Button();

        int razm = 35;

        public Knopka(int typeofpic, int tag)
        {
            this.typeofpic = typeofpic;


            b = new Button();
            b.Tag = tag;
            b.Width = razm;
            b.Height = razm;
            b.Content = "";
            b.Margin = new Thickness(2);
        }
    }
}
