using System.Runtime.Serialization;

namespace Rsoi.Lab3.GatewayService.HttpApi.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class BookInfo
    {
        /// <summary>
        /// UUID книги
        /// </summary>
        /// <value>UUID книги</value>
        [DataMember(Name="bookUid")]
        public Guid BookUid { get; set; }

        /// <summary>
        /// Название книги
        /// </summary>
        /// <value>Название книги</value>
        [DataMember(Name="name")]
        public string Name { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        /// <value>Автор</value>
        [DataMember(Name="author")]
        public string Author { get; set; }

        /// <summary>
        /// Жанр
        /// </summary>
        /// <value>Жанр</value>
        [DataMember(Name="genre")]
        public string Genre { get; set; }

        public BookInfo(Guid bookUid, 
            string name, 
            string author,
            string genre)
        {
            BookUid = bookUid;
            Name = name;
            Author = author;
            Genre = genre;
        }
    }
}
