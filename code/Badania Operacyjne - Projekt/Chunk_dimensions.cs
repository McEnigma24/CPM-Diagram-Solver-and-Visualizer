using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Badania_Operacyjne___Projekt
{
    public class pos_index_value
    {
        public Point pos;
        public int index_value_BASE_0;

        public pos_index_value(Point _pos_, int _index_value_BASE_0_)
        {
            pos = _pos_;
            index_value_BASE_0 = _index_value_BASE_0_;
        }        
    }

    public class connested_indexes
    {
        public pos_index_value A;
        public pos_index_value B;

        public connested_indexes(pos_index_value a, pos_index_value b)
        {
            A = a;
            B = b;
        }
    }


    public class Circle
    {
        public PointF center;
        public float radious;

        public Circle(PointF c, float r)
        {
            center = c;
            radious = r;
        }

        public Circle(Point c, float r)
        {
            center.X = (float)c.X;
            center.Y = (float)c.Y;
            radious = r;
        }
    }
    public class vector
    {
        public float x;
        public float y;
        public float distance;

        public vector(Point A, Point B)
        {
            x = B.X - A.X;
            y = B.Y - A.Y;

            normalize();
        }
        public vector(PointF A, PointF B)
        {
            x = B.X - A.X;
            y = B.Y - A.Y;

            normalize();
        }
        public vector(Circle A, Circle B)
        {
            x = B.center.X - A.center.X;
            y = B.center.Y - A.center.Y;

            normalize();
        }
        // wpisuję wynik odejmowania
        public vector(float _x, float _y)
        {
            x = _x;
            y = _y;

            normalize();
        }
        public void normalize()
        {
            distance = (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            x /= distance;
            y /= distance;
        }
    }

    public class Chunk_dimensions
    {
        public int x_start;
        public int x_stop;
        public int y_start;
        public int y_stop;

        public int x_length;
        public int y_length;

        public Chunk_dimensions()
        {
            int value = -1;

            x_start = value;
            x_stop = value;
            y_start = value;
            y_stop = value;

            x_length = value;
            y_length = value;
        }
        public Chunk_dimensions(int x_sta, int x_sto, int y_sta, int y_sto)
        {
            x_start = x_sta;
            x_stop = x_sto;
            y_start = y_sta;
            y_stop = y_sto;

            calc_lengths();
        }

        public void calc_lengths()
        {
            x_length = x_stop - x_start;
            y_length = y_stop - y_start;
        }
    }    
}
