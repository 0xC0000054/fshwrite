// Copyright 2009, 2022 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace fshwrite
{
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsProcessorFeaturePresent(uint processorFeature);

        private const uint SSE2 = 10; // check if SSE2 is available for Squish

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                using (Form1 form = new Form1())
                {
                    if (!IsProcessorFeaturePresent(SSE2))
                    {
                        form.TypeBox1.Items.RemoveAt(3);
                        form.TypeBox1.Items.RemoveAt(2);
                        form.hasSSE2 = false;
                    }

                    if (args.Length > 0)
                    {
                        int count = -1;
                        for (int i = 0; i < args.Length; i++)
                        {
                            string arg = args[i];

                            if (arg.StartsWith("/b:") || arg.StartsWith("/a:") || arg.StartsWith("/outdir:") || arg.StartsWith("/fshname:"))
                            {
                                count++;
                            }
                        }

                        string rootdir = Path.GetDirectoryName(typeof(Program).Assembly.Location);

                        for (int i = 0; i < args.Length; i++)
                        {
                            string arg = args[i];

                            if (arg.StartsWith("/b:") || arg.StartsWith("/a:") || arg.StartsWith("/outdir:") || arg.StartsWith("/fshname:"))
                            {
                                if (arg.StartsWith("/b:") || arg.StartsWith("/a:"))
                                {
                                    string split = arg.Substring(3, (arg.Length - 3));

                                    if (!string.IsNullOrEmpty(split))
                                    {
                                        if (!Path.IsPathRooted(split))
                                        {
                                            split = Path.Combine(rootdir, split);
                                        }

                                        FileInfo fi = new FileInfo(split);
                                        if (fi.Exists)
                                        {
                                            if (fi.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || fi.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                                            {
                                                if (arg.StartsWith("/b:", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    form.color = new Bitmap(fi.FullName);
                                                    form.bmpbox.Text = fi.FullName;
                                                    string alpath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.FullName) + "_a" + fi.Extension);
                                                    if (File.Exists(alpath))
                                                    {
                                                        form.alpha = new Bitmap(alpath);
                                                    }
                                                }
                                                else if (arg.StartsWith("/a:", StringComparison.OrdinalIgnoreCase))
                                                {
                                                    form.alpha = new Bitmap(fi.FullName);
                                                    form.alphabox.Text = fi.FullName;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentException(string.Concat("The ", arg.Substring(0, 3), " path must not be empty"));
                                    }
                                }
                                else if (arg.StartsWith("/outdir:"))
                                {
                                    string dir = arg.Substring(8, (arg.Length - 8));
                                    if (!string.IsNullOrEmpty(dir))
                                    {
                                        if (Directory.Exists(dir))
                                        {
                                            form.outputdir = dir;
                                        }
                                        else
                                        {
                                            throw new DirectoryNotFoundException(string.Concat("The output dir ", dir, " could not be found"));
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentException("The /outdir: path must not be empty");
                                    }
                                }
                                else if (arg.StartsWith("/fshname:"))
                                {
                                    string name = arg.Substring(9, (arg.Length - 9));
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        form.fshname = name;
                                    }
                                }

                                if (i == count && form.color != null)
                                {
                                    if (form.bmpbox.Text.StartsWith("hd"))
                                    {
                                        if (form.alpha != null)
                                        {
                                            form.TypeBox1.SelectedIndex = 1; // 32-bit
                                        }
                                        else
                                        {
                                            form.TypeBox1.SelectedIndex = 0; //24-bit
                                        }
                                    }
                                    else
                                    {
                                        if (form.hasSSE2)
                                        {
                                            if (form.alpha != null)
                                            {
                                                form.TypeBox1.SelectedIndex = 3; //Dxt3
                                            }
                                            else
                                            {
                                                form.TypeBox1.SelectedIndex = 2; //Dxt1
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("A processor that supports SSE2 is required to save DXT1 and DXT3 fsh images");
                                        }
                                    }

                                    form.dirBox1.Text = "FiSH";
                                    form.Writebtn_Click(null, null);
                                }
                            }
                            else if (arg.StartsWith("/?"))
                            {
                                ShowHelp();
                            }
                        }
                    }
                    else
                    {
                        Application.Run(form);
                    }
                }

            }
            catch (ArgumentException ax)
            {
                MessageBox.Show(ax.Message, "fshwrite", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "fshwrite", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private static void ShowHelp()
        {
            MessageBox.Show("fshwrite /b:<Bitmap> [/a:<alpha>] [/outdir:<directory>] [/fshname:<name>] [/?]\n\n /b:<Bitmap> the input bitmap. \n /a:<alpha> the input alpha. \n /outdir:<directory> the directory to put the output files. \n /fshname:<name> the name of the output fsh. \n /? show this help. \n\n parameters in brackets are optional. \n Paths containing spaces must be encased in quotes.", "fshwrite");
        }
    }
}
