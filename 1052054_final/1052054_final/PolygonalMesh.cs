using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Tao.OpenGl;

namespace Example5
{
    struct Point3
    {
        public float x,y,z;
    }

    struct Vector3
    {
        public float x,y,z;
    }

    struct Color
    {
        public byte r, g, b;
    }

    struct VertexID
    {
        public int vertIndex, normIndex, colorIndex;
    }

    struct Face
    {
        public int nVerts;
	    public VertexID[] pVert;
    }

    class Mesh
    {
        private int nVerts=0;
        private Point3[] pPt;
        private int nNormals=0;
        private Vector3[] pNorm;
        private int nColors=0;
        private Color[] pColor;
        private int nFaces=0;
        private Face[] pFace;

        private void CalNormal(Point3 p1, Point3 p2, Point3 p3, out Vector3 normal)
        {
            float[] v1 = new float[3], v2 = new float[3];
            float len;

	        v1[0] = p2.x - p1.x;
	        v1[1] = p2.y - p1.y;
	        v1[2] = p2.z - p1.z;

	        v2[0] = p3.x - p1.x;
	        v2[1] = p3.y - p1.y;
	        v2[2] = p3.z - p1.z;

	        normal.x = v1[1]*v2[2]-v1[2]*v2[1];
	        normal.y = v1[2]*v2[0]-v1[0]*v2[2];
	        normal.z = v1[0]*v2[1]-v1[1]*v2[0];

	        // normalize
	        len = normal.x * normal.x + normal.y * normal.y + normal.z * normal.z;
	        if(len > 0.0)
	        {
		        len = (float)Math.Sqrt(len);
		        normal.x/=len;
		        normal.y/=len;
		        normal.z/=len;
	        }
        }

        public void ReadFile(string filename)
        {
            try
            {
                StreamReader sr = new StreamReader(filename);
                while (sr.Peek()>0)
                {
                    string[] StrArray = new string[3];

                    string line = sr.ReadLine();
                    line = line.Trim();

                    if(!string.IsNullOrEmpty(line))
                    {
                        StrArray = line.Split(':');
                        if(StrArray.Length!=2) 
                        { 
                            throw new Exception("不正確的資料項目");
                        }

                        if (StrArray[0].Trim() == "Vertex List")
                        {
                            string[] DataArray = new string[4];

                            nVerts = int.Parse(StrArray[1]);
                            pPt = new Point3[nVerts];
                            for (int i = 0; i < nVerts; i++)
                            {
                                line = sr.ReadLine();
                                StrArray = line.Split(':');
                                DataArray = StrArray[1].Split(',');
                                if (DataArray.Length != 3) throw new Exception("Vertex List 資料格式錯誤");
                                pPt[i].x = float.Parse(DataArray[0]);
                                pPt[i].y = float.Parse(DataArray[1]);
                                pPt[i].z = float.Parse(DataArray[2]);
                            }
                        }
                        else if (StrArray[0].Trim() == "Normal List")
                        {
                            string[] DataArray = new string[4];

                            nNormals = int.Parse(StrArray[1]);
                            pNorm = new Vector3[nNormals];
                            for (int i = 0; i < nNormals; i++)
                            {
                                line = sr.ReadLine();
                                StrArray = line.Split(':');
                                DataArray = StrArray[1].Split(',');
                                if (DataArray.Length != 3) throw new Exception("Normal List 資料格式錯誤");
                                pNorm[i].x = float.Parse(DataArray[0]);
                                pNorm[i].y = float.Parse(DataArray[1]);
                                pNorm[i].z = float.Parse(DataArray[2]);
                            }
                        }
                        else if (StrArray[0].Trim() == "Color List")
                        {
                            string[] DataArray = new string[4];

                            nColors = int.Parse(StrArray[1]);
                            pColor = new Color[nColors];
                            for (int i = 0; i < nColors; i++)
                            {
                                line = sr.ReadLine();
                                StrArray = line.Split(':');
                                DataArray = StrArray[1].Split(',');
                                if (DataArray.Length != 3) throw new Exception("Color List 資料格式錯誤");
                                pColor[i].r = byte.Parse(DataArray[0]);
                                pColor[i].g = byte.Parse(DataArray[1]);
                                pColor[i].b = byte.Parse(DataArray[2]);
                            }
                        }
                        else if (StrArray[0].Trim() == "Face List")
                        {
                            nFaces = int.Parse(StrArray[1]);
                            pFace = new Face[nFaces];
                            for (int i = 0; i < nFaces; i++)
                            {
                                line = sr.ReadLine();
                                StrArray = line.Split(':');
                                pFace[i].nVerts = int.Parse(StrArray[1]);
                                pFace[i].pVert = new VertexID[pFace[i].nVerts];

                                string[] DataArray = new string[pFace[i].nVerts+1];
                                line = sr.ReadLine();
                                DataArray = line.Split(',');
                                if (DataArray.Length != pFace[i].nVerts) throw new Exception("Face List 資料格式錯誤");
                                for (int j = 0; j < pFace[i].nVerts; j++)
                                {
                                    pFace[i].pVert[j].vertIndex = int.Parse(DataArray[j]);
                                }
                                line = sr.ReadLine();
                                DataArray = line.Split(',');
                                if (DataArray.Length != pFace[i].nVerts) throw new Exception("Face List 資料格式錯誤");
                                for (int j = 0; j < pFace[i].nVerts; j++)
                                {
                                    pFace[i].pVert[j].normIndex = int.Parse(DataArray[j]);
                                }
                                line = sr.ReadLine();
                                DataArray = line.Split(',');
                                if (DataArray.Length != pFace[i].nVerts) throw new Exception("Face List 資料格式錯誤");
                                for (int j = 0; j < pFace[i].nVerts; j++)
                                {
                                    pFace[i].pVert[j].colorIndex = int.Parse(DataArray[j]);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("不正確的資料項目");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"讀檔錯誤");
                return;
            }
        }

        public void DrawByOpenGL(bool DisableColor)
        {
            Vector3 norm;

            for (int i = 0; i < nFaces; i++)
            {
                Gl.glBegin(Gl.GL_POLYGON);
                if (pFace[i].pVert[0].normIndex < 0)
                {
                    CalNormal(pPt[pFace[i].pVert[0].vertIndex], pPt[pFace[i].pVert[1].vertIndex], pPt[pFace[i].pVert[2].vertIndex], out norm);
                    Gl.glNormal3f(norm.x, norm.y, norm.z);
                }
                for (int j = 0; j < pFace[i].nVerts; j++)
                {
                    if (!DisableColor)
                        Gl.glColor3ub(pColor[pFace[i].pVert[j].colorIndex].r, pColor[pFace[i].pVert[j].colorIndex].g, pColor[pFace[i].pVert[j].colorIndex].b);
                    if (pFace[i].pVert[j].normIndex >= 0)
                        Gl.glNormal3f(pNorm[pFace[i].pVert[j].normIndex].x, pNorm[pFace[i].pVert[j].normIndex].y, pNorm[pFace[i].pVert[j].normIndex].z);
                    Gl.glVertex3f(pPt[pFace[i].pVert[j].vertIndex].x, pPt[pFace[i].pVert[j].vertIndex].y, pPt[pFace[i].pVert[j].vertIndex].z);
                }
                Gl.glEnd();
            }
        }
        public void DrawByOpenGL()
        {
            Vector3 norm;

            for (int i = 0; i < nFaces; i++)
            {
                Gl.glBegin(Gl.GL_POLYGON);
                if (pFace[i].pVert[0].normIndex < 0)
                {
                    CalNormal(pPt[pFace[i].pVert[0].vertIndex], pPt[pFace[i].pVert[1].vertIndex], pPt[pFace[i].pVert[2].vertIndex], out norm);
                    Gl.glNormal3f(norm.x, norm.y, norm.z);
                }

                for (int j = 0; j < pFace[i].nVerts; j++)
                {
                    Gl.glColor3ub(pColor[pFace[i].pVert[j].colorIndex].r, pColor[pFace[i].pVert[j].colorIndex].g, pColor[pFace[i].pVert[j].colorIndex].b);
                    if (pFace[i].pVert[j].normIndex >= 0) 
                        Gl.glNormal3f(pNorm[pFace[i].pVert[j].normIndex].x, pNorm[pFace[i].pVert[j].normIndex].y, pNorm[pFace[i].pVert[j].normIndex].z);
                    Gl.glVertex3f(pPt[pFace[i].pVert[j].vertIndex].x, pPt[pFace[i].pVert[j].vertIndex].y, pPt[pFace[i].pVert[j].vertIndex].z);
                }
                Gl.glEnd();
            }
        }
    }
}
