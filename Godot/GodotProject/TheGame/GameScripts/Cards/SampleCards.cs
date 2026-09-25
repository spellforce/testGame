using GameLogic.Design;

namespace GameLogic.Cards
{
    /// <summary>
    /// 示例卡牌工厂——在正式配置表（Luban）接入之前，用代码构造一批可运行的卡，
    /// 覆盖全部 8 个家族与主角，用于验证棋盘、堆叠、家族辨识与拖拽。
    ///
    /// 这些卡没有玩法字段，只有视图需要的：家族、短名、全名、徽章、子暗示。
    /// 接入 <c>TbEntityConfig</c> / 卡牌表后本类应被替换，而不是继续扩充。
    /// </summary>
    public static class SampleCards
    {
        /// <summary>8 家族 + 主角，各一张，顺序与 <see cref="CardFamilies.All"/> 一致。</summary>
        public static CardData[] CreateAll()
        {
            var list = new System.Collections.Generic.List<CardData>
            {
                Create(CardFamily.Vehicle, "scout_buggy", "越野车"),
                Create(CardFamily.Enemy, "raider_truck", "掠夺者"),
                Create(CardFamily.Site, "ruined_depot", "废弃站"),
                Create(CardFamily.Event, "sandstorm", "沙暴"),
                Create(CardFamily.Facility, "garage", "车库"),
                Create(CardFamily.Part, "main_cannon", "主炮"),
                Create(CardFamily.Region, "dried_seabed", "干涸海床"),
                Create(CardFamily.Cargo, "scrap_plate", "废钢板"),
                Create(CardFamily.Protagonist, "hero", "主角"),
            };
            return list.ToArray();
        }

        /// <summary>按家族构造一张示例卡，带该家族的默认徽章与子暗示。</summary>
        public static CardData Create(CardFamily family, string id, string title)
        {
            var data = new CardData
            {
                Id = id,
                Family = family,
                Title = title,
                FullName = title,
                Description = $"{CardFamilies.Get(family).Token} · {id}",
            };

            switch (family)
            {
                case CardFamily.Vehicle:
                    // 11-card-taxonomy.md 徽章默认值：左 = SP 百分比，右 = 货舱占用
                    data.BadgeLeft = 78;
                    data.BadgeRight = 12;
                    break;

                case CardFamily.Enemy:
                    // 左 = 等级，右 = HP
                    data.BadgeLeft = 3;
                    data.BadgeRight = 45;
                    break;

                case CardFamily.Site:
                    // 左 = 剩余搜索次数
                    data.BadgeLeft = 2;
                    break;

                case CardFamily.Event:
                    // 右 = 倒计时秒数
                    data.BadgeRight = 30;
                    break;

                case CardFamily.Part:
                    // 左 = 重量
                    data.BadgeLeft = 8;
                    data.PartKind = PartKind.MainCannon;
                    break;

                case CardFamily.Cargo:
                    // 左 = 堆叠数，右 = 单件价值
                    data.BadgeLeft = 3;
                    data.BadgeRight = 8;
                    break;

                case CardFamily.Region:
                    // 左 = 时限（周期数）
                    data.BadgeLeft = 6;
                    break;

                case CardFamily.Protagonist:
                    // 左 = 驾驶等级
                    data.BadgeLeft = 4;
                    break;
            }

            return data;
        }
    }
}
