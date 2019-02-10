using ElfKingdom;

namespace MyBot
{
    class PortalCommands : StrategicCalculations
    {
        public int TotalPortals { get; set; } = 0;
        public int TurnsWithoutTrolls { get; set; } = 0;

        public override void DoTurn(Game game)
        {
            TurnsWithoutTrolls++;
            Elf[] enemyElves;
            Portal[] portals = game.GetMyPortals();
            TotalPortals += portals.Length;
            if (portals.Length >= 1)
            {
                if (TurnsWithoutTrolls >= IceTrollSummonRate)
                {
                    bool flag = false;
                    Creature[] enemyGiants = game.GetEnemyLavaGiants();
                    if (enemyGiants != null && !flag)
                    {
                        foreach (Creature giant in enemyGiants)
                        {
                            if (giant.Distance(game.GetMyCastle()) <= EnemyAggressiveLavaGiantRangeFromCastle)
                            {
                                Portal currentBest = FindNearest(giant, game);
                                if (currentBest.CanSummonIceTroll() && !flag)
                                {
                                    currentBest.SummonIceTroll();
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
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
                                if (elf.Distance(current) <= EnemyAggressiveElfRangeFromPortal)
                                {
                                    if (currentTarget == null)
                                    {
                                        currentTarget = elf;
                                        summoner = current;
                                    }
                                    else
                                    {
                                        if (currentTarget.Distance(game.GetMyCastle()) > elf.Distance(game.GetMyCastle()) || (
                                            currentTarget.Distance(game.GetMyCastle()) == elf.Distance(game.GetMyCastle()) &&
                                            elf.Distance(current) < currentTarget.Distance(current)))
                                        {
                                            currentTarget = elf;
                                            summoner = current;
                                        }
                                    }
                                }
                            }
                        }
                        if (summoner != null)
                        {
                            if (summoner.CanSummonIceTroll() && !flag)
                            {
                                summoner.SummonIceTroll();
                                flag = true;
                            }
                        }
                    }
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
                                if (currentBest.CanSummonIceTroll() && !flag)
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
                    if (currentBest.CanSummonLavaGiant())
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
                    if (currentBest.CanSummonLavaGiant())
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
            Portal currentBest = portals[0];
            foreach (Portal current in portals)
            {
                if (current.Distance(gameObject) < currentBest.Distance(gameObject))
                {
                    currentBest = current;
                }
            }
            return currentBest;
        }
    }
}
