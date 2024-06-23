namespace WorldModify
{
    internal class ReTileInfo
    {
        public TileInfo before = new TileInfo();

        public TileInfo after = new TileInfo();

        public string comment = "";

        public ReTileInfo(int beforeID, int afterID, int bType = 0, int aType = 0, string _comment = "")
        {
            before.id = beforeID;
            after.id = afterID;
            before.type = bType;
            after.type = aType;
            if (!string.IsNullOrEmpty(_comment))
            {
                comment = _comment;
            }
        }
    }
}
