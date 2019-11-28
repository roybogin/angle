using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace angle
{
    public partial class Form1 : Form
    {
        int y = 350;    //top of the triangle
        double angle = 0;
        double rad = 0;
        int width = 149;    //size of the box
        int height = 79;
        public Form1()
        {
            InitializeComponent();
        }

        public static Bitmap RotateImage(Bitmap bmp, float angle, Color bkColor)    //rotate an image to fit the angle
        {
            angle = angle % 360;
            if (angle > 180)
                angle -= 360;

            System.Drawing.Imaging.PixelFormat pf = default(System.Drawing.Imaging.PixelFormat);
            if (bkColor == Color.Transparent)
            {
                pf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            float sin = (float)Math.Abs(Math.Sin(angle * Math.PI / 180.0)); // this function takes radians
            float cos = (float)Math.Abs(Math.Cos(angle * Math.PI / 180.0)); // this one too
            float newImgWidth = sin * bmp.Height + cos * bmp.Width;
            float newImgHeight = sin * bmp.Width + cos * bmp.Height;
            float originX = 0f;
            float originY = 0f;

            if (angle > 0)
            {
                if (angle <= 90)
                    originX = sin * bmp.Height;
                else
                {
                    originX = newImgWidth;
                    originY = newImgHeight - sin * bmp.Width;
                }
            }
            else
            {
                if (angle >= -90)
                    originY = sin * bmp.Width;
                else
                {
                    originX = newImgWidth - sin * bmp.Height;
                    originY = newImgHeight;
                }
            }

            Bitmap newImg = new Bitmap((int)newImgWidth, (int)newImgHeight, pf);
            Graphics g = Graphics.FromImage(newImg);
            g.Clear(bkColor);
            g.TranslateTransform(originX, originY); // offset the origin to our calculated values
            g.RotateTransform(angle); // set up rotate
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(bmp, 0, 0); // draw the image at 0, 0
            g.Dispose();

            return newImg;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        { 
            Graphics gr = e.Graphics;
            Pen p = new Pen(SystemColors.Highlight, 10);
            SolidBrush b = new SolidBrush(SystemColors.Highlight);
            Point[] point = { new Point(200, 350), new Point(500, 350), new Point(500, y) };
            gr.DrawPolygon(p, point);
            gr.FillPolygon(b, point);
            b.Dispose();
            p.Dispose();
            gr.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double mass;    //mass of object
            double coef;    //coefficient of friction
            try
            {
                mass = Convert.ToDouble(textBox2.Text);
                coef = Convert.ToDouble(textBox3.Text);
                angle = Convert.ToDouble(textBox1.Text);
            }
            catch
            {
                MessageBox.Show("The conversion of the inputs to numbers didn't work properly", "Problem Detected");
                return;
            }

            double acc;
            if (mass <= 0 || coef < 0 || angle < 0 || angle >= 90)
            {
                MessageBox.Show("At least one of the inputs doesn't make sense", "Problem Detected");
                return;
            }
            pictureBox1.Load("../../images/system.png");    //load the x and y arrows
            pictureBox2.Load("../../images/object.png");    //load the object
            pictureBox1.Image = RotateImage(new Bitmap(pictureBox1.Image), 360 - (float)angle, Color.Transparent);  //Rotate the images and make the background transparent
            pictureBox2.Image = RotateImage(new Bitmap(pictureBox2.Image), 360 - (float)angle, Color.Transparent);
            rad = angle * Math.PI / 180.0;
            y = (int)(350 - Math.Tan(rad) * 300);
            //calculate new x and y for the box (top left transparent pixel)
            int box_x = (int)(350 - (width * Math.Cos(rad) / 2.0) - (height * Math.Sin(rad)));
            int box_y = (int)(((350 + y) / 2.0) - (width * Math.Sin(rad) / 2.0) - (height * Math.Cos(rad)));
            pictureBox2.Location = new Point(box_x, box_y); 
            this.Invalidate();
            double totalforce = mass * 10 * Math.Sin(rad) - mass * 10 * Math.Cos(rad) * coef;   //calculate force on box
            totalforce = ((totalforce + Math.Abs(totalforce)) / 2.0);   //if the force is negative, turn it to 0
            acc = totalforce / mass;    //Newton's second law
            label5.Text = totalforce + " N";
            label6.Text = acc + "  m/s\xB2";
        }
    }
}
