using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AACoursework.Tasks;

namespace AACoursework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void ShowNotification(string mess)
        {
            MessageBox.Show(mess);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowNotification("Курсовая работа по предмету 'Анализ алгоритмов'\r\nВариант № 1\r\nВыполнил студент группы xxx yyy");
        }

        #region Tab1a
        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                button2.Visible = false;
                var value = int.Parse(textBox1.Text);

                var generatedValues = new List<string>();

                textBox2.Text = "Generation in progress..";

                backgroundWorker1.RunWorkerAsync();

                var result = "";

                await Task.Run(() =>
                {
                    Task_1_a.DistributeEntry(value, generatedValues);
                    result = string.Join("\r\n", generatedValues);
                });

                textBox2.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ";" + ex.StackTrace);
            }
            finally
            {
                backgroundWorker1.CancelAsync();
                button2.Visible = true;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0);

            for (int i = 0; i <= 100; i++)
            {
                if (this.backgroundWorker1.CancellationPending)
                {
                    backgroundWorker1.ReportProgress(0);
                    e.Cancel = true;
                    return;
                }

                System.Threading.Thread.Sleep(100);
                backgroundWorker1.ReportProgress(i);

                if (i == 100)
                {
                    i = 0;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        #endregion Tab1a

        #region Tab1b
        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox3.Text) || !IsDigitsOnly(textBox3.Text))
                {
                    MessageBox.Show("Only digits allowed");
                    return;
                }

                var generatedValues = new Dictionary<int, int>();
                textBox4.Text = "Distribution in progress..";

                var elements = textBox3.Text.ToArray().Select(x => Int32.Parse(x.ToString())).ToList();
                var n = elements.Sum();

                await Task.Run(() => Task_1_b.DistributeEntry(n, elements, generatedValues));
                var result = string.Join("\r\n", generatedValues.Select(x => "q" + x.Key + " = " + x.Value));
                textBox4.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ";" + ex.StackTrace);
            }
        }

        #endregion Tab1a

        #region Tab2

        bool Tab2Checks()
        {
            if (string.IsNullOrEmpty(textBox5.Text))
            {
                ShowNotification("Empty input values");
                return false;
            }

            var letters = textBox5.Text
                .Replace("+", "")
                .Replace("-", "")
                .Replace("*", "")
                .Replace("/", "")
                .Replace(" ", "")
                .Replace("=", "")
              .Select(x => x).Distinct().ToList();

            if (letters.Count > 10)
            {
                ShowNotification("Too much letters. Only allowed no more than 10 distinct!Sho.");
                return false;
            }

            if (!letters.All(c => c >= 'A' && c <= 'Z'))
            {
                ShowNotification("Invalid letters inside input string");
                return false;
            }

            if ((textBox5.Text.Contains("+") || textBox5.Text.Contains("-")) && (textBox5.Text.Contains("*") || textBox5.Text.Contains("/")))
            {
                ShowNotification("Can not mix +- and */ operation. Use only those with same priority");
                return false;
            }

            return true;
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (Tab2Checks())
            {
                button4.Visible = false;
                var inputValues = textBox5.Text;
                var processedResult = "";

                textBox6.Text = DateTime.Now + ": Calculating in progress..\r\n";

                backgroundWorker2.RunWorkerAsync();

                await Task.Run(() =>
                {
                    try
                    {
                        if (checkBox1.Checked)
                        {
                            processedResult = Task_2.ProcessAlphametricEntryQueued(inputValues);
                        }
                        else
                        {
                            processedResult = Task_2.ProcessAlphametricEntry(inputValues);
                        }

                        if (string.IsNullOrEmpty(processedResult))
                        {
                            processedResult = "Phew. Sorry. No result exist!";
                        }
                    }
                    catch (Exception ex)
                    {
                        processedResult = "Error occured: " + ex.Message;
                    }
                });

                textBox6.Text += DateTime.Now + ":" + processedResult;
                button4.Visible = true;
                backgroundWorker2.CancelAsync();
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker2.ReportProgress(0);

            for (int i = 0; i <= 100; i++)
            {
                if (this.backgroundWorker2.CancellationPending)
                {
                    backgroundWorker2.ReportProgress(0);
                    e.Cancel = true;
                    return;
                }

                System.Threading.Thread.Sleep(100);
                backgroundWorker2.ReportProgress(i);

                if (i == 100)
                {
                    i = 0;
                }
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }
        #endregion Tab2

        #region Tab3

        bool Tab3Checks()
        {
            if (string.IsNullOrEmpty(textBox7.Text))
            {
                ShowNotification("Empty input");
                return false;
            }

            var splitedValues = textBox7.Text.Split(new string[] { ",", "-", ";" }, StringSplitOptions.RemoveEmptyEntries);
            var uniqElements = splitedValues.Distinct().ToArray();
            if (uniqElements.Length < 2)
            {
                ShowNotification("Not enough elements");
                return false;
            }

            if (checkBox2.Checked)
            {
                var elementsShowNum = (int)numericUpDown2.Value;
                if (elementsShowNum > uniqElements.Length)
                {
                    ShowNotification("Incorrect num of elements for show");
                    return false;
                }
            }

            return true;
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            if (Tab3Checks())
            {
                button5.Visible = false;
                var inputValues = textBox7.Text;
                var processedResult = "";

                var splitedValues = textBox7.Text.Split(new string[] { ",", "-", ";" }, StringSplitOptions.RemoveEmptyEntries);

                textBox8.Text = DateTime.Now + ": Calculating in progress..\r\n";

                backgroundWorker3.RunWorkerAsync();

                var uniqElements = splitedValues.Distinct().ToArray();
                var maxRank = uniqElements.Length;
                var maxElementInItem = 0;
                if (checkBox2.Checked)
                {
                    maxRank = (int)numericUpDown1.Value;
                    maxElementInItem = (int)numericUpDown2.Value;
                }

                await Task.Run(() =>
                {
                    try
                    {
                        processedResult = Task_3.GenerateGraySequenceEntryQueued(uniqElements, maxRank, maxElementInItem);
                    }
                    catch (Exception ex)
                    {
                        processedResult = "Error occured: " + ex.Message;
                    }
                });

                textBox8.Text += DateTime.Now + ":" + processedResult;
                button5.Visible = true;
                backgroundWorker3.CancelAsync();
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker3.ReportProgress(0);

            for (var i = 0; i <= 100; i++)
            {
                if (this.backgroundWorker3.CancellationPending)
                {
                    backgroundWorker3.ReportProgress(0);
                    e.Cancel = true;
                    return;
                }

                System.Threading.Thread.Sleep(100);
                backgroundWorker3.ReportProgress(i);

                if (i == 100)
                {
                    i = 0;
                }
            }
        }

        private void backgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar3.Value = e.ProgressPercentage;
        }

        #endregion Tab3

        #region Tab4

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown3.Visible = !numericUpDown3.Visible;
            label7.Visible = !label7.Visible;
        }

        bool Tab4Checks()
        {
            if (string.IsNullOrEmpty(textBox9.Text))
            {
                ShowNotification("Empty input");
                return false;
            }

            var splitedValues = textBox9.Text.Split(new string[] { ",", "-", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (splitedValues.Length != 2)
            {
                ShowNotification("Text field should be used only for n,m values input");
                return false;
            }

            if (checkBox3.Checked)
            {
                var n = int.Parse(splitedValues.ElementAt(0));//число
                var m = int.Parse(splitedValues.ElementAt(1));//кол-во разрядов
                var selectedConst = numericUpDown3.Value;

                if (!(n >= m * selectedConst))
                {
                    ShowNotification("Wrong constant. It should be such constant that n >= m*Const");
                    return false;
                }
            }


            return true;
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            if (Tab4Checks())
            {
                button6.Visible = false;
                var inputValues = textBox9.Text;
                var processedResult = "";

                var splitedValues = textBox9.Text.Split(new string[] { ",", "-", ";" }, StringSplitOptions.RemoveEmptyEntries);

                textBox10.Text = DateTime.Now + ": Calculating in progress..\r\n";

                backgroundWorker4.RunWorkerAsync();


                var n = int.Parse(splitedValues.ElementAt(0));//число
                var m = int.Parse(splitedValues.ElementAt(1));//кол-во разрядов
                var useModVersion = checkBox3.Checked;
                var selectedConst = (int)numericUpDown3.Value;

                await Task.Run(() =>
                {
                    try
                    {
                        processedResult = Task_4.AlgorithmHEntryQueued(n, m, useModVersion, selectedConst);
                    }
                    catch (Exception ex)
                    {
                        processedResult = "Error occured: " + ex.Message;
                    }
                });

                textBox10.Text += DateTime.Now + ":" + processedResult;
                button6.Visible = true;
                backgroundWorker4.CancelAsync();
            }
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker4.ReportProgress(0);

            for (var i = 0; i <= 100; i++)
            {
                if (this.backgroundWorker4.CancellationPending)
                {
                    backgroundWorker4.ReportProgress(0);
                    e.Cancel = true;
                    return;
                }

                System.Threading.Thread.Sleep(100);
                backgroundWorker4.ReportProgress(i);

                if (i == 100)
                {
                    i = 0;
                }
            }
        }

        private void backgroundWorker4_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar4.Value = e.ProgressPercentage;
        }


        #endregion Tab4

        #region Tab5

        bool Tab5Checks()
        {
            if (string.IsNullOrEmpty(textBox11.Text))
            {
                ShowNotification("Empty input");
                return false;
            }

            return true;
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            if (Tab5Checks())
            {
                button7.Visible = false;
                var inputValues = textBox11.Text;
                var processedResult = "";

                var splitedValues = textBox11.Text.Split(new string[] { ",", "-", ";" }, StringSplitOptions.RemoveEmptyEntries);

                textBox12.Text = DateTime.Now + ": Calculating in progress..\r\n";

                backgroundWorker5.RunWorkerAsync();

                await Task.Run(() =>
                {
                    try
                    {
                        processedResult = Task_5.GenerateSubsetsEntryQueued(splitedValues);
                    }
                    catch (Exception ex)
                    {
                        processedResult = "Error occured: " + ex.Message;
                    }
                });

                textBox12.Text += DateTime.Now + ":" + processedResult;
                button7.Visible = true;
                backgroundWorker5.CancelAsync();
            }
        }

        private void backgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker5.ReportProgress(0);

            for (var i = 0; i <= 100; i++)
            {
                if (this.backgroundWorker5.CancellationPending)
                {
                    backgroundWorker5.ReportProgress(0);
                    e.Cancel = true;
                    return;
                }

                System.Threading.Thread.Sleep(100);
                backgroundWorker5.ReportProgress(i);

                if (i == 100)
                {
                    i = 0;
                }
            }
        }

        private void backgroundWorker5_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar5.Value = e.ProgressPercentage;
        }

        #endregion Tab5

        #region β

        bool TabβChecks()
        {
            if (string.IsNullOrEmpty(textBox14.Text))
            {
                ShowNotification("Empty input");
                return false;
            }

            return true;
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        #endregion β
    }
}