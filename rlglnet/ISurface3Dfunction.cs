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

    interface IColorFunction
    {
        void Value(out float[] color, float height, float[] normal);
    }

    class TerrainColorFunction : IColorFunction
    {
        public void Value(out float[] color, float height, float[] normal)
        {

            if(height < 0.4f)
            {
                color = new float[3] { 0.0f, 0.0f, 1.0f };
            }
            else if (height < 0.85f)
            {
                color = new float[3] { 0.0f, 1.0f, 0.0f };

                if (normal[2] < 0.75) 
                {
                    color = new float[3] { 0.5f, 0.25f, 0.125f };
                }

            }
            else 
            {
                color = new float[3] { 1.0f, 1.0f, 1.0f };
            }

        }

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

    class SimplexNoise2Dfunction : ISurface3Dfunction
    {
        public float Amplitude { private get; set; }
        public int Octaves { private get; set; }
        public float Frequency { private get; set; }
        public float Roughness { private get; set; }
        public float Persistance { private get; set; }
        public float Value(float x, float y)
        {
            return Amplitude * (float)SimplexNoise.NoiseValue(x, y, Octaves, Frequency, Persistance, Roughness);
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
