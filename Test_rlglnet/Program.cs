using System;
using System.Collections.Generic;
using rlglnet;

namespace Test_rlglnet
{
    class Program
    {

        static void Test_QuadTreeGetQuads(float size, int maxSubdivisions, GlmNet.vec3 pos)
        {
            rlglnet.rlglQuadTree quadTree = new rlglQuadTree(size, maxSubdivisions);
            List<rlglQuadTreeElement> quads = quadTree.GetQuads(pos);

            //https://scatterplot.online/
            /*foreach (rlglQuadTreeElement quad in quads)
            {
                Console.WriteLine("[" + quad.Level + "] " + quad.Center.x + ", " + quad.Center.y + ", " + quad.Center.z);
                //Console.WriteLine(quad.Center.x + ", " + quad.Center.y);
            }*/
        }

        static void Test_QuadTreeGetQuadsMultiTakeTime(int nRuns, int maxSubdvisions)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Random rand = new Random(13);

            float size = 10.0f;
            for(int i = 0; i < nRuns; i++)
            {
                GlmNet.vec3 pos = new GlmNet.vec3(
                    size * (2.0f * ((float)rand.NextDouble() - 0.5f)), 
                    size * (2.0f * ((float)rand.NextDouble() - 0.5f)), 
                    0.0f);

                Test_QuadTreeGetQuads(size, maxSubdvisions, pos);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("time = " + elapsedMs + " nruns = " + nRuns + " maxSubd. = " + maxSubdvisions);
        }

        static void Main(string[] args)
        {

            Test_QuadTreeGetQuadsMultiTakeTime(10000, 3);
            Test_QuadTreeGetQuadsMultiTakeTime(10000, 4);
            Test_QuadTreeGetQuadsMultiTakeTime(10000, 5);
            Test_QuadTreeGetQuadsMultiTakeTime(100000, 3);
            Test_QuadTreeGetQuadsMultiTakeTime(100000, 4);
            Test_QuadTreeGetQuadsMultiTakeTime(100000, 5);
            Test_QuadTreeGetQuadsMultiTakeTime(1000000, 3);
            Test_QuadTreeGetQuadsMultiTakeTime(1000000, 4);
            Test_QuadTreeGetQuadsMultiTakeTime(1000000, 5);

        }
    }
}
