using System;
using Newtonsoft.Json;

namespace DoctorLight
{
    /// <summary>
    /// Double precision Vector3d
    ///
    /// Generously made by sldsmkd and released on
    /// https://github.com/sldsmkd/vector3d
    ///
    /// == and != operators were edited to force up to 8 digits accuracy ( more is detrimental to what I am using it for )
    /// </summary>
    public struct Vector3d {
        public const float kEpsilon = 1E-05f;
        public double x;
        public double y;
        public double z;

        public double this[int index] {
            get {
                switch (index) {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    case 2:
                        return this.z;
                    default:
                        throw new IndexOutOfRangeException("Invalid index!");
                }
            }
            set {
                switch (index) {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3d index!");
                }
            }
        }

        [JsonIgnore]
        public Vector3d normalized {
            get {
                return Vector3d.Normalize(this);
            }
        }

        [JsonIgnore]

        public double magnitude {
            get {
                return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            }
        }

        [JsonIgnore]

        public double sqrMagnitude {
            get {
                return this.x * this.x + this.y * this.y + this.z * this.z;
            }
        }

        public static Vector3d zero {
            get {
                return new Vector3d(0d, 0d, 0d);
            }
        }

        public static Vector3d one {
            get {
                return new Vector3d(1d, 1d, 1d);
            }
        }

        public static Vector3d forward {
            get {
                return new Vector3d(0d, 0d, 1d);
            }
        }

        public static Vector3d back {
            get {
                return new Vector3d(0d, 0d, -1d);
            }
        }

        public static Vector3d up {
            get {
                return new Vector3d(0d, 1d, 0d);
            }
        }

        public static Vector3d down {
            get {
                return new Vector3d(0d, -1d, 0d);
            }
        }

        public static Vector3d left {
            get {
                return new Vector3d(-1d, 0d, 0d);
            }
        }

        public static Vector3d right {
            get {
                return new Vector3d(1d, 0d, 0d);
            }
        }

        [Obsolete("Use Vector3d.forward instead.")]
        public static Vector3d fwd {
            get {
                return new Vector3d(0d, 0d, 1d);
            }
        }

        public Vector3d(double x, double y, double z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3d(float x, float y, float z) {
            this.x = (double)x;
            this.y = (double)y;
            this.z = (double)z;
        }

        public Vector3d(double x, double y) {
            this.x = x;
            this.y = y;
            this.z = 0d;
        }

        public static Vector3d operator +(Vector3d a, Vector3d b) {
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3d operator -(Vector3d a, Vector3d b) {
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3d operator -(Vector3d a) {
            return new Vector3d(-a.x, -a.y, -a.z);
        }

        public static Vector3d operator *(Vector3d a, double d) {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3d operator *(double d, Vector3d a) {
            return new Vector3d(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3d operator /(Vector3d a, double d) {
            return new Vector3d(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3d lhs, Vector3d rhs)
        {
            return SqrMagnitude(lhs - rhs) < 0.000000001d;
        }

        public static bool operator !=(Vector3d lhs, Vector3d rhs) 
        {
            return SqrMagnitude(lhs - rhs) >= 0.000000001d;
        }

        public override int GetHashCode() {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        }

        public override bool Equals(object other) {
            if (!(other is Vector3d))
                return false;
            Vector3d vector3d = (Vector3d)other;
            if (this.x.Equals(vector3d.x) && this.y.Equals(vector3d.y))
                return this.z.Equals(vector3d.z);
            else
                return false;
        }

        public static Vector3d Reflect(Vector3d inDirection, Vector3d inNormal) {
            return -2d * Vector3d.Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        public static Vector3d Normalize(Vector3d value) {
            double num = Vector3d.Magnitude(value);
            if (num > 9.99999974737875E-06)
                return value / num;
            else
                return Vector3d.zero;
        }

        public void Normalize() {
            double num = Vector3d.Magnitude(this);
            if (num > 9.99999974737875E-06)
                this = this / num;
            else
                this = Vector3d.zero;
        }
        // TODO : fix formatting
        public override string ToString() {
            return "(" + this.x + " - " + this.y + " - " + this.z + ")";
        }

        public static double Dot(Vector3d lhs, Vector3d rhs) {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        public static Vector3d Project(Vector3d vector, Vector3d onNormal) {
            double num = Vector3d.Dot(onNormal, onNormal);
            if (num < 1.40129846432482E-45d)
                return Vector3d.zero;
            else
                return onNormal * Vector3d.Dot(vector, onNormal) / num;
        }

        public static Vector3d Exclude(Vector3d excludeThis, Vector3d fromThat) {
            return fromThat - Vector3d.Project(fromThat, excludeThis);
        }

        public static double Angle(Vector3d from, Vector3d to) {
            return Mathd.Acos(Mathd.Clamp(Vector3d.Dot(from.normalized, to.normalized), -1d, 1d)) * 57.29578d;
        }

        public static double Distance(Vector3d a, Vector3d b) {
            Vector3d vector3d = new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
            return Math.Sqrt(vector3d.x * vector3d.x + vector3d.y * vector3d.y + vector3d.z * vector3d.z);
        }

        public static Vector3d ClampMagnitude(Vector3d vector, double maxLength) {
            if (vector.sqrMagnitude > maxLength * maxLength)
                return vector.normalized * maxLength;
            else
                return vector;
        }

        public static double Magnitude(Vector3d a) {
            return Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
        }

        public static double SqrMagnitude(Vector3d a) {
            return a.x * a.x + a.y * a.y + a.z * a.z;
        }

        public static Vector3d Min(Vector3d lhs, Vector3d rhs) {
            return new Vector3d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y), Mathd.Min(lhs.z, rhs.z));
        }

        public static Vector3d Max(Vector3d lhs, Vector3d rhs) {
            return new Vector3d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y), Mathd.Max(lhs.z, rhs.z));
        }

        [Obsolete("Use Vector3d.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
        public static double AngleBetween(Vector3d from, Vector3d to) {
            return Mathd.Acos(Mathd.Clamp(Vector3d.Dot(from.normalized, to.normalized), -1d, 1d));
        }
    }
}