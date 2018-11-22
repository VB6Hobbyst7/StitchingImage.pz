using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StitchingImageLib;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;

namespace StitchingImage
{
    public partial class TestUI : Form
    {
        public TestUI()
        {
            InitializeComponent();
        }
        string firstDataSet = @"First.txt";
        string lastDataSet = @"Last.txt";
         static string firstImageFile = @"first.bmp";
        static string lastImageFile = @"last.bmp";
        double[,] FirstDataArray = new double[0,0];
        double[,] LastDataArray = new double[0, 0];
        Image<Gray, byte> firstImage = new Image<Gray, byte>(new Bitmap(firstImageFile));
        Image<Gray, byte> lastImage = new Image<Gray, byte>(new Bitmap(lastImageFile));


        double[,] getDataFromTXT(string filename)
        {
            string[] dataStr = File.ReadAllLines(filename);
            double[,] DataArray = new double[dataStr.Length, 2];
            for (int i = 0; i < dataStr.Length; i++)
            {
                string[] subdata = dataStr[i].Split(',');
                for (int k = 0; k < subdata.Length; k++)
                {
                    DataArray[i, k] = Convert.ToDouble(subdata[k]);
                }
            }
            return DataArray;

        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            FirstDataArray = getDataFromTXT(firstDataSet);
            LastDataArray = getDataFromTXT(lastDataSet);
            double XOffset;
            double YOffset;
            StitchingImageTool.XYOffsetCalc(FirstDataArray, LastDataArray, out XOffset, out YOffset);
            //var res = StitchingImageTool.StitchingImage(firstImage, lastImage, XOffset, 0);
            //res.Save(@"C:\test.bmp");
            List<Image<Gray, byte>> imagesQueue = new List<Image<Gray, byte>>();
            imagesQueue.Add(firstImage);
            imagesQueue.Add(lastImage);
            Point[] points = new Point[] { new Point(0, 0), new Point((int)XOffset, (int)YOffset) };
            var res = StitchingImageTool.StitchingImageQueue(imagesQueue, points);


            imageBox1.Image = res;
            richTextBox1.Text = $"{res.Width}+{res.Height}";



        }
    }
}
