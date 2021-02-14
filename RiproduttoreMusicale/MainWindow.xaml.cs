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
using TagLib;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls.Primitives;
using Estensione;

namespace RiproduttoreMusicale
{
    /// <summary>
    /// Progetto: Creazione di un riproduttore musicale con playlist e altre funzioni
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variabili globali
        //-------------------------------------------------------------
        GestioneMetadata md = null;
        Window objplay;
        ListView playlistnames;
        string imgAlbum;
        DispatcherTimer mytimer = new DispatcherTimer();
        bool resume = true;
        System.Windows.Controls.TextBox tb;
        Window playlistName;
        ListView lv;
        Window selectionSongs;
        List<Playlist> lista_Playlist = new List<Playlist>();
        WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
        ElencoBrani eb = new ElencoBrani();
        string temps;
        Brano temp;
        int index;
        bool max = false;
        bool checkPlay = true;
        //-------------------------------------------------------------
        #endregion
        public MainWindow()
        {
            //settaggio timer
            mytimer.Tick += Mytimer_Tick;
            mytimer.Interval = TimeSpan.FromMilliseconds(100);
            //settaggio messaggi errore windows media player
            player.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            InitializeComponent();
            //se esiste già un archivio con delle canzoni all'interno lo importa in una lista
            if (System.IO.File.Exists(Environment.CurrentDirectory + @"\..\..\Archivi\Songs.xml"))
            {
                //if true
                eb.elencobrani = GestioneFileXML.LeggiLista(@"../../Archivi/Songs.xml");
            }
        }
        //evento che gestisce i pulsanti laterali preinseriti via codice
        private void _Click(object sender, MouseButtonEventArgs e)
        {
            StackPanel stackpanelTemp = (StackPanel)sender;
            switch (stackpanelTemp.Name)
            {
                #region crea playlist
                case "crea_playlist":
                    //MessageBox.Show("crea playlist");
                    playlistName = new Window();
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Vertical;
                    //---
                    tb = new System.Windows.Controls.TextBox();
                    tb.Height = 30;
                    tb.HorizontalAlignment = HorizontalAlignment.Stretch;
                    Thickness margin = tb.Margin;
                    margin.Top = 10;
                    tb.Margin = margin;
                    //---
                    Button OkButton = new Button();
                    OkButton.Height = 30;
                    OkButton.Width = 100;
                    OkButton.Content = "Crea";
                    OkButton.Name = "Okbutton";
                    OkButton.Margin = margin;
                    OkButton.Click += OkButton_Click;
                    //---
                    sp.Children.Add(tb);
                    sp.Children.Add(OkButton);
                    playlistName.Content = sp;
                    playlistName.Width = 350;
                    playlistName.Height = 150;
                    playlistName.Title = "Insersci il nome della playlist";
                    playlistName.ResizeMode = ResizeMode.CanMinimize;
                    playlistName.Show();
                    break;
                #endregion
                #region aggiungi canzone
                case "add_song":
                    bool check = false;
                    OpenFileDialog filebrowser = new OpenFileDialog();
                    filebrowser.Filter = "Music files (*.mp3,*.flac,*.m4a,*.wav)| *.mp3;*.flac;*.m4a;*.wav";
                    filebrowser.InitialDirectory = @"C:\";
                    filebrowser.Title = "Select a song to import";
                    filebrowser.ShowDialog();
                    if (!string.IsNullOrEmpty(filebrowser.FileName))
                    {
                        foreach (var oggetto in eb.elencobrani)
                        {
                            if (oggetto.path == filebrowser.FileName)
                            {
                                check = true;
                            }
                        }
                        if (check == false)
                        {
                            ElaborateSongInfos(filebrowser.FileName);
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("La canzone è già presente nell'elenco dei brani, vuoi aggiungerla ugualmente?", "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                ElaborateSongInfos(filebrowser.FileName);
                            }
                        }
                    }
                    break;
                #endregion
                #region brani
                case "brani":
                    //MessageBox.Show("brani");
                    checkPlay = true;
                    if (System.IO.File.Exists(Environment.CurrentDirectory + @"\..\..\Archivi\Songs.xml"))
                    {
                        //if true
                        ImageSource a = null;
                        LV_Brani.ItemsSource = GestioneFileXML.LeggiLista(@"../../Archivi/Songs.xml");
                        PLaylist_text.Text = "Brani";
                        Album_image.Source = a;
                    }
                    break;
                #endregion
                #region Rimuovi playlist
                case "RemovePlaylist":
                    if (System.IO.File.Exists(Environment.CurrentDirectory + @"\..\..\Archivi\Playlists.xml"))
                    { 
                        objplay = new Window();
                        StackPanel stack = new StackPanel();
                        objplay.ResizeMode = ResizeMode.CanMinimize;
                        playlistnames = new ListView();
                        Button ok = new Button();
                        objplay.Title = "Seleziona una playlist da rimuovere";
                        objplay.Width = 400;
                        objplay.Height = 500;
                        lista_Playlist = GestioneFileXML.LeggiPlaylist(@"../../Archivi/Playlists.xml");
                        playlistnames.HorizontalAlignment = HorizontalAlignment.Stretch;
                        playlistnames.VerticalAlignment = VerticalAlignment.Stretch;
                        playlistnames.Height = 300;
                        GridView lvBinding = new GridView();
                        GridViewColumn nome = new GridViewColumn();
                        lvBinding.Columns.Add(nome);
                        playlistnames.View = lvBinding;
                        nome.Width = 400; nome.Header = "Nome"; nome.DisplayMemberBinding = new Binding("nomePlaylist");
                        playlistnames.ItemsSource = lista_Playlist;
                        stack.Orientation = Orientation.Vertical;
                        stack.Children.Add(playlistnames);
                        stack.Children.Add(ok);
                        ok.Content = "Ok";
                        ok.Width = 100;
                        ok.Height = 30;
                        ok.Click += Ok_Click;
                        objplay.Content = stack;
                        objplay.Show();
                    }
                    break;
                #endregion
                default:
                    MessageBox.Show("Errore");
                    break;
            }
        }
        //evento che viene utilizzato dal bottone "ok" che appare quando selezioni la playlist da eliminare
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            
            if (playlistnames.SelectedItems.Count > 0)
            {
                //MessageBox.Show("entro");
                Playlist ItemToRemove = null;
                foreach (var item in lista_Playlist)
                {
                    Playlist a = (Playlist)playlistnames.SelectedItem;
                    if (item.nomePlaylist == a.nomePlaylist)
                    {
                        ItemToRemove = item;
                    }
                }
                if (ItemToRemove != null)
                {
                    lista_Playlist.Remove(ItemToRemove);
                }
                GestioneFileXML.SetNome("Playlists.xml");
                GestioneFileXML.ScriviPlaylist(lista_Playlist);
                SP_Playlist.Children.Clear();
                foreach (var item in lista_Playlist)
                {
                    Button pp = new Button();
                    pp.Content = item.nomePlaylist;
                    pp.Name = item.nomePlaylist;
                    pp.Click += _Click_Playlist;
                    Thickness margin = pp.Margin;
                    margin.Top = 5;
                    pp.Margin = margin;
                    SP_Playlist.Children.Add(pp);
                }
            }
            objplay.Close();
        }
        //evento che viene utilizzato dal pulsante di conferma durante la creazione di una playlist
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            //MessageBox.Show(tb.Text);
            playlistName.Close();
            if (b.Content.ToString() == "Crea")
            {
                //MessageBox.Show(Environment.CurrentDirectory);
                if (!string.IsNullOrEmpty(tb.Text) && !string.IsNullOrWhiteSpace(tb.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Vuoi aggiungere una copertina personalizzata alla playlist?", "Domanda", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        OpenFileDialog imgPersonalizzata = new OpenFileDialog();
                        imgPersonalizzata.Filter = "Images files (*.png,*.jpeg,*.jpg)| *.png;*.jpeg;*.jpg";
                        imgPersonalizzata.InitialDirectory = @"C:\";
                        imgPersonalizzata.Title = "Select an image to import";
                        imgPersonalizzata.ShowDialog();
                        if (!string.IsNullOrEmpty(imgPersonalizzata.FileName))
                        {
                            imgAlbum = imgPersonalizzata.FileName;
                        }
                        else
                        {
                            imgAlbum = Environment.CurrentDirectory + " ../../../Images/default_cover.jpg";
                        }
                    }else
                    {
                        imgAlbum = Environment.CurrentDirectory + "../../../Images/default_cover.jpg";
                    }
                    selectionSongs = new Window();
                    lv = new ListView();
                    GridView lvBinding = new GridView();
                    GridViewColumn nome = new GridViewColumn();
                    nome.Width = 200; nome.Header = "Nome"; nome.DisplayMemberBinding = new Binding("nome");
                    GridViewColumn artista = new GridViewColumn();
                    artista.Width = 200; artista.Header = "Artista"; artista.DisplayMemberBinding = new Binding("artista");
                    lvBinding.Columns.Add(nome);
                    lvBinding.Columns.Add(artista);
                    lv.View = lvBinding;
                    if (System.IO.File.Exists(Environment.CurrentDirectory + @"\..\..\Archivi\Songs.xml"))
                    {
                        lv.ItemsSource = GestioneFileXML.LeggiLista(@"../../Archivi/Songs.xml");
                    }
                    lv.SelectionMode = SelectionMode.Multiple;
                    Button OkButton = new Button();
                    StackPanel sp = new StackPanel();
                    OkButton.Height = 30;
                    OkButton.Width = 100;
                    OkButton.Content = "Fatto";
                    OkButton.Name = "Okbutton";
                    Thickness margin = OkButton.Margin;
                    margin.Top = 10;
                    OkButton.Margin = margin;
                    OkButton.Click += OkButton_Click;
                    //---
                    sp.Children.Add(lv);
                    sp.Children.Add(OkButton);
                    selectionSongs.Content = sp;
                    selectionSongs.Title = "Seleziona i brani da inserire nella playlist";
                    selectionSongs.ResizeMode = ResizeMode.CanMinimize;
                    selectionSongs.Show();
                }
            }
            else
            {
                //apro window con lista e faccio selezionare i file, per poi andare a creare la playlist e la salvo in un archivio playlist
                //MessageBox.Show("entro")
                GestioneFileXML.SetNome("Playlists.xml");
                Playlist obj = new Playlist();
                obj.nomePlaylist = tb.Text;
                obj.pathImgAlbum = imgAlbum;
                foreach (var oggetto in lv.SelectedItems)
                {
                    obj.braniPlaylist.Add((Brano)oggetto);
                }
                lista_Playlist.Add(obj);
                Button pp = new Button();
                pp.Content = tb.Text;
                pp.Name = tb.Text;
                pp.Click += _Click_Playlist;
                Thickness margin = pp.Margin;
                margin.Top = 5;
                pp.Margin = margin;
                SP_Playlist.Children.Add(pp);
                selectionSongs.Close();
                GestioneFileXML.ScriviPlaylist(lista_Playlist);
            }
        }
        //quando un pulsante nella categoria playlist viene premuto fa partire questo evento che va a sostituire i brani nella listview con quelli della playlist
        private void _Click_Playlist(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            checkPlay = false;
            foreach (var item in lista_Playlist)
            {
                if (item.nomePlaylist == b.Name)
                {
                    LV_Brani.ItemsSource = item.braniPlaylist;
                    PLaylist_text.Text = item.nomePlaylist;
                    Album_image.Source = new ImageSourceConverter().ConvertFromString(item.pathImgAlbum) as ImageSource;
                }
            }
        }
        //evento di riferimento per i pulsanti che vanno modificare il comportamento del player
        private void _Click_Player(object sender, RoutedEventArgs e)
        {
            Image b = (Image)sender;
            switch (b.Name)
            {
                case "prev_song":
                    if (LV_Brani.SelectedItems.Count > 0)
                    {
                        //MessageBox.Show("prev");
                        temp = (Brano)LV_Brani.Items[index];
                        index--;
                        if (index >= 0)
                        {
                            Brano canzone = (Brano)LV_Brani.Items[index];
                            LV_Brani.SelectedIndex = index;
                            player.URL = canzone.path;
                            Album_image.Source = GeneraBitmap(canzone);
                            SongPlaying.Text = canzone.nome + " , " + canzone.artista;
                            player.controls.play();
                            mytimer.Start();
                        }
                        else
                            index = 0;
                    }
                    break;
                case "Pause_Resume":
                    //MessageBox.Show("p/R");
                    byte guardian = 0;
                    if (resume == false && guardian == 0)
                    {
                        //E' in pausa
                        Pause_Resume.Source = new BitmapImage(new Uri("../../Images/play.png", UriKind.Relative));
                        resume = true;
                        guardian = 1;
                        if (player.currentMedia != null)
                        {
                            mytimer.Stop();
                            player.controls.pause();
                        }
                    }
                    if (LV_Brani.SelectedItems.Count > 0) //controllo se c'è qualcosa selezionato prima di eseguire il codice
                    {
                        if (resume == true && guardian == 0)
                        {
                            //Sta riproducendo

                            Pause_Resume.Source = new BitmapImage(new Uri("../../Images/pause.png", UriKind.Relative));

                            resume = false;
                            guardian = 1;
                            temp = (Brano)LV_Brani.SelectedItem;
                            if (checkPlay == true)
                            {
                                Album_image.Source = GeneraBitmap(temp);
                            }
                            player.settings.volume = (int)volume.Value;
                            foreach (var oggetto in eb.elencobrani)
                            {
                                if (oggetto.path == temp.path)
                                {
                                    GestioneFileXML.SetNome("Songs.xml");
                                    oggetto.riproduzioni++;
                                    GestioneFileXML.ScriviXml(eb.elencobrani);
                                }
                            }
                            if (player.currentMedia != null && temps == temp.nome)  //permette di riprendere la riproduzione dopo che era stata messa in pausa
                            {
                                //MessageBox.Show("entro nel primo if");
                                mytimer.Start();
                                player.controls.play();

                            }
                            if (LV_Brani.SelectedItem != null && temps != temp.nome)  //permette di cambiare canzone, ha un controllo aggiuntivo per evitare che la canzone scelta sia la stessa
                            {
                                index = LV_Brani.Items.IndexOf(temp);
                                player.URL = temp.path;
                                temps = temp.nome;
                                SongPlaying.Text = temp.nome + " , " + temp.artista;
                                mytimer.Start();
                                player.controls.play();
                            }
                        }
                    }
                    break;
                case "next_song":
                    if (LV_Brani.SelectedItems.Count > 0)
                    {
                        //MessageBox.Show("next");
                        temp = (Brano)LV_Brani.Items[index];
                        index++;
                        //MessageBox.Show(index +"");
                        //MessageBox.Show(LV_Brani.Items.Count + "");
                        if (index <= LV_Brani.Items.Count - 1)
                        {
                            Brano canzone = (Brano)LV_Brani.Items[index];
                            LV_Brani.SelectedIndex = index;
                            player.URL = canzone.path;
                            Album_image.Source = GeneraBitmap(canzone);
                            SongPlaying.Text = canzone.nome + " , " + canzone.artista;
                            player.controls.play();
                            mytimer.Start();
                        }
                        else
                            index = LV_Brani.Items.Count - 1;
                    }
                    break;
                case "shuffle":
                    if (LV_Brani.Items.Count > 0)
                    {
                        int indiceGenerato = new Random().Next(0, eb.elencobrani.Count);
                        player.URL = eb.elencobrani[indiceGenerato].path;
                        Album_image.Source = GeneraBitmap(eb.elencobrani[indiceGenerato]);
                        LV_Brani.SelectedIndex = indiceGenerato;
                        SongPlaying.Text = eb.elencobrani[indiceGenerato].nome + " , " + eb.elencobrani[indiceGenerato].artista;
                        Pause_Resume.Source = new BitmapImage(new Uri("../../Images/pause.png", UriKind.Relative));
                        resume = false;
                        player.controls.play();
                        mytimer.Start();
                    }
                    break;
                default:
                    MessageBox.Show("Errore");
                    break;
            }
        }
        //evento richiamato dal timer, va a gestire la trackbar del brano
        private void Mytimer_Tick(object sender, EventArgs e)
        {
            trackbar.Maximum = (double)player.currentMedia.duration; //ho dovuto mettere qua la parte di codice perchè senò dava errore
            trackbar.Value = this.player.controls.currentPosition;

        }
        //elabora i file dei brani e ne estrapola l'artista,nome,durata e path
        public void ElaborateSongInfos(string path)
        {
            TagLib.File song = TagLib.File.Create(path);
            string nome = song.Tag.Title;
            TimeSpan durata = song.Properties.Duration;
            string artista = song.Tag.FirstPerformer;
            DateTime dataOdierna = DateTime.Now;
            string tempo = durata.Minutes + ":" + durata.Seconds;
            string data = dataOdierna.Day + "/" + dataOdierna.Month + "/" + dataOdierna.Year;
            GestioneFileXML.SetNome("Songs.xml");
            //extracting image from file
            if (song.Tag.Pictures.Length >= 1)
            {
                TagLib.IPicture pic = song.Tag.Pictures[0];
                //converto i metadata dell'immagine in base64
                string img = Convert.ToBase64String(pic.Data.Data);
                byte[] imgFromBase65 = Convert.FromBase64String(img);
                //MessageBox.Show(nome + " " + tempo + " " + artista + " " + data);
                if (System.IO.File.Exists(Environment.CurrentDirectory + @"\..\..\Archivi\Songs.xml"))
                {
                    //if true
                    eb.elencobrani = GestioneFileXML.LeggiLista(@"../../Archivi/Songs.xml");
                    eb.elencobrani.Add(new Brano(nome, artista, tempo, 0, data, path, imgFromBase65));
                    GestioneFileXML.ScriviXml(eb.elencobrani);
                }
                else
                {
                    eb.elencobrani.Add(new Brano(nome, artista, tempo, 0, data, path, imgFromBase65));
                    GestioneFileXML.ScriviXml(eb.elencobrani);
                }
            }
            else
            {
                if (System.IO.File.Exists(Environment.CurrentDirectory + @"\..\..\Archivi\Songs.xml"))
                {
                    //if true
                    eb.elencobrani = GestioneFileXML.LeggiLista(@"../../Archivi/Songs.xml");
                    eb.elencobrani.Add(new Brano(nome, artista, tempo, 0, data, path, null));
                    GestioneFileXML.ScriviXml(eb.elencobrani);
                }
                else
                {
                    eb.elencobrani.Add(new Brano(nome, artista, tempo, 0, data, path, null));
                    GestioneFileXML.ScriviXml(eb.elencobrani);
                }
            }
            LV_Brani.ItemsSource = eb.elencobrani;
            
        }
        //inizializza la listview con i brani già presenti nell'archivio ( se esiste)
        private void LV_Brani_Initialized(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Environment.CurrentDirectory + @"\..\..\Archivi\Songs.xml"))
            {
            //if true
             LV_Brani.ItemsSource = GestioneFileXML.LeggiLista(@"../../Archivi/Songs.xml");
            }
        }
        //All'avvio del programma (se c'è un archivio playlist) va a creare i bottoni con i corrispettivi nomi delle playlist salvate
        private void SP_Playlist_Initialized(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Environment.CurrentDirectory + @"\..\..\Archivi\Playlists.xml"))
            {
                //if true
                lista_Playlist = GestioneFileXML.LeggiPlaylist(@"../../Archivi/Playlists.xml");
            }
            foreach (var item in lista_Playlist)
            {
                Button pp = new Button();
                pp.Content = item.nomePlaylist;
                pp.Name = item.nomePlaylist;
                pp.Click += _Click_Playlist;
                Thickness margin = pp.Margin;
                margin.Top = 5;
                pp.Margin = margin;
                SP_Playlist.Children.Add(pp);
            }
        }
        //Metodo di riferimento dello slider per il volume, al cambiamento del sue valore esegue il codice specificato
        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.settings.volume = (int)volume.Value; //cambia il volume del brano
        }
        //Evento initialized che serve a settare un volume default che viene utilizzato dal player all'avvio
        private void Volume_Initialized(object sender, EventArgs e)
        {
            volume.Value = 50; 
        }
        //Evento utilizzato dalla barra di ricerca, utilizza un estensione del metodo Contains implementata nella classe Estensione.cs
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox searchbar = (System.Windows.Controls.TextBox)sender;
            string userString = searchbar.Text;
            List<Brano> tempList = new List<Brano>();
            foreach (var item in eb.elencobrani)
            {
                if (item.nome != null)
                {
                    if (item.nome.Contains(userString, StringComparison.OrdinalIgnoreCase))
                    {
                        tempList.Add(item);
                    }
                }
            }
            LV_Brani.ItemsSource = tempList;
        }
        //Serve a generare un BitmapImage da un array di byte
        private BitmapImage GeneraBitmap(Brano obj)
        {
            BitmapImage bitmap = new BitmapImage();
            if (obj.img != null)
            {
                MemoryStream ms = new MemoryStream(obj.img);
                ms.Seek(0, SeekOrigin.Begin);
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();
            }
            return bitmap;  
        }
        //Eventi che va a modificare i metadata di una canzone
        #region Modifica MetaData (doppio click per aprire)
        Brano song;
        public void ModifySongMetaData(object sender,MouseButtonEventArgs e) //da completare
        {
            lock(sender){
                song = (Brano)LV_Brani.SelectedItem;
                BitmapImage bitmap = new BitmapImage();
                

                bool checkimg = false;
                //Trasformo la canzone da byte a BitmapImage
                if (song.img != null) {
                    bitmap = GeneraBitmap(song);
                    checkimg = true;
                }

                //-----
                if (checkimg == true)
                {
                    md = new GestioneMetadata(song.artista, song.nome, bitmap);
                }
                if (checkimg == false)
                {
                    md = new GestioneMetadata(song.artista, song.nome, null);
                }
                md.Closed += Md_Closed;
                md.Show();
            }
        }
        private void Md_Closed(object sender, EventArgs e)
        {
            //test
            string titolo = md.ritornaTitolo();
            string[] artista = { md.ritornaArtista() };
            BitmapImage image = md.ritornaImmagine();
            var file = TagLib.File.Create(song.path);

            
            file.Tag.Title = titolo;
            file.Tag.Performers = artista;

            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            IPicture newArt = new Picture(data);
            file.Tag.Pictures = new IPicture[1] { newArt };
            file.Save();
            foreach (var obj in eb.elencobrani)
            {
                if (obj.path == song.path)
                {
                    obj.artista = artista[0];
                    obj.nome = titolo;
                    obj.img = data;
                    foreach (var item in lista_Playlist)
                    {
                        foreach (var Initem in item.braniPlaylist)
                        {
                            if (Initem.path == song.path)
                            {
                                obj.artista = artista[0];
                                obj.nome = titolo;
                                obj.img = data;
                            }
                        }
                    }
                }
            }

            LV_Brani.ItemsSource = eb.elencobrani;
            LV_Brani.Items.Refresh();
            GestioneFileXML.SetNome("Songs.xml");
            GestioneFileXML.ScriviXml(eb.elencobrani);
            GestioneFileXML.SetNome("Playlists.xml");
            GestioneFileXML.ScriviPlaylist(lista_Playlist);

            //MessageBox.Show(artista + titolo + "Ipotetica immagine");
        }
        #endregion
        #region Setta le immagini ai blocchi image (doppio click per aprire)
        //Eventi initialized che vanno a impostare l'immagine sui corrispettivi elementi
        private void Shuffle_Initialized(object sender, EventArgs e)
        {
            shuffle.Source = new BitmapImage(new Uri("../../Images/shuffle.png", UriKind.Relative));
        }
        private void Prev_song_Initialized(object sender, EventArgs e)
        {
            prev_song.Source = new BitmapImage(new Uri("../../Images/left-arrow.png", UriKind.Relative));
        }
        private void Pause_Resume_Initialized(object sender, EventArgs e)
        {
            Pause_Resume.Source = new BitmapImage(new Uri("../../Images/play.png", UriKind.Relative));
        }
        private void Next_song_Initialized(object sender, EventArgs e)
        {
            next_song.Source = new BitmapImage(new Uri("../../Images/right-arrow.png", UriKind.Relative));
        }
        #endregion
        #region Errori in mediaplayer (doppio click per aprire)
        //Eventi per gli errori del player
        private void Player_MediaError(object pMediaObject)
        {
            MessageBox.Show("Errore in media Player");
            this.Close();
        }

        #endregion
        //permette di rimuovere un brano
        private void RimuoviBrano_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (LV_Brani.SelectedItems.Count > 0)
            {
                Brano a = (Brano)LV_Brani.Items[LV_Brani.SelectedIndex];
                Brano temp = null;
                foreach (var item in eb.elencobrani)
                {
                    if (item.path == a.path)
                    {
                        temp = item;
                    }
                }
                if (temp != null)
                    eb.elencobrani.Remove(temp);
                temp = null;
                foreach (var item in lista_Playlist)
                {
                    foreach (var subitem in item.braniPlaylist)
                    {
                        if (subitem.path == a.path)
                        {
                            temp = subitem;
                        }
                    }
                    if (temp != null)
                        item.braniPlaylist.Remove(temp);
                }
                GestioneFileXML.SetNome("Songs.xml");
                GestioneFileXML.ScriviXml(eb.elencobrani);
                GestioneFileXML.SetNome("Playlists.xml");
                GestioneFileXML.ScriviPlaylist(lista_Playlist);
                LV_Brani.ItemsSource = eb.elencobrani;
                LV_Brani.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Selezione un brano");
            }
        }

        //rende possibile muovere la window sul desktop + operazioni sulla window
        private void AllowDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void window_close(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void window_minimize(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void window_maximize(object sender, MouseButtonEventArgs e)
        {
            int guardian = 0;
            if (max == true && guardian == 0)
            {
                this.WindowState = WindowState.Normal;
                guardian = 1;
                max = false;
            }
            if (max == false && guardian == 0)
            {
                this.WindowState = WindowState.Maximized;
                guardian = 1;
                max = true;
            }
        }
    }
}