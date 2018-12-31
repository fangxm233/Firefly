using FireflyUtility.Math;
using FireflyUtility.Structure;
using System.Numerics;

namespace FireflyUtility.Renderable
{
    public class Entity
    {
        public string Name;
        public Vector3 Position, Rotation;
        public Mesh Mesh;
        public Material Material;

        private Matrix4x4 _matrix;
        
        public Entity(Vector3 position, Vector3 rotation, Mesh mesh)
        {
            Position = position;
            Rotation = rotation;
            Mesh = mesh;
        }

        public Entity(string name, Vector3 position, Vector3 rotation, Mesh mesh, Material material)
        {
            Name = name;
            Position = position;
            Rotation = rotation;
            Mesh = mesh;
            Material = material;
        }

        private void CalculateNormal()
        {
            Vertex[] vertices = new Vertex[Mesh.Triangles.Length];
            int[] triangles = new int[Mesh.Triangles.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                triangles[i] = i;
                vertices[i] = Mesh.Vertices[Mesh.Triangles[i]];
            }

            for (int i = 0; i + 2 < triangles.Length; i += 3)
            {
                Vector3 normal = Vector3.Cross(vertices[i + 1].Point - vertices[i].Point, vertices[i + 2].Point - vertices[i + 1].Point);
                vertices[i].Normal = normal;
                vertices[i + 1].Normal = normal;
                vertices[i + 2].Normal = normal;
            }

            Mesh.Triangles = triangles;
            Mesh.Vertices = vertices;
        }

        public void CalculateMatrix() => 
            _matrix = Matrix4x4.Transpose(Mathf.GetRotationMatrix(Rotation) * Matrix4x4.CreateTranslation(Position));

        public Vertex ToWorld(Vertex v) => new Vertex() { Point = Mathf.MulVector3AndMatrix4x4(v.Point, _matrix), Color = v.Color };
    }
}
