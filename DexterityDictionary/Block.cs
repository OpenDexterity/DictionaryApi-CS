using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DexterityDictionary {
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

            //validity checking
            if (!Enum.IsDefined(typeof(Enums.BlockType), type))
                throw new Exception($"Encountered unknown block type {type} when parsing block {this.Number}");
            else
                this.Type = (Enums.BlockType)type;
        }
    }
}
