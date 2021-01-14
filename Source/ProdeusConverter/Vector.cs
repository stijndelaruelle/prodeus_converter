using System;
using System.Globalization;
using System.Text;

namespace ProdeusConverter
{
    public enum VectorValue
    {
        X = 0,
        Y = 1,
        Z = 2,
        W = 3
    }

    //Nullable Vector containers
    public class Vector2f
    {
        private float? m_X = null;
        public float? X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public float? U
        {
            get { return X; }
            set { X = value; }
        }

        private float? m_Y = null;
        public float? Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        public float? V
        {
            get { return Y; }
            set { Y = value; }
        }

        //Constructors & Destructors
        public Vector2f()
        {
            m_X = null;
            m_Y = null;
        }

        public Vector2f(float x)
        {
            m_X = x;
            m_Y = null;
        }

        public Vector2f(float x, float y)
        {
            m_X = x;
            m_Y = y;
        }

        public Vector2f(Vector2f otherVector)
        {
            //Copy constructor
            m_X = otherVector.X;
            m_Y = otherVector.Y;
        }

        ~Vector2f()
        {
            m_X = null;
            m_Y = null;
        }

        //Mutators & Accessors
        public void AddVector2f(Vector2f otherVector)
        {
            if (otherVector.X != null)
            {
                if (m_X == null) { m_X = otherVector.X; }
                else             { m_X += otherVector.X; }
            }

            if (otherVector.Y != null)
            {
                if (m_Y == null) { m_Y = otherVector.Y; }
                else             { m_Y += otherVector.Y; }
            }
        }

        public bool IsNull()
        {
            return (m_X == null && m_Y == null);
        }

        //Serialization
        public bool Deserialize(string data, char separator = ' ')
        {
            string[] subStrings = data.Split(separator);

            if (subStrings.Length == 0 || subStrings.Length > 2)
            {
                Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Vector2f (data count is " + subStrings.Length + ")");
                return false;
            }

            float?[] tempArray = new float?[2] { null, null }; //Cached as we only want to fill in values when everything is valid
            bool overallSuccess = true; //We want to check every value, give the end user as much info as possible before exiting out

            for (int i = 0; i < subStrings.Length; ++i)
            {
                float parsedFloat = 0.0f;
                bool parseSuccess = float.TryParse(subStrings[i], NumberStyles.Any, CultureInfo.InvariantCulture, out parsedFloat);
                if (parseSuccess == false)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Vector2f (" + ((VectorValue)i).ToString() + " is not a valid float)");
                    overallSuccess = false;
                }
                else
                {
                    tempArray[i] = parsedFloat;
                }
            }

            if (overallSuccess == false)
                return false;

            //All parsed values are valid
            m_X = tempArray[0];
            m_Y = tempArray[1];

            return true;
        }

        public string Serialize(char separator = ' ')
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool addSpace = false;

            if (m_X != null)
            {
                stringBuilder.Append(((float)m_X).ToString(CultureInfo.InvariantCulture.NumberFormat));
                addSpace = true;
            }

            if (m_Y != null)
            {
                if (addSpace) { stringBuilder.Append(separator); addSpace = false; }
                stringBuilder.Append(((float)m_Y).ToString(CultureInfo.InvariantCulture.NumberFormat));
                addSpace = true;
            }

            return stringBuilder.ToString();
        }

        //Overrides
        public override string ToString()
        {
            return Serialize();
        }

        //https://docs.microsoft.com/en-us/dotnet/api/system.object.equals?view=net-5.0
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Vector2f otherVector = (Vector2f)obj;

                return (m_X == otherVector.X && m_Y == otherVector.Y);
            }
        }

        public override int GetHashCode()
        {
            //Super bad default hash, but I don't need it for this project
            return (int)(m_X + m_Y);
        }
    }

    public class Vector3f
    {
        private float? m_X = null;
        public float? X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public float? U
        {
            get { return X; }
            set { X = value; }
        } //alternate name for when using texture space (readability)

        private float? m_Y = null;
        public float? Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        public float? V
        {
            get { return Y; }
            set { Y = value; }
        }

        private float? m_Z = null;
        public float? Z
        {
            get { return m_Z; }
            set { m_Z = value; }
        }
        public float? W
        {
            get { return Z; }
            set { Z = value; }
        }

        //Constructors & Destructors
        public Vector3f()
        {
            m_X = null;
            m_Y = null;
            m_Z = null;
        }

        public Vector3f(float x)
        {
            m_X = x;
            m_Y = null;
            m_Z = null;
        }

        public Vector3f(float x, float y)
        {
            m_X = x;
            m_Y = y;
            m_Z = null;
        }

        public Vector3f(float x, float y, float z)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }

        public Vector3f(Vector3f otherVector)
        {
            //Copy constructor
            m_X = otherVector.X;
            m_Y = otherVector.Y;
            m_Z = otherVector.Z;
        }

        ~Vector3f()
        {
            m_X = null;
            m_Y = null;
            m_Z = null;
        }

        //Mutators & Accessors
        public void AddVector2f(Vector2f otherVector)
        {
            if (otherVector.X != null)
            {
                if (m_X == null) { m_X = otherVector.X; }
                else             { m_X += otherVector.X; }
            }

            if (otherVector.Y != null)
            {
                if (m_Y == null) { m_Y = otherVector.Y; }
                else             { m_Y += otherVector.Y; }
            }
        }

        public void AddVector3f(Vector3f otherVector)
        {
            if (otherVector.X != null)
            {
                if (m_X == null) { m_X = otherVector.X; }
                else             { m_X += otherVector.X; }
            }

            if (otherVector.Y != null)
            {
                if (m_Y == null) { m_Y = otherVector.Y; }
                else             { m_Y += otherVector.Y; }
            }

            if (otherVector.Z != null)
            {
                if (m_Z == null) { m_Z = otherVector.Z; }
                else             { m_Z += otherVector.Z; }
            }
        }

        public bool IsNull()
        {
            return (m_X == null && m_Y == null && m_Z == null);
        }

        //Serialization
        public bool Deserialize(string data, char separator = ' ')
        {
            string[] subStrings = data.Split(separator);

            if (subStrings.Length == 0 || subStrings.Length > 3)
            {
                Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Vector3f (data count is " + subStrings.Length + ")");
                return false;
            }

            float?[] tempArray = new float?[3] { null, null, null }; //Cached as we only want to fill in values when everything is valid
            bool overallSuccess = true; //We want to check every value, give the end user as much info as possible before exiting out

            for (int i = 0; i < subStrings.Length; ++i)
            {
                float parsedFloat = 0.0f;
                bool parseSuccess = float.TryParse(subStrings[i], NumberStyles.Any, CultureInfo.InvariantCulture, out parsedFloat);
                if (parseSuccess == false)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Vector3f (" + ((VectorValue)i).ToString() + " is not a valid float)");
                    overallSuccess = false;
                }
                else
                {
                    tempArray[i] = parsedFloat;
                }
            }

            if (overallSuccess == false)
                return false;

            //All parsed values are valid
            m_X = tempArray[0];
            m_Y = tempArray[1];
            m_Z = tempArray[2];

            return true;
        }

        public string Serialize(char separator = ' ')
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool addSpace = false;

            if (m_X != null)
            {
                stringBuilder.Append(((float)m_X).ToString(CultureInfo.InvariantCulture.NumberFormat));
                addSpace = true;
            }

            if (m_Y != null)
            {
                if (addSpace) { stringBuilder.Append(separator); addSpace = false; }
                stringBuilder.Append(((float)m_Y).ToString(CultureInfo.InvariantCulture.NumberFormat));
                addSpace = true;
            }

            if (m_Z != null)
            {
                if (addSpace) { stringBuilder.Append(separator); addSpace = false; }
                stringBuilder.Append(((float)m_Z).ToString(CultureInfo.InvariantCulture.NumberFormat));
                addSpace = true;
            }

            return stringBuilder.ToString();
        }

        //Overrides
        public override string ToString()
        {
            return Serialize();
        }

        //https://docs.microsoft.com/en-us/dotnet/api/system.object.equals?view=net-5.0
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Vector3f otherVector = (Vector3f)obj;

                return (m_X == otherVector.X && m_Y == otherVector.Y && m_Z == otherVector.Z);
            }
        }

        public override int GetHashCode()
        {
            //Super bad default hash, but I don't need it for this project
            return (int)(m_X + m_Y + m_Z);
        }
    }

    public class Vector4f
    {
        private float? m_X;
        public float? X
        {
            get { return m_X; }
            set { m_X = value; }
        }

        private float? m_Y;
        public float? Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        private float? m_Z;
        public float? Z
        {
            get { return m_Z; }
            set { m_Z = value; }
        }

        private float? m_W;
        public float? W
        {
            get { return m_W; }
            set { m_W = value; }
        }

        //Constructors & Destructor
        public Vector4f()
        {
            m_X = null;
            m_Y = null;
            m_Z = null;
            m_W = null;
        }

        public Vector4f(float x)
        {
            m_X = x;
            m_Y = null;
            m_Z = null;
            m_W = null;
        }

        public Vector4f(float x, float y)
        {
            m_X = x;
            m_Y = y;
            m_Z = null;
            m_W = null;
        }

        public Vector4f(float x, float y, float z)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
            m_W = null;
        }

        public Vector4f(float x, float y, float z, float w)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
            m_W = w;
        }

        public Vector4f(Vector4f otherVector)
        {
            //Copy constructor
            m_X = otherVector.X;
            m_Y = otherVector.Y;
            m_Z = otherVector.Z;
            m_W = otherVector.W;
        }

        ~Vector4f()
        {
            m_X = null;
            m_Y = null;
            m_Z = null;
            m_W = null;
        }

        //Mutators & Accessors
        public void AddVector2f(Vector2f otherVector)
        {
            if (otherVector.X != null)
            {
                if (m_X == null) { m_X = otherVector.X; }
                else             { m_X += otherVector.X; }
            }

            if (otherVector.Y != null)
            {
                if (m_Y == null) { m_Y = otherVector.Y; }
                else             { m_Y += otherVector.Y; }
            }
        }

        public void AddVector3f(Vector3f otherVector)
        {
            if (otherVector.X != null)
            {
                if (m_X == null) { m_X = otherVector.X; }
                else             { m_X += otherVector.X; }
            }

            if (otherVector.Y != null)
            {
                if (m_Y == null) { m_Y = otherVector.Y; }
                else             { m_Y += otherVector.Y; }
            }

            if (otherVector.Z != null)
            {
                if (m_Z == null) { m_Z = otherVector.Z; }
                else             { m_Z += otherVector.Z; }
            }
        }

        public void AddVector4f(Vector4f otherVector)
        {
            if (otherVector.X != null)
            {
                if (m_X == null) { m_X = otherVector.X; }
                else             { m_X += otherVector.X; }
            }

            if (otherVector.Y != null)
            {
                if (m_Y == null) { m_Y = otherVector.Y; }
                else             { m_Y += otherVector.Y; }
            }

            if (otherVector.Z != null)
            {
                if (m_Z == null) { m_Z = otherVector.Z; }
                else             { m_Z += otherVector.Z; }
            }

            if (otherVector.W != null)
            {
                if (m_W == null) { m_W = otherVector.W; }
                else             { m_W += otherVector.W; }
            }
        }

        public bool IsNull()
        {
            return (m_X == null && m_Y == null && m_Z == null && m_W == null);
        }

        //Serialization
        public bool Deserialize(string data, char separator = ' ')
        {
            string[] subStrings = data.Split(separator);

            if (subStrings.Length == 0 || subStrings.Length > 4)
            {
                Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Vector4f (data count is " + subStrings.Length + ")");
                return false;
            }

            float?[] tempArray = new float?[4] { null, null, null, null }; //Cached as we only want to fill in values when everything is valid
            bool overallSuccess = true; //We want to check every value, give the end user as much info as possible before exiting out

            for (int i = 0; i < subStrings.Length; ++i)
            {
                float parsedFloat = 0.0f;
                bool parseSuccess = float.TryParse(subStrings[i], NumberStyles.Any, CultureInfo.InvariantCulture, out parsedFloat);
                if (parseSuccess == false)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Vector4f (" + ((VectorValue)i).ToString() + " is not a valid float)");
                    overallSuccess = false;
                }
                else
                {
                    tempArray[i] = parsedFloat;
                }
            }

            if (overallSuccess == false)
                return false;

            //All parsed values are valid
            m_X = tempArray[0];
            m_Y = tempArray[1];
            m_Z = tempArray[2];
            m_W = tempArray[3];

            return true;
        }

        public string Serialize(char separator = ' ')
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool addSpace = false;

            if (m_X != null)
            {
                stringBuilder.Append(((float)m_X).ToString(CultureInfo.InvariantCulture.NumberFormat));
                addSpace = true;
            }

            if (m_Y != null)
            {
                if (addSpace) { stringBuilder.Append(separator); addSpace = false; }
                stringBuilder.Append(((float)m_Y).ToString(CultureInfo.InvariantCulture.NumberFormat));
                addSpace = true;
            }

            if (m_Z != null)
            {
                if (addSpace) { stringBuilder.Append(separator); addSpace = false; }
                stringBuilder.Append(((float)m_Z).ToString(CultureInfo.InvariantCulture.NumberFormat));
                addSpace = true;
            }

            if (m_W != null)
            {
                if (addSpace) { stringBuilder.Append(separator); addSpace = false; }
                stringBuilder.Append(((float)m_W).ToString(CultureInfo.InvariantCulture.NumberFormat));
            }

            return stringBuilder.ToString();
        }

        //Overrides
        public override string ToString()
        {
            return Serialize();
        }

        //https://docs.microsoft.com/en-us/dotnet/api/system.object.equals?view=net-5.0
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Vector4f otherVector = (Vector4f)obj;

                return (m_X == otherVector.X && m_Y == otherVector.Y && m_Z == otherVector.Z && m_W == otherVector.W);
            }
        }

        public override int GetHashCode()
        {
            //Super bad default hash, but I don't need it for this project
            return (int)(m_X + m_Y + m_Z + m_W);
        }
    }
}
