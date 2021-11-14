using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelColor
{
    public partial class PixelColor : Form
    {
        public PixelColor()
        { InitializeComponent(); }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        public Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        { this.KeyPreview = true; }

        //color tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            Point cursor = new Point();
            GetCursorPos(ref cursor);

            var c = GetColorAt(cursor);
            pictureBox1.BackColor = c;
            ButtonHEX.Text = String.Format("#{0:X6}", c.ToArgb() & 0x00FFFFFF);
        }

        private void picture_exit_Click(object sender, EventArgs e)
        { Application.Exit(); }

        //form control
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.B))
            { timer1.Stop(); Clipboard.SetText(ButtonHEX.Text); timer1.Start(); }
        }

        //Bring
        private void picture_settings_Click(object sender, EventArgs e)
        { panel1.BringToFront(); }
        private void picture_apply_Click(object sender, EventArgs e)
        { pictureBox1.BringToFront(); }

        //Topmost
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            { this.TopMost = false; }
            else
            { this.TopMost = true; }
        }
        //Colors
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                this.BackColor = Color.FromArgb(245, 246, 250);
                panel1.BackColor = Color.FromArgb(220, 221, 225);
                ButtonHEX.BackColor = Color.FromArgb(220, 221, 225);
            }
            else
            {
                this.BackColor = Color.FromArgb(47, 54, 64);
                panel1.BackColor = Color.FromArgb(59, 64, 71);
                ButtonHEX.BackColor = Color.FromArgb(59, 64, 71);
            }
        }
    }
}
