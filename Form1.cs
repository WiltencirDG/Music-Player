using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using AxWMPLib;
using System.IO;

namespace Music_Player
{
    public partial class MusicPlayer : Form
    {
        #region Global variables
        string path = "";
        int last = 0;
        int first = 0;
        int id = 0;

        Random idMus = new Random();
        
        OpenFileDialog openMusic;

        bool needRes = false;
        bool isvisible = false;
        bool shuffle = false;
        bool repeat = false;
        bool endSong = false;

        List<string> playlist = new List<string>();
        #endregion

        // Inicialize Components
        public MusicPlayer()
        {
            InitializeComponent();      
        }
        
        // Quit Button
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // About Button
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Music Player created by: Wiltencir");
        }

        // Play Button
        private void playButton_Click(object sender, EventArgs e)
        {
            if (needRes == false)
            {
                WMPLib.IWMPControls3 controls = (WMPLib.IWMPControls3)Player.Ctlcontrols;
                if (controls.isAvailable["pause"])
                {
                    controls.pause();
                    playButton.Text = "Play";
                    needRes = true;
                }
            }
            else
            {
                WMPLib.IWMPControls3 controls2 = (WMPLib.IWMPControls3)Player.Ctlcontrols;
                if (controls2.isAvailable["play"])
                {
                    controls2.play();
                    playButton.Text = "Pause";
                    needRes = false;
                }
            }
        }

        // Stop Button
        private void stopButton_Click(object sender, EventArgs e)
        {
            WMPLib.IWMPControls3 controls = (WMPLib.IWMPControls3)Player.Ctlcontrols;
            if (controls.isAvailable["stop"])
            {
                Player.Ctlcontrols.stop();
            }
        }

        // Open Music Button
        private void openMusicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openMusic = new OpenFileDialog();
            openMusic.Filter = "|*.mp3|*.wma|*.wav";
            openMusic.Title = "Music File";
            openMusic.Multiselect = true;
            if (openMusic.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in openMusic.SafeFileNames)
                {
                    listBox1.Items.Add(file
                        .ToString()
                        .Replace(".mp3","")
                        );
                }
                foreach (String paths in openMusic.FileNames)
                {
                    path = paths.ToString();
                    playlist.Add(path.ToString());
                }
            }
            checkPlaylist();
            countPlaylist();
        }
        
        // Volume +
        private void plusVolume_Click(object sender, EventArgs e)
        {
            Player.settings.volume += 5;
            setVolumeText();
        }

        // Volume -
        private void minusVolume_Click(object sender, EventArgs e)
        {
            Player.settings.volume -= 5;
            setVolumeText();
        }

        // Setter Volume
        private void setVolumeText()
        {
            label1.Text = Player.settings.volume.ToString();
        }

        // On Load
        private void MusicPlayer_Load(object sender, EventArgs e)
        {
            setVolumeText();
            checkPlaylist();
        }

        // Playlist Button
        private void playlist_Click(object sender, EventArgs e)
        {
            if (isvisible == false)
            {                
                panel1.Visible = true;
                isvisible = true;
            }
            else
            {
                panel1.Visible = false;
                isvisible = false;
            }            
        }

        // Clear Button
        private void clear_Click(object sender, EventArgs e)
        {
            checkPlaylist();
            listBox1.Items.Clear();
            Player.Ctlcontrols.stop();
        }

        // Remove Button
        private void remove_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.Items.Count == 1)
                {
                    Player.Ctlcontrols.stop();
                    path = "";
                    checkPlaylist();
                }

                if (checkLastSong() == false)
                {
                    id = listBox1.SelectedIndex;
                    listBox1.Items.RemoveAt(id);
                    playlist.RemoveAt(id);
                    listBox1.SetSelected(id, true);
                    Player.URL = (playlist[id]);
                    checkPlaylist();
                }
                else
                {
                    id = listBox1.SelectedIndex;
                    listBox1.Items.RemoveAt(id);
                    playlist.RemoveAt(id);
                    countPlaylist();
                    checkPlaylist();
                    listBox1.SetSelected(last, true);
                    Player.URL = (playlist[last]);
                }
            }
            catch (ArgumentOutOfRangeException)
            {

            }
        }

        // Next Button
        private void nextMusic_Click(object sender, EventArgs e)
        {
            nextMusicFunc();
        }

        // Previous Button
        private void prevMusic_Click(object sender, EventArgs e)
        {
            prevMusicFunc();
        }

        // Next Music Function
        private void nextMusicFunc()
        {
            try
            {
                /*countPlaylist();
                id = listBox1.SelectedIndex +1;
                listBox1.SetSelected(id, true);
                this.Player.URL = (playlist[id]);
                Console.WriteLine(playlist[id]);
                */
                /*if ((shuffle == true) && ( repeat == false))
                {
                    if (checkLastSong() == false)
                    {
                        id = idMus.Next(last - 1);
                        Player.URL = (playlist[id]);
                        listBox1.SetSelected(id, true);
                        Player.Ctlcontrols.play();
                    }
                    else
                    {
                        Player.Ctlcontrols.stop();
                    }
                }
                else if ((shuffle == false) && (repeat == true))
                {
                    if (checkLastSong() == false)
                    {
                        listBox1.SetSelected(first, true);
                        Player.URL = (playlist[first]);
                        Player.Ctlcontrols.play();
                    }
                    else
                    {
                        id = Convert.ToInt32(listBox1.SelectedIndex + 1);
                        Player.URL = (playlist[id]);
                        Player.Ctlcontrols.play();
                        listBox1.SetSelected(id, true);
                    }
                }
                else if ((shuffle == true) && (repeat == true))
                {
                    if (checkLastSong() == true)
                    {
                        id = idMus.Next(last - 1);
                        Player.URL = (playlist[id]);
                        Player.Ctlcontrols.play();
                        listBox1.SetSelected(id, true);
                    }
                    else
                    {
                        id = idMus.Next(last - 1);
                        Player.URL = (playlist[id]);
                        Player.Ctlcontrols.play();
                        listBox1.SetSelected(id, true);
                    }
                }
                else
                {
                    if (checkLastSong() == false)
                    {
                        id = Convert.ToInt32(listBox1.SelectedIndex + 1);
                        Player.URL = (playlist[id]);
                        Player.Ctlcontrols.play();
                        listBox1.SetSelected(id, true);
                    }
                    else
                    {
                        listBox1.SetSelected(first, true);
                        Player.URL = (playlist[first]);
                        Player.Ctlcontrols.play();
                    }
                }
                */

            }
            catch (ArgumentOutOfRangeException)
            {
                listBox1.SetSelected(first, true);
                Player.URL = (playlist[first]);
            }
        }

        // Previous Music Function
        private void prevMusicFunc()
        {
            try
            {
                countPlaylist();
                if (shuffle == true)
                {
                    id = idMus.Next(last);
                    Player.URL = (playlist[id]);
                    listBox1.SetSelected(id, true);
                }
                else
                {
                    id = Convert.ToInt32(listBox1.SelectedIndex - 1);
                    Player.URL = (playlist[id]);
                    listBox1.SetSelected(id, true);
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                countPlaylist();
                listBox1.SetSelected(last, true);
                Player.URL = (playlist[last]);
            }
        }

        // Shuffle Button
        private void shuffleButton_Click(object sender, EventArgs e)
        {
            if (shuffle == false) 
            {
                shuffle = true;
                shuffleButton.BackColor = Color.DarkGray;
            }
            else
            {
                shuffle = false;
                shuffleButton.BackColor = Color.FromName("Control");
                        
            }
        }

        // Repeat Button
        private void repeatButton_Click(object sender, EventArgs e)
        {
            if (repeat == false)
            {
                repeat = true;
                repeatButton.BackColor = Color.DarkGray;

            }
            else
            {
                repeat = false;
                repeatButton.BackColor = Color.FromName("Control");
            }
        }

        // End of the song
        private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            try
            {
                if (e.newState == 8)
                {
                    Console.WriteLine("Media ended");
                    int index = listBox1.SelectedIndex +1;
                    listBox1.SetSelected(index, true);
                    Player.URL = @playlist[index];
                    Console.WriteLine(playlist[index]);
                    //nextMusicFunc();

                    /*if (endSong == false)
                    {
                        endSong = true;
                        nextMusicFunc();
                    }
                    else
                    {
                        endSong = false;
                        Player.Ctlcontrols.stop();
                    }
                    */
                }
            }
            catch
            {
                Console.WriteLine("ERRO PLAYSTATECHANGE");
            }
        }

        // Check if the Playlist is Empty
        private void checkPlaylist()
        {
            if(playlist.Count == 0)
            {
                nextMusic.Enabled = false;
                prevMusic.Enabled = false;
                playButton.Enabled = false;
                stopButton.Enabled = false;
                shuffleButton.Enabled = false;
                repeatButton.Enabled = false;
                clearButton.Enabled = false;
                removeButton.Enabled = false;
            }
            else
            {
                nextMusic.Enabled = true;
                prevMusic.Enabled = true;
                playButton.Enabled = true;
                stopButton.Enabled = true;
                shuffleButton.Enabled = true;
                repeatButton.Enabled = true;
                clearButton.Enabled = true;
                removeButton.Enabled = true;
            }
        }

        // Count how many items the playlist have
        private void countPlaylist()
        {
            last = listBox1.Items.Count - 1;
        }

        // Check if is the last song 
        private bool checkLastSong()
        {
            countPlaylist();
            if (endSong == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Playlist selection function
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                Player.URL = playlist[index];
            }
        }
    }
}
