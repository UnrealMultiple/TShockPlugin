namespace WorldModify
{
    public class WallProp
    {
        public int id = 0;

        public string name;

        public string color;

        public string FullColor => "#FF" + color;

        public string Desc => $"{name}({id})";

        public override string ToString()
        {
            return $"{id},{name},{FullColor}";
        }
    }
}
