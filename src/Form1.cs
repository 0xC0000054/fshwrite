// Copyright 2009, 2022 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace fshwrite
{
    public partial class Form1 : Form
    {
        internal Bitmap color = null;
        internal Bitmap alpha = null;
        internal bool hasSSE2 = true;
        internal string fshname = null;
        internal string outputdir = null;
        private string dirname = null;
        private int type = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bmpbox.Text = openFileDialog1.FileName;
                color = new Bitmap(openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Builds the alpha channel bitmap if it does not exist
        /// </summary>
        internal void PrepareAlpha()
        {
            if (color != null && alpha == null)
            {
                alpha = new Bitmap(color.Width, color.Height);

                for (int y = 0; y < alpha.Height; y++)
                {
                    for (int x = 0; x < alpha.Width; x++)
                    {
                        alpha.SetPixel(x, y, Color.White);
                    }
                }
            }
        }

        internal Bitmap BlendBmp(Bitmap colorbmp, Bitmap bmpalpha)
        {
            Bitmap image = null;
            if (colorbmp != null && bmpalpha != null)
            {
                image = new Bitmap(colorbmp.Width, colorbmp.Height, PixelFormat.Format32bppArgb);
            }
            if (colorbmp.Size != bmpalpha.Size)
            {
                throw new ArgumentException("The bitmap and alpha must be equal size");
            }
            BitmapData colordata = colorbmp.LockBits(new Rectangle(0, 0, colorbmp.Width, colorbmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData alphadata = bmpalpha.LockBits(new Rectangle(0, 0, bmpalpha.Width, bmpalpha.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bdata = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr scan0 = bdata.Scan0;
            unsafe
            {
                byte* clrdata = (byte*)(void*)colordata.Scan0;
                byte* aldata = (byte*)(void*)alphadata.Scan0;
                byte* destdata = (byte*)(void*)scan0;
                int offset = bdata.Stride - image.Width * 4;
                int clroffset = colordata.Stride - image.Width * 4;
                int aloffset = alphadata.Stride - image.Width * 4;
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        destdata[3] = aldata[0];
                        destdata[0] = clrdata[0];
                        destdata[1] = clrdata[1];
                        destdata[2] = clrdata[2];


                        destdata += 4;
                        clrdata += 4;
                        aldata += 4;
                    }
                    destdata += offset;
                    clrdata += clroffset;
                    aldata += aloffset;
                }

            }
            colorbmp.UnlockBits(colordata);
            bmpalpha.UnlockBits(alphadata);
            image.UnlockBits(bdata);
            return image;
        }

        /// <summary>
        /// The function that writes the fsh
        /// </summary>
        /// <param name="code">The bitmap type in the fsh entry header </param>
        /// <param name="output">The output file to write to</param>
        private unsafe void WriteFsh(int code, Stream output)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //write header
                ms.Write(Encoding.ASCII.GetBytes("SHPI"), 0, 4);
                ms.Write(BitConverter.GetBytes(0), 0, 4); // placeholder for the length
                ms.Write(BitConverter.GetBytes(1), 0, 4); // one image in the fsh
                ms.Write(Encoding.ASCII.GetBytes("G264"), 0, 4);
                //write directory
                ms.Write(Encoding.ASCII.GetBytes(dirname), 0, 4); // directory id
                ms.Write(BitConverter.GetBytes(((int)ms.Position + 4)), 0, 4); // Write the Entry offset -- 24
                // write entry header
                ms.Write(BitConverter.GetBytes(code), 0, 4); // write the Entry bitmap code
                ms.Write(BitConverter.GetBytes((short)color.Width), 0, 2);
                ms.Write(BitConverter.GetBytes((short)color.Height), 0, 2);

                for (int m = 0; m < 4; m++)
                {
                    ms.Write(BitConverter.GetBytes((short)0), 0, 2);// write misc data
                }

                if (code == 0x7F) // 24-bit RGB
                {
                    byte[] px = new byte[3];
                    BitmapData d = color.LockBits(new Rectangle(0, 0, color.Width, color.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                    for (int y = 0; y < color.Height; y++)
                    {
                        byte* p = (byte*)d.Scan0 + (y * color.Width * 3);
                        for (int x = 0; x < color.Width; x++)
                        {
                            px[0] = p[0];
                            px[1] = p[1];
                            px[2] = p[2];
                            ms.Write(px, 0, 3);
                            p += 3;
                        }
                    }

                    color.UnlockBits(d);
                }
                else if (code == 0x7D) // 32-bit RGBA
                {
                    if (alpha == null)
                    {
                        PrepareAlpha();
                    }

                    byte[] px = new byte[4];
                    BitmapData d = color.LockBits(new Rectangle(0, 0, color.Width, color.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    BitmapData al = alpha.LockBits(new Rectangle(0, 0, color.Width, color.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    for (int y = 0; y < color.Height; y++)
                    {
                        byte* p = (byte*)d.Scan0 + (y * color.Width * 4);
                        byte* a = (byte*)al.Scan0 + (y * alpha.Width * 4);

                        for (int x = 0; x < color.Width; x++)
                        {
                            px[0] = p[0];
                            px[1] = p[1];
                            px[2] = p[2];
                            px[3] = a[0];
                            ms.Write(px, 0, 4);
                            p += 4;
                            a += 4;
                        }
                    }

                    color.UnlockBits(d);
                    alpha.UnlockBits(al);
                }
                else if (code == 0x60) //DXT1
                {
                    if (alpha == null)
                    {
                        PrepareAlpha();
                    }

                    Bitmap temp = BlendBmp(color, alpha);

                    int flags = (int)SquishCompFlags.kDxt1;
                    flags |= (int)SquishCompFlags.kColourIterativeClusterFit;

                    byte[] data = Squish.CompressImage(temp, flags);

                    ms.Write(data, 0, data.Length);
                }
                else if (code == 0x61) // DXT3
                {
                    if (alpha == null)
                    {
                        PrepareAlpha();
                    }

                    Bitmap temp = BlendBmp(color, alpha);

                    int flags = (int)SquishCompFlags.kDxt3;
                    flags |= (int)SquishCompFlags.kColourIterativeClusterFit;

                    byte[] data = Squish.CompressImage(temp, flags);

                    ms.Write(data, 0, data.Length);
                }

                ms.Position = 4L;
                ms.Write(BitConverter.GetBytes((int)ms.Length), 0, 4); // write the files length
                ms.WriteTo(output); // write the memory stream to the file
            }
        }

        internal void Writebtn_Click(object sender, EventArgs e)
        {
            if (color != null)
            {
                FileStream fs = null;
                try
                {
                    string dir;
                    if (!string.IsNullOrEmpty(outputdir) && Directory.Exists(outputdir))
                    {
                        dir = outputdir;
                    }
                    else
                    {
                        dir = Path.GetDirectoryName(bmpbox.Text);
                    }
                    string name = GetFshName();

                    string fn = Path.Combine(dir, name);
                    fs = new FileStream(fn, FileMode.Create, FileAccess.Write);
                    WriteFsh(type, fs);
                    fs.Close();
                    fs = null;
                }
                catch (Exception ex)
                {
                    IWin32Window win = this.Visible ? this : null;
                    MessageBox.Show(win, ex.Message + "\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                        fs = null;
                    }
                    bmpbox.Text = string.Empty;
                    alphabox.Text = string.Empty;
                    color = null;
                    alpha = null;
                    outputdir = string.Empty;
                    fshname = string.Empty;
                }
            }
        }

        /// <summary>
        /// Sets the name of the output fsh file for the Command line processing
        /// </summary>
        /// <returns>The name of the output fsh</returns>
        private string GetFshName()
        {
            string name = string.Empty;
            if (!string.IsNullOrEmpty(fshname))
            {
                if (fshname.EndsWith(".fsh", StringComparison.OrdinalIgnoreCase))
                {
                    name = fshname;
                }
                else
                {
                    name = string.Concat(fshname, ".fsh");
                }
            }
            else
            {
                name = string.Concat(Path.GetFileNameWithoutExtension(bmpbox.Text), ".fsh");
            }
            return name;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (hasSSE2)
            {
                TypeBox1.SelectedIndex = 2;
            }
            else
            {
                TypeBox1.SelectedIndex = 0;
            }
            dirBox1.Text = "FiSH";
        }

        private void alphabtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                alpha = new Bitmap(openFileDialog1.FileName);
            }
        }

        internal void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hasSSE2)
            {
                switch (TypeBox1.SelectedIndex)
                {
                    case 0:
                        type = 0x7F;
                        break;
                    case 1:
                        type = 0x7D;
                        break;
                    case 2:
                        type = 0x60;
                        break;
                    case 3:
                        type = 0x61;
                        break;
                }
            }
            else
            {
                switch (TypeBox1.SelectedIndex)
                {
                    case 0:
                        type = 0x7F;
                        break;
                    case 1:
                        type = 0x7D;
                        break;
                }
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (dirBox1.Text.Length > 0 && dirBox1.Text.Length == 4)
            {
                dirname = dirBox1.Text;
            }
        }

        private void outdirbtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dirdialog = new FolderBrowserDialog { ShowNewFolderButton = true };
            if (dirdialog.ShowDialog() == DialogResult.OK)
            {
                outputdir = dirdialog.SelectedPath;
            }
        }
    }
}
