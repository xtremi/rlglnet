using GlmNet;
using System;
namespace rlglnet
{
    interface ISurface3Dfunction
    {
        float Value(float x, float y);
        float Max();
        float Min();
    }

    class FlatSurfaceFunction : ISurface3Dfunction
    {
        public float Height { private get; set; }

        public float Value(float x, float y)
        {
            return Height;
        }
        public float Max()
        {
            return Height;
        }
        public float Min()
        {
            return 0.0f;
        }
    }

    class SineSurfaceFunction : ISurface3Dfunction
    {
        public float Amplitude { private get; set; }
        public float WaveLength{ private get; set; }
        public float Value(float x, float y)
        {
            return Amplitude * glm.sin(x * 2.0f * MathF.PI / WaveLength);
        }
        public float Max()
        {
            return Amplitude;
        }
        public float Min()
        {
            return -Amplitude;
        }
    }

    class PlaneWaveFunction : ISurface3Dfunction
    {
        public float Amplitude { private get; set; }
        public float WaveLength { private get; set; }
        public float Value(float x, float y)
        {
            float R = MathF.Sqrt(MathF.Pow(x, 2.0f) + MathF.Pow(y, 2.0f));
            float z = Amplitude * glm.cos(R / WaveLength);
            return z;
        }
        public float Max()
        {
            return Amplitude;
        }
        public float Min()
        {
            return -Amplitude;
        }
    }

}
