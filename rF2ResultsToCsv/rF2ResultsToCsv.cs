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
            MessageBox.Show(fp_SaveFile);
            return fp_SaveFile;
        }
        // Define class for holding the relevant lap data
        public class rf2_lap
        {
            public string driver { get; set; }
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

        public rF2ResultsToCsv()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        // Currently named "Parse"
        private void button1_Click(object sender, EventArgs e)
        {
            // declaring variables needed later
            // For retrieving data:
            string driver_name;
            string carclass;
            string carnumber;
            string teamname;
            string lap_num;
            string lap_pos;
            string lap_start;
            string s1;
            string s2;
            string s3;
            string fuel;
            string tires_front;
            string tires_rear;
            string pit;
            string laptime;


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
            // Container for csv output
            var csv = new StringBuilder();
            string newline;

            // Needs Code to figure out driver swaps and accurately assign driver names to laps, as the results file does list all laps under a single (starting?) driver

            foreach (XmlNode node in xmlnode)
            {
                // Find and set driver name
                if (node.SelectSingleNode("Name").InnerText != null)
                {
                    driver_name = node.SelectSingleNode("Name").InnerText;
                }
                else
                {
                    driver_name = "[Unknown]";
                }

                // Select all laps for a given driver.
                XmlNodeList laps = xmlinput.DocumentElement.SelectNodes("//Driver[Name=\"" + driver_name + "\"]/Lap");

                // Debugging outputs
                if (laps.Count < 0)
                {
                    MessageBox.Show("Number of laps found for driver " + driver_name + ": " + laps.Count);
                }
                // Add laps to list

                    foreach (XmlNode lapnode in laps)
                    {
                    // Check if the attribute exists, then set it to the value found in the results file - or set it to the default
                    if (lapnode.Attributes["CarClass"] != null)
                    {
                        carclass = lapnode.Attributes["CarClass"].InnerText;
                    }
                    else
                    {
                        carclass = "[Unknown]";
                    }
                    if (lapnode.Attributes["CarNumber"] != null)
                    {
                        carnumber = lapnode.Attributes["CarNumber"].InnerText;
                    }
                    else
                    {
                        carnumber = "[Unknown]";
                    }
                    if (lapnode.Attributes["TeamName"] != null)
                    {
                        teamname = lapnode.Attributes["TeamName"].InnerText;
                    }
                    else
                    {
                        teamname = "[Unknown]";
                    }
                    if (lapnode.Attributes["num"] != null)
                    {
                        lap_num = lapnode.Attributes["num"].InnerText;
                    }
                    else
                    {
                        lap_num = "[Unknown]";
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
                        pit = lapnode.Attributes["pit"].InnerText;
                    }
                    else
                    {
                        pit = "0";
                    }
                    if (lapnode.InnerText != null)
                    {
                        laptime = lapnode.InnerText;
                    }
                    else
                    {
                        laptime = "999.999";
                    }
                    // Write the selected values to the list
                    laps_list.Add(new rf2_lap
                    {
                            driver = driver_name,       carclass = carclass,    carnumber = carnumber,  teamname = teamname,
                            lap_num = lap_num,          lap_pos = lap_pos,      lap_start = lap_start,  s1 = s1,
                            s2 = s2,                    s3 = s3,                fuel = fuel,            tires_front = tires_front,
                            tires_rear = tires_rear,    pit = pit,              laptime = laptime,
                        }       );        
                }
                // Stuff to do for each driver goes here
            }
            // Stuff done once after cycling through all the laps and drivers goes here
            newline = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}", "[Driver]","[CarClass]", "[CarNumber]", "[TeamName]", "[LapNum]", "[LapPos]", "[LapStart]", "[Sector1]", "[Sector2]", "[Sector3]", "[Fuel]", "[TiresFront]", "[TiresRear]", "[Pit]", "[Laptime]");
            csv.AppendLine(newline);

            int num_laps; int n;
            num_laps = laps_list.Count;
            n = 0;
            while (n < num_laps)
            {
                rf2_lap output = laps_list[n];
                newline = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}", output.driver, output.carclass, output.carnumber, output.teamname, output.lap_num, output.lap_pos, output.lap_start, output.s1, output.s2, output.s3, output.fuel, output.tires_front, output.tires_rear, output.pit, output.laptime);
                csv.AppendLine(newline);
                n++;
            }
            string output_fpath = SaveFilePath();
            File.WriteAllText(output_fpath, csv.ToString());
        }
    }
}