using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAvi.Output;

namespace TestClient.ScreenCapture
{
    abstract class Encoder
    {
        public abstract IAviVideoStream GetStream(AviWriter writer, int width, int height);

        public AviWriter GetWriter(string filepath, int fps)
        {
            return new AviWriter(filepath)
            {
                FramesPerSecond = fps,
                EmitIndex1 = true //enabled for compatibility (see documentation)
            };
        }
    }
}
