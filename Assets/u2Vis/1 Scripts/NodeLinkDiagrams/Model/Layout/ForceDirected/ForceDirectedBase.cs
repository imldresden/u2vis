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
    public abstract class ForceDirectedBase : IGraphLayout
    {
        protected Dictionary<int, PointFD> m_nodePoints;
        protected Dictionary<int, SpringFD> m_edgeSprings;

        #region Public Properties
        public float Stiffness { get; set; }
        public float Repulsion { get; set; }
        public float Damping { get; set; }
        public float Threshold { get; set; }
        public bool WithinThreshold { get; protected set; }
        public IGraph<INode, IEdge<INode>> Graph { get; protected set; }
        #endregion

               
        public ForceDirectedBase(IGraph<INode, IEdge<INode>> graph, float stiffness, float repulsion, float damping)
        {
            Graph = graph;
            Stiffness = stiffness;
            Repulsion = repulsion;
            Damping = damping;
            m_nodePoints = new Dictionary<int, PointFD>();
            m_edgeSprings = new Dictionary<int, SpringFD>();

            Threshold = 0.01f;
        }

    
        #region Protected
        protected void applyCoulombsLaw(bool twoD = false)
        {
            foreach (var n1 in Graph.Nodes)
            {
                PointFD point1 = GetPoint(n1);
                foreach (var n2 in Graph.Nodes)
                {
                    PointFD point2 = GetPoint(n2);
                    if (point1 != point2)
                    {
                        Vector3 d = point1.Position - point2.Position;
                        float distance = d.magnitude + 0.1f;
                        Vector3 direction = d.normalized;
                        if (twoD)
                            direction.z = 0;
                        if (point1.Pinned && point2.Pinned)
                        {
                            point1.ApplyForce(direction * 0.0f);
                            point2.ApplyForce(direction * 0.0f);
                        }
                        else if (point1.Pinned)
                        {
                            point1.ApplyForce(direction * 0.0f);
                            //point2.ApplyForce((direction * Repulsion) / (distance * distance * -1.0f));
                            point2.ApplyForce((direction * Repulsion) / (distance * -1.0f));
                        }
                        else if (point2.Pinned)
                        {
                            //point1.ApplyForce((direction * Repulsion) / (distance * distance));
                            point1.ApplyForce((direction * Repulsion) / (distance));
                            point2.ApplyForce(direction * 0.0f);
                        }
                        else
                        {
                            //                             point1.ApplyForce((direction * Repulsion) / (distance * distance * 0.5f));
                            //                             point2.ApplyForce((direction * Repulsion) / (distance * distance * -0.5f));
                            point1.ApplyForce((direction * Repulsion) / (distance * 0.5f));
                            point2.ApplyForce((direction * Repulsion) / (distance * -0.5f));
                        }
                    }
                }
            }
        }

        protected void applyHookesLaw(bool twoD = false)
        {
            foreach (var e in Graph.Edges)
            {
                SpringFD spring = GetSpring(e);
                Vector3 d = spring.point2.Position - spring.point1.Position;
                float displacement = spring.Length - d.magnitude;
                Vector3 direction = d.normalized;
                if (twoD)
                    direction.z = 0;

                if (spring.point1.Pinned && spring.point2.Pinned)
                {
                    spring.point1.ApplyForce(direction * 0.0f);
                    spring.point2.ApplyForce(direction * 0.0f);
                }
                else if (spring.point1.Pinned)
                {
                    spring.point1.ApplyForce(direction * 0.0f);
                    spring.point2.ApplyForce(direction * (spring.K * displacement));
                }
                else if (spring.point2.Pinned)
                {
                    spring.point1.ApplyForce(direction * (spring.K * displacement * -1.0f));
                    spring.point2.ApplyForce(direction * 0.0f);
                }
                else
                {
                    spring.point1.ApplyForce(direction * (spring.K * displacement * -0.5f));
                    spring.point2.ApplyForce(direction * (spring.K * displacement * 0.5f));
                }
            }
        }

        protected void attractToCentre(bool twoD = false)
        {
            foreach (var n in Graph.Nodes)
            {
                PointFD point = GetPoint(n);
                Vector3 direction = point.Position * -1.0f;
                float displacement = direction.magnitude;
                direction = direction.normalized;
                if (twoD)
                    direction.z = 0;
                if (!point.Pinned)
                {
                    point.ApplyForce(direction * (Stiffness * displacement * 0.4f));
                }
            }
        }

        protected void updateVelocity(float timeStep)
        {
            foreach (var n in Graph.Nodes)
            {
                PointFD point = GetPoint(n);
                point.Velocity += point.Acceleration * timeStep;
                point.Velocity *= Damping;
                point.Acceleration = Vector3.zero;
            }
        }

        protected void updatePosition(float timeStep)
        {
            foreach (var n in Graph.Nodes)
            {
                PointFD point = GetPoint(n);
                point.Position += point.Velocity * timeStep*0.01f;
            }
        }

        protected float getTotalEnergy()
        {
            float energy = 0.0f;
            foreach (var n in Graph.Nodes)
            {
                PointFD point = GetPoint(n);
                float speed = point.Velocity.magnitude;
                energy += 0.5f * point.Mass * speed * speed;
            }
            return energy;
        }
        #endregion

        //calculate the applied laws to point of Model
        public void Calculate(float iTimeStep) // time in second
        {
            applyCoulombsLaw();
            applyHookesLaw();
            attractToCentre();
            updateVelocity(iTimeStep);
            updatePosition(iTimeStep);
            if (getTotalEnergy() < Threshold)
            {
                WithinThreshold = true;
            }
            else
                WithinThreshold = false;
        }

        //gives a dict of calculated data, to return to presenter
        public Dictionary<int,Vector3> ApplyCalculation()
        {
            Dictionary<int, Vector3> dict = new Dictionary<int, Vector3>();
            foreach (var n in Graph.Nodes)
            {
                PointFD point = GetPoint(n);
                dict.Add(point.Node.Uid,point.Position);
                //model should be updated here. 
                //point.Node.PosX = point.Position.x;
                //point.Node.PosY = point.Position.y;
                //point.Node.PosZ = point.Position.z;
            }
            return dict;
        }

        //pin a node, so now physics get applied
        public void PinNode(INode node, bool pin)
        {
            PointFD point = GetPoint(node);
            point.Pinned = pin;
        }

        public void UpdatePositionInteracted(INode node, Vector3 position)
        {
            PointFD point = GetPoint(node);
            point.Position = position;
        }
        //protected NearestPoint Nearest(Vector3 position)
        //{
        //    NearestPoint min = new NearestPoint();
        //    foreach (var n in Graph.Nodes)
        //    {
        //        PointFD point = GetPoint(n);
        //        float distance = (point.Position - position).magnitude;
        //        if (min.distance == null || distance < min.distance)
        //        {
        //            min.node = n;
        //            min.point = point;
        //            min.distance = distance;
        //        }
        //    }
        //    return min;
        //}

        //get point in model associated to node of graph of node link
        public virtual PointFD GetPoint(INode node)
        {
            if (!(m_nodePoints.ContainsKey(node.Uid)))
            {
                Vector3 iniPosition = new Vector3(node.PosX, node.PosY, node.PosZ);
                m_nodePoints[node.Uid] = new PointFD(iniPosition, Vector3.zero, Vector3.zero, node);
                m_nodePoints[node.Uid].SetMass(1);
            }
            return m_nodePoints[node.Uid];
        }

        //get spring associated to edge
        public SpringFD GetSpring(IEdge<INode> edge)
        {
            //if (!(m_edgeSprings.ContainsKey(edge.Uid)))
            //{
            //    float length = GetEdgeLength(edge);
            //    SpringFD existingSpring = null;

            //    List<IEdge<INode>> fromEdges = (SimpleGraph)Graph.GetEdges(edge.Source, edge.Target);
            //    if (fromEdges != null)
            //    {
            //        foreach (Edge e in fromEdges)
            //        {
            //            if (existingSpring == null && m_edgeSprings.ContainsKey(e.ID))
            //            {
            //                existingSpring = m_edgeSprings[e.ID];
            //                break;
            //            }
            //        }

            //    }
            //    if (existingSpring != null)
            //    {
            //        return new SpringFD(existingSpring.point1, existingSpring.point2, 0.0f, 0.0f);
            //    }

            //    List<Edge> toEdges = Graph.GetEdges(edge.Target, edge.Source);
            //    if (toEdges != null)
            //    {
            //        foreach (Edge e in toEdges)
            //        {
            //            if (existingSpring == null && m_edgeSprings.ContainsKey(e.ID))
            //            {
            //                existingSpring = m_edgeSprings[e.ID];
            //                break;
            //            }
            //        }
            //    }

            //    if (existingSpring != null)
            //    {
            //        return new SpringFD(existingSpring.point2, existingSpring.point1, 0.0f, 0.0f);
            //    }
            //    m_edgeSprings[edge.ID] = new SpringFD(GetPoint(edge.Source), GetPoint(edge.Target), length, Stiffness);

            //}
            //return m_edgeSprings[edge.Uid];
            SpringFD spring;
            if (!m_edgeSprings.TryGetValue(edge.Uid, out spring))
            {
                spring = new SpringFD(GetPoint(edge.Source), GetPoint(edge.Target), GetEdgeLength(edge), Stiffness);
                m_edgeSprings.Add(edge.Uid, spring);
            }
            return m_edgeSprings[edge.Uid];
        }

        public void LayoutGraph(IGraph<INode, IEdge<INode>> graph)
        {
            throw new System.NotImplementedException();
        }

        //public abstract BoundingBox GetBoundingBox();

        #region Static
        public static float GetEdgeLength(IEdge<INode> edge)
        {
            return (new Vector3(edge.Source.PosX, edge.Source.PosY, edge.Source.PosZ) - new Vector3(edge.Target.PosX, edge.Target.PosY, edge.Target.PosZ)).magnitude;
        }
        #endregion

        #region Nested
        protected class NearestPoint
        {
            public NearestPoint()
            {
                node = null;
                point = null;
                distance = null;
            }
            public INode node;
            public PointFD point;
            public float? distance;
        }

        public class BoundingBox
        {
            public static float defaultBB = 2.0f;
            public static float defaultPadding = 0.07f; // ~5% padding

            public BoundingBox()
            {
                topRightBack = Vector3.zero;
                bottomLeftFront = Vector3.zero;
            }
            public Vector3 topRightBack;
            public Vector3 bottomLeftFront;
        }
        #endregion
    }
}