using System.Collections.Generic;
using System.Text;

//Just for readability
using Vertex = ProdeusConverter.Vector4f;
using TextureCoordinate = ProdeusConverter.Vector3f;
using Normal = ProdeusConverter.Vector3f;

namespace ProdeusConverter
{
    //Contains 1 Brush
    public class ModelObject
    {
        private string m_Name = string.Empty;
        private List<Vertex> m_Vertices = null;
        private List<TextureCoordinate> m_TextureCoordinates = null;
        private List<Normal> m_Normals = null;
        private List<Face> m_Faces = null;
        private List<Edge> m_Edges = null; //Doesn't actually contain any new data, it's just a list of all the unique edges in all the faces (used for EMAP format)

        //Constructors & Destructor
        public ModelObject(string name)
        {
            m_Name = name;
            m_Vertices = new List<Vertex>();
            m_TextureCoordinates = new List<TextureCoordinate>();
            m_Normals = new List<TextureCoordinate>();
            m_Faces = new List<Face>();
            m_Edges = new List<Edge>();
        }

        ~ModelObject()
        {
            m_Name = string.Empty;
            m_Vertices = null;
            m_TextureCoordinates = null;
            m_Normals = null;
            m_Faces = null;
            m_Edges = null;
        }

        //Accessors
        public int GetVertexCount()
        {
            return m_Vertices.Count;
        }

        public int GetTextureCoordinateCount()
        {
            return m_TextureCoordinates.Count;
        }

        public int GetNormalCount()
        {
            return m_Normals.Count;
        }

        public bool IsEmpty()
        {
            return (m_Vertices.Count == 0 && m_TextureCoordinates.Count == 0 && m_Normals.Count == 0 && m_Faces.Count == 0 && m_Edges.Count == 0);
        }

        //Mutators
        public void CalculateEdges()
        {
            //Edges don't really add any new info as the order of the verticices in a face already do that but Prodeus includes them for convienience
            for (int i = 0; i < m_Faces.Count; ++i)
            {
                List<Edge> faceEdges = m_Faces[i].CalculateEdges();

                //Go trough the list and add all the unique ones
                //NOTE: This is super slow... should optimize at some point but right now I just want to check if it works. (+ you really shouldn't export big models!)
                foreach (Edge faceEdge in faceEdges)
                {
                    bool isUnique = true;
                    foreach (Edge collectionEdge in m_Edges)
                    {
                        if (collectionEdge.Equals(faceEdge))
                        {
                            isUnique = false;
                            break;
                        }
                    }

                    if (isUnique == true)
                        m_Edges.Add(faceEdge);
                }
            }
        }

        //Serialization
        public bool DeserializePart(string data, int vertexIndexOffset, int textureCoordinateIndexOffset, int normalIndexOffset)
        {
            //NOTE: We are only parsing a very small selection of OBJ features!
            //Full list of features: https://en.wikipedia.org/wiki/Wavefront_.obj_file

            //Vertex
            if (data.StartsWith("v "))
            {
                //Remove the "v " so Vector4f can parse it (w is optional)
                data = data.Substring(2, data.Length - 2);
                Vertex newVertex = new Vertex();
                bool success = newVertex.Deserialize(data);

                if (success == true)
                    m_Vertices.Add(newVertex);

                return success;
            }

            //Texture coordinate
            else if (data.StartsWith("vt "))
            {
                //Remove the "vt " so Vector3f can parse it (w is optional)
                data = data.Substring(3, data.Length - 3);
                TextureCoordinate newTextureCoordinate = new TextureCoordinate();
                bool success = newTextureCoordinate.Deserialize(data);

                if (success == true)
                    m_TextureCoordinates.Add(newTextureCoordinate);

                return success;
            }

            //Normal (currently not used by Prodeus, but here for the sake of completeness)
            else if (data.StartsWith("vn "))
            {
                //Remove the "vn " so Vector3f can parse it
                data = data.Substring(3, data.Length - 3);
                Normal newNormal = new Normal();
                bool success = newNormal.Deserialize(data);

                if (success == true)
                    m_Normals.Add(newNormal);

                return success;
            }

            //Face
            else if (data.StartsWith("f "))
            {
                Face newFace = new Face();
                bool success = newFace.DeserializeOBJ(data, vertexIndexOffset, textureCoordinateIndexOffset, normalIndexOffset);

                if (success == true)
                    m_Faces.Add(newFace);

                return success;
            }

            return true;
        }

        public string SerializeOBJ()
        {
            //Temp
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("o " + m_Name);

            foreach (Vertex vertex in m_Vertices)
            {
                stringBuilder.AppendLine("v " + vertex.ToString());
            }

            foreach (TextureCoordinate textureCoordinate in m_TextureCoordinates)
            {
                stringBuilder.AppendLine("vt " + textureCoordinate.ToString());
            }

            foreach (Normal normal in m_Normals)
            {
                stringBuilder.AppendLine("vn " + normal.ToString());
            }

            foreach (Face face in m_Faces)
            {
                stringBuilder.AppendLine(face.SerializeOBJ());
            }

            return stringBuilder.ToString();
        }

        public string SerializeEMAP(ref int seed)
        {
            StringBuilder stringBuilder = new StringBuilder();

            //Brush settings
            stringBuilder.AppendLine("Brush{");
            stringBuilder.AppendLine("parent=-1");
            stringBuilder.AppendLine("layer=-1");
            stringBuilder.AppendLine("pos=0,0,0"); //Always place at the origin for now

            //Vertices
            StringBuilder pointStringBuilder = new StringBuilder("points=");
            for (int i = 0; i < m_Vertices.Count; ++i)
            {
                if (i != 0) { pointStringBuilder.Append(";"); }
                pointStringBuilder.Append(m_Vertices[i].Serialize(','));
            }
            stringBuilder.AppendLine(pointStringBuilder.ToString());

            //Edges
            StringBuilder edgesStringBuilder = new StringBuilder("edges=");
            for (int i = 0; i < m_Edges.Count; ++i)
            {
                if (i != 0) { edgesStringBuilder.Append(";"); }
                edgesStringBuilder.Append(m_Edges[i].SerializeEMAP());
            }
            stringBuilder.AppendLine(edgesStringBuilder.ToString());

            //Faces
            for (int i = 0; i < m_Faces.Count; ++i)
            {
                stringBuilder.Append(m_Faces[i].SerializeEMAP(ref seed, m_TextureCoordinates));
            }

            stringBuilder.AppendLine("}");

            return stringBuilder.ToString();
        }

        //Overrides
        public override string ToString()
        {
            return SerializeOBJ();
        }
    }
}