using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ProdeusConverter
{
    public enum SerializeMode
    {
        Append = 0,
        Overwrite = 1
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string s_DefaultImagePath = "./DefaultImage.jpg";
        private static string s_DefaultImageThumbPath = "./DefaultImage_thumb.jpg";

        //Constructor & Destructor
        public MainWindow()
        {
            InitializeComponent();

            //Listen to log events
            Logger.MessagedLoggedEvent += OnMessageLogged;

            //Setup InfoTextbox
            InfoTextbox.Document.Blocks.Clear();
            InfoTextbox.AppendText("Select input & output files then press convert.");
        }

        ~MainWindow()
        {
            //Stop listening to log events
            Logger.MessagedLoggedEvent -= OnMessageLogged;
        }

        //UI Callbacks
        private void SelectInputFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog?view=net-5.0

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Prodeus map (*.emap)|*.emap|obj files (*.obj)|*.obj|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 2;

            if (openFileDialog.ShowDialog() == true)
            {
                InputFilePathTextbox.Text = openFileDialog.FileName;
            }
        }

        private void SelectOutputFilePathButton_Click(object sender, RoutedEventArgs e)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.savefiledialog?view=net-5.0

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Prodeus map (*.emap)|*.emap|obj files (*.obj)|*.obj|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == true)
            {
                OutputFilePathTextbox.Text = saveFileDialog.FileName;
            }
        }

        private void SelectInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(InputFilePathTextbox.Text);
        }

        private void SelectOutputFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(OutputFilePathTextbox.Text);
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            //Filepath checks
            string inputFilePath = InputFilePathTextbox.Text;
            string outputFilePath = OutputFilePathTextbox.Text;

            if (File.Exists(inputFilePath) == false)
            {
                Logger.LogMessage(Logger.LogType.Warning, "Input filepath doesn't exist!");
                return;
            }

            if (outputFilePath == string.Empty)
            {
                Logger.LogMessage(Logger.LogType.Warning, "No output filepath selected!");
                return;
            }

            //--------------------
            // Deserialize
            //--------------------
            Model model = new Model();

            try
            {
                Logger.LogMessage(Logger.LogType.Normal, "Reading input file...");
                bool success = model.Deserialize(inputFilePath);

                if (success == false)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Error parsing input file");
                    return;
                }
            }
            catch(Exception exception)
            {
                Logger.LogMessage(Logger.LogType.Error, "Unhandled input error: " + exception.Message);
                return;
            }

            //--------------------
            // Serialize EMAP
            //--------------------
            try
            {
                Logger.LogMessage(Logger.LogType.Normal, "Writing to output file...");

                SerializeMode serializeMode = SerializeMode.Append;
                if (OverwriteRadioButton.IsChecked == true) serializeMode = SerializeMode.Overwrite;

                bool success = model.Serialize(outputFilePath, serializeMode);

                if (success == false)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Error writing output file");
                    return;
                }
            }
            catch (Exception exception)
            {
                Logger.LogMessage(Logger.LogType.Error, "Unhandled output error: " + exception.Message);
                return;
            }

            //-------------------------
            // Copy thumbnail images
            //-------------------------
            Logger.LogMessage(Logger.LogType.Normal, "Copying thumbnail images...");

            //Check if we can find the "DefaultImage". If so, copy it to the same folder with the same name (.jpg)
            string fileName = Path.GetFileNameWithoutExtension(outputFilePath);
            string filePath = outputFilePath.Substring(0, outputFilePath.Length - Path.GetFileName(outputFilePath).Length);

            if (File.Exists(s_DefaultImagePath))
            {
                string newPath = filePath + fileName + ".jpg";

                if (File.Exists(newPath) == false)
                    File.Copy(s_DefaultImagePath, newPath);
            }

            //Same process for "DefaultImage_thumb"
            if (File.Exists(s_DefaultImageThumbPath))
            {
                string newPath = filePath + fileName + "_thumb.jpg";

                if (File.Exists(newPath) == false)
                    File.Copy(s_DefaultImageThumbPath, newPath);
            }

            //Done!
            Logger.LogMessage(Logger.LogType.Normal, "Conversion success!");
        }

        //Utility
        private void OpenFile(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                Logger.LogMessage(Logger.LogType.Warning, "File doesn't exist!");
                return;
            }

            try
            {
                //Try opening the file with the windows default program
                System.Diagnostics.Process.Start(filePath);
            }
            catch
            {
                try
                {
                    //If that doesn't work, try notepad? (especially .emap doesn't have a program by default)
                    System.Diagnostics.Process.Start("notepad.exe", filePath);
                }
                catch (Exception exception)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Couldn't open file: " + exception.Message);
                }
            }
        }

        //Logger Callback
        private void OnMessageLogged(Logger.LogType logType, string message)
        {
            //Determine message color
            SolidColorBrush colorBrush = Brushes.White;

            switch (logType)
            {
                case Logger.LogType.Warning:
                    colorBrush = Brushes.Yellow;
                    break;

                case Logger.LogType.Error:
                    colorBrush = Brushes.Red;
                    break;

                case Logger.LogType.Normal:
                default:
                    colorBrush = Brushes.White;
                    break;
            }

            //Add to textbox
            TextRange textRange = new TextRange(InfoTextbox.Document.ContentEnd, InfoTextbox.Document.ContentEnd);
            textRange.Text = "\n" + message;
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, colorBrush);
            
            //Scroll to the bottom so it can instantly be read
            InfoTextbox.ScrollToEnd();
        }
    }
}
