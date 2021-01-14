using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

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
        public bool Deserialize(string filePath)
        {
            string inputFileType = Path.GetExtension(filePath);
            bool success = true;

            switch (inputFileType)
            {
                case ".obj":  { success = DeserializeOBJ(filePath); break; }
                case ".emap": { success = DeserializeEMAP(filePath); break; }

                default:
                {
                    Logger.LogMessage(Logger.LogType.Warning, "Unable to read input filetype. Please select an .obj or .emap file");
                    success = false;
                    break;
                }
            }

            return success;
        }

        private bool DeserializeOBJ(string filePath)
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
                if (currentLine.StartsWith("o "))
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

                    bool success = currentModelObject.DeserializeOBJPart(currentLine, vertexIndexOffset, textureCoordinateIndexOffset, normalIndexOffset);

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

        private bool DeserializeEMAP(string filePath)
        {
            //Read the file
            StreamReader streamReader = new StreamReader(filePath);
            string fileContent = streamReader.ReadToEnd();
            
            streamReader.Close();

            //Very basic check to see if it's a Prodeus file? Easily bypassed of course, but why would you?
            //Note: We really assume here that the file has been constructed by the game itself. If not this will all fail horribly.
            if (fileContent.StartsWith("Version_1") == false)
            {
                Logger.LogMessage(Logger.LogType.Error, "Couldn't write to existing output file, this file was most likely not saved by Prodeus itself.");
                return false;
            }

            int breaklineLength = 0;
            if (fileContent.Contains("\r\n"))    { breaklineLength = 2; }
            else if (fileContent.Contains("\n")) { breaklineLength = 1; }
            else
            {
                Logger.LogMessage(Logger.LogType.Error, "File is using unconventional line endings!");
                return false;
            }

            //---------------------------
            // Get the "Brushes" block
            //---------------------------

            //Determine start point
            string brushHeader = "Brushes{";
            int brushesStartIndex = fileContent.IndexOf(brushHeader);
            if (brushesStartIndex < 0)
            {
                Logger.LogMessage(Logger.LogType.Error, "Couldn't write to existing output file, this file was most likely not saved by Prodeus itself.");
                return false;
            }

            //Determine end point of brushes (Brushes are always above Nodes when Prodeus exports the file)
            int nodeStartIndex = fileContent.IndexOf("Nodes{");
            if (nodeStartIndex < 0)
            {
                Logger.LogMessage(Logger.LogType.Error, "Couldn't write to existing output file, this file was most likely not saved by Prodeus itself.");
                return false;
            }

            int brushesEndIndex = nodeStartIndex - breaklineLength; //breakline
            string brushesContent = fileContent.Substring(brushesStartIndex, brushesEndIndex - brushesStartIndex);

            //---------------------------
            // Loop trough all brushes
            //---------------------------
            int brushstartIndex = brushesContent.IndexOf("Brush{");
            bool isLooping = true;

            while (isLooping)
            {
                int brushEndIndex = brushesContent.IndexOf("Brush{", brushstartIndex + 1);
                if (brushEndIndex < 0)
                {
                    brushEndIndex = brushesContent.Length - 1; // }
                    isLooping = false;
                }

                int tempBrushEndIndex = brushEndIndex - breaklineLength; //Remove the breakline as well

                string brushContent = brushesContent.Substring(brushstartIndex, tempBrushEndIndex - brushstartIndex);

                ModelObject newModelObject = new ModelObject("New Brush"); //Temp
                bool success = newModelObject.DeserializeEMAP(brushContent, breaklineLength);

                //Exit out if a brush couldn't be parsed properly
                if (success == false)
                    return false;

                m_ModelObjects.Add(newModelObject);

                brushstartIndex = brushEndIndex; //Use this for the next iteration
            }

            m_Name = Path.GetFileNameWithoutExtension(filePath);
            m_OriginalFilePath = filePath;

            return true;
        }

        public bool Serialize(string filePath, SerializeMode serializeMode)
        {
            string inputFileType = Path.GetExtension(filePath);
            bool success = true;

            switch (inputFileType)
            {
                case ".obj": { SerializeOBJ(filePath, serializeMode); break; }
                case ".emap": { SerializeEMAP(filePath, serializeMode); break; }

                default:
                    {
                        Logger.LogMessage(Logger.LogType.Warning, "Unable to read output filetype. Please select an .obj or .emap file");
                        success = false;
                        break;
                    }
            }

            return success;
        }

        private void SerializeOBJ(string filePath, SerializeMode serializeMode)
        {
            string fileString = string.Empty;

            //Write
            bool isNewFile = !File.Exists(filePath);

            //---------------
            // Existing file
            //---------------
            if (isNewFile == true || serializeMode == SerializeMode.Overwrite)
            {
                string brushString = SerializeOBJBrushes(0, 0, 0);

                StringBuilder fileStringBuilder = new StringBuilder();
                fileStringBuilder.AppendLine("# Converted from " + m_OriginalFilePath);
                fileStringBuilder.Append(brushString);

                fileString = fileStringBuilder.ToString();
            }
            else if (isNewFile == false && serializeMode == SerializeMode.Append)
            {
                //Open the existing file and count the amount of vertices, texture coordinates & normals
                int vertexIndexOffset = 0;
                int textureCoordinateIndexOffset = 0;
                int normalIndexOffset = 0;

                //Open the file
                StreamReader streamReader = new StreamReader(filePath);
                StringBuilder fileStringBuilder = new StringBuilder();

                //Read the file line by line
                string currentLine = streamReader.ReadLine(); //works for \n and \rn I assume
                while (currentLine != null)
                {
                    if (currentLine.StartsWith("v "))  { vertexIndexOffset += 1; }
                    if (currentLine.StartsWith("vt ")) { textureCoordinateIndexOffset += 1; }
                    if (currentLine.StartsWith("vn ")) { normalIndexOffset += 1; }

                    fileStringBuilder.AppendLine(currentLine);

                    currentLine = streamReader.ReadLine();
                }

                //Close the file
                streamReader.Close();

                string brushString = SerializeOBJBrushes(vertexIndexOffset, textureCoordinateIndexOffset, normalIndexOffset);

                fileStringBuilder.AppendLine("# Converted from " + m_OriginalFilePath);
                fileStringBuilder.Append(brushString);

                fileString = fileStringBuilder.ToString();
            }

            //Actually Write it to a file
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(fileString);
            streamWriter.Close();
        }

        private void SerializeEMAP(string filePath, SerializeMode serializeMode)
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

                int breaklineLength = 0;
                if (fileContent.Contains("\r\n")) { breaklineLength = 2; }
                else if (fileContent.Contains("\n")) { breaklineLength = 1; }
                else
                {
                    Logger.LogMessage(Logger.LogType.Error, "File is using unconventional line endings!");
                    return;
                }

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

                int brushesEndIndex = nodeStartIndex - breaklineLength; //breakline

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
        private string SerializeOBJBrushes(int vertexIndexOffset, int textureCoordinateIndexOffset, int normalIndexOffset)
        {
            StringBuilder brushStringBuilder = new StringBuilder();

            //Convert
            foreach (ModelObject modelObject in m_ModelObjects)
            {
                brushStringBuilder.Append(modelObject.SerializeOBJ(vertexIndexOffset, textureCoordinateIndexOffset, normalIndexOffset));

                vertexIndexOffset            += modelObject.GetVertexCount();
                textureCoordinateIndexOffset += modelObject.GetTextureCoordinateCount();
                normalIndexOffset            += modelObject.GetNormalCount();
            }

            return brushStringBuilder.ToString();
        }

        public override string ToString()
        {
            return SerializeOBJBrushes(0, 0, 0);
        }
    }
}
