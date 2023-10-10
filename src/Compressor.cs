using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;

namespace Compressor
{
    static class Compressor
    {
        #region Public Methods

        public static void Decode(string[] datasetsPath)
        {
            foreach (var dataset in datasetsPath)
            {
                string decodedDataset = Convert.ToBase64String(File.ReadAllBytes(dataset));
                SaveToFile(dataset, decodedDataset, "decoded");
            }
        }

        public static void Rename(string[] datasetsPath)
        {
            string folderPath = Path.GetDirectoryName(datasetsPath[0]);
            string resultPath = $"{folderPath}\\{Directory.CreateDirectory($"{folderPath}\\renamed").ToString()}";
            DirectoryInfo dirInfo = new DirectoryInfo(resultPath);
            dirInfo.Attributes = FileAttributes.Normal;

            for (int i = 0; i < datasetsPath.Length; i++)
            {
                string target = String.Concat(resultPath, "\\dataset_", i.ToString());
                if (!File.Exists(target))
                {
                    File.Copy(Path.GetFullPath(datasetsPath[i]), target);
                }
            }
        }

        public static void Compress(string[] datasetsPath)
        {
            foreach (var dataset in datasetsPath)
            {
                byte[] input = File.ReadAllBytes(dataset);
                var compressedData = CompressDeflate(input);
                SaveToFile(dataset, compressedData, "compressed");
            }
        }

        public static void Decompress(string[] datasetsPath)
        {
            foreach (var compressedData in datasetsPath)
            {
                byte[] input = File.ReadAllBytes(compressedData);
                byte[] output = DecompressDeflate(input);
                string datasetUTF8 = DecodeUtf8(output);
                SaveToFile(compressedData, datasetUTF8, "decompressed");
            }
        }

        #endregion

        #region Private Methods

        private static string DecodeUtf8(byte[] encoded)
        {
            return Encoding.UTF8.GetString(encoded);
        }

        private static string CompressGzip(string data)
        {
            var bytes = Encoding.Unicode.GetBytes(data);
            var output = new MemoryStream();
            var streamToCompress = new MemoryStream(bytes);
            var zipStream = new GZipStream(output, CompressionMode.Compress);

            streamToCompress.CopyToAsync(zipStream).Wait();
            zipStream.Close();
            var outputBytes = output.ToArray();
            return Convert.ToBase64String(outputBytes);
        }

        private static string CompressDeflate(byte[] data)
        {
            var input = new MemoryStream(data);
            var output = new MemoryStream();
            var zipStream = new DeflateStream(output, CompressionMode.Compress);

            input.CopyToAsync(zipStream).Wait();
            zipStream.Close();

            return Convert.ToBase64String(output.ToArray());
        }

        private static string DecompressGzip(string data)
        {
            var compressedBytes = Convert.FromBase64String(data);
            byte[] decompressedData;

            var output = new MemoryStream();
            var compressedStream = new MemoryStream(compressedBytes);
            var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);

            zipStream.CopyTo(output);
            zipStream.Close();
            decompressedData = output.ToArray();

            // check if big ending
            if (decompressedData[0] == 0xFE && decompressedData[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode.GetString(decompressedData).Replace("\uFEFF", string.Empty);
            }
            // check if little ending
            if (decompressedData[1] == 0xFE && decompressedData[0] == 0xFF)
            {
                return Encoding.Unicode.GetString(decompressedData).Replace("\uFEFF", string.Empty);
            }
            // if incoming array does not contains an info about encoding we will try to encode it using little ending
            return Encoding.Unicode.GetString(decompressedData);
        }

        private static byte[] DecompressDeflate(byte[] compressedBytes)
        {
            var output = new MemoryStream();
            var compressedStream = new MemoryStream(compressedBytes);
            var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress);

            decompressorStream.CopyToAsync(output).Wait();
            decompressorStream.Close();
            return output.ToArray();
        }

        private static void SaveToFile(string filePath, string data, string outputFolderName)
        {
            string fileName = Path.GetFileName(filePath);
            string folderPath = Path.GetDirectoryName(filePath);
            string resultPath = $"{folderPath}\\{Directory.CreateDirectory($"{folderPath}\\{outputFolderName}").ToString()}";

            DirectoryInfo dirInfo = new DirectoryInfo(resultPath);
            dirInfo.Attributes = FileAttributes.Normal;

            FileStream fstream = new FileStream($"{resultPath}\\{fileName}", FileMode.Create);
            byte[] array = Encoding.Default.GetBytes(data);
            fstream.WriteAsync(array, 0, array.Length).Wait();
            fstream.Close();
        }

        #endregion
    }
}