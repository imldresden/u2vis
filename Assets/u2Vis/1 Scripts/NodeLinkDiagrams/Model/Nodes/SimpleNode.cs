using System;

namespace u2vis.NodeLink
{
    public class SimpleNode: INode
    {
        private float _posX;
        private float _posY;
        private float _posZ;

        public int Uid { get; }
        public float PosX
        {
            get { return _posX; }
            set
            {
                _posX = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
        public float PosY
        {
            get { return _posY; }
            set
            {
                _posY = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
        public float PosZ
        {
            get { return _posZ; }
            set
            {
                _posZ = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
        public string Label { get; set; }

        public event EventHandler Updated;

        public SimpleNode(int uid, float x, float y, float z, string label)
        {
            Uid = uid;
            _posX = x;
            _posY = y;
            _posZ = z;
            Label = label;
        }
    }
}
