﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIM.Autoplay.Util;
using AIM.Autoplay.Util.Data;
using AIM.Autoplay.Util.Objects;
using BehaviorSharp;
using BehaviorSharp.Components.Actions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AIM.Autoplay.Behaviors.Positioning
{
    internal class Actions
    {
        internal BehaviorAction PushLane = new BehaviorAction(
            () =>
            {
                try
                {
                    var objConstants = new Constants();
                    var isInDanger = ObjectManager.Player.UnderTurret(true) && Modes.Base.InDangerUnderEnemyTurret();
                    if (isInDanger)
                    {
                        var orbwalkingPos = new Vector2();
                        orbwalkingPos.X = ObjectManager.Player.Position.X + objConstants.DefensiveAdditioner;
                        orbwalkingPos.Y = ObjectManager.Player.Position.Y + objConstants.DefensiveAdditioner;
                        ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, orbwalkingPos.To3D());
                        return BehaviorState.Success;
                    }
                    if (Modes.Base.LeadingMinion != null)
                    {
                        var orbwalkingPos = new Vector2();
                        orbwalkingPos.X = Modes.Base.LeadingMinion.Position.X + objConstants.DefensiveAdditioner;
                        orbwalkingPos.Y = Modes.Base.LeadingMinion.Position.Y + objConstants.DefensiveAdditioner;
                        Modes.Base.OrbW.ExecuteMixedMode(orbwalkingPos.To3D());
                        return BehaviorState.Success;
                    }
                    return BehaviorState.Failure;
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e);
                }
                return BehaviorState.Failure;
            });

        internal BehaviorAction StayWithinExpRange = new BehaviorAction(
            () =>
            {
                var objConstants = new Constants();
                var isInDanger = ObjectManager.Player.UnderTurret(true) && Modes.Base.InDangerUnderEnemyTurret();
                if (isInDanger)
                {
                    var orbwalkingPos = new Vector2();
                    orbwalkingPos.X = ObjectManager.Player.Position.X + objConstants.DefensiveAdditioner;
                    orbwalkingPos.Y = ObjectManager.Player.Position.Y + objConstants.DefensiveAdditioner;
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, orbwalkingPos.To3D());
                    return BehaviorState.Success;
                }
                if (Modes.Base.ClosestEnemyMinion != null)
                {
                    //this is a temp workaround untill the planned smart positioning
                    var orbwalkingPos = new Vector2();
                    if (Heroes.Me.Distance(Modes.Base.ClosestEnemyMinion) > 1300)
                    {
                        orbwalkingPos.X = Modes.Base.ClosestEnemyMinion.Position.X + Randoms.Rand.Next(300,900) * objConstants.AggressiveMultiplier;
                        orbwalkingPos.Y = Modes.Base.ClosestEnemyMinion.Position.Y + Randoms.Rand.Next(300, 900) * objConstants.AggressiveMultiplier;
                        Heroes.Me.IssueOrder(GameObjectOrder.MoveTo, orbwalkingPos.To3D());
                        return BehaviorState.Running;
                    }
                    if (Heroes.Me.Distance(Modes.Base.ClosestEnemyMinion) < 750)
                    {
                        orbwalkingPos.X = Modes.Base.ClosestEnemyMinion.Position.X + Randoms.Rand.Next(300, 900) * objConstants.DefensiveMultiplier;
                        orbwalkingPos.Y = Modes.Base.ClosestEnemyMinion.Position.Y + Randoms.Rand.Next(300, 900) * objConstants.DefensiveMultiplier;
                        Heroes.Me.IssueOrder(GameObjectOrder.MoveTo, orbwalkingPos.To3D());
                        return BehaviorState.Running;
                    }
                    if (Heroes.Me.Distance(Modes.Base.ClosestEnemyMinion) > 750 &&
                        Heroes.Me.Distance(Modes.Base.ClosestEnemyMinion) < 1300)
                    {
                        Modes.Base.OrbW.ExecuteMixedMode(Heroes.Me.Position);
                        return BehaviorState.Success;
                    }
                }
                return BehaviorState.Failure;
            });

        internal BehaviorAction KillEnemy = new BehaviorAction(
            () =>
            {
                var spells = new List<SpellSlot> { SpellSlot.Q, SpellSlot.W, SpellSlot.E };
                var heroes = new Heroes();
                var killableEnemy = heroes.EnemyHeroes.FirstOrDefault(h => h.Health < Heroes.Me.GetComboDamage(h, spells) + Heroes.Me.GetAutoAttackDamage(Heroes.Me));
                if (killableEnemy == null || killableEnemy.IsDead || !killableEnemy.IsValidTarget() || killableEnemy.IsInvulnerable ||
                                              killableEnemy.UnderTurret(true) || Heroes.Me.IsDead)
                {
                    Orbwalker.Stop = false;
                    return BehaviorState.Success;
                }
                Orbwalker.Stop = true;
                Heroes.Me.IssueOrder(GameObjectOrder.AutoAttack, killableEnemy);
                return BehaviorState.Running;
            });
        internal BehaviorAction CollectHealthRelic = new BehaviorAction(
            () =>
            {
                if (Heroes.Me.Position != Relics.ClosestRelic().Position)
                {
                    Orbwalker.Stop = true;
                    Heroes.Me.IssueOrder(GameObjectOrder.MoveTo, Relics.ClosestRelic().Position);
                    return BehaviorState.Running;
                }
                Orbwalker.Stop = false;
                return BehaviorState.Success;
            });
    }
}
