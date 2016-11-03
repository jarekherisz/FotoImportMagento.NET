using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace FotoImportMagento
{
    public partial class FotoImportMagento : Form
    {
        //Instance of class
        private static FotoImportMagento instance;

        //Instance of config windows
        public Config config;

        //Dane do raportu
        public string mailRaportBody = "";

        //Wersja demo
        public bool demo = true;

        public FotoImportMagento()
        {
            InitializeComponent();
            _ProgressBarMaximum = new DProgressBarMaximum(progressBarMaximum);
            _ProgressBarUpdate = new DProgressBarUpdate(progressBarUpdate);
            _Label1TextChande = new DLabel1TextChande(label1TextChande);
            _Label2TextChande = new DLabel2TextChande(label2TextChande);
            config = new Config();
        }

        private void FotoImportMagento_Load(object sender, EventArgs e)
        {
            if (this.demo == true)
            {
                OProgramie oprogramie = new OProgramie();
                oprogramie.Owner = this;
                oprogramie.ShowDialog();
                if (oprogramie.DialogResult != DialogResult.Yes)
                    this.Close();
            }
        }

        public static FotoImportMagento getInstance(string[] args)
        {
            if (instance == null)
            {
                instance = new FotoImportMagento();
                if (args.Count() > 0 && args[0] == "start" && instance.demo == false) instance.start();
            }
            return instance;
        }

        public static FotoImportMagento getInstance()
        {
            if (instance == null)
            {
                instance = new FotoImportMagento();
            }
            return instance;
        }

        delegate void DProgressBarMaximum(int value);
        private DProgressBarMaximum _ProgressBarMaximum;
        private void progressBarMaximum(int value)
        {
            progressBar1.Style = ProgressBarStyle.Blocks;
            this.progressBar1.Maximum = value;
        }

        delegate void DProgressBarUpdate(int value);
        private DProgressBarUpdate _ProgressBarUpdate;
        private void progressBarUpdate(int value)
        {
            this.progressBar1.Value = value;
        }

        delegate void DLabel1TextChande(string value);
        private DLabel1TextChande _Label1TextChande;
        private void label1TextChande(string value)
        {
            this.label1.Text = value;
        }

        delegate void DLabel2TextChande(string value);
        private DLabel2TextChande _Label2TextChande;
        private void label2TextChande(string value)
        {
            this.label2.Text = value;
        }

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            config.Show();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.start();
        }

        private void start()
        {
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            progressBar1.Style = ProgressBarStyle.Marquee;
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BeginInvoke(_Label1TextChande, new object[] { "Wczytuje pliki" });
            string[] tmpFilePaths = Directory.GetFiles(this.config.textBoxPath.Text);
            //////////////////////////////////////////////////////
            //Wczytuje wszystkie pliki                          //
            //////////////////////////////////////////////////////
            foreach(string filePath in tmpFilePaths)
            {
                //Buduje baze plikow
                try
                {
                    string fileName = filePath.Split('\\')[filePath.Split('\\').Length - 1];
                    string id = fileName.Split('_')[0];
                    string poss = fileName.Split('_')[1].Split('.')[0];
                    string typ = fileName.Split('_')[1].Split('.')[1];
                    if (MagentoProducts.findMagentoProduct(id) == null)
                    {
                        MagentoProducts.addMagentoProduct(new MagentoProduct(id));
                    }
                    MagentoProducts.findMagentoProduct(id).addPic(filePath, poss);
                }
                catch
                {
                    //Tu informacja do logu ze plik ma nieprawidlowa nazwe
                    mailRaportBody += "Błąd plik " + filePath + " ma nieprawidłową nazwe <br />";
                }

                

            }
            //////////////////////////////////////////////////////
            //Łączenie z sklepem                                //
            //////////////////////////////////////////////////////
            BeginInvoke(_Label1TextChande, new object[] { "Łączenie z sklepem" });
            MyMagentoAPI magentoApi = new MyMagentoAPI();
            magentoApi.Connect();
            //////////////////////////////////////////////////////
            //Rozpoczynam wysyłanie plików                      //
            //////////////////////////////////////////////////////
            int i = 1;
            int progres = 0;
            BeginInvoke(_ProgressBarMaximum, new object[] { MagentoProducts.magentoProducts.Count });
            //Przechodzę po produktach
            foreach (MagentoProduct magentoProduct in MagentoProducts.magentoProducts)
            {
                BeginInvoke(_Label1TextChande, new object[] { "Import dla produktu : "+magentoProduct.id });
                BeginInvoke(_ProgressBarUpdate, new object[] { progres });
                //Usuwam stare zdjecia
                BeginInvoke(_Label2TextChande, new object[] { "Kasowanie starych" });
                magentoApi.delAllImage(magentoProduct.id);
                //Raportuje stan 
                mailRaportBody += i+"\tRozpoczynam import dla sku : " + magentoProduct.id + "<br />";
                i++;
                foreach (MagentoPic picture in magentoProduct.picture)
                {
                    //Raportuje stan
                    mailRaportBody += i + "\tRozpoczynam import zdjecia : " + picture.path + "<br />";
                    BeginInvoke(_Label2TextChande, new object[] { picture.path });
                    magentoApi.addNewImage(magentoProduct.id ,picture);
                    i++;
                }
                progres++;
            }
        }



        private void sendRaport()
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                NetworkCredential basicCredential =
                    new NetworkCredential(config.textBoxSmtpUser.Text, config.textBoxSmtpPass.Text);
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress(config.textBoxMailFrom.Text);

                smtpClient.Host = config.textBoxSmtpHost.Text;
                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = basicCredential;

                message.From = fromAddress;
                message.Subject = "Raport wykonania importu do magento";
                //Set IsBodyHtml to true means you can send HTML email.
                message.IsBodyHtml = true;
                message.Body = mailRaportBody;
                message.To.Add(config.textBoxMailTo.Text);
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                //Error, could not send the message
                MessageBox.Show("Błąd wysyłki raportu : "+ex.Message);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.buttonStop.Enabled = false;
            this.buttonStart.Enabled = true;
            progressBar1.Value = 0;
            this.label1.Text = "";
            this.label2.Text = "";
            sendRaport();
        }

        private void FotoImportMagento_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker.Dispose();
            config.Close();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.buttonStop.Enabled = false;
            this.label1.Text = "Kończenie zadania. Czekaj.";
            backgroundWorker.CancelAsync();
        }

        private void oProgramieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OProgramie oprogramie = new OProgramie();
            oprogramie.Show();
        }

        private void pomocToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pomoc pomoc = new Pomoc();
            pomoc.Show();
        }
    }
}
