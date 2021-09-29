using System.IO;
using System.Text;

namespace OpenDexterity.DictionaryApi {
    public class Module {
        /// <summary>
        /// Module's type
        /// </summary>
        public ushort Type { get; }

        /// <summary>
        /// Module's ID
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Block number of module's directory block
        /// </summary>
        public uint DirBlock { get; }

        /// <summary>
        /// Module's name offset
        /// </summary>
        /// <remarks>Module names are in block 2.</remarks>
        public uint NameOffset { get; }

        /// <summary>
        /// Module's name length, includes null terminator.
        /// </summary>
        /// <remarks>Unit = characters. String charset is ASCII.</remarks>
        public ushort NameLength { get; }

        /// <summary>
        /// Dictionary that this module belongs to
        /// </summary>
        private Dictionary Parent { get; }

        /// <summary>
        /// Is this module blank?
        /// </summary>
        public bool IsBlank => this.Type == 0;

        /// <summary>
        /// Class for dictionary modules
        /// </summary>
        /// <param name="type">Module type</param>
        /// <param name="id">Module ID</param>
        /// <param name="dirBlock">Module directory block number</param>
        /// <param name="nameOffset">Module name offset (in block 2)</param>
        /// <param name="nameLength">Module name length (ASCII chars)</param>
        /// <param name="parentDict">Dictionary that this module belongs to</param>
        public Module(ushort type, uint id, uint dirBlock, uint nameOffset, ushort nameLength, Dictionary parentDict) {
            //vars
            this.Type = type;
            this.Id = id;
            this.DirBlock = dirBlock;
            this.NameOffset = nameOffset;
            this.NameLength = nameLength;
            this.Parent = parentDict;

            //validity checking
            //TODO
        }

        /// <summary>
        /// Gets this module's name from block 2.
        /// </summary>
        /// <returns>Module's name as string</returns>
        public string GetName() {
            //getting the block
            Block names = this.Parent.GetBlock(Consts.ModNamesBlock);

            //getting the name
            StringBuilder modName = new();
            using (BinaryReader reader = names.GetReader()) {
                //go to the name offset
                reader.BaseStream.Seek(this.NameOffset, SeekOrigin.Current);

                //grab the characters out of the name block
                for (uint i = 1; i < this.NameLength; i++) {
                    var curByte = reader.ReadByte();

                    modName.Append((char)curByte);
                }
            }

            //all done
            return modName.ToString();
        }
    }
}
