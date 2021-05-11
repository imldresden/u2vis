/*! 
@file Point.cs
@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
		<http://github.com/juhgiyo/epForceDirectedGraph.cs>
@date August 08, 2013
@brief Point Interface
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

An Interface for the Point Class.

*/
using UnityEngine;

namespace u2vis.NodeLink
{
    public class PointFD
    {
        public Vector3 Position { get; set; }
        public INode Node { get; private set; }
        public float Mass { get; private set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public bool Pinned { get; set; }

        public PointFD(Vector3 position, Vector3 velocity, Vector3 acceleration, INode node)
        {
            Position = position;
            Node = node;
            Velocity = velocity;
            Acceleration = acceleration;
            Pinned = false;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;
            PointFD p = obj as PointFD;
            if ((System.Object)p == null)
                return false;
            return Position == p.Position;
        }

        public bool Equals(PointFD p)
        {
            if ((object)p == null)
                return false;
            return Position == p.Position;
        }

        public static bool operator ==(PointFD a, PointFD b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return (a.Position == b.Position);
        }

        public static bool operator !=(PointFD a, PointFD b)
        {
            return !(a == b);
        }

        public void ApplyForce(Vector3 force)
        {
            Acceleration += force / Mass;
        }

        //Warum ist Mass private Set? Das hier macht vll keinen sinn, für testen.
        public void SetMass (float mass)
        {
            Mass = mass;
        }
    }
}
