using ExcelConverter.Script;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelConverter
{
    public partial class Form1 : Form
    {
        ConfigManager configData;
        ConvertClass convertClass;

        public Form1()
        {
            configData = new ConfigManager();
            configData.MessageBoxCreate = ShowMessage;
            convertClass = new ConvertClass();
            convertClass.MessageBoxCreate = ShowMessage;
            InitializeComponent();

            var loadData = configData.LoadConfig();
            excelPath.Text = loadData.ExcelPath;
            clientCodePath.Text = loadData.ClientPath;
            serverCodePath.Text = loadData.ServerPath;

            convertClass.SetPathData(excelPath.Text, clientCodePath.Text, serverCodePath.Text);

            int excelCountData = convertClass.GetExcelCount();
            if (excelCountData != 0)
            {
                excelCount.Text = excelCountData.ToString();
            }

            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            convertClass.ConvertExcel();
        }

        private void PathSetButton_Click(object sender, EventArgs e)
        {
            convertClass.SetPathData(excelPath.Text, clientCodePath.Text, serverCodePath.Text);

            int excelCountData = convertClass.GetExcelCount();
            if (excelCountData != 0)
            {
                excelCount.Text = excelCountData.ToString();
            }
        }

        public void ShowMessage(string msg)
        {
            MessageBox.Show(msg);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConfigData config = new ConfigData();
            config.ExcelPath = excelPath.Text;
            config.ClientPath = clientCodePath.Text;
            config.ServerPath = serverCodePath.Text;

            configData.SaveConfig(config);
        }

    }
}
