using UnityEngine;

namespace u2vis.NodeLink
{
    public class NodeTemplate : MonoBehaviour
    {
        [SerializeField]
        private Mesh _mesh = null;
        [SerializeField]
        private Material _material = null;
        [SerializeField]
        private float _scale = 1.0f;

        public Mesh Mesh { get { return _mesh; } }
        public Material Material { get { return _material; } }
        public float Scale { get { return _scale; } }
    }
}
