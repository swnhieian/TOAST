using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TOAST
{
    class PositionParams
    {
        private double offsetX, offsetY;
        private double scaleX, scaleY;
        private double rotateAngle;
        private double centerX, centerY;
        private RotateTransform rotTransform;

        public PositionParams(double xOffset, double yOffset, double xScale, double yScale, double angle, double centerX, double centerY)
        {
            this.offsetX = xOffset;
            this.offsetY = yOffset;
            this.scaleX = xScale;
            this.scaleY = yScale;
            this.rotateAngle = angle;
            this.centerX = centerX;
            this.centerY = centerY;
            RotTransform = new RotateTransform(rotateAngle,scaleX * centerX + offsetX, scaleY * centerY + OffsetY);
        }
        public Point transform(Point pos)
        {
            double newX = scaleX * pos.X + offsetX;
            double newY = scaleY * pos.Y + offsetY;
            return RotTransform.Transform(new Point(newX, newY));
        }
        public Point inverseTransform(Point pos)
        {
            Point tempP = RotTransform.Inverse.Transform(pos);
            double newX = (pos.X - offsetX) / scaleX;
            double newY = (pos.Y - offsetY) / scaleY;
            return new Point(newX, newY);
        }

        public double OffsetX
        {
            get
            {
                return offsetX;
            }

            set
            {
                offsetX = value;
                rotTransform.CenterX = centerX * scaleX + value;
            }
        }

        public double OffsetY
        {
            get
            {
                return offsetY;
            }

            set
            {
                offsetY = value;
                RotTransform.CenterY = centerY * scaleY + value;
            }
        }

        public double ScaleX
        {
            get
            {
                return scaleX;
            }

            set
            {
                scaleX = value;
                RotTransform.CenterX = CenterX * value + offsetX;
            }
        }

        public double ScaleY
        {
            get
            {
                return scaleY;
            }

            set
            {
                scaleY = value;
                RotTransform.CenterY = CenterY * value + OffsetY;
            }
        }

        public double RotateAngle
        {
            get
            {
                return rotateAngle;
            }

            set
            {
                rotateAngle = value;
                rotTransform.Angle = value;
            }
        }

        public RotateTransform RotTransform
        {
            get
            {
                return rotTransform;
            }

            set
            {
                rotTransform = value;
            }
        }

        public double CenterX
        {
            get
            {
                return centerX;
            }

            set
            {
                centerX = value;
                rotTransform.CenterX = value * scaleX + offsetX;
            }
        }

        public double CenterY
        {
            get
            {
                return centerY;
            }

            set
            {
                centerY = value;
                RotTransform.CenterY = value * scaleY + OffsetY;
            }
        }
    }
}
