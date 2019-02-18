namespace MyBot
{
    abstract class Math
    {
        protected const double PI = System.Math.PI;
        protected double Floor(double v)
        {
            return System.Math.Floor(v);
        }
        protected decimal Floor(decimal v)
        {
            return System.Math.Floor(v);
        }
        protected double Ceiling(double v)
        {
            return System.Math.Ceiling(v);
        }
        protected decimal Ceiling(decimal v)
        {
            return System.Math.Ceiling(v);
        }
        protected decimal Min(decimal v1, decimal v2)
        {
            return System.Math.Min(v1, v2);
        }
        protected decimal Max(decimal v1, decimal v2)
        {
            return System.Math.Max(v1, v2);
        }
        protected double Cos(double degree)
        {
            return System.Math.Cos(degree);
        }
        protected double Sin(double degree)
        {
            return System.Math.Sin(degree);
        }
        protected double Asin(double v)
        {
            return System.Math.Asin(v);
        }
        protected double Abs(int v)
        {
            return System.Math.Abs(v);
        }
        protected double Pow(int v1, int v2)
        {
            return System.Math.Pow(v1, v2);
        }
        protected double Sqrt(double p)
        {
            return System.Math.Sqrt(p);
        }
    }
}
