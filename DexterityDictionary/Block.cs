using System;
using System.IO;

namespace OpenDexterity.DictionaryApi {
    public class Block {
        #region Block Components

        /// <summary>
        /// Block number
        /// </summary>
        public uint Number { get; }
        /// <summary>
        /// Block type
        /// </summary>
        public Enums.BlockType Type { get; }
        /// <summary>
        /// Offset in the dictionary file where this block begins
        /// </summary>
        public uint Start { get; }
        /// <summary>
        /// Block size in bytes
        /// </summary>
        public uint Size { get; }
        /// <summary>
        /// Block's unused space in bytes
        /// </summary>
        public uint Unused { get; }

        /// <summary>
        /// Block's used space in bytes
        /// </summary>
        /// <remarks>Calculated field</remarks>
        public uint Used => this.Size - this.Unused;

        #endregion

        #region Misc Vars

        private Dictionary Parent { get; }

        /// <summary>
        /// Returns a BinaryReader instance for this block.
        /// </summary>
        public BinaryReader GetReader() {
            //setting up the file read stream
            var strm = File.OpenRead(this.Parent.FilePath);
            strm.Seek(this.Start + 1, SeekOrigin.Begin);

            //returning binaryreader
            return new(strm);
        }

        #endregion

        /// <summary>
        /// Class for blocks in a dictionary file.
        /// </summary>
        /// <param name="blockNum">Block number</param>
        /// <param name="type">Block type</param>
        /// <param name="start">Block start offset</param>
        /// <param name="size">Block size in bytes</param>
        /// <param name="unused">Unused space in bytes</param>
        /// <param name="parentDict">The dictionary that this block belongs to</param>
        public Block(uint blockNum, ushort type, uint start, uint size, uint unused, Dictionary parentDict) {
            //vars
            this.Number = blockNum;
            this.Start = start;
            this.Size = size;
            this.Unused = unused;
            this.Parent = parentDict;

            #region Validity Checking
            
            //type
            if (!Enum.IsDefined(typeof(Enums.BlockType), type))
                throw new Exception($"Encountered unknown block type {type} when parsing block {this.Number}");
            else
                this.Type = (Enums.BlockType)type;

            //start offset
            if (this.Start > parentDict.FileInfo.Length)
                throw new Exception($"Block #{this.Number}: Block start offset is greater than the length of the file.");

            //size+offset
            if ((this.Size + this.Start) > parentDict.FileInfo.Length)
                throw new Exception($"Block #{this.Number}: End of block is past the end of the file.");

            //unused space & block size
            if (this.Unused > this.Size)
                throw new Exception($"Block #{this.Number}: Unused space is greater than the size of the block.");

            //unused space & file size
            if (this.Unused > this.Parent.FileInfo.Length)
                throw new Exception($"Block #{this.Number}: Unused space is greater than the size of the file.");

            #endregion
        }
    }
}
