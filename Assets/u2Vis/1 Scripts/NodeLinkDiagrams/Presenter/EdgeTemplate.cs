using UnityEngine;

namespace u2vis.NodeLink
{
    public class EdgeTemplate : MonoBehaviour
    {
        [SerializeField]
        private Material _material = null;
        [SerializeField]
        private float _scale = 1.0f;

        public Material Material { get { return _material; } }
        public float Scale { get { return _scale; } }
    }
}
