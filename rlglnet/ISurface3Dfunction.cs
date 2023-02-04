using GlmNet;
using System;

namespace rlglnet
{
    interface ISurface3Dfunction
    {
        float Value(float x, float y);
        vec3 Normal(float x, float y, float ds);
        float Max();
        float Min();

        float NormalizeValue(float value);
    }

    interface IColorFunction
    {
        void Value(out vec3 color, float height, vec3 normal);
    }

    class TerrainColorFunction : IColorFunction
    {
        public void Value(out vec3 color, float height, vec3 normal)
        {

            if(height < 0.4f)
            {
                color = new vec3(0.0f, 0.0f, 1.0f);
            }
            else if (height < 0.85f)
            {
                color = new vec3(0.0f, 1.0f, 0.0f);

                if (normal[2] < 0.75) 
                {
                    color = new vec3(0.5f, 0.25f, 0.125f);
                }

            }
            else 
            {
                color = new vec3(1.0f, 1.0f, 1.0f);
            }

        }

    }

    abstract class SurfaceFunction : ISurface3Dfunction
    {
        public float Height { protected get; set; }
        public virtual float Value(float x, float y)
        {
            return 0.0f;
        }
        public virtual vec3 Normal(float x, float y, float ds)
        {

            vec3 pos = new vec3(x, y, Value(x, y));
            vec3 posDX0 = new vec3(x - ds, y,      Value(x - ds, y));
            vec3 posDX1 = new vec3(x + ds, y,      Value(x + ds, y));
            vec3 posDY0 = new vec3(x,      y - ds, Value(x,      y - ds));
            vec3 posDY1 = new vec3(x,      y + ds, Value(x, y + ds));

            vec3 dir1 = glm.normalize(posDX1 - posDX0);
            vec3 dir2 = glm.normalize(posDY1 - posDY0);

            return glm.normalize(glm.cross(dir1, dir2));

        }
        public virtual float Max()
        {
            return Height;
        }
        public virtual float Min()
        {
            return 0.0f;
        }
        public float NormalizeValue(float value)
        {
            float normValue = (value - Min()) / (Max() - Min());
            if (float.IsNaN(normValue))
            {
                return 0.0f;
            }
            return normValue;
        }

    }

    class FlatSurfaceFunction : SurfaceFunction
    {
    }

    class SineSurfaceFunction : SurfaceFunction
    {
        public float Amplitude { 
            protected get { return Height; } 
            set { Height = value; } 
        }
        public float WaveLength{ protected get; set; }
        public override float Value(float x, float y)
        {
            return Amplitude * glm.sin(x * 2.0f * MathF.PI / WaveLength);
        }
        public override float Max()
        {
            return Amplitude;
        }
        public override float Min()
        {
            return -Amplitude;
        }
    }

    class PlaneWaveFunction : SineSurfaceFunction
    {
        public override float Value(float x, float y)
        {
            float R = MathF.Sqrt(MathF.Pow(x, 2.0f) + MathF.Pow(y, 2.0f));
            float z = Amplitude * glm.cos(R / WaveLength);
            return z;
        }
    }

    class SimplexNoise2Dfunction : SurfaceFunction
    {
        public float Amplitude { private get; set; }
        public vec2 Offset { private get; set; }
        public int Octaves { private get; set; }
        public float Frequency { private get; set; }
        public float Roughness { private get; set; }
        public float Persistance { private get; set; }
        public override float Value(float x, float y)
        {
            return Amplitude * (float)SimplexNoise.NoiseValue(x + Offset.x, y + Offset.y, Octaves, Frequency, Persistance, Roughness);
        }
        public override float Max()
        {
            return Amplitude;
        }
        public override float Min()
        {
            return -Amplitude;
        }
    }


}
