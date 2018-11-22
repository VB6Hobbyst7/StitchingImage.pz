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
        string _01_2_1DataSet = @"01_2_1.txt";
        string _02_2_1DataSet = @"02_2_1.txt";
        string _01_3_2DataSet = @"01_3_2.txt";
        string _02_3_2DataSet = @"02_3_2.txt";
        string _01_4_3DataSet = @"01_4_3.txt";
        string _02_4_3DataSet = @"02_4_3.txt";

        static string firstImageFile = @"01.bmp";
        static string secondImageFile = @"02.bmp";
        static string thirdImageFile = @"03.bmp";
        static string fourthImageFile = @"04.bmp";
        double[,] FirstDataArray = new double[0,0];
        double[,] LastDataArray = new double[0, 0];
        Image<Gray, byte> firstImage = new Image<Gray, byte>(new Bitmap(firstImageFile));
        Image<Gray, byte> secondImage = new Image<Gray, byte>(new Bitmap(secondImageFile));
        Image<Gray, byte> thirdImage = new Image<Gray, byte>(new Bitmap(thirdImageFile));
        Image<Gray, byte> fourthImage = new Image<Gray, byte>(new Bitmap(fourthImageFile));
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
            FirstDataArray = getDataFromTXT(_01_2_1DataSet);
            LastDataArray = getDataFromTXT(_02_2_1DataSet);
            double XOffset2_1;
            double YOffset2_1;
            StitchingImageTool.XYOffsetCalc(FirstDataArray, LastDataArray, out XOffset2_1, out YOffset2_1);

            FirstDataArray = getDataFromTXT(_01_3_2DataSet);
            LastDataArray = getDataFromTXT(_02_3_2DataSet);
            double XOffset3_2;
            double YOffset3_2;
            StitchingImageTool.XYOffsetCalc(FirstDataArray, LastDataArray, out XOffset3_2, out YOffset3_2);


            FirstDataArray = getDataFromTXT(_01_4_3DataSet);
            LastDataArray = getDataFromTXT(_02_4_3DataSet);
            double XOffset4_3;
            double YOffset4_3;
            StitchingImageTool.XYOffsetCalc(FirstDataArray, LastDataArray, out XOffset4_3, out YOffset4_3);
            //var res = StitchingImageTool.StitchingImage(firstImage, lastImage, XOffset, 0);
            //res.Save(@"C:\test.bmp");
            List<Image<Gray, byte>> imagesQueue = new List<Image<Gray, byte>>();
            imagesQueue.Add(firstImage);
            imagesQueue.Add(secondImage);
            imagesQueue.Add(thirdImage);
            imagesQueue.Add(fourthImage);
            ///
            // Y 向标定精度太差 用0 代替

            Point[] points = new Point[] { new Point(0, 0), new Point((int)XOffset2_1, (int)0) , new Point((int)(XOffset2_1+XOffset3_2), (int)YOffset3_2)
                ,new Point((int)(XOffset2_1+XOffset3_2+XOffset4_3), (int)0)
            };
            var res = StitchingImageTool.StitchingImageQueue(imagesQueue, points);


            imageBox1.Image = res;
            richTextBox1.Text = $"{YOffset2_1}+{YOffset3_2}+{YOffset4_3}";



        }
    }
}
