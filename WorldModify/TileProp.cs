namespace WorldModify
{
    public class TileProp
    {
        public int id = 0;

        public int w = 1;

        public int h = 1;

        public bool isFrame = false;

        public string name = "";

        public string color = "";

        public List<FrameProp> frames = new List<FrameProp>();

        public void Add(int frameX, int frameY, string name = "", string variety = "")
        {
            if (!string.IsNullOrEmpty(variety))
            {
                variety = "(" + variety + ")";
            }
            frames.Add(new FrameProp
            {
                style = frames.Count,
                frameX = frameX,
                frameY = frameY,
                name = name + variety
            });
        }
    }
}
