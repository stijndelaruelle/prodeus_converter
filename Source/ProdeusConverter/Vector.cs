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

        ~Vector2f()
        {
            m_X = null;
            m_Y = null;
        }


        //Serialization
        public bool Deserialize(string data)
        {
            string[] subStrings = data.Split(' ');

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

        ~Vector3f()
        {
            m_X = null;
            m_Y = null;
            m_Z = null;
        }

        //Serialization
        public bool Deserialize(string data)
        {
            string[] subStrings = data.Split(' ');

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

        public Vector4f(string data)
        {
            m_X = null;
            m_Y = null;
            m_Z = null;
            m_W = null;

            //Shortcut
            Deserialize(data);
        }

        ~Vector4f()
        {
            m_X = null;
            m_Y = null;
            m_Z = null;
            m_W = null;
        }

        //Serialization
        public bool Deserialize(string data)
        {
            string[] subStrings = data.Split(' ');

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
    }
}
