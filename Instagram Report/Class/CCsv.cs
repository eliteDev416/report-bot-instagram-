using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instagram_Email_Scrape.Class
{
    class CCsv
    {
        public class Instagram
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Followers { get; set; }
            public string Following { get; set; }
            public string Posts { get; set; }
            public string new_Followers { get; set; }
            public string new_Following { get; set; }
            public string new_Posts { get; set; }
            public string Image { get; set; }
        }

        public List<T> ReadCsv<T>(string path)
        {
            List<T> list = new List<T>();
            try
            {
                using (var textReader = File.OpenText(path))
                {
                    var csv = new CsvReader(textReader);
                    while (csv.Read())
                    {
                        var record = csv.GetRecord<T>();
                        list.Add(record);
                    }
                    textReader.Close();
                }
            }
            catch (Exception)
            {
            }
            return list;
        }
        public void AppendCsv<T>(T val, string path)
        {
            List<T> list = new List<T>();
            list.AddRange(ReadCsv<T>(path));
            list.Add(val);
            SaveCsv<T>(list, path);
        }
        public void SaveCsv<T>(List<T> list, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            using (CsvWriter cw = new CsvWriter(sw))
            {
                cw.WriteHeader<T>();
                cw.NextRecord();
                foreach (T item in list)
                {
                    cw.WriteRecord<T>(item);
                    cw.NextRecord();
                }
            }
        }
    }
}
