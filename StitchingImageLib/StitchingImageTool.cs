using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Stitching;
using System.Drawing;

namespace StitchingImageLib
{
    public class StitchingImageTool
    {
        /// <summary>
        /// 两幅图像拼接，第一幅是主图像，计算第二幅图像偏移量
        /// </summary>
        /// <param name="inFirstKeyPoints">第一幅图像取点</param>
        /// <param name="inLastKeyPoints">第二幅图像取点</param>
        /// <param name="xOffset">X偏移</param>
        /// <param name="yOffset">Y偏移</param>
        public static void XYOffsetCalc(double[,] inFirstKeyPoints, double[,] inLastKeyPoints, out double xOffset, out double yOffset )
        {
            xOffset = 0;
            yOffset = 0;
            double[,] LastKeyPointsHome;
            homogeneousMatrix(inLastKeyPoints, out LastKeyPointsHome);
            Matrix<double> firstMatrix = new Matrix<double>(inFirstKeyPoints);
            Matrix<double> lastMatrix = new Matrix<double>(LastKeyPointsHome);

            Matrix<double> TMatrix = new Matrix<double>(new double[3,2]);
            CvInvoke.Solve(lastMatrix, firstMatrix, TMatrix, Emgu.CV.CvEnum.DecompMethod.Svd);

            xOffset = TMatrix[2, 0];
            yOffset = TMatrix[2, 1];
        }

        /// <summary>
        /// 将二维点位数据转换成齐次Matrix
        /// </summary>
        /// <param name="inMatrix"></param>
        /// <param name="homogeneousMatrix"></param>
         static void homogeneousMatrix(double[,] inMatrix, out double[,] homogeneousMatrix)
        {
            homogeneousMatrix = new double[0, 0];
            int length = inMatrix.GetLength(0);
            int rank = inMatrix.Rank;
            homogeneousMatrix = new double[length, rank + 1];
            for (int i = 0; i < homogeneousMatrix.GetLength(0); i++)
            {
                for (int k = 0; k < rank; k++)
                {
                    homogeneousMatrix[i, k] = inMatrix[i, k];
                }
                homogeneousMatrix[i, rank] = 1;
            }
        }

        /// <summary>
        /// 两张图像拼接
        /// </summary>
        /// <param name="inFirst"></param>
        /// <param name="inLast"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        public static Image<Gray,byte> StitchingImage(Image<Gray, byte> inFirst, Image<Gray, byte> inLast, double xOffset, double yOffset)
        {
            int width = inFirst.Width;
            int height = inFirst.Height;
            Matrix<double> mapMat = new Matrix<double>(new double[,] { { 1, 0, 0 }, { 0, 1, yOffset} });
            var outLast = inLast.WarpAffine(mapMat.Mat, Emgu.CV.CvEnum.Inter.Nearest, Emgu.CV.CvEnum.Warp.Default, Emgu.CV.CvEnum.BorderType.Constant, new Gray(0));
            Image<Gray, byte> dstImage = new Image<Gray, byte>(width * 2, height);
            dstImage.ROI = new System.Drawing.Rectangle(0, 0, width, height);

            inFirst.CopyTo(dstImage);
            dstImage.ROI = new System.Drawing.Rectangle((int)xOffset, 0, width,height);
            outLast.CopyTo(dstImage);
            dstImage.ROI = Rectangle.Empty;

            var res = dstImage.GetSubRect(new Rectangle(0, 0, inFirst.Width+(int)xOffset, dstImage.Height));
            return res;
        }

        /// <summary>
        /// 多帧图像拼接
        /// </summary>
        /// <param name="imagesQueue">按照顺序排列的图像组</param>
        /// <param name="Offset">拼接偏移量 (X21 Y21)  (X31 Y31) (X41 Y41.)..</param>
        /// <returns></returns>
        public static Image<Gray, byte> StitchingImageQueue(List<Image<Gray, byte>> imagesQueue, Point[] Offset)
        {
            //参数数量确认
            if ( imagesQueue.Count>=2&&Offset.Length == imagesQueue.Count )
            {
                int SingleWidth = imagesQueue[0].Width;
                int SingleHeight = imagesQueue[0].Height;
                int count = imagesQueue.Count;
                
                //首先对Y向偏移
                List<Image<Gray, byte>> YOffsetImages = new List<Image<Gray, byte>>();
                for (int i = 0; i < imagesQueue.Count; i++)
                {
                    Matrix<double> mapMat = new Matrix<double>(new double[,] { { 1, 0, 0 }, { 0, 1, Offset[i].Y } });
                    var YTrans = imagesQueue[i].WarpAffine(mapMat.Mat, Emgu.CV.CvEnum.Inter.Nearest, Emgu.CV.CvEnum.Warp.Default, Emgu.CV.CvEnum.BorderType.Constant, new Gray(0));
                    YOffsetImages.Add(YTrans);
                }

                //X向偏移 填充

                Image<Gray, byte> dstImage = new Image<Gray, byte>(SingleWidth*count, SingleHeight);
                for (int i = 0; i < YOffsetImages.Count; i++)
                {
                    dstImage.ROI = new Rectangle(Offset[i].X, 0, SingleWidth, SingleHeight);
                    YOffsetImages[i].CopyTo(dstImage);
                }
                dstImage.ROI = Rectangle.Empty;

                var subImage = dstImage.GetSubRect(new Rectangle(0,0,Offset[Offset.Length - 1].X + SingleWidth, SingleHeight));
                return subImage;
            }

            return null;
        }
    }
}
