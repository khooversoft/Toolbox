using System;
using System.IO;

namespace Khooversoft.Parser
{
    public static class FileExtensions
    {

        public static string VerifyFileWithExtension(this string self, string extension)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                throw new ArgumentException(nameof(self));
            }

            string originalFile = self;
            if (File.Exists(self))
            {
                return self;
            }

            self = Path.ChangeExtension(self, extension);
            if (File.Exists(self))
            {
                return self;
            }

            throw new ArgumentException($"Cannot find configuration {originalFile}");
        }
    }
}
