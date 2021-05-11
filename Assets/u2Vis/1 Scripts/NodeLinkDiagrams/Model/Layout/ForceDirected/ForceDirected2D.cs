/*! 
@file ForceDirected.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/epForceDirectedGraph.cs>
@date August 08, 2013
@brief ForceDirected Interface
@version 1.0

@section LICENSE

The MIT License (MIT)

Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

@section DESCRIPTION

An Interface for the ForceDirected Class.

*/
using System.Collections.Generic;
using UnityEngine;

namespace u2vis.NodeLink
{
    public class ForceDirected2D : ForceDirectedBase
    {
        public ForceDirected2D(IGraph<INode, IEdge<INode>> graph, float stiffness, float repulsion, float damping)
            : base(graph, stiffness, repulsion, damping)
        {

        }

        //gives point in graph model for the interacted node of actual graph
        public override PointFD GetPoint(INode node)
        {
            if (!(m_nodePoints.ContainsKey(node.Uid)))
            {
                Vector3 iniPosition = new Vector3(node.PosX, node.PosY, 0);
                m_nodePoints[node.Uid] = new PointFD(iniPosition, Vector3.zero, Vector3.zero, node);
                m_nodePoints[node.Uid].SetMass(1);
            }
            return m_nodePoints[node.Uid];
        }

        //calculate the forces acting on point
        public new void Calculate(float iTimeStep) // time in second
        {
            applyCoulombsLaw(true);
            applyHookesLaw(true);
            attractToCentre(true);
            updateVelocity(iTimeStep);
            updatePosition(iTimeStep);
            if (getTotalEnergy() < Threshold)
            {
                WithinThreshold = true;
            }
            else
                WithinThreshold = false;
        }

    }
}
