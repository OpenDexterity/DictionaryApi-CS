using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenDexterity.DictionaryApi {
    public class Dictionary {
        #region Dictionary Components

        /// <summary>
        /// Dictionary file's signature; the first few bytes of the file.
        /// </summary>
        public byte[] Signature { get; } = new byte[Consts.ValidSignatureLength];

        /// <summary>
        /// Dictionary file's size according to the file's header
        /// </summary>
        public uint HeaderFileSize { get; }

        /// <summary>
        /// Offset in the file where the block table is located
        /// </summary>
        public uint BlockTableOffset { get; }

        /// <summary>
        /// Number of records in the block table
        /// </summary>
        /// <remarks>From dictionary file's header</remarks>
        public uint BlockTableLength { get; }

        /// <summary>
        /// Count of unallocated blocks in the block table
        /// </summary>
        /// <remarks>From dictionary file's header</remarks>
        public uint UnallocatedBlocks { get; }

        /// <summary>
        /// Parsed block table header from dictionary
        /// </summary>
        /// <remarks>C# arrays are zero indexed but this table isn't. Block number - 1 = index in this array.</remarks>
        public Block[] Blocks { get; }

        /// <summary>
        /// Number of module slots that are used
        /// </summary>
        public ushort UsedModules { get; }

        /// <summary>
        /// Number of module slots that are unused
        /// </summary>
        public ushort UnusedModules { get; }

        /// <summary>
        /// Total number of modules that are allocated
        /// </summary>
        public int TotalModules => this.UsedModules + this.UnusedModules;

        /// <summary>
        /// This dictionary's modules
        /// </summary>
        public Module[] Modules { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a block.
        /// </summary>
        /// <returns>DexterityDictionary.Block instance</returns>
        public Block GetBlock(uint BlockNum) => this.Blocks[BlockNum - 1];

        #endregion

        #region Public Vars

        /// <summary>
        /// Path to this dictionary file
        /// </summary>

        public string FilePath { get; }

        #endregion

        /// <summary>
        /// Parses a Dexterity dictionary.
        /// </summary>
        /// <param name="dictPath">Path to a .dic or .cnk file</param>
        public Dictionary(string dictPath) {
            //make sure we have permission to read this file
            this.FilePath = Path.GetFullPath(dictPath);

            //does file exist?
            if (!File.Exists(this.FilePath))
                throw new Exception("Specified file does not exist.");

            //checking for .dic or .cnk extensions
            switch (Path.GetExtension(this.FilePath).ToLower()) {
                case ".cnk":
                case ".dic":
                    //All good
                    break;
                default:
                    throw new Exception("Specified file does not have .dic or .cnk extension.");
            }

            //reading file
            using (BinaryReader reader = new(File.OpenRead(this.FilePath))) {
                #region Dictionary Header
                
                //00,01,02,03: signature
                this.Signature = reader.ReadBytes(Consts.ValidSignatureLength);

                //validating signature too
                if (!Consts.ValidSignature.SequenceEqual(this.Signature))
                    throw new Exception("Dictionary's header is not valid.");

                //04,05,06,07 are unknown. skipping for now.
                reader.BaseStream.Seek(0x8, SeekOrigin.Begin);

                //08,09,0A,0B: OS file size on disk
                this.HeaderFileSize = reader.ReadUInt32();

                //0C,0D are unknown, skipping.
                reader.BaseStream.Seek(0xe, SeekOrigin.Begin);

                //0E,0F,10,11: block table offset
                this.BlockTableOffset = reader.ReadUInt32();

                //12,13,14,15: block table length
                this.BlockTableLength = reader.ReadUInt32();

                //16,17,18,19: unallocated block count
                this.UnallocatedBlocks = reader.ReadUInt32();

                #endregion

                #region Block Table

                //prep
                reader.BaseStream.Seek(this.BlockTableOffset, SeekOrigin.Begin);
                List<Block> blocks = new();

                //loop through each record. each record is 0xe bytes in size. file header shows # of records.
                for (uint i = 0, blkNum = 1; i < (this.BlockTableLength * Consts.BlkTblRecordSize); i += Consts.BlkTblRecordSize, blkNum++) {
                    //00,01: type
                    var type = reader.ReadUInt16();

                    //02,03,04,05: block start offset
                    var start = reader.ReadUInt32();

                    //06,07,08,09: size
                    var size = reader.ReadUInt32();

                    //0A,0B,0C,0D: unused space
                    var unused = reader.ReadUInt32();

                    //doing class things then adding to array
                    Block blk = new(blkNum, type, start, size, unused, this);
                    blocks.Add(blk);
                }

                //finishing up
                this.Blocks = blocks.ToArray();

                #endregion

                #region Module Table

                //module table is always block 1. go to that block and read it
                reader.BaseStream.Seek(this.Blocks[0].Start, SeekOrigin.Begin);

                //module table has a header
                //00,01: used module count
                this.UsedModules = reader.ReadUInt16();
                
                //02,03: unused module count
                this.UnusedModules = reader.ReadUInt16();

                //04,05,06,07: unknown field. skipping
                reader.BaseStream.Seek(4, SeekOrigin.Current);

                //record prep
                List<Module> mods = new();

                //parsing module table records. module table records are 0x10 bytes in size.
                for (uint i = 0; i < (this.TotalModules * Consts.ModTblRecordSize); i += Consts.ModTblRecordSize) {
                    //00,01: module type
                    ushort type = reader.ReadUInt16();

                    //02,03,04,05: module ID
                    uint id = reader.ReadUInt32();

                    //06,07,08,09: directory block number
                    uint dir = reader.ReadUInt32();

                    //0A,0B,0C,0D: name offset
                    uint noff = reader.ReadUInt32();

                    //0E,0F: name length
                    ushort nlen = reader.ReadUInt16();

                    //making class, adding to list
                    Module m = new(type, id, dir, noff, nlen, this);
                    mods.Add(m);
                }

                //finishing
                this.Modules = mods.ToArray();

                #endregion
            }

        }
    }
}
