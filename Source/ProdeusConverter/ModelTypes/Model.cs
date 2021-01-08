using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProdeusConverter
{
    //Main object, includes multiple brushes (ModelObjects)
    public class Model
    {
        private string m_Name = string.Empty;
        private string m_OriginalFilePath = string.Empty;

        private List<ModelObject> m_ModelObjects = null;

        //Constructors & Destructor
        public Model()
        {
            m_ModelObjects = new List<ModelObject>();
        }

        public Model(string data)
        {
            m_ModelObjects = new List<ModelObject>();

            //Shortcut
            DeserializeOBJ(data);
        }

        ~Model()
        {
            m_ModelObjects = null;
        }

        //Serialization
        public bool DeserializeOBJ(string filePath)
        {
            ModelObject currentModelObject = null;

            //Indices are incremental across multiple ModelObjects in OBJ (Brushes)
            int vertexIndexOffset = 0;
            int textureCoordinateIndexOffset = 0;
            int normalIndexOffset = 0;

            //Open the file
            StreamReader streamReader = new StreamReader(filePath);

            //Read the file line by line
            string currentLine = streamReader.ReadLine(); //works for \n and \rn I assume
            while (currentLine != null)
            {
                //Create object when encountering an o
                if (currentLine.StartsWith("o"))
                {
                    //Add in the current object
                    if (currentModelObject != null && currentModelObject.IsEmpty() == false)
                    {
                        currentModelObject.CalculateEdges();
                        m_ModelObjects.Add(currentModelObject);

                        vertexIndexOffset += currentModelObject.GetVertexCount();
                        textureCoordinateIndexOffset += currentModelObject.GetTextureCoordinateCount();
                        normalIndexOffset += currentModelObject.GetNormalCount();
                    }

                    //o name
                    currentModelObject = new ModelObject(currentLine.Substring(2, currentLine.Length - 2));
                }

                //Otherwise extend the current object
                else
                {
                    //Export from maya didn't have a default object.
                    if (currentModelObject == null)
                    {
                        currentModelObject = new ModelObject("Default Object");
                    }

                    bool success = currentModelObject.DeserializePart(currentLine, vertexIndexOffset, textureCoordinateIndexOffset, normalIndexOffset);

                    if (success == false)
                    {
                        streamReader.Close();
                        return false;
                    }
                }

                currentLine = streamReader.ReadLine();

                //Add in the last object
                if (currentLine == null && currentModelObject != null)
                {
                    currentModelObject.CalculateEdges();
                    m_ModelObjects.Add(currentModelObject);
                }
            }

            //Close the file
            streamReader.Close();

            m_Name = Path.GetFileNameWithoutExtension(filePath);
            m_OriginalFilePath = filePath;

            return true;
        }

        public string SerializeOBJ()
        {
            //Temp
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("# Converted from " + m_OriginalFilePath);

            foreach (ModelObject modelObject in m_ModelObjects)
            {
                stringBuilder.AppendLine(modelObject.SerializeOBJ());
            }

            return stringBuilder.ToString();
        }

        public void SerializeEMAP(string filePath, SerializeMode serializeMode)
        {
            StringBuilder brushStringBuilder = new StringBuilder();

            //Convert
            int seed = 0;
            for (int i = 0; i < m_ModelObjects.Count; ++i)
            {
                brushStringBuilder.Append(m_ModelObjects[i].SerializeEMAP(ref seed));
            }

            string brushString = brushStringBuilder.ToString();
            string fileString = string.Empty;

            //Write
            bool isNewFile = !File.Exists(filePath);

            //---------------
            // Existing file
            //---------------
            if (isNewFile == false)
            {
                //Find out where we should start writing
                StreamReader streamReader = new StreamReader(filePath);
                string fileContent = streamReader.ReadToEnd();
                streamReader.Close();

                //Very basic check to see if it's a Prodeus file? Easily bypassed of course, but why would you?
                //Note: We really assume here that the file has been constructed by the game itself. If not this will all fail horribly.
                if (fileContent.StartsWith("Version_1") == false)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Couldn't write to existing output file, this file was most likely not saved by Prodeus itself.");
                    return;
                }

                //Determine end point of brushes (Brushes are always above Nodes when Prodeus exports the file)
                int nodeStartIndex = fileContent.IndexOf("Nodes{");
                if (nodeStartIndex < 0)
                {
                    Logger.LogMessage(Logger.LogType.Error, "Couldn't write to existing output file, this file was most likely not saved by Prodeus itself.");
                    return;
                }

                int brushesEndIndex = nodeStartIndex - 2; //breakline and }

                string preNewBrushString = string.Empty;
                string postNewBrushString = fileContent.Substring(brushesEndIndex - 1, fileContent.Length - brushesEndIndex);

                if (serializeMode == SerializeMode.Overwrite)
                {
                    //Determine start point
                    string brushHeader = "Brushes{";
                    int brushStartIndex = fileContent.IndexOf(brushHeader);
                    brushStartIndex += brushHeader.Length;

                    preNewBrushString = fileContent.Substring(0, brushStartIndex);

                    //Bit of a cheaty newline
                    brushString = "\n" + brushString;
                }

                else if (serializeMode == SerializeMode.Append)
                {
                    //Determine start point
                    preNewBrushString = fileContent.Substring(0, brushesEndIndex - 1);
                }

                //Recombine with the new brushes in the middle
                StringBuilder fileStringBuilder = new StringBuilder();
                fileStringBuilder.Append(preNewBrushString);
                fileStringBuilder.Append(brushString);
                fileStringBuilder.Append(postNewBrushString);

                fileString = fileStringBuilder.ToString();
            }

            //---------------
            // New file
            //---------------
            else
            {
                StringBuilder fileStringBuilder = new StringBuilder();

                //EMAP Version number
                fileStringBuilder.AppendLine("Version_1");

                //Default map properties
                fileStringBuilder.AppendLine("MapProperties{");
                fileStringBuilder.AppendLine("mapID=");
                fileStringBuilder.AppendLine("mapTitle=" + m_Name);
                fileStringBuilder.AppendLine("mapDescription=Converted from " + m_OriginalFilePath);
                fileStringBuilder.AppendLine("mapTags=");
                fileStringBuilder.AppendLine("isCampaign=False");
                fileStringBuilder.AppendLine("mapRunes=");
                fileStringBuilder.AppendLine("mapWeapons=");
                fileStringBuilder.AppendLine("mapEnemyCount=0");
                fileStringBuilder.AppendLine("music=Hotspot");
                fileStringBuilder.AppendLine("lavaColor=0.15,0.1,0.05,1");
                fileStringBuilder.AppendLine("lavaEmissiveColor=1,0.35,0.15,1");
                fileStringBuilder.AppendLine("waterColor=0,0.05,0.15,1");
                fileStringBuilder.AppendLine("waterEmissiveColor=0,0,0,1");
                fileStringBuilder.AppendLine("wasteColor=0.05,0.1,0.03,1");
                fileStringBuilder.AppendLine("wasteEmissiveColor=0.2,1,0.1,1");
                fileStringBuilder.AppendLine("}");

                //No layers
                fileStringBuilder.AppendLine("Layers{");
                fileStringBuilder.AppendLine("}");

                //Default color
                fileStringBuilder.AppendLine("Colors{");
                fileStringBuilder.AppendLine("Default=1,1,1,1");
                fileStringBuilder.AppendLine("}");

                //Default material
                fileStringBuilder.AppendLine("Materials{");
                fileStringBuilder.AppendLine("Blockout");
                fileStringBuilder.AppendLine("}");

                //The actual model
                fileStringBuilder.AppendLine("Brushes{");
                fileStringBuilder.Append(brushString);
                fileStringBuilder.AppendLine("}");

                //No nodes
                fileStringBuilder.AppendLine("Nodes{");
                fileStringBuilder.AppendLine("}");

                fileString = fileStringBuilder.ToString();
            }
            
            //Actually Write it to a file
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(fileString);
            streamWriter.Close();
        }

        //Overrides
        public override string ToString()
        {
            return SerializeOBJ();
        }
    }
}
