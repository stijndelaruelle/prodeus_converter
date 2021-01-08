using System.Text;

namespace ProdeusConverter
{
    public class FaceVertex
    {
        private int m_VertexIndex = 0; //Also called indice
        public int VertexIndex
        {
            get { return m_VertexIndex; }
            set { m_VertexIndex = value; }
        }

        private int m_TextureCoordinateIndex = 0;
        public int TextureCoordinateIndex
        {
            get { return m_TextureCoordinateIndex; }
            set { m_TextureCoordinateIndex = value; }
        }

        private int m_NormalIndex = 0;
        public int NormalIndex
        {
            get { return m_NormalIndex; }
            set { m_NormalIndex = value; }
        }

        //Constructors & Destructor
        public FaceVertex()
        {
            m_VertexIndex = 0;
            m_TextureCoordinateIndex = 0;
            m_NormalIndex = 0;
        }

        public FaceVertex(int vertexIndex, int textureCoordinateIndex, int normalIndex)
        {
            m_VertexIndex = vertexIndex;
            m_TextureCoordinateIndex = textureCoordinateIndex;
            m_NormalIndex = normalIndex;
        }

        ~FaceVertex()
        {
            m_VertexIndex = 0;
            m_TextureCoordinateIndex = 0;
            m_NormalIndex = 0;
        }

        //Serialization
        public bool DeserializeOBJ(string data, int vertexIndexOffset, int textureCoordinateIndexOffset, int normalIndexOffset)
        {
            //TODO: Negative id's are not supported at the moment!

            //Data is in the vertex index/texture coordinate index/normal index format (v/vt/vn)
            string[] subStrings = data.Split('/');

            if (subStrings.Length == 0 || subStrings.Length > 3)
            {
                Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid FaceVertex (data substring count is " + subStrings.Length + ")");
                return false;
            }

            //Vertex Index
            bool success = int.TryParse(subStrings[0], out m_VertexIndex);
            if (success)
            {
                m_VertexIndex -= 1; //OBJ is 1 based, we move to 0 based
                m_VertexIndex -= vertexIndexOffset; //Reduce by the cummulative vertex count of other objects
            }
            else
            {
                Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid FaceVertex (vertex index is not an integer)");
                return false;
            }

            //Texturecoordinate Index
            if (subStrings.Length > 1)
            {
                int.TryParse(subStrings[1], out m_TextureCoordinateIndex);
                if (success)
                {
                    m_TextureCoordinateIndex -= 1; //OBJ is 1 based, we move to 0 based
                    m_TextureCoordinateIndex -= textureCoordinateIndexOffset; //Reduce by the cummulative vertex count of other objects
                }

                //Fail is not a problem, this is not required
            }

            //Normal Index
            if (subStrings.Length > 2)
            {
                int.TryParse(subStrings[2], out m_NormalIndex);
                if (success)
                {
                    m_NormalIndex -= 1; //OBJ is 1 based, we move to 0 based
                    m_NormalIndex -= normalIndexOffset; //Reduce by the cummulative vertex count of other objects
                }

                //Fail is not a problem, this is not required
            }

            return true;
        }

        public string SerializeOBJ()
        {
            //Temp
            if (m_VertexIndex < 0)
                return "invalid FaceVertex";

            StringBuilder stringBuilder = new StringBuilder("" + m_VertexIndex);

            if (m_TextureCoordinateIndex >= 0 || m_NormalIndex >= 0) //If there is no texCoord but there is a normal it displays v//vn
                stringBuilder.Append("/");

            if (m_TextureCoordinateIndex >= 0)
                stringBuilder.Append(m_TextureCoordinateIndex);

            if (m_NormalIndex >= 0)
                stringBuilder.Append("/" + m_NormalIndex);

            return stringBuilder.ToString();
        }

        //Overrides
        public override string ToString()
        {
            return SerializeOBJ();
        }
    }
}
