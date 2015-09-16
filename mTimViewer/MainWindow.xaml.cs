using HelixToolkit.Wpf;
using Microsoft.Win32;
using mTIM.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mTimViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.FileOk += (_, __) =>
            {
                var file = ofd.FileName;
                Result r;
                using (var fs = File.OpenRead(file))
                    r = ProtoBuf.Serializer.Deserialize<Result>(fs);

                _model.Children.Clear();

                var material = MaterialHelper.CreateMaterial(Colors.Red, 1);

                foreach (var ele in r.Elements)
                {
                    foreach (var geoRef in ele.Geometries)
                    {
                        var geo = r.Geometries[geoRef.GeometryIndex];
                        var triangulated = geo as TriangulatedGeometryData;
                        if (triangulated != null)
                        {
                            var builder = new MeshBuilder(true, false);
                            foreach (var t in triangulated.Triangles)
                            {
                                var a = r.PointsAndVectors[t.A].ToWpfPoint();
                                var b = r.PointsAndVectors[t.B].ToWpfPoint();
                                var c = r.PointsAndVectors[t.C].ToWpfPoint();
                                builder.AddTriangle(a, c, b);
                            }
                            var mesh = builder.ToMesh(true);
                            var geoModel = new GeometryModel3D { Geometry = mesh, Material = material };

                            Transform3D matrix = null;
                            if (geoRef.Transform != null)
                            {
                                matrix = new MatrixTransform3D(MatrixHelper.FromArray(geoRef.Transform.Matrix));
                                geoModel.Transform = matrix;
                            }
                            var modelVisual = new ModelVisual3D { Content = geoModel };

                            if (_lines.IsChecked == true)
                            {
                                foreach (var l in triangulated.Lines)
                                {
                                    var a = r.PointsAndVectors[l.A].ToWpfPoint();
                                    var b = r.PointsAndVectors[l.B].ToWpfPoint();
                                    var lineGeo = new LinesVisual3D { Color = Colors.Green, Points = new[] { a, b } };
                                    if (matrix != null)
                                        lineGeo.Transform = matrix;

                                    modelVisual.Children.Add(lineGeo);
                                }
                            }

                            _model.Children.Add(modelVisual);
                        }
                        else
                        {
                            throw new Exception("unknown geo data: " + geo.GetType());
                        }
                    }
                }
                _viewPort.ZoomExtents(500);
            };
            ofd.ShowDialog();
        }
    }

    public static class PointHelper
    {
        public static Point3D ToWpfPoint(this XYZ self)
        {
            return new Point3D { X = self.X, Y = self.Y, Z = self.Z };
        }
    }

    public static class MatrixHelper
    {
        public static Matrix3D FromArray(float[] matrix)
        {
            // TIM saves the Matrix in Column-Major
            // WPF (DirectX) uses Row-Major
            return new Matrix3D(matrix[0], matrix[4], matrix[8], matrix[12], matrix[1], matrix[5], matrix[9], matrix[13], matrix[2], matrix[6], matrix[10], matrix[14], matrix[3], matrix[7], matrix[11], matrix[15]);
        }
    }
}
