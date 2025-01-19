


namespace CyberedgeImageProcess2024
{
    internal class FloatBlitter
    {
        public static float divideByZeroValue = float.PositiveInfinity;

        private FloatProcessor ip;
        private int width, height;
        private float[] pixels;

        /** Constructs a FloatBlitter from a FloatProcessor. */
        public FloatBlitter(FloatProcessor ip)
        {
            this.ip = ip;
            width = ip.Width;
            height = ip.Height;
            pixels = (float[])ip.GetPixels();
        }


        /** Copies the float image in 'ip' to (x,y) using the specified mode. */
        public void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode)
        {
            int srcIndex, dstIndex;
            int xSrcBase, ySrcBase;
            float[] srcPixels;

            if (!(ip is FloatProcessor))
                ip = ip.ConvertToFloat();
            int srcWidth = ip.Width;
            int srcHeight = ip.Height;
            Rectangle r1 = new Rectangle(0, 0, srcWidth, srcHeight);
            r1.Location = new Point(xloc, yloc);
            Rectangle r2 = new Rectangle(0, 0, width, height);
            if (Rectangle.Intersect(r1, r2) == Rectangle.Empty)
                return;

            srcPixels = (float[])ip.GetPixels();
            r1 = Rectangle.Intersect(r1, r2);
            xSrcBase = (xloc < 0) ? -xloc : 0;
            ySrcBase = (yloc < 0) ? -yloc : 0;
            bool useDBZValue = !float.IsInfinity(divideByZeroValue);
            float src, dst;
            for (int y = r1.Y; y < (r1.Y + r1.Height); y++)
            {
                srcIndex = (y - yloc) * srcWidth + (r1.X - xloc);
                dstIndex = y * width + r1.X;
                switch (mode)
                {
                    case Blitter.COPY:
                    case Blitter.COPY_INVERTED:
                    case Blitter.COPY_TRANSPARENT:
                        for (int i = r1.Width; --i >= 0;)
                            pixels[dstIndex++] = srcPixels[srcIndex++];
                        break;
                    case Blitter.COPY_ZERO_TRANSPARENT:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++];
                            if (src == 0f)
                                dst = pixels[dstIndex];
                            else
                                dst = src;
                            pixels[dstIndex++] = dst;
                        }
                        break;
                    case Blitter.ADD:
                        for (int i = r1.Width; --i >= 0; srcIndex++, dstIndex++)
                            pixels[dstIndex] = srcPixels[srcIndex] + pixels[dstIndex];
                        break;
                    case Blitter.AVERAGE:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (srcPixels[srcIndex++] + pixels[dstIndex]) / 2;
                            pixels[dstIndex++] = dst;
                        }
                        break;
                    case Blitter.DIFFERENCE:
                        for (int i = r1.Width; --i >= 0; srcIndex++, dstIndex++)
                        {
                            dst = pixels[dstIndex] - srcPixels[srcIndex];
                            pixels[dstIndex] = dst < 0 ? -dst : dst;
                        }
                        break;
                    case Blitter.SUBTRACT:
                        for (int i = r1.Width; --i >= 0; srcIndex++, dstIndex++)
                            pixels[dstIndex] = pixels[dstIndex] - srcPixels[srcIndex];
                        break;
                    case Blitter.MULTIPLY:
                        for (int i = r1.Width; --i >= 0; srcIndex++, dstIndex++)
                            pixels[dstIndex] = srcPixels[srcIndex] * pixels[dstIndex];
                        break;
                    case Blitter.DIVIDE:
                        for (int i = r1.Width; --i >= 0; srcIndex++, dstIndex++)
                        {
                            src = srcPixels[srcIndex];
                            if (useDBZValue && src == 0.0)
                                pixels[dstIndex] = divideByZeroValue;
                            else
                                pixels[dstIndex] = pixels[dstIndex] / src;
                        }
                        break;
                    case Blitter.AND:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (int)srcPixels[srcIndex++] & (int)pixels[dstIndex];
                            pixels[dstIndex++] = dst;
                        }
                        break;
                    case Blitter.OR:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (int)srcPixels[srcIndex++] | (int)pixels[dstIndex];
                            pixels[dstIndex++] = dst;
                        }
                        break;
                    case Blitter.XOR:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (int)srcPixels[srcIndex++] ^ (int)pixels[dstIndex];
                            pixels[dstIndex++] = dst;
                        }
                        break;
                    case Blitter.MIN:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++];
                            dst = pixels[dstIndex];
                            if (src < dst) dst = src;
                            pixels[dstIndex++] = dst;
                        }
                        break;
                    case Blitter.MAX:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++];
                            dst = pixels[dstIndex];
                            if (src > dst) dst = src;
                            pixels[dstIndex++] = dst;
                        }
                        break;
                }
            }
        }
    }

}
