﻿using System;
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
    public partial class rF2ResultsToCsv : Form
    {
        // "Global" Variables needed in both button methods
        string output_fpath;
        StringBuilder csv = new StringBuilder();
        int use_period_in_laptime;

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
            return fp_ResultsFile;
        }
        string SaveFilePath()
        {
            string fp_SaveFile;
            SaveFileDialog saveFileDialog_SaveFile = new SaveFileDialog();
            // Set filter options and filter index.
            saveFileDialog_SaveFile.Filter = "Comma-separated file (.csv)|*.csv|All Files (*.*)|*.*";
            saveFileDialog_SaveFile.Title = "Save results to .csv-file:";
            if (saveFileDialog_SaveFile.ShowDialog() == DialogResult.OK)
            {
                fp_SaveFile = saveFileDialog_SaveFile.FileName;
            }
            else
            {
                fp_SaveFile = "Error";
            }
            return fp_SaveFile;
        }
        // Define class for holding the relevant lap data
        public class rf2_lap
        {
            public string driver { get; set; }
            public string cartype { get; set; }
            public string carclass { get; set; }
            public string carnumber { get; set; }
            public string teamname { get; set; }
            public int lap_num { get; set; }
            public string lap_pos { get; set; }
            public string lap_start { get; set; }
            public string s1 { get; set; }
            public string s2 { get; set; }
            public string s3 { get; set; }
            public string fuel { get; set; }
            public string tires_front { get; set; }
            public string tires_rear { get; set; }
            public string pitin { get; set; }
            public string pitout { get; set; }
            public string laptime { get; set; }
        }
        // Define class for holding the driver swap data
        public class swap_data
        {
            public string driver { get; set; }
            public int begin { get; set; }
            public int end { get; set; }
        }

        public rF2ResultsToCsv()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        // Currently named "Open Results File"
        private void button1_Click(object sender, EventArgs e)
        {
            // declaring variables needed later
            // For retrieving data:
            string driver_name; // currently iterated over driver name
            string driver_name_org; // original driver name as defined in the results file, needs to be accessible because the main driver node is referenced by it instead of the currently in-car driver
            string driver_write; // driver name to be written to list that will be the basis for csv output
            string cartype;
            string carclass;
            string carnumber;
            string teamname;
            int lap_num;
            string lap_pos;
            string lap_start;
            string s1;
            string s2;
            string s3;
            string fuel;
            string tires_front;
            string tires_rear;
            string pitin;
            string pitout;
            string laptime;
            int swap_start;
            int swap_end;
            // Initialize pitout (will be set "passively" after lines which have pitin = 1, thus we need to initialize for all laps until the first pit stop is recorded)
            pitout = "0";

            toolStripStatusLabel1.Text = "Opening Results File";
            XmlDocument xmlinput = new XmlDocument();
            string filepath = GetFilePath();
            label1.Text = "Results File: " + filepath;

            toolStripStatusLabel1.Text = "Processing Results File";
            xmlinput.Load(filepath);
            XmlNodeList xmlnode = xmlinput.DocumentElement.SelectNodes("//Driver");
            label2.Text = "Drivers found: " + xmlnode.Count;

            // Create list to dump lap data into
            List<rf2_lap> laps_list = new List<rf2_lap>();
            // Holder for building the string
            string newline;
            toolStripStatusLabel1.Text = "Getting laps info";

            // Loop for each driver
            foreach (XmlNode node in xmlnode)
            {
                // Find and set variables that are consistent for each driver node entry (driver is an exception, will be overwritten depending on swaps list later):
                if (node.SelectSingleNode("Name").InnerText != null)
                {
                    driver_name = node.SelectSingleNode("Name").InnerText;
                }
                else
                {
                    driver_name = "[Unknown]";
                }
                driver_name_org = driver_name;
                if (node.SelectSingleNode("CarType").InnerText != null)
                {
                    cartype = node.SelectSingleNode("CarType").InnerText;
                }
                else
                {
                    cartype = "[Unknown]";
                }
                if (node.SelectSingleNode("CarClass").InnerText != null)
                {
                    carclass = node.SelectSingleNode("CarClass").InnerText;
                }
                else
                {
                    carclass = "[Unknown]";
                }
                if (node.SelectSingleNode("CarNumber") != null)
                {
                    carnumber = node.SelectSingleNode("CarNumber").InnerText;
                }
                else
                {
                    carnumber = "[Unknown]";
                }
                if (node.SelectSingleNode("TeamName") != null)
                {
                    teamname = node.SelectSingleNode("TeamName").InnerText;
                }
                else
                {
                    teamname = "[Unknown]";
                }

                // Stuff to do for each driver goes here
                List<swap_data> swap_list = new List<swap_data>();
                XmlNodeList swaps = xmlinput.DocumentElement.SelectNodes("//Driver[Name=\"" + driver_name + "\"]/Swap");
                foreach(XmlNode swapnode in swaps)
                {
                    if (swapnode.InnerText != null)
                    {
                        driver_name = swapnode.InnerText;
                    }
                    if (swapnode.Attributes["startLap"] != null)
                    {
                        swap_start = Int32.Parse(swapnode.Attributes["startLap"].InnerText);
                    }
                    else
                    {
                        swap_start = -1;
                    }
                    if (swapnode.Attributes["endLap"] != null)
                    {
                        swap_end = Int32.Parse(swapnode.Attributes["endLap"].InnerText);
                    }
                    else
                    {
                        swap_end = -1;
                    }
                    // Write to list:
                    swap_list.Add(new swap_data
                    {
                        driver = driver_name, begin = swap_start, end = swap_end
                    });
                }
                // Select all laps for a given driver.
                XmlNodeList laps = xmlinput.DocumentElement.SelectNodes("//Driver[Name=\"" + driver_name + "\"]/Lap");
                toolStripStatusLabel1.Text = "Formatting laps info";
                // Add laps to list
                foreach (XmlNode lapnode in laps)
                    {
                    // Check if the attribute exists, then set it to the value found in the results file - or set it to the default
                    if (lapnode.Attributes["num"] != null)
                    {
                        lap_num = Int32.Parse(lapnode.Attributes["num"].InnerText);
                    }
                    else
                    {
                        lap_num = -1;
                    }
                    if (lapnode.Attributes["p"] != null)
                    {
                        lap_pos = lapnode.Attributes["p"].InnerText;
                    }
                    else
                    {
                        lap_pos = "[Unknown]";
                    }
                    if (lapnode.Attributes["et"] != null)
                    {
                        lap_start = lapnode.Attributes["et"].InnerText;
                    }
                    else
                    {
                        lap_start = "[Unknown]";
                    }
                    if (lapnode.Attributes["s1"] != null)
                    {
                        s1 = lapnode.Attributes["s1"].InnerText;
                    }
                    else
                    {
                        s1 = "[Unknown]";
                    }
                    if (lapnode.Attributes["s2"] != null)
                    {
                        s2 = lapnode.Attributes["s2"].InnerText;
                    }
                    else
                    {
                        s2 = "[Unknown]";
                    }
                    if (lapnode.Attributes["s3"] != null)
                    {
                        s3 = lapnode.Attributes["s3"].InnerText;
                    }
                    else
                    {
                        s3 = "[Unknown]";
                    }
                    if (lapnode.Attributes["fuel"] != null)
                    {
                        fuel = lapnode.Attributes["fuel"].InnerText;
                    }
                    else
                    {
                        fuel = "[Unknown]";
                    }
                    if (lapnode.Attributes["fcompound"] != null)
                    {
                        tires_front = lapnode.Attributes["fcompound"].InnerText;
                    }
                    else
                    {
                        tires_front = "[Unknown]";
                    }
                    if (lapnode.Attributes["rcompound"] != null)
                    {
                        tires_rear = lapnode.Attributes["rcompound"].InnerText;
                    }
                    else
                    {
                        tires_rear = "[Unknown]";
                    }
                    if (lapnode.Attributes["pit"] != null)
                    {
                        pitin = lapnode.Attributes["pit"].InnerText;
                    }
                    else
                    {
                        pitin = "0";
                    }
                    if (lapnode.InnerText != null)
                    {
                        if (use_period_in_laptime == 0)
                        {
                            laptime = lapnode.InnerText;
                            laptime = laptime.Replace('.', ',');
                        }
                        else 
                            laptime = lapnode.InnerText;
                    }
                    else
                    {
                        if (use_period_in_laptime == 1)
                        {
                            laptime = "999.999";
                        }
                        if (use_period_in_laptime == 0)
                        {
                            laptime = "999,999";
                        }
                        else
                            laptime = "999.999";
                        
                    }

                    toolStripStatusLabel1.Text = "Getting driver swaps info";
                    // Find wanted entry from swaps list
                    swap_data wanted_swap = swap_list.Find(entry => entry.begin <= lap_num && entry.end >= lap_num);
                    if (wanted_swap != null)
                    {
                        driver_write = wanted_swap.driver;
                    }
                    else
                    {
                        driver_write = driver_name;
                    }

                    // Write the selected values to the list
                    laps_list.Add(new rf2_lap
                    {
                            driver = driver_write,      cartype = cartype,      carclass = carclass,    carnumber = carnumber,  teamname = teamname,
                            lap_num = lap_num,          lap_pos = lap_pos,      lap_start = lap_start,  s1 = s1,
                            s2 = s2,                    s3 = s3,                fuel = fuel,            tires_front = tires_front,
                            tires_rear = tires_rear,    pitin = pitin,          pitout = pitout,        laptime = laptime,
                        }       );
                    // As pitout is dependent on a previous pitin, we need to re-set it to 0 after writing it out to the lap.
                    pitout = "0";
                    // Set pitout for the next lap to 1 if current lap included entering the pitlane
                    if (pitin == "1")
                    {
                        pitout = "1";
                    }

                }
                // Stuff to do for each driver goes here
            }
            // Stuff done once after cycling through all the laps and drivers goes here
            toolStripStatusLabel1.Text = "Preparing data";
            newline = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16}", "Driver","CarType", "CarClass", "CarNumber", "TeamName", "LapNum", "LapPos", "LapStart", "Sector1", "Sector2", "Sector3", "Fuel", "TiresFront", "TiresRear", "PitIn", "PitOut", "Laptime");
            csv.AppendLine(newline);

            int num_laps; int n;
            num_laps = laps_list.Count;
            // Set the label before doing further work
            label3.Text = "Laps found: " + num_laps;
            n = 0;
            while (n < num_laps)
            {
                rf2_lap output = laps_list[n];
                newline = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16}", output.driver, output.cartype, output.carclass, output.carnumber, output.teamname, output.lap_num, output.lap_pos, output.lap_start, output.s1, output.s2, output.s3, output.fuel, output.tires_front, output.tires_rear, output.pitin, output.pitout, output.laptime);
                csv.AppendLine(newline);
                n++;
            }
            toolStripStatusLabel1.Text = "Select output file";
            output_fpath = SaveFilePath();
            label4.Text = "Output File: " + output_fpath;
            toolStripStatusLabel1.Text = "Finished processing results file, ready to write";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                toolStripStatusLabel1.Text = "Writing to file";
                File.WriteAllText(output_fpath, csv.ToString());
                toolStripStatusLabel1.Text = "Writing complete";
                MessageBox.Show("File has been written.");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Sorry, the file could not be written to. The file could already be open and can not be modified. Another possible issue is that there is not enough space left on your hard drive." + Environment.NewLine + "Please select your results file and preferred output again. " + Environment.NewLine + Environment.NewLine + "Error Code: " + Environment.NewLine + ex);
            }
        }
        // Checkbox to let user select if laptimes should be marked with a period or a comma between seconds and miliseconds
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (use_period_in_laptime == 1)
            {
                use_period_in_laptime = 0;
            }
            else
            {
                use_period_in_laptime = 1;
            }
        }
    }
}
