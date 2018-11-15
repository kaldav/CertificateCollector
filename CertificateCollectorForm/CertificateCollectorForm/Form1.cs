using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace CertificateCollectorForm
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.listBox1.DataSource = this.assemblies;
        }

        private List<string> assemblies = new List<string>();

        private void button1_Click(object sender, EventArgs e)
        {
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.progressBar1.MarqueeAnimationSpeed = 100;
            this.button1.Enabled = false;
            this.assemblies.Clear();
            this.listBox1.DataSource = null;

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }


        void DoWork(object sender, DoWorkEventArgs e)
        {
            var dirs = Directory.GetDirectories(this.textBox1.Text, "*", SearchOption.AllDirectories).ToList();
            dirs.Add(this.textBox1.Text);
            foreach (var dir in dirs)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                foreach (FileInfo file in directoryInfo.GetFiles("*.dll"))
                {
                    int index = file.FullName.IndexOf(this.textBox1.Text);
                    string simplifiedFileName = (index < 0)
                                           ? "." + file.FullName
                                           : "." + file.FullName.Remove(index, this.textBox1.Text.Length);
                    Assembly assembly;
                    try
                    {
                        assembly = Assembly.LoadFile(file.FullName);
                    }
                    catch
                    {
                        this.assemblies.Add(simplifiedFileName + ": The file is blocked.");
                        continue;
                    }

                    Module module = assembly.GetModules()[0];
                    var certificate = module.GetSignerCertificate();
                    if (certificate == null)
                    {
                        this.assemblies.Add(simplifiedFileName + ": No signature found.");
                    }
                    else
                    {
                        //var cert2 = new X509Certificate2(cert);
                        //if (cert2.NotAfter <= DateTime.Now)
                        //{
                        //    list.Add(file.Name, "Lejárt: "+cert.GetExpirationDateString());
                        //}
                    }
                }
            }
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar1.MarqueeAnimationSpeed = 0;
            this.progressBar1.Style = ProgressBarStyle.Blocks;
            this.progressBar1.Value = progressBar1.Minimum;

            this.button1.Enabled = true;
            this.listBox1.DataSource = null;
            this.listBox1.DataSource = this.assemblies;
            MessageBox.Show("Folder analyzed successfully.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.IO.StreamWriter SaveFile = new System.IO.StreamWriter(this.textBox2.Text);
            foreach (var item in listBox1.Items)
            {
                SaveFile.WriteLine(item.ToString());
            }
            SaveFile.Close();

            MessageBox.Show("Exported successfully to: "+ this.textBox2.Text);
        }
        

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = this.saveFileDialog1.FileName;
            }
        }
    }
}
