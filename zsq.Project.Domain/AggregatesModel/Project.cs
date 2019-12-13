using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zsq.Project.Domain.Events;
using zsq.Project.Domain.SeedWork;

namespace zsq.Project.Domain.AggregatesModel
{
    public class Project : Entity, IAggregateRoot
    {
        public Project()
        {
            Viewers = new List<ProjectViewer>();
            Contributors = new List<ProjectContributor>();

            AddDomainEvent(new ProjectCreatedEvent
            {
                Project = this
            });
        }

        public int UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// 项目logo
        /// </summary>
        public string Avatar { get; set; }

        public string Company { get; set; }

        /// <summary>
        /// 源文件地址
        /// </summary>
        public string OriginBPFile { get; set; }

        /// <summary>
        /// 格式转化后的地址
        /// </summary>
        public string FormatBPFile { get; set; }

        /// <summary>
        /// 是否显示敏感信息
        /// </summary>
        public bool ShowSecurityInfo { get; set; }

        public int ProvinceId { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        public int CityId { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }

        public int AreaId { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }

        public DateTime RegisterTime { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 出让比例
        /// </summary>
        public string FinPercentage { get; set; }

        /// <summary>
        /// 融资阶段
        /// </summary>
        public string FinStage { get; set; }

        /// <summary>
        /// 融资金额
        /// </summary>
        public string FinMoney { get; set; }

        /// <summary>
        /// 收入
        /// </summary>
        public int Income { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public int Revenue { get; set; }

        /// <summary>
        /// 估值
        /// </summary>
        public int Valuation { get; set; }

        /// <summary>
        /// 佣金分配方式
        /// </summary>
        public int BrokerageOptions { get; set; }

        /// <summary>
        /// 是否委托给平台
        /// </summary>
        public bool OnPlatform { get; set; }

        /// <summary>
        /// 可见范围
        /// </summary>
        public ProjectVisibleRule VisibleRule { get; set; }

        /// <summary>
        /// 佣金分配方式
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// 佣金分配方式
        /// </summary>
        public int ReferenceId { get; set; }

        /// <summary>
        /// 项目标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 项目属性
        /// </summary>
        public List<ProjectProperty> Properties { get; set; }

        /// <summary>
        /// 项目贡献者
        /// </summary>
        public List<ProjectContributor> Contributors { get; set; }

        /// <summary>
        /// 查看者
        /// </summary>
        public List<ProjectViewer> Viewers { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime CreateTime { get; set; }

        private Project CloneProject(Project source)
        {
            if (source == null)
            {
                source = this;
            }

            var newProject = new Project
            {
                Area = source.Area,
                AreaId = source.AreaId,
                Avatar = source.Avatar,
                BrokerageOptions = source.BrokerageOptions,
                City = source.City,
                CityId = source.CityId,
                Company = source.Company,
                Contributors = new List<ProjectContributor>(),
                CreateTime = DateTime.Now,
                FinMoney = source.FinMoney,
                FinPercentage = source.FinPercentage,
                FinStage = source.FinStage,
                FormatBPFile = source.FormatBPFile,
                Income = source.Income,
                Introduction = source.Introduction,
                OnPlatform = source.OnPlatform,
                OriginBPFile = source.OriginBPFile,
                Province = source.Province,
                ProvinceId = source.ProvinceId,
                VisibleRule = source.VisibleRule == null ? null : new ProjectVisibleRule
                {
                    Visible = source.VisibleRule.Visible,
                    Tags = source.VisibleRule.Tags
                },
                Tags = source.Tags,
                Valuation = source.Valuation,
                ShowSecurityInfo = source.ShowSecurityInfo,
                RegisterTime = source.RegisterTime,
                Revenue = source.Revenue,
                Viewers = new List<ProjectViewer>()
            };

            newProject.Properties = new List<ProjectProperty>();
            foreach (var item in source.Properties)
            {
                newProject.Properties.Add(new ProjectProperty
                {
                    Key = item.Key,
                    Value = item.Value,
                    Text = item.Text
                });
            }

            return newProject;
        }

        /// <summary>
        /// 参与者得到项目拷贝
        /// </summary>
        /// <param name="contributorId"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public Project ContributorFork(int contributorId, Project source = null)
        {
            if (source == null)
            {
                source = this;
            }

            var newProject = CloneProject(source);
            newProject.UserId = contributorId;
            newProject.SourceId = source.SourceId == 0 ? source.Id : source.SourceId;
            newProject.ReferenceId = source.ReferenceId == 0 ? source.Id : source.ReferenceId;
            newProject.UpdateTime = DateTime.Now;

            return newProject;
        }

        public void AddViewer(int userId, string userName, string avatar)
        {
            var viewer = new ProjectViewer
            {
                UserId = userId,
                UserName = userName,
                Avatar = avatar
            };

            if (Viewers.Any(v => v.UserId == userId))
                return;

            Viewers.Add(viewer);

            AddDomainEvent(new ProjectViewedEvent
            {
                Company = this.Company,
                Introduction = this.Introduction,
                Avatar = this.Avatar,
                Viewer = viewer
            });
        }

        public void AddContributor(ProjectContributor contributor)
        {
            if (Contributors.Any(v => v.UserId == contributor.UserId))
                return;

            Contributors.Add(contributor);

            AddDomainEvent(new ProjectJoinedEvent
            {
                Company = this.Company,
                Introduction = this.Introduction,
                Avatar = this.Avatar,
                Contributor = contributor
            });
        }
    }
}
