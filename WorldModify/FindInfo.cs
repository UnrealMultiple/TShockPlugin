namespace WorldModify
{
    public class FindInfo
    {
        public int id = 0;

        public int w = 1;

        public int h = 1;

        public int style = 0;

        public int frameX = -1;

        public int frameY = -1;

        public FindInfo()
        {
        }

        public FindInfo(int _id, int _style = 0, int _w = 1, int _h = 1, int _frameX = -1, int _frameY = -1)
        {
            id = _id;
            w = _w;
            h = _h;
            style = _style;
            frameX = _frameX;
            frameY = _frameY;
        }

        public override string ToString()
        {
            return $"id={id},style={style},w={w},h={h}";
        }
    }
}
