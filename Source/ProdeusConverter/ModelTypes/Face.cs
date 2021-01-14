using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Just for readability
using Vertex = ProdeusConverter.Vector4f;
using TextureCoordinate = ProdeusConverter.Vector3f;
using Normal = ProdeusConverter.Vector3f;

namespace ProdeusConverter
{
    public class Face
    {
        private List<FaceVertex> m_FaceVertices = null;

        //Constructors & Destructor
        public Face()
        {
            m_FaceVertices = new List<FaceVertex>();
        }

        ~Face()
        {
            m_FaceVertices = null;
        }

        //Utility
        public List<Edge> CalculateEdges()
        {
            List<Edge> edges = new List<Edge>();

            //Add sequential edges
            for (int i = 0; i < m_FaceVertices.Count - 1; ++i)
            {
                Edge edge = new Edge(m_FaceVertices[i].VertexIndex, m_FaceVertices[i + 1].VertexIndex);
                edges.Add(edge);
            }

            //Add last edge (between last & first vertex in the list)
            Edge lastEdge = new Edge(m_FaceVertices[m_FaceVertices.Count - 1].VertexIndex, m_FaceVertices[0].VertexIndex);
            edges.Add(lastEdge);

            return edges;
        }

        //Serialization
        public bool DeserializeOBJ(string data, int vertexIndexOffset, int textureCoordinateIndexOffset, int normalIndexOffset)
        {
            //Format is f v/vt/vn v/vt/vn v/vt/vn ... (v/vt/vn = FaceVertex)
            if (data.StartsWith("f") == false)
            {
                Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Face (data doesn't start with f)");
                return false;
            }

            string[] subStrings = data.Split(' ');

            if (subStrings.Length < 4) //f and at least 3 vertices
            {
                Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Face (vertex count count is less than 3 (" + (subStrings.Length - 1) + "))");
                return false;
            }

            //Create & Parse FaceVertices
            for (int i = 1; i < subStrings.Length; ++i)
            {
                FaceVertex newFaceVertex = new FaceVertex();
                bool success = newFaceVertex.DeserializeOBJ(subStrings[i], vertexIndexOffset, textureCoordinateIndexOffset, normalIndexOffset);

                if (success == true)
                {
                    m_FaceVertices.Add(newFaceVertex);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool DeserializeEMAP(ModelObject modelObject, string data, int breaklineLength)
        {
            if (modelObject == null)
                return false;

            List<int> indices = new List<int>();
            List<int> uvs = new List<int>();

            //----------------------------
            // Indices (Vertex Index)
            //----------------------------
            string indiceListName = "points=";

            //Indice start index
            int vertexListStartIndex = data.IndexOf(indiceListName);
            vertexListStartIndex += indiceListName.Length;

            //Find indice end index (first breakline character)
            int vertexListEndIndex = 0;
            for (int i = vertexListStartIndex; i < data.Length; ++i)
            {
                if (data[i] == '\n' || data[i] == '\r')
                {
                    vertexListEndIndex = i;
                    break;
                }
            }

            //Split into indiceStrings
            string indiceListString = data.Substring(vertexListStartIndex, vertexListEndIndex - vertexListStartIndex);
            string[] indiceStrings = indiceListString.Split(';');

            //Loop trough them and finally deserialize
            for (int i = 0; i < indiceStrings.Length; ++i)
            {
                int indiceIndex = 0;
                bool success = int.TryParse(indiceStrings[i], out indiceIndex);
                if (success == false)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Trying to Parse invalid Face (vertex index is not an integer)");
                    return false;
                }

                indices.Add(indiceIndex);
            }

            //----------------------------
            // UVS
            //----------------------------
            string uvsListName = "uvs=";

            //UV start index
            int uvListStartIndex = data.IndexOf(uvsListName);
            uvListStartIndex += uvsListName.Length;

            //Find uv end index (first breakline character)
            int uvListEndIndex = 0;
            for (int i = uvListStartIndex; i < data.Length; ++i)
            {
                if (data[i] == '\n' || data[i] == '\r')
                {
                    uvListEndIndex = i;
                    break;
                }
            }

            //Split into uvStrings
            string uvsListString = data.Substring(uvListStartIndex, uvListEndIndex - uvListStartIndex);
            string[] uvStrings = uvsListString.Split(';');

            //Loop trough them and finally deserialize
            for (int i = 0; i < uvStrings.Length; ++i)
            {
                TextureCoordinate newTextureCoordinate = new TextureCoordinate();
                bool success = newTextureCoordinate.Deserialize(uvStrings[i], ',');

                if (success == false)
                    return false;

                //As they are stored in the ModelObject, we need to send them there and use their index here
                int textureCoordinateIndex = modelObject.GetOrAddTextureCoordinate(newTextureCoordinate);
                uvs.Add(textureCoordinateIndex);
            }

            //----------------------------
            // Create FaceVertices
            //----------------------------
            if (indices.Count != uvs.Count)
            {
                Logger.LogMessage(Logger.LogType.Error, "Face has different amount of indices & uvs");
                return false;
            }

            for (int i = 0; i < indices.Count; ++i)
            {
                FaceVertex newFaceVertex = new FaceVertex();
                bool success =  newFaceVertex.DeserializeEMAP(indices[i], uvs[i]);

                if (success == false)
                    return false;

                m_FaceVertices.Add(newFaceVertex);
            }

            return true;
        }

        public string SerializeOBJ(int vertexIndexOffset, int textureCoordinateIndexOffset, int normalIndexOffset)
        {
            //Temp
            StringBuilder stringBuilder = new StringBuilder("f");

            foreach (FaceVertex faceVertex in m_FaceVertices)
            {
                stringBuilder.Append(" " + faceVertex.SerializeOBJ(vertexIndexOffset, textureCoordinateIndexOffset, normalIndexOffset));
            }

            return stringBuilder.ToString();
        }

        public string SerializeEMAP(ref int seed, List<TextureCoordinate> textureCoordinates)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Face{");

            //Default surface settings
            stringBuilder.AppendLine("surf={");
            stringBuilder.AppendLine("localMapping=False");
            stringBuilder.AppendLine("mappingType=5"); //Todo: now only local
            stringBuilder.AppendLine("material=0");
            stringBuilder.AppendLine("color=0");
            stringBuilder.AppendLine("colorEmissive=0");
            stringBuilder.AppendLine("seed=" + seed);
            stringBuilder.AppendLine("halfRes=False");
            stringBuilder.AppendLine("uvScaleBias=1,1,0,0");
            stringBuilder.AppendLine("uvScroll=0,0");
            stringBuilder.AppendLine("localOffset=0,0,0");
            stringBuilder.AppendLine("worldOffset=0,0,0");
            stringBuilder.AppendLine("}");

            //Vertices
            StringBuilder pointStringBuilder = new StringBuilder("points=");
            for (int i = 0; i < m_FaceVertices.Count; ++i)
            {
                if (i != 0) { pointStringBuilder.Append(";"); }
                pointStringBuilder.Append(m_FaceVertices[i].VertexIndex);
            }
            stringBuilder.AppendLine(pointStringBuilder.ToString());

            //UV
            StringBuilder uvStringBuilder = new StringBuilder("uvs=");
            for (int i = 0; i < m_FaceVertices.Count; ++i)
            {
                if (i != 0) { uvStringBuilder.Append(";"); }

                //In case there are no texture coordinates in the file
                if (textureCoordinates.Count == 0 || m_FaceVertices[i].TextureCoordinateIndex < 0)
                {
                    uvStringBuilder.Append("0,0");
                }
                else
                {
                    TextureCoordinate textureCoordinate = textureCoordinates[m_FaceVertices[i].TextureCoordinateIndex]; //0 based!
                    uvStringBuilder.Append(textureCoordinate.Serialize(','));
                }
            }
            stringBuilder.AppendLine(uvStringBuilder.ToString());

            stringBuilder.AppendLine("}");

            seed += 1;
            return stringBuilder.ToString();
        }

        //Overrides
        public override string ToString()
        {
            return SerializeOBJ(0, 0, 0);
        }
    }
}
