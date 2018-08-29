using LinqToDB;
using LinqToDB.DataProvider;
using LinqToDB.Mapping;
using System;

namespace KIMParserWinForms
{
    public partial class LOCAL_DB : LinqToDB.Data.DataConnection
    {
        public ITable<KIMStorage> kIMStorages { get { return this.GetTable<KIMStorage>(); } }

        public LOCAL_DB()
        {
            InitDataContext();
        }

        public LOCAL_DB(string configuration)
            : base(configuration)
        {
            InitDataContext();
        }

        public LOCAL_DB(IDataProvider dataProvider, string configuration)
            : base(dataProvider, configuration)
        {
            InitDataContext();
        }

        partial void InitDataContext();

        [Table(Name = "KIMStorage")]
        public partial class KIMStorage
        {
            // PK
            [NotNull, PrimaryKey]
            public Guid KimId { get; set; }
            // Дата занесения
            [Column, Nullable]
            public DateTime CreateTime { get; set; }
            // Есть ли аудио часть?
            [Column, Nullable]
            public bool HasAudio { get; set; }
            // Сам аудио файл
            [Column, Nullable]
            public byte[] AudioFile { get; set; }
            // Предмет
            [Column, Nullable]
            public string Subject { get; set; }
            // Раздел
            [Column, Nullable]
            public string Theme { get; set; }
            // Текст задания
            [Column, Nullable]
            public string Task { get; set; }
        }

    }
}
