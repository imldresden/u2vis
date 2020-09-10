using System.Collections.Generic;
using UnityEngine;

namespace u2vis.Utilities
{
    public class IntermediateMesh
    {
        public readonly List<Vector3> Vertices;
        public readonly List<Vector3> Normals;
        public readonly List<Vector2> TexCoords;
        public readonly List<Color> Colors;
        public readonly List<int> Indices;

        public IntermediateMesh()
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            TexCoords = new List<Vector2>();
            Colors = new List<Color>();
            Indices = new List<int>();
        }

        public Mesh GenerateMesh(string name, MeshTopology topology)
        {
            var mesh = new Mesh();
            mesh.name = name;
            mesh.vertices = Vertices.ToArray();
            mesh.normals = Normals.ToArray();
            mesh.uv = TexCoords.ToArray();
            mesh.colors = Colors.ToArray();
            mesh.SetIndices(Indices.ToArray(), topology, 0);
            return mesh;
        }
    }
}
