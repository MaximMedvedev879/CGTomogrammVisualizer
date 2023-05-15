using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
//сейчас порезана сверху плоскость, надо порезать спереди плоскость
namespace LabCG2
{
    public partial class Form1 : Form
    {
        Bin bin = new Bin();
        bool loaded = false; // чтобы не запускать отрисовку, пока не загружены данные
        View view = new View();

        int currentLayer = 0; // номер слоя визуализации
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        bool needReload = false;

        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("Просмотр томограмм (FPS={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        public Form1()
        {
            InitializeComponent();
        }


        private void Открыть_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                //trackBar1.Maximum = Bin.Z-1;
                trackBar1.Maximum = Bin.X-2;
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (radioButton1.Checked)
                {
                    view.DrawQuads(currentLayer);
                }
                else
                {
                    
                        if (needReload)
                        {

                            view.generateTextureImage(currentLayer);
                            view.Load2DTexture();
                            needReload = false;
                        }
                        view.DrawTexture();
                    }
                    glControl1.SwapBuffers();
                }
            }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }

        
    }


}
