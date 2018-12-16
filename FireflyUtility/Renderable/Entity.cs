using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FireflyUtility.Structure;
using FireflyUtility.Math;

namespace FireflyUtility.Renderable
{
    public class Entity
    {
        public Vector3 Position, PositionToCamera, Rotation, RotationToCamera;
        public Mesh Mesh;

        private Matrix4x4 _matrix;

        public Entity(Vector3 position, Vector3 rotation, Mesh mesh)
        {
            Position = position;
            PositionToCamera = position;
            Rotation = rotation;
            RotationToCamera = rotation;
            Mesh = mesh;
        }

        public void CalculateMatrix() => 
            _matrix = Matrix4x4.Transpose(Mathf.GetRotationMatrix(Rotation) * Matrix4x4.CreateTranslation(Position));

        public Vertex ToWorld(Vertex v) => new Vertex() { Point = Mathf.MulVector3AndMatrix4x4(v.Point, _matrix), Color = v.Color };
    }
}
