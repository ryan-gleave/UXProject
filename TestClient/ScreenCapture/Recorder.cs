using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAvi.Output;
using System.IO;

namespace TestClient.ScreenCapture
{
    class Recorder : StartStopThread
    {
        private int width;
        private int height;
        private AviWriter writer;
        private Encoder encoder;
        private int fps;
        private int chunkCount = 0;
        private DateTime timestamp_started;
        private bool paused = false;
        private bool skipsave = false;

        private bool chunkingEnabled = true;
        public bool ChunkingEnabled { get { return chunkingEnabled; } }

        private string filepath;
        public string Filepath { get { return filepath; } }

        public TimeSpan RecordDuration { get { return DateTime.Now - timestamp_started; } }

        public Recorder(string filepath, int fps, Encoder encoder)
        {
            this.encoder = encoder;
            this.filepath = filepath;
            this.fps = fps;
            height = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenHeight);
            width  = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenWidth);

            try
            {
                this.chunkingEnabled = Properties.Settings.Default.ChunkRecordEnabled;
            }
            catch (Exception ex)
            {
                //property doesn't exist? - revert to default rather than breaking
                Console.WriteLine(ex.Message);
                this.chunkingEnabled = false;
            }
        }

        private byte[] CaptureScreen(int width, int height)
        {
            try
            {
                //Create a bitmap for the current frame, takes a screencap
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);
                System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(bitmap);
                gfx.CopyFromScreen(0, 0, 0, 0, bitmap.Size, System.Drawing.CopyPixelOperation.SourceCopy);

                // create a buffer for this frame (to turn bitmap into bytes)
                var buffer = new byte[width * height * 4];
                // Now copy bits from bitmap to buffer
                var bits = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                System.Runtime.InteropServices.Marshal.Copy(bits.Scan0, buffer, 0, buffer.Length);
                bitmap.UnlockBits(bits);

                return buffer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public override void Start()
        {
            timestamp_started = DateTime.Now;
            base.Start();
        }

        public void Stop(bool save)
        {
            this.skipsave = !save;
            base.Stop();
        }

        //Overrides from MyThread
        protected override void Run()
        {
            DirectoryInfo linfo = new DirectoryInfo(filepath);
            string localName = linfo.Name;
            if (ChunkingEnabled) localName += "-c" + chunkCount;

            string currentFilepath = filepath + localName + "/" + localName + ".avi";

            System.IO.FileInfo info = new System.IO.FileInfo(currentFilepath);
            if (!info.Directory.Exists)
                System.IO.Directory.CreateDirectory(info.Directory.FullName);

            writer = encoder.GetWriter(currentFilepath, fps);
            IAviVideoStream stream = encoder.GetStream(writer, width, height);

            Console.WriteLine("Recording started...");

            while (base.running)
            {
                //Performance adjustment, will lower cpu usage massively while paused and slightly while recording with no quality loss
                System.Threading.Thread.Sleep(1); 
                if (paused) continue;

                //Capture the screen for this frame
                byte[] buffer = CaptureScreen(stream.Width, stream.Height);
                if (buffer == null) continue;

                //Serialize this frame to the file (stream)
                stream.WriteFrame(true, buffer, 0, buffer.Length);

                if (ChunkingEnabled)
                {
                    if (stream.FramesWritten >= 900)
                    {
                        Console.WriteLine("Saved {0} frames of video", stream.FramesWritten);
                        writer.Close();
                        FTPQueue.Add(currentFilepath);

                        chunkCount++;
                        if (ChunkingEnabled) localName = linfo.Name + "-c" + chunkCount;
                        currentFilepath = filepath + localName + "/" + localName + ".avi";

                        info = new System.IO.FileInfo(currentFilepath);
                        if (!info.Directory.Exists)
                            System.IO.Directory.CreateDirectory(info.Directory.FullName);

                        writer = encoder.GetWriter(currentFilepath, fps);
                        stream = encoder.GetStream(writer, width, height);
                    }
                }
            }

            Console.WriteLine("Saved {0} frames of video", stream.FramesWritten);
            writer.Close();

            //TODO: Delete local files (split up delete and upload)
            if(!skipsave)
                FTPQueue.Add(currentFilepath);
        }

        public void Pause()
        {
            paused = true;
        }

        public void Resume()
        {
            paused = false; 
        }

    }
}
