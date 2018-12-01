using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Firefly.Render.Structure;

namespace Firefly.Render.Renderable
{
    public class Entity
    {
        public Vector3 Position, PositionToCamera, Rotation, RotationToCamera;
        public Mesh Mesh;

        private Matrix4x4 matrix;

        public Entity(Vector3 position, Vector3 rotation, Mesh mesh)
        {
            Position = position;
            PositionToCamera = position;
            Rotation = rotation;
            RotationToCamera = rotation;
            Mesh = mesh;
        }

        public void CalculateMatrix() => 
            matrix = Matrix4x4.Transpose(Math.Mathf.GetRotationMatrix(Rotation) * Matrix4x4.CreateTranslation(Position));

        public Vertex ToWorld(Vertex v) => new Vertex() { Point = Math.Mathf.MulVector3AndMatrix4x4(v.Point, matrix), Color = v.Color };
    }
}
