using System.Diagnostics;

// from https://github.com/raminrahimzada/CSharp-Helper-Classes/blob/master/Math/DecimalMath/DecimalMath.cs
namespace Ametrin.Utils;

public static class MathM
{
    public const decimal Half = 0.5M;
    public const decimal PI = 3.14159265358979323846264338327950288419716939937510M; //output: 3.1415926535897932384626433833
    /// <summary>
    /// represents a small value
    /// </summary>
    public const decimal Epsilon = 0.0000000000000000001M;
    public const decimal E = 2.7182818284590452353602874713526624977572470936999595749M;
    private const decimal PIx2 = PI * 2;
    private const decimal PIdiv2 = PI / 2.0M;
    private const decimal PIdiv4 = PI / 4.0M;
    private const decimal Einv = decimal.One / E;
    private const decimal Log10Inv = 0.434294481903251827651128918916605082294397005803666566114M;
    private const int MaxTaylorIteration = 100;

    extension(decimal)
    {
        public static decimal Exp(decimal x)
        {
            var count = 0;

            if (x > decimal.One)
            {
                count = decimal.ToInt32(decimal.Truncate(x));
                x -= decimal.Truncate(x);
            }

            if (x < decimal.Zero)
            {
                count = decimal.ToInt32(decimal.Truncate(x) - 1);
                x = decimal.One + (x - decimal.Truncate(x));
            }

            var iteration = 1;
            var result = decimal.One;
            var factorial = decimal.One;
            decimal cachedResult;
            do
            {
                cachedResult = result;
                factorial *= x / iteration++;
                result += factorial;
            } while (cachedResult != result);

            return count == 0 ? result : result * PowN(E, count);
        }

        public static decimal Pow(decimal value, decimal pow)
        {
            if (pow == decimal.Zero)
                return decimal.One;
            if (pow == decimal.One)
                return value;
            if (value == decimal.One)
                return decimal.One;

            if (value == decimal.Zero)
            {
                if (pow > decimal.Zero)
                {
                    return decimal.Zero;
                }

                throw new InvalidOperationException("Invalid Operation: zero base and negative power");
            }

            if (pow == -decimal.One)
                return decimal.One / value;

            var isPowerInteger = decimal.IsInteger(pow);
            if (value < decimal.Zero && !isPowerInteger)
            {
                throw new InvalidOperationException("Invalid Operation: negative base and non-integer power");
            }

            if (isPowerInteger && value > decimal.Zero)
            {
                int powerInt = (int)pow;
                return PowN(value, powerInt);
            }

            if (isPowerInteger && value < decimal.Zero)
            {
                int powerInt = (int)pow;
                if (powerInt % 2 == 0)
                {
                    return Exp(pow * Log(-value));
                }

                return -Exp(pow * Log(-value));
            }

            return Exp(pow * Log(value));
        }

        /// <summary>
        /// Power to the integer value
        /// </summary>
        public static decimal PowN(decimal value, int power)
        {
            while (true)
            {
                if (power == decimal.Zero)
                    return decimal.One;
                if (power < decimal.Zero)
                {
                    value = decimal.One / value;
                    power = -power;
                    continue;
                }

                var q = power;
                var prod = decimal.One;
                var current = value;
                while (q > 0)
                {
                    if (q % 2 == 1)
                    {
                        // detects the 1s in the binary expression of power
                        prod = current * prod; // picks up the relevant power
                        q--;
                    }

                    current *= current; // value^i -> value^(2*i)
                    q >>= 1;
                }

                return prod;
            }
        }

        public static decimal Log10(decimal x) => Log(x) * Log10Inv;
        public static decimal Log(decimal x)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(x, decimal.Zero);
            if (x == decimal.One)
                return decimal.Zero;

            var count = 0;
            while (x >= decimal.One)
            {
                x *= Einv;
                count++;
            }

            while (x <= Einv)
            {
                x *= E;
                count--;
            }

            x--;
            if (x == decimal.Zero)
                return count;

            var result = decimal.Zero;
            var iteration = 0;
            var y = decimal.One;
            var cacheResult = result - decimal.One;

            while (cacheResult != result && iteration < MaxTaylorIteration)
            {
                iteration++;
                cacheResult = result;
                y *= -x;
                result += y / iteration;
            }

            return count - result;
        }

        public static decimal Cos(decimal x)
        {
            //truncating to  [-2*PI;2*PI]
            TruncateToPeriodicInterval(ref x);

            // now x in (-2pi,2pi)
            if (x >= PI && x <= PIx2)
            {
                return -Cos(x - PI);
            }
            if (x >= -PIx2 && x <= -PI)
            {
                return -Cos(x + PI);
            }

            x *= x;
            //y=1-x/2!+x^2/4!-x^3/6!...
            var xx = -x * Half;
            var y = decimal.One + xx;
            var cachedY = y - decimal.One;//init cache  with different value
            for (var i = 1; cachedY != y && i < MaxTaylorIteration; i++)
            {
                cachedY = y;
                decimal factor = i * ((i << 1) + 3) + 1; //2i^2+2i+i+1=2i^2+3i+1
                factor = -Half / factor;
                xx *= x * factor;
                y += xx;
            }
            return y;
        }

        public static decimal Sin(decimal x)
        {
            var cos = Cos(x);
            return CalculateSinFromCos(x, cos);
        }

        public static decimal Tan(decimal x)
        {
            var cos = Cos(x);
            if (cos == decimal.Zero)
                throw new ArgumentOutOfRangeException(nameof(x));
            var sin = CalculateSinFromCos(x, cos);
            return sin / cos;
        }

        public static decimal Sqrt(decimal x, decimal epsilon = decimal.Zero)
        {
            if (x < decimal.Zero)
                throw new OverflowException("Cannot calculate square root from a negative number");

            // initial approximation
            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == decimal.Zero)
                    return decimal.Zero;
                current = (previous + x / previous) * Half;
            } while (decimal.Abs(previous - current) > epsilon);
            return current;
        }

        public static decimal Sinh(decimal x)
        {
            var y = Exp(x);
            var yy = decimal.One / y;
            return (y - yy) * Half;
        }

        public static decimal Cosh(decimal x)
        {
            var y = Exp(x);
            var yy = decimal.One / y;
            return (y + yy) * Half;
        }

        public static decimal Tanh(decimal x)
        {
            var y = Exp(x);
            var yy = decimal.One / y;
            return (y - yy) / (y + yy);
        }

        public static decimal Asin(decimal x)
        {
            if (x > decimal.One || x < -decimal.One)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "x must be in [-1,1]");
            }
            //known values
            if (x == decimal.Zero)
                return decimal.Zero;
            if (x == decimal.One)
                return PIdiv2;
            //asin function is odd function
            if (x < decimal.Zero)
                return -Asin(-x);

            //my optimize trick here

            // used a math formula to speed up :
            // asin(x)=0.5*(pi/2-asin(1-2*x*x)) 
            // if x>=0 is true

            var newX = decimal.One - 2 * x * x;

            //for calculating new value near to zero than current
            //because we gain more speed with values near to zero
            if (decimal.Abs(x) > decimal.Abs(newX))
            {
                var t = Asin(newX);
                return Half * (PIdiv2 - t);
            }
            var y = decimal.Zero;
            var result = x;
            decimal cachedResult;
            var i = 1;
            y += result;
            var xx = x * x;
            do
            {
                cachedResult = result;
                result *= xx * (decimal.One - Half / (i));
                y += result / ((i << 1) + 1);
                i++;
            } while (cachedResult != result);
            return y;
        }

        public static decimal ATan(decimal x) => x switch
        {
            decimal.Zero => decimal.Zero,
            decimal.One => PIdiv4,
            _ => Asin(x / Sqrt(decimal.One + x * x))
        };

        public static decimal Acos(decimal x) => x switch
        {
            decimal.Zero => PIdiv2,
            decimal.One => decimal.Zero,
            < decimal.Zero => PI - Acos(-x),
            _ => PIdiv2 - Asin(x)
        };

        public static decimal Atan2(decimal y, decimal x) => x switch
        {
            > decimal.Zero => ATan(y / x),
            < decimal.Zero when y >= decimal.Zero => ATan(y / x) + PI,
            < decimal.Zero when y < decimal.Zero => ATan(y / x) - PI,
            decimal.Zero when y > decimal.Zero => PIdiv2,
            decimal.Zero when y < decimal.Zero => -PIdiv2,
            _ => throw new ArgumentException("invalid atan2 arguments")
        };
    }

    private static decimal CalculateSinFromCos(decimal x, decimal cos)
    {
        var moduleOfSin = Sqrt(decimal.One - (cos * cos));
        var sineIsPositive = IsSignOfSinePositive(x);
        return sineIsPositive ? moduleOfSin : -moduleOfSin;
    }

    /// <summary>
    /// Truncates to [-2*PI;2*PI]
    /// </summary>
    private static void TruncateToPeriodicInterval(ref decimal x)
    {
        while (x >= PIx2)
        {
            var divide = int.Abs(decimal.ToInt32(x / PIx2));
            x -= divide * PIx2;
        }

        while (x <= -PIx2)
        {
            var divide = int.Abs(decimal.ToInt32(x / PIx2));
            x += divide * PIx2;
        }
    }


    private static bool IsSignOfSinePositive(decimal x)
    {
        //truncating to  [-2*PI;2*PI]
        TruncateToPeriodicInterval(ref x);

        //now x in [-2*PI;2*PI]
        return x switch
        {
            >= -PIx2 and <= -PI => true,
            >= -PI and <= decimal.Zero => false,
            >= decimal.Zero and <= PI => true,
            >= PI and <= PIx2 => false,
            _ => throw new UnreachableException(nameof(x))
        };
    }
}