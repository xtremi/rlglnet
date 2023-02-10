﻿using System;
using System.Collections.Generic;
using rlglnet;

namespace Test_rlglnet
{
    class Program
    {
        static void Main(string[] args)
        {
            rlglnet.rlglQuadTree quadTree = new rlglQuadTree(10.0f, 6);

            GlmNet.vec3 pos1 = new GlmNet.vec3(2.5f, 2.5f, 0.0f);
            List<rlglQuadTreeElement> quads = quadTree.getQuads(pos1);

            //https://scatterplot.online/
            foreach (rlglQuadTreeElement quad in quads)
            {
                //Console.WriteLine("[" + quad.Level + "] " + quad.Center.x + ", " + quad.Center.y + ", " + quad.Center.z);
                Console.WriteLine(quad.Center.x + ", " + quad.Center.y);
            }

        }
    }
}