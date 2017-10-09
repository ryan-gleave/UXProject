using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAvi.Output;
using SharpAvi.Codecs;

namespace TestClient.ScreenCapture.Encoders
{
    class MpegEncoder : Encoder
    {
        private int quality;

        public MpegEncoder(int quality)
        {
            this.quality = quality;
        }

        public override IAviVideoStream GetStream(AviWriter writer, int width, int height)
        {
            IVideoEncoder encoder = new MotionJpegVideoEncoderWpf(width, height, quality);
            IAviVideoStream stream = writer.AddEncodingVideoStream(encoder);
            stream.Width = width;
            stream.Height = height;
            return stream;
        }
    }
}
