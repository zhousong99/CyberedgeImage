

namespace CyberedgeImageProcess2024
{
    internal class ShortBlitter
    {
        private ShortProcessor ip;
        private int width, height;
        private short[] pixels;


        /** Constructs a ShortBlitter from a ShortProcessor. */
        public ShortBlitter(ShortProcessor ip)
        {
            this.ip = ip;
            width = ip.Width;
            height = ip.Height;
            pixels = (short[])ip.GetPixels();
        }

        public void SetTransparentColor(Color c)
        {
        }

        /** Copies the byte image in 'ip' to (x,y) using the specified mode. */
        public void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode)
        {

            int srcIndex, dstIndex;
            int xSrcBase, ySrcBase;
            short[] srcPixels;

            int srcWidth = ip.Width;
            int srcHeight = ip.Height;
            Rectangle r1 = new Rectangle(0, 0, srcWidth, srcHeight);
            r1.Location = new Point(xloc, yloc);
            Rectangle r2 = new Rectangle(0, 0, width, height);
            if (Rectangle.Intersect(r1, r2) == Rectangle.Empty)
                return;
            srcPixels = (short[])ip.GetPixels();
            //new ij.ImagePlus("srcPixels", new ShortProcessor(srcWidth, srcHeight, srcPixels, null)).show();
            r1 = Rectangle.Intersect(r1, r2);
            xSrcBase = xloc < 0 ? -xloc : 0;
            ySrcBase = yloc < 0 ? -yloc : 0;
            int src, dst;
            for (int y = r1.Y; y < r1.Y + r1.Height; y++)
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
                            src = srcPixels[srcIndex++] & 0xffff;
                            if (src == 0)
                                dst = pixels[dstIndex];
                            else
                                dst = src;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.ADD:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (srcPixels[srcIndex++] & 0xffff) + (pixels[dstIndex] & 0xffff);
                            if (dst < 0) dst = 0;
                            if (dst > 65535) dst = 65535;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.AVERAGE:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = ((srcPixels[srcIndex++] & 0xffff) + (pixels[dstIndex] & 0xffff)) / 2;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.DIFFERENCE:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (pixels[dstIndex] & 0xffff) - (srcPixels[srcIndex++] & 0xffff);
                            if (dst < 0) dst = -dst;
                            if (dst > 65535) dst = 65535;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.SUBTRACT:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (pixels[dstIndex] & 0xffff) - (srcPixels[srcIndex++] & 0xffff);
                            if (dst < 0) dst = 0;
                            if (dst > 65535) dst = 65535;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.MULTIPLY:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = (srcPixels[srcIndex++] & 0xffff) * (pixels[dstIndex] & 0xffff);
                            if (dst < 0) dst = 0;
                            if (dst > 65535) dst = 65535;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.DIVIDE:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++] & 0xffff;
                            if (src == 0)
                                dst = 65535;
                            else
                                dst = (pixels[dstIndex] & 0xffff) / src;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.AND:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = srcPixels[srcIndex++] & pixels[dstIndex] & 0xffff;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.OR:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = srcPixels[srcIndex++] | pixels[dstIndex];
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.XOR:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            dst = srcPixels[srcIndex++] ^ pixels[dstIndex];
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.MIN:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++] & 0xffff;
                            dst = pixels[dstIndex] & 0xffff;
                            if (src < dst) dst = src;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                    case Blitter.MAX:
                        for (int i = r1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++] & 0xffff;
                            dst = pixels[dstIndex] & 0xffff;
                            if (src > dst) dst = src;
                            pixels[dstIndex++] = (short)dst;
                        }
                        break;
                }
            }
        }
    }
}
