using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace _3D_Projection.Drawing
{

    internal class DrawCube
    {

        private double cx = 984/2;
        private double cy = 960/2;

        public DrawCube()
        {
            Point3[] pts = CubePoints();
            Point[] pta = new Point[5];

          

            collection = pta;
        }

        public Point DrawStar(double sx,double sy,double sz)
        {
            Point result=new Point(0,0);

            Point3[] pts = new Point3 [1];

            pts[0]= new Point3(sx,sy,sz,1);

            Matrix3 m = Matrix3.AzimuthElevation(0,30,0);

            pts[0].Transform(m);
            
            result = Point2D(new Point(pts[0].X, pts[0].Y));

            return result;
        }

        public Point3D AnimateStar(double sx,double sy,double sz, double rx, double ry,
            double rz,double elevation,double azimuth)
        {
            Point3D result = new Point3D(0, 0, 0);
            
            Point3[] pts = new Point3[1];

            pts[0] = new Point3(sx, sy, sz, 1);

            Matrix3 r3 = Matrix3.Rotate3Z(rz);

            pts[0].Transform(r3);

            Matrix3 r1 = Matrix3.Rotate3X(rx);

            pts[0].Transform(r1);

            Matrix3 r2 = Matrix3.Rotate3Y(ry);

            pts[0].Transform(r2);

            Matrix3 m = Matrix3.AzimuthElevation(elevation, azimuth, 0);
            pts[0].Transform(m);
 
            result = new Point3D(pts[0].X, pts[0].Y, pts[0].Z);
            return result;
        }

        public Point Point2D(Point pt)
        {
            Point aPoint = new Point();

            aPoint.X = pt.X; // cx + pt.X;
            aPoint.Y = pt.Y; // cy + pt.Y;

            return aPoint;

        }

        public Point3[] CubePoints()
        {

            double a = 200;

            Point3[] pts = new Point3[8];

            pts[0] = new Point3(0, 0, 0, 0);
            pts[1] = new Point3(a, 0, 0, 0);
            pts[2] = new Point3(a, a, 0, 0);
            pts[3] = new Point3(0, a, 0, 0);
            pts[4] = new Point3(0, 0, a, 1);
            pts[5] = new Point3(a, 0, a, 1);
            pts[6] = new Point3(a, a, a, 1);
            pts[7] = new Point3(0, a, a, 1);
           

            return pts;
        }

        public Point[] collection { get; set; }
        public Canvas drawStage { get; set; }
        
    
        public void DrawFrontView(Canvas stage)
        {
            Point3[] pts = CubePoints();
          //  Matrix3 m = Matrix3.Euler(30, 60, -120);
              Matrix3 m = Matrix3.Axonometric(60,-30);
          //  Matrix3 m= Matrix3.AzimuthElevation(-60,30,0.5);
            PointCollection pc = new PointCollection();

            Polygon poly = new Polygon();

            Matrix3 r1 = Matrix3.Rotate3Y(0);

            for (int i = 0; i < pts.Length; i++)
            {
                pts[i].Transform(r1);
            }

            Matrix3 r2 = Matrix3.Rotate3Z(30);

            for (int i = 0; i < pts.Length; i++)
            {
                pts[i].Transform(r2);
            }

            for (int i = 0; i < pts.Length; i++)
            {
                pts[i].Transform(m);
            }

            Point[] pta = new Point[8];

            pta[0] = Point2D(new Point(pts[0].X, pts[0].Y)); 
            pta[1] = Point2D(new Point(pts[1].X, pts[1].Y)); 
            pta[2] = Point2D(new Point(pts[2].X, pts[2].Y)); 
            pta[3] = Point2D(new Point(pts[3].X, pts[3].Y));
            pta[4] = Point2D(new Point(pts[4].X, pts[4].Y));
            pta[5] = Point2D(new Point(pts[5].X, pts[5].Y)); 
            pta[6] = Point2D(new Point(pts[6].X, pts[6].Y)); 
            pta[7] = Point2D(new Point(pts[7].X, pts[7].Y));

            Line line = new Line();

            // front

            pc.Add(pta[0]);
            pc.Add(pta[1]);
            pc.Add(pta[2]);
            pc.Add(pta[3]);
          
            poly.Fill = new SolidColorBrush(Colors.Blue);
            poly.Stroke = new SolidColorBrush(Colors.White);
            poly.StrokeThickness = 3;

            poly.Points = pc;

            stage.Children.Add(poly);

            pc = new PointCollection();

            poly = new Polygon();

            // back

            pc.Add(pta[4]);
            pc.Add(pta[5]);
            pc.Add(pta[6]);
            pc.Add(pta[7]);

            poly.Fill = new SolidColorBrush(Colors.Red);
            poly.Stroke = new SolidColorBrush(Colors.White);
            poly.StrokeThickness = 1;

            poly.Points = pc;

            stage.Children.Add(poly);

            pc = new PointCollection();

            poly = new Polygon();

            // left

            pc.Add(pta[0]);
            pc.Add(pta[4]);
            pc.Add(pta[7]);
            pc.Add(pta[3]);

            poly.Fill = new SolidColorBrush(Colors.Green);
            poly.Stroke = new SolidColorBrush(Colors.White);
            poly.StrokeThickness = 1;

            poly.Points = pc;

            stage.Children.Add(poly);

            pc = new PointCollection();

            poly = new Polygon();

            // right

            pc.Add(pta[1]);
            pc.Add(pta[5]);
            pc.Add(pta[6]);
            pc.Add(pta[2]);

            poly.Fill = new SolidColorBrush(Colors.Yellow);
            poly.Stroke = new SolidColorBrush(Colors.White);
            poly.StrokeThickness = 1;

            poly.Points = pc;

            stage.Children.Add(poly);

            pc = new PointCollection();

            poly = new Polygon();
            // drawStage = stage;

        }
    }
}
