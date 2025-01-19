namespace CyberedgeImageProcess2024
{
    public class FloatPolygon
    {
        private Rectangle bounds;
        private float minX, minY, maxX, maxY;

        /** The number of points. */
        public int npoints;

        /* The array of x coordinates. */
        public float[] xpoints;

        /* The array of y coordinates. */
        public float[] ypoints;

        /** Constructs an empty FloatPolygon. */
        public FloatPolygon()
        {
            npoints = 0;
            xpoints = new float[10];
            ypoints = new float[10];
        }

        /** Constructs a FloatPolygon from x and y arrays. */
        public FloatPolygon(float[] xpoints, float[] ypoints)
        {
            if (xpoints.Length != ypoints.Length)
                throw new Exception("xpoints.length!=ypoints.length");
            this.npoints = xpoints.Length;
            this.xpoints = xpoints;
            this.ypoints = ypoints;
        }

        /** Constructs a FloatPolygon from x and y arrays. */
        public FloatPolygon(float[] xpoints, float[] ypoints, int npoints)
        {
            this.npoints = npoints;
            this.xpoints = xpoints;
            this.ypoints = ypoints;
        }








    }
}
