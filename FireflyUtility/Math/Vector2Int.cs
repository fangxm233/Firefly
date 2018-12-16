namespace FireflyUtility.Math
{
    public struct Vector2Int
    {
        public int X, Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public float Magnitude() => (float)System.Math.Sqrt(X * X + Y * Y);

        public override string ToString() => "< " + X + ", " + Y + " >";
    }
}