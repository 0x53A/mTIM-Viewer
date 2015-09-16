using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mTIM.Graphics
{
    [ProtoContract]
    [ProtoInclude(1, typeof(TriangulatedGeometryData))]
    public abstract class GeometryData : IEquatable<GeometryData>, IComparable<GeometryData>
    {
        public abstract bool Equals(GeometryData other);
        public abstract int CompareTo(GeometryData other);
    }

    [ProtoContract]
    public class Transform : IEquatable<Transform>, IComparable<Transform>
    {
        [ProtoMember(1, IsRequired = true)]
        public float[] Matrix { get; set; }

        public override bool Equals(object obj)
        {
            var otherTransform = obj as Transform;
            if (false == Object.ReferenceEquals(otherTransform, null))
                return this.Equals(otherTransform);
            else
                return false;
        }

        public bool Equals(Transform other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            return this.Matrix.SequenceEqual(other.Matrix);
        }

        public int CompareTo(Transform other)
        {
            if (Object.ReferenceEquals(other, null)) return 1;
            for (int i = 0; i < this.Matrix.Length; i++)
            {
                var c = this.Matrix[i].CompareTo(other.Matrix[i]);
                if (c != 0)
                    return c;
            }
            return 0;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var f in Matrix)
                unchecked { hash = hash * 486187739 + f.GetHashCode(); }
            return hash;
        }
    }

    [ProtoContract]
    public struct XYZ : IEquatable<XYZ>, IComparable<XYZ>
    {
        [ProtoMember(1, IsRequired = true)]
        public float X { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public float Y { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public float Z { get; set; }

        bool IEquatable<XYZ>.Equals(XYZ other)
        {
            return (this.X == other.X) &&
                    (this.Y == other.Y) &&
                    (this.Z == other.Z);
        }

        int IComparable<XYZ>.CompareTo(XYZ other)
        {
            var cx = this.X.CompareTo(other.X);
            if (cx == 0)
            {
                var cy = this.Y.CompareTo(other.Y);
                if (cy == 0)
                {
                    return this.Z.CompareTo(other.Z);
                }
                else return cy;
            }
            else return cx;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 486187739 + X.GetHashCode();
                hash = hash * 486187739 + Y.GetHashCode();
                hash = hash * 486187739 + Z.GetHashCode();
            }
            return hash;
        }
    }

    [ProtoContract]
    public class Triangle : IEquatable<Triangle>, IComparable<Triangle>
    {
        [ProtoMember(1, IsRequired = true)]
        public uint A { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public uint B { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public uint C { get; set; }

        bool IEquatable<Triangle>.Equals(Triangle other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            return this.A == other.A &&
                    this.B == other.B &&
                    this.C == other.C;
        }

        int IComparable<Triangle>.CompareTo(Triangle other)
        {
            if (Object.ReferenceEquals(other, null)) return 1;
            var ca = this.A.CompareTo(other.A);
            if (ca == 0)
            {
                var cb = this.B.CompareTo(other.B);
                if (cb == 0)
                {
                    var cc = this.C.CompareTo(other.C);
                    return cc;
                }
                else return cb;
            }
            else return ca;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 486187739 + A.GetHashCode();
            hash = hash * 486187739 + B.GetHashCode();
            hash = hash * 486187739 + C.GetHashCode();
            return hash;
        }
    }

    [ProtoContract]
    public class Line : IEquatable<Line>, IComparable<Line>
    {
        [ProtoMember(1, IsRequired = true)]
        public uint A { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public uint B { get; set; }

        bool IEquatable<Line>.Equals(Line other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            return this.A == other.A &&
                    this.B == other.B;
        }

        int IComparable<Line>.CompareTo(Line other)
        {
            if (Object.ReferenceEquals(other, null)) return 1;
            var ca = this.A.CompareTo(other.A);
            if (ca == 0)
            {
                var cb = this.B.CompareTo(other.B);
                return cb;
            }
            else return ca;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 486187739 + A.GetHashCode();
            hash = hash * 486187739 + B.GetHashCode();
            return hash;
        }
    }

    [ProtoContract]
    public class TriangulatedGeometryData : GeometryData, IEquatable<TriangulatedGeometryData>, IComparable<TriangulatedGeometryData>
    {
        [ProtoMember(1, IsRequired = true)]
        public Triangle[] Triangles { get; set; }
        [ProtoMember(2, IsRequired = false)]
        public Line[] Lines { get; set; }

        public bool Equals(TriangulatedGeometryData other)
        {
            if (this.Triangles.Length != other.Triangles.Length)
                return false;

            var thisLinesNull = Object.ReferenceEquals(null, this.Lines);
            var otherLinesNull = Object.ReferenceEquals(null, other.Lines);
            if (thisLinesNull != otherLinesNull)
                return false;
            if (false == thisLinesNull)
            {
                if (this.Lines.Length != other.Lines.Length)
                    return false;
                if (false == this.Lines.SequenceEqual(other.Lines))
                    return false;
            }
            return this.Triangles.SequenceEqual(other.Triangles);
        }

        public int CompareTo(TriangulatedGeometryData other)
        {
            if (Object.ReferenceEquals(other, null)) return 1;

            // compare length
            if (this.Triangles.Length != other.Triangles.Length)
                return this.Triangles.Length.CompareTo(other.Triangles.Length);

            // length equal, compare elements
            for (int i = 0; i < Triangles.Length; i++)
            {
                var c = ((IComparable<Triangle>)this.Triangles[i]).CompareTo(other.Triangles[i]);
                if (c != 0)
                    return c;
            }

            // elements equal
            return 0;
        }

        public override bool Equals(GeometryData other)
        {
            var otherTriangulated = other as TriangulatedGeometryData;
            if (otherTriangulated != null)
            {
                return this.Equals(otherTriangulated);
            }
            else return false;
        }

        public override int CompareTo(GeometryData other)
        {
            var otherTriangulated = other as TriangulatedGeometryData;
            if (otherTriangulated != null)
            {
                return this.CompareTo(otherTriangulated);
            }
            else return 1;
        }

        public override bool Equals(object obj)
        {
            var otherTriangulated = obj as TriangulatedGeometryData;
            if (otherTriangulated != null)
            {
                return ((IEquatable<TriangulatedGeometryData>)this).Equals(otherTriangulated);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var t in Triangles)
                unchecked { hash = hash * 486187739 + t.GetHashCode(); }
            return hash;
        }
    }

    [ProtoContract]
    public class GeometryReference
    {
        /// <summary>
        /// Index into Result.Geometries
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public uint GeometryIndex { get; set; }
        /// <summary>
        /// A (optional) transform applied to the geometry.
        /// </summary>
        [ProtoMember(2)]
        public Transform Transform { get; set; }
        /// <summary>
        /// Defines how the geometry should be painted.
        /// </summary>
        [ProtoMember(3, IsRequired = true)]
        public string ColorTag { get; set; }
        /// <summary>
        /// In which Layer this Geometry is.
        /// (Concrete, Reinforcement, ...)
        /// </summary>
        [ProtoMember(4, IsRequired = true)]
        public string LayerTag { get; set; }
    }

    [ProtoContract]
    public class Visualizable
    {
        /// <summary>
        /// The ID of the Element, referenced from externally
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public Guid Id { get; set; }
        /// <summary>
        /// The Geometries which form this Visualizable.
        /// </summary>
        [ProtoMember(2, IsRequired = true)]
        public GeometryReference[] Geometries { get; set; }
    }

    [ProtoContract]
    public class Result
    {
        /// <summary>
        /// All Precast Elements.
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public Visualizable[] Elements { get; set; }
        /// <summary>
        /// The geometries of the Elements.
        /// </summary>
        [ProtoMember(2, IsRequired = true)]
        public GeometryData[] Geometries { get; set; }
        /// <summary>
        /// A Vertex List shared by all Geometries.
        /// </summary>
        [ProtoMember(3, IsRequired = true)]
        public XYZ[] PointsAndVectors { get; set; }


        public static string GetProtoDefinition()
        {
            return ProtoBuf.Serializer.GetProto<Result>();
        }
    }


    //####################################
    //## Just examples, not implemented ##
    //####################################


    public abstract class Node
    {
        /// <summary>
        /// optional
        /// </summary>
        public Node Parent { get; set; }

        /// <summary>
        /// optional
        /// </summary>
        public IList<Node> Children { get; set; }
    }

    public class Leaf : Node
    {
        public IList<PrecastElementRef> Elements { get; set; }
    }

    public class PrecastElementRef
    {
        public Guid PrecastElementId { get; set; }
        public Transform Transform { get; set; }
    }

    public class ReinforcementGeometryData : GeometryData
    {
        public XYZ[] PolyLine3D { get; set; }
        public float Diameter { get; set; }
        public float BendingRadius { get; set; }

        public override bool Equals(GeometryData other)
        {
            throw new NotImplementedException();
        }

        public override int CompareTo(GeometryData other)
        {
            throw new NotImplementedException();
        }
    }
}
