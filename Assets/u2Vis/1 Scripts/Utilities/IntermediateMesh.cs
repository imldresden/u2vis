using System.Collections.Generic;
using UnityEngine;

namespace u2vis.Utilities
{
    /// <summary>
    /// Class that can be used for intermediate storage of mesh compontnets (vertices, normals, etc.).
    /// All components can be appended, so that this class can be used during calculations or within loops.
    /// A normal unity mesh can be constructed from the intermediate mesh.
    /// </summary>
    public class IntermediateMesh
    {
        /// <summary>
        /// A list of all vertices of the mesh.
        /// </summary>
        public readonly List<Vector3> Vertices;
        /// <summary>
        /// A list of all normals of the mesh.
        /// </summary>
        public readonly List<Vector3> Normals;
        /// <summary>
        /// a list of all texture coordinates of the mesh.
        /// </summary>
        public readonly List<Vector2> TexCoords;
        /// <summary>
        /// A list of all vertex colors of the mesh.
        /// </summary>
        public readonly List<Color> Colors;
        /// <summary>
        /// The index list of the mesh.
        /// </summary>
        public readonly List<int> Indices;
        /// <summary>
        /// Create a new instance if IntermediateMesh,
        /// </summary>
        public IntermediateMesh()
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            TexCoords = new List<Vector2>();
            Colors = new List<Color>();
            Indices = new List<int>();
        }
        /// <summary>
        /// Generate a mesh of out of the currently stored mesh components.
        /// </summary>
        /// <param name="name">Name of the generated mesh.</param>
        /// <param name="topology">The topology of the generate mesh (triangles, lines, points, etc.)</param>
        /// <returns>The generated mesh.</returns>
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
        /// <summary>
        /// Implicit conversion of this IntermediateMesh to a standard Unity mesh.
        /// The mesh topology is assumed to be 'triangles', the name is set to 'unnamed'.
        /// </summary>
        /// <param name="m">The IntermediateMesh that is converted.</param>
        public static implicit operator Mesh(IntermediateMesh m) => m.GenerateMesh("unnamed", MeshTopology.Triangles);
    }
}
