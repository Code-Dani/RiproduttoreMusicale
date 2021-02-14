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
using System.Windows.Shapes;
using Microsoft.Win32;

namespace RiproduttoreMusicale
{
    /// <summary>
    /// Logica di interazione per GestioneMetadata.xaml
    /// </summary>
    public partial class GestioneMetadata : Window
    {
        #region variabili globali
        BitmapImage Immagine;
        BitmapImage defImage;
        string defTitolo;
        string defArtista;
        #endregion

        public GestioneMetadata(string Artista, string Titolo, BitmapImage immagine)
        {
            InitializeComponent();
            TB_artista.Text = Artista;
            defArtista = Artista;
            defTitolo = Titolo;
            TB_titolo.Text = Titolo;

            if (immagine != null)
            {
                Immagine = immagine;
                defImage = immagine;
                Img_Album.Source = immagine;
            }
            else
            {
                defImage = new BitmapImage(new Uri("../../Images/default_cover.jpg",UriKind.Relative));
                Immagine = new BitmapImage(new Uri("../../Images/default_cover.jpg", UriKind.Relative));
            }
        }
        private void CB_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Name == "CB_Titolo" && cb.IsChecked == true)
            {
                TB_titolo.IsEnabled = true;
            }
            if (cb.Name == "CB_Titolo" && cb.IsChecked == false)
            {
                TB_titolo.IsEnabled = false;
            }
            if (cb.Name == "CB_Artista" && cb.IsChecked == true)
            {
                TB_artista.IsEnabled = true;
            }
            if (cb.Name == "CB_Artista" && cb.IsChecked == false)
            {
                TB_artista.IsEnabled = false;
            }
        }
        public BitmapImage ritornaImmagine()
        {
            return Immagine;
        }
        public string ritornaTitolo()
        {
            return TB_titolo.Text;
        }
        public string ritornaArtista()
        {
            return TB_artista.Text;
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog imgPersonalizzata = new OpenFileDialog();
            imgPersonalizzata.Filter = "Images files (*.png,*.jpeg,*.jpg)| *.png;*.jpeg;*.jpg";
            imgPersonalizzata.InitialDirectory = @"C:\";
            imgPersonalizzata.Title = "Select an image to import";
            imgPersonalizzata.ShowDialog();
            if (!string.IsNullOrEmpty(imgPersonalizzata.FileName))
            {
                Immagine = new BitmapImage(new Uri(imgPersonalizzata.FileName));
                Img_Album.Source = Immagine;
            }
        }
        private void Salva_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            Immagine = defImage;
            TB_artista.Text = defArtista;
            TB_titolo.Text = defTitolo;
            this.Close();
        }
    }
}