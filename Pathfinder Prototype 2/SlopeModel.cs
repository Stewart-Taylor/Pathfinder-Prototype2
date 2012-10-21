using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;

namespace Pathfinder_Prototype_2
{
    class SlopeModel
    {
        int width;
        int height;

        float[,] model;
        float[,] slopeModel;


        Bitmap slopeBitmap;
        Bitmap image;
        float lowestGradient;
        float highestGradient;

        string algorithmType;

        public SlopeModel(float[,] modelT , string type)
        {
            model = modelT;

            algorithmType = type;
            algorithmType = algorithmType.ToUpper();


            width = model.GetLength(0);
            height = model.GetLength(1);

            generateSlopeModel();
        }





        private void generateSlopeModel()
        {
            slopeModel = new float[model.GetLength(0), model.GetLength(1)];

            SlopeAlgrotithm slopeAlgortihm ;

            if (algorithmType == "MAXIMUM")
            {
                slopeAlgortihm = new SlopeMax(model);
            }
            else if (algorithmType == "AVERAGE")
            {
                slopeAlgortihm = new SlopeAverage(model);
            }
            else if (algorithmType == "HORN")
            {
                slopeAlgortihm = new SlopeAverage(model);
            }
            else
            {
                slopeAlgortihm = new SlopeAverage(model);
            }

            slopeModel = slopeAlgortihm.getSlopeModel();

            generateSlopeModelImage();
        }


        private void generateSlopeModelImage()
        {
            Bitmap bitmap = new Bitmap(model.GetLength(0), model.GetLength(1));
            getGradientLimits();


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color tempColor = getSlopeColorValue(slopeModel[x, y]);
                    bitmap.SetPixel(x, y, tempColor);
                }
            }

            slopeBitmap = bitmap;
        }



        private void getGradientLimits()
        {
            float low = slopeModel[0,0];
            float high = slopeModel[0, 0];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (low > slopeModel[x, y])
                    {
                        low = slopeModel[x, y];
                    }

                    if (high < slopeModel[x, y])
                    {
                        high = slopeModel[x, y];
                    }
                }
            }

            lowestGradient = low;
            highestGradient = high;
        }



        private System.Drawing.Color getSlopeColorValue(float gradient)
        {
            System.Drawing.Color color = System.Drawing.Color.White;

            gradient = Math.Abs(gradient);

            float green = 255;
            float red = 255;
            float blue = 0;



            if (gradient <= 1f)
            {

                red = (1f - gradient) * 255;
               
            }
            else if (gradient <= 3f)
            {
                float percent = (gradient) / (3f);

                green = (1f - percent) * 255;
            }
            else
            {
                green = 0;
            }

           
 

            color = System.Drawing.Color.FromArgb(255, (int)red, (int)green, (int)blue);

            return color;
        }



      


        public float[,] getSlopeModel()
        {
            return slopeModel;
        }


        public ImageSource getSlopeModelImage()
        {
            MemoryStream ms = new MemoryStream();
            slopeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            ImageSource img = bi;

            return img;
        }


    }
}

