using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.DevIl;

namespace Show3DModels
{
    struct ChunkInfo
    {
        public ushort ID;
        public int Size;
    }
 
    struct Vertex
    {
        public float x,y,z;
    }
 
    struct Face // triangle face
    {
        public ushort p1,p2,p3;
    }
 
    struct UVCoord
    {
        public float U,V;
    }

    struct ObjMesh
    {
        public Vertex[] vtx;
        public Vertex[] norm;
        public int nVtx, nFace, nUV;
        public Face[] face;
        public UVCoord[] UV;
        //public string MatName;
    }
 
    class ObjMatl
    {
        public string Name;
        public float[] ambient;
        public float[] diffuse;
        public float[] specular;
        public float shininess;
        public string TexName;
        public int Smooth;
        public int[] nFace;
        public int[][] FaceList;
        public bool UseTexture;
        public ObjMatl()
        {
            ambient = new float[4];
            diffuse = new float[4];
            specular = new float[4];
            ambient[0] = ambient[1] = ambient[2] = 0.588235f;
            diffuse[0] = diffuse[1] = diffuse[2] = 0.0f;
            specular[0] = specular[1] = specular[2] = 0.898039f;
            ambient[3] = diffuse[3] = specular[3] = 1.0f;
            shininess = 100.0f;
            TexName = null;
            UseTexture = false;
        }
    }

    class Obj3DS
    {
        int fSize;
        int nMesh,nMatl;
        byte[] data;
        uint[] TextureID;
        string path;

        ObjMesh[] Mesh;
        ObjMatl[] Matl;

        public Obj3DS()
        {
            fSize = 0;
            Mesh = null;
            Matl = null;
            path = null;
            data = null;
            TextureID = null;
        }

        int CountParts(int DataLen)
        {
            int Offset=6;
            ChunkInfo Info;

            nMesh=0;
            nMatl=0;

            while(Offset<DataLen)
            {
                Info=GetChunkInfo(Offset);

                switch(Info.ID)
                {
                    case 0x3D3D: // Start of Obj
                        Offset+=6;
                        break;

                    case 0x4000: // Start of Mesh
                    nMesh++;
                    Offset+=Info.Size;
                    break;

                    case 0xAFFF: // Start of Material
                    nMatl++;
                    Offset+=Info.Size;
                    break;

                    default:
                    Offset+=Info.Size;
                    break;
                }
            }
            return nMesh;
        }

        ChunkInfo GetChunkInfo(int Offset)
        {
            ChunkInfo Chunk;
            Chunk.ID = BitConverter.ToUInt16(data, Offset);
            Chunk.Size = BitConverter.ToInt32(data, Offset + 2);
            return Chunk;
        }

        public bool Load(string fName)
        {
            ChunkInfo Info;
            int Offset,SubChunkSize,Value,MatDex,MatIndex,MeshDex,Loop,LOff,strlen;
            short Val;
            ushort uVal; 
            float fVal;
            char[] ch_array = new char[256];
            const int NONE = 0, AMBIENT = 1, DIFFUSE = 2, SPECULAR = 3, SHININESS = 4;
            int field=NONE;

            int n = fName.LastIndexOf("\\");
            if (n > 0) path = fName.Substring(0, n+1);

            MatDex=-1;
            MeshDex=-1;
            MatIndex = 0;

            if(File.Exists(fName)==false) 
            {
                return false;
            }

            // Get filelength
            FileInfo fInfo = new FileInfo(fName);
            fSize = (int)(fInfo.Length);

            data = new byte[fSize];

            // Open file
            using (BinaryReader br = new BinaryReader(File.Open(fName, FileMode.Open, FileAccess.Read)))
            {
                data = br.ReadBytes(fSize);
            }

            // Check header
            Offset=0;
            Info=GetChunkInfo(Offset);

            if(Info.ID!=0x4D4D) return false;

            if(Info.Size!=fSize) return false;

            CountParts(fSize);

            Mesh=new ObjMesh[nMesh];
            Matl=new ObjMatl[nMatl];

            for (int i = 0; i < nMatl; i++)
            {
                Matl[i] = new ObjMatl();
                Matl[i].FaceList = new int[nMesh][];
                Matl[i].nFace = new int[nMesh];
            }

            Offset=6;

            while(Offset<fSize)
            {
                Info=GetChunkInfo(Offset);
                switch(Info.ID)
                {
                    case 0x0002: // Version
                        Value = BitConverter.ToInt32(data,Offset+6);
                        Offset+=Info.Size;
                        break;

                    case 0x0010: // RGB1 (float)
                        if (field != NONE)
                        {
                            switch (field)
                            {
                                case AMBIENT:
                                    Matl[MatDex].ambient[0] = BitConverter.ToSingle(data, Offset + 6);
                                    Matl[MatDex].ambient[1] = BitConverter.ToSingle(data, Offset + 10);
                                    Matl[MatDex].ambient[2] = BitConverter.ToSingle(data, Offset + 14); 
                                    break;
                                case DIFFUSE:
                                    Matl[MatDex].diffuse[0] = BitConverter.ToSingle(data, Offset + 6);
                                    Matl[MatDex].diffuse[1] = BitConverter.ToSingle(data, Offset + 10);
                                    Matl[MatDex].diffuse[2] = BitConverter.ToSingle(data, Offset + 14); 
                                    break;
                                case SPECULAR:
                                    Matl[MatDex].specular[0] = BitConverter.ToSingle(data, Offset + 6);
                                    Matl[MatDex].specular[1] = BitConverter.ToSingle(data, Offset + 10);
                                    Matl[MatDex].specular[2] = BitConverter.ToSingle(data, Offset + 14); 
                                    break;
                                case SHININESS:
                                    Matl[MatDex].shininess = BitConverter.ToSingle(data, Offset + 6); 
                                    break;
                            }
                            field = NONE;
                        }
                        Offset += Info.Size;
                        break;

                    case 0x0011: // RGB1 (byte)
                        if(field!=NONE)
                        {
                            switch(field)
                            {
                                case AMBIENT:
                                    Matl[MatDex].ambient[0] = data[Offset + 6] / 255.0f;
                                    Matl[MatDex].ambient[1] = data[Offset + 7] / 255.0f;
                                    Matl[MatDex].ambient[2] = data[Offset + 8] / 255.0f;
                                    break;
                                case DIFFUSE:
                                    Matl[MatDex].diffuse[0] = data[Offset + 6] / 255.0f;
                                    Matl[MatDex].diffuse[1] = data[Offset + 7] / 255.0f;
                                    Matl[MatDex].diffuse[2] = data[Offset + 8] / 255.0f;
                                    break;
                                case SPECULAR:
                                    Matl[MatDex].specular[0] = data[Offset + 6] / 255.0f;
                                    Matl[MatDex].specular[1] = data[Offset + 7] / 255.0f;
                                    Matl[MatDex].specular[2] = data[Offset + 8] / 255.0f;
                                    break;
                                case SHININESS:
                                    Matl[MatDex].shininess = data[Offset + 6] / 255.0f;
                                    break;
                            }
                            field = NONE;
                        }
                        Offset+=Info.Size;
                        break;
 
                    case 0x0012: // RGB2
                        Offset+=Info.Size;
                        break;
 
                    case 0x0030: // Quantity value for parent chunks (int)
                        if (field == SHININESS)
                        {
                            Matl[MatDex].shininess = BitConverter.ToInt16(data, Offset + 6)/100.0f;
                            field = NONE;
                        }
                        Offset+=Info.Size;
                        break;

                    case 0x0031: // Quantity value for parent chunks (float)
                        if (field == SHININESS)
                        {
                            Matl[MatDex].shininess = BitConverter.ToSingle(data, Offset + 6); 
                            field = NONE;
                        }
                        Offset += Info.Size;
                        break;
 
                    case 0x0100: // Config (Ignore)
                        Offset+=Info.Size;
                        break;
 
                    case 0x3D3D: // Start of Obj
                        SubChunkSize=Info.Size+Offset; // Set end limit for subchunk
                        Offset+=6;
                        break;
 
                    case 0x3D3E: // Editor config (Ignore)
                        Offset+=Info.Size;
                        break;
 
                    case 0x4000: // Start of Mesh
                        MeshDex++;
                        Offset += 6;
                        while (data[Offset] != 0) // Seek end of string
                            Offset++;
                        Offset++; // One more to move past the NULL
                        break;

                    case 0x4100: // Mesh data
                        Offset+=6;
                        break;
 
                    case 0x4110: // Vertex List
                        uVal = BitConverter.ToUInt16(data,Offset+6);
                        Mesh[MeshDex].nVtx=uVal;
                        Mesh[MeshDex].vtx = new Vertex[uVal+1];
                        for(Loop=0,LOff=Offset+8;Loop!=uVal;++Loop,LOff+=12)
                        {
                            Mesh[MeshDex].vtx[Loop].x = BitConverter.ToSingle(data,LOff);
                            Mesh[MeshDex].vtx[Loop].y = BitConverter.ToSingle(data,LOff+4);
                            Mesh[MeshDex].vtx[Loop].z = BitConverter.ToSingle(data,LOff+8);
                        }
                        Offset+=Info.Size;
                        break;
 
                    case 0x4111: // Vertex Options
                        Offset+=Info.Size;
                        break;
 
                    case 0x4120: // Face List
                        uVal = BitConverter.ToUInt16(data,Offset+6);
                        Mesh[MeshDex].nFace=uVal;
                        Mesh[MeshDex].face = new Face[uVal+1];
                        for(Loop=0,LOff=Offset+8;Loop!=uVal;++Loop,LOff+=8)
                        {
                            Mesh[MeshDex].face[Loop].p1 = BitConverter.ToUInt16(data,LOff);
                            Mesh[MeshDex].face[Loop].p2 = BitConverter.ToUInt16(data,LOff+2);
                            Mesh[MeshDex].face[Loop].p3 = BitConverter.ToUInt16(data,LOff+4);
                        }
                        Offset += (6 + 2 + uVal * 8);
                        MatIndex = 0;
                        break;
 
                    case 0x4130: // Face Material Desc
                        if (MatIndex < nMatl)
                        {
                            for (strlen = 0; strlen < 256; strlen++)
                            {
                                if (data[Offset + 6 + strlen] != '\0') ch_array[strlen] = (char)data[Offset + 6 + strlen];
                                else break;
                            }
                            string mat_name = new string(ch_array, 0, strlen);
                            Matl[MatIndex].nFace[MeshDex] = BitConverter.ToUInt16(data, Offset + 6 + strlen + 1);
                            Matl[MatIndex].FaceList[MeshDex] = new int[Matl[MatIndex].nFace[MeshDex]];
                            for (Loop = 0, LOff = Offset + 6 + strlen + 1 + 2; Loop < Matl[MatIndex].nFace[MeshDex]; Loop++, LOff += 2)
                            {
                                Matl[MatIndex].FaceList[MeshDex][Loop] = BitConverter.ToUInt16(data, LOff);
                            }
                            MatIndex++;
                        }
                        Offset += Info.Size;
                        break;
 
                    case 0x4140: // UV Map List
                        uVal = BitConverter.ToUInt16(data,Offset+6);
                        Mesh[MeshDex].nUV=uVal;
                        Mesh[MeshDex].UV = new UVCoord[uVal+1];
                        for(Loop=0,LOff=Offset+8;Loop!=uVal;++Loop,LOff+=8)
                        {
                            Mesh[MeshDex].UV[Loop].U = BitConverter.ToSingle(data,LOff);
                            Mesh[MeshDex].UV[Loop].V = BitConverter.ToSingle(data,LOff+4);
                        }
                        Offset+=Info.Size;
                        break;
 
                    case 0xA000: // Material Name
                        for (Loop = 0; Loop < 256; Loop++)
                        {
                            if (data[Offset + 6 + Loop] != '\0') ch_array[Loop] = (char)data[Offset + 6 + Loop];
                            else break;
                        }
                        Matl[MatDex].Name = new string(ch_array, 0, Loop);
                        Offset+=Info.Size;
                        break;
 
                    case 0xA010: // Material - Ambient Color
                        field = AMBIENT;
                        Offset+=6;//Info.Size;
                        break;
 
                    case 0xA020: // Material - Diffuse Color
                        field = DIFFUSE;
                        Offset+=6;//Info.Size;
                        break;

                    case 0xA030: // Material - Spec Color
                        field = SPECULAR;
                        Offset+=6;//Info.Size;
                        break;

                    case 0xA040: // Material - Shininess
                        field = SHININESS;
                        Offset+=6;//Info.Size;
                        break;
 
                    case 0xA041: // Material - Shine Strength
                        Offset+=6;//Info.Size;
                        break;
 
                    case 0xA050: // Material - Transparency
                        Offset+=6;//Info.Size;
                        break;
 
                    case 0xA100: // Material - Type (Flat,Gourad, Phong, Metal)
                        Val = BitConverter.ToInt16(data,Offset+6);
                        Matl[MatDex].Smooth=Val;
                        Offset+=Info.Size;
                    break;
 
                    case 0xA200: // Material - Start of Texture Info
                        Offset+=6;
                        break;
 
                    case 0xA300: // Material - Texture Name
                        for (Loop = 0; Loop < 256; Loop++)
                        {
                            if (data[Offset + 6 + Loop] != 0) ch_array[Loop] = (char)data[Offset + 6 + Loop];
                            else break;
                        }
                        Matl[MatDex].TexName = new string(ch_array, 0, Loop);
                        Offset+=Info.Size;
                        break;
 
                    case 0xA351: // Material - Texture Options
                        Val = BitConverter.ToInt16(data,Offset+6);
                        Offset+=Info.Size;
                        break;
 
                    case 0xA354: // Material - Texture U Scale
                        fVal = BitConverter.ToSingle(data,Offset+6);
                        Offset+=Info.Size;
                        break;
 
                    case 0xA356: // Material - Texture V Scale
                        fVal = BitConverter.ToSingle(data,Offset+6);
                        Offset+=Info.Size;
                        break;
 
                    case 0xA35A: // Material - Texture V Offset
                        fVal = BitConverter.ToSingle(data,Offset+6);
                        Offset+=Info.Size;
                        break;
 
                    case 0xA35C: // Material - Texture V Offset
                        fVal = BitConverter.ToSingle(data,Offset+6);
                        Offset+=Info.Size;
                        break;

                    case 0xAFFF: // Material Start
                        MatDex++;
                        Offset+=6;
                        break;
 
                    default:
                        Offset+=Info.Size;
                        break;
                }
            }
            //generateFaceNormals();
            generateSmoothNormals();
            LoadTextures();
            return true;
        }

        public void CalNormal(Vertex p1, Vertex p2, Vertex p3, out Vertex normal)
        {
            float[] v1 = new float[3], v2 = new float[3];
            float len;

            v1[0] = p2.x - p1.x;
            v1[1] = p2.y - p1.y;
            v1[2] = p2.z - p1.z;

            v2[0] = p3.x - p1.x;
            v2[1] = p3.y - p1.y;
            v2[2] = p3.z - p1.z;

            normal.x = v1[1] * v2[2] - v1[2] * v2[1];
            normal.y = v1[2] * v2[0] - v1[0] * v2[2];
            normal.z = v1[0] * v2[1] - v1[1] * v2[0];

            // normalize
            len = normal.x * normal.x + normal.y * normal.y + normal.z * normal.z;
            if (len > 0.0)
            {
                len = (float)Math.Sqrt(len);
                normal.x /= len;
                normal.y /= len;
                normal.z /= len;
            }
        }

        private void generateFaceNormals()
        {
            for (int i = 0; i < nMesh; i++)
            {
                Mesh[i].norm = new Vertex[Mesh[i].nFace];

                for (int j = 0; j < Mesh[i].nFace; j++)
                {
                    CalNormal(Mesh[i].vtx[Mesh[i].face[j].p1], Mesh[i].vtx[Mesh[i].face[j].p2], Mesh[i].vtx[Mesh[i].face[j].p3], out Mesh[i].norm[j]);
                }
            }
        }

        private void generateSmoothNormals()
        {
            for (int i = 0; i < nMesh; i++)
            {
                if (Mesh[i].nFace > 0)
                {
                    Vertex[] TmpNormals = new Vertex[Mesh[i].nFace];

                    for (int j = 0; j < Mesh[i].nFace; j++)
                    {
                        CalNormal(Mesh[i].vtx[Mesh[i].face[j].p1], Mesh[i].vtx[Mesh[i].face[j].p2], Mesh[i].vtx[Mesh[i].face[j].p3],out TmpNormals[j]);
                    }

                    Mesh[i].norm = new Vertex[Mesh[i].nVtx];

                    for (int j = 0; j < Mesh[i].nVtx; j++)
                    {
                        Vertex thisNormal;
                        thisNormal.x = thisNormal.y = thisNormal.z = 0.0f;
                        for (int k = 0; k < Mesh[i].nFace; k++)
                        {
                            if (Mesh[i].face[k].p1 == j)
                            {
                                thisNormal.x += TmpNormals[k].x;
                                thisNormal.y += TmpNormals[k].y;
                                thisNormal.z += TmpNormals[k].z;
                            }
                            if (Mesh[i].face[k].p2 == j)
                            {
                                thisNormal.x += TmpNormals[k].x;
                                thisNormal.y += TmpNormals[k].y;
                                thisNormal.z += TmpNormals[k].z;
                            }
                            if (Mesh[i].face[k].p3 == j)
                            {
                                thisNormal.x += TmpNormals[k].x;
                                thisNormal.y += TmpNormals[k].y;
                                thisNormal.z += TmpNormals[k].z;
                            }
                        }
                        float len = thisNormal.x * thisNormal.x + thisNormal.y * thisNormal.y + thisNormal.z * thisNormal.z;
                        if (len > 0)
                        {
                            len = (float)Math.Sqrt(len);
                            Mesh[i].norm[j].x = thisNormal.x / len;
                            Mesh[i].norm[j].y = thisNormal.y / len;
                            Mesh[i].norm[j].z = thisNormal.z / len;
                        }
                    }

                }
            }
        }

        public void render()
        {
            for (int i = 0; i < nMesh; i++)
            {
                for (int j = 0; j < nMatl; j++)
                {
                    if (Matl[j].UseTexture)
                    {
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, TextureID[j]);
                        Gl.glEnable(Gl.GL_TEXTURE_2D);
                    }
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, Matl[j].ambient);
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, Matl[j].diffuse);
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, Matl[j].specular);
                    //Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, Matl[j].shininess * 256.0f);
                    Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 100.0f);
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    for (int k = 0; k < Matl[j].nFace[i]; k++)
                    {
                        int v1 = Mesh[i].face[Matl[j].FaceList[i][k]].p1;
                        int v2 = Mesh[i].face[Matl[j].FaceList[i][k]].p2;
                        int v3 = Mesh[i].face[Matl[j].FaceList[i][k]].p3;
                        if (Matl[j].UseTexture && Mesh[i].UV!=null) Gl.glTexCoord2f(Mesh[i].UV[v1].U, Mesh[i].UV[v1].V);
                        Gl.glNormal3f(Mesh[i].norm[v1].x, Mesh[i].norm[v1].y, Mesh[i].norm[v1].z);
                        Gl.glVertex3f(Mesh[i].vtx[v1].x, Mesh[i].vtx[v1].y, Mesh[i].vtx[v1].z);
                        if (Matl[j].UseTexture && Mesh[i].UV!=null) Gl.glTexCoord2f(Mesh[i].UV[v2].U, Mesh[i].UV[v2].V);
                        Gl.glNormal3f(Mesh[i].norm[v2].x, Mesh[i].norm[v2].y, Mesh[i].norm[v2].z);
                        Gl.glVertex3f(Mesh[i].vtx[v2].x, Mesh[i].vtx[v2].y, Mesh[i].vtx[v2].z);
                        if (Matl[j].UseTexture && Mesh[i].UV != null) Gl.glTexCoord2f(Mesh[i].UV[v3].U, Mesh[i].UV[v3].V);
                        Gl.glNormal3f(Mesh[i].norm[v3].x, Mesh[i].norm[v3].y, Mesh[i].norm[v3].z);
                        Gl.glVertex3f(Mesh[i].vtx[v3].x, Mesh[i].vtx[v3].y, Mesh[i].vtx[v3].z);
                    }
                    Gl.glEnd();
                    if (Matl[j].UseTexture) Gl.glDisable(Gl.GL_TEXTURE_2D);
                }
            }
        }

        public void renderByFlatShading()
        {
            for (int i = 0; i < nMesh; i++)
            {
                for (int j = 0; j < nMatl; j++)
                {
                    if (Matl[j].UseTexture)
                    {
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, TextureID[j]);
                        Gl.glEnable(Gl.GL_TEXTURE_2D);
                    }
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, Matl[j].ambient);
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, Matl[j].diffuse);
                    Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, Matl[j].specular);
                    //Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, Matl[j].shininess * 256.0f);
                    Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 256.0f);
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    for (int k = 0; k < Matl[j].nFace[i]; k++)
                    {
                        int v1 = Mesh[i].face[Matl[j].FaceList[i][k]].p1;
                        int v2 = Mesh[i].face[Matl[j].FaceList[i][k]].p2;
                        int v3 = Mesh[i].face[Matl[j].FaceList[i][k]].p3;
                        Gl.glNormal3f(Mesh[i].norm[Matl[j].FaceList[i][k]].x, Mesh[i].norm[Matl[j].FaceList[i][k]].y, Mesh[i].norm[Matl[j].FaceList[i][k]].z);
                        Gl.glTexCoord2f(Mesh[i].UV[v1].U, Mesh[i].UV[v1].V);
                        Gl.glVertex3f(Mesh[i].vtx[v1].x, Mesh[i].vtx[v1].y, Mesh[i].vtx[v1].z);
                        Gl.glTexCoord2f(Mesh[i].UV[v2].U, Mesh[i].UV[v2].V);
                        Gl.glVertex3f(Mesh[i].vtx[v2].x, Mesh[i].vtx[v2].y, Mesh[i].vtx[v2].z);
                        Gl.glTexCoord2f(Mesh[i].UV[v3].U, Mesh[i].UV[v3].V);
                        Gl.glVertex3f(Mesh[i].vtx[v3].x, Mesh[i].vtx[v3].y, Mesh[i].vtx[v3].z);
                    }
                    Gl.glEnd();
                    if (Matl[j].UseTexture) Gl.glDisable(Gl.GL_TEXTURE_2D);
                }
            }
        }


        private void LoadTexture(string filename, uint texture)
        {
            int TextureSizeX,TextureSizeY;

            if (Il.ilLoadImage(filename))
            {
                int Width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int Height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int BitsPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);

                //Ilu.iluImageParameter(Ilu.ILU_FILTER, Ilu.ILU_BILINEAR);
                //Ilu.iluImageParameter(Ilu.ILU_FILTER, Ilu.ILU_NEAREST);

                if (Width > 128) TextureSizeX = 128;
                else TextureSizeX = 64;
                if (Height > 128) TextureSizeY = 128;
                else TextureSizeY = 64;

                Ilu.iluScale(TextureSizeX, TextureSizeY, 1);

                string ext=null;
                int n = filename.LastIndexOf(".");
                if (n > 0) ext = filename.Substring(n+1);

                if (ext.ToLower() != "bmp") Ilu.iluFlipImage();

                // bind to our texture object we just created
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture);

                // set the filtering and wrap modes for the texture
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                if (BitsPerPixel == 24) Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, TextureSizeX, TextureSizeY, 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Il.ilGetInteger(Il.IL_IMAGE_TYPE), Il.ilGetData());
                if (BitsPerPixel == 32) Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, TextureSizeX, TextureSizeY, 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Il.ilGetInteger(Il.IL_IMAGE_TYPE), Il.ilGetData());
            }
            else
            {
                string message = "Cannot open file " + filename + ".";
                MessageBox.Show(message, "Image file open error!!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void LoadTextures()
        {
            TextureID = new uint[nMatl];
            Gl.glGenTextures(nMatl, TextureID);
            for (int i = 0; i < nMatl; i++)
            {
                if (Matl[i].TexName != null)
                {
                    LoadTexture(path+Matl[i].TexName, TextureID[i]);
                    Matl[i].UseTexture = true;
                }
                else Matl[i].UseTexture = false;
            }
            //Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            //Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
        }

        public void CenterAndScaleModel(float scale)
        {
            float minX, maxX, minY, maxY, minZ, maxZ;
            float cx,cy,cz,s;

            minX = minY = minZ = float.MaxValue;
            maxX = maxY = maxZ = float.MinValue;
            for (int i = 0; i < nMesh; i++)
            {
                for (int j = 0; j < Mesh[i].nVtx; j++)
                {
                    if (Mesh[i].vtx[j].x > maxX) maxX = Mesh[i].vtx[j].x;
                    if (Mesh[i].vtx[j].x < minX) minX = Mesh[i].vtx[j].x;
                    if (Mesh[i].vtx[j].y > maxY) maxY = Mesh[i].vtx[j].y;
                    if (Mesh[i].vtx[j].y < minY) minY = Mesh[i].vtx[j].y;
                    if (Mesh[i].vtx[j].z > maxZ) maxZ = Mesh[i].vtx[j].z;
                    if (Mesh[i].vtx[j].z < minZ) minZ = Mesh[i].vtx[j].z;
                }
            }

            cx = (maxX + minX) * 0.5f;
            cy = (maxY + minY) * 0.5f;
            cz = (maxZ + minZ) * 0.5f;
            s = ((maxX - minX) > (maxY - minY)) ? (maxX - minX) : (maxY - minY);
            s = (s > (maxZ - minZ)) ? s : (maxZ - minZ);
            s = (scale/s);

            for (int i = 0; i < nMesh; i++)
            {
                for (int j = 0; j < Mesh[i].nVtx; j++)
                {
                    Mesh[i].vtx[j].x = (Mesh[i].vtx[j].x - cx) * s;
                    Mesh[i].vtx[j].y = (Mesh[i].vtx[j].y - cy) * s;
                    Mesh[i].vtx[j].z = (Mesh[i].vtx[j].z - cz) * s; 
                }
            }
            //generateFaceNormals();
            generateSmoothNormals();
        }
    }
}
