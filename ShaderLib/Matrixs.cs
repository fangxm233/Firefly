using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace ShaderLib
{
    public class Matrixs
    {
        /// 模型*观察*投影矩阵，用于将顶点/方向矢量从模型空间变换到裁切空间
        public static Matrix4x4 MVP;
        /// 模型*观察矩阵，用于将顶点/方向矢量从模型空间变换到观察空间
        public static Matrix4x4 MV;
        /// 观察矩阵，用于将顶点/方向矢量从世界空间变换到观察空间
        public static Matrix4x4 V;
        /// 投影矩阵，用于将顶点/方向矢量从观察空间变换到裁切空间
        public static Matrix4x4 P;
        /// 观察*投影矩阵， 用于将顶点/方向矢量从世界空间变换到裁切空间
        public static Matrix4x4 VP;
        /// MV的转置矩阵
        public static Matrix4x4 T_MV;
        /// MV的逆转置矩阵，用于将法线从模型空间变换到观察空间
        public static Matrix4x4 IT_MV;
        /// 模型矩阵，用于将顶点/法线从模型空间变换到世界空间
        public static Matrix4x4 Entity2World;
        /// Entity2World的逆矩阵，用于将顶点/法线从世界空间变换到模型空间
        public static Matrix4x4 World2Entity;

        public static Matrix4x4 O;

        public static Vector3 CameraPosition, CameraRotation;
        public static Vector3 EntityPosition, EntityRotation;
        public static float FOV, Aspect, Near, Far, Size;
        public static float Width, Height;

        public static void CalculateMatrixs()
        {
            Entity2World = Matrix4x4.Transpose(GetRotationMatrix(EntityRotation) * Matrix4x4.CreateTranslation(EntityPosition));
            Matrix4x4.Invert(Entity2World, out World2Entity);
            V = Matrix4x4.Transpose(GetRotationMatrix(CameraRotation) * Matrix4x4.CreateTranslation(CameraPosition));
            P = PerspectiveFieldOfView(FOV, Aspect, Near, Far);
            //P = Matrix4x4.Transpose(Matrix4x4.CreatePerspectiveFieldOfView(FOV, Aspect, Near, Far));
            O = GetOrthographic(Size, Aspect, Near, Far);
            //P = O;
            MV = V * Entity2World;
            T_MV = Matrix4x4.Transpose(MV);
            Matrix4x4.Invert(T_MV, out IT_MV);
            VP = P * V;
            MVP = P * (V * Entity2World);
        }

        private static Matrix4x4 GetRotationMatrix(Vector3 v) =>
            Matrix4x4.CreateRotationY(v.Y) * (Matrix4x4.CreateRotationX(v.X) * Matrix4x4.CreateRotationZ(v.Z));

        private static Matrix4x4 PerspectiveFieldOfView(float fov, float aspect, float near, float far) => 
            new Matrix4x4(MathF.Atan(fov / 2) / aspect, 0, 0, 0, 0, MathF.Atan(fov / 2), 0, 0, 
                0, 0, -(far + near) / (far - near), -(2 * near * far) / (far - near), 0, 0, -1, 0);

        private static Matrix4x4 GetOrthographic(float size, float aspect, float near, float far) =>
            new Matrix4x4(1 / (aspect * size), 0, 0, 0, 0, 1 / size, 0, 0,
                0, 0, -2 / (far - near), -(far + near) / (far - near), 0, 0, 0, 1);
    }
}
