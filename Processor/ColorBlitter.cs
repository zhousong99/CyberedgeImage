


namespace CyberedgeImageProcess2024
{
    internal class ColorBlitter
    {
        private ColorProcessor ip;
        private int width, height;
        private Color[] pixels;
        private Color transparent = Color.White;

        /** Constructs a ColorBlitter from a ColorProcessor. */
        public ColorBlitter(ColorProcessor ip)
        {
            this.ip = ip;
            width = ip.Width;
            height = ip.Height;
            pixels = (Color[])ip.GetPixels();
        }


        /** Copies the RGB image in 'ip' to (x,y) using the specified mode. */
        public void CopyBits(ImageProcessor ip, int xloc, int yloc, int mode)
        {
            int srcIndex, dstIndex;
            int xSrcBase, ySrcBase;
            Color[] srcPixels;

            int srcWidth = ip.Width;
            int srcHeight = ip.Height;
            Rectangle rect1 = new Rectangle(0, 0, srcWidth, srcHeight);
            rect1.Location = new Point(xloc, yloc);
            Rectangle rect2 = new Rectangle(0, 0, width, height);
            if (Rectangle.Intersect(rect1, rect2) == Rectangle.Empty)
                return;
            if (ip is ByteProcessor)
            {
                byte[] pixels8 = (byte[])ip.GetPixels();
                int size = ip.Width * ip.Height;
                srcPixels = new Color[size];
                for (int i = 0; i < size; i++)
                    srcPixels[i] = Color.FromArgb(255, pixels8[i] & 255, pixels8[i] & 255, pixels8[i] & 255);
            }
            else
                srcPixels = (Color[])ip.GetPixels();
            rect1 = Rectangle.Intersect(rect1, rect2);
            xSrcBase = (xloc < 0) ? -xloc : 0;
            ySrcBase = (yloc < 0) ? -yloc : 0;
            Color c1, c2;
            int r1, g1, b1, r2, g2, b2;
            Color src, dst;

            if (mode == Blitter.COPY || mode == Blitter.COPY_TRANSPARENT || mode == Blitter.COPY_ZERO_TRANSPARENT)
            {
                for (int y = rect1.Y; y < (rect1.Y + rect1.Height); y++)
                {
                    srcIndex = (y - yloc) * srcWidth + (rect1.X - xloc);
                    dstIndex = y * width + rect1.X;
                    Color trancolor = mode == Blitter.COPY_ZERO_TRANSPARENT ? Color.Black : transparent;
                    if (mode == Blitter.COPY)
                    {
                        for (int i = rect1.Width; --i >= 0;)
                            pixels[dstIndex++] = srcPixels[srcIndex++];
                    }
                    else
                    {
                        for (int i = rect1.Width; --i >= 0;)
                        {
                            src = srcPixels[srcIndex++];
                            dst = pixels[dstIndex];
                            pixels[dstIndex++] = src == trancolor ? dst : src;
                        }
                    }
                }
                return;
            }

            for (int y = rect1.Y; y < (rect1.Y + rect1.Height); y++)
            {
                srcIndex = (y - yloc) * srcWidth + (rect1.X - xloc);
                dstIndex = y * width + rect1.X;
                for (int i = rect1.Width; --i >= 0;)
                {
                    c1 = srcPixels[srcIndex++];
                    r1 = c1.R;
                    g1 = c1.G;
                    b1 = c1.B;
                    c2 = pixels[dstIndex];
                    r2 = c2.R;
                    g2 = c2.G;
                    b2 = c2.B;
                    switch (mode)
                    {
                        case Blitter.COPY_INVERTED:
                            break;
                        case Blitter.ADD:
                            r2 = (byte)(r1 + r2); g2 = (byte)(g1 + g2); b2 = (byte)(b1 + b2);
                            if (r2 > 255) r2 = 255; if (g2 > 255) g2 = 255; if (b2 > 255) b2 = 255;
                            break;
                        case Blitter.AVERAGE:
                            r2 = (r1 + r2) / 2; g2 = (g1 + g2) / 2; b2 = (b1 + b2) / 2;
                            break;
                        case Blitter.SUBTRACT:
                            r2 = r2 - r1; g2 = g2 - g1; b2 = b2 - b1;
                            if (r2 < 0) r2 = 0; if (g2 < 0) g2 = 0; if (b2 < 0) b2 = 0;
                            break;
                        case Blitter.DIFFERENCE:
                            r2 = r2 - r1; if (r2 < 0) r2 = -r2;
                            g2 = g2 - g1; if (g2 < 0) g2 = -g2;
                            b2 = b2 - b1; if (b2 < 0) b2 = -b2;
                            break;
                        case Blitter.MULTIPLY:
                            r2 = r1 * r2; g2 = g1 * g2; b2 = b1 * b2;
                            if (r2 > 255) r2 = 255; if (g2 > 255) g2 = 255; if (b2 > 255) b2 = 255;
                            break;
                        case Blitter.DIVIDE:
                            if (r1 == 0) r2 = 255; else r2 = r2 / r1;
                            if (g1 == 0) g2 = 255; else g2 = g2 / g1;
                            if (b1 == 0) b2 = 255; else b2 = b2 / b1;
                            break;
                        case Blitter.AND:
                            r2 = r1 & r2; g2 = g1 & g2; b2 = b1 & b2;
                            break;
                        case Blitter.OR:
                            r2 = r1 | r2; g2 = g1 | g2; b2 = b1 | b2;
                            break;
                        case Blitter.XOR:
                            r2 = r1 ^ r2; g2 = g1 ^ g2; b2 = b1 ^ b2;
                            break;
                        case Blitter.MIN:
                            if (r1 < r2) r2 = r1;
                            if (g1 < g2) g2 = g1;
                            if (b1 < b2) b2 = b1;
                            break;
                        case Blitter.MAX:
                            if (r1 > r2) r2 = r1;
                            if (g1 > g2) g2 = g1;
                            if (b1 > b2) b2 = b1;
                            break;
                    }
                    pixels[dstIndex++] = Color.FromArgb(255, r2, g2, b2);
                }
            }
        }
    }
}
