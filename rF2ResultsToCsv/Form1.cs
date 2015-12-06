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

        // Function to select a file and return the filepath
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
        // Define class for holding the relevant lap data
        public class rf2_lap
        {
            public string carclass { get; set; }
            public string carnumber { get; set; }
            public string teamname { get; set; }
            public string lap_num { get; set; }
            public string lap_pos { get; set; }
            public string lap_start { get; set; }
            public string s1 { get; set; }
            public string s2 { get; set; }
            public string s3 { get; set; }
            public string fuel { get; set; }
            public string tires_front { get; set; }
            public string tires_rear { get; set; }
            public string pit { get; set; }
            public string laptime { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        // Currently named "Drivers"
        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmlinput = new XmlDocument();
            string filepath = GetFilePath();// @"G:\rFactor2_VEC\UserData\Log\Results\2015_10_31_19_14_20-74Q1.xml";
            xmlinput.Load(filepath);
            if (xmlinput.HasChildNodes)
            {
                MessageBox.Show("Selected resultsfile is not empty and has been loaded.");
            }
            XmlNodeList xmlnode = xmlinput.DocumentElement.SelectNodes("//Driver");
            if (xmlnode.Count > 0)
            {
                MessageBox.Show("Drivers list is not empty. " + xmlnode.Count + " drivers have been found in the results file.");
            }
            // Create list to dump lap data into
            List<rf2_lap> laps_list = new List<rf2_lap>();



            foreach (XmlNode node in xmlnode)
            {
                // Find and set driver name
                string driver_name = "";
                driver_name = node.SelectSingleNode("Name").InnerText;


                // Select all laps for a given driver.
                XmlNodeList laps = xmlinput.DocumentElement.SelectNodes("//Driver[Name=\"" + driver_name + "\"]/Lap");

                // Debugging outputs
                if (laps.Count > 0)
                {
                    MessageBox.Show("Number of laps found for driver " + driver_name + ": " + laps.Count);
                }
                // Add laps to list
                if (driver_name == "Dennis Grebe")
                {

                    foreach (XmlNode lapnode in laps)
                    {
                        // If all sectors exist in results file load complete lap data - currently restricted to laps which have s1 to s3 defined as those elements are missing entirely if not recorded
                        if (lapnode.Attributes["s1"] != null && lapnode.Attributes["s2"] != null && lapnode.Attributes["s3"] != null)
                        {
                            laps_list.Add(new rf2_lap
                            {
                                carclass = node.SelectSingleNode("CarClass").InnerText ?? "N/A",
                                carnumber = node.SelectSingleNode("CarNumber").InnerText ?? "N/A",
                                teamname = node.SelectSingleNode("TeamName").InnerText ?? "N/A",
                                lap_num = lapnode.Attributes["num"].Value ?? "N/A",
                                lap_pos = lapnode.Attributes["p"].Value ?? "N/A",
                                lap_start = lapnode.Attributes["et"].Value ?? "N/A",
                                s1 = lapnode.Attributes["s1"].Value ?? "N/A",
                                s2 = lapnode.Attributes["s2"].Value ?? "N/A",
                                s3 = lapnode.Attributes["s3"].Value ?? "N/A",
                                fuel = lapnode.Attributes["fuel"].Value ?? "N/A",
                                tires_front = lapnode.Attributes["fcompound"].Value ?? "N/A",
                                tires_rear = lapnode.Attributes["rcompound"].Value ?? "N/A",
                                pit = "1", //lapnode.Attributes["pit"].Value,
                                laptime = lapnode.InnerText ?? "N/A",
                            }
                            );
                        }
                    }                    

                    rf2_lap output = laps_list[1];
                    MessageBox.Show("Carclass: " + output.carclass + Environment.NewLine + "CarNumber: " + output.carnumber + Environment.NewLine + "TeamName: " + output.teamname + Environment.NewLine + "Lap number: " + output.lap_num + Environment.NewLine
                        + "Lap Pos: " + output.lap_pos + Environment.NewLine + "Lap start: " + output.lap_start + Environment.NewLine + "Sector 1: " + output.s1 + Environment.NewLine
                        + "Sector 2: " + output.s2 + Environment.NewLine + "Sector 3: " + output.s3 + Environment.NewLine + "Fuel: " + output.fuel + Environment.NewLine
                        + "Tires Front: " + output.tires_front + Environment.NewLine + "Tires Rear: " + output.tires_rear + Environment.NewLine + "Laptime : " + output.laptime);
                    
                }
                //MessageBox.Show(driver_name);
            }
           // string[,] array = new string[,];
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

        // Load results file button at the top
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
