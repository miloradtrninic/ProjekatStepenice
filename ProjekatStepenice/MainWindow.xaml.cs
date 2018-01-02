using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL.SceneGraph;

namespace ProjekatStepenice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        

         #region Atributi
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        ///  Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        private World m_world;
        

        private bool isEnabledAnimation = true;

        public bool IsEnabledAnimation
        {
            set
            {
                isEnabledAnimation = value;
                OnPropertyChanged("IsEnabledAnimation");
            }
            get { return isEnabledAnimation; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            // Kreiranje OpenGL sveta
            try
            {
              //  m_world = new World(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3DModel\\knight"), "knightObject.obj", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                m_world = new World(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3DModel\\Body"), "BodyMesh.obj", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
               // IsEnabledAnimation = !m_world.Animation;
                float[] tackaBoja = m_world.TackastaBoja;
                slColorR.Value = tackaBoja[0] * 255f;
                slColorG.Value = tackaBoja[1] * 255f;
                slColorB.Value = tackaBoja[2] * 255f;
                DataContext = m_world;
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                Close();
            }
          
        }

        public World MWorld { 
            get;
            set;
        }


        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //Iscrtaj svet
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!slColorB.IsEnabled)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.E: m_world.MXRotation += 2.0f; break;
                case Key.D: m_world.MXRotation -= 2.0f; break;
                case Key.S: m_world.MYRotation += 2.0f; break;
                case Key.F: m_world.MYRotation -= 2.0f; break;

                case Key.Add: m_world.MZDistance += 10.0f; break;
                case Key.Subtract: m_world.MZDistance -= 10.0f; break;

                case Key.I: m_world.MEyeZ += 10f; break;
                case Key.K: m_world.MEyeZ -= 10f; break;

                case Key.J: m_world.MEyeX += 0.5f; break;
                case Key.L: m_world.MEyeX -= 10.5f; break;
                case Key.V: m_world.Animacija(); break;
            }
        }

        private void SirinaSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_world.Fatindex = (float)SirinaSlider.Value;
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_world.TackastaBoja = new float[] { (float)slColorR.Value / 255f, (float)slColorG.Value / 255f, (float)slColorB.Value / 255f, 1f };
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
    

