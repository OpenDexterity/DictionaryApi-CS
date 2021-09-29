namespace DexterityDictionary {
    public class Consts {
        public static byte[] ValidSignature { get; } = { 0x25, 0x56, 0x54, 0x4c };
        public static int ValidSignatureLength { get; } = ValidSignature.Length;
        public static uint DictHeaderSize { get; } = 0x1a;
        public static uint BlkTblRecordSize { get; } = 14;
        public static uint ModTblHeadSize { get; } = 8;
        public static uint ModTblRecordSize { get; } = 0x10;
        public static uint ModDirBlkHeadSize { get; } = 0xe;
        public static uint ModDirTypeTblRecordSize { get; } = 0x10;
        public static uint ModDirResTblRecordSize { get; } = 0x10;
        public static uint ProdInfoSize { get; } = 0x156;
        public static uint ModNamesBlock { get; } = 2;
    }
}
