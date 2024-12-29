using ImageMagick;

namespace PaSharper.Utilities.File;

public static class PdfImageCutter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="pageNumber"></param>
    /// <param name="upperEdgeByPercent">由底部向上的裁切上边缘占页面高的百分比</param>
    /// <param name="lowerEdgeByPercent">由底部向上的裁切下边缘占页面高的百分比</param>
    /// <returns></returns>
    public static IMagickImage<ushort> CutImage(this MagickImageCollection fileImageCollection, int pageNumber, double upperEdgeByPercent, double lowerEdgeByPercent,uint quality = 80)
    {
        /*
        using (MagickImageCollection images = new MagickImageCollection())
        {
            var settings = new MagickReadSettings
            {
                Density = new Density(300, 300),
                AntiAlias = true,
                TextAntiAlias = true,
                StrokeAntiAlias = true
            };
            images.Read(filePath,settings);
*/
            var targetPage = fileImageCollection[pageNumber];
            uint pageWidth = targetPage.Width;
            uint pageHeight = targetPage.Height;

            int upperTargetY = (int)Math.Floor(pageHeight * (1 - upperEdgeByPercent));
            uint targetHight = (uint)Math.Floor(pageHeight * (1 - lowerEdgeByPercent) - upperTargetY);
            
            targetPage.BackgroundColor = MagickColors.White;
            targetPage.Format = MagickFormat.Jpeg;
            targetPage.Quality = quality;
            
            targetPage.Crop(new MagickGeometry(0, upperTargetY, pageWidth, targetHight));

            return targetPage;
    }
}