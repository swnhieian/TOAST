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

        //angle is in rad but in rotatetransform angle must be in degree
        public PositionParams(double xOffset, double yOffset, double xScale, double yScale, double angle, double centerX, double centerY)
        {
            this.offsetX = xOffset;
            this.offsetY = yOffset;
            this.scaleX = xScale;
            this.scaleY = yScale;
            this.rotateAngle = angle;
            this.centerX = centerX;
            this.centerY = centerY;
            RotTransform = new RotateTransform(rotateAngle*180/Math.PI,scaleX * centerX + offsetX, scaleY * centerY + OffsetY);
        }
        public Point transform(Point pos)
        {
            double newX = scaleX * pos.X + offsetX;
            double newY = scaleY * pos.Y + offsetY;
            return RotTransform.Transform(new Point(newX, newY));
            //return new Point(newX, newY);
        }
        public Point inverseTransform(Point pos)
        {
            Point tempP = RotTransform.Inverse.Transform(pos);
            double newX = (tempP.X - offsetX) / scaleX;
            double newY = (tempP.Y - offsetY) / scaleY;
            return new Point(newX, newY);
        }

        public static PositionParams avg(PositionParams param1, PositionParams param2)
        {
            double ox = (param1.OffsetX + param2.OffsetX) / 2;
            double oy = (param1.OffsetY + param2.OffsetY) / 2;
            double sx = (param1.ScaleX + param2.ScaleX) / 2;
            double sy = (param1.ScaleY + param2.ScaleY) / 2;
            double a = (param1.RotateAngle + param2.RotateAngle) / 2;
            PositionParams ret = new PositionParams(ox, oy, sx, sy, a, 0, 0);
            return ret;
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
                rotTransform.Angle = value * 180 / Math.PI;
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
                RotTransform.CenterX = value * scaleX + offsetX;
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
        public override string ToString()
        {
            return String.Format("off:{0},{1}\nsca:{2},{3}\nang:{4}\ncen:{5},{6}", offsetX, offsetY, scaleX, scaleY, rotateAngle, centerX, centerY);
        }
    }
}
