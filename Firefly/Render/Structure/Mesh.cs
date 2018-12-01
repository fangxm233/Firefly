﻿namespace Firefly.Render.Structure
{
    public class Mesh
    {
        public Vertex[] Vertices;
        public int[] Triangles;

        public Mesh(Vertex[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }

        public Vertex GetPoint(int i) => Vertices[Triangles[i]];
    }
}