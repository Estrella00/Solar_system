using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.DevIl;
using Show3DModels;

namespace _1052054_final
{
    public partial class Form1 : Form
    {
        double angle1;
        double angle2;
        double angle3, angle4, angle5, angle6, angle7, angle8;
        const double DEGREE_TO_RAD = 0.01745329; // 3.1415926/180
        double Radius = 100.0, Longitude = 90.0, Latitude = 0.0;
        Obj3DS model = new Obj3DS();

        private WMPLib.WindowsMediaPlayer wmp;  //music

        uint[] texName = new uint[9]; 

        public Form1()
        {
            InitializeComponent();
            this.simpleOpenGlControl1.InitializeContexts();
            Glut.glutInit();

            Il.ilInit();
            Ilu.iluInit();

        }

        private void simpleOpenGlControl1_Load(object sender, EventArgs e)
        {

            SetViewingVolumne();
            MyInit();
        }


        void SetViewingVolumne()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);  //矩陣模式:投影矩陣
            Gl.glLoadIdentity(); //仔入單位矩陣

            double aspect = (double)this.simpleOpenGlControl1.Size.Width / (double)this.simpleOpenGlControl1.Size.Height;
            Glu.gluPerspective(45, aspect, 0.1, 100.0);

            Gl.glViewport(0, 0, simpleOpenGlControl1.Size.Width,
                  simpleOpenGlControl1.Size.Height);    //大小設成跟視窗一樣大
        }


        void MyInit()
        {
            Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);//把畫面變黑色
           Gl.glClearDepth(1.0f);   //旋轉到後面部會覆蓋
            
            Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE);
            
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            //Gl.glEnable(Gl.GL_DEPTH_TEST);//深度測試

            //----標準動作，每個幾乎都會需要-------end---///
          /*  float[] global_ambient = new float[4] { 0.2f, 0.2f, 0.2f, 0.1f };
            float[] light0_ambient = new float[] { 0.2f, 0.2f, 0.2f };  //環境光
            float[] light0_diffuse = new float[] { 0.7f, 0.7f, 0.7f };  //散射光(顏色影響最多)
            float[] light0_specular = new float[] { 0.9f, 0.9f, 0.9f }; //鏡射光

            Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, global_ambient); //全域環境光的數值

            Gl.glLightModeli(Gl.GL_LIGHT_MODEL_LOCAL_VIEWER, Gl.GL_TRUE); //觀者位於場景內
            Gl.glLightModeli(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_FALSE); //只對物體正面進行光影計算 

            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, light0_ambient); //設定第一個光源的環境光成份
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, light0_diffuse); //設定第一個光源的散射光成份
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, light0_specular); //設定第一個光源的鏡射光成份
            */

           
            //設定紋理環境參數
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);

            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);//波離


            //Gl.glEnable(Gl.GL_LIGHTING);//光影計算
            //Gl.glEnable(Gl.GL_LIGHT0);//光影計算(關掉會沒光)

           // Gl.glEnable(Gl.GL_LIGHT1);

            Gl.glEnable(Gl.GL_NORMALIZE);


            //-------------poster------------//
            Gl.glGenTextures(9, texName); //產生紋理物件

            model.Load("C:\\Users\\user\\Desktop\\KWHA\\3DMODEL\\KWHA_VL.3DS");
            LoadTexture("image\\sol.jpg", texName[0]);
            LoadTexture("image\\me.jpg", texName[1]);
            LoadTexture("image\\ven.jpg", texName[2]);
            LoadTexture("image\\worldmap.jpg", texName[3]);
            LoadTexture("image\\mar.jpg", texName[4]);
            LoadTexture("image\\j.jpg", texName[5]);
            LoadTexture("image\\su.jpg", texName[6]);
            LoadTexture("image\\ur.jpg", texName[7]);
            LoadTexture("image\\ne.jpg", texName[8]);
            Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE);


          //  Gl.glEnable(Gl.GL_DEPTH_TEST);
           // Gl.glEnable(Gl.GL_LIGHTING);//光影計算
           // Gl.glEnable(Gl.GL_LIGHT0);//光影計算(關掉會沒光)
        }

        private void playBom() //播放撞擊音樂方法
        {
            wmp = new WMPLib.WindowsMediaPlayer();
            //var player1 = new WMPLib.WindowsMediaPlayer();
            wmp.URL = "space.wav"; //聲，我們的聲音檔像圖片一樣加入專案中。
        }

        private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
        {
            SetViewingVolumne();
        }
        private void LoadTexture(string filename, uint texture)
        {
            if (Il.ilLoadImage(filename)) //載入影像檔
            {
                int BitsPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL); //取得儲存每個像素的位元數
                int Depth = Il.ilGetInteger(Il.IL_IMAGE_DEPTH); //取得影像的深度值
                Ilu.iluScale(512, 512, Depth); //將影像大小縮放為2的指數次方
                Ilu.iluFlipImage(); //顛倒影像
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture); //連結紋理物件
                //設定紋理參數
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                //建立紋理物件
                if (BitsPerPixel == 24) Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, 512, 512, 0,
                 Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Il.ilGetInteger(Il.IL_IMAGE_TYPE), Il.ilGetData());
                if (BitsPerPixel == 32) Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 512, 512, 0,
                 Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Il.ilGetInteger(Il.IL_IMAGE_TYPE), Il.ilGetData());
            }
            else
            {   // 若檔案無法開啟，顯示錯誤訊息
                string message = "Cannot open file " + filename + ".";
                MessageBox.Show(message, "Image file open error!!!", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }
 void stars()
{

            
            Random rn = new Random();

            Gl.glPointSize(2.0f);
            Gl.glColor3ub(255, 255, 255);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);


            for (int i = 0; i < 10000; i++)
            {
                //-----星星大小隨機----//
                Gl.glPointSize(rn.Next(0,3));  //0~2之間大小, pointsize指令放在begin&end裡無效
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glVertex2i(rn.Next(0, this.simpleOpenGlControl1.Size.Width),
                              rn.Next(0, this.simpleOpenGlControl1.Size.Height));
                
                Gl.glEnd();
            }
           
        }


 void stars1()
 {


     Random rn = new Random();

     Gl.glPointSize(2.0f);
     Gl.glColor3ub(255, 255, 255);

     Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);


     for (int i = 0; i < 10000; i++)
     {
         //-----星星大小隨機----//
         Gl.glPointSize(rn.Next(0, 3));  //0~2之間大小, pointsize指令放在begin&end裡無效
         Gl.glBegin(Gl.GL_POINTS);
         Gl.glVertex2i(rn.Next(0, this.simpleOpenGlControl1.Size.Width),
                       rn.Next(0, this.simpleOpenGlControl1.Size.Height));

         Gl.glEnd();
     }

 }

 void manystars()
 {

     //上//

     Gl.glPushMatrix();
     Gl.glRotated(90, 0, 1.0f, 0);//绕X轴旋转90度
     Gl.glTranslated(-50.0, -50.0, -20.0);
     stars();
     Gl.glPopMatrix();
     //下//

     Gl.glPushMatrix();
     Gl.glRotated(90, 0, 1.0f, 0);//绕X轴旋转90度
     Gl.glTranslated(-50.0, -50.0, 20.0);
     stars1();
     Gl.glPopMatrix();
 
 }


        void DrawTorus(double Radius = 5.5, double TubeRadius = 1, int Sides = 20, int Rings = 20)
        {
            double sideDelta = 2.0 * Math.PI / Sides;
            double ringDelta = 2.0 * Math.PI / Rings;
            double theta = 0;
            double cosTheta = 1.0;
            double sinTheta = 0.0;

            double phi, sinPhi, cosPhi;
            double dist;

            Gl.glColor3ub(205, 170, 125);

            for (int i = 0; i < Rings; i++)
            {
                double theta1 = theta + ringDelta;
                double cosTheta1 = Math.Cos(theta1);
                double sinTheta1 = Math.Sin(theta1);

                Gl.glBegin(Gl.GL_QUAD_STRIP);
                phi = 0;
                for (int j = 0; j <= Sides; j++)
                {
                    phi = phi + sideDelta;
                    cosPhi = Math.Cos(phi);
                    sinPhi = Math.Sin(phi);
                    dist = Radius + (TubeRadius * cosPhi);

                    Gl.glNormal3d(cosTheta * cosPhi, sinTheta * cosPhi, sinPhi);
                    Gl.glVertex3d(cosTheta * dist, sinTheta * dist, TubeRadius * sinPhi);

                    Gl.glNormal3d(cosTheta1 * cosPhi, sinTheta1 * cosPhi, sinPhi);
                    Gl.glVertex3d(cosTheta1 * dist, sinTheta1 * dist, TubeRadius * sinPhi);
                }
                Gl.glEnd();
                theta = theta1;
                cosTheta = cosTheta1;
                sinTheta = sinTheta1;

            }


        }
     
        private void earth(double r, int Stacks, int Slices)
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[3]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();


              Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }

        private void sol(double r, int Stacks, int Slices)
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[0]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();


            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }
        private void mer(double r, int Stacks, int Slices) //水
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[1]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }

        private void venus(double r, int Stacks, int Slices)
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[2]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }

        private void mars(double r, int Stacks, int Slices)
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[4]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();


            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }
        private void jupi(double r, int Stacks, int Slices)
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[5]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();


            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }

        private void saturn(double r, int Stacks, int Slices)
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[6]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();


            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }

        private void uranus(double r, int Stacks, int Slices)
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[7]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();


            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }

        private void neptune(double r, int Stacks, int Slices)
        {

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName[8]); //連結紋理物件

            Gl.glEnable(Gl.GL_TEXTURE_2D); //開啟紋理映射功能


            double dp = 180.0 / Stacks;
            double dt = 360.0 / Slices;
            double x, y, z;
            Random rn = new Random(3);
            for (double phi = -(90 - dp); phi < 90 - dp; phi += dp)
            {

                Gl.glBegin(Gl.GL_QUAD_STRIP);


                for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
                {

                    x = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin((phi + dp) * Math.PI / 180.0);
                    z = r * Math.Cos((phi + dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + dp + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                    x = r * Math.Cos(phi * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                    y = r * Math.Sin(phi * Math.PI / 180.0);
                    z = r * Math.Cos(phi * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                    Gl.glNormal3d(x / r, y / r, z / r);

                    Gl.glTexCoord2d(theta / 360.0, (phi + 90) / 180.0);

                    Gl.glVertex3d(x, y, z);
                }
                Gl.glEnd();
            }

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, 1.0, 0.0);
            Gl.glVertex3d(0.0, r, 0.0);
            y = r * Math.Sin((90 - dp) * Math.PI / 180.0);
            for (double theta = 0.0; theta <= 360.0 + dt * 0.5; theta += dt)
            {

                x = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((90 - dp) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glNormal3d(0.0, -1.0, 0.0);
            Gl.glVertex3d(0.0, -r, 0.0);
            y = r * Math.Sin((dp - 90) * Math.PI / 180.0);
            for (double theta = 360.0; theta >= 0 - dt * 0.5; theta -= dt)
            {

                x = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Sin(theta * Math.PI / 180.0);
                z = r * Math.Cos((dp - 90) * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
                Gl.glNormal3d(x / r, y / r, z / r);
                Gl.glVertex3d(x, y, z);
            }
            Gl.glEnd();

            Gl.glDisable(Gl.GL_TEXTURE_2D);

        }
        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT /*| Gl.GL_DEPTH_BUFFER_BIT*/); //旋轉到後面部會覆蓋

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();


            Glu.gluLookAt(Radius * Math.Cos(Latitude * DEGREE_TO_RAD)
                     * Math.Sin(Longitude * DEGREE_TO_RAD),
              Radius * Math.Sin(Latitude * DEGREE_TO_RAD),
              Radius * Math.Cos(Latitude * DEGREE_TO_RAD)
                     * Math.Cos(Longitude * DEGREE_TO_RAD),
              0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
            //Glu.gluLookAt(60.0, 60.0, 60.0, 0.0, 0.0, -10.0, 0.0, 1.0, 0.0);

           /* float[] mat_ambient = new float[] { 0.0f, 0.1f, 0.0f, 1.0f }; //偏綠色材質
            float[] mat_diffuse = new float[] { 0.0f, 0.7f, 0.0f, 1.0f }; //偏綠色材質
            float[] mat_specular = new float[] { 0.9f, 0.9f, 0.9f, 1.0f };
            float mat_shininess = 32.0f; //閃亮係數的數值
            

            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, mat_ambient); //設定材質的環境光反射係數
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, mat_diffuse); //設定材質的散射光反射係數
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, mat_specular); //設定材質的鏡射光反射係數
            Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, mat_shininess); //設定材質的閃亮係數
*/

            Gl.glEnable(Gl.GL_COLOR_MATERIAL);

            

            manystars();

            //---sun----//
            Gl.glPushMatrix();
            Gl.glColor3ub(255, 255, 255);
           // Glut.glutSolidSphere(5.0, 20, 20);
            sol(16.0, 20, 20);
            Gl.glPopMatrix();


            //----1-----mercury水----------//
            Gl.glColor3ub(255, 255, 255);
            Gl.glPushMatrix();
            Gl.glRotated(angle1, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 18.0);
            //Glut.glutSolidSphere(1.0, 20, 20);
            mer(0.5, 20, 20);
            Gl.glPopMatrix();

            //----2-----venus金----------//
            Gl.glColor3ub(255, 255, 255);
            Gl.glPushMatrix();
            Gl.glRotated(angle2, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 22.0);
           // Glut.glutSolidSphere(1.0, 20, 20);
            venus(1.1, 20, 20);
            Gl.glPopMatrix();

            //----3-----earth---地-------//
            Gl.glColor3d(255, 255, 255);
            Gl.glPushMatrix();
            Gl.glRotated(angle3, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 26.0);
            //Glut.glutSolidSphere(1.0, 20, 20);
            earth(1.6, 20, 20);
            Gl.glPopMatrix();


            //----4-----mars---火-------//
            Gl.glColor3d(255, 255, 255);
            Gl.glPushMatrix();
            Gl.glRotated(angle4, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 30.0);
            //Glut.glutSolidSphere(1.0, 20, 20);
            mars(1.0, 20, 20);
            Gl.glPopMatrix();


            //----5-----jupiter---木-------//
            Gl.glColor3d(255, 255, 255);
            Gl.glPushMatrix();
            Gl.glRotated(angle5, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 37.0);
            //Glut.glutSolidSphere(1.0, 20, 20);
            jupi(4, 20, 20);
            Gl.glPopMatrix();


            //----6-----saturn---土-------//
            Gl.glColor3d(255, 255, 255);
            Gl.glPushMatrix();
            Gl.glRotated(angle6, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 47.0);
           // Glut.glutSolidSphere(1.0, 20, 20);
            saturn(2.8, 20, 20);
            
            Gl.glPopMatrix();


            //-----圓環1-----//
            
            Gl.glPushMatrix();
            Gl.glRotated(angle6, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 47.0);
            Gl.glRotated(70, 1.0f, 0, 0);//绕X轴旋转90度
            Gl.glScaled(1, 1, 0.2);
            DrawTorus();
            Gl.glPopMatrix();

            //-----圓環2-----//
           
            Gl.glPushMatrix();
            Gl.glRotated(angle6, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 47.0);
            Gl.glRotated(70, 1.0f, 0, 0);//绕X轴旋转90度
            Gl.glScaled(0.65, 0.65, 0.2);
            DrawTorus();
            Gl.glPopMatrix();


            //----7-----uranus---天王-------//
            Gl.glColor3d(255, 255, 255);
            Gl.glPushMatrix();
            Gl.glRotated(angle7, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 55.0);
            //Glut.glutSolidSphere(1.0, 20, 20);
            uranus(2.1, 20, 20);
            Gl.glPopMatrix();

          



            //----8-----neptune---海王-------//
         
            Gl.glColor3d(255, 255, 255);
            Gl.glPushMatrix();
            Gl.glRotated(angle8, 0.0, 1.0, 0.0);
            Gl.glTranslated(0.0, 0.0, 60.0);
           // Glut.glutSolidSphere(1.0, 20, 20);
            neptune(1.7, 20, 20);
            Gl.glPopMatrix();

           //----虎鯨---//
            Gl.glPushMatrix();
            Gl.glTranslated(0.0, 25.0, 0.0);
            Gl.glRotated(270, 1.0f, 0, 0);//绕X轴旋转90度
            Gl.glScaled(0.005f, 0.005f, 0.005f);
            model.render();
            Gl.glPopMatrix();
            
        }


        private void simpleOpenGlControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    Longitude -= 5.0;
                    break;
                case Keys.Right:
                    Longitude += 5.0;
                    break;
                case Keys.Up:
                    Latitude += 5.0;
                    if (Latitude >= 90.0) Latitude = 89.0;
                    break;
                case Keys.Down:
                    Latitude -= 5.0;
                    if (Latitude <= -90.0) Latitude = -89.0;
                    break;
                case Keys.PageUp:
                    Radius += 5.0;
                    break;
                case Keys.PageDown:
                    Radius -= 5.0;
                    if (Radius < 10.0) Radius = 10.0;
                    break;
                case Keys.Space:
                    //timer1.Enabled = false;
                    if (timer1.Enabled == true)
                    {
                        timer1.Enabled = false;
                    }
                    else 
                    {
                        timer1.Enabled = true;
                    }
                    break;
                
                case Keys.M:
                    
                        playBom();
                  
                    break;

                case Keys.S:

                    wmp.controls.stop();

                    break;
                default:
                    break;
            }
            this.simpleOpenGlControl1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            angle1 += 8;
            angle2 += 7;
            angle3 += 6;
            angle4 += 5;
            angle5 += 4;
            angle6 += 3;
            angle7 += 2;
            angle8 += 1;
           
            this.simpleOpenGlControl1.Refresh();
        }
    }
}
