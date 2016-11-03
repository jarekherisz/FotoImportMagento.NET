using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace FotoImportMagento
{
    public partial class Config : Form
    {
        string filename = "conf.xml";

        public Config()
        {
            InitializeComponent();
            Config_Read();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.save();
        }

        private void save()
        {
            //if file is not found, create a new xml file
            XmlTextWriter xmlWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteStartElement("Import");
            //If WriteProcessingInstruction is used as above,
            //Do not use WriteEndElement() here
            //xmlWriter.WriteEndElement();
            //it will cause the &ltRoot></Root> to be &ltRoot />
            xmlWriter.Close();

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(filename);

            XmlNode Import = xmlDoc.DocumentElement;


            XmlElement magentourl = xmlDoc.CreateElement("magentourl");
            magentourl.SetAttribute("Value", textBoxMagentoUrl.Text);

            XmlElement magentouser = xmlDoc.CreateElement("magentouser");
            magentouser.SetAttribute("Value", textBoxMagentoUser.Text);

            XmlElement magentokey = xmlDoc.CreateElement("magentokey");
            magentokey.SetAttribute("Value", textBoxMagentoKey.Text);

            XmlElement smptUser = xmlDoc.CreateElement("smptUser");
            smptUser.SetAttribute("Value", textBoxSmtpUser.Text);

            XmlElement smtpPass = xmlDoc.CreateElement("smtpPass");
            smtpPass.SetAttribute("Value", textBoxSmtpPass.Text);

            XmlElement smtpHost = xmlDoc.CreateElement("smtpHost");
            smtpHost.SetAttribute("Value", textBoxSmtpHost.Text);

            XmlElement mailTo = xmlDoc.CreateElement("mailTo");
            mailTo.SetAttribute("Value", textBoxMailTo.Text);

            XmlElement mailFrom = xmlDoc.CreateElement("mailFrom");
            mailFrom.SetAttribute("Value", textBoxMailFrom.Text);

            XmlElement path = xmlDoc.CreateElement("path");
            path.SetAttribute("Value", textBoxPath.Text);

            Import.AppendChild(magentourl);
            Import.AppendChild(magentouser);
            Import.AppendChild(magentokey);
            Import.AppendChild(smptUser);
            Import.AppendChild(smtpPass);
            Import.AppendChild(smtpHost);
            Import.AppendChild(mailFrom);
            Import.AppendChild(mailTo);
            Import.AppendChild(path);

            xmlDoc.Save(filename);

            this.Hide();
        }

        private void Config_Read()
        {
            if (!System.IO.File.Exists(filename)) this.save();
            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            case "magentourl":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxMagentoUrl.Text = reader.Value;
                                break;
                            case "magentouser":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxMagentoUser.Text = reader.Value;
                                break;
                            case "magentokey":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxMagentoKey.Text = reader.Value;
                                break;
                            case "smptUser":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxSmtpUser.Text = reader.Value;
                                break;
                            case "smtpPass":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxSmtpPass.Text = reader.Value;
                                break;
                            case "smtpHost":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxSmtpHost.Text = reader.Value;
                                break;
                            case "mailTo":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxMailTo.Text = reader.Value;
                                break;
                            case "mailFrom":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxMailFrom.Text = reader.Value;
                                break;
                            case "path":
                                // Detect this element.
                                reader.MoveToAttribute(0);
                                textBoxPath.Text = reader.Value;
                                break;
                        }
                    }
                }
            }

        }

        private void Config_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void buttonPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog();
        }

        private void folderBrowserDialog()
        {
            folderBrowserDialog1.ShowDialog();
            textBoxPath.Text = folderBrowserDialog1.SelectedPath;
        }


        private void textBoxPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog();
        }

    }
}
