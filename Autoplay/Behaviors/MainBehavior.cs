﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIM.Autoplay.Behaviors.Positioning;
using AIM.Autoplay.Util.Objects;
using BehaviorSharp;
using BehaviorSharp.Components.Composites;
using LeagueSharp.Common;

namespace AIM.Autoplay.Behaviors
{
    internal class MainBehavior
    {
        internal static Behavior Root = new Behavior(new IndexSelector(
            () =>
            {
                var heroes = new Heroes();
                if (Heroes.Me.IsDead)
                {
                    return 0;
                }
                if (heroes.AllyHeroes.All(h => h.InFountain()))
                {
                    return 1;
                }
                if (Heroes.Me.HealthPercentage() < Modes.Base.Menu.Item("LowHealth").GetValue<Slider>().Value)
                {
                    return 2;
                }
                return 3;
            }, new Sequence(),  Sequences.LanePush, Sequences.CollectHealthPack, Sequences.StayWithinExpRange));
    }
}
