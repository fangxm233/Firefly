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

        public void CalculateMatrix() => 
            _matrix = Matrix4x4.Transpose(Mathf.GetRotationMatrix(Rotation) * Matrix4x4.CreateTranslation(Position));

        public Vertex ToWorld(Vertex v) => new Vertex() { Point = Mathf.MulVector3AndMatrix4x4(v.Point, _matrix), Color = v.Color };
    }
}
