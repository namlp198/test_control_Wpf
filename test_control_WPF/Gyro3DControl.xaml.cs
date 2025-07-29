using System;
using System.Collections.Generic;
using System.Linq;
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

namespace test_control_WPF
{
    //public partial class Gyro3DControl : UserControl
    //{
    //    private AxisAngleRotation3D yawRot, pitchRot, rollRot;
    //    private TextBlock yawTextBlock, pitchTextBlock, rollTextBlock;
    //    private Viewport2DVisual3D yawLabel3D, pitchLabel3D, rollLabel3D;
    //    private MatrixTransform3D yawMatrix, pitchMatrix, rollMatrix;

    //    public Gyro3DControl()
    //    {
    //        InitializeComponent();
    //        SetupScene();
    //        CompositionTarget.Rendering += OnRendering;
    //    }

    //    private void SetupScene()
    //    {
    //        viewport.Children.Clear();

    //        // Camera
    //        viewport.Camera = new PerspectiveCamera
    //        {
    //            Position = new Point3D(3, 3, 5),
    //            LookDirection = new Vector3D(-3, -3, -5),
    //            UpDirection = new Vector3D(0, 1, 0),
    //            FieldOfView = 60
    //        };

    //        // Light
    //        var light = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -2));
    //        viewport.Children.Add(new ModelVisual3D { Content = light });

    //        // Cube (robot)
    //        var robotGeo = new GeometryModel3D
    //        {
    //            Geometry = MeshGeometryHelper.CreateCube(1.0),
    //            Material = new DiffuseMaterial(Brushes.Red),
    //            BackMaterial = new DiffuseMaterial(Brushes.Red)
    //        };

    //        yawRot = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
    //        pitchRot = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
    //        rollRot = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);

    //        var transformGroup = new Transform3DGroup();
    //        transformGroup.Children.Add(new RotateTransform3D(rollRot));
    //        transformGroup.Children.Add(new RotateTransform3D(pitchRot));
    //        transformGroup.Children.Add(new RotateTransform3D(yawRot));
    //        robotGeo.Transform = transformGroup;

    //        viewport.Children.Add(new ModelVisual3D { Content = robotGeo });

    //        // Rings
    //        AddRing("XZ", Brushes.Red);     // Pitch
    //        AddRing("YZ", Brushes.Green);   // Roll
    //        AddRing("XY", Brushes.Blue);    // Yaw

    //        // Axis Arrows
    //        AddAxisArrows();

    //        viewport.Children.Add(CreateText3D(ref yawTextBlock, ref yawLabel3D, ref yawMatrix, new Point3D(0, 1.7, 0), Brushes.Blue));

    //        viewport.Children.Add(CreateText3D(ref pitchTextBlock, ref pitchLabel3D, ref pitchMatrix, new Point3D(1.7, 0, 0), Brushes.Red));

    //        viewport.Children.Add(CreateText3D(ref rollTextBlock, ref rollLabel3D, ref rollMatrix, new Point3D(0, 0, 1.7), Brushes.Green));
    //    }

    //    private void AddRing(string plane, Brush color)
    //    {
    //        var mesh = RingMeshFactory.CreateRing(1.5, 0.05, 64, plane);
    //        var model = new GeometryModel3D
    //        {
    //            Geometry = mesh,
    //            Material = new DiffuseMaterial(color),
    //            BackMaterial = new DiffuseMaterial(color)
    //        };
    //        viewport.Children.Add(new ModelVisual3D { Content = model });
    //    }

    //    private void AddAxisArrows()
    //    {
    //        double length = 2.5;
    //        viewport.Children.Add(CreateArrowModel(new Vector3D(length, 0, 0), Brushes.Red));    // X
    //        viewport.Children.Add(CreateArrowModel(new Vector3D(0, length, 0), Brushes.Green));  // Y
    //        viewport.Children.Add(CreateArrowModel(new Vector3D(0, 0, length), Brushes.Blue));   // Z
    //    }

    //    private ModelVisual3D CreateArrowModel(Vector3D dir, Brush color)
    //    {
    //        var mesh = MeshGeometryHelper.CreateCube(1);
    //        var scale = new ScaleTransform3D(0.05, 0.05, dir.Length);
    //        var angle = Vector3D.AngleBetween(new Vector3D(0, 0, 1), dir);
    //        var axis = Vector3D.CrossProduct(new Vector3D(0, 0, 1), dir);
    //        var rotate = new RotateTransform3D(new AxisAngleRotation3D(axis, angle));

    //        var transform = new Transform3DGroup();
    //        transform.Children.Add(scale);
    //        transform.Children.Add(rotate);

    //        var model = new GeometryModel3D
    //        {
    //            Geometry = mesh,
    //            Material = new DiffuseMaterial(color),
    //            BackMaterial = new DiffuseMaterial(color),
    //            Transform = transform
    //        };

    //        return new ModelVisual3D { Content = model };
    //    }

    //    private Viewport2DVisual3D CreateText3D(ref TextBlock textRef, ref Viewport2DVisual3D visualRef, ref MatrixTransform3D matrixRef, Point3D position, Brush color)
    //    {
    //        var text = new TextBlock
    //        {
    //            Text = "0°",
    //            Foreground = color,
    //            Background = Brushes.Transparent,
    //            FontWeight = FontWeights.Bold,
    //            FontSize = 14,
    //            HorizontalAlignment = HorizontalAlignment.Center,
    //            VerticalAlignment = VerticalAlignment.Center,
    //            IsHitTestVisible = false
    //        };
    //        textRef = text;

    //        var mesh = new MeshGeometry3D
    //        {
    //            Positions = new Point3DCollection
    //    {
    //        new Point3D(-0.25, -0.1, 0),
    //        new Point3D( 0.25, -0.1, 0),
    //        new Point3D( 0.25,  0.1, 0),
    //        new Point3D(-0.25,  0.1, 0)
    //    },
    //            TextureCoordinates = new PointCollection
    //    {
    //        new Point(0, 1), new Point(1, 1),
    //        new Point(1, 0), new Point(0, 0)
    //    },
    //            TriangleIndices = new Int32Collection { 0, 1, 2, 2, 3, 0 }
    //        };

    //        var material = new DiffuseMaterial(Brushes.Transparent);
    //        Viewport2DVisual3D.SetIsVisualHostMaterial(material, true);

    //        matrixRef = new MatrixTransform3D();
    //        var transformGroup = new Transform3DGroup();
    //        transformGroup.Children.Add(matrixRef); // để cập nhật theo camera mỗi frame
    //        transformGroup.Children.Add(new TranslateTransform3D(position.X, position.Y, position.Z));

    //        var visual3D = new Viewport2DVisual3D
    //        {
    //            Geometry = mesh,
    //            Material = material,
    //            Visual = text,
    //            Transform = transformGroup
    //        };

    //        visualRef = visual3D;
    //        return visual3D;
    //    }


    //    private void OnRendering(object sender, EventArgs e)
    //    {
    //        var cam = viewport.Camera as ProjectionCamera;
    //        if (cam == null) return;

    //        Vector3D lookDir = cam.LookDirection;
    //        lookDir.Normalize();
    //        Vector3D up = cam.UpDirection;
    //        up.Normalize();
    //        Vector3D right = Vector3D.CrossProduct(lookDir, up);

    //        var matrix = new Matrix3D(
    //            right.X, up.X, lookDir.X, 0,
    //            right.Y, up.Y, lookDir.Y, 0,
    //            right.Z, up.Z, lookDir.Z, 0,
    //            0, 0, 0, 1
    //        );

    //        yawMatrix.Matrix = matrix;
    //        pitchMatrix.Matrix = matrix;
    //        rollMatrix.Matrix = matrix;
    //    }

    //    private void ApplyBillboard(Viewport2DVisual3D label3D, Matrix3D matrix)
    //    {
    //        if (label3D?.Transform is Transform3DGroup group &&
    //            group.Children[0] is MatrixTransform3D matTrans)
    //        {
    //            matTrans.Matrix = matrix;
    //        }
    //    }

    //    // Properties to bind
    //    public double Yaw
    //    {
    //        get => yawRot.Angle;
    //        set
    //        {
    //            yawRot.Angle = value;
    //            if (yawTextBlock != null)
    //                yawTextBlock.Text = $"Yaw: {value:F0}°";
    //        }
    //    }

    //    public double Pitch
    //    {
    //        get => pitchRot.Angle;
    //        set
    //        {
    //            pitchRot.Angle = value;
    //            if (pitchTextBlock != null)
    //                pitchTextBlock.Text = $"Pitch: {value:F0}°";
    //        }
    //    }

    //    public double Roll
    //    {
    //        get => rollRot.Angle;
    //        set
    //        {
    //            rollRot.Angle = value;
    //            if (rollTextBlock != null)
    //                rollTextBlock.Text = $"Roll: {value:F0}°";
    //        }
    //    }
    //}

    public partial class Gyro3DControl : UserControl
    {
        // rotation angles
        private AxisAngleRotation3D yawRot, pitchRot, rollRot;

        private TextBlock yawTextBlock, pitchTextBlock, rollTextBlock;

        private double _thicknessRing = 0.1;

        // model container
        private ModelVisual3D robotModel;
        private ModelVisual3D pitchRing;
        private ModelVisual3D rollRing;
        private ModelVisual3D yawRing;

        public Gyro3DControl()
        {
            InitializeComponent();
            SetupScene();
        }

        private void SetupScene()
        {
            // 1. Clear viewport nếu có sẵn
            viewport.Children.Clear();

            // 2. Camera (nếu chưa có trong XAML)
            if (viewport.Camera == null)
            {
                viewport.Camera = new PerspectiveCamera
                {
                    Position = new Point3D(0, 0, 6),
                    LookDirection = new Vector3D(0, 0, -6),
                    UpDirection = new Vector3D(0, 1, 0),
                    FieldOfView = 60
                };
            }

            // 3. Ánh sáng
            var light = new DirectionalLight(Colors.White, new Vector3D(-1, -1, -2));
            viewport.Children.Add(new ModelVisual3D { Content = light });

            // 4. Robot trung tâm (cube)
            var robotGeo = new GeometryModel3D
            {
                Geometry = MeshGeometryHelper.CreateCube(1.0),
                Material = new DiffuseMaterial(Brushes.Red),
                BackMaterial = new DiffuseMaterial(Brushes.Red)
            };

            // 4. Robot trung tâm (ảnh robot)
            /*string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "robot.png");

            if (!System.IO.File.Exists(imagePath))
            {
                MessageBox.Show("Không tìm thấy ảnh robot tại: " + imagePath, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute))
            };

            var plane = new MeshGeometry3D
            {
                Positions = new Point3DCollection
    {
        new Point3D(-0.75, -0.5, 0),
        new Point3D( 0.75, -0.5, 0),
        new Point3D( 0.75,  0.5, 0),
        new Point3D(-0.75,  0.5, 0)
    },
                TextureCoordinates = new PointCollection
    {
        new Point(0, 1), new Point(1, 1),
        new Point(1, 0), new Point(0, 0)
    },
                TriangleIndices = new Int32Collection { 0, 1, 2, 2, 3, 0 }
            };

            var robotGeo = new GeometryModel3D
            {
                Geometry = plane,
                Material = new DiffuseMaterial(imageBrush),
                BackMaterial = new DiffuseMaterial(imageBrush)
            };*/

            /*(var robotMesh = RobotExtrudedMeshFactory.CreateExtruded(0.2);
            var robotGeo = new GeometryModel3D
            {
                Geometry = robotMesh,
                Material = new DiffuseMaterial(Brushes.Red),
                BackMaterial = new DiffuseMaterial(Brushes.Red)
            };*/

            // rotation transforms
            yawRot = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);   // Yaw around Y
            pitchRot = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0); // Pitch around X
            rollRot = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);  // Roll around Z

            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new RotateTransform3D(rollRot));
            transformGroup.Children.Add(new RotateTransform3D(pitchRot));
            transformGroup.Children.Add(new RotateTransform3D(yawRot));
            robotGeo.Transform = transformGroup;

            robotModel = new ModelVisual3D { Content = robotGeo };
            viewport.Children.Add(robotModel);

            // 5. Vòng Pitch (mặt ZX – quay quanh Y)
            var pitchMesh = RingMeshFactory.CreateRing(1.5, _thicknessRing, 64, "XZ");
            var pitchModel = new GeometryModel3D
            {
                Geometry = pitchMesh,
                Material = new DiffuseMaterial(Brushes.Red),
                BackMaterial = new DiffuseMaterial(Brushes.Red)
            };
            pitchRing = new ModelVisual3D { Content = pitchModel };
            viewport.Children.Add(pitchRing);

            // 6. Vòng Roll (mặt YZ – quay quanh X)
            var rollMesh = RingMeshFactory.CreateRing(1.5, _thicknessRing, 64, "YZ");
            var rollModel = new GeometryModel3D
            {
                Geometry = rollMesh,
                Material = new DiffuseMaterial(Brushes.Green),
                BackMaterial = new DiffuseMaterial(Brushes.Green)
            };
            rollRing = new ModelVisual3D { Content = rollModel };
            viewport.Children.Add(rollRing);

            // 7. Vòng Yaw (mặt XY – quay quanh Z)
            var yawMesh = RingMeshFactory.CreateRing(1.5, _thicknessRing, 64, "XY");
            var yawModel = new GeometryModel3D
            {
                Geometry = yawMesh,
                Material = new DiffuseMaterial(Brushes.Blue),
                BackMaterial = new DiffuseMaterial(Brushes.Blue)
            };
            yawRing = new ModelVisual3D { Content = yawModel };
            viewport.Children.Add(yawRing);

            // Thêm các label hiển thị góc lên không gian 3D
            viewport.Children.Add(CreateText3D(ref yawTextBlock, new Point3D(0, 1.7, 0), Brushes.Blue));
            viewport.Children.Add(CreateText3D(ref pitchTextBlock, new Point3D(1.7, 0, 0), Brushes.Red));
            viewport.Children.Add(CreateText3D(ref rollTextBlock, new Point3D(0, 0, 1.7), Brushes.Green));

            AddAxisArrows();
        }

        private Viewport2DVisual3D CreateText3D(ref TextBlock textRef, Point3D position, Brush color)
        {
            var text = new TextBlock
            {
                Text = "0°",
                Foreground = color,
                Background = Brushes.Transparent,
                FontWeight = FontWeights.Bold,
                FontSize = 26,
                IsHitTestVisible = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            textRef = text;

            var mesh = new MeshGeometry3D
            {
                Positions = new Point3DCollection
        {
            new Point3D(-0.25, -0.1, 0),
            new Point3D( 0.25, -0.1, 0),
            new Point3D( 0.25,  0.1, 0),
            new Point3D(-0.25,  0.1, 0)
        },
                TextureCoordinates = new PointCollection
        {
            new Point(0, 1),
            new Point(1, 1),
            new Point(1, 0),
            new Point(0, 0)
        },
                TriangleIndices = new Int32Collection { 0, 1, 2, 2, 3, 0 }
            };

            var material = new DiffuseMaterial(Brushes.Transparent);
            Viewport2DVisual3D.SetIsVisualHostMaterial(material, true);

            var visual3D = new Viewport2DVisual3D
            {
                Geometry = mesh,
                Material = material,
                Visual = text,
                Transform = new TranslateTransform3D(position.X, position.Y, position.Z)
            };

            return visual3D;
        }
        private void AddAxisArrows()
        {
            double length = 2.2;
            double thickness = 0.1;

            viewport.Children.Add(CreateArrowModel(new Vector3D(length, 0, 0), Brushes.Red));    // X
            viewport.Children.Add(CreateArrowModel(new Vector3D(0, length, 0), Brushes.Green));  // Y
            viewport.Children.Add(CreateArrowModel(new Vector3D(0, 0, length), Brushes.Blue));   // Z
        }

        private ModelVisual3D CreateArrowModel(Vector3D dir, Brush color)
        {
            var mesh = MeshGeometryHelper.CreateCube(1); // có thể đổi sang cylinder
            var scale = new ScaleTransform3D(0.05, 0.05, dir.Length);
            var rotate = new RotateTransform3D(new AxisAngleRotation3D(
                Vector3D.CrossProduct(new Vector3D(0, 0, 1), dir), Vector3D.AngleBetween(new Vector3D(0, 0, 1), dir)));

            var transform = new Transform3DGroup();
            transform.Children.Add(scale);
            transform.Children.Add(rotate);

            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = new DiffuseMaterial(color),
                BackMaterial = new DiffuseMaterial(color),
                Transform = transform
            };

            return new ModelVisual3D { Content = model };
        }


        public double Yaw
        {
            get => yawRot.Angle;
            set
            {
                yawRot.Angle = value;
                if (yawTextBlock != null)
                    yawTextBlock.Text = $"{value:F0}°";
            }
        }

        public double Pitch
        {
            get => pitchRot.Angle;
            set
            {
                pitchRot.Angle = value;
                if (pitchTextBlock != null)
                    pitchTextBlock.Text = $"{value:F0}°";
            }
        }

        public double Roll
        {
            get => rollRot.Angle;
            set
            {
                rollRot.Angle = value;
                if (rollTextBlock != null)
                    rollTextBlock.Text = $"{value:F0}°";
            }
        }

    }



    public static class RingMeshFactory
    {
        public static MeshGeometry3D CreateRing(
            double radius = 1.0,
            double thickness = 0.05,
            int segments = 64,
            string plane = "XY")
        {
            var mesh = new MeshGeometry3D();

            for (int i = 0; i <= segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double cos = Math.Cos(angle);
                double sin = Math.Sin(angle);

                Point3D outer, inner;

                switch (plane.ToUpper())
                {
                    case "XY":
                        outer = new Point3D(cos * radius, sin * radius, 0);
                        inner = new Point3D(cos * (radius - thickness), sin * (radius - thickness), 0);
                        break;
                    case "YZ":
                        outer = new Point3D(0, cos * radius, sin * radius);
                        inner = new Point3D(0, cos * (radius - thickness), sin * (radius - thickness));
                        break;
                    case "XZ":
                        outer = new Point3D(cos * radius, 0, sin * radius);
                        inner = new Point3D(cos * (radius - thickness), 0, sin * (radius - thickness));
                        break;
                    default:
                        throw new ArgumentException("Invalid plane. Use XY, YZ, or XZ.");
                }

                mesh.Positions.Add(outer);
                mesh.Positions.Add(inner);

                if (i > 0)
                {
                    int baseIndex = i * 2;
                    mesh.TriangleIndices.Add(baseIndex - 2);
                    mesh.TriangleIndices.Add(baseIndex - 1);
                    mesh.TriangleIndices.Add(baseIndex);

                    mesh.TriangleIndices.Add(baseIndex);
                    mesh.TriangleIndices.Add(baseIndex - 1);
                    mesh.TriangleIndices.Add(baseIndex + 1);
                }
            }

            return mesh;
        }
    }


    public static class MeshGeometryHelper
    {
        public static MeshGeometry3D CreateCube(double size = 1.0)
        {
            double h = size / 2.0;

            var positions = new Point3DCollection
        {
            new Point3D(-h, -h, -h), // 0
            new Point3D( h, -h, -h), // 1
            new Point3D( h,  h, -h), // 2
            new Point3D(-h,  h, -h), // 3
            new Point3D(-h, -h,  h), // 4
            new Point3D( h, -h,  h), // 5
            new Point3D( h,  h,  h), // 6
            new Point3D(-h,  h,  h), // 7
        };

            var triangleIndices = new Int32Collection
        {
            // Front face
            0, 1, 2,  2, 3, 0,
            // Right face
            1, 5, 6,  6, 2, 1,
            // Back face
            5, 4, 7,  7, 6, 5,
            // Left face
            4, 0, 3,  3, 7, 4,
            // Top face
            3, 2, 6,  6, 7, 3,
            // Bottom face
            4, 5, 1,  1, 0, 4
        };

            return new MeshGeometry3D
            {
                Positions = positions,
                TriangleIndices = triangleIndices
            };
        }
    }

}
