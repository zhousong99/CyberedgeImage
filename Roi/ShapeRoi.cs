using System.Collections;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace CyberedgeImageProcess2024
{
    class ShapeRoi : Roi
    {
        const double MAXERROR = 1.0e-3;
        const double FLATNESS = 0.1;

        private const int MAXPOLY = 10; 
        private const int OR = 0, AND = 1, XOR = 2, NOT = 3;
        private GraphicsPath shape;

        /** Constructs a ShapeRoi from an Roi. */
        public ShapeRoi(Roi r) : this(r, ShapeRoi.FLATNESS, ShapeRoi.MAXERROR, false, false, false, ShapeRoi.MAXPOLY)
        {
        }

         ShapeRoi(Roi r, double flatness, double maxerror, bool forceAngle, bool forceTrace, bool flatten, int maxPoly) : base(r.StartX, r.StartY, r.Width, r.Height)
        {
        }


        public GraphicsPath Shape
        {
            get { return shape; }
        }

        /// <summary>
        /// 形状shape转换为ROI
        /// </summary>
        /// <returns></returns>
        public Roi ShapeToRoi()
        {
            if (shape == null || !(shape is GraphicsPath))
                return null;
            GraphicsPathIterator pIter = new GraphicsPathIterator(shape);
            ArrayList rois = new ArrayList();
            parsePath(shape, ONE_ROI, rois);
            if (rois.Count == 1)
                return (Roi)rois[0];
            else
                return null;
        }
        
        public Roi trySimplify()
        {
            Roi roi = ShapeToRoi();
            return  roi ?? this; ;
        }

        #region Logical operations on shaped rois
        public ShapeRoi or(ShapeRoi sr) { return unaryOp(sr, OR); }

        public ShapeRoi not(ShapeRoi sr) { return unaryOp(sr, NOT); }

        ShapeRoi unaryOp(ShapeRoi sr, int op)
        {
            Matrix at = new Matrix();
            at.Translate(x, y);
            GraphicsPath transformedShape1 = new GraphicsPath();
            transformedShape1.AddPath(shape, true);
            transformedShape1.Transform(at);

            at = new Matrix();
            at.Translate(sr.x, sr.y);
            GraphicsPath transformedShape2 = new GraphicsPath();
            transformedShape2.AddPath(sr.Shape, true);
            transformedShape2.Transform(at);

            Region a1 = new Region(transformedShape1);
            Region a2 = new Region(transformedShape2);

            try
            {
                switch (op)
                {
                    case OR: a1.Union(a2); break;
                    case AND: a1.Intersect(a2); break;
                    case XOR: a1.Xor(a2); break;
                    case NOT: a1.Exclude(a2); break;
                }
            }
            catch (Exception) { }
            RectangleF r = a1.GetBounds(Graphics.FromHwnd(IntPtr.Zero));
            at = new Matrix();
            at.Translate(-r.X, -r.Y);
            GraphicsPath resultPath = new GraphicsPath();
            resultPath = PublicFunctions.ConvertRegionToGraphicsPath(a1);
            resultPath.Transform(at);

            shape = resultPath;
            x = (int)r.X;
            y = (int)r.Y;
            cachedMask = null;
            return this;
        }
        #endregion

        const int ALL_ROIS = 0, ONE_ROI = 1, GET_LENGTH = 2; //task types
        const int NO_SEGMENT_ANY_MORE = -1; //pseudo segment type when closed
        
        /// <summary>
        /// 将路径shape生成roi添加到rois中
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="task"></param>
        /// <param name="rois"></param>
        /// <returns></returns>
        double parsePath(GraphicsPath shape, int task, ArrayList rois)
        {
            if (shape == null)
                return 0.0;
            if (imp != null)
            {
            }
            float xbase = (float)GetXBase();
            float ybase = (float)GetYBase();

            List<float> xPoints = new List<float>();    //vertex coordinates of current subpath
            List<float> yPoints = new List<float>();
            List<float> shapeArray = new List<float>(); //values for creating a GeneralPath for the current subpath
            bool getLength = task == GET_LENGTH;
            int nSubPaths = 0;           // the number of subpaths
            bool horVertOnly = true; // subpath has only horizontal or vertical lines
            bool closed = false;
            //bool success = false;
            float[] fcoords = new float[6];  // unscaled float coordinates of the path segment
            double[] coords = new double[6]; // scaled (calibrated) coordinates of the path segment
            double uncalLength = 0.0; // uncalibrated length of polygon, NaN in case of curves
            bool done = false;


            //下面是简化的，认为只有一个Roi
            // Get all X coordinates
            float[] xpf = Array.ConvertAll(shape.PathPoints, p => p.X);

            // Get all Y coordinates
            float[] ypf = Array.ConvertAll(shape.PathPoints, p => p.Y);
            closed = closed || (xpf.Length > 0 && xpf[0] == xpf.Last() && ypf[0] == ypf.Last());
            if (Double.IsNaN(uncalLength) || !AllInteger(xpf) || !AllInteger(ypf))
                horVertOnly = false;         //allow conversion to rectangle or traced roi only for integer coordinates
            bool forceTrace = getLength && (!done || nSubPaths > 0);  //when calculating the length for >1 subpath, assume traced rois if it can be such
            int roiType = GuessType(xPoints.Count, uncalLength, horVertOnly, forceTrace, closed);


            rois.Add(this);
            return 0.0;
        }



        bool AllInteger(float[] a)
        {
            for (int i = 0; i < a.Length; i++)
                if (a[i] != (int)a[i]) return false;
            return true;
        }

        private int GuessType(int nSegments, double polygonLength, bool horizontalVerticalIntOnly, bool forceTrace, bool closed)
        {
            return Roi.RECTANGLE;
        }

    }
}