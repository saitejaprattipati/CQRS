using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Logging;


namespace Author.Core.Functionapp
{
    public static class ImageResize
    {
        [FunctionName("ImageResize")]
        public static void Run([BlobTrigger("testimg/{name}", Connection = "BlobConnection")]Stream image, string name, ILogger log, [Blob("testsmallimg/{name}", FileAccess.Write, Connection = "BlobConnection")] Stream imageSmall,
        [Blob("testmedimg/{name}", FileAccess.Write, Connection = "BlobConnection")] Stream imageMedium, [Blob("testesmall/{name}", FileAccess.Write, Connection = "BlobConnection")] Stream imageeSmall)
        {
            //log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            IImageFormat format;

            using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
            {
                ResizeImage(input, imageSmall, ImageSize.Small, format);
            }

            image.Position = 0;
            using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
            {
                ResizeImage(input, imageMedium, ImageSize.Medium, format);
            }

            image.Position = 0;
            using (Image<Rgba32> input = Image.Load<Rgba32>(image, out format))
            {
                ResizeImage(input, imageeSmall, ImageSize.ExtraSmall, format);
            }
        }
        public static void ResizeImage(Image<Rgba32> input, Stream output, ImageSize size, IImageFormat format)
        {
            var dimensions = imageDimensionsTable[size];

            input.Mutate(x => x.Resize(dimensions.Item1, dimensions.Item2));
            input.Save(output, format);
        }

        public enum ImageSize { ExtraSmall, Small, Medium }

        private static Dictionary<ImageSize, (int, int)> imageDimensionsTable = new Dictionary<ImageSize, (int, int)>() {
        { ImageSize.ExtraSmall, (320, 200) },
        { ImageSize.Small,      (640, 400) },
        { ImageSize.Medium,     (800, 600) }
    };
    }
}
