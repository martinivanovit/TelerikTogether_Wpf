using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TelerikTogether.Math;
using TelerikTogether.Sunburst;

namespace WPFSampleApp
{
	public class Sunburst : Control
    {
		private const string PART_DrawingSurfaceName = "PART_DrawingSurface";
		private Random random = new Random(0);
		private SunburstItemCollection items = new SunburstItemCollection();
		private Canvas drawingSurface;

		public Sunburst()
        {
			for (int i = 1; i <= 5; i++)
			{
				SunburstItem item = new SunburstItem(null, "Item " + i, i * 30);
				item.Children = new SunburstItemCollection();

				for (int j = 1; j <= 3; j++)
				{
					SunburstItem subItem = new SunburstItem(null, "Item " + i + "." + j, j * 24);
					item.Children.Add(subItem);
				}

				items.Add(item);
			}

            SizeChanged += Sunburst_SizeChanged;
		}

        private void Sunburst_SizeChanged(object sender, SizeChangedEventArgs e)
        {
			UpdateGeometry();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
			drawingSurface = GetTemplateChild(PART_DrawingSurfaceName) as Canvas;			
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
			UpdateGeometry();
		}

        private void UpdateGeometry()
		{
            if (drawingSurface == null)
            {
				return;
            }

			drawingSurface.Children.Clear();

			SunburstLayoutEngine engine = new SunburstLayoutEngine();
			SolidColorBrush brush = null;			
			var bounds = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
			var viewInfos = engine.CalculateViewInfos(items);
			
			double innerRadiusStart = 30;
			int levelsCount = 2; //to do: calculate the count instead of using hardcoded value
			double diameter = Math.Min(bounds.Width, bounds.Height);
			SunburstContext context = new SunburstContext();
			context.Diameter = diameter;
			context.LevelSize = ((diameter / 2) - innerRadiusStart) / levelsCount;
			context.LevelsCount = levelsCount;
			context.LayoutSlot = bounds;
			context.MinInnerRadius = innerRadiusStart;


			foreach (SunburstItemViewInfo viewInfo in viewInfos)
			{
				if (viewInfo.Level == 0)
				{
					brush = new SolidColorBrush(Color.FromRgb((byte)this.random.Next(256), (byte)this.random.Next(256), (byte)this.random.Next(256)));
				}
				Path visual = CreateSliceVisual(viewInfo, context, brush);
				drawingSurface.Children.Add(visual);
			}
		}

		private Path CreateSliceVisual(SunburstItemViewInfo viewInfo, SunburstContext context, Brush brush)
        {
			var path = new Path();
			path.Stroke = Brushes.Black;
			path.StrokeThickness = 1;
			path.Fill = brush;

			var geometry = ConstructDonutGraphicsGeometry(viewInfo, context);
			path.Data = geometry;
			return path;
		}

		protected Geometry ConstructDonutGraphicsGeometry(SunburstItemViewInfo viewInfo, SunburstContext context)
		{
			double innerRadius = (viewInfo.Level * context.LevelSize) + context.MinInnerRadius;
            Point center = new Point(context.LayoutSlot.X + context.LayoutSlot.Width / 2f, context.LayoutSlot.Y + context.LayoutSlot.Height / 2f);
			double outerRadius = innerRadius + context.LevelSize;

			ArcSegment outerArc = new ArcSegment();
			outerArc.Size = new Size(outerRadius, outerRadius);
			outerArc.IsLargeArc = Math.Abs(viewInfo.SweepAngle) > 180;
			outerArc.SweepDirection = SweepDirection.Clockwise;
			outerArc.Point = RadMath.GetArcPoint(viewInfo.StartAngle + viewInfo.SweepAngle, center, outerRadius);

			LineSegment firstLine = new LineSegment();
			firstLine.Point = RadMath.GetArcPoint(viewInfo.StartAngle + viewInfo.SweepAngle, center, innerRadius);
			
			ArcSegment secondArc = new ArcSegment();
			secondArc.Size = new Size(innerRadius, innerRadius);
			secondArc.IsLargeArc = Math.Abs(viewInfo.SweepAngle) > 180;
			secondArc.SweepDirection = SweepDirection.Counterclockwise;
			secondArc.Point = RadMath.GetArcPoint(viewInfo.StartAngle, center, innerRadius);


			Point startPoint = RadMath.GetArcPoint(viewInfo.StartAngle, center, outerRadius);
			PathGeometry geometry = new PathGeometry();
			var figure = new PathFigure();
			figure.IsClosed = true;
			figure.IsFilled = true;
			figure.StartPoint = startPoint;
			figure.Segments.Add(outerArc);
			figure.Segments.Add(firstLine);
			figure.Segments.Add(secondArc);

			geometry.Figures.Add(figure);
			return geometry;
		}

		protected Rect GetPieSectionRect(SunburstItemViewInfo viewInfo, Rect modelLayoutSlot, float diameter)
		{
			Rect result;
			float x = (float)(modelLayoutSlot.X + (modelLayoutSlot.Width - diameter) / 2f);
			float y = (float)(modelLayoutSlot.Y + (modelLayoutSlot.Height - diameter) / 2f);
			result = new Rect(x, y, Math.Max(diameter, 1f), Math.Max(diameter, 1f));

			return result;
		}
	}

	public class SunburstContext
    {
		public Rect LayoutSlot;
		public double Diameter;
		public double LevelsCount;
		public double LevelSize;
		public double MinInnerRadius;
    }
}
