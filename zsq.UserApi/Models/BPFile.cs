using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zsq.UserApi.Models
{
    public class BPFile
    {
        public int Id { get; set; }

        public int AppUserId { get; set; }

        /// <summary>
        /// 暂时无用
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 源文件地址
        /// </summary>
        public string OriginFilePath { get; set; }

        /// <summary>
        /// 格式转化后的地址
        /// </summary>
        public string FormatFilePath { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
