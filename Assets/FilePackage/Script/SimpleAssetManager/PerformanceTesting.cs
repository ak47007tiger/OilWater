//using System;
//using System.IO;

//public class PerformanceTesting
//{
//    static void Main(string[] args)
//    {
//        using (var stream = File.OpenRead("/Users/Gui/Desktop/和风物语BGM.mp3"))
//        {
//            bool perfectMatch = true;
//            using (var stream2 = File.OpenRead("/Users/Gui/Desktop/和风物语BGM副本.mp3"))
//            {
//                int a = stream.ReadByte();
//                int b = stream2.ReadByte();
//                for(int i = 0;i < stream.Length;i++)
//                {
//                    if (a != b)
//                    {
//                        perfectMatch = false;
//                        break;
//                    }
//                    a = stream.ReadByte();
//                    b = stream2.ReadByte();
//                }
//                if (perfectMatch)
//                {
//                    Console.WriteLine("two files are identical.");
//                }
//            }

//                Console.WriteLine(stream.Length);
//            //using (var writer = File.OpenWrite("/Users/Gui/Desktop/和风物语BGM副本.mp3"))
//            //{
//            //    int cache = stream.ReadByte();
//            //    byte[] cacheByte = BitConverter.GetBytes(cache);
//            //    while(cache != -1)
//            //    {
//            //        writer.Write(cacheByte);
//            //        cache = stream.ReadByte();
//            //        cacheByte = BitConverter.GetBytes(cache);
//            //    }

//            //    long streamLength = stream.Length;
//            //    int bufferSize = (int)Math.Min(1024, streamLength);
//            //    long start = 0;
//            //    long remain = streamLength;
//            //    int len = (int)Math.Min(bufferSize, remain);

//            //    stream.Seek(0, SeekOrigin.Begin);

//            //    while (remain > 0)
//            //    {
//            //        byte[] buffer = new byte[len];
//            //        stream.Read(buffer, 0, len);
//            //        writer.Write(buffer);
//            //        start += len;
//            //        remain = streamLength - start;
//            //        len = (int)Math.Min(bufferSize, remain);
//            //    }
//            //}
//        }
//    }
//}
