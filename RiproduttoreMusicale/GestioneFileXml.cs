using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace RiproduttoreMusicale
{
    static class GestioneFileXML
    {
        static string path = Environment.CurrentDirectory + @"\..\..\Archivi\"; //percorso con \ finale
        static string nomeFile { get; set; }
        public static void SetNome(string n)
        {
            nomeFile = n;
        }
        public static void ScriviXml(List<Brano> l)
        {
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer xmls = new XmlSerializer(typeof(List<Brano>));
                StreamWriter sw = new StreamWriter(path + nomeFile, false); //aggiunge
                xmls.Serialize(sw, l);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore in scrittura file {0}\n{1}", path + nomeFile, e.Message); //messaggio di errore
            }
        }
        public static void ScriviPlaylist(List<Playlist> l)
        {
            try
            {
                //Istanzio l'oggetto serializzatore
                XmlSerializer xmls = new XmlSerializer(typeof(List<Playlist>));
                StreamWriter sw = new StreamWriter(path + nomeFile, false); //aggiunge
                xmls.Serialize(sw, l);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Errore in scrittura file {0}\n{1}", path + nomeFile, e.Message); //messaggio di errore
            }
        }
        public static List<Brano> LeggiLista(string pth)
        {
            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(List<Brano>));
                StreamReader sr = new StreamReader(pth);
                List<Brano> list = (List<Brano>)xmls.Deserialize(sr);
                sr.Close();
                return list;

            }
            catch (Exception e)
            {
                Console.WriteLine("Errore in scrittura file {0}\n{1}", pth, e.Message); //messaggio di errore
                return null;
            }
        }
        public static List<Playlist> LeggiPlaylist(string pth)
        {
            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(List<Playlist>));
                StreamReader sr = new StreamReader(pth);
                List<Playlist> list = (List<Playlist>)xmls.Deserialize(sr);
                sr.Close();
                return list;

            }
            catch (Exception e)
            {
                Console.WriteLine("Errore in scrittura file {0}\n{1}", pth, e.Message); //messaggio di errore
                return null;
            }
        }
    }
}
