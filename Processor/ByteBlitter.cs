

namespace CyberedgeImageProcess2024
{
    internal class ByteBlitter
    {
        private ByteProcessor ip;
        private int width, height;
        private byte[] pixels;
        private int transparent = 255;

        /** Constructs a ByteBlitter from a ByteProcessor. */
        public ByteBlitter(ByteProcessor ip)
        {
            this.ip = ip;
            width = ip.Width;
            height = ip.Height;
            pixels = (byte[])ip.GetPixels();
        }



        /** Copies the byte image in 'ip' to (x,y) using the specified mode. */
        public void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode)
        {
            Rectangle r1, r2;
            int srcIndex, dstIndex;
            int xSrcBase, ySrcBase;
            byte[] srcPixels;

            int srcWidth = ip.Width;
            int srcHeight = ip.Height;
            r1 = new Rectangle(0, 0, srcWidth, srcHeight);
            r1.Location = new Point(xloc, yloc);
            r2 = new Rectangle(0, 0, width, height);
            if ((Rectangle.Intersect(r1, r2)) == Rectangle.Empty)
                return;
            if (ip is ColorProcessor)
            {
                Color[] pixels32 = (Color[])ip.GetPixels();
                int size = ip.Width * ip.Height;
                srcPixels = new byte[size];

                for (int i = 0; i < size; i++)
                    srcPixels[i] = (byte)(pixels32[i].G);
            }
            else
            {
                ip = ip.ConvertToByte(true);
                srcPixels = (byte[])ip.GetPixels();
            }
            r1 = Rectangle.Intersect(r1, r2);
            xSrcBase = (xloc < 0) ? -xloc : 0;
            ySrcBase = (yloc < 0) ? -yloc : 0;
            int src, dst;
            for (int y = r1.Y; y < (r1.Y + r1.Height); y++)
            {
                srcIndex = (y - yloc) * srcWidth + (r1.X - xloc);
                dstIndex = y * width + r1.X;
                switch (mode)
                {
                    case Blitter.COPY:
                        for (int i = r1.Width; --i >= 0;)
                            pixels[dstIndex++] = srcPixels[srcIndex++];
                        break;
                    case Blitter.COPY_INVERTED:
                        for (int i = r1.Width; --i >= 0;)
                            pixels[dstIndex++] = (byte)(255 - srcPixels[srcIndex++] & 255);
                        break;
                    case Blitter.COPY_TRANSPARENT:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++] & 255;
                            if (src == transparent)
                                dst = pixels[dstIndex];
                            else
                                dst = src;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.COPY_ZERO_TRANSPARENT:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++] & 255;
                            if (src == 0)
                                dst = pixels[dstIndex];
                            else
                                dst = src;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.ADD:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (srcPixels[srcIndex++] & 255) + (pixels[dstIndex] & 255);
                            if (dst > 255) dst = 255;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.AVERAGE:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = ((srcPixels[srcIndex++] & 255) + (pixels[dstIndex] & 255)) / 2;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.SUBTRACT:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (pixels[dstIndex] & 255) - (srcPixels[srcIndex++] & 255);
                            if (dst < 0) dst = 0;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.DIFFERENCE:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (pixels[dstIndex] & 255) - (srcPixels[srcIndex++] & 255);
                            if (dst < 0) dst = -dst;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.MULTIPLY:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (srcPixels[srcIndex++] & 255) * (pixels[dstIndex] & 255);
                            if (dst > 255) dst = 255;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.DIVIDE:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++] & 255;
                            if (src == 0)
                                dst = 255;
                            else
                                dst = (pixels[dstIndex] & 255) / src;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.AND:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = srcPixels[srcIndex++] & pixels[dstIndex];
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.OR:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = srcPixels[srcIndex++] | pixels[dstIndex];
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.XOR:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = srcPixels[srcIndex++] ^ pixels[dstIndex];
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.MIN:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++] & 255;
                            dst = pixels[dstIndex] & 255;
                            if (src < dst) dst = src;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                    case Blitter.MAX:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++] & 255;
                            dst = pixels[dstIndex] & 255;
                            if (src > dst) dst = src;
                            pixels[dstIndex++] = (byte)dst;
                        }
                        break;
                }
            }
        }
    }
}
