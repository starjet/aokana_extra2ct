#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

using System.Collections.ObjectModel;
using System.Text;
using ImageMagick;
using Newtonsoft.Json;

namespace aokana_extra2_stuff_2
{
    internal class Program
    {
        public static FileStream fs;
        public static Dictionary<string, fe> ti = new Dictionary<string, fe>();
        public static List<FaceDef> facedef = new List<FaceDef>();
        public static Dictionary<string, uint> faceidx = new Dictionary<string, uint>();
        public static List<CharDef> chars;

        public struct FaceDef
        {
            public string id;
            public ushort scalever;
            public ushort sw;
            public ushort sh;
            public ushort x;
            public ushort y;
            public ushort w;
            public ushort h;
        }

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

        public static byte[] GetSprite(string fn)
        {
            byte[] sprite = Data("sprites/" + fn);
            sprite ??= Data("hsprites/" + fn);
            return sprite;
        }

        public static List<string> TxtToList(byte[] readbuf)
        {
            List<string> list = new List<string>();
            int num = readbuf.Length;
            int i = 0;
            while (i < num)
            {
                int num2 = i;
                while (i < num && readbuf[i] != 13 && readbuf[i] != 10)
                {
                    i++;
                }
                string @string = Encoding.GetEncoding(65001).GetString(readbuf, num2, i - num2);
                list.Add(@string);
                if (i < num && readbuf[i] == 13)
                {
                    i++;
                }
                if (i < num && readbuf[i] == 10)
                {
                    i++;
                }
            }
            return list;
        }

        private static void InitFaceDefData(string fndef, int scalever)
        {
            byte[] array = File.ReadAllBytes(fndef);
            if (array == null)
            {
                return;
            }
            List<string> list = TxtToList(array);
            int num = facedef.Count - 1;
            ushort num2 = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Length >= 1)
                {
                    if (list[i][0] == '>')
                    {
                        string[] array2 = list[i].Split(',', StringSplitOptions.None);
                        FaceDef item = new FaceDef
                        {
                            scalever = (ushort)scalever,
                            id = array2[0].Substring(1),
                            sw = Convert.ToUInt16(array2[1]),
                            sh = Convert.ToUInt16(array2[2]),
                            x = Convert.ToUInt16(array2[3]),
                            y = Convert.ToUInt16(array2[4]),
                            w = Convert.ToUInt16(array2[5]),
                            h = Convert.ToUInt16(array2[6])
                        };
                        facedef.Add(item);
                        num++;
                        num2 = 0;
                    }
                    else
                    {
                        uint value = (uint)(num << 16 | (int)num2);
                        string key = (scalever > 1) ? ("hi/" + list[i]) : list[i];
                        faceidx.Add(key, value);
                        num2 += 1;
                    }
                }
            }
        }

        private static void GenSeqList(int n, List<byte> L)
        {
            byte b = 1;
            while ((int)b <= n)
            {
                L.Add(b);
                b += 1;
            }
        }

        private static void UseSeqList(string[] s, List<byte> L)
        {
            foreach (string value in s)
            {
                L.Add(Convert.ToByte(value));
            }
        }

        private static void LoadCharDef()
        {
            List<string> list = TxtToList(File.ReadAllBytes("faceinfo.csv"));
            chars = new List<CharDef>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Length >= 1 && list[i][0] != ';')
                {
                    string[] array = list[i].Split(',', StringSplitOptions.None);
                    List<byte> list2 = new List<byte>();
                    List<byte> list3 = new List<byte>();
                    int nf = 0;
                    int nf2 = 0;
                    bool alt = false;
                    for (int j = 2; j < array.Length; j++)
                    {
                        if (array[j][0] == 'B')
                        {
                            alt = true;
                        }
                        else if (array[j][0] == 'A')
                        {
                            int n = Convert.ToInt32(array[j].Substring(1));
                            GenSeqList(n, list2);
                        }
                        else if (array[j][0] == 'F')
                        {
                            int n2 = Convert.ToInt32(array[j].Substring(1));
                            GenSeqList(n2, list3);
                        }
                        else if (array[j][0] == 'a')
                        {
                            string[] s = array[j].Substring(1).Split('/', StringSplitOptions.None);
                            UseSeqList(s, list2);
                        }
                        else if (array[j][0] == 'f')
                        {
                            string[] s2 = array[j].Substring(1).Split('/', StringSplitOptions.None);
                            UseSeqList(s2, list3);
                        }
                        else if (array[j][0] == 'H')
                        {
                            nf = Convert.ToInt32(array[j].Substring(1));
                        }
                        else if (array[j][0] == 'G')
                        {
                            nf2 = Convert.ToInt32(array[j].Substring(1));
                        }
                    }
                    CharDef item = new CharDef(array[0], array[1], list2, list3, nf, nf2, alt);
                    chars.Add(item);
                }
            }
        }

        private static string UpdateSprite(CharDef charDef)
        {
            char c = charDef.poselist[charDef.pose];
            char c2;
            List<byte> list;
            if (c <= 'B')
            {
                if (c == 'A' || c == 'B')
                {
                    c2 = 'H';
                    list = charDef.outfitS;
                    goto IL_65;
                }
            }
            else
            {
                if (c == 'F')
                {
                    c2 = 'G';
                    list = charDef.outfitF;
                    goto IL_65;
                }
                if (c == 'I')
                {
                    c2 = 'J';
                    list = charDef.outfitF;
                    goto IL_65;
                }
            }
            c2 = ' ';

            list = null;
        IL_65:
            string text = "";
            int num = charDef.face + 1;
            string fname = "";
            //Console.WriteLine("test" + JsonConvert.SerializeObject(charDef));
            if (charDef.prefix != "sit")
            {
                string text2 = "a";
                switch (charDef.distance)
                {
                    case 0:
                        text = "SS";
                        break;
                    case 1:
                        text = "S";
                        break;
                    case 2:
                        text = "M";
                        break;
                    case 3:
                        text = "L";
                        break;
                }
                fname = string.Format("{0}{1}{2:00}{3}+{4}{5}{6:00}{7}{8}", new object[]
                {
                charDef.prefix,
                c,
                list[charDef.outfit],
                text,
                charDef.prefix,
                c2,
                num,
                text,
                text2
                });
            }
            return fname;
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
                Directory.CreateDirectory("_out_cgs");
                string[] cgList = File.ReadAllLines("vcglist.csv");
                foreach (string cg in cgList)
                {
                    string[] cgVariants = cg.ToLower().Split(' ');
                    string cgName = cgVariants[0];
                    Console.WriteLine(cgName);
                    byte[] baseImage = GetCg(cgVariants[1] + ".webp");
                    if (baseImage != null)
                    {
                        MagickImage magickImage = new MagickImage(baseImage);
                        for (int i = 2; i < cgVariants.Length; i++)
                        {
                            MagickImage variantPart = new MagickImage(GetCg(cgVariants[i] + ".webp"));
                            magickImage.Composite(variantPart, CompositeOperator.Over);
                        }
                        magickImage.Write(@"_out_cgs\" + cgName + ".jpg");
                    }
                }
            }
            if (path.Contains("sprites.dat") || path.Contains("adult.dat"))
            {
                Directory.CreateDirectory("_out_sprites");
                InitFaceDefData("defs.txt", 1);
                LoadCharDef();
                List<string> sprites = new List<string>();
                foreach (CharDef char1 in chars)
                {
                    foreach (char pose in char1.poselist)
                    {
                        for (int dist = 0; dist < 4; dist++)
                        {
                            IEnumerable<byte> outfits = char1.outfitS.Union(char1.outfitF);
                            foreach (byte outfit in outfits)
                            {
                                char1.pose = Array.IndexOf(char1.poselist, pose);
                                char1.distance = dist;
                                List<byte> outfitList = pose <= 'B' ? char1.outfitS : char1.outfitF;
                                char1.outfit = outfitList.IndexOf(outfit);
                                if (char1.outfit > -1)
                                {
                                    string sprite = UpdateSprite(char1);
                                    if (sprite != "")
                                    {
                                        sprites.Add(sprite);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (string sprite in sprites)
                {
                    string[] spriteSplit = sprite.Split('+');
                    string spriteBase = spriteSplit[0];
                    string spriteFace = spriteSplit[1];
                    byte[] baseData = GetSprite(spriteBase.ToLower() + ".webp");
                    if (baseData != null)
                    {
                        uint value;
                        if (faceidx.TryGetValue(spriteFace, out value))
                        {
                            int index = (int)(value >> 16);
                            int pos = (int)(value & 65535U);
                            FaceDef faceDef = facedef[index];
                            byte[] faceData = GetSprite(faceDef.id.ToLower() + ".webp");
                            if (faceData != null)
                            {
                                MagickImage faceImage = new MagickImage(faceData);
                                //faceImage.Crop(faceDef.w, faceDef.h);
                                //baseImage.Composite(faceImage, faceDef.x, faceDef.y, CompositeOperator.Over);
                                //baseImage.Write(@"_out_sprites\" + sprite.Replace('+', '_') + ".png");
                                //IReadOnlyCollection<IMagickImage<byte>> faceParts = faceImage.CropToTiles(faceDef.w, faceDef.h);
                                //int i = 0;
                                //foreach (IMagickImage<byte> face in faceParts)
                                //{
                                //    Console.WriteLine(sprite.Replace('+', '_') + (++i).ToString());
                                //    //MagickImage baseImage = new MagickImage(baseData);
                                //    //baseImage.Composite(face, faceDef.x, faceDef.y, CompositeOperator.Over);
                                //    //baseImage.Write(@"_out_sprites\" + sprite.Replace('+', '_') + (++i).ToString() + ".png");
                                //}
                                Console.WriteLine(sprites.ToArray().Length);
                            }
                        }
                    }
                }
            }
        }
    }
}
