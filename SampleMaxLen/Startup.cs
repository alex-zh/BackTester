using Amazon;

using System.IO;

namespace SampleMaxLen
{
    public class Startup
    {
        public MaxLenOutput OnActivated(MaxLenInput input)
        {
            var buffer = GetBytes(input.DataStorage, input.FileName);
            var result = FindMaxChain(buffer);

            return result;
        }

        private byte[] GetBytes(string dataStorage, string fileName)
        {
            using (var client = new Amazon.S3.AmazonS3Client(RegionEndpoint.USWest2))
            {
                using (var response = client.GetObject(dataStorage, fileName))
                {
                    using (var memotyStream = new MemoryStream())
                    {
                        response.ResponseStream.CopyTo(memotyStream);
                        return memotyStream.ToArray();
                    }
                }
            }
        }

        private MaxLenOutput FindMaxChain(byte[] buffer)
        {
            byte maxRepeatingByte = 0;
            int maxRepeatingByteIndex = 0;
            int maxLength = 0;

            byte currentRepeatingByte = 0;
            int currentRepeatingByteIndex = 0;
            int currentLength = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (i == 0)
                    continue;

                if (buffer[i] == buffer[i - 1])
                {
                    if (currentRepeatingByte == buffer[i])
                    {
                        currentLength++;
                    }
                    else
                    {
                        currentRepeatingByte = buffer[i];
                        currentRepeatingByteIndex = i - 1;
                        currentLength = 2;
                    }
                }
                else
                {
                    if (currentLength >= maxLength)
                    {
                        maxRepeatingByte = currentRepeatingByte;
                        maxRepeatingByteIndex = currentRepeatingByteIndex;
                        maxLength = currentLength;
                    }

                    currentRepeatingByte = 0;
                    currentRepeatingByteIndex = i;
                    currentLength = 0;
                }
            }

            if (currentLength >= maxLength)
            {
                maxRepeatingByte = currentRepeatingByte;
                maxRepeatingByteIndex = currentRepeatingByteIndex;
                maxLength = currentLength;
            }

            return new MaxLenOutput
            {
                Byte = maxRepeatingByte,
                Length = maxLength,
                Index = maxRepeatingByteIndex
            };
        }
    }
}