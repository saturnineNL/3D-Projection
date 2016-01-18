using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;
using _3D_Projection.Dataset;
using _3D_Projection.Drawing;
using Path = System.Windows.Shapes.Path;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace _3D_Projection
{

    public partial class MainWindow : Window
    {

        Dictionary<int, StarSystem> starSystemCollection = new Dictionary<int, StarSystem>();
        List<Tuple<double, double, double>> coordinates = new List<Tuple<double, double, double>>();
        List<string>  systemName = new List<string>();
        int count = 0;
        
        private double rx = 0;
        private double ry = 0;
        private double rz = 0;
        private double hg = 0;

        private double xFactor = 0.0;
        private double yFactor = 0.0;
        private double zFactor = 0.0;
        private double rFactor = 0.0;

        private double maxDist = 64;
        private double minDist = 32;
        private double zoom = 7.5;

        private int delta = 0;

        private double elevation = 30;
        private double azimuth = 0;

        private double elevationFactor = 0;
        private double azimuthFactor = 0;

        private int eSign = 1;
        private int aSign = 1;

        private List<Star> starCollection = new List<Star>();

        DispatcherTimer timer = new DispatcherTimer();

        Point3D p3D=new Point3D();

        private int targetStar = 0;

        private Point reference=new Point(0,0);

        private int targetSystem;

        bool render = false;

        private bool lmb = false;

        List<string> inputList = new List<string>();

        private string oldSearch = "";

        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += Timer_Tick;

            String systemDataSet = EmbeddedRead.GetJSON();

            dynamic result = JsonConvert.DeserializeObject(systemDataSet);

            StringBuilder builder = new StringBuilder();                     

            tb.FontFamily = new FontFamily(new Uri("pack://application:,,,/Dataset/"), "./#Euro Caps");
            inputBox.FontFamily = new FontFamily(new Uri("pack://application:,,,/Dataset/"), "./#Euro Caps");
            inputBox.FontSize = 24;
            inputBox.VerticalContentAlignment=VerticalAlignment.Center;

            foreach (var systemData in result)
            {

                starSystemCollection[count] = new StarSystem();
                starSystemCollection[count].name = systemData.name;
                starSystemCollection[count].faction = systemData.faction;
                starSystemCollection[count].primary_economy = systemData.primary_economy;
                starSystemCollection[count].state = systemData.state;
                starSystemCollection[count].government = systemData.government;
                starSystemCollection[count].id = systemData.id;
                starSystemCollection[count].x = systemData.x;
                starSystemCollection[count].y = systemData.y;
                starSystemCollection[count].z = systemData.z;

                count += 1;

            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    SearchControlSetup(i, j, "");
                }
            }

            Random generator = new Random((int)DateTime.Now.Ticks);

            targetSystem = generator.Next(0, count);

            findTargetSystem(targetSystem);

        }

        private void InputBox_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void findTargetSystem(int target)

        {
            targetSystem = target;

          //  ProjectionViewCanvas.Children.Clear();

            starCollection.Clear();

            zoom = (64 / maxDist) * 7.5;

            double targetX = starSystemCollection[target].x;
            double targetY = starSystemCollection[target].y;
            double targetZ = starSystemCollection[target].z;

            int counter = 0;
            
            foreach (var systemNames in starSystemCollection.OrderBy(p => p.Value.z))
            {

                double distance = calculateDistance(target, systemNames.Key);

                if (distance == 0 || (distance>=minDist && distance <= maxDist))
                               
                {
                    
                    Star star = new Star();                    

                    star.starID = systemNames.Key;

                    star.SetName(systemNames.Value.name);

                    star.X = (targetX - systemNames.Value.x) * zoom;
                    star.Y = (targetY - systemNames.Value.y) * zoom;
                    star.Z = (targetZ - systemNames.Value.z) * zoom;

                    star.originX = systemNames.Value.x;
                    star.originY = systemNames.Value.y;
                    star.originZ = systemNames.Value.z;

                    star.rotaX = 0;
                    star.rotaY = 0;
                    star.rotaZ = 0;

                    star.centerX = ProjectionViewCanvas.Width/2;
                    star.centerY = ProjectionViewCanvas.Height/2;

                    star.distance = distance;

                    star.maxDistance = maxDist;

                    star.rendered = false;

                    star.size = 12;
                    
                    star.SetEllipseSize();
                    
                    starCollection.Add(star);
                                       
                    counter += 1;

                }
                 
            }

            tb.Text = "[" + target + "] " + starSystemCollection[target].name + " : (" + targetX.ToString("F") +
                              "," + targetY.ToString("F") + "," + targetZ.ToString("F") + ") " + " zoom: " + maxDist + "LY" + "\ttotal:" +
                               counter;

            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Start();

            xFactor = 0.0;
            yFactor = 0.0;
            zFactor = 0.5;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ProjectionViewCanvas.Children.Clear();
            
            if (!lmb)
            {
                rx += xFactor;
                ry += yFactor;
                rz += zFactor;
            }

            if (rz == 360) rz = 0;

            Drawplane();

            DrawEdge();

            DrawControls();

            foreach (var star in starCollection.OrderBy(p => p.Z))
            {
                star.rotaX = rx;
                star.rotaY = ry;
                star.rotaZ = rz;

                star.elevation = elevation;
                star.azimuth = azimuth;

                star.Draw3D();

                star.SetEllipseSize();

                star.setColor(hg);

                star.SetLabel();

                ProjectionViewCanvas.Children.Add(star.starCanvas);

            }

        }

        private void DrawControls()
        {
            SolidColorBrush xAxisColor=new SolidColorBrush(Colors.DarkGreen);
            SolidColorBrush yAxisColor = new SolidColorBrush(Colors.Blue);
            SolidColorBrush zAxisColor = new SolidColorBrush(Colors.Crimson);

            double thickness = 3;

            Line xAxisLine = drawLine(new Point3D(0, 0, 0), new Point3D(10 * 7.5, 0, 0));
            Line yAxisLine = drawLine(new Point3D(0, 0, 0), new Point3D(0, 10 * 7.5, 0));
            Line zAxisLine = drawLine(new Point3D(0, 0, 0), new Point3D(0, 0, 10 * 7.5));

            xAxisLine.Stroke = xAxisColor;
            xAxisLine.StrokeThickness = thickness;

            yAxisLine.Stroke = yAxisColor;
            yAxisLine.StrokeThickness = thickness;

            zAxisLine.Stroke = zAxisColor;
            zAxisLine.StrokeThickness = thickness;

            ProjectionViewCanvas.Children.Add(xAxisLine);
            ProjectionViewCanvas.Children.Add(yAxisLine);
            ProjectionViewCanvas.Children.Add(zAxisLine);

        }

        private Line drawLine(Point3D start, Point3D end)
        {
            Line line = new Line();

            DrawCube projector = new DrawCube();

            Point3D projectedStartPoint = projector.AnimateStar(start.X, start.Y, start.Z, rx, ry, -rz, elevation,azimuth);
            Point3D projectedEndPoint   = projector.AnimateStar(end.X, end.Y, end.Z, rx, ry, -rz, elevation, azimuth);

            line.X1 = ProjectionViewCanvas.Width/2 - projectedStartPoint.X;
            line.X2 = ProjectionViewCanvas.Width / 2 - projectedEndPoint.X ;

            line.Y1 = ProjectionViewCanvas.Height / 2 - projectedStartPoint.Y;
            line.Y2 = ProjectionViewCanvas.Height / 2 - projectedEndPoint.Y ;

            return line;
        }

        private void Drawplane()
        {
            DrawCube projector = new DrawCube();
            Point3D projectionPoint = new Point3D();

            Polygon plane = new Polygon();

            PointCollection pointCollection = new PointCollection();

            double distance = 480;

            for (int degreeStep = 0; degreeStep < 180; degreeStep+=2)
            {
                double degree = -degreeStep * (2*Math.PI/180);

                projectionPoint = projector.AnimateStar(distance* Math.Sin(degree),  distance* Math.Cos(degree), 0 ,rx, ry,  -rz,elevation, azimuth);

                pointCollection.Add(new Point(ProjectionViewCanvas.Width / 2 - projectionPoint.X, ProjectionViewCanvas.Height / 2 - projectionPoint.Y));
            }

            plane.Points = pointCollection;
            plane.Stroke = new SolidColorBrush(Color.FromArgb(0xF0, 0x3D, 0x95, 0xDE));
            plane.StrokeThickness = 3;

            ProjectionViewCanvas.Children.Add(plane);
        }

        private void DrawEdge()
        {
            Ellipse edge = new Ellipse();

            edge.Height = edge.Width = 128 * 7.5;

            edge.Margin = new Thickness(0, 0, 0, 0);

            edge.Stroke = new SolidColorBrush(Color.FromArgb(0xF0, 0x3D, 0x95, 0xDE));
            edge.StrokeThickness = 3;

            ProjectionViewCanvas.Children.Add(edge);
        }

        private double calculateWidth(double a, double b)
        {
            double result = 0;

            double degree = 90 - Math.Sinh(a / b) / (Math.PI / 180);

            double calculator1 = 0;
            double calculator2 = 0;
            double calculator3 = 0;

            calculator1 = (a * a) + (b * b);
            calculator2 = 2 * (a * b);
            calculator3 = Math.Cos(degree * (Math.PI / 180));

            result = Math.Sqrt(calculator1 - (calculator2 * calculator3));

            return result;
        }

        private double calculateDistance(int homeSystem, int targetSystem)
        {
            double distance = -1;

            double homeSystemX = starSystemCollection[homeSystem].x; double homeSystemY= starSystemCollection[homeSystem].y; double homeSystemZ = starSystemCollection[homeSystem].z;
            double targetSystemX = starSystemCollection[targetSystem].x; double targetSystemY = starSystemCollection[targetSystem].y; double targetSystemZ = starSystemCollection[targetSystem].z;

            double distanceX=999999;
            double distanceY=999999;
            double distanceZ=999999;

            if (homeSystemX <= 0 && targetSystemX < 0) distanceX = Math.Min(homeSystemX, targetSystemX) + Math.Abs(Math.Max(homeSystemX, targetSystemX));
            if (homeSystemY <= 0 && targetSystemY < 0) distanceY = Math.Min(homeSystemY, targetSystemY) + Math.Abs(Math.Max(homeSystemY, targetSystemY));
            if (homeSystemZ <= 0 && targetSystemZ < 0) distanceZ = Math.Min(homeSystemZ, targetSystemZ) + Math.Abs(Math.Max(homeSystemZ, targetSystemZ));

            if ((homeSystemX <= 0 && targetSystemX > 0) || (homeSystemX > 0 && targetSystemX < 0)) distanceX = Math.Abs(Math.Min(homeSystemX, targetSystemX)) + Math.Max(homeSystemX, targetSystemX);
            if ((homeSystemY <= 0 && targetSystemY > 0) || (homeSystemY > 0 && targetSystemY < 0)) distanceY = Math.Abs(Math.Min(homeSystemY, targetSystemY)) + Math.Max(homeSystemY, targetSystemY);
            if ((homeSystemZ <= 0 && targetSystemZ > 0) || (homeSystemZ > 0 && targetSystemZ < 0)) distanceZ = Math.Abs(Math.Min(homeSystemZ, targetSystemZ)) + Math.Max(homeSystemZ, targetSystemZ);

            if (homeSystemX >= 0 && targetSystemX > 0) distanceX = Math.Max(homeSystemX, targetSystemX) - Math.Min(homeSystemX, targetSystemX);
            if (homeSystemY >= 0 && targetSystemY > 0) distanceY = Math.Max(homeSystemY, targetSystemY) - Math.Min(homeSystemY, targetSystemY);
            if (homeSystemZ >= 0 && targetSystemZ > 0) distanceZ = Math.Max(homeSystemZ, targetSystemZ) - Math.Min(homeSystemZ, targetSystemZ);

            if (homeSystemX == targetSystemX) distanceX = 0;
            if (homeSystemY == targetSystemY) distanceY = 0;
            if (homeSystemZ == targetSystemZ) distanceZ = 0;

            distance = Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2) + Math.Pow(distanceZ, 2));

            return distance;
        }       

        private void lmbd_Handler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!lmb)
            {
                reference.X = Mouse.GetPosition(ProjectionViewCanvas).X-(ProjectionViewCanvas.Width/2);
                reference.Y = Mouse.GetPosition(ProjectionViewCanvas).Y-(ProjectionViewCanvas.Height/2);

                lmb = true; 
            }
        }

        private void lmbu_Handler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            lmb = false; 
        }

        private void rmbd_Handler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Random generator = new Random((int)DateTime.Now.Ticks);

            int id = generator.Next(0, count);

            findTargetSystem(id);

            
        }

        private void move_Handler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point movePoint = new Point();

            movePoint.X = Mouse.GetPosition(ProjectionViewCanvas).X - (ProjectionViewCanvas.Width / 2);
            movePoint.Y = Mouse.GetPosition(ProjectionViewCanvas).Y - (ProjectionViewCanvas.Width / 2);

            double el = 0;
            double az = 0;
            if (lmb)
            {
                az = ((reference.X - movePoint.X) / ProjectionViewCanvas.Width) * 128;
                el = ((reference.Y - movePoint.Y) / ProjectionViewCanvas.Height) * 128;
            }

            elevation -= el;
            azimuth += az;

            reference.X = movePoint.X;
            reference.Y = movePoint.Y;
        }

        private void wheel_Handler(object sender, MouseWheelEventArgs e)
        {
            delta = (e.Delta/30);

            maxDist -= (double)delta;

            if (maxDist < 4) { maxDist = 4; delta = 0; }
           
            if (maxDist > 80) { maxDist = 80; delta = 0; }

            if (maxDist >= 32)  {minDist = maxDist - 32;}

            
               findTargetSystem(targetSystem);

        }

        private void SearchClick(object sender, MouseButtonEventArgs e)
        {
            Label target = sender as Label;

            string search = target.Tag.ToString();

            Keyboard.ClearFocus();

            foreach ( var systemName in starSystemCollection.Where(p => p.Value.name == search))
            {

                findTargetSystem(systemName.Key);

            }
        }

        private void SearchControlSetup(int col,int row,string systemName)
        {
            Label label = new Label();

            label.Tag = systemName;

            string splitName=String.Empty;

            if (systemName.Contains(" "))
            {

                splitName = "";

                string[] nameArray = systemName.Split(' ');

                for (int i = 0; i < nameArray.Length; i++)
                {
                    if (nameArray[i].Length > 5 && i>0)
                    {
                        nameArray[i] = "\n" + nameArray[i];
                    }

                    splitName += nameArray[i] + " ";

                    if (splitName.Length > 0)
                    {
                        systemName = splitName;
                    }
                }
            }
            

            label.FontFamily = new FontFamily(new Uri("pack://application:,,,/Dataset/"), "./#Euro Caps");
            label.FontSize = 13;
            label.Content = systemName;
            label.HorizontalContentAlignment = HorizontalAlignment.Left;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            
            label.Foreground = new SolidColorBrush(Color.FromArgb(0xF0, 0x3D, 0x95, 0xDE));
            label.BorderBrush = new SolidColorBrush(Color.FromArgb(0xF0, 0x3D, 0x95, 0xDE));
            label.BorderThickness = new Thickness(1,1,1,1);

            if (systemName.Length > 0)
            {
                label.MouseLeftButtonDown += SearchClick;
            }

            selectGrid.Children.Add(label);

            Grid.SetRow(label, row);
            Grid.SetColumn(label, col);

        }

        private void RunSearch(string search)
        {
            if (search != oldSearch)
            {
                int count = 0;

                List<string> systemNames = new List<string>();

                selectGrid.Children.Clear();

                foreach (
                    var system in
                        starSystemCollection.Where(p => p.Value.name.ToLower().IndexOf(search.ToLower()) != -1)
                            .OrderBy(p => p.Value.name))
                {
                    systemNames.Add(system.Value.name);

                    count += 1;

                    if (count > 24) break;

                }

                int col = 0;
                int row = 0;

                for (int i = 0; i < systemNames.Count; i++)
                {
                    string name = systemNames[i];

                    SearchControlSetup(col, row, name);

                    col += 1;

                    if (col > 4)
                    {
                        row += 1;
                        col = 0;
                    }

                }
            }

            oldSearch = search;
        }

        private void EnterSearch(object sender, KeyboardFocusChangedEventArgs e)
        {
            inputBox.Text=String.Empty;
            
        }

        private void CaptureInput(object sender, KeyEventArgs e)
        {

              if(inputBox.Text!=String.Empty)  RunSearch(inputBox.Text);
           
        }

        private void noFocus(object sender, RoutedEventArgs e)
        {
            inputBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xF0, 0x3D, 0x95, 0xDE));
        }

        private void NoFocus(object sender, MouseEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }

    public class Star
    {

        private SolidColorBrush brush = new SolidColorBrush();

        private SolidColorBrush foregroundBrush = new SolidColorBrush(Color.FromArgb(0xF0, 0x3D, 0x95, 0xDE));
        private SolidColorBrush backgroundBrush = new SolidColorBrush(Color.FromArgb(0x40, 0x3D, 0x95, 0xDE));

        private string splitName="";

        public Star()
        {
            starGFX = new Ellipse();
            starSEL = new Ellipse();

            starCanvas = new Canvas();

            starLabel = new Label();

            starCanvas.Children.Add(starGFX);

            starCanvas.Children.Add(starLabel);

            starCanvas.Children.Add(starSEL);

            starGFX.Fill = backgroundBrush;

            starLabel.FontFamily = new FontFamily(new Uri("pack://application:,,,/Dataset/"), "./#Euro Caps");
            starLabel.Foreground = backgroundBrush;

            renderDepth = 0;

            //  starCanvas.MouseLeftButtonDown += insert Method here;
        }

        public void SetEllipseSize()
        {

            size = 3.5 + ((7.5 * (80 / maxDistance)) * Math.Abs(renderDepth));
         
            if (projectorZ / centerX <= 0)

            {
               size = 3.5 + ((7.5 * (80 / maxDistance)) * Math.Abs(renderDepth));
           
            }

            starGFX.Width = size;
            starGFX.Height = size;

            if (distance == 0)
            {
                starGFX.Width = 24;
                starGFX.Height = 24;

                starSEL.Width = starGFX.Width*2;
                starSEL.Height = starGFX.Height*2;
                starSEL.StrokeThickness = 4;
            }
        }

        public void setColor(double hg)
        {

            if (renderDepth>=0)
            {
              starGFX.Fill = foregroundBrush;
              starLabel.Foreground = foregroundBrush;

                rendered = true;
            }

            if ((!rendered || renderDepth < 0) && distance!=0)
            {
              
                starGFX.Fill = backgroundBrush;
                starLabel.Foreground = backgroundBrush;
            }

            if (distance == 0)
            {
                starGFX.Fill = new SolidColorBrush(Colors.AliceBlue); ;
                starSEL.Stroke = new SolidColorBrush(Colors.AliceBlue); ;
                starLabel.Foreground = new SolidColorBrush(Colors.AliceBlue); ;
            }

           
        }

        public void SetName(string starName)
        {
            name = starName;

            starLabel.Content = starName;

            if (starName.Contains(" "))
            {
                splitName = "";

                string[] nameArray = starName.Split(' ');

                if (nameArray.Length > 2)
                {

                    for (int i = 0; i < nameArray.Length; i++)
                    {
                        splitName += nameArray[i] + " ";

                        if (i == 1) splitName += "\n";
                    }

                    if (splitName.Length > 0)
                    {
                        starLabel.Content = splitName;
                    }
                }
            }
  
        }

        public void SetLabel()
        {
            starLabel.Visibility = Visibility.Hidden;

            if (size >= 18 && renderDepth > 0 && distance !=0) 
            {
                starLabel.Visibility = Visibility.Visible;

                starLabel.FontSize = 3.5 + ((5*(80/maxDistance))*Math.Abs(renderDepth));

                starLabel.Measure(new Size(500,500));

                starLabel.Width = 0;
                starLabel.Height = 0;

                starLabel.Width = starLabel.DesiredSize.Width*2;
                starLabel.Height = starLabel.DesiredSize.Height;

                starLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                starLabel.VerticalContentAlignment = VerticalAlignment.Top;

                starLabel.Margin = new Thickness(size - starLabel.Width / 2, size / 2, 0, 0);

            }

            if (distance == 0)
            {
                starLabel.Visibility = Visibility.Visible;

                starLabel.FontSize = 28;

                starLabel.Foreground= new SolidColorBrush(Colors.AliceBlue);

                starLabel.Width = 750;
                starLabel.Height = 250;

                starLabel.HorizontalContentAlignment = HorizontalAlignment.Left;
                starLabel.VerticalContentAlignment = VerticalAlignment.Top;

                starLabel.Margin = new Thickness(30, -20, 0, 0);

                if (splitName.Length > 0) starLabel.Margin = new Thickness(30,-40, 0, 0);

            }
        }

        private Point Calc3D()
        {
            double x = X;
            double y = Y;
            double z = Z;

            double rx = rotaX;
            double ry = rotaY;
            double rz = rotaZ;

            double e = -elevation;
            double a = azimuth;

            Point3D ProjectedPoint = new Point3D();

            DrawCube Projector = new DrawCube();

            ProjectedPoint = Projector.AnimateStar(x, y, z, rx, ry, rz, e, a);

            projectorZ = ProjectedPoint.Z;

            renderDepth = projectorZ/centerX;

            return new Point(ProjectedPoint.X,ProjectedPoint.Y);
        }
    
        public Canvas Draw3D()
        {
            Point projectionPoint = Calc3D();

            projectorX = projectionPoint.X;
            projectorY = projectionPoint.Y;

            starGFX.Margin = new Thickness(-(starGFX.Width/2), -(starGFX.Height/2), 0, 0);
            starSEL.Margin = new Thickness(-(starGFX.Width*2) / 2, -(starGFX.Height*2) / 2, 0, 0);

            starCanvas.Margin=new Thickness(centerX + projectorX, centerY + projectorY,0,0);

            return starCanvas;
        }

        public int starID { get; set; }
        public string name { get; set; }
        public Ellipse starGFX { get; set; }
        public Ellipse starSEL { get; set; }

       
        public Label starLabel { get; set; }
        public Canvas starCanvas { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double originX { get; set; }
        public double originY { get; set; }
        public double originZ { get; set; }

        public double projectorX { get; set; }
        public double projectorY { get; set; }
        public double projectorZ { get; set; }

        public double rotaX { get; set; }
        public double rotaY { get; set; }
        public double rotaZ { get; set; }

        public double azimuth { get; set; }
        public double elevation { get; set; }

        public double distance { get; set; }
        public double maxDistance { get; set; }
       
        public bool rendered { get; set; }
        public double renderDepth { get; set;}
        public double centerX { get; set; }
        public double centerY { get; set; }

        public double size { get; set; }

    }

    class StarSystem
    {
        public int id { get; set; }

        public string name { get; set; }

        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public string faction { get; set; }
        public int population { get; set; }

        public string government { get; set; }
        public string allegiance { get; set; }

        public string state { get; set; }
        public string security { get; set; }

        public string primary_economy { get; set; }

        public string power { get; set; }
        public string power_state { get; set; }

        public string simbad_ref { get; set; }

        public Star star { get; set; }

    }

    public class Station
    {

        public int id { get; set; }
        public string name { get; set; }
        public int system_id { get; set; }
        public string max_landing_pad_size { get; set; }
        public int distance_to_star { get; set; }
        public string faction { get; set; }
        public string government { get; set; }
        public string state { get; set; }
        public int type_id { get; set; }
        public string type { get; set; }
        public int has_blackmarket { get; set; }
        public int has_market { get; set; }
        public int has_refuel { get; set; }
        public int has_repair { get; set; }
        public int has_rearm { get; set; }
        public int has_outfitting { get; set; }
        public int has_shipyard { get; set; }
        public int has_commodity { get; set; }
        public List<string> import_commodities { get; set; }
        public List<string> export_commodities { get; set; }
        public List<string> prohibited_commodities { get; set; }
        public List<string> economies { get; set; }
        public int updated_at { get; set; }
        public int shipyard_updated_at { get; set; }
        public int outfitting_updated_at { get; set; }
        public int market_updated_at { get; set; }
        public int is_planetary { get; set; }
        public List<string> selling_ships { get; set; }
        public List<int> seling_modules { get; set; }

    }

    public class Commodities
    {
        public int commodity_id { get; set; }
        public string commodity_name { get; set; }
        public int category_id { get; set; }
        public int averageprice { get; set; }
        public List<string> category_name { get; set; }
    }


}
