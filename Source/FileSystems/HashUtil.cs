using PreappPartnersLib.Utils;
using System;

namespace PreappPartnersLib.FileSystems
{
    public static class HashUtil
    {
        private static string NormalizePath1(string path)
        {
            var start = 0;
            if (path[0] == '.')
            {
                if (path[1] == '\\')
                    start = 2;
                else if (path[start + 1] == '/')
                    start += 2;
            }

            if (path[start] == '/' && path[start + 1] == '/')
                start += 2;

            var startChar = path[start];
            if (path[start] == '\\')
                startChar = path[start + 1];

            var start2 = start + 1;
            if (startChar != '\\')
                start2 = start;

            start = start + 1;
            if (startChar != '/')
                start = start2;

            return path.Substring(start);
        }

        private static int NormalizePath2(Span<byte> dst, ReadOnlySpan<byte> src)
        {
            int srcIdx;      // esi
            int dstIdx;      // edi
            byte srcChar;    // cl
            byte dstChar;    // al

            srcIdx = 0;
            dstIdx = 0;
            if (src[0] != 0)
            {
                while (true)
                {
                    srcChar = src[srcIdx];
                    if (src[srcIdx] > 0x7F) // IsDBCSLeadByte
                    {
                        dst[dstIdx++] = srcChar;
                        dstChar = src[++srcIdx];
                        goto LABEL_14;
                    }

                    if (srcChar == '/')
                    {
                        if (src[srcIdx + 1] == '/')
                        {
                            ++srcIdx;
                            dstChar = (byte)'\\';
                            goto LABEL_14;
                        }
                    }
                    else if (srcChar == '\\')
                    {
                        if (src[srcIdx + 1] != '\\')
                        {
                            dstChar = (byte)char.ToLower((char)srcChar);
                        }
                        else
                        {
                            ++srcIdx;
                            dstChar = (byte)'\\';
                        }

                        goto LABEL_14;
                    }

                    if (srcChar != '/')
                    {
                        dstChar = (byte)char.ToLower((char)srcChar);
                        goto LABEL_14;
                    }

                    dstChar = (byte)'\\';
                LABEL_14:
                    ++srcIdx;
                    dst[dstIdx++] = dstChar;
                    if (src[srcIdx] == 0)
                    {
                        dst[dstIdx] = 0;
                        return dstIdx;
                    }
                }
            }

            return dstIdx;
        }

        private static unsafe int NormalizePath3(byte* dst, byte* src)
        {
            int srcIdx;      // esi
            int dstIdx;      // edi
            byte srcChar;    // cl
            byte dstChar;    // al

            srcIdx = 0;
            dstIdx = 0;
            if (src[0] != 0)
            {
                while (true)
                {
                    srcChar = src[srcIdx];
                    if (src[srcIdx] > 0x7F) // IsDBCSLeadByte
                    {
                        dst[dstIdx++] = srcChar;
                        dstChar = src[++srcIdx];
                        goto LABEL_14;
                    }

                    if (srcChar == '/')
                    {
                        if (src[srcIdx + 1] == '/')
                        {
                            ++srcIdx;
                            dstChar = (byte)'\\';
                            goto LABEL_14;
                        }
                    }
                    else if (srcChar == '\\')
                    {
                        if (src[srcIdx + 1] != '\\')
                        {
                            dstChar = (byte)char.ToLower((char)srcChar);
                        }
                        else
                        {
                            ++srcIdx;
                            dstChar = (byte)'\\';
                        }

                        goto LABEL_14;
                    }

                    if (srcChar != '/')
                    {
                        dstChar = (byte)char.ToLower((char)srcChar);
                        goto LABEL_14;
                    }

                    dstChar = (byte)'\\';
                LABEL_14:
                    ++srcIdx;
                    dst[dstIdx++] = dstChar;
                    if (src[srcIdx] == 0)
                    {
                        dst[dstIdx] = 0;
                        return dstIdx;
                    }
                }
            }

            return dstIdx;
        }

        public static ushort ComputeHash(string path)
        {
            var path2 = NormalizePath1(path);

            Span<byte> pathBuf = stackalloc byte[260];
            EncodingCache.ShiftJIS.GetBytes(path2, pathBuf);

            Span<byte> pathBuf2 = stackalloc byte[260];
            var pathBuf2Size = NormalizePath2(pathBuf2, pathBuf);

            uint hash = 0;
            if (pathBuf2Size > 0)
            {
                for (int i = 0; i < pathBuf2Size; i++)
                {
                    hash |= pathBuf2[i];
                    for (int j = 0; j < 8; j++)
                    {
                        var temp = 2 * hash;
                        hash = temp ^ 0x1102100;
                        if ((0x1000000 & temp) == 0)
                            hash = temp;
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var temp = 2 * hash;
                    hash = temp ^ 0x1102100;
                    if ((0x1000000 & temp) == 0)
                        hash = temp;
                }
            }

            return (ushort)(hash >> 8);
        }
    }
}
