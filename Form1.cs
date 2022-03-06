using System;
using System.Drawing;
using System.Windows.Forms;

namespace MusicPlayerApp
{
    public partial class Form1 : Form
    {
        int x1 = 12, StartIndex = 0; ///indexul de start
        string[] CaleFila, NumeFila; /// cu aceste siruri de caract extragem si memoram fisierele de muzica 
        bool _redata = false, _muta = false, _repetata = false;
        public EventHandler onActon = null, onActon2 = null, onrepeat = null;

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // no borders
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true); // this is to avoid visual artifacts
            this.TransparencyKey = Color.Empty;


            GC.KeepAlive(timer1);
            GC.KeepAlive(timer2);
            timer1.GetLifetimeService();
            timer2.GetLifetimeService();

            axWindowsMediaPlayer1.settings.setMode("loop", false);

            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.btnclose, "Închidere");
            System.Windows.Forms.ToolTip ToolTip2 = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.btnmaximize, "Restaurare");
            System.Windows.Forms.ToolTip ToolTip3 = new System.Windows.Forms.ToolTip();
            ToolTip3.SetToolTip(this.btnminimize, "Minimizare");

            _ = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.bunifuImageButton6, "Închidere");
            _ = new System.Windows.Forms.ToolTip();
            ToolTip2.SetToolTip(this.bunifuImageButton3, "Restaurare");
            _ = new System.Windows.Forms.ToolTip();
            ToolTip3.SetToolTip(this.bunifuImageButton5, "Minimizare");


            System.Windows.Forms.ToolTip ToolTip4 = new System.Windows.Forms.ToolTip();
            ToolTip4.SetToolTip(this.bunifuImageButton4, "Activaţi/Dezactivaţi sunetul");
            System.Windows.Forms.ToolTip ToolTip5 = new System.Windows.Forms.ToolTip();
            ToolTip5.SetToolTip(this.repeat, "Activaţi/Dezactivaţi repetare melodie");
            System.Windows.Forms.ToolTip ToolTip6 = new System.Windows.Forms.ToolTip();
            ToolTip5.SetToolTip(this.btnprev, "Precedent");
            System.Windows.Forms.ToolTip ToolTip7 = new System.Windows.Forms.ToolTip();
            ToolTip6.SetToolTip(this.actionbtn, "Pauză/Redă");
            System.Windows.Forms.ToolTip ToolTip8 = new System.Windows.Forms.ToolTip();
            ToolTip7.SetToolTip(this.btnnext, "Următor");

            _ = new System.Windows.Forms.ToolTip();
            ToolTip8.SetToolTip(this.btnstop, "Întrerupere");
            _ = new System.Windows.Forms.ToolTip();
            ToolTip8.SetToolTip(this.shufflebtn, "Apasă pentru a reda aleatoriu");
            _ = new System.Windows.Forms.ToolTip();
            ToolTip4.SetToolTip(this.inapoi10, "10 secunde înapoi");
            _ = new System.Windows.Forms.ToolTip();
            ToolTip4.SetToolTip(this.inainte10, "10 secunde înainte");

            //oprim comenzile pt a nu putea fi utilizate daca nu s-au adaugat piese muzicale
            inapoi10.Enabled = repeat.Enabled = btnprev.Enabled = actionbtn.Enabled = btnnext.Enabled = inainte10.Enabled = shufflebtn.Enabled = btnstop.Enabled = false;
        }

        //This gives us the ability to resize the borderless from any borders instead of just the lower right corner
        protected override void WndProc(ref Message m)
        {
            const int wmNcHitTest = 0x84;
            const int htLeft = 10, htRight = 11;
            const int htTop = 12, htTopLeft = 13, htTopRight = 14;
            const int htBottom = 15, htBottomLeft = 16, htBottomRight = 17;

            if (m.Msg == wmNcHitTest)
            {
                int x = (int)(m.LParam.ToInt64() & 0xFFFF);
                int y = (int)((m.LParam.ToInt64() & 0xFFFF0000) >> 16);
                Point pt = PointToClient(new Point(x, y));
                Size clientSize = ClientSize;
                ///allow resize on the lower right corner
                if (pt.X >= clientSize.Width - 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomLeft : htBottomRight);
                    return;
                }
                ///allow resize on the lower left corner
                if (pt.X <= 16 && pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htBottomRight : htBottomLeft);
                    return;
                }
                ///allow resize on the upper right corner
                if (pt.X <= 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htTopRight : htTopLeft);
                    return;
                }
                ///allow resize on the upper left corner
                if (pt.X >= clientSize.Width - 16 && pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(IsMirrored ? htTopLeft : htTopRight);
                    return;
                }
                ///allow resize on the top border
                if (pt.Y <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htTop);
                    return;
                }
                ///allow resize on the bottom border
                if (pt.Y >= clientSize.Height - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htBottom);
                    return;
                }
                ///allow resize on the left border
                if (pt.X <= 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htLeft);
                    return;
                }
                ///allow resize on the right border
                if (pt.X >= clientSize.Width - 16 && clientSize.Height >= 16)
                {
                    m.Result = (IntPtr)(htRight);
                    return;
                }
            }
            base.WndProc(ref m);
        }
        ///This gives us the drop shadow behind the borderless form
        private const int CS_DROPSHADOW = 0x20000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }

        //butoane de inchidere, maximizare si minimizare a aplicatiei de  muzica
        private void Btnclose_Click(object sender, EventArgs e)
        {
            Application.Exit(); //se inchide aplicatia
        }

        private void Btnmaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) //daca este maximizata
                this.WindowState = FormWindowState.Normal; //se face fereastra la normal la apasarea butonului
            else //daca nu este maximizata fereastra
            {
                MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea; //limita de marire pana la bara de instrumente
                WindowState = FormWindowState.Maximized; // maximizam
            }
        }

        private void Btnminimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized; //minimizam = aplicatia ramane deschisa in bara
        }

        //butoane pt a inchide, a porni si a pune pe pauza o melodie
        private void Btnstop_Click(object sender, EventArgs e)
        {
            bunifuProgressBar1.Value = 0;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
            axWindowsMediaPlayer1.Ctlcontrols.pause();
            timer1.Stop();
            timer1.Interval = 100;
            timer2.Stop();
            timer2.Interval = 100;
            bunifuCustomLabel3.SetBounds(12, 45, 1, 1);
            x1 = 12;
        }

        public bool Esteredata
        {
            get
            {
                return _redata;
            }
            set
            {
                _redata = value;
                if (_redata)
                {
                    axWindowsMediaPlayer1.Ctlcontrols.pause();
                    actionbtn.Image = reda.Image;
                }
                else
                {
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    actionbtn.Image = pauza.Image;
                }
            }
        }

        private void Actionbtn_Click(object sender, EventArgs e)
        {
            Esteredata = !Esteredata;
            if (onActon != null)
            {
                onActon.Invoke(this, e);
            }
        }

        private void BunifuCustomLabel1_Click(object sender, EventArgs e) { }

        private void Btnsong_Click(object sender, EventArgs e)
        {
            listBox1.BringToFront(); //la apasarea pe "Lista de redare" se aduce in prim-plan lista de cantece
        }

        private void Btnnowplay_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.BringToFront(); //la apasarea pe "Redata acum" se aduce in prim-plan WMPlayer-ul
        }

        private void BunifuSlider1_ValueChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = bunifuSlider1.Value; //bara de volum schimba volumul aplicatiei windows music player
            bunifuCustomLabel4.Text = bunifuSlider1.Value.ToString(); //preia valoarea volumului (0-100) in campul de langa bara de volum
            if (bunifuSlider1.Value == 0)
                bunifuImageButton4.Image = bunifuImageButton1.Image;
            else
                bunifuImageButton4.Image = bunifuImageButton2.Image;
        }

        private void Reda_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void Pauza_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timp1.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
            timp2.Text = axWindowsMediaPlayer1.Ctlcontrols.currentItem.durationString.ToString();
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
                bunifuProgressBar1.Value = (int)axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
        }

        private void Inapoi10_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused && axWindowsMediaPlayer1.Ctlcontrols.currentPosition - 10 > 0)
            {
                bunifuProgressBar1.Value -= 10;
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition -= 10;
            }
        }

        private void Inainte10_Click(object sender, EventArgs e)
        {
            if ((axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying || axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused) && axWindowsMediaPlayer1.Ctlcontrols.currentPosition + 10 < axWindowsMediaPlayer1.Ctlcontrols.currentItem.duration)
            {
                bunifuProgressBar1.Value += 10;
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition += 10;
            }
        }

        public void AxWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                bunifuProgressBar1.Maximum_Value = (int)axWindowsMediaPlayer1.Ctlcontrols.currentItem.duration;
                timer1.Start();
                timer2.Start();
            }
            else if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                timer1.Stop();
            }
            else if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                timer1.Stop();
                timer2.Stop();
                bunifuCustomLabel3.SetBounds(12, 45, 1, 1);
                x1 = 12;
                bunifuProgressBar1.Value = 0;
                //Daca aceasta conditie este adevarata, atunci va fi redat urmatorul fisier din lista de redare

                if (e.newState == 1 && listBox1.SelectedIndex != listBox1.Items.Count - 1)
                {
                    BeginInvoke(new Action(() => { StartIndex = ++listBox1.SelectedIndex; }));
                }
            }
        }

        // buton de mut 
        public bool Estemuta
        {
            get
            {
                return _muta;
            }
            set
            {
                _muta = value;
                if (_muta)
                {
                    axWindowsMediaPlayer1.settings.volume = 0;
                    bunifuImageButton4.Image = bunifuImageButton1.Image;
                }
                else
                {
                    axWindowsMediaPlayer1.settings.volume = bunifuSlider1.Value;
                    bunifuImageButton4.Image = bunifuImageButton2.Image;
                }
            }
        }

        private void BunifuImageButton4_Click(object sender, EventArgs e)
        {
            Estemuta = !Estemuta;
            if (onActon2 != null)
            {
                onActon2.Invoke(this, e);
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartIndex = listBox1.SelectedIndex;
            PlayFile(StartIndex);
            bunifuCustomLabel3.Text = NumeFila[StartIndex];
        }
        public void PlayFile(int playlistindex)
        {
            if (listBox1.Items.Count <= 0)
                return;
            if (playlistindex < 0)
                return;
            axWindowsMediaPlayer1.settings.autoStart = true;
            axWindowsMediaPlayer1.URL = CaleFila[playlistindex];
            axWindowsMediaPlayer1.Ctlcontrols.next();
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void Btnbrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnFileDlg = new OpenFileDialog
            {
                Multiselect = true,
                Filter = ".mp3 |*.mp3|.mp4 |*.mp4|.wav |*.wav|.m4a |*.m4a|.mov |*.mov|.wmv |*.wmv|.mpg |*.mpg|.avi |*.avi|.3gp |*.3gp|All files (*.*)|*.*"
            };

            if (opnFileDlg.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                NumeFila = opnFileDlg.SafeFileNames;
                CaleFila = opnFileDlg.FileNames;
                for (int i = 0; i < NumeFila.Length; i++)
                {
                    string nou = NumeFila[i];
                    nou = nou.Remove(nou.Length - 4);
                    NumeFila[i] = nou;
                    if (i + 1 < 10)
                        listBox1.Items.Add("   " + (i + 1) + ".     " + nou);
                    else if (i + 1 < 100)
                        listBox1.Items.Add(" " + (i + 1) + ".     " + nou);
                    else
                        listBox1.Items.Add((i + 1) + ".     " + nou);
                }

                ///activam comenzile
                inapoi10.Enabled = true;
                repeat.Enabled = true;
                btnprev.Enabled = true;
                actionbtn.Enabled = true;
                btnnext.Enabled = true;
                inainte10.Enabled = true;
                shufflebtn.Enabled = true;
                btnstop.Enabled = true;
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e) { }

        private void BunifuProgressBar1_progressChanged(object sender, EventArgs e) { }

        private void BunifuFlatButton1_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void BunifuImageButton6_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BunifuImageButton3_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) //daca este maximizata
                this.WindowState = FormWindowState.Normal; //se face fereastra la normal la apasarea butonului
            else //daca nu este maximizata fereastra
            {
                MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea; //limita de marire pana la bara de instrumente
                WindowState = FormWindowState.Maximized; //maximizam
            }
        }

        private void BunifuImageButton5_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized; //minimizam = aplicatia ramane deschisa in bara
        }

        private void Panel2_Paint(object sender, PaintEventArgs e) { }

        private void BunifuCustomTextbox1_TextChanged(object sender, EventArgs e) { }

        //buton pt a porni melodia anterioara
        private void Btnprev_Click(object sender, EventArgs e)
        {
            if (StartIndex > 0)
                StartIndex--;
            listBox1.SelectedIndex = StartIndex;
            bunifuCustomLabel3.Text = NumeFila[StartIndex];
            PlayFile(StartIndex);
        }

        //buton pt a porni melodia urmatoare
        private void Btnnext_Click(object sender, EventArgs e)
        {
            if (StartIndex == listBox1.Items.Count - 1)
                StartIndex = listBox1.Items.Count - 1;
            else
                if (StartIndex < listBox1.Items.Count)
                StartIndex++;

            listBox1.SelectedIndex = StartIndex;
            bunifuCustomLabel3.Text = NumeFila[StartIndex];
            PlayFile(StartIndex);
        }

        private void BunifuCustomLabel1_Click_1(object sender, EventArgs e) { }

        private void Shufflebtn_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int x = r.Next(0, listBox1.Items.Count);
            StartIndex = x;
            listBox1.SelectedIndex = StartIndex;
            bunifuCustomLabel3.Text = NumeFila[StartIndex];
            PlayFile(StartIndex);
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            bunifuCustomLabel3.SetBounds(x1, 45, 1, 1);
            x1 += 5;
            if (x1 >= this.Width - bunifuCustomLabel3.Width - 12)
                x1 = 12;
        }

        //butoane pt a repeta melodia redata sau nu
        public bool Esterepetata
        {
            get
            {
                return _repetata;
            }
            set
            {
                _repetata = value;
                if (_repetata)
                {
                    axWindowsMediaPlayer1.settings.setMode("loop", true);
                    repeat.Image = repeat_on.Image;
                }
                else
                {

                    axWindowsMediaPlayer1.settings.setMode("loop", false);
                    repeat.Image = repeat_off.Image;
                }
            }
        }

        private void Repeat_Click(object sender, EventArgs e)
        {
            Esterepetata = !Esterepetata;
            if (onrepeat != null)
            {
                onrepeat.Invoke(this, e);
            }
        }
    }
}