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
        private RotateTransform rotTransform;

        public PositionParams(double xOffset, double yOffset, double xScale, double yScale, double angle)
        {
            this.offsetX = xOffset;
            this.offsetY = yOffset;
            this.scaleX = xScale;
            this.scaleY = yScale;
            this.rotateAngle = angle;
            rotTransform = new RotateTransform(rotateAngle, offsetX, OffsetY);
        }
        public Point transform(Point pos)
        {
            double newX = scaleX * pos.X + offsetX;
            double newY = scaleY * pos.Y + offsetY;
            return rotTransform.Transform(new Point(newX, newY));
        }
        public Point inverseTransform(Point pos)
        {
            Point tempP = rotTransform.Inverse.Transform(pos);
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
            }
        }
    }
}
