using MongoDB.Bson.Serialization.Attributes;
using System;

namespace zsq.ContactApi.Models
{
    [BsonIgnoreExtraElements]
    public class ContactApplyRequest
    {
        //public int Id { get; set; }

        /// <summary>
        /// 被添加人id
        /// </summary>
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public string Title { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// 申请人id
        /// </summary>
        public int ApplierId { get; set; }

        public int Approvaled { get; set; }

        public DateTime HandleTime { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
