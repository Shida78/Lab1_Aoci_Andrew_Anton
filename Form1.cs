using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV.Superres;

namespace Lab1_Aoci_Andrew_Anton
{
    public partial class Form1 : Form
    {
		public Form1()
		{
			InitializeComponent();
		}

		private Image<Bgr, byte> sourceImage;
        private VideoCapture capture;

        public string fileSelected;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                sourceImage = new Image<Bgr, byte>(fileName);
            }

            imageBox1.Image = sourceImage.Resize(640, 480, Inter.Linear);

            imageUpdateThres();

            fileSelected = "image";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                capture = new VideoCapture(fileName);
            }

            fileSelected = "video";

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            videoUpdate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (fileSelected == "image")
                imageUpdateThres();
            if (fileSelected == "video")
                videoUpdate();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (fileSelected == "image")
                imageUpdateThres();
            if (fileSelected == "video")
                videoUpdate();
        }

        private void imageUpdateThres()
        {
            Image<Gray, byte> grayImage = sourceImage.Convert<Gray, byte>();

            var tempImage = grayImage.PyrDown();
            var destImage = tempImage.PyrUp();

            //double cannyThreshold = 80.0;
            //double cannyThresholdLinking = 40.0;
            double cannyThreshold = trackBar1.Value;
            double cannyThresholdLinking = trackBar2.Value;
            Image<Gray, byte> cannyEdges = destImage.Canny(cannyThreshold, cannyThresholdLinking);

            var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
            var resultImage = sourceImage.Sub(cannyEdgesBgr);

            for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                for (int x = 0; x < resultImage.Width; x++)
                    for (int y = 0; y < resultImage.Height; y++)
                    {
                        byte color = resultImage.Data[y, x, channel];
                        if (color <= 50)
                            color = 0;
                        else if (color <= 100)
                            color = 25;
                        else if (color <= 150)
                            color = 180;
                        else if (color <= 200)
                            color = 210;
                        else
                            color = 255;

                        resultImage.Data[y, x, channel] = color;
                    }
            imageBox2.Image = resultImage;
        }

        private void videoUpdate()
        {
            var frame = capture.QueryFrame();

            imageBox1.Image = frame;

            Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();
            image = image.Resize(640, 480, Inter.Linear);               //мастхэв

            Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

            var tempImage = grayImage.PyrDown();
            var destImage = tempImage.PyrDown();

            //double cannyThreshold = 80.0;
            //double cannyThresholdLinking = 40.0;
            double cannyThreshold = trackBar1.Value;
            double cannyThresholdLinking = trackBar2.Value;
            Image<Gray, byte> cannyEdges = destImage.Canny(cannyThreshold, cannyThresholdLinking);

            var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
            cannyEdgesBgr = cannyEdgesBgr.Resize(640, 480, Inter.Linear); //главное решение
            var resultImage = image.Sub(cannyEdgesBgr);

            for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                for (int x = 0; x < resultImage.Width; x++)
                    for (int y = 0; y < resultImage.Height; y++)
                    {
                        byte color = resultImage.Data[y, x, channel];
                        if (color <= 50)
                            color = 0;
                        else if (color <= 100)
                            color = 25;
                        else if (color <= 150)
                            color = 180;
                        else if (color <= 200)
                            color = 210;
                        else
                            color = 255;

                        resultImage.Data[y, x, channel] = color;
                    }

            imageBox2.Image = resultImage;
        }
    }
}
