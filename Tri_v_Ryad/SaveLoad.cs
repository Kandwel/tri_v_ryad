using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Tri_v_Ryad
{
    class SaveLoad
    {
        string FileName;

        public void SaveFile(List<Igrok> p)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            DialogResult dlgres = dlg.ShowDialog();
            JsonSerializer serializer = new JsonSerializer();

            if (dlgres == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(dlg.FileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, p);
                }
            }
        }
        public List<Igrok> LoadFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult dlgres = dlg.ShowDialog();
            if (dlgres == DialogResult.OK)
            {
                FileName = dlg.FileName;
            }
            return JsonConvert.DeserializeObject<List<Igrok>>
                (File.ReadAllText(FileName));

        }

    }
}
