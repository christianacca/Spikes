using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Eca.Spikes.WinFormsApplication.Forms
{
    public partial class DrawingExamples : Form
    {
        private const string ExampleText = "Hello World. This is going to be too large for the form to display in full";
        private const string GraphicsFileName = "MyCoolGraphics.gif";
        private const string SampleImageFileName = "SampleImage.jpg";
        private bool _diagramDrawn;
        private bool _textDrawn;


        public DrawingExamples()
        {
            InitializeComponent();
        }


        private Pen FancyPen
        {
            get
            {
                //should be making this a field and disposing of it in the Dispose method of this form
                Pen fancyPen = new Pen(new HatchBrush(HatchStyle.Cross, Color.SpringGreen, Color.Black), 10);
                fancyPen.StartCap = LineCap.Triangle;
                fancyPen.EndCap = LineCap.Round;
                return fancyPen;
            }
        }


        private void DrawingExamples_Paint(object sender, PaintEventArgs e)
        {
            if (_diagramDrawn)
                DrawFormGraphics();
            else if (_textDrawn)
                DrawTextOntoForm();
        }


        private void drawGraphicsButton_Click(object sender, EventArgs e)
        {
            ResetFormForDrawing();
            DrawFormGraphics();
        }


        private void drawPictureButton_Click(object sender, EventArgs e)
        {
            ResetFormForDrawing();
            ShowImageInPictureBox();
        }


        private void drawImageOnFormButton_Click(object sender, EventArgs e)
        {
            ResetFormForDrawing();
            DrawImageOnForm();
        }


        private void drawModifiedIcon_Click(object sender, EventArgs e)
        {
            ResetFormForDrawing();
            DrawModifiedIconOnForm();
        }


        private void drawTextButton_Click(object sender, EventArgs e)
        {
            ResetFormForDrawing();
            DrawTextOntoForm();
        }


        private void saveGraphicsButton_Click(object sender, EventArgs e)
        {
            using (Bitmap bitmap = CreateGraphicsImage())
            {
                bitmap.Save(GraphicsFileName, ImageFormat.Gif);
            }
        }


        private void updateGraphicsButton_Click(object sender, EventArgs e)
        {
            using (Bitmap bitmap = new Bitmap(GraphicsFileName))
            {
                DrawDiagonolLineAcross(bitmap, FancyPen);
                bitmap.Save(GraphicsFileName, ImageFormat.Gif);
            }
        }


        private Bitmap CreateGraphicsImage()
        {
            Rectangle drawingArea = OuterElipseCoordinates();
            Bitmap bitmap = new Bitmap(drawingArea.Size.Width + 100, drawingArea.Size.Height + 100);
            using (Graphics drawingSurface = Graphics.FromImage(bitmap))
            {
                drawingSurface.Clear(Color.White);
                DrawNestedElipses(drawingSurface, drawingArea);
            }
            return bitmap;
        }


        private void DrawFormGraphics()
        {
            Graphics drawingSurface = CreateGraphics();
            DrawNestedElipses(drawingSurface, OuterElipseCoordinates());
            DrawPolygone(drawingSurface);
            drawingSurface.SmoothingMode = SmoothingMode.HighQuality;
            _diagramDrawn = true;
        }


        private void ResetFormForDrawing()
        {
            _diagramDrawn = false;
            _textDrawn = false;
            pictureBox.Visible = false;
            CreateGraphics().Clear(BackColor);
        }


        private Rectangle OuterElipseCoordinates()
        {
            Rectangle outerElipse = Clone(ClientRectangle);
            outerElipse.Inflate(-50, -50);
            return outerElipse;
        }


        private void DrawNestedElipses(Graphics drawingSurface, Rectangle drawingArea)
        {
            DrawHoritontallyCrossedElipse(drawingArea, drawingSurface);
            DrawInnerElipse(drawingArea, drawingSurface);
        }


        private void DrawHoritontallyCrossedElipse(Rectangle drawingArea, Graphics drawingSurface)
        {
            drawingSurface.DrawRectangle(Pens.Azure, drawingArea);
            drawingSurface.DrawEllipse(Pens.Red, drawingArea);
            int middleOfElipse = drawingArea.Location.Y + drawingArea.Height/2;
            Point middleLeftOfElipse = new Point(drawingArea.Location.X, middleOfElipse);
            Point middleRightOfElipse =
                new Point(drawingArea.Location.X + drawingArea.Size.Width, middleOfElipse);
            drawingSurface.DrawLine(Pens.BlueViolet, middleLeftOfElipse, middleRightOfElipse);
        }


        private void DrawInnerElipse(Rectangle outerElipseDrawingArea, Graphics drawingSurface)
        {
            Rectangle innerRectangle = Clone(outerElipseDrawingArea);
            innerRectangle.Inflate(-50, -50);
            using (HatchBrush hatchedBrush = new HatchBrush(HatchStyle.DarkHorizontal, Color.DeepPink, Color.Aqua))
            {
                drawingSurface.FillEllipse(hatchedBrush, innerRectangle);
            }
        }


        private void DrawPolygone(Graphics surface)
        {
            Point[] polygoneCoordinates =
                {
                    new Point(10, 10),
                    new Point(10, 100),
                    new Point(50, 65),
                    new Point(100, 100),
                    new Point(85, 40)
                };
            using (
                LinearGradientBrush professionalBrush =
                    new LinearGradientBrush(new Point(1, 1), new Point(100, 100), Color.Red, Color.DarkViolet))
            {
                surface.FillPolygon(professionalBrush, polygoneCoordinates, FillMode.Winding);
            }
        }


        private Rectangle Clone(Rectangle original)
        {
            return new Rectangle(original.Location, original.Size);
        }


        private void ShowImageInPictureBox()
        {
            Image image = Image.FromFile(SampleImageFileName);
            pictureBox.BackgroundImage = image;
            pictureBox.Visible = true;
        }


        private void DrawImageOnForm()
        {
            Graphics drawingSurface = CreateGraphics();
            drawingSurface.DrawImage(Image.FromFile(SampleImageFileName),
                                     1,
                                     1,
                                     ClientRectangle.Size.Width,
                                     ClientRectangle.Size.Height - 30);
        }


        private void DrawDiagonolLineAcross(Image image, Pen pen)
        {
            //not sure why but this is failing with an exception
            using (Graphics drawingSurface = Graphics.FromImage(image))
            {
                drawingSurface.DrawLine(pen, 1, 1, image.Width, image.Height);
            }
        }


        private void DrawTextOntoForm()
        {
            Graphics drawingSurface = CreateGraphics();
            Font font = new Font("Ariel", 30, FontStyle.Bold);
            Rectangle drawingArea = Clone(ClientRectangle);
            drawingArea.Inflate(-50, -200);
            StringFormat format = new StringFormat();
            format.Alignment =StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.FormatFlags =  StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
            format.Trimming = StringTrimming.EllipsisWord;
            drawingSurface.DrawString(ExampleText, font, Brushes.BlueViolet, drawingArea, format);
            _textDrawn = true;
        }


        private void DrawModifiedIconOnForm()
        {
            Icon hand = SystemIcons.Hand;
            using (Bitmap bitmap = hand.ToBitmap())
            {
                DrawDiagonolLineAcross(bitmap, FancyPen);
                CreateGraphics().DrawImage(bitmap, 10, 10);
            }
        }
    }
}