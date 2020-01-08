using System;
using System.Collections.Generic;
using System.Text;

using MessagePack;

namespace CrumbShared
{
    [MessagePackObject]
    public class Vector2f
    {
        [Key(0)]
        private readonly float[] values;
        [IgnoreMember]
        public float X { get { return values[0]; } set { values[0] = value; } }
        [IgnoreMember]
        public float Y { get { return values[1]; } set { values[1] = value; } }

        public static Vector2f Zero => new Vector2f(0f, 0f);
        public static Vector2f One => new Vector2f(1f, 1f);

        public Vector2f()
        {
            values = new float[2] { 0, 0 };
        }

        public Vector2f(float x, float y)
        {
            values = new float[] { x, y };
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public float SqrMagnitude()
        {
            return X * X + Y * Y;
        }

        public float Dot(Vector2f b)
        {
            return this.X * b.X + this.Y * b.Y;
        }

        public void Normalize()
        {
            float mag = Magnitude();

            if (mag == 0)
                return;

            X /= mag;
            Y /= mag;
        }

        public static Vector2f Lerp(Vector2f from, Vector2f to, float t)
        {
            return from + ((to - from) * t);
        }

        public static Vector2f operator +(Vector2f a, Vector2f b)
            => new Vector2f(a.X + b.X, a.Y + b.Y);

        public static Vector2f operator -(Vector2f a, Vector2f b)
            => new Vector2f(a.X - b.X, a.Y - b.Y);

        public static Vector2f operator *(Vector2f a, float f)
            => new Vector2f(a.X * f, a.Y * f);

        public static bool operator ==(Vector2f a, Vector2f b)
            => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Vector2f a, Vector2f b)
            => a.X != b.X || a.Y != b.Y;

#region GeneratedOverrides
        public override bool Equals(object obj)
        {
            /*
            var f = obj as Vector2f;
            return f != null &&
                   EqualityComparer<float[]>.Default.Equals(values, f.values) &&
                   X == f.X &&
                   Y == f.Y &&
                   Z == f.Z;
            */
            var v = obj as Vector2f;
            return X == v.X && Y == v.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1373780539;
            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(values);
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
#endregion
    }
}
