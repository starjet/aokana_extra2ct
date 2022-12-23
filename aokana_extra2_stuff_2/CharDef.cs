namespace aokana_extra2_stuff_2
{
    internal class CharDef
    {
        public CharDef(string _prefix, string _dispname, List<byte> oS, List<byte> oF, int nf1, int nf2, bool alt)
        {
            this.prefix = _prefix;
            this.dispname = _dispname;
            this.outfitS = oS;
            this.outfitF = oF;
            this.nfaceS = (byte)nf1;
            this.nfaceF = (byte)nf2;
            this.altpose = alt;
            this.npose = (byte)(((this.outfitS.Count > 0) ? 1 : 0) + ((this.outfitF.Count > 0) ? 2 : 0) + (this.altpose ? 1 : 0));
            this.nflypose = (byte)(((this.outfitS.Count > 0) ? 1 : 0) + (this.altpose ? 1 : 0));
            this.poselist = new char[(int)this.npose];
            int num = 0;
            if (this.outfitS.Count > 0)
            {
                this.poselist[num++] = 'A';
            }
            if (this.altpose)
            {
                this.poselist[num++] = 'B';
            }
            if (this.outfitF.Count > 0)
            {
                this.poselist[num++] = 'F';
                this.poselist[num++] = 'I';
            }
            this.Reset();
        }

        public void Reset()
        {
            this.defoutfit = 0;
            this.pose = 0;
            this.face = 0;
            this.distance = 1;
            this.active = false;
            this.slot = -1;
            if (this.prefix == "mis")
            {
                this.defoutfit = 2;
            }
            this.outfit = this.defoutfit;
        }

        public string prefix;

        public string dispname;

        public List<byte> outfitS;

        public List<byte> outfitF;

        public byte nfaceS;

        public byte nfaceF;

        public byte npose;

        public byte nflypose;

        public bool altpose;

        public char[] poselist;

        public bool active;

        public int outfit;

        public int pose;

        public int face;

        public int distance;

        public int slot;

        public int defoutfit;
    }
}

