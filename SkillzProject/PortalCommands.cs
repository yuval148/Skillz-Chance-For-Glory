using ElfKingdom;

namespace MyBot
{
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
                    //Technically, we should just add DefendAgainstPortals using Tornadoes, but I'll do it later.
                    //Right now, I just want to beat this bot.
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
                    if (enemyElves != null)
                    {
                        Elf currentTarget = null;
                        Portal summoner = null;
                        foreach (Elf elf in enemyElves)
                        {
                            if (elf.Location == null)
                            {
                                continue;
                            }
                            foreach (Portal current in portals)
                            {
                                if (!IsWorthIt(current, game))
                                {
                                    continue;
                                }
                                if (elf.Distance(current) <= EnemyAggressiveElfRangeFromPortal)
                                {
                                    if (currentTarget == null)
                                    {
                                        currentTarget = elf;
                                        summoner = current;
                                    }
                                    else
                                    {
                                        if (currentTarget.Distance(game.GetMyCastle()) > elf.Distance(game.GetMyCastle()))
                                        {
                                            currentTarget = elf;
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
                            if (summoner != null && summoner.CanSummonIceTroll() && !flag)
                            {
                                summoner.SummonIceTroll();
                                flag = true;
                            }
                        }
                    }

                    //Defend against tornadoes
                    Tornado[] enemyTornadoes = game.GetEnemyTornadoes();
                    if (enemyTornadoes != null)
                    {
                        Tornado currentTarget = null;
                        Portal summoner = null;
                        foreach (Tornado tornado in enemyTornadoes)
                        {
                            if (tornado.Location == null)
                            {
                                continue;
                            }
                            foreach (Portal current in portals)
                            {
                                if (!IsWorthIt(current, game))
                                {
                                    continue;
                                }
                                if (tornado.Distance(current) <= EnemyAggressiveTornadoRangeFromPortal)
                                {
                                    if (currentTarget == null)
                                    {
                                        currentTarget = tornado;
                                        summoner = current;
                                    }
                                    else
                                    {
                                        if (currentTarget.Distance(game.GetMyCastle()) > tornado.Distance(game.GetMyCastle()))
                                        {
                                            currentTarget = tornado;
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
                            if (summoner != null && summoner.CanSummonIceTroll() && !flag)
                            {
                                summoner.SummonIceTroll();
                                flag = true;
                            }
                        }
                    }

                    //Defend against portals
                    Portal[] enemyPortals = game.GetEnemyPortals();
                    if (enemyPortals != null)
                    {
                        Portal currentTarget = null;
                        Portal summoner = null;
                        foreach (Portal enemyPortal in enemyPortals)
                        {
                            if (enemyPortal.Location == null)
                            {
                                continue;
                            }
                            foreach (Portal current in portals)
                            {
                                if (!IsWorthIt(current, game))
                                {
                                    continue;
                                }
                                if (enemyPortal.Distance(current) <= EnemyAggressiveElfRangeFromPortal)
                                {
                                    if (currentTarget == null)
                                    {
                                        currentTarget = enemyPortal;
                                        summoner = current;
                                    }
                                    else
                                    {
                                        if (currentTarget.Distance(game.GetMyCastle()) > enemyPortal.Distance(game.GetMyCastle()))
                                        {
                                            currentTarget = enemyPortal;
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
                            if (summoner != null && summoner.CanSummonTornado() && !flag)
                            {
                                summoner.SummonTornado();
                                flag = true;
                            }
                        }
                    }

                    //Defend against lava giants
                    Creature[] enemyGiants = game.GetEnemyLavaGiants();
                    if (enemyGiants != null && !flag)
                    {
                        foreach (Creature giant in enemyGiants)
                        {
                            if (giant.Distance(game.GetMyCastle()) <= EnemyAggressiveLavaGiantRangeFromCastle)
                            {
                                Portal currentBest = FindNearest(giant, game);
                                if (currentBest != null && currentBest.CanSummonIceTroll() && !flag)
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
    }
}
