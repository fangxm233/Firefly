using System.Numerics;

namespace FireflyUtility.Structure
{
    public class Mesh
    {
        public string Name;
        public Vertex[] Vertices;
        public int[] Triangles;

        public Mesh(string Name, Vertex[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
            ExpandVertices();
            CalculateTangent();
        }

        private void ExpandVertices()
        {
            Vertex[] vertices = new Vertex[Triangles.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vertices[Triangles[i]];
                Triangles[i] = i;
            }
            Vertices = vertices;
        }

        /// 计算顶点切线
        private void CalculateTangent()
        {
            for (int i = 0; i < Triangles.Length - 2; i += 3)
            {
                Vertex ver0 = Vertices[Triangles[i]];
                Vertex ver1 = Vertices[Triangles[i + 1]];
                Vertex ver2 = Vertices[Triangles[i + 2]];

                Vector3 E0 = ver1.Point - ver0.Point;
                Vector3 E1 = ver2.Point - ver0.Point;

                Vector2 uv1 = ver1.UV - ver0.UV;
                Vector2 uv2 = ver2.UV - ver0.UV;

                float w = 1 / (uv1.X * uv2.Y - uv1.Y * uv2.Y);

                Vector3 t = new Vector3
                {
                    X = (uv2.Y * E0.X - uv1.Y * E1.X) * w,
                    Y = (uv2.Y * E0.Y - uv1.Y * E1.Y) * w,
                    Z = (uv2.Y * E0.Z - uv1.Y * E1.Z) * w
                };
                Vector3 b = new Vector3
                {
                    X = (uv1.X * E1.X - uv2.X * E0.X) * w,
                    Y = (uv1.X * E1.Y - uv2.X * E0.Y) * w,
                    Z = (uv1.X * E1.Z - uv2.X * E0.Z) * w
                };

                ver0.Tangent = Vector3.Normalize(t);
                ver1.Tangent = Vector3.Normalize(t);
                ver2.Tangent = Vector3.Normalize(t);
                ver0.Bitangent = Vector3.Normalize(b);
                ver1.Bitangent = Vector3.Normalize(b);
                ver2.Bitangent = Vector3.Normalize(b);
                Vertices[Triangles[i]] = ver0;
                Vertices[Triangles[i + 1]] = ver1;
                Vertices[Triangles[i + 2]] = ver2;
            }
        }

        public Vertex GetPoint(int i) => Vertices[Triangles[i]];
    }
}
