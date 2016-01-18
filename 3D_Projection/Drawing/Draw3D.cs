using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace _3D_Projection.Drawing
{
    public class Draw3D
    {
    }

    public class Point3
    {
        public double W;
        public double X;
        public double Y;
        public double Z;

        public Point3()
        {
        }

        public Point3(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public void Transform(Matrix3 m)
        {
            var result = m.VectorMultiply(new double[4] {X, Y, Z, W});
            X = result[0];
            Y = result[1];
            Z = result[2];
            W = result[3];
        }

        public void TransformNormalize(Matrix3 m)
        {
            var result = m.VectorMultiply(new double[4] {X, Y, Z, W});

            X = result[0]/result[3];
            Y = result[1]/result[3];
            Z = result[2];
            W = 1;
        }
    }

    public class Matrix3
    {
        public double[,] M = new double[4, 4];

        public Matrix3()
        {
            Identity3();

            Matrix3D m3 = new Matrix3D();

            m3.SetIdentity();

        }

        public Matrix3(double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33)
        {
            M[0, 0] = m00;
            M[0, 1] = m01;
            M[0, 2] = m02;
            M[0, 3] = m03;

            M[1, 0] = m10;
            M[1, 1] = m11;
            M[1, 2] = m12;
            M[1, 3] = m13;

            M[2, 0] = m20;
            M[2, 1] = m21;
            M[2, 2] = m22;
            M[2, 3] = m23;

            M[3, 0] = m30;
            M[3, 1] = m31;
            M[3, 2] = m32;
            M[3, 3] = m33;
        }

        public void Identity3()
        {
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if (i == j)
                    {
                        M[i, j] = 1;
                    }
                    else
                    {
                        M[i, j] = 0;
                    }
                }
            }
        }


        public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            var result = new Matrix3();

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    double element = 0;

                    for (var k = 0; k < 4; k++)
                    {
                        element += m1.M[i, k]*m2.M[k, j];
                    }

                    result.M[i, j] = element;
                }
            }

            return result;
        }

        public double[] VectorMultiply(double[] vector)
        {
            var result = new double[4];

            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    result[i] += (M[i, j]*vector[j]);
                }
            }

            return result;
        }

        public static Matrix3 Scale3(double sx, double sy, double sz)
        {
            var result = new Matrix3();

            result.M[0, 0] = sx;
            result.M[1, 1] = sy;
            result.M[2, 2] = sz;

            return result;
        }

        public static Matrix3 Translate3(double dx, double dy, double dz)
        {
            var result = new Matrix3();

            result.M[0, 3] = dx;
            result.M[1, 3] = dy;
            result.M[2, 3] = dz;

            return result;
        }

        public static Matrix3 Rotate3X(double theta)
        {
            theta = theta*Math.PI/180.0f;

            var sn = Math.Sin(theta);
            var cn = Math.Cos(theta);

            var result = new Matrix3();

            result.M[1, 1] = cn;
            result.M[1, 2] = -sn;
            result.M[2, 1] = sn;
            result.M[2, 2] = cn;

            return result;
        }

        public static Matrix3 Rotate3Y(double theta)
        {
            theta = theta*Math.PI/180.0f;

            var sn = Math.Sin(theta);
            var cn = Math.Cos(theta);

            var result = new Matrix3();

            result.M[0, 0] = cn;
            result.M[0, 2] = sn;
            result.M[2, 0] = -sn;
            result.M[2, 2] = cn;

            return result;
        }

        public static Matrix3 Rotate3Z(double theta)
        {
            theta = theta*Math.PI/180.0f;

            var sn = Math.Sin(theta);
            var cn = Math.Cos(theta);

            var result = new Matrix3();

            result.M[0, 0] = cn;
            result.M[0, 1] = -sn;
            result.M[1, 0] = sn;
            result.M[1, 1] = cn;

            return result;
        }

        public static Matrix3 FrontView()
        {
            var result = new Matrix3();

            result.M[2, 2] = 0;

            return result;
        }

        public static Matrix3 SideView()
        {
            var result = new Matrix3();

            result.M[0, 0] = 0;
            result.M[2, 2] = 0;
            result.M[0, 2] = -1;

            return result;
        }

        public static Matrix3 TopView()
        {
            var result = new Matrix3();

            result.M[1, 1] = 0;
            result.M[2, 2] = 0;
            result.M[1, 2] = -1;

            return result;
        }

        public static Matrix3 Axonometric(double alpha, double beta)
        {
            var result = new Matrix3();

            var sna = Math.Sin(alpha*Math.PI/180);
            var cna = Math.Cos(alpha*Math.PI/180);

            var snb = Math.Sin(beta*Math.PI/180);
            var cnb = Math.Sin(beta*Math.PI/180);

            result.M[0, 0] = cnb;
            result.M[0, 2] = snb;
            result.M[1, 0] = sna*snb;
            result.M[1, 1] = cna;
            result.M[1, 2] = -sna*cnb;
            result.M[2, 2] = 0;

            return result;
        }

        public static Matrix3 Oblique(double alpha, double theta)
        {
            var result = new Matrix3();

            var ta = Math.Tan(alpha*Math.PI/180);
            var snt = Math.Sin(theta*Math.PI/180);
            var cnt = Math.Cos(theta*Math.PI/180);

            result.M[0, 2] = -cnt/ta;
            result.M[1, 2] = -snt/ta;
            result.M[2, 2] = 0;

            return result;
        }

        public static Matrix3 Perspective(double d)
        {
            var result = new Matrix3();

            result.M[3, 2] = -1/d;

            return result;
        }

        public Point3 Cylindrical(double r, double theta, double y)
        {
            var pt = new Point3();

            var sn = Math.Sin(theta*Math.PI/180);
            var cn = Math.Cos(theta*Math.PI/180);

            pt.X = r*cn;
            pt.Y = y;
            pt.Z = -r*sn;
            pt.W = 1;

            return pt;
        }

        public Point3 Spherical(double r, double theta, double phi)
        {
            var pt = new Point3();

            var snt = Math.Sin(theta*Math.PI/180);
            var cnt = Math.Cos(theta*Math.PI/180);

            var snp = Math.Sin(phi*Math.PI/180);
            var cnp = Math.Cos(phi*Math.PI/180);

            pt.X = r*snt*cnp;
            pt.Y = r*cnt;
            pt.Z = -r*snt*cnp;
            pt.W = 1;

            return pt;
        }

        public static Matrix3 Euler(double alpha, double beta, double gamma)
        {
            var result = new Matrix3();

            alpha = alpha*Math.PI/180.0f;
            beta = beta*Math.PI/180.0f;
            gamma = gamma*Math.PI/180.0f;

            var sna = Math.Sin(alpha*Math.PI/180);
            var cna = Math.Cos(alpha*Math.PI/180);

            var snb = Math.Sin(beta*Math.PI/180);
            var cnb = Math.Cos(beta*Math.PI/180);

            var sng = Math.Sin(gamma*Math.PI/180);
            var cng = Math.Cos(gamma*Math.PI/180);

            result.M[0, 0] = cna*cng - sna*snb*sng;
            result.M[0, 1] = -snb*sng;
            result.M[0, 2] = sna*cng - cna*cnb*sng;

            result.M[1, 0] = -sna*snb;
            result.M[1, 1] = cnb;
            result.M[1, 2] = cna*cnb;

            result.M[2, 0] = -cna*sng - sna*cnb*cng;
            result.M[2, 1] = -snb*cng;
            result.M[2, 2] = cna*cnb*cng - sna*snb;

            return result;
        }

        public static Matrix3 AzimuthElevation(double elevation, double azimuth, double oneOverd)
        {
            var result = new Matrix3();
            var rotate = new Matrix3();

            if (elevation > 90)
            {
                elevation = 90;
            }

            else if (elevation < -90)
            {
                elevation = -90;
            }

            if (azimuth > 180)
            {
                azimuth = 180;
            }

            else if (azimuth < -180)
            {
                azimuth = -180;
            }

            elevation = elevation*Math.PI/180.0;
            azimuth = azimuth*Math.PI/180.0;

            var sne = Math.Sin(elevation);
            var cne = Math.Cos(elevation);

            var sna = Math.Sin(azimuth);
            var cna = Math.Cos(azimuth);

            rotate.M[0, 0] = cna;
            rotate.M[0, 1] = sna;
            rotate.M[0, 2] = 0;

            rotate.M[1, 0] = -sne*sna;
            rotate.M[1, 1] = sne*cna;
            rotate.M[1, 2] = cne;

            rotate.M[2, 0] = cne*sna;
            rotate.M[2, 1] = -cne*cna;
            rotate.M[2, 2] = sne;

            if (oneOverd <= 0)
            {
                result = rotate;
            }

            else if (oneOverd > 0)
            {
                var perspective = Perspective(1/oneOverd);
                result = perspective*rotate;
            }

            return result;
        }
    }

    public class Draw
    {
        public Canvas stage { get; set; }

        public Point DrawPoint(Point3 convertPoint)
        {
            var result = new Point();

            return result;
        }
    }
}