using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.ComponentModel;
using System.IO;

namespace TestClient
{
    static class FTPClient
    {
        private static string ftp_address
        {
            get
            {
                //needs testing- performance? this would be better off in a constructor (singleton?)
                string s = Properties.Settings.Default.FTPConnectionString;
                if (!s.EndsWith("/")) s += "/";
                if (Uri.IsWellFormedUriString(s, UriKind.Absolute))
                {
                    return s;
                }
                return "ftp://127.0.0.1/";
            }
        }

        private static string ftp_username = "Ryan";
        private static string ftp_password = "";

        public static bool TestConnection()
        {
            FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(ftp_address + "test");
            requestDir.Credentials = new NetworkCredential(ftp_username, ftp_password);
            //requestDir.Timeout = 2000;//set our own timeout?
            WebResponse response = null;
            bool res = false;
            try
            {
                response = requestDir.GetResponse();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Waited {0} ms but could not establish connection to FTP server, details:\n" + ex.Message, requestDir.Timeout);
                return false;
            }
            finally
            {
                if(response != null)
                    response.Close();
            }
            return res;
        }

        public static bool Upload(string filename, bool deleteLocalfile = false)
        {
            //filepath + "chunk-"+chunkCount + "/chunk-"+chunkCount +".avi"
            //e.g bin/Debug/video/chunk-1/chunk-1.avi
            //src bin/Debug/video/chunk-1/
            //tgt bin/Debug/video/chunk-1.zip
            //del bin/Debug/video/chunk-1 (recursively) and bin/Debug/video/chunk-1.zip

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            try
            {
                FileInfo zip = new FileInfo(Compress(filename));

                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftp_username, ftp_password);
                    client.UploadFile(
                        ftp_address + "\\" /*+ "UNIQUE_NAME" + "\\"*/ + zip.Name, 
                        "STOR", 
                        zip.FullName);
                }
                Console.WriteLine("File upload successful");

                if (deleteLocalfile)
                {
                    DirectoryInfo src = new DirectoryInfo(filename);
                    if (!src.Exists) src = src.Parent;

                    if (System.IO.Directory.Exists(src.FullName))
                        System.IO.Directory.Delete(src.FullName, true);
                    if (System.IO.File.Exists(zip.FullName))
                        System.IO.File.Delete(zip.FullName);

                    Console.WriteLine("Cleaned up uploaded files: \"{0}\", \"{1}\"", src.Name, zip.Name);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            timer.Stop();
            if (timer.ElapsedMilliseconds > 9999)
                Console.WriteLine("Upload took {0} secs", timer.ElapsedMilliseconds / 1000);
            else
                Console.WriteLine("Upload took {0} ms", timer.ElapsedMilliseconds);

            return true;
        }

        //Precondition: parameter is a valid filepath or directory that exists
        private static string Compress(string src)
        {
            DirectoryInfo dir = new DirectoryInfo(src);
            if (!dir.Exists)
            {
                dir = dir.Parent;
            }

            FileInfo contents = dir.GetFiles().FirstOrDefault();
            string tgt = dir.Parent.FullName + "\\" + contents.Name.Replace(contents.Extension, ".zip");
            
            System.IO.Compression.ZipFile.CreateFromDirectory(dir.FullName+"\\", tgt, System.IO.Compression.CompressionLevel.Optimal, false);
            Console.WriteLine("Zip compression completed");
            try
            {
                System.IO.FileInfo one = new System.IO.FileInfo(dir.FullName + "\\" + contents.Name);
                System.IO.FileInfo two = new System.IO.FileInfo(tgt);
                Console.WriteLine("Zipping compressed {0} bytes", one.Length - two.Length);
            }
            catch { }

            return tgt;
        }

        public static bool Download(string filename)
        {
            try
            {
                Console.WriteLine("Downloading file {0}", filename);
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftp_username, ftp_password);
                    client.DownloadFile(ftp_address + filename, "DL-" + filename);
                }
                Console.WriteLine("Download complete");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }

    public class FtpCompleteEventArgs
    {
        private bool successful;
        public bool Successful { get { return successful; } }

        public FtpCompleteEventArgs(bool success)
        {
            this.successful = success;
        }

    }
}
