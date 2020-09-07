using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Linq;

namespace AtxInfo
{
    class AtrDisk
    {
        public struct atr_header
        {
            public UInt16 magic;
            public UInt16 paragraphs;
            public UInt16 sector_size;
            public Byte paragraphs_ex;
        };
        const int atr_header_bytecount = 16;

        bool _verbose = false;
        int _paragraphs = 0;

        atr_header _header;

        public AtrDisk(bool verbose = false)
        {
            _verbose = verbose;
        }
        public bool load_info(string fpath)
        {
            Console.WriteLine();
            Console.WriteLine($"      File: {Path.GetFileName(fpath)}");

            // Check size of file
            FileInfo info = new FileInfo(fpath);

            // Get file header contents
            using (BinaryReader reader = new BinaryReader(File.Open(fpath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                _header.magic = reader.ReadUInt16();
                if (_header.magic != 0x0296)
                {
                    Console.WriteLine("ERROR:: File missing ATR header");
                    return false;
                }

                _header.paragraphs = reader.ReadUInt16();
                _header.sector_size = reader.ReadUInt16();
                _header.paragraphs_ex = reader.ReadByte();

                _paragraphs = _header.paragraphs + (_header.paragraphs_ex << 16);
                int _bytes = _paragraphs * 16;

                Console.WriteLine($"   Density: {_header.sector_size}");
                Console.WriteLine($"Paragraphs: {_paragraphs:N0}");
                Console.WriteLine($"     Bytes: {_bytes:N0}");

                if(_bytes + atr_header_bytecount != info.Length)
                    Console.WriteLine($" WARNING:: File size = {info.Length:N0} (diff={info.Length - _paragraphs * 16:N0})");

                if(_header.sector_size != 128 && _header.sector_size != 256)
                    Console.WriteLine("WARNING:: Sector size is not 128 or 256");

            }

            return true;
        }

    }
}