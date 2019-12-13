using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.RecommendApi.Models
{
    public class ProjectRecommend
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int FromUserId { get; set; }

        public string FromUserName { get; set; }

        public string FromUserAvatar { get; set; }

        public EnumRecommendType RecommendType { get; set; }

        public int ProjectId { get; set; }

        public string ProjectAvatar { get; set; }

        public string ProjectCompany { get; set; }

        public string ProjectIntroduction { get; set; }

        public string ProjectTags { get; set; }

        public string ProjectFinStage { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime RecommendTime { get; set; }
    }

    public enum EnumRecommendType
    {
        Friend = 1,
        System = 2,
        FriendOfAFriend = 3
    }
}
