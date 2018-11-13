using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertificateCollectorForm
{
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            DirectoryInfo d = new DirectoryInfo(this.textBox1.Text);

            var list = new Dictionary<string,string>();
            foreach (FileInfo file in d.GetFiles("*.dll"))
            {
                Assembly asm = Assembly.LoadFile(file.FullName);
                Module m = asm.GetModules()[0];
                var cert = m.GetSignerCertificate();
                if (cert != null)
                {
                    var cert2 = new X509Certificate2(cert);
                    if (cert2.NotAfter <= DateTime.Now)
                    {
                        list.Add(file.Name, "Lejárt: "+cert.GetExpirationDateString());
                    }
                }
                else
                {
                    list.Add(file.Name, "NINCS ALÁÍRVA");
                }

            }

            foreach (var key in list)
            {
                this.listBox1.Items.Add(key);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
        }
    }
}
