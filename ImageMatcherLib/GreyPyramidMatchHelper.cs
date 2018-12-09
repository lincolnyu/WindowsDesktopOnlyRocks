namespace ImageMatcherLib
{
    using System;
    using System.Drawing;
    using static ImageHelper;

    public static class GreyPyramidMatchHelper
    {
        /**
         * Pre:
         *  src: the image to look for the key in
         *  key: the image to find a match as sub-image of src
         *  mseLow: takes mse that the result should be no greater than
         * Post:
         *  returns true if a match is found
         *  mse: returns the actual result
         *  location: top-left corner of the subimage that matches key
         * */
        public static bool Search(this Bitmap src, Bitmap key, ref double mseLow, out Rectangle location)
        {
            var giSrc = new GreyImage(src);
            var giKey = new GreyImage(key);

            return Search(giSrc, giKey, ref mseLow, out location);
        }

        private static bool Search(GreyImage giSrc, GreyImage giKey, ref double mseLow, out Rectangle location)
        {
            var histo = new HistoMap(giSrc);

            throw new NotImplementedException();
        }
    }
}
