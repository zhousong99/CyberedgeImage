using Emgu.CV;
using Emgu.CV.Util;

namespace CyberedgeImageProcess2024
{
    public partial class EdgeImagePlus
    {
        int maxChannels = 7;
        Color[] colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Gray, Color.Cyan, Color.Magenta, Color.Yellow };
        public void SplitRGB(EdgeImagePlus imp)
        {
            bool keepSource = PublicFunctions.IsKeyPressed(Keys.Alt);
            String title = imp.Title;
            int pos = imp.GetCurrentSlice();
            Emgu.CV.Mat[] channels = imp.img.Split();
            if (!keepSource)
            { imp.changes = false; imp.Close(); }
            EdgeImagePlus rImp = new EdgeImagePlus(title + " (red)", channels[2]);
            rImp.Show();
            EdgeImagePlus gImp = new EdgeImagePlus(title + " (green)", channels[1]);
            gImp.Show();
            EdgeImagePlus bImp = new EdgeImagePlus(title + " (blue)", channels[0]);
            bImp.Show();
        }

        public void ColorMerge()
        {


            ColorMerge gd = new ColorMerge();

            if (gd.ShowDialog() == DialogResult.Cancel) return;
            ImageWindow[] wins = gd.GetWindows();


            int[] activeChannel = new int[3];   //用于存放每个通道有几个图像有数据



            EdgeImagePlus[] images = new EdgeImagePlus[maxChannels];
            int stackSize = 0;
            int width = 0;
            int height = 0;
            int bitDepth = 0;
            for (int i = 0; i < maxChannels; i++)
            {
                //IJ.log(i+"  "+index[i]+"	"+titles[index[i]]+"  "+wList.length);
                if (wins[i] == null) continue;

                images[i] = wins[i].Imp;
                if (width == 0)
                {
                    width = images[i].Width;
                    height = images[i].Height;
                    stackSize = images[i].getStackSize();
                    bitDepth = images[i].GetBitDepth();
                    //slices = images[i].getNSlices();
                    //frames = images[i].getNFrames();
                }

            }
            if (width == 0)
            {
                MessageBox.Show("Requires at least one source image or stack.");
                return;
            }

            VectorOfMat sourceImages = new VectorOfMat();
            Mat r = new Mat(new Size(width, height), Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            Mat g = new Mat(new Size(width, height), Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            Mat b = new Mat(new Size(width, height), Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            for (int i = 0; i < maxChannels; i++)
            {
                EdgeImagePlus img = images[i];
                if (img == null) continue;
                
                if (img.Width != width || images[i].Height != height)
                {
                    MessageBox.Show("The source images or stacks must have the same width and height.");
                    return;
                }


                Mat matR = new Mat();
                Mat matG = new Mat();
                Mat matB = new Mat();
                GetColorMatFromChannels(img.Img, colors[i], ref matR, ref matG, ref matB, ref activeChannel);
                CvInvoke.Add(r, matR, r);
                CvInvoke.Add(g, matG, g);
                CvInvoke.Add(b, matB, b);

            }

            if (activeChannel[2] != 0) r = r / activeChannel[2];
            if (activeChannel[1] != 0) g = g / activeChannel[1];
            if (activeChannel[0] != 0) b = b / activeChannel[0];
            sourceImages.Push(b);
            sourceImages.Push(g);
            sourceImages.Push(r);

            Mat mat = new Mat();
            CvInvoke.Merge(sourceImages, mat);

            EdgeImagePlus edgeImagePlus = new EdgeImagePlus("RGB", mat);
            edgeImagePlus.Show();
        }

        private void GetColorMatFromChannels(Mat image, Color color, ref Mat r, ref Mat g, ref Mat b, ref int[] activeChannel)
        {
            int[] RGBChannel = new int[3];
            if (color.B != 0) RGBChannel[0] = 1;
            if (color.G != 0) RGBChannel[1] = 1;
            if (color.R != 0) RGBChannel[2] = 1;


            Mat blankMat = new Mat(image.Size, Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            Mat[] mats = image.Split();

            r = blankMat;
            g = blankMat;
            b = blankMat;
            if (color.B != 0)
            {
                b = mats[0];
                activeChannel[0]++;
            }
            if (color.G != 0)
            {
                g = mats[1];
                activeChannel[1]++;
            }
            if (color.R != 0)
            {
                r = mats[2];
                activeChannel[2]++;
            }

        }
    }
}
