using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace rF2ResultsToCsv
{
    public partial class Form1 : Form
    {
        string GetFilePath ()
        {
            string fp_ResultsFile;
            OpenFileDialog openFileDialog_ResultsFiles = new OpenFileDialog();
            // Set filter options and filter index.
            openFileDialog_ResultsFiles.Filter = "rF2 results file (.xml)|*.xml|All Files (*.*)|*.*";
            openFileDialog_ResultsFiles.FilterIndex = 1;
            openFileDialog_ResultsFiles.Multiselect = false;

            if (openFileDialog_ResultsFiles.ShowDialog() == DialogResult.OK)
            {
                fp_ResultsFile = openFileDialog_ResultsFiles.FileName;
            }
            else
            {
                fp_ResultsFile = "Error";
            }
            MessageBox.Show(fp_ResultsFile);
            return fp_ResultsFile;
        }
  

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmlinput = new XmlDocument();
            string filepath = GetFilePath();// @"G:\rFactor2_VEC\UserData\Log\Results\2015_10_31_19_14_20-74Q1.xml";
            xmlinput.Load(filepath);
            if (xmlinput.HasChildNodes)
            {
                MessageBox.Show("xmpinput ist nicht leer");
            }
            XmlNodeList xmlnode = xmlinput.DocumentElement.SelectNodes("//Driver");
            if (xmlnode.Count > 0)
            {
                MessageBox.Show("xmlnode ist nicht leer: " + xmlnode.Count);
            }
            string driver_name = "";
            foreach (XmlNode node in xmlnode)
            {
                driver_name = node.SelectSingleNode("Name").InnerText;
                MessageBox.Show(driver_name);
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string fp_resultsfile;
            OpenFileDialog openFileDialog_ResultsFiles = new OpenFileDialog();
            // Set filter options and filter index.
            openFileDialog_ResultsFiles.Filter = "rF2 results file (.xml)|*.xml|All Files (*.*)|*.*";
            openFileDialog_ResultsFiles.FilterIndex = 1;
            openFileDialog_ResultsFiles.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            if (openFileDialog_ResultsFiles.ShowDialog() == DialogResult.OK)
            {
                fp_resultsfile = openFileDialog_ResultsFiles.FileName;
            }
            else
            {
                fp_resultsfile = "null";
            }
/* return fp_resultsfile; */
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fp_ResultsFile;

            OpenFileDialog openFileDialog_ResultsFiles = new OpenFileDialog();
            // Set filter options and filter index.
            openFileDialog_ResultsFiles.Filter = "rF2 results file (.xml)|*.xml|All Files (*.*)|*.*";
            openFileDialog_ResultsFiles.FilterIndex = 1;
            openFileDialog_ResultsFiles.Multiselect = false;

            if (openFileDialog_ResultsFiles.ShowDialog() == DialogResult.OK)
            {
                fp_ResultsFile = openFileDialog_ResultsFiles.FileName;
            }
            else
            {
                fp_ResultsFile = "Error";

            }

            MessageBox.Show(fp_ResultsFile);

        }
    }
}
