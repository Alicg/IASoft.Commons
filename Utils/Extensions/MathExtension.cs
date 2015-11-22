namespace Utils.Extensions
{
    public static class MathExtension
    {
        public static int Align(this int x, int alignBound)
        {
            int res = x;
            if (alignBound == 0) return res;
            if (res > 0)
            {
                if (res%alignBound != 0)
                {
                    res -= alignBound;
                    do
                    {
                        res++;
                    } while (res%alignBound != 0);
                }
            }
            else
            {
                if (res%alignBound != 0)
                {
                    res += alignBound;
                    do
                    {
                        res--;
                    } while (res%alignBound != 0);
                }
            }
            return res;
        }
    }
}