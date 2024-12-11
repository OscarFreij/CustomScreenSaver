using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomScreenSaver
{
    public partial class ScreenSaverForm : Form
    {
        private List<string> imagePaths = new List<string>();
        private bool previewMode = false;

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        public ScreenSaverForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();

            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);

            // Make text smaller
            //textLabel.Font = new System.Drawing.Font("Arial", 6);

            previewMode = true;

            this.KeyPreview = true;
        }

        public ScreenSaverForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
            this.KeyPreview = true;
        }



        private Random rand = new Random();

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            // Use the string from the Registry if it exists
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\CustomScreenSaver");

            if (System.IO.Directory.Exists((string)key.GetValue("path")))
            {
                foreach (string path in System.IO.Directory.GetFiles((string)key.GetValue("path")))
                {
                    imagePaths.Add(path);
                }
            }

            if (key == null)
            {
                this.pictureBox1.Visible = false;
            }
            else
            {
                switch (key.GetValue("mode"))
                {
                    case "Normal":
                        this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                        break;

                    case "StretchImage":
                        this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        break;

                    case "AutoSize":
                        this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                        break;

                    case "CenterImage":
                        this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                        break;

                    case "Zoom":
                        this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        break;
                    default:
                        this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                        break;
                }

                if (this.imagePaths.Count > 0)
                {
                    if (!System.IO.File.Exists(this.imagePaths[0]))
                    {
                        this.pictureBox1.Visible = false;
                        this.textLabel.Text = $"invalid file: {this.imagePaths[0]}";
                    }
                    else
                    {
                        this.pictureBox1.Visible = true;
                        this.pictureBox1.Load(this.imagePaths[0]);
                        this.pictureBox1.BackColor = getAverageColor(new Bitmap(this.pictureBox1.Image));
                        this.imagePaths.Add(this.imagePaths[0]);
                        this.imagePaths.RemoveAt(0);
                    }
                }
            }

            Cursor.Hide();
            TopMost = true;

            moveTimer.Interval = 8000;
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            moveTimer.Start();
        }

        private void moveTimer_Tick(object sender, System.EventArgs e)
        {
            if (this.imagePaths.Count > 0)
            {
                if (!System.IO.File.Exists(this.imagePaths[0]))
                {
                    this.pictureBox1.Visible = false;
                }
                else
                {
                    this.pictureBox1.Visible = true;
                    this.pictureBox1.Load(this.imagePaths[0]);
                    this.pictureBox1.BackColor = getAverageColor(new Bitmap(this.pictureBox1.Image));
                    this.imagePaths.Add(this.imagePaths[0]);
                    this.imagePaths.RemoveAt(0);
                    
                }
            }
        }

        private Point mouseLocation;

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                if (!mouseLocation.IsEmpty)
                {
                    // Terminate if mouse is moved a significant distance
                    if (Math.Abs(mouseLocation.X - e.X) > 5 ||
                        Math.Abs(mouseLocation.Y - e.Y) > 5)
                        Application.Exit();
                }
            }
                
            // Update current mouse location
            mouseLocation = e.Location;
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                Application.Exit();
            }
        }

        private void ScreenSaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
            {
                Application.Exit();
            }
        }

        private static Color getAverageColor(Bitmap bmp)
        {

            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }
    }
}
