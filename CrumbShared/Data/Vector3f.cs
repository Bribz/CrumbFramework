using System;
using System.Collections.Generic;
using System.Text;

using MessagePack;

namespace CrumbShared
{
    [MessagePackObject]
    public class Vector3f
    {
        [Key(0)]
        private readonly float[] values;
        [IgnoreMember]
        public float X { get { return values[0]; } set { values[0] = value; } }
        [IgnoreMember]
        public float Y { get { return values[1]; } set { values[1] = value; } }
        [IgnoreMember]
        public float Z { get { return values[2]; } set { values[2] = value; } }

        public static Vector3f Zero => new Vector3f(0f, 0f, 0f);
        public static Vector3f One => new Vector3f(1f, 1f, 1f);

        public Vector3f()
        {
            values = new float[3] { 0, 0, 0 };
        }

        public Vector3f(float x, float y, float z)
        {
            values = new float[] { x, y, z };
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public float SqrMagnitude()
        {
            return X * X + Y * Y + Z * Z;
        }

        public float Dot(Vector3f b)
        {
            return this.X * b.X + this.Y * b.Y + this.Z * b.Z;
        }

        public void Normalize()
        {
            float mag = Magnitude();

            if (mag == 0)
                return;

            X /= mag;
            Y /= mag;
            Z /= mag;
        }

        public static Vector3f Lerp(Vector3f from, Vector3f to, float t)
        {
            return from + ((to - from) * t);
        }

        public static Vector3f operator +(Vector3f a, Vector3f b)
            => new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3f operator -(Vector3f a, Vector3f b)
            => new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3f operator *(Vector3f a, float f)
            => new Vector3f(a.X * f, a.Y * f, a.Z * f);

        public static bool operator ==(Vector3f a, Vector3f b)
            => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

        public static bool operator !=(Vector3f a, Vector3f b)
            => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

#region GeneratedOverrides
        public override bool Equals(object obj)
        {
            /*
            var f = obj as Vector3f;
            return f != null &&
                   EqualityComparer<float[]>.Default.Equals(values, f.values) &&
                   X == f.X &&
                   Y == f.Y &&
                   Z == f.Z;
            */
            var v = obj as Vector3f;
            return X == v.X && Y == v.Y && Z == v.Z;
        }

        public override int GetHashCode()
        {
            var hashCode = 1373780539;
            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(values);
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }
        #endregion
    }
}
