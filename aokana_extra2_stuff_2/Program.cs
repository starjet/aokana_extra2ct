#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Text;
using ImageMagick;

namespace aokana_extra2_stuff_2
{
    internal class Program
    {
        public static FileStream fs;
        public static Dictionary<string, fe> ti = new Dictionary<string, fe>();

        public struct fe
        {
            public uint p;
            public uint L;
            public uint k;
        }

        protected static void Init2(byte[] rtoc, byte[] rpaths, int numfiles)
        {
            int num = 0;
            for (int i = 0; i < numfiles; i++)
            {
                int num2 = 16 * i;
                uint l = BitConverter.ToUInt32(rtoc, num2);
                int num3 = BitConverter.ToInt32(rtoc, num2 + 4);
                uint k = BitConverter.ToUInt32(rtoc, num2 + 8);
                uint p = BitConverter.ToUInt32(rtoc, num2 + 12);
                int num4 = num3;
                while (num4 < rpaths.Length && rpaths[num4] != 0)
                {
                    num4++;
                }
                string key = Encoding.ASCII.GetString(rpaths, num, num4 - num).ToLower();
                fe value = default(fe);
                value.p = p;
                value.L = l;
                value.k = k;
                ti.Add(key, value);
                num = num4 + 1;
            }
        }

        private static void gk(byte[] b, uint k0)
        {
            uint num = k0 * 4892U + 42816U;
            uint num2 = num << 7 ^ num;
            for (int i = 0; i < 256; i++)
            {
                num -= k0;
                num += num2;
                num2 = num + 156U;
                num *= (num2 & 206U);
                b[i] = (byte)num;
                num >>= 3;
            }
        }

        protected static void dd(byte[] b, int L, uint k)
        {
            byte[] array = new byte[256];
            gk(array, k);
            for (int i = 0; i < L; i++)
            {
                byte b2 = b[i];
                b2 ^= array[i % 179];
                b2 += 3;
                b2 += array[i % 89];
                b2 ^= 119;
                b[i] = b2;
            }
        }

        public static byte[] Data(string fn)
        {
            fe fe;
            if (!ti.TryGetValue(fn, out fe))
            {
                return null;
            }
            fs.Position = (long)((ulong)fe.p);
            byte[] array = new byte[fe.L];
            fs.Read(array, 0, array.Length);
            dd(array, array.Length, fe.k);
            return array;
        }

        public static byte[] GetCg(string fn)
        {
            byte[] cg = Data("evcg/en/" + fn);
            cg ??= Data("evcg/" + fn);
            cg ??= Data("evcg2/en/" + fn);
            cg ??= Data("evcg2/" + fn);
            cg ??= Data("evcg3/en/" + fn);
            cg ??= Data("evcg3/" + fn);
            cg ??= Data("hevcg/" + fn);
            return cg;
        }

        static void Main(string[] args)
        {
            string path = args[0];
            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Position = 0L;
            byte[] array = new byte[1024];
            fs.Read(array, 0, 1024);
            int num = 0;
            for (int i = 3; i < 255; i++)
            {
                num += BitConverter.ToInt32(array, i * 4);
            }
            byte[] array2 = new byte[16 * num];
            fs.Read(array2, 0, array2.Length);
            dd(array2, 16 * num, BitConverter.ToUInt32(array, 212));
            int num2 = BitConverter.ToInt32(array2, 12) - (1024 + 16 * num);
            byte[] array3 = new byte[num2];
            fs.Read(array3, 0, array3.Length);
            dd(array3, num2, BitConverter.ToUInt32(array, 92));
            Init2(array2, array3, num);
            foreach (string file in ti.Keys)
            {
                Console.WriteLine(file);
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                byte[] contents = Data(file);
                if (contents != null)
                {
                    File.WriteAllBytes(file, Data(file));
                }
            }
            if (path.Contains("evcg.dat") || path.Contains("adult.dat"))
            {
                string[] cgList = File.ReadAllLines("vcglist.csv");
                foreach (string cg in cgList)
                {
                    string[] cgVariants = cg.ToLower().Split(' ');
                    string cgName = cgVariants[0];
                    Console.WriteLine(cgName);
                    byte[] baseImage = GetCg(cgVariants[1] + ".webp");
                    Directory.CreateDirectory("_out");
                    if (baseImage != null)
                    {
                        MagickImage magickImage = new MagickImage(baseImage);
                        for (int i = 2; i < cgVariants.Length; i++)
                        {
                            MagickImage variantPart = new MagickImage(GetCg(cgVariants[i] + ".webp"));
                            magickImage.Composite(variantPart, CompositeOperator.Over);
                        }
                        magickImage.Write(@"_out\" + cgName + ".jpg");
                    }
                    else
                    {
                        //File.AppendAllText("_notfound.txt", cgName + "\r\n");
                    }
                }
            }
        }
    }
}
