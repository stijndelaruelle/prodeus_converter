using System;

namespace ProdeusConverter
{
    public class Edge
    {
        private int m_FirstVertexIndex = 0; //Also called indice
        public int FirstVertexIndex
        {
            get { return m_FirstVertexIndex; }
            set { m_FirstVertexIndex = value; }
        }

        private int m_SecondVertexIndex = 0;
        public int SecondVertexIndex
        {
            get { return m_SecondVertexIndex; }
            set { m_SecondVertexIndex = value; }
        }

        //Constructors & Destructor
        public Edge()
        {
            m_FirstVertexIndex = 0;
            m_SecondVertexIndex = 0;
        }

        public Edge(int firstVertexIndex, int secondVertexIndex)
        {
            m_FirstVertexIndex = firstVertexIndex;
            m_SecondVertexIndex = secondVertexIndex;
        }

        ~Edge()
        {
            m_FirstVertexIndex = 0;
            m_SecondVertexIndex = 0;
        }

        //Serialization
        public string SerializeEMAP()
        {
            return (m_FirstVertexIndex + "," + m_SecondVertexIndex);
        }

        //Overrides
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
                Edge otherEdge = (Edge)obj;

                if (m_FirstVertexIndex == otherEdge.FirstVertexIndex && m_SecondVertexIndex == otherEdge.SecondVertexIndex)
                    return true;

                if (m_FirstVertexIndex == otherEdge.SecondVertexIndex && m_SecondVertexIndex == otherEdge.FirstVertexIndex)
                    return true;

                return false;
            }
        }

        public override int GetHashCode()
        {
            return (m_FirstVertexIndex << 2) ^ m_SecondVertexIndex;
        }
    }
}
