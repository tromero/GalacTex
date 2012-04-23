using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace GalacTex
{
    class TilingPerlinNoise
    {
        // permutation table
        static byte[] permutation = { 151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
            };

        // gradients for 3d noise
        static Vector3[] g = {
            new Vector3(1,1,0),
            new Vector3(-1,1,0),
            new Vector3(1,-1,0),
            new Vector3(-1,-1,0),
            new Vector3(1,0,1),
            new Vector3(-1,0,1),
            new Vector3(1,0,-1),
            new Vector3(-1,0,-1),
            new Vector3(0,1,1),
            new Vector3(0,-1,1),
            new Vector3(0,1,-1),
            new Vector3(0,-1,-1),
            new Vector3(1,1,0),
            new Vector3(0,-1,1),
            new Vector3(-1,1,0),
            new Vector3(0,-1,-1)
        };

        static byte perm(int i)
        {
            return permutation[i % 256];
        }

        static Vector3 texfade(Vector3 t)
        {
            return t * t * t * (t * (t * 6 - new Vector3(15)) + new Vector3(10));
            
        }

        static float texgrad(int hash, Vector3 p)
        {
            return Vector3.Dot(g[hash % 16], p);
        }

        static float texgradperm(int x, Vector3 p)
        {
            return texgrad(perm(x), p);
        }

        public static float Noise(Vector3 position, int repeat, int offset)
        {
            int[] I = { (int)Math.Floor(position.X) % repeat, (int)Math.Floor(position.Y) % repeat, (int)Math.Floor(position.Z) % repeat };
            int[] J = { (I[0] + 1) % repeat, (I[1] + 1) % repeat, (I[2] + 1) % repeat};
            I[0] += offset;
            I[1] += offset;
            I[2] += offset;
            J[0] += offset;
            J[1] += offset;
            J[2] += offset;

            position.X -= (float)Math.Floor(position.X);
            position.Y -= (float)Math.Floor(position.Y);
            position.Z -= (float)Math.Floor(position.Z);

            Vector3 f = texfade(position);
            int A  = perm(I[0]);
            int AA = perm(A+I[1]);
            int AB = perm(A+J[1]);

            int B  = perm(J[0]);
            int BA = perm(B+I[1]);
            int BB = perm(B+J[1]);

            return // a whole bunch of lerp derp derp
                MathHelper.Lerp(
                    MathHelper.Lerp(
                        MathHelper.Lerp(
                            texgradperm(AA + I[2], position + new Vector3(0, 0, 0)),
                            texgradperm(BA + I[2], position + new Vector3(-1, 0, 0)),
                            f.X),
                        MathHelper.Lerp(
                            texgradperm(AB + I[2], position + new Vector3(0, -1, 0)),
                            texgradperm(BB + I[2], position + new Vector3(-1, -1, 0)),
                            f.X),
                        f.Y),
                    MathHelper.Lerp(
                        MathHelper.Lerp(
                            texgradperm(AA + J[2], position + new Vector3(0, 0, -1)),
                            texgradperm(BA + J[2], position + new Vector3(-1, 0, -1)),
                            f.X),
                        MathHelper.Lerp(
                            texgradperm(AB + J[2], position + new Vector3(0, -1, -1)),
                            texgradperm(BB + J[2], position + new Vector3(-1, -1, -1)),
                            f.X),
                        f.Y),
                    f.Z);

        }
    }
}
