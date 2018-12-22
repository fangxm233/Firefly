namespace FireflyUtility.Structure
{
    public class Mesh
    {
        public string Name;
        public Vertex[] Vertices;
        public int[] Triangles;

        public Mesh() { }

        public Mesh(Vertex[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }

        public Vertex GetPoint(int i) => Vertices[Triangles[i]];
    }
}
