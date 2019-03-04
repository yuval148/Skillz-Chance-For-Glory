using ElfKingdom;

namespace MyBot
{
    public enum CreatureType { LavaGiant, IceTroll, Tornado }

    class PortalCommands : StrategicCalculations
    {
        public int TotalPortals { get; set; } = 0;
        public int TurnsWithoutTrolls { get; set; } = 0;
        private bool MazganBot = false;

        public override void DoTurn(Game game)
        {
            TurnsWithoutTrolls++;
            Elf[] enemyElves;
            Portal[] portals = game.GetMyPortals();
            TotalPortals += portals.Length;
            if (game.Turn == 1)
            {
                MazganBot = game.GetMyPortals().Length == 5 && game.GetEnemyPortals().Length == 5;
            }
            if (portals.Length >= 1)
            {
                //Specific bot strategies
                if (game.GetAllMyElves().Length == 0 && game.Turn < TheLongestDay)
                {
                    //HeProteccButAlsoAttacc...
                    foreach (Portal portal in portals)
                    {
                        if (portal.CanSummonTornado())
                        {
                            portal.SummonTornado();
                        }
                    }
                }
                if (MazganBot)
                {
                    //Mazgan bot...
                    if (game.Turn == 1)
                    {
                        foreach (Portal portal in portals)
                        {
                            if (portal.CanSummonTornado())
                            {
                                portal.SummonTornado();
                            }
                        }
                    }
                    else if (game.Turn < TheLongestDay)
                    {
                        foreach (Portal portal in portals)
                        {
                            if (portal.CanSummonIceTroll())
                            {
                                portal.SummonIceTroll();
                            }
                        }
                    }
                }
                if (TurnsWithoutTrolls >= IceTrollSummonRate)
                {
                    bool flag = false;

                    //Defend against enemy elves
                    enemyElves = game.GetAllEnemyElves();
                    DefendAgainst(enemyElves, EnemyAggressiveElfRangeFromPortal, CreatureType.IceTroll, portals, ref flag, game);

                    //Defend against tornadoes
                    DefendAgainst(game.GetEnemyTornadoes(), EnemyAggressiveTornadoRangeFromPortal, CreatureType.IceTroll, portals, ref flag, game);

                    //Defend against portals
                    DefendAgainst(game.GetEnemyPortals(), EnemyAggressiveElfRangeFromPortal, CreatureType.Tornado, portals, ref flag, game);

                    //Defend against lava giants
                    Creature[] enemyGiants = game.GetEnemyLavaGiants();
                    if (enemyGiants != null && !flag)
                    {
                        foreach (Creature giant in enemyGiants)
                        {
                            if (giant.Distance(game.GetMyCastle()) <= EnemyAggressiveLavaGiantRangeFromCastle)
                            {
                                Portal currentBest = FindNearest(giant, game);
                                if (currentBest != null && !flag && currentBest.CanSummonIceTroll())
                                {
                                    currentBest.SummonIceTroll();
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }

                    //Defend against enemy elves (close to castle)
                    if (enemyElves != null)
                    {
                        foreach (Elf elf in enemyElves)
                        {
                            if (elf.Location == null)
                            {
                                continue;
                            }
                            if (elf.Distance(game.GetMyCastle()) <= EnemyAggressiveElfRangeFromCastle)
                            {
                                Portal currentBest = FindNearest(game.GetMyCastle(), game);
                                if (currentBest != null && currentBest.CanSummonIceTroll() && !flag)
                                {
                                    currentBest.SummonIceTroll();
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        TurnsWithoutTrolls = 0;
                    }
                }
                if (game.Turn % 40 == 1 && game.Turn > 1)
                {
                    Portal currentBest = FindNearest(game.GetEnemyCastle(), game);
                    if (currentBest != null && currentBest.CanSummonLavaGiant())
                    {
                        currentBest.SummonLavaGiant();
                    }
                    else if (portals[0].CanSummonLavaGiant())
                    {
                        portals[0].SummonLavaGiant();
                    }
                }
                game.Debug("Average portals: " + (float)TotalPortals / game.Turn);
                if ((game.Turn >= TheLongestDay && (TotalPortals / game.Turn <= portals.Length)) || (game.GetMyCastle().CurrentHealth < 40 && game.GetMyMana() > 50))
                {
                    Portal currentBest = FindNearest(game.GetEnemyCastle(), game);
                    if (currentBest != null && currentBest.CanSummonLavaGiant())
                    {
                        currentBest.SummonLavaGiant();
                    }
                    foreach (Portal portal in game.GetMyPortals())
                    {
                        if (portal.CanSummonLavaGiant())
                        {
                            portal.SummonLavaGiant();
                        }
                    }
                }
            } // End of if (portals.Length >= 1)
        }
        public Portal FindNearest(GameObject gameObject, Game game)
        {
            Portal[] portals = game.GetMyPortals();
            Portal currentBest = null;
            foreach (Portal current in portals)
            {
                if (IsWorthIt(current, game))
                {
                    currentBest = current;
                    break;
                }
            }
            if (currentBest == null)
            {
                return null;
            }
            foreach (Portal current in portals)
            {
                if (!IsWorthIt(current, game))
                {
                    continue;
                }
                if (current.Distance(gameObject) < currentBest.Distance(gameObject))
                {
                    currentBest = current;
                }
            }
            return currentBest;
        }
        public bool IsWorthIt(Portal portal, Game game)
        {
            int health = portal.CurrentHealth;
            foreach (var elf in game.GetEnemyLivingElves())
            {
                if (elf.IsBuilding)
                {
                    continue;
                }
                if (elf.Distance(portal) <= elf.AttackRange + game.PortalSize + game.ElfMaxSpeed * (game.IceTrollSummoningDuration + 0))
                {
                    health -= game.ElfAttackMultiplier * (game.IceTrollSummoningDuration + 0 - (int)Max(0, (elf.Distance(portal) - elf.AttackRange - game.PortalSize) / game.ElfMaxSpeed));
                    game.Debug("Portal " + portal.Id + " is threatend by elf " + elf.Id + ". Health will be " + health + " (was " + portal.CurrentHealth + ").");
                }
            }
            return health > 0;
        }
        public void DefendAgainst(GameObject[] targets, float aggressiveRange, CreatureType defenderType, Portal[] portals, ref bool flag, Game game)
        {
            if (targets != null)
            {
                GameObject currentTarget = null;
                Portal summoner = null;
                foreach (GameObject target in targets)
                {
                    if (target.Location == null)
                    {
                        continue;
                    }
                    foreach (Portal current in portals)
                    {
                        if (!IsWorthIt(current, game))
                        {
                            continue;
                        }
                        if (target.Distance(current) <= aggressiveRange)
                        {
                            if (currentTarget == null)
                            {
                                currentTarget = target;
                                summoner = current;
                            }
                            else
                            {
                                if (currentTarget.Distance(game.GetMyCastle()) > target.Distance(game.GetMyCastle()))
                                {
                                    currentTarget = target;
                                    summoner = current;
                                }
                            }
                        }
                    }
                }
                if (currentTarget != null)
                {
                    foreach (Portal current in portals)
                    {
                        if (!IsWorthIt(current, game))
                        {
                            continue;
                        }
                        if (current.Distance(currentTarget) < summoner.Distance(currentTarget))
                        {
                            summoner = current;
                        }
                    }
                    if (summoner != null && !flag)
                    {
                        switch (defenderType)
                        {
                            case CreatureType.LavaGiant:
                                game.Debug("This just... doesn't make any sense.");
                                if (summoner.CanSummonLavaGiant())
                                {
                                    summoner.SummonLavaGiant();
                                    flag = true;
                                }
                                break;
                            case CreatureType.IceTroll:
                                if (summoner.CanSummonIceTroll())
                                {
                                    summoner.SummonIceTroll();
                                    flag = true;
                                }
                                break;
                            case CreatureType.Tornado:
                                if (summoner.CanSummonTornado())
                                {
                                    summoner.SummonTornado();
                                    flag = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
