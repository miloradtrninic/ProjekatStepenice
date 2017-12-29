using System;
using System.Collections;
using System.ComponentModel;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Core;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using AssimpSample;
using SharpGL.SceneGraph;
using Rectangle = System.Drawing.Rectangle;

namespace ProjekatStepenice
{
    public class World : INotifyPropertyChanged
    {
        #region Atributi
        public event PropertyChangedEventHandler PropertyChanged;
        private float[] refleksionaBoja = {0.0f, 0.0f, 1.0f, 1.0f};

        private Sphere lampA;
        private Sphere lampR;

        private DispatcherTimer timer1;

        private float korakX = 0;
        private float korakZ = 0;

        private float korakGore = 0;
        private float korakNapred = 0;


        private bool animationEnd = true;

       

        private static int stepeniceCount = 20;

        private float[] stepenicaX = new float[stepeniceCount];
        private float[] stepenicaZ = new float[stepeniceCount];

        private int stepenicaCurrentX = 0;
        private int stepenicaCurrentZ = 0;


        private uint[] m_textures = null;

        private enum TextureObjects
        {
            Metal = 0,
            Plocice
        };

        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        private string[] m_textureFiles =
        {
            "..//..//images//metal.jpg", "..//..//images//tile.jpg"
        };
        private float fatindex = 0;
        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        private AssimpScene m_scene;

        private float m_xRotation = 0.0f;


        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose
        /// </summary>
        private float m_yRotation = 0.0f;

        private float m_eyeX = 0.0f;
        private float m_eyeY = 0.0f;
        private float m_eyeZ = 0.0f;

        private float m_centerX = 0f;
        private float m_centerY = 0f;
        private float m_centerZ = -1.0f;

        private float m_upX = 0.0f;
        private float m_upY = 1.0f;
        private float m_upZ = 0.0f;

        private float m_zDistance;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private float m_sceneDistance = 7500.0f;

        #endregion

        #region Properties

        public float[] RefleksionaBoja
        {
            get { return refleksionaBoja; }
            set { refleksionaBoja = value; }
        }

        public float Fatindex
        {
            get { return fatindex; }
            set { fatindex = value; }
        }
        public bool AnimationEnd
        {
            get { return animationEnd; }
            set
            {
                animationEnd = value;
                OnPropertyChanged("AnimationEnd");
            }
        }
        public float MXRotation
        {
            get { return m_xRotation; }
            set
            {
                if (value > -8 && value < 45 * 2)
                {
                    m_xRotation = value;
                }
            }
        }

        public float MYRotation
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public int MWidth
        {
            get { return m_width; }
            set { m_width = value; }
        }

        public AssimpScene MScene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public float MXRotation1
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        public float MYRotation1
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public float MEyeX
        {
            get { return m_eyeX; }
            set { m_eyeX = value; }
        }

        public float MEyeY
        {
            get { return m_eyeY; }
            set { m_eyeY = value; }
        }

        public float MEyeZ
        {
            get { return m_eyeZ; }
            set { m_eyeZ = value; }
        }

        public float MCenterX
        {
            get { return m_centerX; }
            set { m_centerX = value; }
        }

        public float MCenterY
        {
            get { return m_centerY; }
            set { m_centerY = value; }
        }

        public float MCenterZ
        {
            get { return m_centerZ; }
            set { m_centerZ = value; }
        }

        public float MUpX
        {
            get { return m_upX; }
            set { m_upX = value; }
        }

        public float MUpY
        {
            get { return m_upY; }
            set { m_upY = value; }
        }

        public float MUpZ
        {
            get { return m_upZ; }
            set { m_upZ = value; }
        }

        public int MHeight
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public float MSceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        public float MZDistance
        {
            get { return m_zDistance; }
            set { m_zDistance = value; }
        }

        #endregion

        #region Konstruktori

        /// <summary>
        ///		Konstruktor opengl sveta.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;

            m_textures = new uint[m_textureCount];
        }

        #endregion

        #region Metode

        /// <summary>
        /// Korisnicka inicijalizacija i podesavanje OpenGL parametara
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            lampA = new Sphere();
            lampA.CreateInContext(gl);
            lampA.Radius = 5f;
            lampA.Material = new SharpGL.SceneGraph.Assets.Material();

            lampR = new Sphere();
            lampR.CreateInContext(gl);
            lampR.Radius = 5f;
            lampR.Material = new SharpGL.SceneGraph.Assets.Material();

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);

            //testiranje dubine
            gl.Enable(OpenGL.GL_DEPTH_TEST);


            SetupLighting(gl);
            SetupTextures(gl);
            gl.LookAt(m_eyeX, m_eyeY, m_eyeZ, m_centerX, m_centerY, m_centerZ, m_upX, m_upY, m_upZ);
            m_zDistance = -2000;


            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            m_scene.LoadScene();
            m_scene.Initialize();
        }

        public void SetupLighting(OpenGL gl)
        {
            // Podesi na koje parametre materijala se odnose pozivi glColor funkcije
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            float[] light0ambient =  { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse =  { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0specular = { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            float[] smer = {-1f, 0f, 0.0f};
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 35.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, refleksionaBoja);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, refleksionaBoja);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, refleksionaBoja);

            // Ukljuci svetlosni izvor 
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LIGHT1);

            gl.ShadeModel(OpenGL.GL_SMOOTH);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_AUTO_NORMAL);
            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        public void SetupTextures(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            //filter za teksture je NN
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            //wraping da je repeat po obema osama
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int) OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA,
                    OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.LoadIdentity();
            gl.LookAt(m_eyeX, m_eyeY + 200, m_eyeZ, m_centerX, m_centerY + 200, m_centerZ, m_upX, m_upY, m_upZ);

            gl.Translate(0, 0, m_zDistance);
            gl.Rotate(m_xRotation, 1.0f, 0, 0);
            gl.Rotate(m_yRotation, 0, 1.0f, 0);
            DrawFloor(gl);
            gl.PushMatrix();

            //iscrtavanje lampeA desno od stepenica
            gl.PushMatrix();
            float[] pos = new float[] {1000f, 0, 0, 1.0f};
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, pos);
            gl.Translate(pos[0], pos[1], pos[2]);
            lampA.Material.Bind(gl);
            lampA.Render(gl,RenderMode.Render);
            gl.PopMatrix();

            gl.Translate(100f,-40f+korakGore,130.0f-korakNapred);
            gl.Rotate(180.0, 0, 1.0, 0);
            gl.Scale(30+Fatindex, 30.0, 30);
            m_scene.Draw();
      
            gl.PopMatrix();

            gl.Rotate(90f, 0, 1.0f, 0);
            gl.PushMatrix();


            gl.Translate(100.0, 100.0, 0);
            // gl.Rotate(90f,-45f,0);
            gl.Color(0, 0, 1.0);
            Cylinder baseup = new Cylinder();
            baseup.Material = new SharpGL.SceneGraph.Assets.Material();
            baseup.BaseRadius = 20.0;
            baseup.TopRadius = 20.0;
            baseup.Height = 200;
            baseup.CreateInContext(gl);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            baseup.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();
            gl.PushMatrix();

            gl.Translate(-100.0, -100.0, 0);
            // gl.Rotate(90f, -45f, 0);
            gl.Color(0, 0, 1.0);
            Cylinder basedown = new Cylinder();
            basedown.Material = new SharpGL.SceneGraph.Assets.Material();
            basedown.BaseRadius = 20.0;
            basedown.TopRadius = 20.0;
            basedown.Height = 200;
            basedown.CreateInContext(gl);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            basedown.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            //STRANICA
            gl.PushMatrix();
            gl.Rotate(90f, 45f, 0);
            gl.Translate(0, 0, 0);
            gl.Scale(160, 1, 20);
            gl.Color(0, 0, 1.0);

            Cube cube = new Cube();
            cube.Material = new SharpGL.SceneGraph.Assets.Material();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            //DRZAC RUKE STUB (LEVO GORE)
            gl.PushMatrix();
            gl.Color(0, 0, 1.0);
            gl.Rotate(90f, 0, 0);
            gl.Translate(100, 10, -130);
            gl.Scale(10, 10, 40);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();


            //DRZAC RUKE STUB  (DESNO GORE)
            gl.PushMatrix();
            gl.Color(0, 0, 1.0);
            gl.Rotate(90f, 0, 0);
            gl.Translate(100, 190, -130);
            gl.Scale(10, 10, 40);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();


            //DRZAC RUKE STUB  (LEVO DOLE)
            gl.PushMatrix();
            gl.Color(0, 0, 1.0);
            gl.Rotate(90f, 0, 0);
            gl.Translate(-120, 10, 75);
            gl.Scale(10, 10, 50);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();


            //DRZAC RUKE STUB  (DESNO DOLE)
            gl.PushMatrix();
            gl.Color(0, 0, 1.0);
            gl.Rotate(90f, 0, 0);
            gl.Translate(-120, 190, 75);
            gl.Scale(10, 10, 50);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            //STRANICA DRZACA ZA RUKE LEVO
            gl.PushMatrix();
            Cylinder rukohvat = new Cylinder();
            rukohvat.Material = new SharpGL.SceneGraph.Assets.Material();
            rukohvat.BaseRadius = 5.0;
            rukohvat.TopRadius = 5.0;
            rukohvat.Height = 310;
            gl.Translate(-120, -50, 10);
            gl.Rotate(90f, 135f, 0);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            rukohvat.CreateInContext(gl);
            rukohvat.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();


            //STRANICA DRZACA ZA RUKE DESNO
            gl.PushMatrix();
            rukohvat.BaseRadius = 5.0;
            rukohvat.TopRadius = 5.0;
            rukohvat.Height = 310;
            gl.Translate(-120, -50, 190);
            gl.Rotate(90f, 135f, 0);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            rukohvat.CreateInContext(gl);
            rukohvat.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();


            //STRANICA
            gl.PushMatrix();
            gl.Rotate(90f, 45f, 0);
            gl.Translate(0, 200, 0);
            gl.Scale(160, 1, 20);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.PopMatrix();

            //POKLOPAC
            gl.PushMatrix();
            gl.Rotate(90f, 45f, 0);
            gl.Translate(0, 100, -20);
            gl.Scale(135, 100, 1);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();


            //iscrtavanje lampe refleksionog izvora
            gl.PushMatrix();
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, refleksionaBoja);
            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, refleksionaBoja);
            float[] pozicija = {100f, 700f, 100f, 1f};
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, pozicija);
            gl.Translate(pozicija[0], pozicija[1], pozicija[2]);
            //lampR.Material.Emission = refleksionaBojaLampe;
            lampR.Material.Bind(gl);
            lampR.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            drawStairs(gl);

            //PODLOGA STEPENICA
            gl.PushMatrix();
            gl.Color(0, 0, 1.0);
            gl.Rotate(90f, 45f, 0);
            gl.Translate(0, 100, 20);
            gl.Scale(135, 100, 1);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            //STUB
            gl.PushMatrix();
            gl.Color(0, 0, 1.0);
            gl.Rotate(90f, 0, 0);
            gl.Translate(100, 100, 20);
            gl.Scale(20, 100, 110);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
            cube.Render(gl, RenderMode.Render);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();
            gl.PopMatrix();

            DrawText(gl);


            gl.Flush();
        }

        private void drawStairs(OpenGL gl)
        {
            Cube cube = new Cube();
            cube.Material = new SharpGL.SceneGraph.Assets.Material();
            for (int i = 0; i < stepeniceCount; i++)
            {
                gl.PushMatrix();
                gl.Color(0, 1f, 0);
                gl.Rotate(90f, 45f, 0);
                gl.Rotate(0, 135f, 0);
                stepenicaX[i] = 100 - (i * 10) + korakX;
                stepenicaZ[i] = -90 + (i * 10) + korakZ;
                if (stepenicaX[i] <= -100)
                {
                    stepenicaX[i] = 100;
                    korakX = 0;
                }
                if (stepenicaZ[i] >= 110)
                {
                    stepenicaZ[i] = -90;
                    korakZ = 0;
                }
                gl.Translate(stepenicaX[i], 100, stepenicaZ[i]);
                gl.Scale(30, 80, 2);
                gl.Enable(OpenGL.GL_TEXTURE_2D);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Metal]);
                cube.Render(gl, RenderMode.Render);
                gl.Disable(OpenGL.GL_TEXTURE_2D);
                gl.PopMatrix();
            }
        }

        public void Animacija()
        {
            resetValues();
            AnimationEnd = false;
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(50);
            timer1.Tick += new EventHandler(Stepenice);
            timer1.Start();
        }

        public void resetValues()
        {
            korakX = 0f;
            korakZ = 0f;
            korakGore = 0f;
            korakNapred = 0f;
        }

        public void Stepenice(object sender, EventArgs e)
        {
            korakX -= 1.5f - fatindex / 30;
            korakZ += 1.5f - fatindex / 30;
            korakNapred += 1.5f - fatindex / 30;
            korakGore += 1.5f - fatindex / 30;
            if (korakGore >= 210f)
            {
                AnimationEnd = true;
                timer1.Stop();
            }
        }

       

        public void DrawText(OpenGL gl)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.Ortho2D(-10.0, 10.0, -10.0, 10.0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Translate(1.0f, -6.0f, 0.0f);

            string[] text = new string[]
            {
                "Predmet:Racunarska grafika", "Sk. god: 2017/18.", "Ime: Milorad", "Prezime: Trninic",
                "Sifra zad: 11.2"
            };

            gl.Scale(0.6f, 0.6f, 0.6f);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(0.0f, 0.0f, 1.0f);
            for (int i = 0; i < text.Length; i++)
            {
                gl.PushMatrix();
                gl.Translate(0.0f, i * (-1.0f), 0.0f);
                gl.DrawText3D("Arial Bold", 1.0f, 1.0f, 1.0f, text[i]);
                gl.PopMatrix();
            }
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.PopMatrix();
        }


        private void DrawFloor(OpenGL gl)
        {
            gl.PushMatrix();

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int) TextureObjects.Plocice]);

            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0, 1f, 0);
            gl.Color(1.0f, 1.0f, 1.0f);
            //dole levo
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-500.0f, -115.0f, 1000.0);
            //dole desno
            gl.TexCoord(0.0f, 20.0f);
            gl.Vertex(800.0f, -115.0f, 1000.0);
            //gore desno
            gl.TexCoord(20.0f, 20.0f);
            gl.Vertex(800.0f, -115.0f, -1000.0f);
            //gore levo
            gl.TexCoord(20.0f, 0.0f);
            gl.Vertex(-500.0f, -115.0f, -1000.0f);
            gl.End();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            //definisanje viewport-a
            gl.Viewport(0, 0, m_width, m_height);
            // selektuj Projection Matrix
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45f, (double) width / height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            // resetuj ModelView Matrix
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
