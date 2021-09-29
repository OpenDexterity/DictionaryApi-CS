namespace DexterityDictionary {
    public class Enums {
        public enum ResourceTypes {
            TableNormal = 1,
            Form = 2,
            FormMenuOne = 4,
            DataType = 6,
            Composite = 7,
            Format = 8,
            StringSystem = 9,
            Picture = 15,
            Field = 18, //possibly global fields only?
            ProcedureSource = 19, //maybe global specific ID?
            ProcedureDocumentation = 20,
            ProcedureCompiled = 21, //assumed
            ProcedureDebug = 22, //assumed
            Report = 23,
            Global = 26,
            NativePicture = 28,
            Message = 30,
            ProjectCore = 40, //could also be system info stuff?
            TableGroup = 42,
            Constant = 48,
            Install = 50, //Install Information
            TableCoreOne = 52,
            TableCoreTwo = 53,
            TableCoreThree = 54,
            Product = 55, //Product Information
            TableVirtual = 58,
            FunctionSource = 60, //global specific ID?
            FormMenuTwo = 64,
            SourceObjectList = 68,
            Library = 69, //dll reference I think
            Icon = 72,
            StringOne = 73, //strings have multiple IDs assigned
            ProcedureProperties = 76,
            ProcedureUnknown = 77 //marked as "code/data" in report
        }

        public enum Cores {
            //found a list online but it's technically for sql so idk if this will match up
            //https://support.microsoft.com/en-us/topic/how-to-control-the-location-of-the-dexterity-tables-in-sql-server-by-using-dexterity-path-names-in-microsoft-dynamics-gp-9-0-or-in-microsoft-business-solutions-great-plains-c47c7009-ebc2-0bc6-2410-459df16a1de3
            Financial = 1,
            Sales,
            Purchasing,
            Inventory,
            Payroll,
            Project,
            System,
            Company,
            ThirdParty, //no sql path
            PayrollTax,
            Pathname,
            DesignDocument,
            Dexterity,
            DexteritySystem,
            ReportWriter //no sql path
        }

        public enum BlockType : ushort {
            Unused = 0,
            ModuleTable = 1,
            ModuleDirectory = 2,
            ModuleNames = 3,
            ModuleData = 4,
            //no idea what block type 5 is
            BlockTable = 6
        }
    }
}
