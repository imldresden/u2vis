using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace u2vis.NodeLink
{
    public class BaseNodePresenter
    {
        public INode _nodeModel;
        public int Uid { get { return _nodeModel.Uid; } }
        public Vector3 Position { get; set; }
        public Vector3 FirstPosition { get; set; }
        public float Size { get; set; }
        public float FirstSize { get; set; }
		public bool HasVis { get; set; }

        public string Label { get; set; }

        public List<BaseEdgePresenter> targets = new List<BaseEdgePresenter>();
        public List<BaseEdgePresenter> sources = new List<BaseEdgePresenter>();

        public Text labelText;

        public int copyID;

        public bool selected;

        public BaseNodePresenter(INode nodeModel)
        {
            _nodeModel = nodeModel;
            Position = new Vector3(nodeModel.PosX, nodeModel.PosY, nodeModel.PosZ);
            FirstPosition = new Vector3(nodeModel.PosX, nodeModel.PosY, nodeModel.PosZ);
            Label = nodeModel.Label;
            Size = 1f;
            FirstSize = 1f;
            copyID = Uid;

            nodeModel.Updated += NodeModel_Updated;
        }

        private void NodeModel_Updated(object sender, EventArgs e)
        {
            var model = sender as INode;
            Position = new Vector3(model.PosX, model.PosY, model.PosZ);
        }

        public override int GetHashCode() { return copyID; }
        public override bool Equals(object obj)
        {
            return ((BaseNodePresenter)obj).copyID == this.copyID;
        }
    }
}
